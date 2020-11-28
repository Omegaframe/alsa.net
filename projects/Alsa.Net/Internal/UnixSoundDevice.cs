using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;

namespace Alsa.Net.Internal
{
    /// <summary>
    /// Represents a communications channel to a sound device running on Unix.
    /// </summary>
    class UnixSoundDevice : ISoundDevice
    {
        static readonly object playbackInitializationLock = new object();
        static readonly object recordingInitializationLock = new object();
        static readonly object mixerInitializationLock = new object();

        public SoundConnectionSettings Settings { get; }
        public long PlaybackVolume { get => GetPlaybackVolume(); set => SetPlaybackVolume(value); }
        public bool PlaybackMute { get => _playbackMute; set => SetPlaybackMute(value); }
        public long RecordingVolume { get => GetRecordingVolume(); set => SetRecordingVolume(value); }
        public bool RecordingMute { get => _recordingMute; set => SetRecordingMute(value); }

        bool _playbackMute;
        bool _recordingMute;
        IntPtr _playbackPcm;
        IntPtr _recordingPcm;
        IntPtr _mixer;
        IntPtr _elem;
        int _errorNum;

        public UnixSoundDevice(SoundConnectionSettings settings)
        {
            Settings = settings;
            PlaybackMute = false;
            RecordingMute = false;
        }

        public void Play(string wavPath)
        {
            using FileStream fs = File.Open(wavPath, FileMode.Open, FileAccess.Read, FileShare.Read);
            Play(fs);
        }

        public void Play(Stream wavStream)
        {
            var @params = new IntPtr();
            var dir = 0;
            var header = WavHeader.FromStream(wavStream);

            OpenPlaybackPcm();
            PcmInitialize(_playbackPcm, header, ref @params, ref dir);
            WriteStream(wavStream, header, ref @params, ref dir);
            ClosePlaybackPcm();
        }

        public void Record(uint second, string savePath)
        {
            using FileStream fs = File.Open(savePath, FileMode.Create, FileAccess.Write, FileShare.None);
            Record(second, fs);
        }

        public void Record(uint second, Stream saveStream)
        {
            using var tokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(second));
            Record(saveStream, tokenSource.Token);
        }

        public void Record(Stream saveStream, CancellationToken token)
        {
            const uint second = 60; // 60s chunk size

            var @params = new IntPtr();
            var dir = 0;
            var header = WavHeader.Build(second, Settings.RecordingSampleRate, Settings.RecordingChannels, Settings.RecordingBitsPerSample);

            header.WriteToStream(saveStream);
            OpenRecordingPcm();
            PcmInitialize(_recordingPcm, header, ref @params, ref dir);
            ReadStream(saveStream, header, ref @params, ref dir, token);
            CloseRecordingPcm();
        }

        unsafe void WriteStream(Stream wavStream, WavHeader header, ref IntPtr @params, ref int dir)
        {
            ulong frames, bufferSize;

            fixed (int* dirP = &dir)
            {
                _errorNum = InteropAlsa.snd_pcm_hw_params_get_period_size(@params, &frames, dirP);
                ThrowErrorMessage("Can not get period size.");
            }

            bufferSize = frames * header.BlockAlign;
            // In Interop, the frames is defined as ulong. But actucally, the value of bufferSize won't be too big.
            byte[] readBuffer = new byte[(int)bufferSize];
            // Jump wav header.
            wavStream.Position = 44;

            fixed (byte* buffer = readBuffer)
            {
                while (wavStream.Read(readBuffer) != 0)
                {
                    _errorNum = InteropAlsa.snd_pcm_writei(_playbackPcm, (IntPtr)buffer, frames);
                    ThrowErrorMessage("Can not write data to the device.");
                }
            }
        }

