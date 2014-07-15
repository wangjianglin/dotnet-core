using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Lin.Core.Cache
{
    public interface ICache
    {
        string Put(object obj, string key = null, bool isPermanency = false, bool isCrossDomain = false);
        string Put(FileInfo file, string key = null, bool isPermanency = false);
        string Put(DirectoryInfo directory, string key = null, bool isPermanency = false, bool isRecursive = false);
        object Get(string key);
        void Remove(string key);
        void DeleteRemain();
        void MoveFolderTo(DirectoryInfo directorySource, DirectoryInfo directoryTarget);
        void EliminationSpilthFile(long directorySize);
        void TimelyClearCache(UInt64 timeSpan, long size);
    }
}
