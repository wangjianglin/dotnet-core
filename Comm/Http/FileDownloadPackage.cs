﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Lin.Comm.Http.Packages;

namespace Lin.Comm.Http
{
    public class FileDownloadPackage : HttpPackage
    {
        public FileDownloadPackage()
        {
            this.location = "";
            this.RespType = typeof(string);
        }
        public FileInfo file;
    }
}
