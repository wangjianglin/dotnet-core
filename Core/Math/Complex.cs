using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lin.Core.Math
{
    /// <summary>
    /// 
    /// </summary>
    public class Complex
    {
        public static readonly Complex I = new Complex(0, 1);
        public double Real { get; private set; }
        public double Imag { get; private set; }
        public Complex(double real=0,double imag = 0)
        {
            this.Real = real;
            this.Imag = imag;
        }

        public static Complex operator +(Complex a, Complex b)
        {
            return new Complex(a.Real + b.Real, a.Imag + b.Imag);
        }

        public static Complex operator +(double a, Complex b)
        {
            return new Complex(a + b.Real, a + b.Imag);
        }

        public static Complex operator +(Complex a, double b)
        {
            return new Complex(a.Real + b, a.Imag);
        }

        public static Complex operator -(Complex a, Complex b)
        {
            return new Complex(a.Real - b.Real, a.Imag - b.Imag);
        }

        public static Complex operator -(double a, Complex b)
        {
            return new Complex(a - b.Real, -b.Imag);
        }

        public static Complex operator -(Complex a, double b)
        {
            return new Complex(a.Real - b, a.Imag);
        }

        public static Complex operator *(Complex a, double b)
        {
            return new Complex(a.Real * b, a.Imag * b);
        }

        public static Complex operator *(double a, Complex b)
        {
            return new Complex(a * b.Real, a * b.Imag);
        }

        public static Complex operator *(Complex a, Complex b)
        {
            return new Complex(a.Real * b.Real - a.Imag * b.Imag, a.Real * b.Imag + a.Imag * b.Real);
        }

        public static Complex operator /(Complex a, double b)
        {
            return new Complex(a.Real / b, a.Imag / b);
        }

        public static Complex operator /(double a, Complex b)
        {
            double tmp = b.Real * b.Real + b.Imag * b.Imag;
            return new Complex(a * b.Real / tmp, -a * b.Imag / tmp);
        }

        public static Complex operator /(Complex a, Complex b)
        {
            double tmp = b.Real * b.Real + b.Imag * b.Imag;
            return new Complex((a.Real * b.Real + a.Imag * b.Imag) / tmp, (a.Imag * b.Real - a.Real * b.Imag) / tmp);
        }

        public static Complex operator ^(Complex a, Complex b)
        {
            return global::Lin.Core.Math.Math.Pow(a, b);
        }

        public static Complex operator ^(double a, Complex b)
        {
            double tmp = global::System.Math.Log(a);
            return global::Lin.Core.Math.Math.Exp(tmp * b);
        }

        public static Complex operator ^(Complex a, double b)
        {
            return global::Lin.Core.Math.Math.Pow(a, b);
        }

        public static implicit operator Complex(double real){
            return new Complex(real,0);
        }
        public double Abs()
        {
            return global::System.Math.Sqrt(this.Real * this.Real + this.Imag * this.Imag);
        }
        public static explicit operator double(Complex complex)
        {
            return complex.Real;
        }
        public override string ToString()
        {
            return this.Real + "+" + this.Imag + "i";
        }
    }
}
