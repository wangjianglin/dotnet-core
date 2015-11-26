using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AD.Test.Core
{
    public class ValidationPackage : Lin.Comm.Http.HttpPackage
    {
        public ValidationPackage()
        {
            this.location = "/web/action/validation!test1.action";
            //this.Version.Major = 0;
            //this.Version.Minor = 0;
        }

        public override IDictionary<string, object> GetParams()
        {
            IDictionary<string,object> param = new Dictionary<string,object>();
            param.Add("name1", name1);
            param.Add("name2", name2);
            return param;
        }

        public string name1 { get; set; }

        public string name2 { get; set; }
    }
    [TestClass]
    public class ValidationTest:BaseTest
    {
        [TestMethod]
        public void TestValidation()
        {
            ValidationPackage package = new ValidationPackage();
            package.name1 = "name1";
            package.name2 = "name2";
            Request(package, (result, warning) =>
            {
                Console.WriteLine("result:" + result);
                //Assert.IsTrue(package.data.Equals(result));
            }, error =>
            {
                if (error.dataType == 1)
                {
                }
                Console.WriteLine("error:" + error.cause);
            }, 0);
        }
    }
}
