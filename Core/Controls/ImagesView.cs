using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Lin.Core.Controls
{
    /// <summary>
    /// Follow steps 1a or 1b and then 2 to use this custom control in a XAML file.
    ///
    /// Step 1a) Using this custom control in a XAML file that exists in the current project.
    /// Add this XmlNamespace attribute to the root element of the markup file where it is 
    /// to be used:
    ///
    ///     xmlns:MyNamespace="clr-namespace:Lin.Core.Controls"
    ///
    ///
    /// Step 1b) Using this custom control in a XAML file that exists in a different project.
    /// Add this XmlNamespace attribute to the root element of the markup file where it is 
    /// to be used:
    ///
    ///     xmlns:MyNamespace="clr-namespace:Lin.Core.Controls;assembly=Lin.Core.Controls"
    ///
    /// You will also need to add a project reference from the project where the XAML file lives
    /// to this project and Rebuild to avoid compilation errors:
    ///
    ///     Right click on the target project in the Solution Explorer and
    ///     "Add Reference"->"Projects"->[Browse to and select this project]
    ///
    ///
    /// Step 2)
    /// Go ahead and use your control in the XAML file.
    ///
    ///     <MyNamespace:ImagesView/>
    ///
    /// </summary>
    public class ImagesView : Control
    {
        static ImagesView()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ImagesView), new FrameworkPropertyMetadata(typeof(ImagesView)));
        }

        public static DependencyProperty ShowImageNumberProperty = DependencyProperty.Register("ShowImageNumber", typeof(int), typeof(ImagesView), new PropertyMetadata(5, (DependencyObject d, DependencyPropertyChangedEventArgs e) =>
        {
        }));

        public int ShowImageNumber
        {
            get { return (int)this.GetValue(ShowImageNumberProperty); }
            set { this.SetValue(ShowImageNumberProperty, value); }
        }

        public static void SetShowImageNumber(DependencyObject dc,int value)
        {
            dc.SetValue(ShowImageNumberProperty, value);
        }

        public static int GetShowImageNumber(DependencyObject dc)
        {
            return (int)dc.GetValue(ShowImageNumberProperty);
        }

        public static DependencyProperty ImagesProperty = DependencyProperty.Register("Images", typeof(string[]), typeof(ImagesView), new PropertyMetadata(null, (DependencyObject d, DependencyPropertyChangedEventArgs e) => {
        }));

        public string[] Images
        {
            get { return (string[])this.GetValue(ImagesProperty); }
            set { this.SetValue(ImagesProperty, value); }
        }

        public static string[] GetImages(DependencyObject dc)
        {
            return (string[])dc.GetValue(ImagesProperty);
        }

        public static void SetImages(DependencyObject dc,string[] value)
        {
            dc.SetValue(ImagesProperty, value);
        }
    }
}
