using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.IO;
using System.Net;
using Lin.Core.Web.Json;
using Lin.Core.Utils;

namespace Lin.Core.Web.Http
{
    /// <summary>
    /// 实现文件上传
    /// </summary>
    internal class HttpUpload
    {
        static HttpUpload()
        {
            string server = Lin.Core.Config.ConfigManager.Net["Server"];
            if (server != null && server != "")
            {
                if (server.EndsWith("/"))
                {
                    HttpUpload.UploadUri = new Uri(server + "cloud/action/file!upload.action?__result__=json");
                }
                else
                {
                    HttpUpload.UploadUri = new Uri(server + "/cloud/action/file!upload.action?__result__=json");
                }
            }
            else
            {
                HttpUpload.UploadUri = new Uri("http://localhost:8080/web/cloud/action/file!upload.action");
            }
        }
        private static string UploadUriString { get; set; }
        private static Uri _uploadUri;
        public static Uri UploadUri
        {
            get { return _uploadUri; }
            set { _uploadUri = value; UploadUriString = value.AbsoluteUri; }
        }

        private Action<string> Result { get; set; }
        private Action<Error> Fault { get; set; }


        //private SynchronizationContext _syncContext;
        public HttpUpload()
        {
            //_syncContext = SynchronizationContext.Current;
        }
        private FileInfo file;
        private HttpWebRequest request = null;
        public void Upload(HttpCommunicateImpl impl,FileInfo file, Action<string> result, Action<Error> fault,
            string md5, long start, long end, long total, Action<long, long> progress = null)
        {
            this.Result = result;
            this.Fault = fault;
            this.file = file;

            try
            {
                //String uriString = uri.AbsoluteUri;
                //给uri加一个时间戳，停止浏览器缓存
                //采用_time_stamp_" + DateTime.Now.Ticks做为参数名，防止url中有_time_stamp_参数名，
                Uri uri;
                if (UploadUriString.IndexOf('?') == -1)
                {
                    uri = new Uri(UploadUriString + "?_time_stamp_" + DateTime.Now.Ticks + "=1&total=" + total + "&start=" + start + "&end=" + end + "&md5=" + md5);
                }
                else
                {
                    uri = new Uri(UploadUriString + "&_time_stamp_" + DateTime.Now.Ticks + "=1&total=" + total + "&start=" + start + "&end=" + end + "&md5=" + md5);
                }
                //uri = new Uri(UploadUriString);

                request = (HttpWebRequest)WebRequest.Create(uri);
                string boundary = "---------------------------7d429871607fe";
                //HttpWebRequest httpWebRequest2 = (HttpWebRequest)WebRequest.Create(uri);
                request.Credentials = CredentialCache.DefaultCredentials;
                request.CookieContainer = impl.Cookies;
                request.ContentType = "multipart/form-data; boundary=" + boundary;
                request.Method = "POST";

                // Build up the post message header
                StringBuilder sb = new StringBuilder();
                sb.Append("--");
                sb.Append(boundary);
                sb.Append("\r\n");
                sb.Append("Content-Disposition:form-data;name=\"upload\";filename=\"");
                //sb.Append(file.FullName);
                sb.Append(HttpUtility.UrlEncode(file.Name, Encoding.UTF8));
                sb.Append("\"");
                sb.Append("\r\n");
                sb.Append("Content-Type: application/octet-stream");
                sb.Append("\r\n");
                sb.Append("\r\n");

                string postHeader = sb.ToString();
                byte[] postHeaderBytes = Encoding.UTF8.GetBytes(postHeader);

                // Build the trailing boundary string as a byte array
                // ensuring the boundary appears on a line by itself
                byte[] boundaryBytes = Encoding.ASCII.GetBytes("\r\n--" + boundary + "--\r\n");

                byte[] bs = Encoding.ASCII.GetBytes("Content-Disposition: form-data; name=\"total\"\r\n\r\n10");

                //FileStream fileStream = new FileStream(file.Name, FileMode.Open, FileAccess.Read);
                FileStream fileStream = new FileStream(file.FullName, FileMode.Open, FileAccess.Read);

                request.ContentLength = fileStream.Length + (long)postHeaderBytes.Length + (long)boundaryBytes.Length;   //一定需要长度吗?
                //request.ContentLength = fileStream.Length + (long)postHeaderBytes.Length + (long)boundaryBytes.Length
                //    + bs.Length + (long)boundaryBytes.Length;

                // Write out the trailing boundary

                Stream requestStream = request.GetRequestStream();

                requestStream.Write(postHeaderBytes, 0, postHeaderBytes.Length);
                byte[] buffer = new Byte[checked((uint)global::System.Math.Min(4096, (int)fileStream.Length))];
                int bytesRead = 0;
                long writeTotal = 0;
                while ((bytesRead = fileStream.Read(buffer, 0, buffer.Length)) != 0)
                {
                    try
                    {
                        writeTotal += bytesRead;
                        if (progress != null)
                        {
                            progress(writeTotal, 0);
                        }
                    }
                    catch (Exception e)
                    {
                    }
                    requestStream.Write(buffer, 0, bytesRead);
                }
                requestStream.Write(boundaryBytes, 0, boundaryBytes.Length);
                requestStream.Flush();

                fileStream.Close();
                fileStream = null;

                //bs = Encoding.ASCII.GetBytes("\r\n10");
                //requestStream.Write(bs,
                //    0, bs.Length);

                //requestStream.Write(boundaryBytes, 0, boundaryBytes.Length);
                //IAsyncResult r = request.BeginGetRequestStream(new AsyncCallback(SetParams), request);

                //Stream stream = request.EndGetRequestStream
                //request.
                request.BeginGetResponse(new AsyncCallback(RespCallback), request);
                //request.BeginGetRequestStream(new AsyncCallback(SetParams), request);
            }
            catch (System.Exception e)
            {
                //如果有异常，表明此次请求失败
                if (Fault != null)
                {
                    //_syncContext.Post((object target) =>
                    //{

                    Error error = new Error();
                    error.code = -2000021;
                    error.message = e.Message;
                    error.stackTrace = Lin.Core.Utils.ExceptionInfoToString.ADExceptionToString(e);
                    Fault(error);
                    //}, null);
                }
            }
        }

