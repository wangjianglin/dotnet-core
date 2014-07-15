using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Windows.Data;
using System.Windows;
using Lin.Core.Config;

namespace AD.Test.Core
{
    public class TestDependencyObject : DependencyObject
    {
        public static readonly DependencyProperty TestProperty = DependencyProperty.Register("Test", typeof(object), typeof(TestDependencyObject),
            new PropertyMetadata(null, TestPropertyChanged));

        private static void TestPropertyChanged(object sender, DependencyPropertyChangedEventArgs args)
        {
            Console.WriteLine("value:"+args.NewValue);
        }
    }
    [TestClass]
    public class ConfigChangedTest
    {


        [TestMethod]
        public void TestRead()
        {
            IConfigManager config = ConfigManager.Net;
            string aaa = config["AAA"];
            Console.WriteLine("Net.AAA:" + aaa);
        }

        [TestMethod]
        public void TestSectionNames()
        {
            IConfigManager config = ConfigManager.Net;
            string[] names = config.Names;
            Console.WriteLine("names:" + names);
            foreach (string name in names)
            {
                Console.WriteLine("name:" + name);
            }

            string[] values = config.Values;
            Console.WriteLine("names:" + values);
            foreach (string value in values)
            {
                Console.WriteLine("value:" + value);
            }
        }

        [TestMethod]
        public void TestConfigChanged()
        {
            Binding binding = new Binding("[Value]");
            binding.Source = ConfigManager.Net;
            //binding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            TestDependencyObject test = new TestDependencyObject();
            //Window window = new Window();
            //window.Show();
            //window.Content = test;
            BindingOperations.SetBinding(test, TestDependencyObject.TestProperty, binding);
           // ConfigManager.Net["Value"] = "---------------";
        }
    }
}
