﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lin.Core.Web.Packages;
using System.IO;

namespace Lin.Core.Web.Http
{
    public class FileDownloadPackage : Package
    {
        public FileDownloadPackage()
        {
            this.location = "";
            this.RespType = typeof(string);
        }
        public FileInfo file;
    }
}