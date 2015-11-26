using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows;

namespace Lin.Core.Template
{
    /// <summary>
    /// 模版选择器
    /// </summary>
    public class TypeTemplateSelector : System.Windows.Controls.DataTemplateSelector
    {
        public List<TypeTemplate> DataTemplates { get; set; }
        public TypeTemplateSelector()
        {
            DataTemplates = new List<TypeTemplate>();
        }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item != null)
            {
                if (DataTemplates != null)
                {
                    Type baseType = item.GetType();
                    while (true)
                    {
                        foreach (TypeTemplate type in DataTemplates)
                        {
                            if (type.TargetType == baseType)
                            {
                                return type.DataTemplate;
                            }
                        }
                        baseType = baseType.BaseType;
                        if (baseType == typeof(object).BaseType)
                        {
                            break;
                        }
                    }
                }      
            }
            return base.SelectTemplate(item, container);
        }
    }
}
