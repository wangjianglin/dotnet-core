using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace Lin.Core.Template
{
    /// <summary>
    /// 
    /// </summary>
    public class PropertyTemplate:System.Windows.DependencyObject
    {
        /// <summary>
        /// 
        /// </summary>
        public string Property { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public object Value { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public DataTemplate DataTemplate { get; set; }
    }
}
