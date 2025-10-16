namespace Alsa.Net;

/// <summary>
/// settings on how to connect to and use your alsa devices
/// </summary>
public class SoundDeviceSettings
{
    /// <summary>
    /// name of the playback device to use. Default: "default"
    /// </summary>
    public string PlaybackDeviceName { get; set; } = "default";

    /// <summary>
    /// name of the recording device to use. Default: "default"
    /// </summary>
    public string RecordingDeviceName { get; set; } = "default";

    /// <summary>
    /// name of the mixer device to use. Default: "default"
    /// </summary>
    public string MixerDeviceName { get; set; } = "default";

    /// <summary>
    /// sample rate to use for recording. Default: 8000
    /// </summary>
    /// <remarks>check your device specification for supported rates</remarks>
    public uint RecordingSampleRate { get; set; } = 8000;

    /// <summary>
    /// number of chanels to use for recording. Default: 2
    /// </summary>
    /// <remarks>check your device specification for available numbers</remarks>
    public ushort RecordingChannels { get; set; } = 2;

    /// <summary>
    /// number of bits per sample to use for recording. Default: 16
    /// </summary>
    /// <remarks>check device specification for supported bit depths</remarks>
    public ushort RecordingBitsPerSample { get; set; } = 16;
}