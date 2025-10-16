namespace Alsa.Net;

/// <summary>
/// virtual sound interface connected to configured hardware devices
/// </summary>
public interface ISoundDevice : IDisposable
{
    /// <summary>
    /// sound device settings like playback-, mixer- or recording device
    /// </summary>
    SoundDeviceSettings Settings { get; }

    /// <summary>
    /// set or get the volume of the playback device.
    /// </summary>
    /// <remarks>ensure this is supported by your device</remarks>
    long PlaybackVolume { get; set; }

    /// <summary>
    /// mute / unmute the playback device or get the current state
    /// </summary>
    bool PlaybackMute { get; set; }

    /// <summary>
    /// set or get the current recording volume
    /// </summary>
    /// <remarks>ensure this is supported by your device</remarks>
    long RecordingVolume { get; set; }

    /// <summary>
    /// mute / unmute the recording device or get the current state
    /// </summary>
    bool RecordingMute { get; set; }

    /// <summary>
    /// play a wav file on the playback device
    /// </summary>
    /// <param name="wavPath">path to wav file</param>
    void Play(string wavPath);

    /// <summary>
    /// play a wav stream
    /// </summary>
    /// <param name="wavStream">stream of wav data to play</param>
    void Play(Stream wavStream);

    /// <summary>
    /// play a wav file on the playback device until end of file or cancellation
    /// </summary>
    /// <param name="wavPath">path to wav file</param>
    /// <param name="cancellationToken">token to stop playback</param>
    void Play(string wavPath, CancellationToken cancellationToken);

    /// <summary>
    /// play a wav stream until end of stream oder cancellation
    /// </summary>
    /// <param name="wavStream">stream of wav data to play</param>
    /// /// <param name="cancellationToken">token to stop playback</param>
    void Play(Stream wavStream, CancellationToken cancellationToken);

    /// <summary>
    /// record a wav file to the given path and length 
    /// </summary>
    /// <param name="second">recording duration in seconds</param>
    /// <param name="savePath">path to store the file to</param>
    void Record(uint second, string savePath);

    /// <summary>
    /// record wave data to the given stream until cancellation
    /// </summary>
    /// <param name="outputStream">stream to write wav data to</param>
    /// <param name="cancellationToken">token to cancel the recording</param>
    void Record(Stream outputStream, CancellationToken cancellationToken);

    /// <summary>
    /// record wave data to the given stream with the given duration
    /// </summary>
    /// <param name="second">recording duration in seconds</param>
    /// <param name="saveStream">stream to write the wav data to</param>
    void Record(uint second, Stream saveStream);

    /// <summary>
    /// record wave data using the given callback mehtod every time new audio data is available
    /// </summary>
    /// <param name="onDataAvailable">callback action called if new audio data is available</param>
    /// <param name="cancellationToken">token to cancel recording</param>
    void Record(Action<byte[]> onDataAvailable, CancellationToken cancellationToken);
}