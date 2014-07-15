using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Text.RegularExpressions;

namespace AD.Test.Core.DataV
{
    [TestClass]
    public class RegularExpression
    {
        [TestMethod]
        public void TestRegularExpression()
        {
            //char[] end = new char[] { '/', '$' };
            //char[] start = new char[] { '/', '^' };
            //expression = expression.TrimEnd(end);
            //expression = expression.TrimStart(start);
            //if (Regex.IsMatch(value.ToString(), expression) == false)
            //{
            //return new ValidationResult(false, showContent);
            //}
            string expression = @"^[\u4e00-\u9fa5]|\w|[\u4e00-\u9fa5]\w{2,30}$";
            string[] values = new string[] { "地方", "水电费aaaa", "sssssss" };
            foreach (String value in values)
            {
                Console.WriteLine(value + ":" + Regex.IsMatch(value, expression));
            }
        }
    }
}
