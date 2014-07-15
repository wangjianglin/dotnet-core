using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lin.Plugin
{
    /// <summary>
    /// 
    /// </summary>
    public class IndexProperty<K, V>
    {
        System.Func<K, V> get;
        System.Action<K, V> set;
        public IndexProperty(System.Func<K, V> get, System.Action<K, V> set = null)
        {
            this.get = get;
            this.set = set;
        }

        public IndexProperty(System.Func<K, V> get)
        {
            this.get = get;
        }

        public V this[K name]
        {
            get { return get(name); }
            set { set(name, value); }
        }
    }

    public class ReadOnlyIndexProperty<K, V>
    {
        System.Func<K, V> get;
        
        public ReadOnlyIndexProperty(System.Func<K, V> get)
        {
            this.get = get;
        }

        public V this[K name]
        {
            get { return get(name); }
        }
    }

    public class WriteOnlyIndexProperty<K, V>
    {
        System.Action<K, V> set;
        public WriteOnlyIndexProperty(System.Action<K, V> set = null)
        {
            this.set = set;
        }


        public V this[K name]
        {
            set { set(name, value); }
        }
    }
}
