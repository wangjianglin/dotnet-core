using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
//using System.Runtime.Serialization.Json;
using Lin.Core.Web.Packages;
using System.Threading;
using System.Net;
using System.ComponentModel;
using System.Diagnostics;
using Lin.Plugin;
using Lin.Core.Utils;
using Lin.Util;

namespace Lin.Core.Web.Http
{
    /// <summary>
    /// HTTP请求类型
    /// </summary>
    public enum HttpCommunicateType
    {
        Request,//数据包请求
        Upload,//文件上传
        Download//文件下载
    }
    /// <summary>
    /// 实现HTTP请求，包括数据、文件上传与下载
    /// 
    /// 可能通过调用get方法，建立不同的通信通道，用名称区分不同的通道
    /// </summary>
    public static class HttpCommunicate
    {
        /// <summary>
        /// 默认的代理，系统默认采用的是IE设置
        /// </summary>
        private static readonly IWebProxy _default = WebRequest.DefaultWebProxy;

        /// <summary>
        /// 得到IE的代理对象
        /// </summary>
        public static IWebProxy IEProxy { get { return _default; } }

        /// <summary>
        /// 记录通信通道
        /// </summary>
        private static IDictionary<string,HttpCommunicateImpl> impls = new Dictionary<string, HttpCommunicateImpl>();

        public static ReadOnlyIndexProperty<string, HttpCommunicateImpl> Https
        {
            get { return _https; }
        }
        /// <summary>
        /// 获取一个指定名称的通信实现
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        private static ReadOnlyIndexProperty<string, HttpCommunicateImpl> _https = new ReadOnlyIndexProperty<string, HttpCommunicateImpl>(name =>
        {
            if (impls.ContainsKey(name))
            {
                return impls[name];
            }

            lock (impls)
            {
                HttpCommunicateImpl impl = null;
                if (impls.ContainsKey(name))
                {
                    impl = impls[name];
                }
                else
                {        
                    impl = new HttpCommunicateImpl(name);

                    //添加HTTP请求监听事件
                    impl.HttpRequest += (HttpCommunicateType arg1, object arg2) =>
                    {
                        if (GlobalHttpRequest != null)
                        {
                            GlobalHttpRequest(arg1, arg2);
                        }
                    };
                    impl.HttpRequestComplete += (HttpCommunicateType arg1, object arg2, object arg3, IList<Error> arg4) =>
                    {
                        if (GlobalHttpRequestComplete != null)
                        {
                            GlobalHttpRequestComplete(arg1, arg2, arg3, arg4);
                        }
                    };

                    impl.HttpRequestFault += (HttpCommunicateType arg1, object arg2, Error arg3) =>
                    {
                        if (GlobalHttpRequestFault != null)
                        {
                            GlobalHttpRequestFault(arg1, arg2, arg3);
                        }
                    };
                    impls.Add(name, impl);
                }
                return impl;
            }
        });

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        public static void Remove(string name)
        {
            lock (impls)
            {
                if (impls.ContainsKey(name))
                {
                    impls.Remove(name);
                }
            }
        }

        /// <summary>
        /// 默认通信
        /// </summary>
	private readonly static HttpCommunicateImpl global = Https["Global"];

        /// <summary>
        /// 默认通信
        /// </summary>
    public static HttpCommunicateImpl Global { get { return global; } }

        /// <summary>
        /// 通信地址
        /// </summary>
        public static Uri CommUri
        {
            get { return global.CommUri; }
            set { global.CommUri = value; }
        }


        /// <summary>
        /// 设置代理 
        ///                WebProxy wproxy = new WebProxy();
        ///                wproxy.Address = newUri;
        ///                wproxy.Address = newUri;wproxy.Credentials = new NetworkCredential(username, password, domain);
        /// </summary>
        public static IWebProxy Proxy
        {
            set { global.Proxy = value; }
            get { return global.Proxy; }
        }

        /// <summary>
        /// 开始请求
        /// </summary>
        public static event System.Action<HttpCommunicateType, object> HttpRequest
        {
            add { global.HttpRequest += value; }
            remove { global.HttpRequest -= value; }
        }

        /// <summary>
        /// 对全部HTTP开始请求的事件
        /// </summary>
        public static event System.Action<HttpCommunicateType, object> GlobalHttpRequest;

