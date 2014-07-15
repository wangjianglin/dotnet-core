using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;


#region Namespce mapping

[assembly: System.Windows.Markup.XmlnsDefinition("http://ad/2012/xaml/presentation", "Lin.Core.Markup")]

#endregion

namespace Lin.Core.Markup
{
    public class BaseMarkupExtension : MarkupExtension
    {

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            Binding binding = CreateBinding();
            return binding.ProvideValue(serviceProvider);
        }

        private Binding CreateBinding()
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
            binding.Mode = Mode;
            binding.NotifyOnValidationError = NotifyOnValidationError;
            binding.ValidatesOnDataErrors = ValidatesOnDataErrors;
            return binding;
        }



        public object Source { get; set; }
        public RelativeSource RelativeSource { get; set; }
        public string ElementName { get; set; }
        public PropertyPath Path { get; set; }

        [System.ComponentModel.DefaultValue(false)]
        public bool NotifyOnValidationError { get; set; }

        [System.ComponentModel.DefaultValue(false)]
        public bool ValidatesOnDataErrors { get; set; }

        public BindingMode Mode { get; set; }
    }
}
