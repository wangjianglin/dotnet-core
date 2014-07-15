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
using System.Globalization;



#region Namespce mapping

[assembly: System.Windows.Markup.XmlnsDefinition("http://ad/2012/xaml/presentation", "Lin.Core.DataValidation")]

#endregion

namespace Lin.Core.DataValidation
{ 
    public class IsEmptyRule : ValidationRule
    {
        /// <summary>
        /// 显示内容
        /// </summary>
        string _showContext = string.Empty;
        public string ShowContext
        {
            get { return _showContext; }
            set { _showContext = value; }
        }
        private string ShowContextList
        {
            get
            {
                if (!string.IsNullOrEmpty(this._showContext))
                {
                    return this._showContext.ToString();
                }
                else
                {
                    return "";
                }
            }
        }

        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        { 
            string dataValue = value as string;

           if (dataValue == null||dataValue =="")
            {
                return new ValidationResult(false, this.ShowContextList);
            } 
            return ValidationResult.ValidResult;
        }
        
    }
}
