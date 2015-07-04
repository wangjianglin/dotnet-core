using System;
using System.Net;
using System.IO;
using System.Text;
using System.Threading;
using System.Collections.Generic;
using System.Web;
//using System.Runtime.Serialization.Json;

namespace Lin.Comm.Http
{
    public class HttpRequest
    {
        /// <summary>
        /// 用以处理序列号和时间截
        /// </summary>
       
        

        private Action<Object,IList<Error>> Result { get; set; }
        private Action<Error> _fault { get; set; }
        private void Fault(Error error)
        {
            if (this._fault != null)
            {
                lock (this)
                {
                    if (isAbort)
                    {
                        this._fault(new Error(HttpCommunicateResult.ABORT_CODE, "通信操作已被取消"));
                        return;
                    }
                    _fault(error);
                }
            }
        }

        private Package package;

        static HttpRequest()
        {
        }
        //public static readonly String VERSION = "0.1";
        //private static readonly String HTTP_COMM_PROTOCOL = "__http_comm_protocol__";

        private Uri uri;
        private string CommUriString = null;
        private HttpWebRequest request = null;
        public void Request(HttpCommunicateImpl impl,Package package, Action<Object, IList<Error>> result, Action<Error> fault)
        {
            this.Result = result;
            this._fault = fault;
            this.package = package;

            try
            {
                //string urlPath = package.location;
                //string urlPath = "__http_comm_protocol__";
                //String uriString = uri.AbsoluteUri;
                //给uri加一个时间戳，停止浏览器缓存
                //采用_time_stamp_" + DateTime.Now.Ticks做为参数名，防止url中有_time_stamp_参数名，
                //if (package.UrlType == Packages.UrlType.RELATIVE)
                //{
                    //if (package.EnableCache)
                    //{
                    //    CommUriString = impl.CommUri.AbsoluteUri;
                    //    if (CommUriString.EndsWith("/") || urlPath.StartsWith("/"))
                    //    {
                    //        uri = new Uri(CommUriString + urlPath);
                    //    }
                    //    else
                    //    {
                    //        uri = new Uri(CommUriString + "/" + urlPath);
                    //    }
                    //}
                    if(!package.EnableCache)
                    {
                        CommUriString = impl.CommUri.AbsoluteUri;
                        
                        if (package.location.StartsWith("/"))
                        {
                            CommUriString += package.location;
                        }
                        else
                        {
                            CommUriString += "/" + package.location;
                        }
                        if (CommUriString.Contains("?"))
                        {
                            uri = new Uri(CommUriString + "&_time_stamp_" + DateTime.Now.Ticks + "=1");
                        }
                        else
                        {
                            uri = new Uri(CommUriString + "?_time_stamp_" + DateTime.Now.Ticks + "=1");
                        }
                    }
               

                lock (this)
                {
                    request = (HttpWebRequest)WebRequest.Create(uri);
                }
                //Console.WriteLine("proxy:" + request.Proxy);
                //request.Proxy = proxy;
                request.Method = "POST";//以POST方式传递数据
                request.ContentType = "application/x-www-form-urlencoded";
                request.CookieContainer = impl.Cookies;
                //request.Headers.Add(Constants.HTTP_COMM_PROTOCOL, Constants.HTTP_VERSION);
                //request.Timeout
                //Console.WriteLine("time out:" + request.Timeout);
                //if (package.HasParams)
                {
                    request.BeginGetRequestStream(new AsyncCallback(SetParams), request);
                }
                //else
                {
                    //request.BeginGetResponse(new AsyncCallback(RespCallback), request);
                }
                //r.AsyncWaitHandle.WaitOne();
            }
            catch (System.Exception e)
            {
                //如果有异常，表明此次请求失败
                if (_fault != null)
                {
                    Error error = new Error(0x2000002, "无法连接服务器！", e.StackTrace);
                    Fault(error);
                }
            }
            //catch (Error e) { }
        }
         
        /// <summary>
        /// 对返回数据进行处理
        /// </summary>
        /// <param name="resultString"></param>
     

