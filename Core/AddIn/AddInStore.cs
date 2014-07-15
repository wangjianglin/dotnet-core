using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.IO;
using Lin.Core.Utils;

namespace Lin.Core.AddIn
{
    public static class AddInStore
    {
        public static void Update(string path)
        {
            Lin.Core.Controls.AttributeStore.Update(path);
        }

        //IDictionary<Type,IList<Type> addIns;
        public static IList<AddInToken> FindAddIns()
        {
            IList<Type> addIns = Lin.Core.Controls.AttributeStore.FindAttributes(typeof(AddIn));
            List<Attribute> attributes;
            List<AddInToken> tokens = new List<AddInToken>();
            foreach (Type type in addIns)
            {
                attributes = Lin.Core.Controls.AttributeStore.FindTypeAttributes(type, typeof(Lin.Core.AddIn.AddIn));
                foreach (object obj in attributes)
                {
                    if (obj is Lin.Core.AddIn.AddIn)
                    {
                        Lin.Core.AddIn.AddIn addin = (Lin.Core.AddIn.AddIn)obj;
                        AddInToken token = new AddInToken(type);
                        addin.CopyProperty(token);
                        tokens.Add(token);
                    }
                }
            }
            tokens.Sort(new SortClass());
            return tokens;
        }

        private class SortClass : Comparer<AddInToken>
        {

            public override int Compare(AddInToken x, AddInToken y)
            {
                System.Math.Sign(x.Location - y.Location);
                return System.Math.Sign(x.Location - y.Location);
            }
        }


        //if (assemblys != null && assemblys.Count > 0)
        //{
        //    foreach (Assembly assembly in assemblys)
        //    {
        //        try
        //        {
        //            Type[] types = assembly.GetTypes(); 
        //            foreach (Type type in types)
        //            { 
        //                object[] objs = type.GetCustomAttributes(typeof(Lin.Core.AddIn.AddIn), false);
        //                foreach (object obj in objs)
        //                {
        //                    if (obj is Lin.Core.AddIn.AddIn)
        //                    {
        //                        Lin.Core.AddIn.AddIn addin = (Lin.Core.AddIn.AddIn)obj;
        //                        AddInToken token = new AddInToken(type);
        //                        //token.Name = addin.Name;
        //                        //token.Description = addin.Description;
        //                        //token.Publisher = addin.Publisher;
        //                        //token.Version = addin.Version;
        //                        //token.Type = addin.Type;
        //                        //token.Major = addin.Major;
        //                        //token.Minor = addin.Minor;
        //                        //object objToken = token.Content;
        //                        addin.CopyProperty(token);
        //                        tokens.Add(token); 
        //                    }
        //                }
        //            }
        //        }
        //        catch (Exception) { }
        //    }
        //}
    }
}
