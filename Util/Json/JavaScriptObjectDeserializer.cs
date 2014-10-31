using System;
using System.Net;
using System.Globalization;
using System.Text;
using System.Runtime.Serialization;
using System.Collections.Generic;
using System.Collections;

namespace Lin.Util.Json
{
    internal class JavaScriptObjectDeserializer
    {
        // Fields
        private bool _isDataContract;
        internal JavaScriptString _s;
        private const string DateTimePrefix = "\"\\/Date(";
        private const int DateTimePrefixLength = 8;
        private const string DateTimeSuffix = ")\\/\"";
        private const int DateTimeSuffixLength = 4;

        // Methods
        internal JavaScriptObjectDeserializer(string input)
            : this(input, true)
        {
        }

        //[FriendAccessAllowed]
        internal JavaScriptObjectDeserializer(string input, bool isDataContract)
        {
            this._s = new JavaScriptString(input);
            this._isDataContract = isDataContract;
        }

        private void AppendCharToBuilder(char? c, StringBuilder sb)
        {
            if (((c == '"') || (c == '\'')) || (c == '/'))
            {
                sb.Append(c);
            }
            else if (c == 'b')
            {
                sb.Append('\b');
            }
            else if (c == 'f')
            {
                sb.Append('\f');
            }
            else if (c == 'n')
            {
                sb.Append('\n');
            }
            else if (c == 'r')
            {
                sb.Append('\r');
            }
            else if (c == 't')
            {
                sb.Append('\t');
            }
            else
            {
                if (c != 'u')
                {
                    throw new SerializationException(this._s.GetDebugString("ObjectDeserializer_BadEscape"));
                }
                sb.Append((char)int.Parse(this._s.MoveNext(4), NumberStyles.HexNumber, CultureInfo.InvariantCulture));
            }
        }

        //[FriendAccessAllowed]
        internal object BasicDeserialize()
        {
            object obj2 = this.DeserializeInternal(0);
            char? nextNonEmptyChar = this._s.GetNextNonEmptyChar();
            int? nullable3 = nextNonEmptyChar.HasValue ? new int?(nextNonEmptyChar.GetValueOrDefault()) : null;
            if (nullable3.HasValue)
            {
                throw new SerializationException("ObjectDeserializer_IllegalPrimitive");
            }
            return obj2;
        }

        private char CheckQuoteChar(char? c)
        {
            if (c == '\'')
            {
                return c.Value;
            }
            if (c != '"')
            {
                throw new SerializationException(this._s.GetDebugString("ObjectDeserializer_StringNotQuoted"));
            }
            return '"';
        }

        private IDictionary<string, object> DeserializeDictionary(int depth)
        {
            IDictionary<string, object> dictionary = null;
            char? nullable8;
            char? nullable11;
            char? nextNonEmptyChar = this._s.MoveNext();
            if (nextNonEmptyChar != '{')
            {
                throw new SerializationException(this._s.GetDebugString("ObjectDeserializer_UnexpectedToken"));
            }
            bool flag = false;
        Label_0249:
            nullable8 = nextNonEmptyChar = this._s.GetNextNonEmptyChar();
            int? nullable10 = nullable8.HasValue ? new int?(nullable8.GetValueOrDefault()) : null;
            if (nullable10.HasValue)
            {
                this._s.MovePrev();
                if (nextNonEmptyChar == ':')
                {
                    throw new SerializationException(this._s.GetDebugString("ObjectDeserializer_InvalidMemberName"));
                }
                string str = null;
                if (nextNonEmptyChar != '}')
                {
                    str = this.DeserializeMemberName();
                    if (string.IsNullOrEmpty(str))
                    {
                        throw new SerializationException(this._s.GetDebugString("ObjectDeserializer_InvalidMemberName"));
                    }
                    nextNonEmptyChar = this._s.GetNextNonEmptyChar();
                    if (nextNonEmptyChar != ':')
                    {
                        throw new SerializationException(this._s.GetDebugString("ObjectDeserializer_UnexpectedToken"));
                    }
                }
                if (dictionary == null)
                {
                    dictionary = new Dictionary<string, object>();
                    if (string.IsNullOrEmpty(str))
                    {
                        nextNonEmptyChar = this._s.GetNextNonEmptyChar();
                        goto Label_0287;
                    }
                }
                object obj2 = this.DeserializeInternal(depth);
                if (!flag || !str.Equals("__type"))
                {
                    if (dictionary.ContainsKey(str))
                    {
                        throw new SerializationException("JsonDuplicateMemberInInput");
                    }
                    dictionary[str] = obj2;
                    flag = true;
                }
                nextNonEmptyChar = this._s.GetNextNonEmptyChar();
                if (nextNonEmptyChar != '}')
                {
                    if (nextNonEmptyChar != ',')
                    {
                        throw new SerializationException(this._s.GetDebugString("ObjectDeserializer_UnexpectedToken"));
                    }
                    goto Label_0249;
                }
            }
        Label_0287:
            nullable11 = nextNonEmptyChar;
            if ((nullable11.GetValueOrDefault() != '}') || !nullable11.HasValue)
            {
                throw new SerializationException(this._s.GetDebugString("ObjectDeserializer_UnexpectedToken"));
            }
            return dictionary;
        }

