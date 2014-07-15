using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Globalization;



#region Namespce mapping

[assembly: System.Windows.Markup.XmlnsDefinition("http://ad/2012/xaml/presentation", "AD.Core.DataValidation")]

#endregion


namespace AD.Core.DataValidation
{
    public class NumberRangeRule : ValidationRule
    {
        /// <summary>
        /// 最小值
        /// </summary> 
        double min = double.MinValue;
        public double Min
        {
            get { return min; }
            set { min = value; }
        }
        ///<summary>
        ///最大值
        /// </summary> 
        double max = double.MaxValue;
        public double Max
        {
            get { return max; }
            set { max = value; }
        }
        /// <summary>
        /// 显示内容
        /// </summary>
        string _showContext = string.Empty;
        public string ShowContext
        {
            get { return _showContext; }
            set { _showContext = value; }
        }
        private List<string> ShowContextList
        {
            get
            {
                if (!string.IsNullOrEmpty(this._showContext))
                {
                    return this._showContext.Split(';').ToList();
                }
                else
                {
                    return new List<string>();
                }
            }
        }
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            double number;
            if (value == null || "".Equals(value))
            {
                return ValidationResult.ValidResult;
            }
            if (!double.TryParse((string)value, out number))
            {
                return new ValidationResult(false, this.ShowContextList[0]);
            }
            if (number < min || number > max)
            {
                return new ValidationResult(false, string.Format("{2}({0} - {1})", min, max, this.ShowContextList[1]));
            }
            if (!double.TryParse((string)value, out number))
            {
                return new ValidationResult(false, "数据格式不对");
            }
            if (number < min || number > max)
            {
                return new ValidationResult(false, string.Format("范围({0} - {1})", min, max));
            }
            return ValidationResult.ValidResult;
        }
    }
}
