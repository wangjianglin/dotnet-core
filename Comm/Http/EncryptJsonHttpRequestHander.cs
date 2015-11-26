using Lin.Util.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Lin.Util.Extensions;
using System.Net;

namespace Lin.Comm.Http
{
    internal class EncryptJsonHttpRequestHander : IHttpRequestHandle
    {
        internal static class Utils
        {
            private static int _Sequeue = 0;
            /// <summary>
            /// 序列号
            /// </summary>
            public static int Sequeue { get { _Sequeue++; return (int)((1L + _Sequeue + int.MaxValue) % (int.MaxValue + 1L)); } }
            private static readonly long offset = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).Ticks;
            /// <summary>
            /// 时间戳，以伦敦时间1970-01-01 00:00:00.000为基准的毫秒数
            /// </summary>
            public static long Timestamp { get { return (DateTime.UtcNow.Ticks - offset) / 10000; } }
        }

        public byte[] GetParams(HttpWebRequest request, HttpPackage package)
        {
            request.Method = "POST";//以POST方式传递数据
            request.ContentType = "application/x-www-form-urlencoded";
            request.Headers.Add(Constants.HTTP_COMM_PROTOCOL, Constants.HTTP_VERSION);

            StringBuilder sb = new StringBuilder();
            sb.Append('{');
            sb.Append("\"location\":");
            sb.Append("\"" + package.location + "\",");
            sb.Append("\"timestamp\":");
            sb.Append(Utils.Timestamp);
            sb.Append(",\"sequeueid\":");
            sb.Append(Utils.Sequeue);

            //增加版本信息
            sb.Append(",\"version\":{\"major\":");
            sb.Append(package.Version.Major);
            sb.Append(",\"minor\":");
            sb.Append(package.Version.Minor);
            sb.Append("}");

            //增加参数信息
            sb.Append(",\"data\":");
            IDictionary<string, object> dict = package.GetParams();
            //Lin.Core.Web.Json.DataContractJsonSerializer dc = new Lin.Core.Web.Json.DataContractJsonSerializer(typeof(Dictionary<string, object>));
            ////String jsonString = "{}";
            //MemoryStream ms = new MemoryStream();
            //dc.WriteObject(ms, dict);
            //byte[] tmp = ms.ToArray();
            //sb.Append(Encoding.UTF8.GetString(tmp, 0, tmp.Length));
            
                sb.Append(JsonUtil.Serialize(dict));
            
            
            //sb.Append("{\"data\":\"测试中文\"}");

            sb.Append("}");

            string b64s = System.Convert.ToBase64String(Encoding.Default.GetBytes(sb.ToString()));
            //Console.WriteLine("value:" + sb.ToString());
            //Console.WriteLine("b64s:" + b64s);
            //Console.WriteLine("coding:" + Encoding.Default.EncodingName);
            string r = Constants.HTTP_JSON_PARAM+"=" + HttpUtility.UrlEncode(b64s) +
                "&" + Constants.HTTP_REQUEST_CODING+ "=" + Encoding.Default.BodyName +
                //"&__version__=0.1"+
                "&" + Constants.HTTP_RESULT + "=json";
            return Encoding.ASCII.GetBytes(r);
        }

        public void Response(HttpPackage package, byte[] bs,Action<Object,IList<Error>> result, Action<Error> fault)
        {
            string resp = null;
            try
            {
                //resp = Encoding.ASCII.GetDecoder().
                //resp = HttpUtility.UrlDecode(resp);

                //resultString new String(System.Convert.FromBase64String(resultString));
                //Stream stream = new MemoryStream(System.Convert.FromBase64String(resp));
                Stream stream = new MemoryStream(bs);
                resp = new StreamReader(stream, Encoding.UTF8, true).ReadToEnd();
            }
            catch (System.ArgumentNullException e)
            {
                if (fault != null)
                {
                    Error error = new Error(0x2000009, "没有返回数据！", resp, e.StackTrace);
                    fault(error);
                    return;
                }
            }
            catch (System.ArgumentException e)
            {
                if (fault != null)
                {
                    Error error = new Error(0x200000a, "返回数据编码错误！", resp, e.StackTrace);
                    fault(error);
                    return;
                }
            }
            catch (System.FormatException e)
            {
                if (fault != null)
                {
                    Error error = new Error(0x200000b, "返回数据格式错误！", resp, e.StackTrace);
                    fault(error);
                    return;
                }
            }
              ProcessResultData(package,resp,result,fault);
        }


