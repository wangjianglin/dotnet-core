using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lin.Comm.Http
{
    public class Error
    {
        public Error()
        {
        }
        //public Error(long code)
        //{
        //    this.code = code;
        //    this.message = "";
        //}
        //public Error(long code, string message)
        //{
        //    this.code = code;
        //    this.message = message;
        //}
        public Error(long code = -1, string message = "", string cause = "", string stackTrace = "")
        {
            this.code = code;
            this.message = message;
            this.cause = cause;
        }
        public string cause { get; internal set; }
        public long code { get; internal set; }
        public string message { get; internal set; }
        public string stackTrace { get; internal set; }

        public object data { get; set; }

        public int dataType { get; set; }//数据类型,0、正常，1、后台验证错误
    }
}
