using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace ImageAnalysis
{
    class TypesAndFiles
    {
        public static Bitmap OpenImage(string filePath)
        // Opening a Image with given source and makes a bitmap
        {
            try
            {
                var bitImage = (Bitmap) Image.FromFile(filePath, true);
                return bitImage;
            }
            catch (System.IO.FileNotFoundException)
            {
                MessageBox.Show("There was a error " +
                                "with uploading the file." +
                                "Program will close.");
                return null;
            }
        }

        public static ImageSource BitmapToImageSource(Bitmap bitmap)
        // Converting Bitmap Into ImageSource
        // Made by https://stackoverflow.com/users/2659716/gerret
        {
            using (var memory = new MemoryStream())
            {
                try
                {
                    bitmap.Save(memory, System.Drawing.Imaging.ImageFormat.Bmp);
                    memory.Position = 0;
                    var bitmapimage = new BitmapImage();
                    bitmapimage.BeginInit();
                    bitmapimage.StreamSource = memory;
                    bitmapimage.CacheOption = BitmapCacheOption.OnLoad;
                    bitmapimage.EndInit();

                    return bitmapimage;
                }
                catch (System.Runtime.InteropServices.ExternalException)
                {
                    var error = MessageBox.Show("There was a error " +
                                                "with uploading the file.");
                    return null;
                }
            }
        }

        public static byte[] BitmapToByteArray(Bitmap bmp)
        // Creates a byte array of data from bitmap
        {
            var rect = new Rectangle(0, 0, bmp.Width, bmp.Height);
            var bmpData = bmp.LockBits(rect, ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            var array = new byte[bmpData.Stride * bmp.Height];
            Marshal.Copy(bmpData.Scan0, array, 0, array.Length);

            bmp.UnlockBits(bmpData);

            return array;
        }

        public static Bitmap ByteArrayToBitmap(byte[] array, int width, int height)
        // Creates Bitmap from byte array with data
        {
            var bmp = new Bitmap(width, height);
            var rect = new Rectangle(0, 0, bmp.Width, bmp.Height);
            var bmpData = bmp.LockBits(rect, ImageLockMode.WriteOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            Marshal.Copy(array, 0, bmpData.Scan0, array.Length);

            bmp.UnlockBits(bmpData);

            return bmp;
        }

        public static byte[] GrayscaleByteArray(byte[] byteSource)
        // Converting to grayscale
        {
            var bmpArray = byteSource;

            for (var i = 0; i < bmpArray.Length; i += 4)
            {
                var rgb = bmpArray[i + 0] * 0.11f;
                rgb += bmpArray[i + 1] * 0.59f;
                rgb += bmpArray[i + 2] * 0.3f;

                bmpArray[i + 0] = (byte)rgb;
                bmpArray[i + 1] = (byte)rgb;
                bmpArray[i + 2] = (byte)rgb;
                bmpArray[i + 3] = 255;
            }

            return bmpArray;
        }

        public static byte[] ExtractArray(byte [] array, List<System.Drawing.Point> rectList, int bmpWidth, int i)
        // Extracts byte array of shape from byte array of image
        {
            var dataStart = (rectList[i].Y * bmpWidth + rectList[i].X) * 4;
            var width = rectList[i + 1].X - rectList[i].X + 1;
            var height = rectList[i + 1].Y - rectList[i].Y + 1;
            var length = height * width * 4;

            var newArr = new byte[length];

            for (var subY = 0; subY < height; subY++)
            {
                var startIndex = dataStart + bmpWidth * 4 * subY;
                var copyIndex = width * 4 * subY;
                Array.Copy(array, startIndex, newArr, copyIndex, width * 4);
            }

            return newArr;
        }
    }
}