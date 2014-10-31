using System;
using System.Net;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.Globalization;
using System.Runtime.Serialization;
using System.Reflection;

namespace Lin.Util.Json
{
    internal class JavaScriptSerializer
    {
        private Stream outputStream;

        // Methods
        public JavaScriptSerializer(Stream stream)
        {
            this.outputStream = stream;
        }

        /// <summary>
        /// 序列化 bool 对象
        /// </summary>
        /// <param name="o"></param>
        /// <param name="sb"></param>
        private static void SerializeBoolean(bool o, StringBuilder sb)
        {
            if (o)
            {
                sb.Append("true");
                //sb.Append("true");
            }
            else
            {
                sb.Append("false");
                //sb.Append("\"FALSE\"");
            }
        }

        private readonly static long unixEpochTicks = (new DateTime(0x7b2, 1, 1, 0, 0, 0, DateTimeKind.Utc)).Ticks;
        
        /// <summary>
        /// 序列化 DateTime 对象
        /// </summary>
        /// <param name="value"></param>
        /// <param name="sb"></param>
        private static void SerializeDateTime(DateTime value, StringBuilder sb)
        {
            sb.Append("\"");
            sb.Append(value.ToString("yyyy-MM-dd HH:mm:ss"));
            sb.Append("\"");
            //sb.Append("\"\\/Date(");
            //DateTimeFormatInfo fInfo = new DateTimeFormatInfo();
            //Formatter format = new D
            //DateTime time = value.ToUniversalTime();
        //    sb.Append((long)((time.Ticks - unixEpochTicks) / 0x2710L));
        //    switch (value.Kind)
        //    {
        //        case DateTimeKind.Unspecified:
        //        case DateTimeKind.Local:
        //            span = DateTime.SpecifyKind(value, DateTimeKind.Utc).Subtract(time);
        //            if (span.Ticks >= 0L)
        //            {
        //                sb.Append("+");
        //                break;
        //            }
        //            sb.Append("-");
        //            break;

        //        default:
        //            goto Label_00F5;
        //    }
        //    int num = Math.Abs(span.Hours);
        //    sb.Append((num < 10) ? ("0" + num) : num.ToString(CultureInfo.InvariantCulture));
        //    int num2 = Math.Abs(span.Minutes);
        //    sb.Append((num2 < 10) ? ("0" + num2) : num2.ToString(CultureInfo.InvariantCulture));
        //Label_00F5:
        //    sb.Append(")\\/\"");
        }

        /// <summary>
        /// 序列化 IDictionary 对象
        /// </summary>
        /// <param name="o"></param>
        /// <param name="sb"></param>
        /// <param name="depth"></param>
        /// <param name="objectsInUse"></param>
        private void SerializeDictionary(IDictionary o, StringBuilder sb, int depth, Dictionary<object, bool> objectsInUse)
        {
            sb.Append('{');
            bool flag = true;
            foreach (DictionaryEntry entry in o)
            {
                if (!flag)
                {
                    sb.Append(',');
                }
                if (!(entry.Key is string))
                {
                    throw new SerializationException("ObjectSerializer_DictionaryNotSupported");
                }
                SerializeString((string)entry.Key, sb);
                sb.Append(':');
                //this.SerializeValue(entry.Value, sb, depth, objectsInUse);
                this.SerializeValueInternal(entry.Value, sb, depth, objectsInUse);
                flag = false;
            }
            sb.Append('}');
        }

        /// <summary>
        /// 序列化 可枚举的 对象
        /// </summary>
        /// <param name="enumerable"></param>
        /// <param name="sb"></param>
        /// <param name="depth"></param>
        /// <param name="objectsInUse"></param>
        private void SerializeEnumerable(IEnumerable enumerable, StringBuilder sb, int depth, Dictionary<object, bool> objectsInUse)
        {
            sb.Append('[');
            bool flag = true;
            foreach (object obj2 in enumerable)
            {
                if (!flag)
                {
                    sb.Append(',');
                }
                //this.SerializeValue(obj2, sb, depth, objectsInUse);
                this.SerializeValueInternal(obj2, sb, depth, objectsInUse);
                flag = false;
            }
            sb.Append(']');
        }

