using System;
using System.Net;
using System.IO;
using System.Text;

namespace Lin.Core.Web.Json
{
    internal class JavaScriptDeserializer
    {
        // Fields
        private JavaScriptObjectDeserializer deserializer;
        internal const string s_jsonBeta2Prefix = "{\"d\":";

        // Methods
        public JavaScriptDeserializer(Stream stream)
        {
            if (stream == null)
            {
                throw new ArgumentNullException("stream");
            }
            Stream stream2 = stream;
            if (!stream.CanSeek)
            {
                stream2 = new BufferedStreamReader(stream);
            }
            Encoding encoding = DetectEncoding(stream2.ReadByte(), stream2.ReadByte());
            stream2.Position = 0L;
            string input = new StreamReader(stream2, encoding, true).ReadToEnd();
            this.deserializer = new JavaScriptObjectDeserializer(input);
        }

        internal object DeserializeObject()
        {
            return this.deserializer.BasicDeserialize();
        }
        private readonly static UnicodeEncoding ValidatingBEUTF16 = new UnicodeEncoding(true, false, true);
        private readonly static UnicodeEncoding ValidatingUTF16 = new System.Text.UnicodeEncoding(false, false, true);
        private readonly static UTF8Encoding ValidatingUTF8 = new System.Text.UTF8Encoding(false, true);
        public static Encoding DetectEncoding(int b1, int b2)
        {
            if ((b1 != -1) && (b2 != -1))
            {
                if ((b1 == 0) && (b2 != 0))
                {
                    return ValidatingBEUTF16;
                }
                if ((b1 != 0) && (b2 == 0))
                {
                    return ValidatingUTF16;
                }
                if ((b1 == 0) && (b2 == 0))
                {
                    //throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new XmlException(SR2.GetString("JsonInvalidBytes")));
                }
            }
            return ValidatingUTF8;
        }

        // Nested Types
        //[FriendAccessAllowed]
        internal class BufferedStreamReader : Stream
        {
            // Fields
            private byte[] bomBuffer = new byte[2];
            private int bufferedBytesIndex;
            private Stream internalStream;

            // Methods
            //[FriendAccessAllowed]
            internal BufferedStreamReader(Stream stream)
            {
                this.internalStream = stream;
                stream.Read(this.bomBuffer, 0, 2);
            }

            public override void Flush()
            {
            }

            public override int Read(byte[] buffer, int offset, int count)
            {
                int num = 0;
                while ((this.bufferedBytesIndex < 2) && (count > 0))
                {
                    num++;
                    buffer[offset++] = this.bomBuffer[this.bufferedBytesIndex++];
                    count--;
                }
                if (count <= 0)
                {
                    return num;
                }
                return (this.internalStream.Read(buffer, offset, count) + num);
            }

            public override int ReadByte()
            {
                if (this.bufferedBytesIndex >= 2)
                {
                    return this.internalStream.ReadByte();
                }
                return this.bomBuffer[this.bufferedBytesIndex++];
            }

            public override long Seek(long offset, SeekOrigin origin)
            {
                throw new NotSupportedException();
            }

            public override void SetLength(long value)
            {
                throw new NotSupportedException();
            }

            public override void Write(byte[] buffer, int offset, int count)
            {
                throw new NotSupportedException();
            }

            // Properties
            public override bool CanRead
            {
                get
                {
                    return true;
                }
            }

            public override bool CanSeek
            {
                get
                {
                    return false;
                }
            }

            public override bool CanWrite
            {
                get
                {
                    return false;
                }
            }

            public override long Length
            {
                get
                {
                    return this.internalStream.Length;
                }
            }

            public override long Position
            {
                get
                {
                    return this.internalStream.Position;
                }
                set
                {
                    if (value < 2L)
                    {
                        this.bufferedBytesIndex = (int)value;
                    }
                    else
                    {
                        this.internalStream.Position = value;
                    }
                }
            }
        }
    }
}