        private void SetParams(IAsyncResult asynchronousResult)
        {
            try
            {
                HttpWebRequest request = asynchronousResult.AsyncState as HttpWebRequest;
                //request.ContentType = "application/x-www-form-urlencoded";
                Stream stream = request.EndGetRequestStream(asynchronousResult);
                StreamWriter _out = new StreamWriter(stream);
                

                _out.Write("start=0");
                _out.Write("&end=5");
                _out.Close();
                request.BeginGetResponse(new AsyncCallback(RespCallback), request);
            }
            catch (System.Text.EncoderFallbackException e)
            {//如果有异常表示此次请求失败
                if (Fault != null)
                {
                    Error error = new Error(0x2000002, "无法对请求参数进行编码！", "", e.StackTrace);
                    Fault(error);
                }
            }
            catch (System.Exception e)
            {
                Console.WriteLine("e:" + e);
                //如果有异常表示此次请求失败
                if (Fault != null)
                {
                    //Error error = new Error(0x2000003, "无法向服务器发出请求！", this.package.location, e.StackTrace);
                    //Fault(error);
                }
            }
        }

        /// <summary>
        /// 对返回数据进行处理
        /// </summary>
        /// <param name="resultString"></param>
        private void ProcessResultData(String resultString)
        {
            //对返回数据进行处理
            //System.Runtime.Serialization.Json.DataContractJsonSerializer dc = new System.Runtime.Serialization.Json.DataContractJsonSerializer(typeof(ResultData));
            //Stream stream = new MemoryStream(Encoding.UTF8.GetBytes(resultString));
            //Console.WriteLine(resultString);
            //object data = dc.ReadObject(stream);
            JsonValue jsonValue = JsonUtil.Deserialize(resultString) as JsonValue;
            ResultData tmp = JsonUtil.Deserialize<ResultData>(jsonValue);
            if (tmp != null && tmp.code != 0)
            {
                //返回数据有误，抛出异常
                //MessageBox.Show("系统异常：\r\n错误代码：0x" + tmp.code.ToString("X") + "\r\n错误消息：" + tmp.message);
                //tmp.result = null;

                ValidationErrorData errorData = JsonUtil.Deserialize<ValidationErrorData>(jsonValue["result"]);
                Error error = tmp.CopyProperty<Error>();
                error.data = errorData;
                this.Fault(error);
                return;
            }
            //if (tmp.result != null)
            //{
            JsonValue tmpJv = null;
            try
            {
                tmpJv = jsonValue["result"];
            }
            catch (System.Exception e)
            {
                Error error = new Error();
                error.code = -2000021;
                error.message = e.Message;
                error.stackTrace = Lin.Core.Utils.ExceptionInfoToString.ADExceptionToString(e);
                this.Fault(error);
            }
            if (tmpJv != null)
            {
                //Console.WriteLine("result:" + resultString);
                //Console.WriteLine("json:" + tmpJv);
                //object resultObject = JsonUtil.Deserialize(tmpJv, this.package.RespType);
                string resultObject = JsonUtil.Deserialize(tmpJv, typeof(string)) as string;
                Result(resultObject);
            }
            else
            {
                Error error = new Error();
                error.code = -2000021;
                error.cause = "无返回结果";
                error.message = "无返回结果";
                this.Fault(error);
                //Result(null);
            }
            return;
            //}
            //Result(null);
        }

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
                resultString = HttpUtility.UrlDecode(resultString);

                //resultString new String(System.Convert.FromBase64String(resultString));
                Stream stream = new MemoryStream(System.Convert.FromBase64String(resultString));
                resultString = new StreamReader(stream, Encoding.UTF8, true).ReadToEnd();
            }
            catch (System.Exception e)
            {
                if (Fault != null)
                {
                    //if (_syncContext != null)
                    //{
                    //    _syncContext.Post((object target) =>
                    //    {
                    Error error = new Error();
                    error.code = -2000021;
                    error.message = e.Message;
                    error.stackTrace = Lin.Core.Utils.ExceptionInfoToString.ADExceptionToString(e);
                    this.Fault(error);
                    //    }, null);
                    //}
                    //else
                    //{
                    //    Fault(e);
                    //}
                }
                return;
            }
            ProcessResultData(resultString);
        }

        public void Abort()
        {
            if (request != null)
            {
                request.Abort();
            }
        }
    }
}