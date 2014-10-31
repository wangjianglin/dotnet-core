using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading;
using System.Security.Cryptography;

namespace Lin.Comm.Http
{
    public class HttpPartUpload
    {
        private Action<Error> Fault;
        private Action<string> Result;
        private FileInfo file;
        private string md5;
        private Action<long, long> uploadProgressAction = null;

        public void Upload(HttpCommunicateImpl impl,FileInfo file, Action<object> result, Action<Error> fault,
            Action<long, long> progress = null)
        {
            this.progress = progress;
            this.Result = result;
            this.Fault = fault;
            this.file = file;
            md5 = getMD5Hash(file.FullName);
            long total = file.Length;
            
            //一次上传数据大小，以字节为单位
            long step = 1024 * 100;
            long start = 0;
            long end = start + step - 1;
            //先查询 对应 md5值的文件上传情况，然后根据上传的情况  上传还没有上传的数据
            int retryCount = 0;

            long a = 0;

            object lockObj = new object();
            uploadProgressAction = (long a0, long b) =>
            {
                lock (lockObj)
                {
                    a = a0;
                }
            };

            bool isRun = true;
            bool isStop = false;
            bool isError = false;//表示上传是否有错误，true表示有错误，false表示测试错误
            System.Threading.Thread thread = new System.Threading.Thread(new ParameterizedThreadStart(obj =>
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
                                cp = start + a;
                            }
                            try
                            {
                                if (preP != cp)
                                {
                                    this.progress(cp, total);
                                    preP = cp;
                                }
                            }
                            catch (Exception)
                            {
                            }
                        }
                        catch (Exception) { }
                        System.Threading.Thread.Sleep(50);
                    }
                    try
                    {
                        if (preP != total && !isError)
                        {
                            this.progress(total, total);
                        }
                    }
                    catch (Exception )
                    {
                    }
                }
                isStop = true;
            }));
            thread.IsBackground = true;
            thread.Start();

            while (start < total)
            {
                while (!UploadImpl(impl,md5, start, end, total))
                {
                    retryCount++;
                    if (retryCount > 10 || error.code == HttpCommunicateResult.ABORT_CODE)
                    {
                        isError = true;
                        isRun = false;
                        while (!isStop) { System.Threading.Thread.Sleep(1); }
                        if (fault != null)
                        {
                            fault(error);
                        }
                        return;
                    }
                    System.Threading.Thread.Sleep(1000);
                }
                retryCount = 0;
                start = start + step;
                end = start + step - 1;
                //uploadProgressAction(0, 0);
                if (!this.resultObj.ToString().StartsWith("exile list:"))
                {
                    break;
                }
            }
            //result  startWidth
            //uploadProgressAction(total, total);
            
            isRun = false;
            while (!isStop) { System.Threading.Thread.Sleep(1); }
            if (result != null)
            {
                result(this.resultObj);
            }
            //(new HttpUpload()).Upload(file, resultObject => { result(resultObject, null); }, fault);
        }

        private string resultObj;
        private Error error;
        private Action<long, long> progress;

        private HttpUpload upload;
        private bool isAbort = false;
        //private object abortObject = new object();

        private bool UploadImpl(HttpCommunicateImpl impl,string md5, long start, long end, long total)
        {
            lock (this)
            {
                if (isAbort)
                {
                    this.error = new Error(HttpCommunicateResult.ABORT_CODE,"上传操作已被取消");
                    return false;
                }
            }
            long tick = DateTime.Now.Ticks;
            (new DirectoryInfo(Path.GetTempPath() + "tmp_" + tick + "_" + file.Name)).Create();
            FileInfo tmpFile = new FileInfo(Path.GetTempPath() + "tmp_" + tick + "_" + file.Name + "\\" + file.Name);
            FileStream _in = file.OpenRead();
            FileStream _out = tmpFile.Create();
            _in.Seek(start, SeekOrigin.Begin);
            byte[] bs = new byte[end - start + 1];
            _out.Write(bs, 0, _in.Read(bs, 0, bs.Length));
            _out.Close();
            _in.Close();
            AutoResetEvent are = new AutoResetEvent(false);
            bool isSuccess = true;
            lock (this)
            {
                if (isAbort)
                {
                    this.error = new Error(0x2001000, "上传操作已被取消");
                    return false;
                }
                upload = new HttpUpload();
            }
            lock (this)
            {
                upload.Upload(impl,tmpFile, resultObject =>
                {
                    resultObj = resultObject;
                    are.Set();
                }, error =>
                {
                    isSuccess = false;
                    this.error = error;
                    are.Set();
                }, md5, start, end, total, uploadProgressAction);
            }
            are.WaitOne();
            try
            {
                tmpFile.Delete();
                tmpFile.Directory.Delete();
            }
            catch (Exception) { }
            lock (this)
            {
                if (isAbort)
                {
                    this.error = new Error(HttpCommunicateResult.ABORT_CODE, "上传操作已被取消");
                    return false;
                }
            }
            return isSuccess;
        }

        public void Abort()
        {
            lock (this)
            {
                isAbort = true;
                if (upload != null)
                {
                    upload.Abort();
                }
            }
        }
        private string getMD5Hash(string pathName)
        {

            string strResult = "";

            string strHashData = "";



            byte[] arrbytHashValue;

            System.IO.FileStream oFileStream = null;



            System.Security.Cryptography.MD5CryptoServiceProvider oMD5Hasher =

                       new System.Security.Cryptography.MD5CryptoServiceProvider();



            try
            {

                oFileStream = new System.IO.FileStream(pathName, System.IO.FileMode.Open, System.IO.FileAccess.Read, System.IO.FileShare.ReadWrite);

                arrbytHashValue = oMD5Hasher.ComputeHash(oFileStream);//计算指定Stream 对象的哈希值

                oFileStream.Close();

                //由以连字符分隔的十六进制对构成的String，其中每一对表示value 中对应的元素；例如“F-2C-4A”

                strHashData = System.BitConverter.ToString(arrbytHashValue);

                //替换-

                strHashData = strHashData.Replace("-", "");

                strResult = strHashData;

            }

            catch (System.Exception)
            {

                //MessageBox.Show(ex.Message);

            }



            return strResult;

        }


        
    }
}