        private object DeserializeInternal(int depth)
        {
            char? nextNonEmptyChar = this._s.GetNextNonEmptyChar();
            char? nullable2 = nextNonEmptyChar;
            int? nullable4 = nullable2.HasValue ? new int?(nullable2.GetValueOrDefault()) : null;
            if (!nullable4.HasValue)
            {
                return null;
            }
            this._s.MovePrev();
            if (this.IsNextElementDateTime())
            {
                return this.DeserializeStringIntoDateTime();
            }
            if (IsNextElementObject(nextNonEmptyChar))
            {
                return this.DeserializeDictionary(depth);
            }
            if (IsNextElementArray(nextNonEmptyChar))
            {
                return this.DeserializeList(depth);
            }
            if (IsNextElementString(nextNonEmptyChar))
            {
                return this.DeserializeString();
            }
            return this.DeserializePrimitiveObject();
        }

        private IList DeserializeList(int depth)
        {
            char? nullable5;
            IList list = new List<object>();
            char? nextNonEmptyChar = this._s.MoveNext();
            if (nextNonEmptyChar != '[')
            {
                throw new SerializationException(this._s.GetDebugString("ObjectDeserializer_UnexpectedToken"));
            }
            bool flag = false;
        Label_010F:
            nullable5 = nextNonEmptyChar = this._s.GetNextNonEmptyChar();
            int? nullable7 = nullable5.HasValue ? new int?(nullable5.GetValueOrDefault()) : null;
            if (nullable7.HasValue && (nextNonEmptyChar != ']'))
            {
                this._s.MovePrev();
                object obj2 = this.DeserializeInternal(depth);
                list.Add(obj2);
                flag = false;
                nextNonEmptyChar = this._s.GetNextNonEmptyChar();
                if (nextNonEmptyChar != ']')
                {
                    flag = true;
                    if (nextNonEmptyChar != ',')
                    {
                        throw new SerializationException(this._s.GetDebugString("ObjectDeserializer_UnexpectedToken"));
                    }
                    goto Label_010F;
                }
            }
            if (flag)
            {
                throw new SerializationException(this._s.GetDebugString("ObjectDeserializer_InvalidArrayExtraComma"));
            }
            if (nextNonEmptyChar != ']')
            {
                throw new SerializationException(this._s.GetDebugString("ObjectDeserializer_UnexpectedToken"));
            }
            return list;
        }

        private string DeserializeMemberName()
        {
            char? nextNonEmptyChar = this._s.GetNextNonEmptyChar();
            char? nullable2 = nextNonEmptyChar;
            int? nullable4 = nullable2.HasValue ? new int?(nullable2.GetValueOrDefault()) : null;
            if (!nullable4.HasValue)
            {
                return null;
            }
            this._s.MovePrev();
            if (IsNextElementString(nextNonEmptyChar))
            {
                return this.DeserializeString();
            }
            return this.DeserializePrimitiveToken();
        }
        private readonly static char[] FloatingPointCharacters = new char[] { '.', 'e', 'E' };
        private object DeserializePrimitiveObject()
        {
            string s = this.DeserializePrimitiveToken();
            if (s.ToLower().Equals("null"))
            {
                return null;
            }
            if (s.ToLower().Equals("true"))
            {
                return true;
            }
            if (s.ToLower().Equals("false"))
            {
                return false;
            }
            if (s.ToUpper().Equals("INF"))
            {
                return (float)1.0 / (float)0.0;
            }
            if (s.ToUpper().Equals("-INF"))
            {
                return (float)-1.0 / (float)0.0;
            }
            if (s.IndexOfAny(FloatingPointCharacters) < 0)
            {
                int num;
                long num2;
                if (int.TryParse(s, NumberStyles.Float, CultureInfo.InvariantCulture, out num))
                {
                    return num;
                }
                if (long.TryParse(s, NumberStyles.Float, CultureInfo.InvariantCulture, out num2))
                {
                    return num2;
                }
                if (this._isDataContract)
                {
                    return ParseJsonNumberAsDoubleOrDecimal(s);
                }
            }
            object obj2 = ParseJsonNumberAsDoubleOrDecimal(s);
            if (obj2.GetType() == typeof(string))
            {
                throw new SerializationException("ObjectDeserializer_IllegalPrimitive");
            }
            if (!this._isDataContract)
            {
                return obj2;
            }
            return s;
        }

