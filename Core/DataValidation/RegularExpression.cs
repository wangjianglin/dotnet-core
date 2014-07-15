using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Text.RegularExpressions;
using System.Windows;

namespace Lin.Core.DataValidation
{
    public class RegularExpression : ValidationRule
    {
        /// <summary>
        /// 正则表达式
        /// </summary>
        private string expression;
        public string Expression
        {
            get { return this.expression; }
            set { this.expression = value; }
        }

        /// <summary>
        /// 显示的错误信息
        /// </summary>
        private string showContent;
        public string ShowContent
        {
            get { return this.showContent; }
            set { this.showContent = value; }
        }

        public override ValidationResult Validate(object value, System.Globalization.CultureInfo cultureInfo)
        {
            if (Regex.IsMatch(value.ToString(), @expression) == false)
            {
                return new ValidationResult(false, showContent);
            }

            return ValidationResult.ValidResult;
        }
    }
}
