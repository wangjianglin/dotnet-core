using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.IO;

namespace Lin.Core.Web.Json
{
    public class JsonObject : JsonValue, IDictionary<string, JsonValue>, ICollection<KeyValuePair<string, JsonValue>>, IEnumerable<KeyValuePair<string, JsonValue>>, IEnumerable
    {
        // Fields
        private Dictionary<string, JsonValue> values;

        // Methods
        public JsonObject(IEnumerable<KeyValuePair<string, JsonValue>> items)
        {
            this.values = new Dictionary<string, JsonValue>();
            JsonValue.CheckNull(items, "items");
            this.AddRange(items);
        }

        internal JsonObject(IDictionary dictValue)
        {
            this.values = new Dictionary<string, JsonValue>();
            JsonValue.CheckNull(dictValue, "dictValue");
            foreach (DictionaryEntry entry in dictValue)
            {
                this.values.Add((string)entry.Key, JsonValue.Convert(entry.Value));
            }
        }

        public JsonObject(params KeyValuePair<string, JsonValue>[] items)
        {
            this.values = new Dictionary<string, JsonValue>();
            if (items != null)
            {
                this.AddRange(items);
            }
        }

        public JsonObject(DateTimeOffset dto)
        {
            this.values = new Dictionary<string, JsonValue>();
            this.values.Add("DateTime", dto.UtcDateTime);
            this.values.Add("OffsetMinutes", (int)dto.Offset.TotalMinutes);
        }

        public void Add(KeyValuePair<string, JsonValue> item)
        {
            JsonValue.CheckNull(item, "item");
            this.values.Add(item.Key, item.Value);
        }

        public void Add(string key, JsonValue value)
        {
            JsonValue.CheckNull(key, "key");
            this.values.Add(key, value);
        }

        public void AddRange(params KeyValuePair<string, JsonValue>[] items)
        {
            JsonValue.CheckNull(items, "items");
            foreach (KeyValuePair<string, JsonValue> pair in items)
            {
                this.values.Add(pair.Key, pair.Value);
            }
        }

        public void AddRange(IEnumerable<KeyValuePair<string, JsonValue>> items)
        {
            JsonValue.CheckNull(items, "items");
            foreach (KeyValuePair<string, JsonValue> pair in items)
            {
                this.values.Add(pair.Key, pair.Value);
            }
        }

        public void Clear()
        {
            this.values.Clear();
        }

        public override bool ContainsKey(string key)
        {
            JsonValue.CheckNull(key, "key");
            return this.values.ContainsKey(key);
        }

        public void CopyTo(KeyValuePair<string, JsonValue>[] array, int arrayIndex)
        {
            JsonValue.CheckNull(array, "array");
            //this.values.
            //this.values.CopyTo(array, arrayIndex);
        }

        public IEnumerator<KeyValuePair<string, JsonValue>> GetEnumerator()
        {
            return this.values.GetEnumerator();
        }

        public bool Remove(string key)
        {
            JsonValue.CheckNull(key, "key");
            return this.values.Remove(key);
        }

        public override void Save(Stream stream)
        {
            JsonValue.CheckNull(stream, "stream");
            StreamWriter textWriter = new StreamWriter(stream);
            bool flag = true;
            textWriter.Write('{');
            foreach (KeyValuePair<string, JsonValue> pair in this.values)
            {
                if (!flag)
                {
                    textWriter.Write(',');
                }
                textWriter.Write(new JsonPrimitive(pair.Key).ToString());
                textWriter.Write(':');
                if (pair.Value == null)
                {
                    textWriter.Write("null");
                }
                else
                {
                    pair.Value.Save(textWriter);
                }
                flag = false;
            }
            textWriter.Write('}');
            textWriter.Flush();
        }

        bool ICollection<KeyValuePair<string, JsonValue>>.Contains(KeyValuePair<string, JsonValue> item)
        {
            JsonValue.CheckNull(item, "item");
            return this.values.Contains(item);
        }

        bool ICollection<KeyValuePair<string, JsonValue>>.Remove(KeyValuePair<string, JsonValue> item)
        {
            JsonValue.CheckNull(item, "item");
            //return this.values.Remove(item);
            return this.values.Remove(item.Key);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.values.GetEnumerator();
        }

        public bool TryGetValue(string key, out JsonValue value)
        {
            JsonValue.CheckNull(key, "key");
            return this.values.TryGetValue(key, out value);
        }

        // Properties
        public sealed override int Count
        {
            get
            {
                return this.values.Count;
            }
        }

        public sealed override JsonValue this[string key]
        {
            get
            {
                return this.values[key];
            }
            set
            {
                this.values[key] = value;
            }
        }

        public sealed override JsonType JsonType
        {
            get
            {
                return JsonType.Object;
            }
        }

        public ICollection<string> Keys
        {
            get
            {
                return this.values.Keys;
            }
        }

        bool ICollection<KeyValuePair<string, JsonValue>>.IsReadOnly
        {
            get
            {
                //return this.values.IsReadOnly;
                return true;
            }
        }

        internal override object Value
        {
            get
            {
                return this.values;
            }
        }

        public ICollection<JsonValue> Values
        {
            get
            {
                return this.values.Values;
            }
        }
    }
}
