using Alsa.Net;

namespace Example.Configuration;

class Program
{
    static void Main()
    {
        // create a config for your audio hardware setup
        var config = new SoundDeviceSettings
        {
            RecordingDeviceName = "hw:recording01", // alsa name of recording device. use arecord -L for a list of available devices. "default" for systems default
            PlaybackDeviceName = "hw:playback01", // alsa name of playback device. use aplay -L for a list of available devices. "default" for systems default
            MixerDeviceName = "hw:mixer01", // alsa name of mixer device. ensure your device actually supports this. some might not have volume channels etc.

            RecordingBitsPerSample = 16, // bit depth of recorded audio data. check your devices capabilities on values to set here. default is 16
            RecordingChannels = 2, // number of audio channels to record. default is 2
            RecordingSampleRate = 8000 // number of samples per second of the recorded audio data. check your device on values to set here. default is 8000 even though this sounds terrible
        };

        // create virtual interface to use your config
        using var alsaDevice = AlsaDeviceBuilder.Create(config);

        // do something with your device e.g. record and play something
        // alsaDevice.Record(10, "output.wav");
        // alsaDevice.Play("output.wav");
    }
}