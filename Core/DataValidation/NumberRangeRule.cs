using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Globalization;


namespace Lin.Core.DataValidation
{
    public enum MeasuringRang
    {
        LColseRClose,   //> <
        LColseROpen,    //>  <=
        LOpenRClose,   //>=  <
        LOpenROpen     //>=  <=
    }
    public class NumberRangeRule : ValidationRule
    {
        /// <summary>
        /// 最大值与最小值所取区间
        /// </summary>
        MeasuringRang rang = MeasuringRang.LOpenROpen;
        public MeasuringRang Rang
        {
            get { return rang; }
            set { rang = value; }
        }
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
        /// 格式有错误
        /// </summary>
        string _showErrorContext = string.Empty;
        public string ShowErrorContext
        {
            get { return _showErrorContext; }
            set { _showErrorContext = value; }
        }
        /// <summary>
        /// 范围
        /// </summary>
        string _showRangeContext = null;
        public string ShowRangeContext
        {
            get { return _showRangeContext; }
            set { _showRangeContext = value; }
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
                return new ValidationResult(false, this.ShowErrorContext);
            }
            if (!double.TryParse((string)value, out number))
            {
                return new ValidationResult(false, "数据格式不对");
            }


            if (Rang == MeasuringRang.LColseRClose)
            {
                if (min == double.MinValue)
                {
                    if (number > max)
                    {
                        if (this.ShowRangeContext == null)
                        {
                            return new ValidationResult(false, string.Format("{1}{0}", max, "应小于："));
                        }
                        else
                        {
                            return new ValidationResult(false, this.ShowRangeContext);
                        }
                    }
                }
                else if (max == double.MaxValue)
                {
                    if (number < min)
                    {
                        if (this.ShowRangeContext == null)
                        {
                            return new ValidationResult(false, string.Format("{1}{0}", min, "应大于："));
                        }
                        else
                        {
                            return new ValidationResult(false, this.ShowRangeContext);
                        }
                    }
                }
                else if (min != double.MinValue && max != double.MaxValue)
                {
                    if (number < min || number > max)
                    {
                        if (this.ShowRangeContext == null)
                        {
                            return new ValidationResult(false, string.Format("{2}({0} - {1})", min, max, "范围"));
                        }
                        else
                        {
                            return new ValidationResult(false, string.Format("{2}({0} - {1})", min, max, this.ShowRangeContext));
                        }
                    }
                }
            }

            if (Rang == MeasuringRang.LColseROpen)
            {
                if (min == double.MinValue)
                {
                    if (number >= max)
                    {
                        if (this.ShowRangeContext == null)
                        {
                            return new ValidationResult(false, string.Format("{1}{0}", max, "应小于："));
                        }
                        else
                        {
                            return new ValidationResult(false, this.ShowRangeContext);
                        }
                    }
                }
                else if (max == double.MaxValue)
                {
                    if (number < min)
                    {
                        if (this.ShowRangeContext == null)
                        {
                            return new ValidationResult(false, string.Format("{1}{0}", min, "应大于："));
                        }
                        else
                        {
                            return new ValidationResult(false, this.ShowRangeContext);
                        }
                    }
                }
                else if (min != double.MinValue && max != double.MaxValue)
                {
                    if (number < min || number >= max)
                    {
                        if (this.ShowRangeContext == null)
                        {
                            return new ValidationResult(false, string.Format("{2}({0} - {1})", min, max, "范围"));
                        }
                        else
                        {
                            return new ValidationResult(false, string.Format("{2}({0} - {1})", min, max, this.ShowRangeContext));
                        }
                    }
                }
            }

            if (Rang == MeasuringRang.LOpenRClose)
            {
                if (min == double.MinValue)
                {
                    if (number > max)
                    {
                        if (this.ShowRangeContext == null)
                        {
                            return new ValidationResult(false, string.Format("{1}{0}", max, "应小于："));
                        }
                        else
                        {
                            return new ValidationResult(false, this.ShowRangeContext);
                        }
                    }
                }
                else if (max == double.MaxValue)
                {
                    if (number <= min)
                    {
                        if (this.ShowRangeContext == null)
                        {
                            return new ValidationResult(false, string.Format("{1}{0}", min, "应大于："));
                        }
                        else
                        {
                            return new ValidationResult(false, this.ShowRangeContext);
                        }
                    }
                }
                else if (min != double.MinValue && max != double.MaxValue)
                {
                    if (number <= min || number > max)
                    {
                        if (this.ShowRangeContext == null)
                        {
                            return new ValidationResult(false, string.Format("{2}({0} - {1})", min, max, "范围"));
                        }
                        else
                        {
                            return new ValidationResult(false, string.Format("{2}({0} - {1})", min, max, this.ShowRangeContext));
                        }
                    }
                }
            }

            if (Rang == MeasuringRang.LOpenROpen)
            {
                if (min == double.MinValue)
                {
                    if (number >= max)
                    {
                        if (this.ShowRangeContext == null)
                        {
                            return new ValidationResult(false, string.Format("{1}{0}", max, "应小于："));
                        }
                        else
                        {
                            return new ValidationResult(false, this.ShowRangeContext);
                        }
                    }
                }
                else if (max == double.MaxValue)
                {
                    if (number <= min)
                    {
                        if (this.ShowRangeContext == null)
                        {
                            return new ValidationResult(false, string.Format("{1}{0}", min, "应大于："));
                        }
                        else
                        {
                            return new ValidationResult(false, this.ShowRangeContext);
                        }
                    }
                }
                else if (min != double.MinValue && max != double.MaxValue)
                {
                    if (number <= min || number >= max)
                    {
                        if (this.ShowRangeContext == null)
                        {
                            return new ValidationResult(false, string.Format("{2}({0} - {1})", min, max, "范围"));
                        }
                        else
                        {
                            return new ValidationResult(false, string.Format("{2}({0} - {1})", min, max, this.ShowRangeContext));
                        }
                    }
                }
            }


            //if (number < min || number > max)
            //{
            //    return new ValidationResult(false, string.Format("{2}({0} - {1})", min, max, this.ShowRangeContext));
            //}
            
            //if (number < min || number > max)
            //{
            //    return new ValidationResult(false, string.Format("范围({0} - {1})", min, max));
            //}


            return ValidationResult.ValidResult;
        }
    }
}
