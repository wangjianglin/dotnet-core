using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Permissions;
using System.Text;
using System.Threading;

namespace Lin.Core.Cache
{
    [Serializable]
    public class CacheProxy : ICache
    {
        /// <summary>
        /// 目录
        /// </summary>
        private DirectoryInfo dir;
        public DirectoryInfo Dir
        {
            get { return this.dir; }
            private set
            {
                this.dir = value;
            }
        }

        /// <summary>
        /// 用来存放临时文件的目录
        /// </summary>
        private DirectoryInfo _tmpDir;
        public DirectoryInfo TmpDir
        {
            get
            {
                _tmpDir = Cache.TmpDir;
                return _tmpDir;
            }
        }

        public CacheProxy(System.IO.DirectoryInfo dir)
        {
            if (!Directory.Exists(dir.FullName))
            {
                DirectoryInfo directory = Directory.CreateDirectory(dir.FullName);
                this.dir = directory;
            }
            this.dir = dir;
        }


        /// <summary>
        /// 写入Object对象
        /// </summary>
        /// <param name="obj">对象</param>
        /// <param name="key">源键</param>
        /// <returns>产生的新键</returns>
        private readonly string putObj = "PutObj_ObjKey_" + DateTime.Now.Ticks;
        private readonly string keyValue = "PutObj_Key_" + DateTime.Now.Ticks;
        private readonly string isPermanencyKey = "PutObj_IsPermanencyKey_" + DateTime.Now.Ticks;
        private readonly string exceptionObjKey = "PutObj_Exception_Key_" + DateTime.Now.Ticks;
        public string Put(object obj, string key = null, bool isPermanency = false, bool isCrossDomain = false)
        {
            if (isCrossDomain)
            {
                string friendlyName = AppDomain.CurrentDomain.FriendlyName;

                AppDomain appDomain = GetAppDomain.GetSingleDomainAndCreate();
                appDomain.SetData(putObj, obj);
                appDomain.SetData(keyValue, key);
                appDomain.SetData(isPermanencyKey, isPermanency);
                appDomain.DoCallBack(() =>
                {
                    try
                    {
                        object obj1 = AppDomain.CurrentDomain.GetData(putObj);
                        object obj3 = AppDomain.CurrentDomain.GetData(isPermanencyKey);
                        string key1 = null;
                        if (AppDomain.CurrentDomain.GetData(keyValue) != null)
                        {
                            key1 = AppDomain.CurrentDomain.GetData(keyValue).ToString();
                        }
                        if (obj1 != null && obj3 != null)
                        {
                            bool isPermanency1 = Convert.ToBoolean(obj3);
                            ICache cache = Cache.GetCache(dir.FullName);
                            key1 = cache.Put(obj1, key1, isPermanency1);
                        }
                        AppDomain.CurrentDomain.SetData(keyValue, key1);
                    }
                    catch (Exception ex)
                    {
                        AppDomain.CurrentDomain.SetData(exceptionObjKey, ex.ToString());
                    }
                });
                if (appDomain.GetData(keyValue) != null)
                {
                    key = appDomain.GetData(keyValue).ToString();
                }
                if (appDomain.GetData(exceptionObjKey) != null)
                {
                    Lin.Core.Controls.TaskbarNotifierPopupUtil.Show(appDomain.GetData(exceptionObjKey), Log.LogLevel.INFO, "温馨提示");
                }
                appDomain.SetData(putObj, null);
                appDomain.SetData(keyValue, null);
                appDomain.SetData(isPermanencyKey, null);
                return key;
            }
            else
            {
                ICache cache = Cache.GetCache(dir.FullName);
                string key1 = cache.Put(obj, key, isPermanency);
                return key1;
            }
        }

        /// <summary>
        /// 写入文件信息
        /// </summary>_
        /// <param name="file">文件对象</param>
        /// <param name="key">传过来的键</param>
        /// <returns>新的键</returns>
        private readonly string putfile = "PutFile_FileInfo_" + DateTime.Now.Ticks;
        private readonly string fileKey = "PutFile_Key_" + DateTime.Now.Ticks;
        private readonly string fileIsPermanencyKey = "PutFile_IsPermanency_" + DateTime.Now.Ticks;
        private readonly string exceptionFileKey = "PutFile_Exception_Key_" + DateTime.Now.Ticks;
        public string Put(System.IO.FileInfo file, string key = null, bool isPermanency = false)
        {
            AppDomain appDomain = GetAppDomain.GetSingleDomainAndCreate();
            appDomain.SetData(putfile, file.FullName);
            appDomain.SetData(fileKey, key);
            appDomain.SetData(fileIsPermanencyKey, isPermanency);
            appDomain.DoCallBack(() =>
            {
                try
                {
                    object obj1 = AppDomain.CurrentDomain.GetData(putfile);
                    object obj3 = AppDomain.CurrentDomain.GetData(fileIsPermanencyKey);
                    string key1 = null;
                    if (AppDomain.CurrentDomain.GetData(fileKey) != null)
                    {
                        key1 = AppDomain.CurrentDomain.GetData(fileKey).ToString();
                    }
                    if (obj1 != null && obj3 != null)
                    {
                        FileInfo file1 = new FileInfo(obj1.ToString());
                        bool isPermanency1 = Convert.ToBoolean(obj3);
                        ICache cache = Cache.GetCache(dir.FullName);
                        key1 = cache.Put(file1, key1, isPermanency1);
                    }
                    AppDomain.CurrentDomain.SetData(fileKey, key1);
                }
                catch (Exception e)
                {
                    AppDomain.CurrentDomain.SetData(exceptionFileKey, e.ToString());
                }
            });
            if (appDomain.GetData(fileKey) != null)
            {
                key = appDomain.GetData(fileKey).ToString();
            }
            if (appDomain.GetData(exceptionFileKey) != null)
            {
                Lin.Core.Controls.TaskbarNotifierPopupUtil.Show(appDomain.GetData(exceptionFileKey), Log.LogLevel.INFO, "温馨提示");
            }
            appDomain.SetData(putfile, null);
            appDomain.SetData(fileKey, null);
            appDomain.SetData(fileIsPermanencyKey, null);

            return key;
        }

