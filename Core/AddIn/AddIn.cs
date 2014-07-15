using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace Lin.Core.AddIn
{ 
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class AddIn : Attribute
    {
        public AddIn(string name)
        {
            this.Name = name;
            this.Type = null;
            this.Major = 0;
            this.Minor = 0;
        }
        public AddIn(string name, string type)
        {
            this.Major = 0;
            this.Minor = 0;
            this.Name = name;
            if (type != null)
            {
                this.Type = type.Trim();
                if (this.Type == "")
                {
                    this.Type = null;
                }
            }
            else
            {
                this.Type = type;
            }
        }
        public string Type { get; private set; }
        public string Name { get; private set; }
        public string Description { get; set; }
        public string[] Params { get; set; }
        public string Publisher { get; set; }
        [DefaultValue(0.0)]
        public double Location { get; set; }
        public string Version { get; set; }
        /// <summary>
        /// 主版本号
        /// </summary>
        public uint Major{get;set;}
        /// <summary>
        /// 副版本号
        /// </summary>
        public uint Minor { get; set; }
    }
}
