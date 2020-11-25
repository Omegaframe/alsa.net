using Alsa.Net.Internal;

namespace Alsa.Net
{
    public static class AlsaDeviceBuilder
    {
        /// <summary>
        /// Create a communications channel to a sound device running on Unix.
        /// </summary>
        /// <param name="settings">The connection settings of a sound device.</param>
        /// <returns>A communications channel to a sound device running on Unix.</returns>
        public static ISoundDevice Create(SoundConnectionSettings settings) => new UnixSoundDevice(settings);
    }
}
