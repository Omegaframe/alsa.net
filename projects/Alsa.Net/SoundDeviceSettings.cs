namespace Alsa.Net
{
    public class SoundDeviceSettings
    {
        /// <summary>
        /// The name of the playback device to use. Default: "default"
        /// </summary>
        public string PlaybackDeviceName { get; set; } = "default";

        /// <summary>
        /// The name of the recording device to use. Default: "default"
        /// </summary>
        public string RecordingDeviceName { get; set; } = "default";

        /// <summary>
        /// The name of the mixer device to use. Default: "default"
        /// </summary>
        public string MixerDeviceName { get; set; } = "default";

        /// <summary>
        /// The sample rate to use for recording. Default: 8000
        /// </summary>
        public uint RecordingSampleRate { get; set; } = 8000;

        /// <summary>
        /// The number of chanels to use for recording. Default: 2
        /// </summary>
        public ushort RecordingChannels { get; set; } = 2;

        /// <summary>
        /// The number of bits per sample to use for recording. Default: 16
        /// </summary>
        public ushort RecordingBitsPerSample { get; set; } = 16;
    }
}
