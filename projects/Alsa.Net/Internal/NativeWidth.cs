namespace Alsa.Net.Internal;

static class NativeWidth
{
    public static unsafe nint ToNint(long value)
    {
        if (sizeof(nint) == 4 && value is > int.MaxValue or < int.MinValue)
            throw new OverflowException($"value of {value} does not fit in nint");

        return (nint)value;
    }

    public static long FromNint(nint value) => value;
}