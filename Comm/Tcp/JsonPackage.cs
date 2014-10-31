using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Lin.Comm.Tcp
{
 
    public abstract class JsonPackage:Package
    {

        private string _path;
        public JsonPackage()
        {
            this._path = this.GetType().GetCustomAttribute<JsonPath>().Path;
        }
        
        public void Parser(string json)
        {

            MethodInfo[] ms = typeof(Lin.Util.Json.JsonUtil).GetMethods();
            //Console.WriteLine("ms:" + ms);
            MethodInfo deserializeMethod = null;
            foreach(MethodInfo m in ms){
                if (m.Name == "Deserialize" && m.IsGenericMethod && m.ReturnParameter.ParameterType.IsGenericParameter && m.GetParameters().Length == 1 && m.GetParameters()[0].ParameterType == typeof(string))
                {
                    deserializeMethod = m;
                    break;
                }
            }

            PropertyInfo pi = this.GetType().GetProperty("Params", BindingFlags.NonPublic | BindingFlags.Instance);
            JsonParamsType jsonType = pi.GetCustomAttribute<JsonParamsType>();
            deserializeMethod = deserializeMethod.MakeGenericMethod(jsonType.Type);
            object[] args = new object[] { json };
            object obj = deserializeMethod.Invoke(null, args);

            this.Params = obj;
        }
        public sealed override byte Type
        {
            get { return 6; }
        }

        public string Path { get { return _path; } }

        public sealed override byte[] Write()
        {
            string json = Lin.Util.Json.JsonUtil.Serialize(Params);
            //return json.GetType()
            StringBuilder builder = new StringBuilder();

            //path
            builder.Append("path:");
            builder.Append(this.Path);
            builder.Append("\r\n");
            //coding
            builder.Append("coding:");
            builder.Append(Encoding.Default.BodyName);
            builder.Append("\r\n");
            //sequeue id
            //builder.Append("sequeue id:");
            //builder.Append("0");
            //builder.Append("\r\n");

            //version
            builder.Append("version:0.1.0build0");
            builder.Append("\r\n");

            //end
            builder.Append("\r\n");
            builder.Append(json);

            return Encoding.Default.GetBytes(builder.ToString());
        }

        protected virtual object Params { get; set; }
        //public abstract void test();
    }
}
