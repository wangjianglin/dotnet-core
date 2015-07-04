using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.ComponentModel;

namespace Lin.Comm.Http
{
    /// <summary>
    /// Url类型，是采用绝对路径，还是采用相对于 配置Server的路径
    /// </summary>
    //public enum UrlType
    //{
    //    RELATIVE, ABSOLUTE
    //}
    /// <summary>
    /// 
    /// </summary>
    public abstract class Package
    {
        public static readonly IHttpRequestHandle JSON = new JsonHttpRequestHandle();
        public static readonly IHttpRequestHandle NONE = new NoneHttpRequestHandle();
        //public virtual UrlType UrlType { get; protected set; }
        private static readonly Type _DefautlRespType = typeof(string);

        static Package()
        {
            
            //如果没有配置主版，则默认为无版本信息

        }

        /// <summary>
        /// 是否启用缓存，默认不启用
        /// </summary>
        //[DefaultValue(false)]
        public virtual bool EnableCache { get; protected set; }
        public Package()
        {
            //EnableCache = false;
            //UrlType = Packages.UrlType.RELATIVE;
            this.RequestHandle = JSON;
            EnableCache = false;
            //HasParams = true;
            this.RespType = _DefautlRespType;
            this.Version = new Version();
            //this.Version.Major = 0;
            //this.Version.Minor = 0;
        }
        virtual public string location { get; set; }
        virtual public Type RespType { get;protected set; }

        /// <summary>
        /// 数据包的版本号
        /// </summary>
        virtual public Version Version { get; protected set; }


        /// <summary>
        /// 表示是否需要进行参数设置
        /// </summary>
        //public virtual bool HasParams { get; protected set; }

        public virtual IHttpRequestHandle RequestHandle { get; protected set; }
        public virtual IDictionary<string, object> GetParams()
        {
            return null;
        }
    }
}
