using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AD.Test.Core
{
    public class TestContextProxy
    {
        public TestContextProxy()
        {
            if (Lin.Core.ViewModel.Context.Global.IsNet == true)
            {
                Console.WriteLine("网络链接顺通");
            }
            else
            {
                Console.WriteLine("单机版登录进去");
            }
        }
    }
}