        /// <summary>
        /// 写入文件夹
        /// </summary>
        /// <param name="directory">文件目录</param>
        /// <param name="key">传过来的键</param>
        /// <param name="isPermanency">是否永久缓存</param>
        /// <returns>返回服务器上文件夹相对应的Key值</returns>
        private readonly string putDirectory = "PutDirectory_DirectoryInfo_" + DateTime.Now.Ticks;
        private readonly string directoryKey = "PutDirectory_Key_" + DateTime.Now.Ticks;
        private readonly string directoryIsPermanencyKey = "PutDirectory_IsPermanency_" + DateTime.Now.Ticks;
        private readonly string exceptionDirKey = "PutDirectory_Exception_Key_" + DateTime.Now.Ticks;
        public string Put(System.IO.DirectoryInfo directory, string key = null, bool isPermanency = false, bool isRecursive = false)
        {
            AppDomain appDomain = GetAppDomain.GetSingleDomainAndCreate();
            appDomain.SetData(putDirectory, directory.FullName);
            appDomain.SetData(directoryKey, key);
            appDomain.SetData(directoryIsPermanencyKey, isPermanency);
            appDomain.DoCallBack(() =>
            {
                try
                {
                    object obj1 = AppDomain.CurrentDomain.GetData(putDirectory);
                    object obj3 = AppDomain.CurrentDomain.GetData(directoryIsPermanencyKey);
                    string key1 = null;
                    if (AppDomain.CurrentDomain.GetData(directoryKey) != null)
                    {
                        key1 = AppDomain.CurrentDomain.GetData(directoryKey).ToString();
                    }
                    if (obj1 != null && obj3 != null)
                    {
                        DirectoryInfo directory1 = new DirectoryInfo(obj1.ToString());
                        bool isPermanency1 = Convert.ToBoolean(obj3);
                        ICache cache = Cache.GetCache(dir.FullName);
                        key1 = cache.Put(directory1, key1, isPermanency1);
                    }
                    AppDomain.CurrentDomain.SetData(directoryKey, key1);
                }
                catch (Exception e)
                {
                    AppDomain.CurrentDomain.SetData(exceptionDirKey, e.ToString());
                }
            });
            if (appDomain.GetData(directoryKey) != null)
            {
                key = appDomain.GetData(directoryKey).ToString();
            }
            if (appDomain.GetData(exceptionDirKey) != null)
            {
                Lin.Core.Controls.TaskbarNotifierPopupUtil.Show(appDomain.GetData(exceptionDirKey), Log.LogLevel.INFO, "温馨提示");
            }
            appDomain.SetData(putDirectory, null);
            appDomain.SetData(directoryKey, null);
            appDomain.SetData(directoryIsPermanencyKey, null);

            return key;
        }

        /// <summary>
        /// 通过Key查找相对应的文件
        /// </summary>
        /// <param name="key">键</param>
        /// <returns></returns>
        private readonly string getKey = "GetCache_Key_" + DateTime.Now.Ticks;
        private readonly string objValueKey = "GetObjValue_Key_" + DateTime.Now.Ticks;
        public object Get(string key)
        {
            AppDomain appDomain = GetAppDomain.GetSingleDomainAndCreate();
            appDomain.SetData(getKey, key);
            object obj = null;
            appDomain.DoCallBack(() =>
            {
                string key1 = AppDomain.CurrentDomain.GetData(getKey).ToString();
                ICache cache = Cache.GetCache(dir.FullName);
                AppDomain.CurrentDomain.SetData(objValueKey, cache.Get(key1));
            });
            if (appDomain.GetData(objValueKey) != null)
            {
                obj = appDomain.GetData(objValueKey);
                if (obj is FileInfo)
                {
                    obj = new FileInfo((obj as FileInfo).FullName);
                }
                if (obj is DirectoryInfo)
                {
                    obj = new DirectoryInfo((obj as DirectoryInfo).FullName);
                }
            }
            appDomain.SetData(getKey, null);
            appDomain.SetData(objValueKey, null);

