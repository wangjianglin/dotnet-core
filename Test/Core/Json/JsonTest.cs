using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AD.Core.Web.Json;
using System.Collections;

namespace AD.Test.Core.Json
{
    [TestClass]
    public class JsonTest
    {
        //[TestMethod]
        public void TestType()
        {
            Console.WriteLine("result:" + typeof(IList<JsonData>).IsAssignableFrom(typeof(IEnumerable)));
            Console.WriteLine("result:" + typeof(IEnumerable).IsAssignableFrom(typeof(IList<JsonData>)));
        }
        
        [TestMethod]
        public void TestJson()
        {
            JsonData data = null;
            data = new JsonData();
            data.name = "name";
            data.password = "password";
            data.enumType = EnumType.TWO;
            data.datas = new double[] { 1.0, 2.0, 3.0, 4.0, 5.0, 6.0, 7.0, 8.0, 9.0, 10.0 };
            data.doubleArrays = new double[][] { new double[] { 1.0, 2.0, 3.0, 4.0 }, new double[] { 5.0, 6.0, 7.0, 8.0 }, new double[] { 9.0, 10.0, 11.0, 12.0 }, new double[] { 13.0, 14.0, 15.0, 16.0 } };

            JsonData tmpData = new JsonData();
            tmpData.name = "name-tmp";
            tmpData.password = "password-tmp";

            data.obj = tmpData;

            data.objArrays = new JsonData[4];
            for (int n = 0; n < 4; n++)
            {

                tmpData = new JsonData();
                tmpData.name = "name-"+(n+1);
                tmpData.password = "password-" + (n + 1);

                data.objArrays[n] = tmpData;
            }

            data.objDoubleArrays = new JsonData[4][];
            for (int n = 0; n < 4; n++)
            {
                data.objDoubleArrays[n] = new JsonData[4];
                for (int k = 0; k < 4; k++)
                {

                    tmpData = new JsonData();
                    tmpData.name = "name-" + (n * 4 + (k + 1));
                    tmpData.password = "password-" + (n * 4 + (k + 1));

                    data.objDoubleArrays[n][k] = tmpData;
                }
            }

            data.list = new List<JsonData>();
            for (int n = 0; n < 16; n++)
            {
                tmpData = new JsonData();
                tmpData.name = "name-list-" + (n + 1);
                tmpData.password = "password-list-" + (n + 1);

                data.list.Add(tmpData);
            }

            data.listNoGeneric = new ArrayList();
            for (int n = 0; n < 16; n++)
            {
                tmpData = new JsonData();
                tmpData.name = "name-list-no-generic-" + (n + 1);
                tmpData.password = "password-list-no-generic-" + (n + 1);

                data.listNoGeneric.Add(tmpData);
            }

            data.dict = new Dictionary<string, JsonData>();
            for (int n = 0; n < 16; n++)
            {
                tmpData = new JsonData();
                tmpData.name = "name-dict-no-generic-" + (n + 1);
                tmpData.password = "password-dict-no-generic-" + (n + 1);

                data.dict.Add(tmpData.name,tmpData);
            }

            data.dictNoGeneric = new Hashtable();
            for (int n = 0; n < 16; n++)
            {
                tmpData = new JsonData();
                tmpData.name = "name-dict-no-generic-" + (n + 1);
                tmpData.password = "password-dict-no-generic-" + (n + 1);

                data.dictNoGeneric.Add(tmpData.name, tmpData);
            }

            string jsonString = JsonUtil.Serialize(data);
            Console.WriteLine("json string:" + jsonString);
            //data = JsonUtil.Deserialize<JsonData>("{\"name\":\"name\",\"password\":\"password\",\"enumType\":\"ONE\"}");
            data = JsonUtil.Deserialize<JsonData>(jsonString);
            Console.WriteLine("data:" + data);

            int length1 = jsonString.Length;
            jsonString = JsonUtil.Serialize(data);
            Console.WriteLine("json string:" + jsonString);

            int length2 = jsonString.Length;

            Console.WriteLine("result:" + (length1 == length2));

            //JsonValue jsonValue = JsonValue.Parse("{\"name\":\"name\",\"password\":\"password\"}");
            //Console.WriteLine("json value:" + jsonValue.ToString());
        }
    }
}
