using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Resources;
using System.Globalization;
using System.Threading;

namespace Lin.Core.Web.Json
{
    internal sealed class SR
    {
        // Fields
        internal const string CannotCastJsonValue = "CannotCastJsonValue";
        internal const string CannotCastStringToChar = "CannotCastStringToChar";
        internal const string CannotCastValue = "CannotCastValue";
        internal const string JsonInvalidBytes = "JsonInvalidBytes";
        internal const string JsonStringCannotBeEmpty = "JsonStringCannotBeEmpty";
        private static SR loader = null;
        private ResourceManager resources;
        private static object s_InternalSyncObject;
        internal const string TypeNotSupportedOnJsonPrimitive = "TypeNotSupportedOnJsonPrimitive";
        internal const string UnsupportedOnThisJsonValue = "UnsupportedOnThisJsonValue";

        // Methods
        internal SR()
        {
            this.resources = new ResourceManager("System.Json", base.GetType().Assembly);
        }

        private static SR GetLoader()
        {
            if (loader == null)
            {
                lock (InternalSyncObject)
                {
                    if (loader == null)
                    {
                        loader = new SR();
                    }
                }
            }
            return loader;
        }

        public static object GetObject(string name)
        {
            SR loader = GetLoader();
            if (loader == null)
            {
                return null;
            }
            return loader.resources.GetObject(name, Culture);
        }

        public static string GetString(string name)
        {
            SR loader = GetLoader();
            if (loader == null)
            {
                return null;
            }
            return loader.resources.GetString(name, Culture);
        }

        public static string GetString(string name, out bool usedFallback)
        {
            usedFallback = false;
            return GetString(name);
        }

        public static string GetString(string name, params object[] args)
        {
            SR loader = GetLoader();
            if (loader == null)
            {
                return null;
            }
            string format = loader.resources.GetString(name, Culture);
            if ((args == null) || (args.Length <= 0))
            {
                return format;
            }
            for (int i = 0; i < args.Length; i++)
            {
                string str2 = args[i] as string;
                if ((str2 != null) && (str2.Length > 0x400))
                {
                    args[i] = str2.Substring(0, 0x3fd) + "...";
                }
            }
            return string.Format(CultureInfo.CurrentCulture, format, args);
        }

        // Properties
        private static CultureInfo Culture
        {
            get
            {
                return null;
            }
        }

        private static object InternalSyncObject
        {
            get
            {
                if (s_InternalSyncObject == null)
                {
                    object obj2 = new object();
                    Interlocked.CompareExchange<object>(ref s_InternalSyncObject, obj2, null);
                }
                return s_InternalSyncObject;
            }
        }

        public static ResourceManager Resources
        {
            get
            {
                return GetLoader().resources;
            }
        }
    }
}
