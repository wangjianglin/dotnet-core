using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lin.Util
{
    /// <summary>
    /// 
    /// </summary>
    public class IndexProperty<K, V> : ReadOnlyIndexProperty<K,V>
    {
        private System.Action<K, V> set;
        public IndexProperty(System.Func<K, V> get, System.Action<K, V> set = null, System.Func<int> length = null, System.Func<K[]> keys = null, System.Func<V[]> values = null)
            :base(get,length,keys,values)
        {
            this.set = set;
        }

        public new V this[K name]
        {
            get { return base[name]; }
            set { set(name, value); }
        }

    }

    public class ReadOnlyIndexProperty<K, V>
    {
        private System.Func<K, V> get;
        private System.Func<int> length;

        private System.Func<K[]> keys;
        private System.Func<V[]> values;

        public ReadOnlyIndexProperty(System.Func<K, V> get, System.Func<int> length = null,System.Func<K[]> keys = null,System.Func<V[]> values = null)
        {
            this.get = get;
            this.length = length;
            this.keys = keys;
            this.values = values;
        }

        public V this[K name]
        {
            get { return get(name); }
        }

        public int Length { get { return this.length(); } }

        public K[] Keys { get { return keys(); } }

        public V[] Values { get { return values(); } }
    }
}
