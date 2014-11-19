using System;
using System.Runtime.InteropServices;

namespace Lin.Util
{
    public static class ByteUtils
    {

        public static void WriteByte(byte[] dest, byte value, int offset = 0)
        {
            dest[offset] = value;
        }

        public static byte ReadByte(byte[] dest, int offset = 0)
        {
            return dest[offset];
        }

        public static void WriteShort(byte[] dest, short value, int offset = 0)
        {
            dest[offset] = (byte)((value & 0xFF00) >> 8);
            dest[offset + 1] = (byte)(value & 0x00FF);
        }

        public static short ReadShort(byte[] dest,int offset = 0)
        {
            return (short)(dest[offset] * 0x100 + dest[offset + 1]);
        }

        public static void WriteInt(byte[] dest, int value, int offset = 0)
        {
            dest[offset] = (byte)((value & 0xff000000) >> 24);
            dest[offset + 1] = (byte)((value & 0x00ff0000) >> 16);
            dest[offset + 2] = (byte)((value & 0x0000ff00) >> 8);
            dest[offset + 3] = (byte)(value & 0x000000ff);
        }

        public static int ReadInt(byte[] dest, int offset = 0)
        {

            return (dest[offset] * 0x1000000
                 + dest[offset + 1] * 0x10000
                 + dest[offset + 2] * 0x100
                 + dest[offset + 3]);
        }


        public static void WriteLong(byte[] dest, long value, int offset = 0)
        {//offset偏移量
            dest[offset] = (byte)(((ulong)value & 0xFF00000000000000) >> 56);
            dest[offset + 1] = (byte)((value & 0x00FF000000000000) >> 48);
            dest[offset + 2] = (byte)((value & 0xFF0000000000) >> 40);
            dest[offset + 3] = (byte)((value & 0x00FF00000000) >> 32);
            dest[offset + 4] = (byte)((value & 0xFF000000) >> 24);
            dest[offset + 5] = (byte)((value & 0xFF0000) >> 16);
            dest[offset + 6] = (byte)((value & 0xFF00) >> 8);
            dest[offset + 7] = (byte)(value & 0x00FF);
        }

        public static long ReadLong(byte[] dest,  int offset = 0)
        {//offset偏移量
            return (dest[offset] * 0x100000000000000
              + dest[offset + 1] * 0x1000000000000
              + dest[offset + 2] * 0x10000000000
              + dest[offset + 3] * 0x100000000
              + dest[offset + 4] * 0x1000000
              + dest[offset + 5] * 0x10000
              + dest[offset + 6] * 0x100
              + dest[offset + 7]);
        }

        public static void WriteBytes(byte[] dest, byte[] value, int lenth, int destoff=0, int sourceoff = 0)
        {
            for (uint n = 0; n < lenth; n++)
            {
                dest[destoff + n] = value[sourceoff + n];
            }
        }

        public static void ReadBytes(byte[] dest, byte[] value, int lenth, int destoff=0, int sourceoff = 0)
        {
            for (uint n = 0; n < lenth; n++)
            {
                value[sourceoff + n] = dest[destoff + n];
            }
        }

        public static unsafe void WriteFloat(byte[] dest, float value, int offset = 0)
        {
            byte* i = (byte*)&value;
            for (int n = 0; n < 4; n++)
            {
                dest[offset + n] = i[n];
            }
        }

        public static unsafe float ReadFloat(byte[] dest, int offset = 0)
        {
            float result = 0;
            byte* i = (byte*)&result;
            for (int n = 0; n < 4; n++)
            {
                i[n] = dest[offset + n];
            }
            return result;
        }


        public static unsafe void WriteDouble(byte[] dest, double value, int offset = 0)
        {
            byte* i = (byte*)&value;
            for (int n = 0; n < 8; n++)
            {
                dest[offset + n] = i[n];
            }
        }

        public static unsafe double ReadDouble(byte[] dest, int offset = 0)
        {
            double result = 0;
            byte* i = (byte*)&result;
            for (int n = 0; n < 8; n++)
            {
                i[n] = dest[offset + n];
            }
            return result;
        }
    }
}

