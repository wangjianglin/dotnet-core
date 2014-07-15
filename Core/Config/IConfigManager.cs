using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace Lin.Core.Config
{
   public interface IConfigManager:INotifyPropertyChanged
    {
       string[] Section{get;}
       string[] Values{get;}
       string[] Names{get;}
       string this[string name] { get; set; }
    }
}
