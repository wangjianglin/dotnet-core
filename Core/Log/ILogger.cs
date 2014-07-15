using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lin.Core.Log
{
    public interface ILogger
    {
        void LogRecord(string logString, LogLevel level = LogLevel.INFO);
        void Error(string error);
        void Info(string info);
        void Warning(string warning);
        void Debug(string debug);
    }
}