        /// <summary>
        /// 请求完成
        /// </summary>
        public static event System.Action<HttpCommunicateType, object, object, IList<Error>> HttpRequestComplete
        {
            add { global.HttpRequestComplete += value; }
            remove { global.HttpRequestComplete -= value; }
        }
        /// <summary>
        /// 用于保存Session会话
        /// </summary>
        public static CookieContainer Cookies
        {
            get { return global.Cookies; }
        }
        /// <summary>
        /// 对全部HTTP请求完成的事件
        /// </summary>
        public static event System.Action<HttpCommunicateType, object, object, IList<Error>> GlobalHttpRequestComplete;

        /// <summary>
        /// 请求错误
        /// </summary>
        public static event System.Action<HttpCommunicateType, object, Error> HttpRequestFault
        {
            add { global.HttpRequestFault += value; }
            remove { global.HttpRequestFault -= value; }
        }

          /// <summary>
        /// 对全部HTTP请求错误的事件
        /// </summary>
        public static event System.Action<HttpCommunicateType, object, Error> GlobalHttpRequestFault;

        static HttpCommunicate()
        {
        }

        /// <summary>
        /// 数据包请求
        /// </summary>
        /// <param name="package"></param>
        /// <param name="result"></param>
        /// <param name="fault"></param>
        public static HttpCommunicateResult Request(Package package, Action<object, IList<Error>> result = null, Action<Error> fault = null)
        {
            return global.Request(package, result, fault);
        }

        /// <summary>
        /// 上传文件
        /// </summary>
        /// <param name="file">需要上传的文件</param>
        /// <param name="result"></param>
        /// <param name="fault"></param>
        /// <param name="progress"></param>
        /// <returns></returns>
        public static HttpCommunicateResult Upload(FileInfo file, Action<object, IList<Error>> result = null,
        Action<Error> fault = null, Action<long, long> progress = null)
        {
            return global.Upload(file, result, fault, progress);
        }

        /// <summary>
        /// 根据 key 下载文件
        /// </summary>
        /// <param name="file"></param>
        /// <param name="result"></param>
        /// <param name="fault"></param>
        /// <param name="progress"></param>
        /// <returns></returns>
        public static HttpCommunicateResult Download(string file, Action<File, IList<Error>> result = null, Action<Error> fault = null,
    Action<long, long> progress = null)
        {
            return global.Download(file, result, fault, progress);
        }

        /// <summary>
        /// 根据Uri下载文件
        /// </summary>
        /// <param name="file"></param>
        /// <param name="result"></param>
        /// <param name="fault"></param>
        /// <param name="progress"></param>
        /// <returns></returns>
        public static HttpCommunicateResult Download(Uri file, Action<File, IList<Error>> result = null, Action<Error> fault = null,
         Action<long, long> progress = null)
        {
            return global.Download(file, result, fault, progress);
        }

    }
    /// <summary>
    /// 实现HTTP通信
    /// </summary>
    public class HttpCommunicateImpl{

        internal HttpCommunicateImpl(string name){this.Name = name;}
        //internal static string CommUriString { get; set; }
        public string Name { get; private set; }
        private Uri _commUri;
        /// <summary>
        /// 远程通信服务器地址
        /// </summary>
        public Uri CommUri
        {
            get { return _commUri; }
            set { _commUri = value; }
        }

        /// <summary>
        /// 设置代理
        /// </summary>
        public IWebProxy Proxy
        {
            set { WebRequest.DefaultWebProxy = value; }
            get { return WebRequest.DefaultWebProxy; }
        }
        
        //cookie对象
        private CookieContainer _cookies = new CookieContainer();
        /// <summary>
        /// 用于保存Session会话
        /// </summary>
        public CookieContainer Cookies { get { return _cookies; } }

        ///// <summary>
        ///// 事件同步锁
        ///// </summary>
        //private object httpEventLock = new object();


        /// <summary>
        /// 开始请求
        /// </summary>
        public event System.Action<HttpCommunicateType, object> HttpRequest
        {
            add { _httpRequest.Add(value); }
            remove { _httpRequest.Remove(value); }
        }
        private WeakReferenceCollection<System.Action<HttpCommunicateType, object>> _httpRequest = new WeakReferenceCollection<System.Action<HttpCommunicateType, object>>();
       
        /// <summary>
        /// 发布开始请求事件
        /// </summary>
        /// <param name="type"></param>
        /// <param name="content"></param>
        private void FireHttpRequest(HttpCommunicateType type, object content)
        {
            _httpRequest.Action(action =>
            {
                action(type, content);
            });
        }


        /// <summary>
        /// 请求完成
        /// </summary>
        public event System.Action<HttpCommunicateType, object, object, IList<Error>> HttpRequestComplete
        {
            add { _httpRequestComplete.Add(value); }
            remove { _httpRequestComplete.Remove(value); }
        }
        private WeakReferenceCollection<System.Action<HttpCommunicateType, object, object, IList<Error>>> _httpRequestComplete = new WeakReferenceCollection<System.Action<HttpCommunicateType, object, object, IList<Error>>>();
       
