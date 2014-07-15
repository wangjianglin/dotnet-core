using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.IO;

namespace Lin.Core.Web.Http
{
    public class HttpPartDownload
    {
           private byte[] buffer = new Byte[4096];
        private void FileCopy(FileInfo src, FileStream dest)
        {
            FileStream _in = src.OpenRead();
            int bytesRead = 0;
            while ((bytesRead = _in.Read(buffer, 0, buffer.Length)) != 0)
            {
                dest.Write(buffer, 0, bytesRead);
            }
            dest.Flush();
            _in.Close();
        }

        private Action<long, long> downloadProgressAction = null;

        public void Download(HttpCommunicateImpl impl,object file, Action<File> result, Action<Error> fault,
            Action<long, long> progress)
        {

            long p = 0;
            long a = 0;
            long total = 0;
            object lockObj = new object();
            downloadProgressAction = (long a0, long b) =>
            {
                lock (lockObj)
                {
                    a = a0;
                }
                total = b;
            };
            bool isRun = true;
            bool isStop = false;
            bool isError = false;//表示上传是否有错误，true表示有错误，false表示测试错误
            Thread thread = new Thread(new ParameterizedThreadStart(obj =>
            {
                long preP = 0;
                long cp = 0;
                if (progress != null)
                {
                    while (isRun)
                    {
                        try
                        {
                            lock (lockObj)
                            {
                                cp = p + a;
                            }
                            if (preP != cp)
                            {
                                progress(cp, total);
                                preP = cp;
                            }
                        }
                        catch (Exception e)
                        {
                        }
                        Thread.Sleep(50);
                    }

                    if (preP != total && progress != null && !isError)
                    {
                        try
                        {
                            progress(total, total);
                        }
                        catch (Exception e)
                        {
                        }
                    }
                }
                isStop = true;
            }));
            thread.IsBackground = true;
            thread.Start();

            int retryCount = 0;
            FileInfo fileInfo = null;// new FileInfo(Lin.Core.ViewModel.Context.Cache.TmpDir.FullName + "\\download_mereg_tmp_" + DateTime.Now.Ticks + "_file_name.tmp");
            FileStream _out = null;// fileInfo.Create();
            while (true)
            {
                retryCount = 0;
                while (!DonwloadImpl(impl,file, p))
                {
                    if (retryCount++ > 10 || error.code == HttpCommunicateResult.ABORT_CODE)
                    {
                        isRun = false;
                        isError = true;
                        while (!isStop) { Thread.Sleep(1); }
                        if (_out != null)
                        {
                            _out.Close();
                        }
                        if (fault != null)
                        {
                            //Error e = new Error();
                            fault(error);
                        }
                        return;
                    }
                    Thread.Sleep(100);
                }
                retryCount = 0;
                if (_out == null)
                {
                    fileInfo = new FileInfo(Lin.Core.ViewModel.Context.Cache.TmpDir.FullName + "\\download_mereg_tmp_" + DateTime.Now.Ticks + "_file_name_tmp_" + resultFile.FileName);
                    _out = fileInfo.Create();
                }
                FileCopy(resultFile.FileInfo, _out);
                resultFile.FileInfo.Delete();
                lock (lockObj)
                {
                    a = 0;
                    p = end + 1;
                }
                if (p >= total - 1)
                {
                    break;
                }
            }
            isRun = false;
            while (!isStop) { Thread.Sleep(1); }
            if (_out != null)
            {
                //_out.Flush();
                //_out.Dispose();
                _out.Close();
            }

            resultFile.FileInfo = fileInfo;
            if (result != null)
            {
                result(resultFile);
            }
        }

        private File resultFile;
        private Error error;
        private long start;
        private long end;
        //private long total;

        private bool DonwloadImpl(HttpCommunicateImpl impl,object file, long p)
        {
            lock (this)
            {
                if (isAbort)
                {
                    this.error = new Error(HttpCommunicateResult.ABORT_CODE, "上传操作已被取消");
                    return false;
                }
            }
            AutoResetEvent are = new AutoResetEvent(false);
            bool flag = false;
            Uri uri = file as Uri;
            lock (this)
            {
                if (isAbort)
                {
                    this.error = new Error(0x2001000, "上传操作已被取消");
                    return false;
                }
                download = new HttpDownload();
            }
            if (uri != null)
            {
                lock (this)
                {
                    download.Download(impl,uri, (resultObject, start, end, total) =>
                    {
                        this.start = start;
                        this.end = end;
                        //this.total = total;
                        resultFile = resultObject;
                        //tmpWarning = null;
                        flag = true;
                        are.Set();
                    }, error =>
                    {
                        this.error = error;
                        are.Set();
                    }, p, downloadProgressAction);
                }
            }
            else
            {
                lock (this)
                {
                    download.Download(impl,file as string, (resultObject, start, end, total) =>
                    {
                        this.start = start;
                        this.end = end;
                        //this.total = total;
                        resultFile = resultObject;
                        //tmpWarning = null;
                        flag = true;
                        are.Set();
                    }, error =>
                    {
                        this.error = error;
                        are.Set();
                    }, p, downloadProgressAction);
                }
            }
            are.WaitOne();
            lock (this)
            {
                if (isAbort)
                {
                    this.error = new Error(HttpCommunicateResult.ABORT_CODE, "上传操作已被取消");
                    return false;
                }
            }
            return flag;
        }

        private HttpDownload download;
        private bool isAbort = false;
        //private object abortObject = new object();
        internal void Abort()
        {
            lock (this)
            {
                isAbort = true;
                if (download != null)
                {
                    download.Abort();
                }
            }
        }
    }
}
