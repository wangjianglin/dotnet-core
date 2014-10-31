using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Collections;

namespace Lin.Util.Json
{
    public class JsonArray : JsonValue, IList<JsonValue>, ICollection<JsonValue>, IEnumerable<JsonValue>, IEnumerable
    {
        // Fields
        private List<JsonValue> values;

        // Methods
        public JsonArray(IEnumerable<JsonValue> items)
        {
            this.values = new List<JsonValue>();
            JsonValue.CheckNull(items, "items");
            this.values.AddRange(items);
        }

        internal JsonArray(IList items)
        {
            this.values = new List<JsonValue>();
            JsonValue.CheckNull(items, "items");
            IEnumerator enumerator = items.GetEnumerator();
            while (enumerator.MoveNext())
            {
                this.values.Add(JsonValue.Convert(enumerator.Current));
            }
        }

        public JsonArray(params JsonValue[] items)
        {
            this.values = new List<JsonValue>();
            if (items != null)
            {
                this.values.AddRange(items);
            }
        }

        public void Add(JsonValue item)
        {
            this.values.Add(item);
        }

        public void AddRange(IEnumerable<JsonValue> items)
        {
            JsonValue.CheckNull(items, "items");
            this.values.AddRange(items);
        }

        public void AddRange(params JsonValue[] items)
        {
            if (items != null)
            {
                this.values.AddRange(items);
            }
        }

        public void Clear()
        {
            this.values.Clear();
        }

        public bool Contains(JsonValue item)
        {
            return this.values.Contains(item);
        }

        public void CopyTo(JsonValue[] array, int arrayIndex)
        {
            JsonValue.CheckNull(array, "array");
            this.values.CopyTo(array, arrayIndex);
        }

        public int IndexOf(JsonValue item)
        {
            return this.values.IndexOf(item);
        }

        public void Insert(int index, JsonValue item)
        {
            if (index < 0)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("index"));
            }
            this.values.Insert(index, item);
        }

        public bool Remove(JsonValue item)
        {
            return this.values.Remove(item);
        }

        public void RemoveAt(int index)
        {
            if ((index < 0) || (index >= this.values.Count))
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("index"));
            }
            this.values.RemoveAt(index);
        }

        public override void Save(Stream stream)
        {
            JsonValue.CheckNull(stream, "stream");
            StreamWriter textWriter = new StreamWriter(stream);
            textWriter.Write('[');
            for (int i = 0; i < this.values.Count; i++)
            {
                if (i != 0)
                {
                    textWriter.Write(',');
                }
                JsonValue value2 = this.values[i];
                if (value2 == null)
                {
                    textWriter.Write("null");
                }
                else
                {
                    value2.Save(textWriter);
                }
            }
            textWriter.Write(']');
            textWriter.Flush();
        }

        IEnumerator<JsonValue> IEnumerable<JsonValue>.GetEnumerator()
        {
            return this.values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.values.GetEnumerator();
        }

        // Properties
        public override int Count
        {
            get
            {
                return this.values.Count;
            }
        }

        public bool IsReadOnly
        {
            get
            {
                return ((IList)this.values).IsReadOnly;
            }
        }

        public override JsonValue this[int index]
        {
            get
            {
                return this.values[index];
            }
            set
            {
                this.values[index] = value;
            }
        }

        public sealed override JsonType JsonType
        {
            get
            {
                return JsonType.Array;
            }
        }

        internal override object Value
        {
            get
            {
                return this.values;
            }
        }
    }
}
