using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
//using System.Runtime.Serialization.Json;

namespace Lin.Core.Web.Json
{
    public class JsonPrimitive : JsonValue
    {
        // Fields
        private JsonType jsonType;
        private object value;

        // Methods
        public JsonPrimitive(bool value)
        {
            this.jsonType = JsonType.Boolean;
            this.value = value;
        }

        public JsonPrimitive(byte value)
        {
            this.jsonType = JsonType.Number;
            this.value = value;
        }

        public JsonPrimitive(char value)
        {
            this.jsonType = JsonType.String;
            this.value = value;
        }

        public JsonPrimitive(DateTime value)
        {
            this.jsonType = JsonType.String;
            this.value = value;
        }

        public JsonPrimitive(decimal value)
        {
            this.jsonType = JsonType.Number;
            this.value = value;
        }

        public JsonPrimitive(double value)
        {
            this.jsonType = JsonType.Number;
            this.value = value;
        }

        public JsonPrimitive(Guid value)
        {
            this.jsonType = JsonType.String;
            this.value = value;
        }

        public JsonPrimitive(short value)
        {
            this.jsonType = JsonType.Number;
            this.value = value;
        }

        public JsonPrimitive(int value)
        {
            this.jsonType = JsonType.Number;
            this.value = value;
        }

        public JsonPrimitive(long value)
        {
            this.jsonType = JsonType.Number;
            this.value = value;
        }

        internal JsonPrimitive(object value)
        {
            JsonValue.CheckNull(value, "value");
            Type type = value.GetType();
            if (type == typeof(bool))
            {
                this.jsonType = JsonType.Boolean;
            }
            else if ((((type == typeof(byte)) || (type == typeof(sbyte))) || ((type == typeof(decimal)) || (type == typeof(double)))) || ((((type == typeof(short)) || (type == typeof(ushort))) || ((type == typeof(int)) || (type == typeof(uint)))) || (((type == typeof(long)) || (type == typeof(ulong))) || (type == typeof(float)))))
            {
                this.jsonType = JsonType.Number;
            }
            else
            {
                if ((((type != typeof(string)) && (type != typeof(char))) && ((type != typeof(DateTime)) && (type != typeof(TimeSpan)))) && ((type != typeof(Uri)) && (type != typeof(Guid))))
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentException(SR.GetString("TypeNotSupportedOnJsonPrimitive", new object[] { type }), "value"));
                }
                this.jsonType = JsonType.String;
            }
            this.value = value;
        }

        //[CLSCompliant(false)]
        public JsonPrimitive(sbyte value)
        {
            this.jsonType = JsonType.Number;
            this.value = value;
        }

        public JsonPrimitive(float value)
        {
            this.jsonType = JsonType.Number;
            this.value = value;
        }

        public JsonPrimitive(string value)
        {
            JsonValue.CheckNull(value, "value");
            this.jsonType = JsonType.String;
            this.value = value;
        }

        public JsonPrimitive(TimeSpan value)
        {
            this.jsonType = JsonType.String;
            this.value = value;
        }

        //[CLSCompliant(false)]
        public JsonPrimitive(ushort value)
        {
            this.jsonType = JsonType.Number;
            this.value = value;
        }

        //[CLSCompliant(false)]
        public JsonPrimitive(uint value)
        {
            this.jsonType = JsonType.Number;
            this.value = value;
        }

        //[CLSCompliant(false)]
        public JsonPrimitive(ulong value)
        {
            this.jsonType = JsonType.Number;
            this.value = value;
        }

        public JsonPrimitive(Uri value)
        {
            JsonValue.CheckNull(value, "value");
            this.jsonType = JsonType.String;
            this.value = value;
        }

        public override void Save(Stream stream)
        {
            JsonValue.CheckNull(stream, "stream");
            new DataContractJsonSerializer(this.Value.GetType()).WriteObject(stream, this.Value);
        }

        // Properties
        public sealed override JsonType JsonType
        {
            get
            {
                return this.jsonType;
            }
        }

        internal override object Value
        {
            get
            {
                return this.value;
            }
        }
    }
}