        unsafe void ReadStream(Stream saveStream, WavHeader header, ref IntPtr @params, ref int dir, CancellationToken cancellationToken)
        {
            ulong frames, bufferSize;

            fixed (int* dirP = &dir)
            {
                _errorNum = InteropAlsa.snd_pcm_hw_params_get_period_size(@params, &frames, dirP);
                ThrowErrorMessage("Can not get period size.");
            }

            bufferSize = frames * header.BlockAlign;
            byte[] readBuffer = new byte[(int)bufferSize];
            saveStream.Position = 44;

            fixed (byte* buffer = readBuffer)
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    _errorNum = InteropAlsa.snd_pcm_readi(_recordingPcm, (IntPtr)buffer, frames);
                    ThrowErrorMessage("Can not read data from the device.");

                    saveStream.Write(readBuffer);
                }
            }
            saveStream.Flush();
        }

        unsafe void PcmInitialize(IntPtr pcm, WavHeader header, ref IntPtr @params, ref int dir)
        {
            _errorNum = InteropAlsa.snd_pcm_hw_params_malloc(ref @params);
            ThrowErrorMessage("Can not allocate parameters object.");

            _errorNum = InteropAlsa.snd_pcm_hw_params_any(pcm, @params);
            ThrowErrorMessage("Can not fill parameters object.");

            _errorNum = InteropAlsa.snd_pcm_hw_params_set_access(pcm, @params, snd_pcm_access_t.SND_PCM_ACCESS_RW_INTERLEAVED);
            ThrowErrorMessage("Can not set access mode.");

            _errorNum = (int)(header.BitsPerSample / 8) switch
            {
                1 => InteropAlsa.snd_pcm_hw_params_set_format(pcm, @params, snd_pcm_format_t.SND_PCM_FORMAT_U8),
                2 => InteropAlsa.snd_pcm_hw_params_set_format(pcm, @params, snd_pcm_format_t.SND_PCM_FORMAT_S16_LE),
                3 => InteropAlsa.snd_pcm_hw_params_set_format(pcm, @params, snd_pcm_format_t.SND_PCM_FORMAT_S24_LE),
                _ => throw new Exception("Bits per sample error. Please reset the value of RecordingBitsPerSample."),
            };
            ThrowErrorMessage("Can not set format.");

            _errorNum = InteropAlsa.snd_pcm_hw_params_set_channels(pcm, @params, header.NumChannels);
            ThrowErrorMessage("Can not set channel.");

            uint val = header.SampleRate;
            fixed (int* dirP = &dir)
            {
                _errorNum = InteropAlsa.snd_pcm_hw_params_set_rate_near(pcm, @params, &val, dirP);
                ThrowErrorMessage("Can not set rate.");
            }

            _errorNum = InteropAlsa.snd_pcm_hw_params(pcm, @params);
            ThrowErrorMessage("Can not set hardware parameters.");
        }

        unsafe void SetPlaybackVolume(long volume)
        {
            OpenMixer();

            // The snd_mixer_selem_set_playback_volume_all method in Raspberry Pi is invalid.
            // So here we adjust the volume by setting the left and right channels separately.
            _errorNum = InteropAlsa.snd_mixer_selem_set_playback_volume(_elem, snd_mixer_selem_channel_id.SND_MIXER_SCHN_FRONT_LEFT, volume);
            _errorNum = InteropAlsa.snd_mixer_selem_set_playback_volume(_elem, snd_mixer_selem_channel_id.SND_MIXER_SCHN_FRONT_RIGHT, volume);
            ThrowErrorMessage("Set playback volume error.");

            CloseMixer();
        }

        unsafe long GetPlaybackVolume()
        {
            long volumeLeft, volumeRight;

            OpenMixer();

            _errorNum = InteropAlsa.snd_mixer_selem_get_playback_volume(_elem, snd_mixer_selem_channel_id.SND_MIXER_SCHN_FRONT_LEFT, &volumeLeft);
            _errorNum = InteropAlsa.snd_mixer_selem_get_playback_volume(_elem, snd_mixer_selem_channel_id.SND_MIXER_SCHN_FRONT_RIGHT, &volumeRight);
            ThrowErrorMessage("Get playback volume error.");

            CloseMixer();

            return (volumeLeft + volumeRight) / 2;
        }

        unsafe void SetRecordingVolume(long volume)
        {
            OpenMixer();

            _errorNum = InteropAlsa.snd_mixer_selem_set_capture_volume(_elem, snd_mixer_selem_channel_id.SND_MIXER_SCHN_FRONT_LEFT, volume);
            _errorNum = InteropAlsa.snd_mixer_selem_set_capture_volume(_elem, snd_mixer_selem_channel_id.SND_MIXER_SCHN_FRONT_RIGHT, volume);
            ThrowErrorMessage("Set recording volume error.");

            CloseMixer();
        }

        unsafe long GetRecordingVolume()
        {
            long volumeLeft, volumeRight;

            OpenMixer();

            _errorNum = InteropAlsa.snd_mixer_selem_get_capture_volume(_elem, snd_mixer_selem_channel_id.SND_MIXER_SCHN_FRONT_LEFT, &volumeLeft);
            _errorNum = InteropAlsa.snd_mixer_selem_get_capture_volume(_elem, snd_mixer_selem_channel_id.SND_MIXER_SCHN_FRONT_RIGHT, &volumeRight);
            ThrowErrorMessage("Get recording volume error.");

            CloseMixer();

            return (volumeLeft + volumeRight) / 2;
        }

        void SetPlaybackMute(bool isMute)
        {
            _playbackMute = isMute;

            OpenMixer();

            _errorNum = InteropAlsa.snd_mixer_selem_set_playback_switch_all(_elem, isMute ? 0 : 1);
            ThrowErrorMessage("Set playback mute error.");

            CloseMixer();
        }

        void SetRecordingMute(bool isMute)
        {
            _recordingMute = isMute;

            OpenMixer();

            _errorNum = InteropAlsa.snd_mixer_selem_set_playback_switch_all(_elem, isMute ? 0 : 1);
            ThrowErrorMessage("Set recording mute error.");

            CloseMixer();
        }

        void OpenPlaybackPcm()
        {
            if (_playbackPcm != default)
            {
                return;
            }

            lock (playbackInitializationLock)
            {
                _errorNum = InteropAlsa.snd_pcm_open(ref _playbackPcm, Settings.PlaybackDeviceName, snd_pcm_stream_t.SND_PCM_STREAM_PLAYBACK, 0);
                ThrowErrorMessage("Can not open playback device.");
            }
        }

        void ClosePlaybackPcm()
        {
            if (_playbackPcm != default)
            {
                _errorNum = InteropAlsa.snd_pcm_drop(_playbackPcm);
                ThrowErrorMessage("Drop playback device error.");

                _errorNum = InteropAlsa.snd_pcm_close(_playbackPcm);
                ThrowErrorMessage("Close playback device error.");

                _playbackPcm = default;
            }
        }

        void OpenRecordingPcm()
        {
            if (_recordingPcm != default)
            {
                return;
            }

            lock (recordingInitializationLock)
            {
                _errorNum = InteropAlsa.snd_pcm_open(ref _recordingPcm, Settings.RecordingDeviceName, snd_pcm_stream_t.SND_PCM_STREAM_CAPTURE, 0);
                ThrowErrorMessage("Can not open recording device.");
            }
        }

        void CloseRecordingPcm()
        {
            if (_recordingPcm != default)
            {
                _errorNum = InteropAlsa.snd_pcm_drop(_recordingPcm);
                ThrowErrorMessage("Drop recording device error.");

                _errorNum = InteropAlsa.snd_pcm_close(_recordingPcm);
                ThrowErrorMessage("Close recording device error.");

                _recordingPcm = default;
            }
        }

        void OpenMixer()
        {
            if (_mixer != default)
            {
                return;
            }

            lock (mixerInitializationLock)
            {
                _errorNum = InteropAlsa.snd_mixer_open(ref _mixer, 0);
                ThrowErrorMessage("Can not open sound device mixer.");

                _errorNum = InteropAlsa.snd_mixer_attach(_mixer, Settings.MixerDeviceName);
                ThrowErrorMessage("Can not attach sound device mixer.");

                _errorNum = InteropAlsa.snd_mixer_selem_register(_mixer, IntPtr.Zero, IntPtr.Zero);
                ThrowErrorMessage("Can not register sound device mixer.");

                _errorNum = InteropAlsa.snd_mixer_load(_mixer);
                ThrowErrorMessage("Can not load sound device mixer.");

                _elem = InteropAlsa.snd_mixer_first_elem(_mixer);
            }
        }

        void CloseMixer()
        {
            if (_mixer != default)
            {
                _errorNum = InteropAlsa.snd_mixer_close(_mixer);
                ThrowErrorMessage("Close sound device mixer error.");

                _mixer = default;
                _elem = default;
            }
        }

        public void Dispose()
        {
            ClosePlaybackPcm();
            CloseRecordingPcm();
            CloseMixer();
        }

        void ThrowErrorMessage(string message)
        {
            if (_errorNum < 0)
            {
                int code = _errorNum;
                string errorMsg = Marshal.PtrToStringAnsi(InteropAlsa.snd_strerror(_errorNum));

                Dispose();
                throw new Exception($"{message}\nError {code}. {errorMsg}.");
            }
        }
    }
}
