using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lin.Plugin.ComponentAttribute
{
    public class FluentMenuItem : ButtonAttribute,IList<FluentMenuItem>
    {
        public List<FluentMenuItem> MenuItems = new List<FluentMenuItem>();

        #region 实现IList接口

        public int IndexOf(FluentMenuItem item)
        {
            return MenuItems.IndexOf(item);
        }

        public void Insert(int index, FluentMenuItem item)
        {
            MenuItems.Insert(index, item);
        }

        public void RemoveAt(int index)
        {
            MenuItems.RemoveAt(index);
        }

        public FluentMenuItem this[int index]
        {
            get
            {
                return MenuItems[index];
            }
            set
            {
                MenuItems[index] = this;
            }
        }

        public void Add(FluentMenuItem item)
        {
            MenuItems.Add(item);
        }

        public void Clear()
        {
            MenuItems.Clear();
        }

        public bool Contains(FluentMenuItem item)
        {
            return MenuItems.Contains(item);
        }

        public void CopyTo(FluentMenuItem[] array, int arrayIndex)
        {
            MenuItems.CopyTo(array, arrayIndex);
        }

        public int Count
        {
            get { return MenuItems.Count; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public bool Remove(FluentMenuItem item)
        {
            return MenuItems.Remove(item);
        }

        public IEnumerator<FluentMenuItem> GetEnumerator()
        {
            return MenuItems.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return MenuItems.GetEnumerator();
        }

        #endregion
    }
}
