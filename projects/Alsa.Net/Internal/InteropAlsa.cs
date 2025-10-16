using System.Runtime.InteropServices;

namespace Alsa.Net.Internal;

static class InteropAlsa
{
    const string AlsaLibrary = "libasound";

    const CallingConvention CConvention = CallingConvention.Cdecl;
    const CharSet CSet = CharSet.Ansi;

    [DllImport(AlsaLibrary, CallingConvention = CConvention, CharSet = CSet)]
    public static extern IntPtr snd_strerror(int errnum); // returns const char*

    [DllImport(AlsaLibrary, CallingConvention = CConvention, CharSet = CSet)]
    public static extern int snd_pcm_open(ref IntPtr pcm, string name, snd_pcm_stream_t stream, int mode);

    [DllImport(AlsaLibrary, CallingConvention = CConvention)]
    public static extern int snd_pcm_start(IntPtr pcm);

    [DllImport(AlsaLibrary, CallingConvention = CConvention)]
    public static extern int snd_pcm_pause(IntPtr pcm, int enable);

    [DllImport(AlsaLibrary, CallingConvention = CConvention)]
    public static extern int snd_pcm_resume(IntPtr pcm);

    [DllImport(AlsaLibrary, CallingConvention = CConvention)]
    public static extern int snd_pcm_drain(IntPtr pcm);

    [DllImport(AlsaLibrary, CallingConvention = CConvention)]
    public static extern int snd_pcm_drop(IntPtr pcm);

    [DllImport(AlsaLibrary, CallingConvention = CConvention)]
    public static extern int snd_pcm_close(IntPtr pcm);

    [DllImport(AlsaLibrary, CallingConvention = CConvention)]
    public static extern int snd_pcm_recover(IntPtr pcm, int err, int silent);
    
    [DllImport(AlsaLibrary, CallingConvention = CConvention)]
    public static extern int snd_pcm_writei(IntPtr pcm, IntPtr buffer, nuint size);

    [DllImport(AlsaLibrary, CallingConvention = CConvention)]
    public static extern int snd_pcm_readi(IntPtr pcm, IntPtr buffer, nuint size);

    [DllImport(AlsaLibrary, CallingConvention = CConvention)]
    public static extern int snd_pcm_set_params(IntPtr pcm, snd_pcm_format_t format, snd_pcm_access_t access, uint channels, uint rate, int softResample, uint latency);

    [DllImport(AlsaLibrary, CallingConvention = CConvention)]
    public static extern int snd_pcm_hw_params_malloc(ref IntPtr @params);

    [DllImport(AlsaLibrary, CallingConvention = CConvention)]
    public static extern int snd_pcm_hw_params_any(IntPtr pcm, IntPtr @params);

    [DllImport(AlsaLibrary, CallingConvention = CConvention)]
    public static extern int snd_pcm_hw_params_set_access(IntPtr pcm, IntPtr @params, snd_pcm_access_t access);

    [DllImport(AlsaLibrary, CallingConvention = CConvention)]
    public static extern int snd_pcm_hw_params_set_format(IntPtr pcm, IntPtr @params, snd_pcm_format_t val);

    [DllImport(AlsaLibrary, CallingConvention = CConvention)]
    public static extern int snd_pcm_hw_params_set_channels(IntPtr pcm, IntPtr @params, uint val);

    [DllImport(AlsaLibrary, CallingConvention = CConvention)]
    public static extern unsafe int snd_pcm_hw_params_set_rate_near(IntPtr pcm, IntPtr @params, uint* val, int* dir);

    [DllImport(AlsaLibrary, CallingConvention = CConvention)]
    public static extern int snd_pcm_hw_params(IntPtr pcm, IntPtr @params);
    
    [DllImport(AlsaLibrary, CallingConvention = CConvention)]
    public static extern unsafe int snd_pcm_hw_params_get_period_size(IntPtr @params, nuint* frames, int* dir);

    [DllImport(AlsaLibrary, CallingConvention = CConvention)]
    public static extern unsafe int snd_pcm_hw_params_set_period_size_near(IntPtr pcm, IntPtr @params, nuint* frames, int* dir);

