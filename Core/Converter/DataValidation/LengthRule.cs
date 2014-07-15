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

namespace AD.Core.DataValidation
{
    public class LengthRule : ValidationRule
    {
        private string sstrLength;
        public string strLength
        {
            get { return sstrLength ;}
            set { sstrLength=value ;}
        }
        string _showContext = string.Empty;
        public string ShowContext
        {
            get { return _showContext; }
            set { _showContext = value; }
        }
        private  string  ShowContextList
        {
            get
            {
                if (!string.IsNullOrEmpty(this._showContext))
                {
                    return this._showContext;
                }
                else
                {
                    return string.Empty;
                }
            }
        }

        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            string stringData = value as string; 
            if (string.IsNullOrEmpty(sstrLength) || stringData.Length > Convert.ToInt32(sstrLength))
            {
                return new ValidationResult(false, this.ShowContextList);
            }

            return ValidationResult.ValidResult;
        }
    }
}
