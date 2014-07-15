using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Security;
using System.Text;

namespace Lin.Util
{


    public static class LinExceptionUtil
    {

        public static int ExceptionCode(this int model, int serial)
        {
            return model << 16 + serial;
        }
    }
//#define LIN_EXCEPTION_CODE(model,seial) 
    /// <summary>
    /// 
    /// 警告与错误的区分标准，当从出现异样情况，整个业务逻辑还能继续完成时，认为是警告，否则就认为是错误，业务逻辑不能继续完成
    /// 
    /// 自定义异常类，用唯一的编码来标识特定的异常
    /// 
    /// 大于>0警告，<0错误，0表示正常，-1表示未知编码
    /// 
    /// -7FFFFFFF  --  7FFFFFFF
    /// -80000000  --  7FFFFFFF
    /// 0保留做特殊用途,-8000000保留
    /// 1-3FFF 作为系统组件    1-7FF Java 800-FFF  .Net 1000 -17FF Android  1800-1FFF iOS 2000-27FF Web
    /// 4000-7FFF业务组件用


    /// Util 800
    /// Plugin 801
    /// Core 802
    /// Chess 4000
    /// </summary>
    public class LinException : System.Exception
    {
        public int ExceptionCode(int model, int code)
        {
            return model << 16 + code;
        }

        public static LinExceptionWarningHandler LinExceptionWarningHandler;
        public static void FireWarning(object sender,LinException warning){
            if (LinExceptionWarningHandler != null)
            {
                LinExceptionWarningHandler(sender,new LinExceptionWarningArgs(warning));
            }
        }
        public LinException(int code)
        {
            this.Code = code;
        }

        //
        // 摘要:
        //     使用指定的错误消息初始化 System.Exception 类的新实例。
        //
        // 参数:
        //   message:
        //     描述错误的消息。
        public LinException(int code, string message)
            : base(message)
        {
            this.Code = code;
        }
        //
        // 摘要:
        //     用序列化数据初始化 System.Exception 类的新实例。
        //
        // 参数:
        //   info:
        //     System.Runtime.Serialization.SerializationInfo，它存有有关所引发异常的序列化的对象数据。
        //
        //   context:
        //     System.Runtime.Serialization.StreamingContext，它包含有关源或目标的上下文信息。
        //
        // 异常:
        //   System.ArgumentNullException:
        //     info 参数为 null。
        //
        //   System.Runtime.Serialization.SerializationException:
        //     类名为 null 或 System.Exception.HResult 为零 (0)。
        [SecuritySafeCritical]
        protected LinException(int code, SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            this.Code = code;
        }
        //
        // 摘要:
        //     使用指定错误消息和对作为此异常原因的内部异常的引用来初始化 System.Exception 类的新实例。
        //
        // 参数:
        //   message:
        //     解释异常原因的错误消息。
        //
        //   innerException:
        //     导致当前异常的异常；如果未指定内部异常，则是一个 null 引用（在 Visual Basic 中为 Nothing）。
        public LinException(int code, string message, Exception innerException)
            : base(message, innerException)
        {
            this.Code = code;
        }

        public LinException(int code, Exception innerException)
            : base("", innerException)
        {
            this.Code = code;
        }

        /// <summary>
        /// 唯一标识码
        /// </summary>
        public int Code { get; private set; }

        /// <summary>
        /// 从error.code文件中读取信息
        /// </summary>
        public override string Message
        {
            get
            {
                return base.Message;
            }
        }
    }
}