        /// <summary>
        /// 处理请求响应
        /// </summary>
        /// <param name="asynchronousResult"></param>
        private void RespCallback(IAsyncResult asynchronousResult)
        {
            string resultString = null;
            try
            {
                HttpWebRequest request = asynchronousResult.AsyncState as HttpWebRequest;
                HttpWebResponse response = (HttpWebResponse)request.EndGetResponse(asynchronousResult);

                Stream _in = response.GetResponseStream();
                StreamReader reader = new StreamReader(_in);
                resultString = reader.ReadToEnd();
                //Stream stream = new MemoryStream(Encoding.UTF8.GetBytes(resultString));
                //Encoding encoding = DetectEncoding(stream.ReadByte(), stream.ReadByte());
                //stream.Position = 0L;
                //resultString = new StreamReader(stream,Encoding.Default, true).ReadToEnd();
                //if (package.HasParams)
                {
                    //resultString = HttpUtility.UrlDecode(resultString);

                    //resultString new String(System.Convert.FromBase64String(resultString));
                    //Stream stream = new MemoryStream(System.Convert.FromBase64String(resultString));
                    //resultString = new StreamReader(stream, Encoding.UTF8, true).ReadToEnd();
                }
                //else
                {
                    //if (Result != null)
                    {
                       // Result(resultString, null);
                    }
                }
                ////Encoding.Default.GetEncoder().
                //StringBuilder sb = new StringBuilder();
                //byte[] bs = System.Convert.FromBase64String(resultString);
                //foreach (byte b in bs)
                //{
                //    sb.Append(b);
                //}
                ////sb.Append(System.Convert.FromBase64String(resultString));
                //resultString = sb.ToString();
                //resultString = new String(System.Text.Encoding.Default.GetChars(System.Text.Encoding.UTF8.GetBytes(resultString)));
                //Console.WriteLine("result:" + resultString);
                //Console.WriteLine("result;" + new String(System.Text.Encoding.Default.GetChars(System.Text.Encoding.UTF8.GetBytes(resultString))));
                //Console.WriteLine("result;" + new String(System.Text.Encoding.UTF8.GetChars(System.Text.Encoding.UTF8.GetBytes(resultString))));
                //Console.WriteLine("result;" + new String(System.Text.Encoding.Default.GetChars(System.Text.Encoding.Default.GetBytes(resultString))));
                //Console.WriteLine("result;" + new String(System.Text.Encoding.UTF8.GetChars(System.Text.Encoding.Default.GetBytes(resultString))));
            }
            catch (System.Exception e)
            {
                if (_fault != null)
                {
                    Error error = new Error(0x2000004, "服务器无响应！", CommUriString, e.StackTrace);
                    Fault(error);
                    return;
                }
            }
            //ProcessResultData(resultString);
            package.RequestHandle.Response(package, resultString, Result, Fault);
        }
        /// <summary>
        /// 设置请求参数
        /// </summary>
        /// <param name="asynchronousResult"></param>
        private void SetParams(IAsyncResult asynchronousResult)
        {
            try
            {
                HttpWebRequest request = asynchronousResult.AsyncState as HttpWebRequest;
                request.ContentType = "application/x-www-form-urlencoded";
                string param = null; 
                try
                {
                    param = package.RequestHandle.GetParams(request,package);
                }
                catch (System.Exception e)
                {
                    //如果有异常表示此次请求失败
                    if (_fault != null)
                    {
                        Error error = new Error(0x2000010, "无法对数据进行封装！", e.Message, e.StackTrace);
                        Fault(error);
                    }
                    return;
                }
                Stream stream = request.EndGetRequestStream(asynchronousResult);
                StreamWriter _out = new StreamWriter(stream);
                //if (PostData != null && PostData != "")
                //{


                _out.Write(param);

                _out.Close();
                request.BeginGetResponse(new AsyncCallback(RespCallback), request);
            }
            catch (System.Text.EncoderFallbackException e)
            {//如果有异常表示此次请求失败
                if (_fault != null)
                {
                    Error error = new Error(0x2000002, "无法对请求参数进行编码！", "", e.StackTrace);
                    Fault(error);
                }
            }
            catch (System.Exception e)
            {
                //如果有异常表示此次请求失败
                if (_fault != null)
                {
                    Error error = new Error(0x2000003, "无法向服务器发出请求！", this.package.location, e.StackTrace);
                    Fault(error);
                }
            }
        }

        

        /// <summary>
        /// 表示验证错误消息
        /// </summary>
        public class Validation
        {
            public IList<String> actionErrors { get; set; }
            public IList<String> actionMessages { get; set; }
            public IDictionary<String, IList<String>> fieldErrors { get; set; }
        }


        private bool isAbort = false;
        //private object abortObject = new object();

        public void Abort()
        {
            lock (this)
            {
                isAbort = true;
                if (request != null)
                {
                    request.Abort();
                }
            }
        }
    }
    
}
