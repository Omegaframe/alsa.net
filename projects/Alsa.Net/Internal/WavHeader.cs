using System.Buffers.Binary;
using System.Text;

namespace Alsa.Net.Internal;

struct WavHeader
{
    public char[] ChunkId { get; set; }

    public uint ChunkSize { get; set; }

    public char[] Format { get; set; }

    public char[] Subchunk1Id { get; set; }

    public uint Subchunk1Size { get; set; }

    public ushort AudioFormat { get; set; }

    public ushort NumChannels { get; set; }

    public uint SampleRate { get; set; }

    public uint ByteRate { get; set; }

    public ushort BlockAlign { get; set; }

    public ushort BitsPerSample { get; set; }

    public char[] Subchunk2Id { get; set; }

    public uint Subchunk2Size { get; set; }

    public void WriteToStream(Stream wavStream)
    {
        Span<byte> writeBuffer2 = stackalloc byte[2];
        Span<byte> writeBuffer4 = stackalloc byte[4];

        try
        {
            Encoding.ASCII.GetBytes(ChunkId, writeBuffer4);
            wavStream.Write(writeBuffer4);

            BinaryPrimitives.WriteUInt32LittleEndian(writeBuffer4, ChunkSize);
            wavStream.Write(writeBuffer4);

            Encoding.ASCII.GetBytes(Format, writeBuffer4);
            wavStream.Write(writeBuffer4);

            Encoding.ASCII.GetBytes(Subchunk1Id, writeBuffer4);
            wavStream.Write(writeBuffer4);

            BinaryPrimitives.WriteUInt32LittleEndian(writeBuffer4, Subchunk1Size);
            wavStream.Write(writeBuffer4);

            BinaryPrimitives.WriteUInt16LittleEndian(writeBuffer2, AudioFormat);
            wavStream.Write(writeBuffer2);

            BinaryPrimitives.WriteUInt16LittleEndian(writeBuffer2, NumChannels);
            wavStream.Write(writeBuffer2);

            BinaryPrimitives.WriteUInt32LittleEndian(writeBuffer4, SampleRate);
            wavStream.Write(writeBuffer4);

            BinaryPrimitives.WriteUInt32LittleEndian(writeBuffer4, ByteRate);
            wavStream.Write(writeBuffer4);

            BinaryPrimitives.WriteUInt16LittleEndian(writeBuffer2, BlockAlign);
            wavStream.Write(writeBuffer2);

            BinaryPrimitives.WriteUInt16LittleEndian(writeBuffer2, BitsPerSample);
            wavStream.Write(writeBuffer2);

            Encoding.ASCII.GetBytes(Subchunk2Id, writeBuffer4);
            wavStream.Write(writeBuffer4);

            BinaryPrimitives.WriteUInt32LittleEndian(writeBuffer4, Subchunk2Size);
            wavStream.Write(writeBuffer4);
        }
        catch (Exception ex)
        {
            throw new WavFormatException(ExceptionMessages.UnableToWriteWavHeader, ex);
        }
    }

    public static WavHeader Build(uint sampleRate, ushort channels, ushort bitsPerSample)
    {
        return new WavHeader
        {
            ChunkId = ['R', 'I', 'F', 'F'],
            ChunkSize = 0xFFFFFFFF,
            Format = ['W', 'A', 'V', 'E'],
            Subchunk1Id = ['f', 'm', 't', ' '],
            Subchunk1Size = 16,
            AudioFormat = 1,
            NumChannels = channels,
            SampleRate = sampleRate,
            ByteRate = sampleRate * bitsPerSample * channels / 8,
            BlockAlign = (ushort)(bitsPerSample * channels / 8),
            BitsPerSample = bitsPerSample,
            Subchunk2Id = ['d', 'a', 't', 'a'],
            Subchunk2Size = 0xFFFFFFFF
        };
    }

    public static WavHeader FromStream(Stream wavStream)
    {
        Span<byte> readBuffer2 = stackalloc byte[2];
        Span<byte> readBuffer4 = stackalloc byte[4];

        var header = new WavHeader();

        try
        {
            var read = wavStream.Read(readBuffer4);
            header.ChunkId = Encoding.ASCII.GetString(readBuffer4[..read]).ToCharArray();

            read = wavStream.Read(readBuffer4);
            header.ChunkSize = BinaryPrimitives.ReadUInt32LittleEndian(readBuffer4[..read]);

            read = wavStream.Read(readBuffer4);
            header.Format = Encoding.ASCII.GetString(readBuffer4[..read]).ToCharArray();

            read = wavStream.Read(readBuffer4);
            header.Subchunk1Id = Encoding.ASCII.GetString(readBuffer4[..read]).ToCharArray();

            read = wavStream.Read(readBuffer4);
            header.Subchunk1Size = BinaryPrimitives.ReadUInt32LittleEndian(readBuffer4[..read]);

            read = wavStream.Read(readBuffer2);
            header.AudioFormat = BinaryPrimitives.ReadUInt16LittleEndian(readBuffer2[..read]);

            read = wavStream.Read(readBuffer2);
            header.NumChannels = BinaryPrimitives.ReadUInt16LittleEndian(readBuffer2[..read]);

            read = wavStream.Read(readBuffer4);
            header.SampleRate = BinaryPrimitives.ReadUInt32LittleEndian(readBuffer4[..read]);

            read = wavStream.Read(readBuffer4);
            header.ByteRate = BinaryPrimitives.ReadUInt32LittleEndian(readBuffer4[..read]);

            read = wavStream.Read(readBuffer2);
            header.BlockAlign = BinaryPrimitives.ReadUInt16LittleEndian(readBuffer2[..read]);

            read = wavStream.Read(readBuffer2);
            header.BitsPerSample = BinaryPrimitives.ReadUInt16LittleEndian(readBuffer2[..read]);

            read = wavStream.Read(readBuffer4);
            header.Subchunk2Id = Encoding.ASCII.GetString(readBuffer4[..read]).ToCharArray();

            read = wavStream.Read(readBuffer4);
            header.Subchunk2Size = BinaryPrimitives.ReadUInt32LittleEndian(readBuffer4[..read]);
        }
        catch (Exception exception)
        {
            throw new WavFormatException(ExceptionMessages.UnableToReadWavHeader, exception);
        }

        return header;
    }
}