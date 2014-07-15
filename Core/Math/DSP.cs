using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lin.Core.Math
{
    public static class DSP
    {
        //public static double[,] operator *(double[] a, double[] b)
        //{
        //    double[,] c = new double[a.Length, a.Length];
        //    return null;
        //}

        public static double[] FFT(this double[] d)
        {
            if (d == null)
            {
                return null;
            }
            if (d.Length == 0)
            {
                return new double[] { };
            }
            int dLength = 1;
            while ((dLength *= 2) < d.Length) ;
            double[] data = new double[dLength * 2 + 1];
            for (int tmpn = 0; tmpn < d.Length; tmpn++)
            {
                data[2 * tmpn + 1] = d[tmpn];
                data[2 * tmpn + 2] = 0;
            }
            for (int tmpn = d.Length * 2 + 1; tmpn < data.Length; tmpn++)
            {
                data[tmpn] = 0;
            }
            int nn = dLength;
            int isign = 1;
            int n, j, i, m, mmax, istep;
            double tempr = 0.0;
            double tempi = 0.0;
            double theta = 0.0;
            double wpr = 0.0;
            double wpi = 0.0;
            double wr = 0.0;
            double wi = 0.0;
            double wtemp = 0.0;

            n = 2 * nn;
            j = 1;
            for (i = 1; i <= n; i = i + 2)
            {
                if (j > i)
                {
                    tempr = data[j];
                    tempi = data[j + 1];
                    data[j] = data[i];
                    data[j + 1] = data[i + 1];
                    data[i] = tempr;
                    data[i + 1] = tempi;
                }
                m = n / 2;
                while (m >= 2 && j > m)
                {
                    j = j - m;
                    m = m / 2;
                }
                j = j + m;
            }
            mmax = 2;
            while (n > mmax)
            {
                istep = 2 * mmax;
                theta = 6.28318530717959 / (isign * mmax);
                wpr = -2.0 * global::System.Math.Sin(0.5 * theta) * global::System.Math.Sin(0.5 * theta);
                wpi = global::System.Math.Sin(theta);
                wr = 1.0;
                wi = 0.0;
                for (m = 1; m <= mmax; m = m + 2)
                {
                    for (i = m; i <= n; i = i + istep)
                    {
                        j = i + mmax;
                        //tempr=double(wr)*data[j]-double(wi)*data[j+1];
                        //tempi=double(wr)*data[j+1]+double(wi)*data[j];
                        tempr = (wr) * data[j] - (wi) * data[j + 1];
                        tempi = (wr) * data[j + 1] + (wi) * data[j];
                        data[j] = data[i] - tempr;
                        data[j + 1] = data[i + 1] - tempi;
                        data[i] = data[i] + tempr;
                        data[i + 1] = data[i + 1] + tempi;
                    }
                    wtemp = wr;
                    wr = wr * wpr - wi * wpi + wr;
                    wi = wi * wpr + wtemp * wpi + wi;
                }
                mmax = istep;
            }
              
            double[] result = new double[dLength];
            for (int k = 0; k < result.Length; k++)
            {
                result[k] = global::System.Math.Sqrt(data[2 * k + 1] * data[2 * k + 1] + data[2 * k + 2] * data[2 * k + 2]);
            }
            return result;
        }

        public static double[] DFT(this double[] data)
        {
            /*
             * N=length(x); 
n=0:N-1;
k=n;
WN=exp(-j*2*pi/N);
nk=n'*k;
WNnk=WN.^nk;
Xk=x*WNnk;
y=Xk;*/
            int N = data.Length;
            double[] n = new double[N];
            double[] k = new double[N];
            for (int r = 0; r < N; r++)
            {
                n[r] = r;
                k[r] = r;
            }
            return null;
        }
    }
}