        private void ProcessResultData(HttpPackage package,String resultString,Action<Object,IList<Error>> Result, Action<Error> Fault)
        {
            //对返回数据进行处理
            //System.Runtime.Serialization.Json.DataContractJsonSerializer dc = new System.Runtime.Serialization.Json.DataContractJsonSerializer(typeof(ResultData));
            //Stream stream = new MemoryStream(Encoding.UTF8.GetBytes(resultString));
            //Console.WriteLine(resultString);
            //object data = dc.ReadObject(stream);
            if (resultString == null || resultString == "")
            {
                Error error = new Error(0x2000005, "无返回数据！", resultString);
                Fault(error);
                return;
            }
            JsonValue jsonValue = null;
            ResultData tmp = null;
            try
            {
                jsonValue = JsonUtil.Deserialize(resultString) as JsonValue;
                tmp = JsonUtil.Deserialize<ResultData>(jsonValue);
            }
            catch (Exception e)
            {
                Error error = new Error(0x2000006, "服务器返回数据格式错误！", resultString, e.StackTrace);
                Fault(error);
                return;
                //this.Fault(resultString);
            }
            if (tmp != null && tmp.code < 0)
            {
                //返回数据有误，抛出异常
                //MessageBox.Show("系统异常：\r\n错误代码：0x" + tmp.code.ToString("X") + "\r\n错误消息：" + tmp.message);
                //tmp.result = null;
                //Console.WriteLine("error:\n" + resultString);
                //Error error = new Error(tmp.error, tmp.message, tmp.cause);
                ValidationErrorData errorData = JsonUtil.Deserialize<ValidationErrorData>(jsonValue["result"]);
                Error error = tmp.CopyProperty<Error>();
                error.data = errorData;
                Fault(error);
                return;
            }
            JsonValue tmpJv = null;
            try
            {
                tmpJv = jsonValue["result"];
            }
            catch (System.Exception e)
            {
                Error error = new Error(0x2000007, "无法得到返回结果！", resultString, e.StackTrace);
                Fault(error);
                return;
            }
            //if (tmpJv != null)
            //{
            //Console.WriteLine("result:" + resultString);
            //Console.WriteLine("json:" + tmpJv);
            object resultObject = null;
            try
            {
                resultObject = JsonUtil.Deserialize(tmpJv, package.RespType);
            }
            catch (Exception e)
            {
                Error error = new Error(0x2000008, "无法解析异常！", resultString, e.StackTrace);
                Fault(error);
                return;
            }
            if (tmp.code > 0)
            {
                if (tmp.warning == null)
                {
                    tmp.warning = new List<Error>();
                }
                Error warning = new Error();
                warning.code = tmp.code;
                warning.cause = tmp.cause;
                warning.message = tmp.message;
                warning.stackTrace = tmp.stackTrace;

                tmp.warning.Add(warning);
            }
            Result(resultObject, tmp.warning);
            //}
            //else
            //{
            //    //Console.WriteLine("error:\n" + resultString);
            //    Result(null, tmp.warning);
            //}
        }



        public class ResultData
        {
            public long code { get; set; }
            public long sequeueid { get; set; }
            //public object result { get; set; }
            public string message { get; set; }
            public IList<Error> warning { get; set; }

            public string cause { get; set; }

            public string stackTrace { get; set; }

            public int dataType { get; set; }
        }
    }


   
}
