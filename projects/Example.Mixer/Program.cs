using Alsa.Net;

namespace Example.Mixer;

class Program
{
    static void Main()
    {
        // create virtual interface to system default audio device
        using var alsaDevice = AlsaDeviceBuilder.Create(new SoundDeviceSettings());

        // set some mixer values
        alsaDevice.RecordingVolume = 100;
        alsaDevice.PlaybackVolume = 45;

        // record 10s of audio
        alsaDevice.Record(10, "output.wav");

        // play recording back to device
        alsaDevice.Play("output.wav");
    }
}