using System;
using System.Drawing;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace ImageAnalysis
{
    class Transformations
    {
        public static byte[] TransScaling(byte[] shape, int shapeWidth, int shapeHeight, int scalingWidth, int scalingHeight)
        // Scaling input byte[] to new desired size
        {
            var scalingLength = scalingWidth * scalingHeight * 4;
            var newShape = new byte[scalingLength];

            if (scalingHeight == 0 || scalingWidth == 0) return null;

            var xRatio =  shapeWidth / (double) scalingWidth;
            var yRatio =  shapeHeight / (double) scalingHeight;

            for (var yy = 0; yy < scalingHeight; yy++)
            {
                for (var xx = 0; xx < scalingWidth; xx++)
                {
                    var dx = Math.Floor(xx * xRatio);
                    var dy = Math.Floor(yy * yRatio);

                    var byteOffset = (yy * scalingWidth + xx) * 4;
                    var dByteOffset = (int) (dy * shapeWidth + dx) * 4;

                    newShape[byteOffset + 0] = shape[dByteOffset + 0];
                    newShape[byteOffset + 1] = shape[dByteOffset + 1];
                    newShape[byteOffset + 2] = shape[dByteOffset + 2];
                    newShape[byteOffset + 3] = 255;
                }
            }

            return newShape;
        }

        public static Bitmap TransTest(Bitmap bmp)
        // Test for transformation code
        {
            const int scaleX = 2;
            const int scaleY = 2;

            var pixelData = TypesAndFiles.BitmapToByteArray(bmp);

            var newPixeldata = TransScaling(pixelData, bmp.Width, bmp.Height, bmp.Width * scaleX, bmp.Height * scaleY);

            var resultBitmap = TypesAndFiles.ByteArrayToBitmap(newPixeldata, bmp.Width * scaleX, bmp.Height * scaleY);

            return resultBitmap;
        }
    }
}