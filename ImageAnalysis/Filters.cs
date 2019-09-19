using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageAnalysis
{
    class Filters
    {
        public static double[,] Sobel3x3Horizontal
        {
            get
            {
                return new double[,]
                {
                    {-1, 0, 1, },
                    {-2, 0, 2, },
                    {-1, 0, 1, },
                };
            }
        }

        public static double[,] Sobel3x3Vertical
        {
            get
            {
                return new double[,]
                {
                    { 1,  2,  1, },
                    { 0,  0,  0, },
                    {-1, -2, -1, },
                };
            }
        }
    }
}
