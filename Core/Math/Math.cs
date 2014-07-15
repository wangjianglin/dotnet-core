using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lin.Core.Math
{
    /// <summary>
    /// 
    /// </summary>
    public static class Math
    {
        public static Complex Pow(Complex a, Complex b)
        {
            //double A = global::System.Math.Sqrt(a.Real * a.Real + a.Imag * a.Imag);
            //double r = global::System.Math.Acos(a.Real / A);
            //return A * Exp(new Complex(-b.Imag * r, b.Real * r));
            return Exp(b * Log(a));
        }

        public static Complex Exp(Complex x)
        {
            double a = global::System.Math.Exp(x.Real);
            return new Complex(a * global::System.Math.Cos(x.Imag), a * global::System.Math.Sin(x.Imag));
        }

        public static Complex Log(Complex x)
        {
            double A = global::System.Math.Sqrt(x.Real * x.Real + x.Imag * x.Imag);
            return new Complex(global::System.Math.Log(A), global::System.Math.Acos(x.Real / A));
        }
        public static Complex Sin(Complex x)
        {
            return null;
        }
    }
}