        private string DeserializePrimitiveToken()
        {
            char? nullable2;
            StringBuilder builder = new StringBuilder();
            char? nullable = null;
        Label_0066:
            nullable2 = nullable = this._s.MoveNext();
            int? nullable4 = nullable2.HasValue ? new int?(nullable2.GetValueOrDefault()) : null;
            if (nullable4.HasValue)
            {
                if ((char.IsLetterOrDigit(nullable.Value) || (nullable.Value == '.')) || (((nullable.Value == '-') || (nullable.Value == '_')) || (nullable.Value == '+')))
                {
                    builder.Append(nullable);
                }
                else
                {
                    this._s.MovePrev();
                    goto Label_00A2;
                }
                goto Label_0066;
            }
        Label_00A2:
            return builder.ToString();
        }

        internal string DeserializeString()
        {
            StringBuilder sb = new StringBuilder();
            bool flag = false;
            char? c = this._s.MoveNext();
            char ch = this.CheckQuoteChar(c);
            while (true)
            {
                char? nullable4 = c = this._s.MoveNext();
                int? nullable6 = nullable4.HasValue ? new int?(nullable4.GetValueOrDefault()) : null;
                if (!nullable6.HasValue)
                {
                    throw new SerializationException(this._s.GetDebugString("ObjectDeserializer_UnterminatedString"));
                }
                if (c == '\\')
                {
                    if (flag)
                    {
                        sb.Append('\\');
                        flag = false;
                    }
                    else
                    {
                        flag = true;
                    }
                }
                else if (flag)
                {
                    this.AppendCharToBuilder(c, sb);
                    flag = false;
                }
                else
                {
                    char? nullable3 = c;
                    int num = ch;
                    if ((nullable3.GetValueOrDefault() == num) && nullable3.HasValue)
                    {
                        return sb.ToString();
                    }
                    sb.Append(c);
                }
            }
        }
        private readonly static long unixEpochTicks = (new DateTime(0x7b2, 1, 1, 0, 0, 0, DateTimeKind.Utc)).Ticks;
        private object DeserializeStringIntoDateTime()
        {
            long num = 0;
            object obj2 = null;
            string str = this.DeserializeString();
            string s = str.Substring(6, str.Length - 8);
            DateTimeKind utc = DateTimeKind.Utc;
            int index = s.IndexOf('+', 1);
            if (index == -1)
            {
                index = s.IndexOf('-', 1);
            }
            if (index != -1)
            {
                utc = DateTimeKind.Local;
                s = s.Substring(0, index);
            }
            try
            {
                num = long.Parse(s, NumberStyles.Integer, CultureInfo.InvariantCulture);
            }
            catch (ArgumentException)
            {
                //throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(XmlExceptionHelper.CreateConversionException(s, "Int64", exception));
            }
            catch (FormatException)
            {
                //throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(XmlExceptionHelper.CreateConversionException(s, "Int64", exception2));
            }
            catch (OverflowException)
            {
                //throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(XmlExceptionHelper.CreateConversionException(s, "Int64", exception3));
            }
            long ticks = (num * 0x2710L) + unixEpochTicks;
            try
            {
                DateTime time = new DateTime(ticks, DateTimeKind.Utc);
                switch (utc)
                {
                    case DateTimeKind.Unspecified:
                        return DateTime.SpecifyKind(time.ToLocalTime(), DateTimeKind.Unspecified);

                    case DateTimeKind.Local:
                        return time.ToLocalTime();
                }
                obj2 = time;
            }
            catch (ArgumentException)
            {
                //throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(XmlExceptionHelper.CreateConversionException(s, "DateTime", exception4));
            }
            return obj2;
        }

        private static bool IsNextElementArray(char? c)
        {
            return (c == '[');
        }

        private bool IsNextElementDateTime()
        {
            string a = this._s.MoveNext(8);
            if (a != null)
            {
                this._s.MovePrev(8);
                return string.Equals(a, "\"\\/Date(", StringComparison.Ordinal);
            }
            return false;
        }

        private static bool IsNextElementObject(char? c)
        {
            return (c == '{');
        }

        private static bool IsNextElementString(char? c)
        {
            return ((c == '"') || (c == '\''));
        }

        internal static object ParseJsonNumberAsDoubleOrDecimal(string input)
        {
            decimal num;
            double num2;
            if (decimal.TryParse(input, NumberStyles.Float, CultureInfo.InvariantCulture, out num) && (num != 0M))
            {
                return num;
            }
            if (double.TryParse(input, NumberStyles.Float, CultureInfo.InvariantCulture, out num2))
            {
                return num2;
            }
            return input;
        }
    }
}