            return obj;
        }

        /// <summary>
        /// 删除指定的Key所对应的文件(文件夹以及数据库中的数据)
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        private readonly string removeKey = "RemoveCache_Key_" + DateTime.Now.Ticks;
        public void Remove(string key)
        {
            AppDomain appDomain = GetAppDomain.GetSingleDomainAndCreate();
            appDomain.SetData(removeKey, key);
            appDomain.DoCallBack(() =>
            {
                string key1 = AppDomain.CurrentDomain.GetData(removeKey).ToString();
                ICache cache = Cache.GetCache(dir.FullName);
                cache.Remove(key1);
            });
            appDomain.SetData(removeKey, null);
        }

        /// <summary>
        /// 删除路径下比数据库中存储多余的文件以及文件夹
        /// </summary>
        public void DeleteRemain()
        {
            AppDomain appDomain = GetAppDomain.GetSingleDomainAndCreate();
            appDomain.DoCallBack(() =>
            {
                ICache cache = Cache.GetCache(dir.FullName);
                cache.DeleteRemain();
            });
        }

        /// <summary>
        /// 用户更改缓存目录后，将之前缓存目录下的内容移动到新的缓存目录
        /// </summary>
        /// <param name="directorySource">旧目录</param>
        /// <param name="directoryTarget">新的缓存目录</param>
        private readonly string directorySourceKey = "MoveFolderTo_SourceKey_" + DateTime.Now.Ticks;
        private readonly string directoryTargetKey = "MoveFolderTo_TargetKey_" + DateTime.Now.Ticks;
        public void MoveFolderTo(DirectoryInfo directorySource, DirectoryInfo directoryTarget)
        {
            AppDomain appDomain = GetAppDomain.GetSingleDomainAndCreate();
            appDomain.SetData(System.Threading.Thread.CurrentThread.ManagedThreadId + directorySourceKey, directorySource);
            appDomain.SetData(System.Threading.Thread.CurrentThread.ManagedThreadId + directoryTargetKey, directoryTarget);
            //System.Threading.Thread.Sleep(1000);
            appDomain.DoCallBack(() =>
            {
                object obj = AppDomain.CurrentDomain.GetData(System.Threading.Thread.CurrentThread.ManagedThreadId + directorySourceKey);
                object obj1 = AppDomain.CurrentDomain.GetData(System.Threading.Thread.CurrentThread.ManagedThreadId + directoryTargetKey);
                if (obj != null && obj1 != null)
                {
                    DirectoryInfo directorySource1 = obj as DirectoryInfo;
                    DirectoryInfo directoryTarget1 = obj1 as DirectoryInfo;
                    ICache cache = Cache.GetCache(dir.FullName);
                    cache.MoveFolderTo(directorySource1, directoryTarget1);
                }
            });
            appDomain.SetData(directorySourceKey, null);
            appDomain.SetData(directoryTargetKey, null);
        }

        /// <summary>
        /// 当缓存文件夹的大小已经超过规定大小时，淘汰最旧不是永久的缓存文件
        /// </summary>
        /// <param name="directorySize">缓存目录大小</param>
        private readonly string directorySizeKey = "EliminationSpilthFile_Key_" + DateTime.Now.Ticks;
        public void EliminationSpilthFile(long directorySize)
        {
            AppDomain appDomain = GetAppDomain.GetSingleDomainAndCreate();
            appDomain.SetData(directorySizeKey, directorySize);
            appDomain.DoCallBack(() =>
            {
                object obj = AppDomain.CurrentDomain.GetData(directorySizeKey);
                if (obj != null)
                {
                    long directorySize1 = Convert.ToInt64(obj);
                    ICache cache = Cache.GetCache(dir.FullName);
                    cache.EliminationSpilthFile(directorySize1);
                }
            });

            appDomain.SetData(directorySizeKey, null);
        }

        /// <summary>
        /// 定时清理缓存
        /// </summary>
        /// <param name="timeSpan"></param>
        /// <param name="size"></param>
        private readonly string timeSpanKey = "TimelyClearCache_TimeSpanKey_" + DateTime.Now.Ticks;
        private readonly string sizeKey = "TimelyClearCache_SizeKey_" + DateTime.Now.Ticks;
        public void TimelyClearCache(ulong timeSpan, long size)
        {
            AppDomain appDomain = GetAppDomain.GetSingleDomainAndCreate();
            appDomain.SetData(timeSpanKey, timeSpan);
            appDomain.SetData(sizeKey, size);
            appDomain.DoCallBack(() =>
            {
                object obj1 = AppDomain.CurrentDomain.GetData(timeSpanKey);
                object obj = AppDomain.CurrentDomain.GetData(sizeKey);
                if (obj != null && obj1 != null)
                {
                    long size1 = Convert.ToInt64(obj);
                    ulong timeSpan1 = Convert.ToUInt64(obj1);
                    ICache cache = Cache.GetCache(dir.FullName);
                    cache.TimelyClearCache(timeSpan1, size1);
                }
            });

            appDomain.SetData(timeSpanKey, null);
            appDomain.SetData(sizeKey, null);
        }
    }
}