        /// <summary>
        /// 序列化 Guid 对象
        /// </summary>
        /// <param name="guid"></param>
        /// <param name="sb"></param>
        private static void SerializeGuid(Guid guid, StringBuilder sb)
        {
            sb.Append("\"").Append(guid.ToString()).Append("\"");
        }

        /// <summary>
        /// 把一个对象序列化成 JSON 格式，并写到outputStream中
        /// </summary>
        /// <param name="obj"></param>
        public void SerializeObject(object obj)
        {
            StringBuilder sb = new StringBuilder();
            this.SerializeValueInternal(obj, sb, 0, null);
            //byte[] bytes = Encoding.UTF8.GetBytes(sb.ToString());
            byte[] bytes = Encoding.Default.GetBytes(sb.ToString());
            this.outputStream.Write(bytes, 0, bytes.Length);
        }

        /// <summary>
        /// 序列化字符串
        /// </summary>
        /// <param name="input"></param>
        /// <param name="sb"></param>
        private static void SerializeString(string input, StringBuilder sb)
        {
            sb.Append('"');
            sb.Append(JavaScriptString.QuoteString(input));
            sb.Append('"');
        }

        /// <summary>
        /// 序列化 Uri 对象
        /// </summary>
        /// <param name="uri"></param>
        /// <param name="sb"></param>
        private static void SerializeUri(Uri uri, StringBuilder sb)
        {
            SerializeString(uri.GetComponents(UriComponents.SerializationInfoString, UriFormat.UriEscaped), sb);
        }

        //private void SerializeValue(object o, StringBuilder sb, int depth, Dictionary<object, bool> objectsInUse)
        //{
        //    this.SerializeValueInternal(o, sb, depth, objectsInUse);
        //}

