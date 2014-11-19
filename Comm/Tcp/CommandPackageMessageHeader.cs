using Lin.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lin.Comm.Tcp
{
    public class CommandPackageMessageHeader
    {
        private int _command;
        /// <summary>
        /// 数据包类型号
        /// </summary>
        public int command
        {
            get { return _command; }
            set { this._command = value; }
        }

        private byte _majorVersion;

        /// <summary>
        /// 主版本号
        /// </summary>
        public byte majorVersion { get { return _majorVersion; } set { this._majorVersion = value; } }
        private byte _minorVersion;
        /// <summary>
        /// 副版本号
        /// </summary>
        public byte minorVersion { get { return _minorVersion; } set { this._minorVersion = value; } }
        private byte _correctVersion;

        /// <summary>
        /// 修正版本号
        /// </summary>
        public byte correctVersion { get { return _correctVersion; } set { this._correctVersion = value; } }

        private int _length = 0;

        /// <summary>
        /// 数据长度
        /// </summary>
        public int length { get { return _length; } set { this._length = value; } }	//数据长度
        //private long _sequeueid;

        ///// <summary>
        ///// 序列号
        ///// </summary>
        //public long sequeueid { get { return _sequeueid; } set { this._sequeueid = value; } }

        public void read(byte[] headers)
        {
            _majorVersion = ByteUtils.ReadByte(headers);
            _minorVersion = ByteUtils.ReadByte(headers, 1);
            _correctVersion = ByteUtils.ReadByte(headers, 2);
            _command = ByteUtils.ReadInt(headers,  3);
            _length = ByteUtils.ReadInt(headers,  7);
        }
    }
}
