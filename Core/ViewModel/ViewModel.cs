using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Collections.Generic;
using System.ComponentModel;
using Lin.Core.Web.Json;
using System.Threading;
using Lin.Core.DataValidation;

namespace Lin.Core.ViewModel
{
    /// <summary>
    /// 实现数据验证
    /// </summary>
     [Serializable]
    public class ViewModel:ViewModelBase,IDataErrorInfo,IDataValidationInfo
    { 
         //[JsonSkip]
        public Dictionary<string, string> errors = new Dictionary<string, string>();
        #region IDataErrorInfo Members

        public string Error
        {
            get { return null; }
        }

        public string this[string columnName]
        {
            get
            {
                //Lin.Core.Controls.TaskbarNotifierUtil.Show(columnName);
                if (errors.ContainsKey(columnName))
                {
                    //Lin.Core.Controls.TaskbarNotifierUtil.Show(columnName);
                    return errors[columnName];
                }
                else
                {
                    return string.Empty; 
                } 
            }
        }

        #endregion

        //设置错误信息
        public virtual void SetError(string propertyName, string errorMessage)
        {
            errors[propertyName] = errorMessage;
            NotifyPropertyChanged(propertyName);
        }
        //清除错误信息
        public virtual void ClearError(string propertyName)
        {
            errors.Remove(propertyName);
            this.OnPropertyChanged(propertyName);
        }
         
        public ViewModel()
        {
        }
      
     
        #region 实现数据验证

        /// <summary>
        /// 
        /// </summary>
        private Dictionary<string, object> DataValidationErrors = new Dictionary<string, object>();
        /// <summary>
        /// 添加错误信息
        /// </summary>
        /// <param name="propertyName"></param>
        /// <param name="content"></param>
        public void Added(string propertyName, object content)
        {
            if (!DataValidationErrors.ContainsKey(propertyName))
            {
                DataValidationErrors.Add(propertyName, content);
            }
            else
            {
                DataValidationErrors[propertyName] = content;
            }
            if (ValidationChanged != null)
            {
                ValidationChanged(this, new EventArgs());
            }
        }
        /// <summary>
        /// 删除错误信息
        /// </summary>
        /// <param name="propertyName"></param>
        public void Removed(string propertyName)
        {
            if (DataValidationErrors.ContainsKey(propertyName))
            {
                DataValidationErrors.Remove(propertyName);
            }
            if (ValidationChanged != null)
            {
                ValidationChanged(this, new EventArgs());
            }
        }
        /// <summary>
        /// 返回当前是验证是否通过,true为有错误信息未通过，false为无错误信息验证通过
        /// </summary>
        /// <returns></returns>
        public bool HasError()
        {
            if (DataValidationErrors.Count > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public event EventHandler ValidationChanged;

        #endregion
    }
}
