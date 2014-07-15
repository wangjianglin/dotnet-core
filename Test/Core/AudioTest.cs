using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
 

namespace AD.Test.Core
{
    [TestClass]
    public class AudioTest
    {
        [TestMethod]
        public void TestWav()
        {
            Lin.Core.Utils.WaveFile wave = new Lin.Core.Utils.WaveFile(@"..\..\..\Test\Core\1.wav");
            wave.Read();
            Console.WriteLine("sample:" + wave.m_Fmt.SamplesPerSec);
            Console.WriteLine("data size:" + wave.m_Data.DataSize);
            for (int n = 0; n < 100; n++)
            {
                Console.Write(wave.m_Data[n] + "\t");
            }
        }
         
    }
}