        /// <summary>
        /// 发布请求完成事件
        /// </summary>
        /// <param name="type"></param>
        /// <param name="content"></param>
        /// <param name="result"></param>
        /// <param name="warning"></param>
        private void FireHttpRequestComplete(HttpCommunicateType type, object content, object result, IList<Error> warning)
        {
            _httpRequestComplete.Action(action =>
            {
                action(type, content, result, warning);
            });
        }

        private WeakReferenceCollection<System.Action<HttpCommunicateType, object, Error>> _httpRequestFault = new WeakReferenceCollection<System.Action<HttpCommunicateType, object, Error>>();
        /// <summary>
        /// 请求错误
        /// </summary>
        public event System.Action<HttpCommunicateType, object, Error> HttpRequestFault
        {
            add { _httpRequestFault.Add(value); }
            remove { _httpRequestFault.Remove(value); }
        }

        /// <summary>
        /// 发布事件
        /// </summary>
        /// <param name="type"></param>
        /// <param name="content"></param>
        /// <param name="error"></param>
        private void FireHttpRequestFault(HttpCommunicateType type, object content, Error error)
        {
            _httpRequestFault.Action(action =>
            {
                action(type, content, error);
            });
        }

      
        /// <summary>
        /// 
        /// </summary>
        /// <param name="package"></param>
        /// <param name="result"></param>
        /// <param name="fault"></param>
        /// <param name="sync">表示是否同步调用，true表示同步调用，false表示异步调用</param>
        //public static HttpCommunicatResult Request(Package package, Action<object, IList<Error>> result = null, Action<Error> fault = null, bool sync = false)
        public HttpCommunicateResult Request(Package package, Action<object, IList<Error>> result = null, Action<Error> fault = null)
        {
            FireHttpRequest(HttpCommunicateType.Request, package);
            AutoResetEvent are = new AutoResetEvent(false);
            HttpRequest request = new Http.HttpRequest();
            HttpCommunicateResult r = new HttpCommunicateResult(are, () =>
            {
                request.Abort();
            });
            request.Request(this,package,
                (rs, warning) =>
                {
                    Lin.Core.Utils.ActionExecute.Execute(() =>
                    {
                        r.Result = true;
                        FireHttpRequestComplete(HttpCommunicateType.Request, package, rs, warning);
                    }, () =>
                    {
                        if (result != null)
                        {
                            result(rs, warning);
                        }
                    }, () =>
                    {
                        are.Set();
                    });
                },
                 e =>
                 {
                     Lin.Core.Utils.ActionExecute.Execute(() =>
                     {
                         r.Result = false;
                         FireHttpRequestFault(HttpCommunicateType.Request, package, e);
                     }, () =>
                     {
                         if (fault != null)
                         {
                             fault(e);
                         }
                     }, () =>
                     {
                         are.Set();
                     });
                 });
            return r;
        }

        /// <summary>
        /// 上传文件
        /// </summary>
        /// <param name="file"></param>
        /// <param name="result"></param>
        /// <param name="fault"></param>
        /// <param name="progress"></param>
        /// <returns></returns>
        public HttpCommunicateResult Upload(FileInfo file, Action<object, IList<Error>> result = null,
        Action<Error> fault = null, Action<long, long> progress = null)
        {
            FireHttpRequest(HttpCommunicateType.Upload, file);
            AutoResetEvent are = new AutoResetEvent(false);
            HttpPartUpload upload = new HttpPartUpload();
            HttpCommunicateResult r = new HttpCommunicateResult(are, () =>
            {
                upload.Abort();
            });
            System.Threading.Thread thread = new System.Threading.Thread(new ParameterizedThreadStart(obj =>
            {
                upload.Upload(this,file, (resultObject) =>
                {
                    Lin.Core.Utils.ActionExecute.Execute(() =>
                    {
                        r.Result = true;
                        FireHttpRequestComplete(HttpCommunicateType.Download, file, resultObject, null);
                    }, () =>
                    {
                        if (result != null)
                        {
                            result(resultObject, null);
                        }
                    }, () =>
                    {
                        are.Set();
                    });
                }, e =>
                {
                    Lin.Core.Utils.ActionExecute.Execute(() =>
                    {
                        r.Result = false;
                        FireHttpRequestFault(HttpCommunicateType.Download, file, new Error());
                    }, () =>
                    {
                        if (fault != null)
                        {
                            fault(e);
                        }
                    }, () =>
                    {
                        are.Set();
                    });

                }, progress);
            }));
            thread.IsBackground = true;
            thread.Start();
            return r;
        }

