using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Net;

namespace Lin.Comm.Http
{
    /// <summary>
    /// 实现文件下载
    /// </summary>
    internal class HttpDownload
    {
        static HttpDownload()
        {
            //string server = Lin.Core.Config.ConfigManager.Net["Server"];
            //if (server != null && server != "")
            //{
            //    if (server.EndsWith("/"))
            //    {
            //        HttpDownload.DownloadUri = new Uri(server + "cloud/action/file!download.action?__result__=json");
            //    }
            //    else
            //    {
            //        HttpDownload.DownloadUri = new Uri(server + "/cloud/action/file!download.action?__result__=json");
            //    }
            //}
            //else
            //{
            //    HttpDownload.DownloadUri = new Uri("http://localhost:8080/web/cloud/action/file!download.action");
            //}
        }
        private static string DownloadUriString { get; set; }
        private static Uri _downloadUri;
        public static Uri DownloadUri
        {
            get { return _downloadUri; }
            set { _downloadUri = value; DownloadUriString = value.AbsoluteUri; }
        }

        private Action<File,long,long,long> Result { get; set; }
        private Action<Error> Fault { get; set; }


        //private SynchronizationContext _syncContext;
        public HttpDownload()
        {
            //_syncContext = SynchronizationContext.Current;
        }


        public long ReadBytes
        {
            get;
            private set;
        }
        private void RespCallback(IAsyncResult asynchronousResult)
        {
            Stream _in = null;
            long start = 0;
            long end = 0;
            long total = 0;
            File f = null;
            try
            {
                HttpWebRequest request = asynchronousResult.AsyncState as HttpWebRequest;
                HttpWebResponse response = (HttpWebResponse)request.EndGetResponse(asynchronousResult);
                _in = response.GetResponseStream();
                //long length = response.ContentLength;
                //StreamReader reader = new StreamReader(_in);
                //reader.
                //string resultString = reader.ReadToEnd();

                f = new File();
                string fileName = response.Headers["Content-Disposition"] + "";
                int index = -1;
                if (!string.IsNullOrEmpty(fileName))
                {
                    index = fileName.IndexOf("filename=");
                }
                //f.FileName = fileName.Substring(index + "filename=".Length + 1, fileName.Length - (index + "filename=".Length + 2));
                if (index != -1)
                {
                    f.FileName = HttpUtility.UrlDecode(fileName.Substring(index + "filename=".Length + 1, fileName.Length - (index + "filename=".Length + 2)));
                }
                else
                {
                    f.FileName = DateTime.Now.Ticks + "-" + (new System.Random()).Next(1000) + ".tmp";
                }
                if (!string.IsNullOrEmpty(f.FileName))
                {
                    int pos = f.FileName.LastIndexOf('\\');
                    if (pos != -1)
                    {
                        f.FileName = f.FileName.Substring(pos + 1);
                    }
                }
                try
                {
                    f.Date = DateTime.Parse(response.Headers["Date"].ToString());
                }
                catch (Exception)
                {
                    f.Date = DateTime.Now;
                }
                f.ContentType = response.Headers["Content-Type"] + "";

                //FileInfo file = new FileInfo(Path.GetTempPath() + "tmp_" + DateTime.Now.Ticks + "_" + f.FileName);
                //FileInfo file = new FileInfo(Lin.Core.ViewModel2.Context.Cache.TmpDir.FullName + "\\download_tmp_" + DateTime.Now.Ticks + "_" + f.FileName);
                FileInfo file = new FileInfo(Path.GetTempPath() + "\\download_tmp_" + DateTime.Now.Ticks + "_" + f.FileName);

                f.FileInfo = file;

                Stream stream = file.Create();
                byte[] buffer = new Byte[4096];
                int bytesRead = 0;
                long writeTotal = 0;

                string bs = response.Headers["Content-Range"];
               
                if (!string.IsNullOrEmpty(bs) && bs.Length > 6)
                {
                    bs = bs.Substring(6);
                    string[] bss = bs.Split('/');
                    total = long.Parse(bss[1]);
                    bss = bss[0].Split('-');
                    start = long.Parse(bss[0]);
                    end = long.Parse(bss[1]);
                }

                while ((bytesRead = _in.Read(buffer, 0, buffer.Length)) != 0)
                {
                    writeTotal += bytesRead;
                    try
                    {
                        if (progress != null)
                        {
                            progress(writeTotal, total);
                        }
                    }
                    catch (Exception e)
                    {

                    }
                    stream.Write(buffer, 0, bytesRead);
                }
                stream.Dispose();
                stream.Close();
                _in.Close();
            }
            catch (System.Exception e)
            {
                if (_in != null)
                {
                    try
                    {
                        _in.Close();
                    }
                    catch (System.Exception)
                    {
                        //Lin.Core.Controls.TaskbarNotifierUtil.Show(new AdException(-0x20199999));
                    }
                }
                if (Fault != null)
                {
                    Error error = new Error();
                    error.code = -2000022;
                    error.message = e.Message;
                    //error.stackTrace = Lin.Core.Utils.ExceptionInfoToString.ADExceptionToString(e);
                    Fault(error);
                }
                return;
            }

            if (Result != null)
            {
                this.Result(f, start, end, total);
            }
        }

