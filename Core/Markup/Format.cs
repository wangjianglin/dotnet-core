using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Markup;
using System.Windows;
using System.Reflection;
using System.Windows.Data;

namespace Lin.Core.Markup
{
    public class Format : MarkupExtension
    {  
        public Format()
        {
            this.Mode = BindingMode.OneWay;
        }

        public Format(string path)
        {
            this.Path = new PropertyPath(path);
        }
         
        public override object ProvideValue(IServiceProvider provider)
        {
            Binding binding = CreateBinding();
            return binding.ProvideValue(provider);
        }
         
        private Binding CreateBinding() // 创建绑定类型实例
        {
            Binding binding = new Binding(Path.Path);
            if (Source != null)
            {
                binding.Source = Source;
            }
            if (RelativeSource != null)
            {
                binding.RelativeSource = RelativeSource;
            }
            if (ElementName != null)
            {
                binding.ElementName = ElementName;
            }
            if (FormatString != null && FormatString != "")
            {
                binding.Converter = new FormatValueConverter(Converter,FormatString);
            }
            else
            {
                binding.Converter = Converter;
            }
            binding.Mode = Mode;
            binding.NotifyOnValidationError = NotifyOnValidationError;
            binding.ValidatesOnDataErrors = ValidatesOnDataErrors;
            binding.ConverterParameter = ConverterParameter;
            return binding;
        }

        private class FormatValueConverter : IValueConverter
        {
            private static readonly System.Globalization.DateTimeFormatInfo dateTimeFormatInfo = new System.Globalization.DateTimeFormatInfo();
            private static readonly System.Globalization.NumberFormatInfo numberFormatInfo = new System.Globalization.NumberFormatInfo();
        
            private IValueConverter Converter;
            private string FormatString;
            private Type type;

            public FormatValueConverter(IValueConverter Converter, string FormatString)
            {
                this.Converter = Converter;
                this.FormatString = FormatString;
            }

            public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
            {
                if (this.Converter != null)
                {
                    value = this.Converter.Convert(value, targetType, parameter, culture);
                }
                type = null;
                if (value != null)
                {
                    type = value.GetType();
                    MethodInfo m = value.GetType().GetMethod("ToString", new Type[] { typeof(string) });
                    if (m != null)
                    {
                        return m.Invoke(value, new object[] { FormatString });
                    }
                    if (value.GetType() == typeof(DateTime))
                    {
                        DateTime dt = (DateTime)value;
                        return dt.ToString(FormatString, dateTimeFormatInfo);
                    }
                    IFormattable d = value as IFormattable;
                    if (d != null)
                    {
                        return d.ToString(FormatString, numberFormatInfo);
                    }
                }
                return null;
            }

            public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
            {
                if (this.Converter != null)
                {
                    value = this.ConvertBack(value, targetType, parameter, culture);
                }
                if (value != null && type != null)
                {
                    try
                    {
                        MethodInfo m = type.GetMethod("Parse", new Type[] { typeof(string) });
                        return m.Invoke(value, new object[] { value.ToString() });
                    }
                    catch (Exception) { }
                    try
                    {
                        ConstructorInfo c = type.GetConstructor(new Type[] { typeof(string) });
                        return c.Invoke(new object[] { value.ToString() });
                    }
                    catch (Exception) { }
                }
                return value;
            }
        }
         

        #region Properties
        public string FormatString { get; set; }

        
        public object Source { get; set; }
        public RelativeSource RelativeSource { get; set; }
        public string ElementName { get; set; }
        public PropertyPath Path { get; set; }
        public IValueConverter Converter { get; set; }
        public object ConverterParameter { get; set; }

        [System.ComponentModel.DefaultValue(false)]
        public bool NotifyOnValidationError { get; set; }

        [System.ComponentModel.DefaultValue(false)]
        public bool ValidatesOnDataErrors { get; set; }
        
        public BindingMode Mode { get; set; }
        #endregion

    }
}
