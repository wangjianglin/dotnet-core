using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Lin.Core.Math;

namespace AD.Test.Core.Math
{
    [TestClass]
    public class ComplexTest
    {
        [TestMethod]
        public void TestComplexExp()
        {
            Complex a = 1;//
            a += new Complex(0, 1);
            Console.WriteLine("exp(1+i):" + global::Lin.Core.Math.Math.Exp(a));
            Console.WriteLine("(1+i)^(1+i):" + (a^a));
        }

        [TestMethod]
        public void TestFFT()
        {
            double[] data = new double[33];
	//initTestData(data,99);
	        for(int n=0;n<data.Length;n++){
            //for (int n = 0; n < 33; n++){
                data[n] = n + 1;
            }
            //for (int n = 33; n < 64; n++)
            {
                //data[n] = 0;
            }
	        double[] fft = data.FFT();
            for (int n = 0; n < fft.Length; n++)
            {
                Console.WriteLine(fft[n]);
            }
        }
    }
}
