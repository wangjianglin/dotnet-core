using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Windows.Markup;
using System.Windows;

namespace Lin.Core.Markup
{
    public  class AndConvert : MarkupExtension
    {
        private  class binding : Lin.Core.ViewModel
        {
        } 
        private dynamic vm = new binding();
        public object Value { get; set; }

        public AndConvert()
        {
            vm.Value = false;
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        { 
            Binding bind = new Binding("Value");
            bind.Path=new PropertyPath(Value);
            bind.Mode=BindingMode.TwoWay;
            bind.Source = vm;
              
            return bind.ProvideValue(serviceProvider);
        }
              
        public static readonly DependencyProperty ValueProperty = DependencyProperty.RegisterAttached("Value", typeof(object), typeof(binding),
         new PropertyMetadata(null, (DependencyObject d, DependencyPropertyChangedEventArgs e) =>
         { 

         })); 
        public static object GetValue(DependencyObject d)
        {
            return (object)d.GetValue(ValueProperty);
        }
        public static void SetValue(DependencyObject d, double value)
        {
            d.SetValue(ValueProperty, value);
        }
    }
}