    [DllImport(AlsaLibrary, CallingConvention = CConvention)]
    public static extern int snd_mixer_open(ref IntPtr mixer, int mode);

    [DllImport(AlsaLibrary, CallingConvention = CConvention)]
    public static extern int snd_mixer_close(IntPtr mixer);

    [DllImport(AlsaLibrary, CallingConvention = CConvention, CharSet = CSet)]
    public static extern int snd_mixer_attach(IntPtr mixer, string name);

    [DllImport(AlsaLibrary, CallingConvention = CConvention)]
    public static extern int snd_mixer_load(IntPtr mixer);

    [DllImport(AlsaLibrary, CallingConvention = CConvention)]
    public static extern int snd_mixer_selem_register(IntPtr mixer, IntPtr options, IntPtr classp);

    [DllImport(AlsaLibrary, CallingConvention = CConvention)]
    public static extern IntPtr snd_mixer_first_elem(IntPtr mixer);

    [DllImport(AlsaLibrary, CallingConvention = CConvention)]
    public static extern IntPtr snd_mixer_elem_next(IntPtr elem);
    
    [DllImport(AlsaLibrary, CallingConvention = CConvention, CharSet = CSet)]
    public static extern string snd_mixer_selem_get_name(IntPtr elem);

    [DllImport(AlsaLibrary, CallingConvention = CConvention)]
    public static extern int snd_mixer_selem_id_malloc(ref IntPtr selemId);

    [DllImport(AlsaLibrary, CallingConvention = CConvention)]
    public static extern void snd_mixer_selem_id_free(IntPtr selemId);

    [DllImport(AlsaLibrary, CallingConvention = CConvention)]
    public static extern unsafe int snd_mixer_selem_get_playback_volume(IntPtr elem, snd_mixer_selem_channel_id channel, nint* value);

    [DllImport(AlsaLibrary, CallingConvention = CConvention)]
    public static extern int snd_mixer_selem_set_playback_volume(IntPtr elem, snd_mixer_selem_channel_id channel, nint value);

    [DllImport(AlsaLibrary, CallingConvention = CConvention)]
    public static extern int snd_mixer_selem_set_playback_volume_all(IntPtr elem, nint value);

    [DllImport(AlsaLibrary, CallingConvention = CConvention)]
    public static extern int snd_mixer_selem_set_playback_switch_all(IntPtr elem, int value);

    [DllImport(AlsaLibrary, CallingConvention = CConvention)]
    public static extern unsafe int snd_mixer_selem_get_playback_volume_range(IntPtr elem, nint* min, nint* max);

    [DllImport(AlsaLibrary, CallingConvention = CConvention)]
    public static extern int snd_mixer_selem_set_playback_volume_range(IntPtr elem, nint min, nint max);

    [DllImport(AlsaLibrary, CallingConvention = CConvention)]
    public static extern unsafe int snd_mixer_selem_get_capture_volume(IntPtr elem, snd_mixer_selem_channel_id channel, nint* value);

    [DllImport(AlsaLibrary, CallingConvention = CConvention)]
    public static extern int snd_mixer_selem_set_capture_volume(IntPtr elem, snd_mixer_selem_channel_id channel, nint value);

    [DllImport(AlsaLibrary, CallingConvention = CConvention)]
    public static extern int snd_mixer_selem_set_capture_volume_all(IntPtr elem, nint value);

    [DllImport(AlsaLibrary, CallingConvention = CConvention)]
    public static extern int snd_mixer_selem_set_capture_switch_all(IntPtr elem, int value);

    [DllImport(AlsaLibrary, CallingConvention = CConvention)]
    public static extern unsafe int snd_mixer_selem_get_capture_volume_range(IntPtr elem, nint* min, nint* max);

    [DllImport(AlsaLibrary, CallingConvention = CConvention)]
    public static extern int snd_mixer_selem_set_capture_volume_range(IntPtr elem, nint min, nint max);

    public static string StrError(int errno) => Marshal.PtrToStringUTF8(snd_strerror(errno)) ?? $"errno {errno}";
}