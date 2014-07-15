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

namespace Lin.Core.Controls.NotifierControls
{
    public class SystemExceptionControl : Control
    {
        static SystemExceptionControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(SystemExceptionControl), new FrameworkPropertyMetadata(typeof(SystemExceptionControl)));
        }
    }
}
