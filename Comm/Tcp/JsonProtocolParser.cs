using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lin.Util.Extensions;

namespace Lin.Comm.Tcp
{
    [ProtocolParserType(6)]
    public class JsonProtocolParser:IProtocolParser
    {
        //private static IDictionary<string, Type> paths = new Dictionary<string, Type>();
        private static Lin.Util.MapIndexProperty<string, Type> paths = new Util.MapIndexProperty<string, Type>();
        static JsonProtocolParser()
        {
            IList<Type> types = Lin.Util.Assemblys.AssemblyStore.FindTypesForCurrentDomain<JsonPackage>();
            foreach (Type type in types)
            {
                if (!type.IsAbstract)
                {
                    try
                    {
                        //parser = System.Activator.CreateInstance(type) as CommandPackage;
                        //CommandPackageManager.RegisterPackage(type.GetCustomAttribute<Command>().Commaand, type);
                        paths[type.GetCustomAttribute<JsonPath>().Path]= type;
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

        /// <summary>
        /// 头结束标识
        /// </summary>
        private static readonly byte[] END_FLAG = Encoding.Default.GetBytes("\r\n\r\n");
        /// <summary>
        /// 换行标识
        /// </summary>
        private static readonly byte[] LINE_FLAG = Encoding.Default.GetBytes("\r\n");


        public Package GetPackage()
        {
            //throw new NotImplementedException();

            //Type type = this.GetType();
            //Console.WriteLine("json type:" + jsonType.Type.Name);
            //byte[] bs = builder.
            //IDictionary<string, string> headers = new Dictionary<string, string>();
            Lin.Util.MapIndexProperty<string, string> headers = new Util.MapIndexProperty<string, string>();
            int end = 0;
            int start = 0;
            for (int n = 0; n < count - 2; n++)
            {
                if (buffer[n] == LINE_FLAG[0]
                    && buffer[n + 1] == LINE_FLAG[1])
                {
                    end = n;
                    if (start == end)
                    {
                        start = end = end + 2;
                        break;
                    }
                    string tmp = Encoding.Default.GetString(buffer, start, end-start);
                    string[] tmp2 = tmp.Split(':');
                    headers[tmp2[0]] = tmp2[1];
                    start = end = end + 2;
                }
            }
            String json = Encoding.Default.GetString(buffer, start, count - start);
            JsonPackage package = Activator.CreateInstance(paths[headers["path"]]) as JsonPackage;
            package.Parser(json);

            return package;
        }
        //private StringBuilder builder = new StringBuilder();
        private byte[] buffer = new byte[100];
        private int bufferInterval = 100;
        private int count = 0;

        /// <summary>
        /// 单线程访问，不用考虑多线程问题
        /// </summary>
        /// <param name="bs"></param>
        public void Put(params byte[] bs)
        {
            //builder.Append(bs);
            this.Expansion();
            for (int n = 0; n < bs.Length; n++)
            {
                buffer[count++] = bs[n];
            }
        }


        private void Expansion()
        {
            if (count >= buffer.Length)
            {
                byte[] tmp = new byte[buffer.Length + bufferInterval];
                for (int n = 0; n < buffer.Length; n++)
                {
                    tmp[n] = buffer[n];
                }
                buffer = tmp;
            }

        }

        public void Clear()
        {
            //builder.Clear();
            count = 0;
        }
    }
}
