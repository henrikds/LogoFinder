using System;
using System.Collections.Generic;
using System.Drawing;

namespace ImageAnalysis
{
    class EdgeOrientation
    {
        public static double DataLimit255(double number)
        {
            if (number > 255) { number = 255; }
            else if (number < 0) { number = 0; }

            return number;
        }

        public static List<Point> EdgeDetectList(Bitmap bmp, double[,] filterMatX, double[,] filterMatY, int edgeLimit)
        {
            var pixelSource = TypesAndFiles.BitmapToByteArray(bmp);
            var edgePointList = new List<Point>();

            // Converting to grayscale
            for (var i = 0; i < pixelSource.Length; i += 4)
            {
                var rgb = pixelSource[i + 0] * 0.11f;
                rgb += pixelSource[i + 1] * 0.59f;
                rgb += pixelSource[i + 2] * 0.3f;

                pixelSource[i + 0] = (byte)rgb;
                pixelSource[i + 1] = (byte)rgb;
                pixelSource[i + 2] = (byte)rgb;
                pixelSource[i + 3] = 255;
            }

            var filterWidth = filterMatX.GetLength(1);
            var filterOffset = (filterWidth - 1) / 2;

            for (var yy = filterOffset; yy < bmp.Height - filterOffset; yy++)
            {
                for (var xx = filterOffset; xx < bmp.Width - filterOffset; xx++)
                {
                    var gx = 0.0;
                    var gy = 0.0;

                    var byteOffset = (yy * bmp.Width + xx) * 4;

                    // Applying filters
                    for (var fx = -filterOffset; fx <= filterOffset; fx++)
                    {
                        for (var fy = -filterOffset; fy <= filterOffset; fy++)
                        {
                            var calcOffset = byteOffset + (fy + fx * bmp.Width) * 4;

                            gx += (pixelSource[calcOffset + 0]) * filterMatX[fx + filterOffset, fy + filterOffset];

                            gy += (pixelSource[calcOffset + 0]) * filterMatY[fx + filterOffset, fy + filterOffset];
                        }
                    }

                    gx = DataLimit255(Math.Sqrt(gx * gx));
                    gy = DataLimit255(Math.Sqrt(gy * gy));

                    var grad = Math.Sqrt((gx * gx) + (gy * gy));

                    var p = new Point(xx, yy);

                    if (grad >= edgeLimit)
                    {
                        edgePointList.Add(p);
                    }
                }
            }

            return edgePointList;
        }

        public static Bitmap EdgeDetectBitmap(Bitmap bmp, double[,] filterMatX, double[,] filterMatY,int edgelimit, bool grayscale)
        {
            var pixelSource = TypesAndFiles.BitmapToByteArray(bmp);
            var pixelsResult = new byte[pixelSource.Length];

            if (grayscale) pixelSource = TypesAndFiles.GrayscaleByteArray(pixelSource);

            var filterWidth = filterMatX.GetLength(1);
            var filterOffset = (filterWidth - 1) / 2;

            for (var yy = filterOffset; yy < bmp.Height - filterOffset; yy++)
            {
                for (var xx = filterOffset; xx < bmp.Width - filterOffset; xx++)
                {
                    var gx = 0.0;
                    var gy = 0.0;

                    var byteOffset = (yy * bmp.Width + xx) * 4;

                    // Applying filters
                    for (var fx = -filterOffset; fx <= filterOffset; fx++)
                    {
                        for (var fy = -filterOffset; fy <= filterOffset; fy++)
                        {
                            var calcOffset = byteOffset + (fy + fx * bmp.Width) * 4;

                            gx += (pixelSource[calcOffset + 0]) * filterMatX[fx + filterOffset, fy + filterOffset];

                            gy += (pixelSource[calcOffset + 0]) * filterMatY[fx + filterOffset, fy + filterOffset];
                        }
                    }

                    var grad = DataLimit255(Math.Sqrt(gx * gx + gy * gy));

                    if (grad < edgelimit) grad = 0;

                    pixelsResult[byteOffset + 0] = (byte)(grad);
                    pixelsResult[byteOffset + 1] = (byte)(grad);
                    pixelsResult[byteOffset + 2] = (byte)(grad);
                    pixelsResult[byteOffset + 3] = 255;
                }
            }

            var resultBitmap = TypesAndFiles.ByteArrayToBitmap(pixelsResult, bmp.Width, bmp.Height);

            return resultBitmap;
        }
    }
}
