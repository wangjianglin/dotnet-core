using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.IO;
using System.Collections;

namespace Lin.Core.Web.Json
{
    /// <summary>
    /// 完成对Json格式的序列化与反序列化操作
    /// </summary>
    public class JsonUtil
    {
        /// <summary>
        /// String类型转成double、float、int
        /// </summary>
      

        public static string Serialize(object obj)
        {
            return Serialize(obj, Encoding.Default);
        }
        public static string Serialize(object obj, Encoding encodeing)
        {
            if (obj == null)
            {
                return "null";
            }
            Lin.Core.Web.Json.DataContractJsonSerializer dc = new Lin.Core.Web.Json.DataContractJsonSerializer(obj.GetType());
            MemoryStream ms = new MemoryStream();
            dc.WriteObject(ms, obj);
            byte[] tmp = ms.ToArray();
            return encodeing.GetString(tmp, 0, tmp.Length);
        }

        /// <summary>
        /// 把json格式的数据反序列化成JsonValue 
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        public static object Deserialize(string json)
        {
            JsonReader reader = new JsonReader();
            return reader.read(json);
        }

        /// <summary>
        /// 把json格式的数据反序列化成 T 类型的对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="json"></param>
        /// <returns></returns>
        public static T Deserialize<T>(string json)
        {
            object obj = Deserialize(json);
            if (obj == null)
            {
                return default(T);
            }
            return (T)Deserialize(obj as JsonValue, typeof(T));
        }

        /// <summary>
        /// 实现JsonValue到返回类型 T 的转换
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        internal static T Deserialize<T>(JsonValue obj)
        {
            return (T)Deserialize(obj, typeof(T));
        }

