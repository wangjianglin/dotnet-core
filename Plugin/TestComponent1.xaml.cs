using Lin.Plugin.ComponentAttribute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Lin.Plugin
{
    [ComponentAttribute.Component(Name = "测试1",
        Type = "TestComponent1",
        Description = "测试组件1",
        NetState = NetState.OnlineAndOffline,
        IsVisual = true,
        Major = 1,
        Minor = 0)]
    [LargeButton(Name = "测试", 
        TargetGroupName = "Adows", 
        Icon = "/AD.Adows;component/Images/AdowsTest.png", 
        IsShowContent = true,
        Command = "Commmand")]
    
    public partial class TestComponent1 : UserControl
    {
        public TestComponent1()
        {
            InitializeComponent();
        }

        public ICommand Commmand
        {
            get
            {
                return new Command();
            }
        }
    }

    public class Command : ICommand
    {

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public event EventHandler CanExecuteChanged;

        public void Execute(object parameter)
        {
            MessageBox.Show("aaa");
        }
    }
}
