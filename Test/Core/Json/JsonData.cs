using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace AD.Test.Core.Json
{
    public class JsonData
    {
        public string name { get; set; }
        public string password { get; set; }

        public EnumType enumType { get; set; }

        public double[] datas { get; set; }
        public double[][] doubleArrays { get; set; }

        public JsonData obj { get; set; }

        public JsonData[] objArrays { get; set; }
        public JsonData[][] objDoubleArrays { get; set; }

        public IList<JsonData> list { get; set; }

        public IList listNoGeneric { get; set; }

        public IDictionary<string, JsonData> dict { get; set; }
        public IDictionary dictNoGeneric { get; set; }
    }
}
