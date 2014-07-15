using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Controls;

namespace Lin.Core.DataValidation
{
    public class DecimalPoint : ValidationRule
    { 
        ///判断小数点保留的位数 
         
        string _showContext = string.Empty;
        public string ShowContext
        {
            get { return _showContext; }
            set { _showContext = value; }
        }

        private string ShowContextValue
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
            double placeData = Convert.ToDouble(value); 

            ///小数点位数
            int count = Regex.Match(placeData.ToString(), @"(?<=\.)\d+?(?=0*$)").Value.ToCharArray().Count();

            if (count > 2)
            {
                return new ValidationResult(false, this.ShowContextValue);
            } 

            return ValidationResult.ValidResult;
        }
    }
}
