using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Lin.Util.Json;

namespace AD.Test.Core
{
    public class TestData
    {
        public string name { get; set; }
    }

    public class MapData<K, V> : Dictionary<K, V>
    {
        public TestData data { get; set; }
    }
    [TestClass]
    public class JsonTest
    {

        [TestMethod]
        public void TestMapData()
        {
            MapData<string, TestData> map = new MapData<string, TestData>();
            map.data = new TestData();
            map.data.name = "data1";
            map.Add("data2", new TestData());
            map["data2"].name = "data2";
            string jsonData = JsonUtil.Serialize(map);
            Console.WriteLine("json data:" + jsonData);
        }

        [TestMethod]
        public void TestJson()
        {
            //IntelligenceModel model =new IntelligenceModel();
            //Dictionary<string, Object> param = new Dictionary<string, object>();
            //model.name = "model.name";
            //model.content = Encoding.Default.GetBytes("model.content");
            //model.date = DateTime.Now;
            //model.id = 4546;
            ////param.Add("model.name", model.name);
            ////param.Add("model.content", model.content);
            ////param.Add("model.date", model.date);
            //param.Add("model", model);

            //Console.WriteLine("json:" + JsonUtil.Serialize(param));


            //param = new Dictionary<string, object>();

            //param.Add("model.name", model.name);
            //param.Add("model.content", model.content);
            //param.Add("model.date", model.date);

            //Console.WriteLine("json:" + JsonUtil.Serialize(param));
        }
    }
}
