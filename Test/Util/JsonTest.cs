using Lin.Util.Json;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AD.Test.Util
{
    [TestClass]
    public class JsonTest
    {
        [TestMethod]
        public void TestJsonParser()
        {
            String json = "{\"code\":1,\"message\":null,\"result\":{\"pages\":364,\"rowCount\":8732,\"currentPage\":1,\"pageSize\":24,\"resultList\":[{\"goodsName\":\"\"}]}}";

            JsonValue jsonValue = JsonUtil.Deserialize(json) as JsonValue;
            System.Console.WriteLine("type:" + jsonValue.JsonType);
            ResultData tmp = JsonUtil.Deserialize<ResultData>(jsonValue);

            ResultData obj = Lin.Util.Json.JsonUtil.Deserialize<ResultData>(json);
            System.Console.WriteLine("ok.");
        }
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

    public class Error
    {
        public Error()
        {
        }
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
