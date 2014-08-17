using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.ComponentModel;
using Lin.Core.Web.Http;
using Lin.Core.Web.Packages;

namespace Lin.Core.DataValidation
{
    /// <summary>
    /// 把后台Java验证不通过的信息，加入前台
    /// </summary>
    public static class JavaValidataion
    {
        public static void AddValidation(Lin.Core.ViewModel obj,ValidationErrorData error,string name="")
        {
            if (obj == null || error == null || error.fieldErrors == null || error.fieldErrors.Count <= 0)
            {
                return;
            }
            Type objType = obj.GetType();
          
            ValidationErrorData errorTmp = new ValidationErrorData();
            IDictionary<string, string[]>  eErrors = new Dictionary<string, string[]>();
            IDictionary<string, string[]>  fFieldErrors = new Dictionary<string, string[]>();

            foreach (KeyValuePair<string, string[]> item in error.fieldErrors)
            {
                fFieldErrors.Add(item.Key, item.Value);
            }
            errorTmp.errors = eErrors;
            errorTmp.fieldErrors = fFieldErrors;
              
            IDictionary<string, string> d = new Dictionary<string, string>();
            Type pyType = null;
            PropertyInfo[] pInfo = objType.GetProperties();  
            foreach(PropertyInfo py in pInfo)
            {
                if (errorTmp.fieldErrors.Count == 0)
                {
                    break;
                }
                if (errorTmp.fieldErrors.ContainsKey(name + py.Name))
                {
                    d.Add(py.Name, errorTmp.fieldErrors[name + py.Name][0]);
                    error.fieldErrors.Remove(name + py.Name);
                    //d.Add(py.Name, "数据不能为中文");
                    //d.Add(py.Name, "数据长度太长");
                    //d.Add(py.Name, "数据不能字母");
                }
                else
                {
                    try
                    {
                        //pyType = py.DeclaringType;
                        //if (pyType == typeof(Lin.Core.ViewModel.ViewModel) || pyType == typeof(Lin.Core.ViewModel.ViewModelBase) || pyType == typeof(Lin.Core.ViewModel.ViewModelProperty))
                        //{
                        //    continue;
                        //}
                        //object o = py.GetValue(obj, null);
                        //if (o != null)
                        //{
                        //    AddValidation(o as Lin.Core.ViewModel.ViewModel, errorTmp, name + py.Name + ".");
                        //}
                    }
                    catch (Exception) { }
                }
            }

            if (d.Count > 0)
            {
                obj.PropertyChanged += (object sender, PropertyChangedEventArgs e) =>
                        {
                            if (d.ContainsKey(e.PropertyName))
                            {
                                //obj.SetError(e.PropertyName, d[e.PropertyName]);
                                d.Remove(e.PropertyName);
                            }
                        };
            } 
        }
        public static void AddValidation(Package package, Lin.Core.Web.Http.ValidationErrorData error)
        {
            IDictionary<string, object> param = package.GetParams();
            foreach (KeyValuePair<string,object> vp in param)
            {
                AddValidation(vp.Value as Lin.Core.ViewModel, error, vp.Key + ".");
            }
        }
    }
}
