using Alsa.Net;

namespace Example.Recording;

class Program
{
    static void Main()
    {
        // create virtual interface to system default audio device
        using var alsaDevice = AlsaDeviceBuilder.Create(new SoundDeviceSettings());

        // create stream to hold recorded data - will be pcm data including wav header
        using var outputStream = new MemoryStream();

        // create a cancellation token to stop recording after 10s
        using var tokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(10));

        // actually record
        alsaDevice.Record(outputStream, tokenSource.Token);

        // alternative: record for 10s directly to file
        //alsaDevice.Record(10, "output.wav");
    }
}