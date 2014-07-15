using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lin.Core.DataValidation
{
    /// <summary>
    /// 数据验证接口
    /// </summary>
    public interface IDataValidationInfo
    {
        void Added(string propertyName, object content);

        void Removed(string propertyName);

        bool HasError();

        event EventHandler ValidationChanged;
    }
}
