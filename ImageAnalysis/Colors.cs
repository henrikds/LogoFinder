using System;
using System.Collections.Generic;
using System.Drawing;

namespace ImageAnalysis
{
    public class Colors
    {
        public static List<Color> ColorMaker => new List<Color>
        {
            Color.FromArgb(0, 0, 0),
            Color.FromArgb(0, 0, 255),
            Color.FromArgb(0, 255, 0),
            Color.FromArgb(255, 0, 0),
            Color.FromArgb(0, 255, 255),
            Color.FromArgb(255, 255, 0),
            Color.FromArgb(255, 0, 255)
        };

        public static Color HitRate(int rateGroup)
        {
            switch (rateGroup)
            {
                case 1:
                    return Color.LimeGreen;
                case 2:
                    return Color.RoyalBlue;
                case 3:
                    return Color.Yellow;
                case 4:
                    return Color.DarkRed;
            }
            return Color.Black;
        }

        public static Color HitRate2(double rate)
        {
            rate = rate * 100;
            var rateGroup = 0;

            if (rate >= 98 && rate <= 100) rateGroup = 1;
            if (rate >= 95 && rate < 98) rateGroup = 2;
            if (rate >= 90 && rate < 95) rateGroup = 3;
            if (rate >= 80 && rate < 90) rateGroup = 4;

            switch (rateGroup)
            {
                case 1:
                    return Color.LimeGreen;
                case 2:
                    return Color.RoyalBlue;
                case 3:
                    return Color.Yellow;
                case 4:
                    return Color.DarkRed;
            }
            return Color.Black;
        }

        public static Color HitRate3(double rate)
        {
            var r = (int) EdgeOrientation.DataLimit255(2 * (1 - rate) * 255);
            var g = (int) EdgeOrientation.DataLimit255(2 * rate * 255);
            
            return Color.FromArgb(r, g, 0);
        }
    }
}