using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lin.Plugin.MenuStructure
{
    public class ViewToken
    {
        private Type _ViewAttributeType;
        public ViewToken(Type type)
        {
            _ViewAttributeType = type;
        }
        public Type ViewType { get { return _ViewAttributeType; } }
        public string Menu { get; set; }
        public string Name { get; set; }
        private object _Content;
        public object Content
        {
            get
            {
                if (_Content == null)
                {
                    _Content = Activator.CreateInstance(_ViewAttributeType);

                }
                return _Content;
            }
        }
        public string Type { get; set; }
        public string Description { get; set; }
        public string[] Params { get; set; }
        public string Icon { get; set; }
        public bool IsNet { get; set; }
        public bool IsVisibility { get; set; }
        public double Location { get; set; }
        public string MenuShortcutKey { get; set; }
        public string Publisher { get; set; }
        public uint Major { get; set; }
        public uint Minor { get; set; }

        public IList<Attribute> Attributes { get; set; }
    }
}