        private string file;
        private Action<long, long> progress;

        public void Download(HttpCommunicateImpl impl, string file, Action<File, long, long, long> result, Action<Error> fault, long p
           , Action<long, long> progress)
        {
            Uri uri;
            if (DownloadUriString.IndexOf('?') == -1)
            {
                uri = new Uri(DownloadUriString + "?key=" + file + "&_time_stamp_" + DateTime.Now.Ticks + "=1");
            }
            else
            {
                uri = new Uri(DownloadUriString + "&key=" + file + "&_time_stamp_" + DateTime.Now.Ticks + "=1");
            }
            Download(impl,uri, result, fault, p, progress);
        }
        
        public void Download(HttpCommunicateImpl impl,Uri file, Action<File, long, long, long> result, Action<Error> fault, long p
            , Action<long, long> progress )
        {
            this.progress = progress;
            this.Result = result;
            this.Fault = fault;
            //this.file = file;

            try
            {
                //String uriString = uri.AbsoluteUri;
                //给uri加一个时间戳，停止浏览器缓存
                //采用_time_stamp_" + DateTime.Now.Ticks做为参数名，防止url中有_time_stamp_参数名，
                //Uri uri;
                //if (DownloadUriString.IndexOf('?') == -1)
                //{
                //    uri = new Uri(DownloadUriString + "?_time_stamp_" + DateTime.Now.Ticks + "=1");
                //}
                //else
                //{
                //    uri = new Uri(DownloadUriString + "&_time_stamp_" + DateTime.Now.Ticks + "=1");
                //}
                //uri = new Uri(UploadUriString);

                request = (HttpWebRequest)WebRequest.Create(file);
                request.CookieContainer = impl.Cookies;
                request.Method = "POST";//以POST方式传递数据
                request.ContentType = "application/x-www-form-urlencoded";
                request.AddRange(p);
                //request.AddRange(p, p + 400 * 1024);
                request.BeginGetRequestStream(new AsyncCallback(SetParams), request);
            }
            catch (System.Exception e)
            {
                //如果有异常，表明此次请求失败
                if (Fault != null)
                {
                    //_syncContext.Post((object target) =>
                    //{

                    Error error = new Error();
                    error.code = -2000022;
                    error.message = e.Message;
                    //error.stackTrace = Lin.Core.Utils.ExceptionInfoToString.ADExceptionToString(e);
                        Fault(error);
                    //}, null);
                }
            }
        }
        HttpWebRequest request = null;
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
                Stream stream = request.EndGetRequestStream(asynchronousResult);
                //StreamWriter _out = new StreamWriter(stream);
                //_out.Write("key=" + file);
                //_out.Close();
                request.BeginGetResponse(new AsyncCallback(RespCallback), request);
            }
            catch (System.Exception e)
            {
                //如果有异常，表明此次请求失败
                if (Fault != null)
                {
                    //_syncContext.Post((object target) =>
                    //{
                    Error error = new Error();
                    error.code = -2000022;
                    error.message = e.Message;
                    //error.stackTrace = Lin.Core.Utils.ExceptionInfoToString.ADExceptionToString(e);
                    Fault(error);
                    //}, null);
                }
            }
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
