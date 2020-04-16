using Unity.Collections;
using Unity.Networking.Transport;

namespace Sibz.Sentry
{
    public static class DataStreamWriterExtensions
    {
        public static void WriteNativeString32(this DataStreamWriter writer, NativeString32 str)
        {
            writer.WriteByte(str.buffer.byte0000);
            writer.WriteByte(str.buffer.byte0001);
            writer.WriteByte(str.buffer.byte0002);
            writer.WriteByte(str.buffer.byte0003);
            writer.WriteByte(str.buffer.byte0004);
            writer.WriteByte(str.buffer.byte0005);
            writer.WriteByte(str.buffer.byte0006);
            writer.WriteByte(str.buffer.byte0007);
            writer.WriteByte(str.buffer.byte0008);
            writer.WriteByte(str.buffer.byte0009);
            writer.WriteByte(str.buffer.byte0010);
            writer.WriteByte(str.buffer.byte0011);
            writer.WriteByte(str.buffer.byte0012);
            writer.WriteByte(str.buffer.byte0013);
            writer.WriteByte(str.buffer.byte0014.byte0000);
            writer.WriteByte(str.buffer.byte0014.byte0001);
            writer.WriteByte(str.buffer.byte0014.byte0002);
            writer.WriteByte(str.buffer.byte0014.byte0003);
            writer.WriteByte(str.buffer.byte0014.byte0004);
            writer.WriteByte(str.buffer.byte0014.byte0005);
            writer.WriteByte(str.buffer.byte0014.byte0006);
            writer.WriteByte(str.buffer.byte0014.byte0007);
            writer.WriteByte(str.buffer.byte0014.byte0008);
            writer.WriteByte(str.buffer.byte0014.byte0009);
            writer.WriteByte(str.buffer.byte0014.byte0010);
            writer.WriteByte(str.buffer.byte0014.byte0011);
            writer.WriteByte(str.buffer.byte0014.byte0012);
            writer.WriteByte(str.buffer.byte0014.byte0013);
            writer.WriteByte(str.buffer.byte0014.byte0014);
            writer.WriteByte(str.buffer.byte0014.byte0015);
        }

        public static NativeString32 ReadNativeString32(this DataStreamReader reader)
        {
            return new NativeString32
            {
                buffer =
                {
                    byte0000 = reader.ReadByte(),
                    byte0001 = reader.ReadByte(),
                    byte0002 = reader.ReadByte(),
                    byte0003 = reader.ReadByte(),
                    byte0004 = reader.ReadByte(),
                    byte0005 = reader.ReadByte(),
                    byte0006 = reader.ReadByte(),
                    byte0007 = reader.ReadByte(),
                    byte0008 = reader.ReadByte(),
                    byte0009 = reader.ReadByte(),
                    byte0010 = reader.ReadByte(),
                    byte0011 = reader.ReadByte(),
                    byte0012 = reader.ReadByte(),
                    byte0013 = reader.ReadByte(),
                    byte0014 =
                    {
                        byte0000 = reader.ReadByte(),
                        byte0001 = reader.ReadByte(),
                        byte0002 = reader.ReadByte(),
                        byte0003 = reader.ReadByte(),
                        byte0004 = reader.ReadByte(),
                        byte0005 = reader.ReadByte(),
                        byte0006 = reader.ReadByte(),
                        byte0007 = reader.ReadByte(),
                        byte0008 = reader.ReadByte(),
                        byte0009 = reader.ReadByte(),
                        byte0010 = reader.ReadByte(),
                        byte0011 = reader.ReadByte(),
                        byte0012 = reader.ReadByte(),
                        byte0013 = reader.ReadByte(),
                        byte0014 = reader.ReadByte(),
                        byte0015 = reader.ReadByte()
                    }
                }
            };
        }
    }
}