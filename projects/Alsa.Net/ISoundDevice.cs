using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Alsa.Net
{
    /// <summary>
    /// The communications channel to a sound device.
    /// </summary>
    public interface ISoundDevice : IDisposable
    {        
        /// <summary>
        /// The connection settings of the sound device.
        /// </summary>
        SoundConnectionSettings Settings { get; }

        /// <summary>
        /// The playback volume of the sound device.
        /// </summary>
        long PlaybackVolume { get; set; }

        /// <summary>
        /// The playback mute of the sound device.
        /// </summary>
        bool PlaybackMute { get; set; }

        /// <summary>
        /// The recording volume of the sound device.
        /// </summary>
        long RecordingVolume { get; set; }

        /// <summary>
        /// The recording mute of the sound device.
        /// </summary>
        bool RecordingMute { get; set; }

        /// <summary>
        /// Play WAV file.
        /// </summary>
        /// <param name="wavPath">WAV file path.</param>
        void Play(string wavPath);

        /// <summary>
        /// Play WAV file.
        /// </summary>
        /// <param name="wavStream">WAV stream.</param>
        void Play(Stream wavStream);

        /// <summary>
        /// Sound recording.
        /// </summary>
        /// <param name="second">Recording duration(In seconds).</param>
        /// <param name="savePath">Recording save path.</param>
        void Record(uint second, string savePath);
        void Record(Stream outputStream, CancellationToken cancellationToken);

        /// <summary>
        /// Sound recording.
        /// </summary>
        /// <param name="second">Recording duration(In seconds).</param>
        /// <param name="saveStream">Recording save stream.</param>
        void Record(uint second, Stream saveStream);
    }
}
