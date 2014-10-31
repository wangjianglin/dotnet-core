using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Lin.Comm.Http
{
    /// <summary>
    /// 
    /// </summary>
    public class File
    {
        internal File() { }
        public FileInfo FileInfo { get; internal set; }
        public string FileName { get; internal set; }

        public DateTime Date { get; internal set; }

        public string ContentType { get; internal set; }
    }
}