        /// <summary>
        /// StringBuilder
        /// </summary>
        /// <param name="o">待序列化的对象</param>
        /// <param name="sb">存储序列化后的字符串</param>
        /// <param name="depth">正写入对象的深度，从0开始</param>
        /// <param name="objectsInUse">用于记录已经被序列化过的对象，防止对象的循环引用</param>
        private void SerializeValueInternal(object o, StringBuilder sb, int depth, Dictionary<object, bool> objectsInUse)
        {
            if ((o == null) || DBNull.Value.Equals(o))
            {
                sb.Append("null");
            }
            else
            {
                string input = o as string;
                if (input != null)
                {
                    SerializeString(input, sb);
                }
                else if (o is Enum)
                {
                    SerializeString(Enum.GetName(o.GetType(), o), sb);
                }
                else if (o is char)
                {
                    SerializeString(XmlConvert.ToString((char)o), sb);
                }
                else if (o is bool)
                {
                    SerializeBoolean((bool)o, sb);
                }
                else if (o is DateTime)
                {
                    SerializeDateTime((DateTime)o, sb);
                }
                else if (o is Guid)
                {
                    SerializeGuid((Guid)o, sb);
                }
                else
                {
                    Uri uri = o as Uri;
                    if (uri != null)
                    {
                        SerializeUri(uri, sb);
                    }
                    else if (o is double)
                    {
                        //处理-INF、NaN、INF的情况，如果调用d.ToString方法，会根据当前系统语言环境，生成本地化的字符串
                        double d = (double)o;
                        if (double.IsInfinity(d))
                        {
                            if (double.IsNegativeInfinity(d))
                            {
                                sb.Append("-INF");
                            }
                            else
                            {
                                sb.Append("INF");
                            }
                        }
                        else if (double.IsNaN(d))
                        {
                            sb.Append("NaN");
                        }
                        else
                        {
                            sb.Append(d.ToString("r", CultureInfo.InvariantCulture));
                        }
                    }
                    else if (o is float)
                    {
                        //处理-INF、NaN、INF的情况，如果调用f.ToString方法，会根据当前系统语言环境，生成本地化的字符串
                        float f = (float)o;
                        if (float.IsInfinity(f))
                        {
                            if (float.IsNegativeInfinity(f))
                            {
                                sb.Append("-INF");
                            }
                            else
                            {
                                sb.Append("INF");
                            }
                        }
                        else if (float.IsNaN(f))
                        {
                            sb.Append("NaN");
                        }
                        else
                        {
                            sb.Append(f.ToString("r", CultureInfo.InvariantCulture));
                        }
                    }
                    else if (o.GetType().IsPrimitive || (o is decimal))
                    {
                        IConvertible convertible = o as IConvertible;
                        if (convertible != null)
                        {
                            sb.Append(convertible.ToString(CultureInfo.InvariantCulture));
                        }
                        else
                        {
                            sb.Append(o.ToString());
                        }
                    }
                    else if (o.GetType() == typeof(object))
                    {
                        sb.Append("{}");
                    }
                    else
                    {
                        Type type = o.GetType();
                        if (type.IsEnum)
                        {
                            sb.Append((int)o);
                        }
                        else
                        {
                            try
                            {
                                if (objectsInUse == null)
                                {
                                    objectsInUse = new Dictionary<object, bool>(new ReferenceComparer());
                                }
                                else if (objectsInUse.ContainsKey(o))
                                {
                                    //throw new InvalidOperationException(SR2.GetString("JsonCircularReferenceDetected", new object[] { type.FullName }));
                                    throw new InvalidOperationException("JsonCircularReferenceDetected");
                                }
                                objectsInUse.Add(o, true);
                                IDictionary dictionary = o as IDictionary;
                                if (dictionary != null)
                                {
                                    this.SerializeDictionary(dictionary, sb, depth, objectsInUse);
                                }
                                else
                                {
                                    IEnumerable enumerable = o as IEnumerable;
                                    if (enumerable != null)
                                    {
                                        this.SerializeEnumerable(enumerable, sb, depth, objectsInUse);
                                    }
                                    else
                                    {
                                        Bean(o, sb, depth, objectsInUse);
                                    }
                                }
                            }
                            finally
                            {
                                if (objectsInUse != null)
                                {
                                    objectsInUse.Remove(o);
                                }
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 把一个Bean对象序列化
        /// </summary>
        /// <param name="o"></param>
        /// <param name="sb"></param>
        /// <param name="depth"></param>
        /// <param name="objectsInUse"></param>
        private void Bean(object o, StringBuilder sb, int depth, Dictionary<object, bool> objectsInUse)
        {
            sb.Append("{");
            Type type = o.GetType();
            if (type == null)
            {
                SerializeValueInternal(null, sb, depth + 1, objectsInUse);
            }
            else
            {
                PropertyInfo[] pInfos = type.GetProperties();
                if (pInfos == null || pInfos.Length == 0)
                {
                    SerializeValueInternal(null, sb, depth + 1, objectsInUse);
                }
                else
                {
                    bool flag = false;
                    object[] atts = null;
                    object[] arrtributes = null;
                    bool skipFlag = false;
                    foreach (PropertyInfo info in pInfos)
                    {
                        //if(info.DeclaringType == typeof(Lin.Core.ViewModel)){
                        //    continue;
                        //}

                        //if (info.DeclaringType == typeof(Lin.Core.ViewModel))
                        //{
                        //    continue;
                        //}
                        //if (info.DeclaringType == typeof(Lin.Core.ViewModel.ViewModelProperty))
                        //{
                        //    continue;
                        //}
                        //arrtributes = info.GetCustomAttributes(true);
                        arrtributes = info.GetCustomAttributes(false);
                        skipFlag = false;
                        foreach (object attribute in arrtributes)
                        {
                            if (attribute is JsonSkip)
                            {
                                skipFlag = true;
                                break;
                            }
                        }
                        if (skipFlag)
                        {
                            continue;
                        }
                        if (info.CanRead)
                        {
                            if (flag)
                            {
                                sb.Append(",");
                            }
                            sb.Append("\"");
                            sb.Append(info.Name);
                            sb.Append("\":");

                            atts = info.GetCustomAttributes(true);

                            //try
                            //{
                                SerializeValueInternal(info.GetValue(o, null), sb, depth + 1, objectsInUse);
                            //}
                            //catch (Exception e)
                            //{
                            //    Console.WriteLine(e);
                            //}
                            flag = true;
                        }
                    }
                    if (!flag)
                    {
                        SerializeValueInternal(null, sb, depth + 1, objectsInUse);
                    }
                }
            }
            sb.Append("}");
        }
        // Nested Types
        private class ReferenceComparer : IEqualityComparer<object>
        {
            // Methods
            bool IEqualityComparer<object>.Equals(object x, object y)
            {
                return (x == y);
            }

            int IEqualityComparer<object>.GetHashCode(object obj)
            {
                return obj.GetHashCode();
            }
        }
    }
}
