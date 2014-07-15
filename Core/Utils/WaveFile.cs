using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Lin.Core.Log;

namespace Lin.Core.Utils
{
    /// <summary>
    /// 读取音频文件
    /// </summary>
    public class WaveFile
    {
        public class Riff
        {
            internal Riff()
            {
                m_RiffID = new byte[4];
                m_RiffFormat = new byte[4];
            }

            internal void ReadRiff(FileStream inFS)
            {
                inFS.Read(m_RiffID, 0, 4);
                BinaryReader binRead = new BinaryReader(inFS);
                m_RiffSize = binRead.ReadUInt32();
                inFS.Read(m_RiffFormat, 0, 4);
            }
            public byte[] RiffID
            {
                get { return m_RiffID; }
            }
            public uint RiffSize
            {
                get { return (m_RiffSize); }
            }
            public byte[] RiffFormat
            {
                get { return m_RiffFormat; }
            }
            private byte[] m_RiffID;
            private uint m_RiffSize;
            private byte[] m_RiffFormat;
        }

        public class Fmt
        {
            internal Fmt()
            {
                m_FmtID = new byte[4];
            }

            internal void ReadFmt(FileStream inFS)
            {
                inFS.Read(m_FmtID, 0, 4);

                //Debug.Assert(m_FmtID[0] == 102, "Format ID Not Valid");

                BinaryReader binRead = new BinaryReader(inFS);

                m_FmtSize = binRead.ReadUInt32();
                m_FmtTag = binRead.ReadUInt16();
                m_Channels = binRead.ReadUInt16();
                m_SamplesPerSec = binRead.ReadUInt32();
                m_AverageBytesPerSec = binRead.ReadUInt32();
                m_BlockAlign = binRead.ReadUInt16();
                m_BitsPerSample = binRead.ReadUInt16();


                inFS.Seek(m_FmtSize + 20, System.IO.SeekOrigin.Begin);
            }

            public byte[] FmtID
            {
                get { return m_FmtID; }
            }

            public uint FmtSize
            {
                get { return m_FmtSize; }
            }

            public ushort FmtTag
            {
                get { return m_FmtTag; }
            }

            public ushort Channels
            {
                get { return m_Channels; }
            }

            public uint SamplesPerSec
            {
                get { return m_SamplesPerSec; }
            }

            public uint AverageBytesPerSec
            {
                get { return m_AverageBytesPerSec; }
            }

            public ushort BlockAlign
            {
                get { return m_BlockAlign; }
            }

            public ushort BitsPerSample
            {
                get { return m_BitsPerSample; }
            }

            private byte[] m_FmtID;
            private uint m_FmtSize;
            private ushort m_FmtTag;
            private ushort m_Channels;
            private uint m_SamplesPerSec;
            private uint m_AverageBytesPerSec;
            private ushort m_BlockAlign;
            private ushort m_BitsPerSample;
        }
    
        public class Data
        {
            internal Data()
            {
                m_DataID = new byte[4];
            }
                
                 
             
            internal void ReadData(FileStream inFS)
            {
                try
                {
                    inFS.Read(m_DataID, 0, 4);

                    //Debug.Assert(m_DataID[0] == 100, "Data ID Not Valid");

                    BinaryReader binRead = new BinaryReader(inFS);

                    m_DataSize = binRead.ReadUInt32();

                    m_Data = new Int16[m_DataSize];

                    inFS.Seek(44, System.IO.SeekOrigin.Begin);

                    m_NumSamples = (int)(m_DataSize / 2);

                    for (int i = 0; i < m_NumSamples; i++)
                    {
                        m_Data[i] = binRead.ReadInt16();
                    }
                }
                catch (Exception e)
                {
                    //Lin.Core.Controls.TaskbarNotifierUtil.Show("读取音频文件失败!", LogLevel.WARNING, "温馨提示");
                } 
            }

            public byte[] DataID
            {
                get { return m_DataID; }
            }

            public uint DataSize
            {
                get { return m_DataSize; }
            }

            public double this[int pos]
            {
                get { return m_Data[pos] / 32768.0; }
            }

            public int NumSamples
            {
                get { return m_NumSamples; }
            }

            private byte[] m_DataID;
            private uint m_DataSize;
            private Int16[] m_Data;
            private int m_NumSamples;
        }

        public WaveFile(FileInfo file)
        {
            m_FileInfo = file;
            m_Filepath = file.FullName;
            Init();
        }
        private void Init()
        {
            m_FileStream = m_FileInfo.OpenRead();

            m_Riff = new Riff();
            m_Fmt = new Fmt();
            m_Data = new Data();
        }
        public WaveFile(String inFilepath)
        {
            m_Filepath = inFilepath;
            m_FileInfo = new FileInfo(inFilepath);
            Init();
        }

        public void Read()
        {
            m_Riff.ReadRiff(m_FileStream);
            m_Fmt.ReadFmt(m_FileStream);
            m_Data.ReadData(m_FileStream);
        }

        private string m_Filepath;
        private FileInfo m_FileInfo;
        private FileStream m_FileStream;

        public Riff m_Riff { get; private set; }
        public Fmt m_Fmt { get; private set; }
        public Data m_Data { get; private set; }

        /// <summary>
        /// 判断文件是否是音频文件
        /// </summary>
        /// <param name="file">需要判断的文件</param>
        /// <returns>返回bool类型值</returns>
        public static bool IsWaveFile(FileInfo file)
        {
            FileStream stream = null; 
            try
            {
                stream = file.OpenRead();
                byte[] ware_FmtID = new byte[4];
                stream.Read(ware_FmtID, 0, 4);
                if (ware_FmtID[0] != 82 || ware_FmtID[1] != 73 || ware_FmtID[2] != 70 || ware_FmtID[3] != 70)
                {
                    return false;
                }
                stream.Seek(9, System.IO.SeekOrigin.Begin);

                stream.Read(ware_FmtID, 0, 4);

                if (ware_FmtID[0] != 65 || ware_FmtID[1] != 86 || ware_FmtID[2] != 69 || ware_FmtID[3] != 102)
                {
                    return false;
                }
                return true;
            }
            catch (System.IO.IOException)
            {
                return false;
            }
            finally
            {
                if (stream != null)
                {
                    stream.Close();
                }
            }
        }
    }
}
