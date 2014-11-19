using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lin.Util.Extensions;

namespace Lin.Comm.Tcp
{
    [ProtocolParserType(1)]
    public class CommandProtocolParser_010 : IProtocolParser
    {
        private static Lin.Util.MapIndexProperty<int, Type> commands = new Util.MapIndexProperty<int, Type>();

        static CommandProtocolParser_010()
        {
            IList<Type> types = Lin.Util.Assemblys.AssemblyStore.FindTypesForCurrentDomain<CommandPackage>();
            //CommandPackage parser = null;
            foreach (Type type in types)
            {
                if (!type.IsAbstract)
                {
                    try
                    {
                        //parser = System.Activator.CreateInstance(type) as CommandPackage;
                        commands[type.GetCustomAttribute<Command>().Commaand] = type;
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine();
                        Console.WriteLine("type:" + type.Name);
                        Console.WriteLine(e.StackTrace);
                    }
                }
            }
        }

        private int packageSize = 0;//已经读取的数据个数
        private int maxPackageSize = 100;//packageBody的大小、
        private byte[] packageBody = new byte[100];//存储消息，包括消息头体

        CommandPackageMessageHeader messageHeader = new CommandPackageMessageHeader();

        private byte[] header = new byte[11];//BYTE header[16];//存储消息头	//2013-08-07 liyk
        public Package GetPackage()
        {
            //throw new NotImplementedException();
            //return CommandPackageManager.Parse(this.packageBody);
            //int num;
            //byte num2;
            //byte num3;
            //byte num4;
            CommandPackage package;
            //Utils.Read(this.packageBody, out num2, 0);
            //Utils.Read(this.packageBody, out num3, 1);
            //Utils.Read(this.packageBody, out num4, 2);
            //Utils.Read(this.packageBody, out num, 3);
            //if (!parsers.ContainsKey(num))
            //{
            //    return null;
            //}
            //package = parsers[num].Parser(bs);
            package = Activator.CreateInstance(commands[messageHeader.command]) as CommandPackage;
            package.Parser(this.packageBody);
            package.Major = 0;
            package.Minor = 1;
            package.Revise = 0;
            return package;
        }

        /// <summary>
        /// 单线程访问，不用考虑多线程问题
        /// </summary>
        /// <param name="bs"></param>
        public void Put(params byte[] bs)
        {
            for (int n = 0; n < bs.Length; n++)
            {
                PutImpl(bs[n]);
            }
        }

        private void PutImpl(byte b){
            if (packageSize < header.Length)
            {//packageSize=19时，跳到packageSize++,读出length，下一次进入下一个if
                header[packageSize] = b;
            }
            if (packageSize >= header.Length)
            {
                if (packageSize >= messageHeader.length)
                {//数据异常
                    //LogFile.InitAndWriteLog("读取文件头19字节时异常, length is %d",length);
                    //state = false;
                    //isFF = false;
                    //isFA = false;
                    //packageSize = 0;
                    //initStatue();
                    //continue;
                    return;
                }
                //					delete [] packageBody;
                packageBody[packageSize - header.Length] = b;
            }
            packageSize++;

            if (packageSize == header.Length)
            {
                //解析消息头
                messageHeader.read(header);
                if (messageHeader.length < header.Length)
                {//数据异常，抛弃些数据段
                    //initStatue();
                }
                else
                {
                    //避免频繁释放内存
                    if (messageHeader.length - header.Length >= maxPackageSize)
                    {
                        packageBody = new byte[messageHeader.length - header.Length];
                        maxPackageSize = messageHeader.length - header.Length;
                    }
                    //CommunicateSocketListener_body(&packageBody,length);
                    //for (int n1 = 0; n1 < header.Length; n1++)
                    //{
                    //    packageBody[n1] = header[n1];
                    //}
                }
            }
        }

        public void Clear()
        {
            packageSize = 0;
        }
    }
}