        private static object PrimitiveDeserialize(string s, Type type)
        {
            if (type == typeof(double))
            {
                return double.Parse(s);
            }
            else if (type == typeof(int))
            {
                return int.Parse(s);
            }
            return s;
        }
        /// <summary>
        /// 实现JsonValue到返回类型type的转换
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        internal static object Deserialize(JsonValue obj, Type type)
        {
            if (obj == null)
            {
                return null;
            }

            if (typeof(DateTime).IsAssignableFrom(type))
            {
                return DateTime.ParseExact(obj.ToString().Replace("\"",""), "yyyy-MM-dd HH:mm:ss",null);
            }

            //如果obj为基本类型，直接返回对应的value
            if (obj is JsonPrimitive)
            {
                return obj.Value;
            }


            //如果是数组类型
            if (type.IsArray)
            {
                JsonArray jsonArray = obj as JsonArray;
                Type elementType = type.GetElementType();
                Array arrayValue = Array.CreateInstance(elementType, jsonArray.Count);
                object tmpValue = null;
                for (int n = 0; n < jsonArray.Count; n++)
                {
                    tmpValue = Deserialize(jsonArray[n], elementType);
                    arrayValue.SetValue(tmpValue, n);
                }
                return arrayValue;
            }

            //得到要返回数据类型的泛型信息
            Type[] types = type.GetGenericArguments();

            //反序列化后的对象
            object tmpObj = null;

            //以下根据不同的返回类型，构造相应的对象，注意，下面的判断顺序不能更改
            if (type.IsInterface)
            {
                if (types != null && types.Length == 2)//如果有两个泛型参数，也就是一个泛型Map对象
                {
                    object tmpDict = null;
                    Type listType = typeof(Dictionary<,>).MakeGenericType(types);
                    tmpDict = Activator.CreateInstance(listType);
                    MethodInfo mInfo = listType.GetMethod("Add");
                    JsonObject jObject = obj as JsonObject;
                    if (jObject != null)
                    {
                        object tmpValue = null;
                        object tmpKey = null;
                        foreach (KeyValuePair<string, JsonValue> jGenericValue in jObject)
                        {
                            tmpValue = Deserialize(jGenericValue.Value, types[1]);
                            tmpKey = jGenericValue.Key;
                            if (tmpKey is JsonPrimitive)
                            {
                                tmpKey = (tmpKey as JsonPrimitive).Value;
                            }
                            tmpKey = PrimitiveDeserialize(tmpKey as string, types[0]);
                            mInfo.Invoke(tmpDict, new object[] { tmpKey, tmpValue });
                        }
                    }
                    tmpObj = tmpDict;
                }
                else if (typeof(IDictionary).IsAssignableFrom(type))//如果是一个无泛型Map对象
                {
                    Hashtable tmpDict = new Hashtable();
                    JsonObject jObject = obj as JsonObject;
                    if (jObject != null)
                    {
                        object tmpValue = null;
                        foreach (KeyValuePair<string, JsonValue> jGenericValue in jObject)
                        {
                            tmpValue = Deserialize(jGenericValue.Value, typeof(IDictionary));
                            tmpDict.Add(jGenericValue.Key, tmpValue);
                        }
                    }
                    tmpObj = tmpDict;
                }
                else if (types != null && types.Length == 1)//如果只有一个泛型参数，也就是一个泛型列表
                {
                    object tmpList = null;
                    Type listType = typeof(List<>).MakeGenericType(types);
                    tmpList = Activator.CreateInstance(listType);
                    MethodInfo mInfo = listType.GetMethod("Add");
                    JsonArray jArray = obj as JsonArray;
                    if (jArray != null)
                    {
                        object tmpValue = null;
                        foreach (JsonValue jGenericValue in jArray)
                        {
                            tmpValue = Deserialize(jGenericValue, types[0]);
                            mInfo.Invoke(tmpList, new object[] { tmpValue });
                        }
                    }
                    tmpObj = tmpList;
                }
                else if (typeof(IEnumerable).IsAssignableFrom(type))//如果是一个无泛型列表
                {
                    ArrayList tmpList = new ArrayList();
                    JsonArray jArray = obj as JsonArray;
                    if (jArray != null)
                    {
                        object tmpValue = null;
                        foreach (JsonValue jGenericValue in jArray)
                        {
                            tmpValue = Deserialize(jGenericValue, typeof(IDictionary));
                            tmpList.Add(tmpValue);
                        }
                    }
                    tmpObj = tmpList;
                }
            }
            else if (type.IsClass)//如果是一个可实例化的类型，并且要有无参构造函数
            {
                tmpObj = Activator.CreateInstance(type);
                if (obj.JsonType == JsonType.Object)
                {
                    JsonObject jObj = obj as JsonObject;
                    object tmpValue;
                    foreach (KeyValuePair<string, JsonValue> jValue in jObj)
                    {
                        PropertyInfo pInfo = type.GetProperty(jValue.Key);
                        if (pInfo == null || !pInfo.CanWrite)
                        {
                            continue;
                        }
                        Type pType = pInfo.PropertyType;
                        tmpValue = Deserialize(jValue.Value, pType);
                        if (pType.IsEnum)
                        {
                            tmpValue = Enum.Parse(pType, tmpValue.ToString());
                        }
                        if (tmpValue is System.Decimal)
                        {
                            System.Decimal d = ((System.Decimal)tmpValue);
                            pInfo.SetValue(tmpObj, ParseValue(pType, d), null);
                            continue;
                        }
                        pInfo.SetValue(tmpObj, tmpValue, null);
                    }
                }
            }
            return tmpObj;
        }

        /// <summary>
        /// 处理基本数据类型
        /// </summary>
        /// <param name="type"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        private static object ParseValue(Type type, System.Decimal value)
        {
            if (type == typeof(System.Double))
            {
                return (System.Double)value;
            }
            else if (type == typeof(System.Byte))
            {
                return (System.Byte)value;
            }
            else if (type == typeof(System.Char))
            {
                return (System.Char)value;
            }
            else if (type == typeof(System.Decimal))
            {
                return value;
            }
            else if (type == typeof(System.Int16))
            {
                return (System.Int16)value;
            }
            else if (type == typeof(System.Int32))
            {
                return (System.Int32)value;
            }
            else if (type == typeof(System.Int64))
            {
                return (System.Int64)value;
            }
            else if (type == typeof(System.String))
            {
                return value.ToString();
            }
            else if (type == typeof(System.UInt16))
            {
                return (System.UInt16)value;
            }
            else if (type == typeof(System.UInt32))
            {
                return (System.UInt32)value;
            }
            else if (type == typeof(System.UInt64))
            {
                return (System.UInt64)value;
            }
            return value;
        }
    }
}