        /// <summary>
        /// 下载文件
        /// </summary>
        /// <param name="file"></param>
        /// <param name="result"></param>
        /// <param name="fault"></param>
        /// <param name="progress"></param>
        /// <returns></returns>
        public HttpCommunicateResult Download(string file, Action<File, IList<Error>> result = null, Action<Error> fault = null,
    Action<long, long> progress = null)
        {
            HttpPartDownload download = new HttpPartDownload();
            AutoResetEvent are = new AutoResetEvent(false);
            HttpCommunicateResult r = new HttpCommunicateResult(are, () =>
            {
                download.Abort();
            });
            System.Threading.Thread thread = new System.Threading.Thread(new ParameterizedThreadStart(obj =>
            {
                download.Download(this,file, (resultObject) =>
                {
                    Lin.Core.Utils.ActionExecute.Execute(() =>
                    {
                        r.Result = true;
                        FireHttpRequestComplete(HttpCommunicateType.Download, file, resultObject, null);
                    }, () =>
                    {
                        if (result != null)
                        {
                            result(resultObject, null);
                        }
                    }, () =>
                    {
                        are.Set();
                    });
                }, e =>
                {
                    Lin.Core.Utils.ActionExecute.Execute(() =>
                    {
                        r.Result = false;
                        FireHttpRequestFault(HttpCommunicateType.Download, file, new Error());
                    }, () =>
                    {
                        if (fault != null)
                        {
                            fault(e);
                        }
                    }, () =>
                    {
                        are.Set();
                    });
                }, progress);
            }));
            thread.IsBackground = true;
            thread.Start();
            return r;
        }

        /// <summary>
        /// 下载文件
        /// </summary>
        /// <param name="file"></param>
        /// <param name="result"></param>
        /// <param name="fault"></param>
        /// <param name="progress"></param>
        /// <returns></returns>
        public HttpCommunicateResult Download(Uri file, Action<File, IList<Error>> result = null, Action<Error> fault = null,
         Action<long, long> progress = null)
        {
            FireHttpRequest(HttpCommunicateType.Download, file);
            AutoResetEvent are = new AutoResetEvent(false);
            HttpPartDownload download = new HttpPartDownload();
            HttpCommunicateResult r = new HttpCommunicateResult(are, () =>
            {
                download.Abort();
            });
            System.Threading.Thread thread = new System.Threading.Thread(new ParameterizedThreadStart(obj =>
            {
                download.Download(this,file, (resultObject) =>
                {
                    Lin.Core.Utils.ActionExecute.Execute(() =>
                    {
                        r.Result = true;
                        FireHttpRequestComplete(HttpCommunicateType.Download, file, resultObject, null);
                    }, () =>
                    {
                        if (result != null)
                        {
                            result(resultObject, null);
                        }
                    }, () =>
                    {
                        are.Set();
                    });
                }, e =>
                {
                    Lin.Core.Utils.ActionExecute.Execute(() =>
                    {
                        r.Result = false;
                        FireHttpRequestFault(HttpCommunicateType.Download, file, new Error());
                    }, () =>
                    {
                        if (fault != null)
                        {
                            fault(e);
                        }
                    }, () =>
                    {
                        are.Set();
                    });
                }, progress);
            }));
            thread.IsBackground = true;
            thread.Start();
            return r;
        }

    }

    /// <summary>
    /// HTTP请求返回结果
    /// </summary>
    public class HttpCommunicateResult
    {
        /// <summary>
        /// 请求中断异常码
        /// </summary>
        public static readonly long ABORT_CODE = 0x2001000;
        private AutoResetEvent are;
        private System.Action abort;

        internal HttpCommunicateResult(AutoResetEvent are, System.Action abort)
        {
            this.are = are;
            this.abort = abort;
        }

        private bool _result = false;

        /// <summary>
        /// 中断请求
        /// </summary>
        public void Abort()
        {
            if (abort != null)
            {
                abort();
            }
        }

        /// <summary>
        /// 等待请求结束
        /// </summary>
        public void WaitForEnd()
        {
            if (are != null)
            {
                are.WaitOne();
            }
        }
        /// <summary>
        /// 请求结果，true表示成功，false表示失败
        /// </summary>
        public bool Result
        {
            get
            {
                if (are != null)
                {
                    are.WaitOne();
                }
                return _result;
            }
            internal set
            {
                _result = value;
            }
        }
    }
}
