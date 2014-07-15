using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;

namespace AD.Test.Core.Utils
{
    [TestClass]
    public class WaveFileTest
    {
        [TestMethod]
        public void TestWaveFile()
        {
            Lin.Core.Utils.WaveFile waveFile = new Lin.Core.Utils.WaveFile(@"..\..\..\Test\Core\Utils\1.wav");
            waveFile.Read();
            Console.WriteLine("length:" + waveFile.m_Data.NumSamples);
         
            for (int n = 0; n < 10; n++)
            {
                Console.Write(waveFile.m_Data[n] + "\t");
            }
        }
         
        [TestMethod]
        public void TestiSWaveFile()
        {
            Lin.Core.Utils.WaveFile.IsWaveFile(new FileInfo(@"..\..\..\Test\Core\Utils\1.wav"));
           
        }
    }
}
