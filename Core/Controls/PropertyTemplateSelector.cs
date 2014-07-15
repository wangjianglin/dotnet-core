using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace Lin.Core.Controls
{
    /// <summary>
    /// 根据属性选择模版
    /// </summary>
    public class PropertyTemplateSelector : System.Windows.Controls.DataTemplateSelector
    {
        public List<PropertyTemplate> PropertyTemplate { get; set; }
        public PropertyTemplateSelector()
        {
            PropertyTemplate = new List<PropertyTemplate>();
        }
        public override System.Windows.DataTemplate SelectTemplate(object item, System.Windows.DependencyObject container)
        {
            if (item != null)
            {
                if (PropertyTemplate != null)
                {
                    foreach (PropertyTemplate type in PropertyTemplate)
                    {
                        PropertyInfo p = item.GetType().GetProperty(type.Property);
                        if (p != null)
                        {
                            if (p.GetValue(item, null) == type.Value)
                            {
                                return type.DataTemplate;
                            }
                        }
                    }
                }
            }
            return base.SelectTemplate(item, container);
        }
    }
}
