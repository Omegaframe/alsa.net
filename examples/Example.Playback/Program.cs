using Alsa.Net;

namespace Example.Playback;

class Program
{
    static void Main()
    {
        // create virtual interface to system default audio device
        using var alsaDevice = AlsaDeviceBuilder.Create(new SoundDeviceSettings());

        // provide a wav stream 
        using var inputStream = new FileStream("example.wav", FileMode.Open, FileAccess.Read, FileShare.Read);

        // play on the device
        alsaDevice.Play(inputStream);

        // alternative: directly play a wav file
        // alsaDevice.Play("example.wav");
    }
}