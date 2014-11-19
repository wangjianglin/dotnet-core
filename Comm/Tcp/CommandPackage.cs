using Lin.Util;
using Lin.Util.Extensions;
using System;
using System.Runtime.InteropServices;
using System.Threading;

namespace Lin.Comm.Tcp
{
   
    //[PackageAttribute]
    public abstract class CommandPackage : Package//ICommandPackageParser,
    {
        private int _command = 0;
        private byte _major = 0;
        private byte _minor = 0;
        private byte _revise = 0;

        protected CommandPackage()
        {
            Type type = this.GetType();
            Command c = type.GetCustomAttribute<Command>(false);
            this._command = c.Commaand;
            
        }

        protected virtual int bodySize()
        {
            return 0;
        }

        protected virtual void bodyWrite(byte[] bs, int offset = 0)
        {
        }

        public int Size
        {
            get { return (this.bodySize() + 11); }
        }

        public sealed override byte[] Write()
        {
            byte[] bs = new byte[this.Size];
            //Utils.Write(bs, 0, 0);
            ByteUtils.WriteByte(bs, this._major, 0);
            ByteUtils.WriteByte(bs, this._minor, 1);
            ByteUtils.WriteByte(bs, this._revise, 2);
            ByteUtils.WriteInt(bs, this._command, 3);
            ByteUtils.WriteInt(bs, this.Size, 7);
            //Utils.Write(bs, 0, 11);
            this.bodyWrite(bs, 11);
            return bs;
        }

        public int Command
        {
            get
            {
                return this._command;
            }
        }

        public byte Major
        {
            get
            {
                return this._major;
            }
            internal set
            {
                this._major = value;
            }
        }

        public byte Minor
        {
            get
            {
                return this._minor;
            }
            internal set
            {
                this._minor = value;
            }
        }

        public byte Revise
        {
            get
            {
                return this._revise;
            }
            internal set
            {
                this._revise = value;
            }
        }

        public sealed override byte Type
        {
            get
            {
                return 1;
            }
        }

        //int PackageParser.Command
        //{
        //    get { throw new NotImplementedException(); }
        //}

        
        //public abstract Package Parser2(byte[] bs);

        public abstract void Parser(byte[] bs);
        //public virtual CommandPackage Parser(byte[] bs)
        //{
        //    //this.GetType()
        //    return (CommandPackage)Activator.CreateInstance(this.GetType());
        //}
    }
}

