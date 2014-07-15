using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lin.Core.Utils
{
    public static class Axis
    {
        /// <summary>
        /// 自动计算步长
        /// </summary>
        /// <param name="actualLength"></param>
        /// <param name="value"></param>
        /// <param name="actualStep"></param>
        /// <param name="isAlign"></param>
        /// <returns></returns>
        public static double Step(double actualLength,double value,double actualStep = 40,bool isAlign = false)
        {
            //double maxVoltage = maxValue;
            value = global::System.Math.Abs(value);
            if(value < 1e-6){
                return 1;
            }
            if (actualStep < 10)
            {
                actualStep = 10;
            }
            actualLength = global::System.Math.Abs(actualLength);
            double m = global::System.Math.Log10(value) - 2;
            double scale = global::System.Math.Pow(10, (int)(m + 1));
            double maxValue = value / scale;


            int n = (int)(actualLength / actualStep + 1.01);
            int step = (int)(maxValue / n + 1);
            double tmpMaxValue = n * step;
            while (tmpMaxValue > maxValue + 0.2)
            {
                step--;
                tmpMaxValue = n * step;
            }
            step++;

            if (isAlign == true)
            {
                if (step > 7.5)
                {
                    step = 10;
                }
                else if (step > 2)
                {
                    step = 5;
                }
            }

            return step * scale;
        }
    }
}
