using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//using System.Runtime.Serialization.Json;
using System.Collections;
using System.IO;
using System.Globalization;

namespace Lin.Util.Json
{
    public abstract class JsonValue : IEnumerable
    {
        // Methods
        internal JsonValue()
        {
        }

        private static void CheckIfJsonType(JsonType valueType, JsonType targetType)
        {
            if (valueType != targetType)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidCastException(SR.GetString("CannotCastJsonValue", new object[] { valueType, targetType })));
            }
        }

        internal static void CheckNull(object obj, string parameterName)
        {
            if (obj == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException(parameterName));
            }
        }

        public virtual bool ContainsKey(string key)
        {
            CheckNull(key, "key");
            throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(SR.GetString("UnsupportedOnThisJsonValue", new object[] { base.GetType() })));
        }

        private static T Convert<T>(JsonValue value)
        {
            if (value.Value is T)
            {
                return (T)value.Value;
            }
            byte[] bytes = Encoding.UTF8.GetBytes(value.ToString());
            DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(T));
            return (T)serializer.ReadObject(new MemoryStream(bytes));
        }

        internal static JsonValue Convert(object value)
        {
            if (value == null)
            {
                return null;
            }
            IDictionary dictValue = value as IDictionary;
            if (dictValue != null)
            {
                return new JsonObject(dictValue);
            }
            IList items = value as IList;
            if (items != null)
            {
                return new JsonArray(items);
            }
            return new JsonPrimitive(value);
        }

        public static JsonValue Load(Stream stream)
        {
            CheckNull(stream, "stream");
            Stream stream2 = stream;
            if (!stream.CanSeek)
            {
                stream2 = new JavaScriptDeserializer.BufferedStreamReader(stream);
            }
            Encoding encoding = JavaScriptDeserializer.DetectEncoding(stream2.ReadByte(), stream2.ReadByte());
            stream2.Position = 0L;
            return Parse(new StreamReader(stream2, encoding, true).ReadToEnd());
        }

        public static JsonValue Load(TextReader textReader)
        {
            CheckNull(textReader, "textReader");
            return Parse(textReader.ReadToEnd());
        }

        public static implicit operator JsonValue(bool value)
        {
            return new JsonPrimitive(value);
        }

        public static implicit operator JsonValue(byte value)
        {
            return new JsonPrimitive(value);
        }

        public static implicit operator JsonValue(char value)
        {
            return new JsonPrimitive(value);
        }

        public static implicit operator JsonValue(DateTime value)
        {
            return new JsonPrimitive(value);
        }

        public static implicit operator JsonValue(DateTimeOffset value)
        {
            return new JsonObject(value);
        }

        public static implicit operator JsonValue(decimal value)
        {
            return new JsonPrimitive(value);
        }

        public static implicit operator JsonValue(double value)
        {
            return new JsonPrimitive(value);
        }

        public static implicit operator JsonValue(Guid value)
        {
            return new JsonPrimitive(value);
        }

        public static implicit operator JsonValue(short value)
        {
            return new JsonPrimitive(value);
        }

        public static implicit operator JsonValue(int value)
        {
            return new JsonPrimitive(value);
        }

        public static implicit operator JsonValue(long value)
        {
            return new JsonPrimitive(value);
        }

        public static implicit operator bool(JsonValue value)
        {
            CheckIfJsonType(value.JsonType, JsonType.Boolean);
            return (bool)value.Value;
        }

        public static implicit operator byte(JsonValue value)
        {
            CheckIfJsonType(value.JsonType, JsonType.Number);
            return System.Convert.ToByte(value.Value);
        }

        public static implicit operator char(JsonValue value)
        {
            CheckIfJsonType(value.JsonType, JsonType.String);
            if (value.Value is char)
            {
                return (char)value.Value;
            }
            char[] chArray = ((string)value.Value).ToCharArray();
            if (chArray.Length != 1)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidCastException(SR.GetString("CannotCastStringToChar")));
            }
            return chArray[0];
        }

        public static implicit operator DateTime(JsonValue value)
        {
            CheckIfJsonType(value.JsonType, JsonType.String);
            return Convert<DateTime>(value);
        }

        public static implicit operator DateTimeOffset(JsonValue value)
        {
            CheckIfJsonType(value.JsonType, JsonType.Object);
            Dictionary<string, JsonValue> dictionary = (Dictionary<string, JsonValue>)value.Value;
            if (((dictionary.Count != 2) || !dictionary.ContainsKey("DateTime")) || !dictionary.ContainsKey("OffsetMinutes"))
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidCastException(SR.GetString("CannotCastValue", new object[] { "Object", "DateTimeOffset" })));
            }
            DateTimeOffset offset = new DateTimeOffset(dictionary["DateTime"]);
            return offset.ToOffset(new TimeSpan(0, dictionary["OffsetMinutes"], 0));
        }

        public static implicit operator decimal(JsonValue value)
        {
            CheckIfJsonType(value.JsonType, JsonType.Number);
            return System.Convert.ToDecimal(value.Value);
        }

        public static implicit operator double(JsonValue value)
        {
            CheckIfJsonType(value.JsonType, JsonType.Number);
            return System.Convert.ToDouble(value.Value);
        }

        public static implicit operator Guid(JsonValue value)
        {
            CheckIfJsonType(value.JsonType, JsonType.String);
            return Convert<Guid>(value);
        }

        public static implicit operator short(JsonValue value)
        {
            CheckIfJsonType(value.JsonType, JsonType.Number);
            return System.Convert.ToInt16(value.Value);
        }

        public static implicit operator int(JsonValue value)
        {
            CheckIfJsonType(value.JsonType, JsonType.Number);
            return System.Convert.ToInt32(value.Value);
        }

        public static implicit operator long(JsonValue value)
        {
            CheckIfJsonType(value.JsonType, JsonType.Number);
            return System.Convert.ToInt64(value.Value);
        }

        //[CLSCompliant(false)]
        public static implicit operator sbyte(JsonValue value)
        {
            CheckIfJsonType(value.JsonType, JsonType.Number);
            return System.Convert.ToSByte(value.Value);
        }

        public static implicit operator float(JsonValue value)
        {
            CheckIfJsonType(value.JsonType, JsonType.Number);
            return System.Convert.ToSingle(value.Value);
        }

        public static implicit operator string(JsonValue value)
        {
            if (value == null)
            {
                return null;
            }
            CheckIfJsonType(value.JsonType, JsonType.String);
            if (value.Value is string)
            {
                return (string)value.Value;
            }
            string str = UnescapeJsonString(value.ToString());
            return str.Substring(1, str.Length - 2);
        }

        public static implicit operator TimeSpan(JsonValue value)
        {
            CheckIfJsonType(value.JsonType, JsonType.String);
            return Convert<TimeSpan>(value);
        }

        //[CLSCompliant(false)]
        public static implicit operator ushort(JsonValue value)
        {
            CheckIfJsonType(value.JsonType, JsonType.Number);
            return System.Convert.ToUInt16(value.Value);
        }

        //[CLSCompliant(false)]
        public static implicit operator uint(JsonValue value)
        {
            CheckIfJsonType(value.JsonType, JsonType.Number);
            return System.Convert.ToUInt32(value.Value);
        }

        //[CLSCompliant(false)]
        public static implicit operator ulong(JsonValue value)
        {
            CheckIfJsonType(value.JsonType, JsonType.Number);
            return System.Convert.ToUInt64(value.Value);
        }

        public static implicit operator Uri(JsonValue value)
        {
            if (value == null)
            {
                return null;
            }
            CheckIfJsonType(value.JsonType, JsonType.String);
            return Convert<Uri>(value);
        }

        //[CLSCompliant(false)]
        public static implicit operator JsonValue(sbyte value)
        {
            return new JsonPrimitive(value);
        }

        public static implicit operator JsonValue(float value)
        {
            return new JsonPrimitive(value);
        }

        public static implicit operator JsonValue(string value)
        {
            if (value != null)
            {
                return new JsonPrimitive(value);
            }
            return null;
        }

        public static implicit operator JsonValue(TimeSpan value)
        {
            return new JsonPrimitive(value);
        }

        //[CLSCompliant(false)]
        public static implicit operator JsonValue(ushort value)
        {
            return new JsonPrimitive(value);
        }

        //[CLSCompliant(false)]
        public static implicit operator JsonValue(uint value)
        {
            return new JsonPrimitive(value);
        }

        //[CLSCompliant(false)]
        public static implicit operator JsonValue(ulong value)
        {
            return new JsonPrimitive(value);
        }

        public static implicit operator JsonValue(Uri value)
        {
            if (value != null)
            {
                return new JsonPrimitive(value);
            }
            return null;
        }

        public static JsonValue Parse(string jsonString)
        {
            if (jsonString == null)
            {
                throw new ArgumentNullException("jsonString");
            }
            if (jsonString.Length == 0)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentException(SR.GetString("JsonStringCannotBeEmpty"), "jsonString"));
            }
            JavaScriptObjectDeserializer deserializer = new JavaScriptObjectDeserializer(jsonString, false);
            return Convert(deserializer.BasicDeserialize());
        }

        private static char ParseChar(string value, NumberStyles style)
        {
            char ch;
            int num = ParseInt(value, style);
            try
            {
                ch = System.Convert.ToChar(num);
            }
            catch (OverflowException exception)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidCastException(exception.Message, exception));
            }
            return ch;
        }

        private static int ParseInt(string value, NumberStyles style)
        {
            int num;
            try
            {
                num = int.Parse(value, style, NumberFormatInfo.InvariantInfo);
            }
            catch (ArgumentException exception)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidCastException(exception.Message, exception));
            }
            catch (FormatException exception2)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidCastException(exception2.Message, exception2));
            }
            catch (OverflowException exception3)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidCastException(exception3.Message, exception3));
            }
            return num;
        }

        public abstract void Save(Stream stream);
        public virtual void Save(TextWriter textWriter)
        {
            CheckNull(textWriter, "textWriter");
            using (MemoryStream stream = new MemoryStream())
            {
                this.Save(stream);
                stream.Position = 0L;
                textWriter.Write(new StreamReader(stream).ReadToEnd());
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(SR.GetString("UnsupportedOnThisJsonValue", new object[] { this.JsonType })));
        }

        public override string ToString()
        {
            using (MemoryStream stream = new MemoryStream())
            {
                this.Save(stream);
                stream.Position = 0L;
                return new StreamReader(stream).ReadToEnd();
            }
        }

        private static string UnescapeJsonString(string val)
        {
            if (val == null)
            {
                return null;
            }
            StringBuilder builder = null;
            int startIndex = 0;
            int count = 0;
            for (int i = 0; i < val.Length; i++)
            {
                if (val[i] == '\\')
                {
                    i++;
                    if (builder == null)
                    {
                        builder = new StringBuilder();
                    }
                    builder.Append(val, startIndex, count);
                    switch (val[i])
                    {
                        case '/':
                        case '\\':
                        case '"':
                        case '\'':
                            builder.Append(val[i]);
                            break;

                        case 'b':
                            builder.Append('\b');
                            break;

                        case 'f':
                            builder.Append('\f');
                            break;

                        case 'r':
                            builder.Append('\r');
                            break;

                        case 't':
                            builder.Append('\t');
                            break;

                        case 'u':
                            builder.Append(ParseChar(val.Substring(i + 1, 4), NumberStyles.HexNumber));
                            i += 4;
                            break;

                        case 'n':
                            builder.Append('\n');
                            break;
                    }
                    startIndex = i + 1;
                    count = 0;
                    continue;
                }
                count++;
            }
            if (builder == null)
            {
                return val;
            }
            if (count > 0)
            {
                builder.Append(val, startIndex, count);
            }
            return builder.ToString();
        }

        // Properties
        public virtual int Count
        {
            get
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(SR.GetString("UnsupportedOnThisJsonValue", new object[] { base.GetType() })));
            }
        }

        public virtual JsonValue this[int index]
        {
            get
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(SR.GetString("UnsupportedOnThisJsonValue", new object[] { base.GetType() })));
            }
            set
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(SR.GetString("UnsupportedOnThisJsonValue", new object[] { base.GetType() })));
            }
        }

        public virtual JsonValue this[string key]
        {
            get
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(SR.GetString("UnsupportedOnThisJsonValue", new object[] { base.GetType() })));
            }
            set
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(SR.GetString("UnsupportedOnThisJsonValue", new object[] { base.GetType() })));
            }
        }

        public abstract JsonType JsonType { get; }

        internal abstract object Value { get; }
    }
}
