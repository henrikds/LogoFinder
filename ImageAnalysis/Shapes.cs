using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Xml;

namespace ImageAnalysis
{
    public class Shapes
    {
        private static List<Point> ConnectedPoints(IReadOnlyList<Point> edgePoints, Point checkingPoint)
        //Returns a list of points that are adjecnt to a given point from a list points
        //WARNING: Will do nothing if point checked has x or y-cordinate of 0
        {
            var adPoints = new List<Point> { checkingPoint };

            for (var i = -1; i <= 1; i++)
            {
                for (var j = -1; j <= 1; j++)
                {
                    var point = new Point(checkingPoint.X + i, checkingPoint.Y + j);
                    if (edgePoints.Contains(point) && point != checkingPoint)
                    {
                        adPoints.Add(point);
                    }
                }
            }

            var result = new List<Point>(adPoints);
            return result;
        }

        private static List<Point> AdjacentPointsShaper(IReadOnlyList<Point> pointList, Point startingPoint)
        // Finner alle sammenhengende punkter til et punkt
        {
            var shapePoints = ConnectedPoints(pointList, startingPoint);
            var adPoints = new List<Point>();
            var checkedPoints = new List<Point>();

            for (var k = 0; k < shapePoints.Count; k++)
            {
                var newPoint = shapePoints[k];

                if (!checkedPoints.Contains(newPoint))
                {
                    adPoints.AddRange(ConnectedPoints(pointList, newPoint));
                    checkedPoints.Add(newPoint);

                    foreach (var point in adPoints)
                    {
                        if (!shapePoints.Contains(point))
                        {
                            shapePoints.Add(point);
                        }
                    }
                }

                adPoints.Clear();
            }

            return shapePoints;
        }

        private static List<Point> ShapeRect(IReadOnlyList<Point> shape)
        // Gir kordiantene til et rektangel rundt en shape
        {
            var result = new List<Point>();

            var xMin = shape.Min(point => point.X);
            var yMin = shape.Min(point => point.Y);
            var xMax = shape.Max(point => point.X);
            var yMax = shape.Max(point => point.Y);

            var pStart = new Point(xMin, yMin);
            var pEnd = new Point(xMax, yMax);
            result.Add(pStart);
            result.Add(pEnd);

            return result;
        }

        private static byte[] SquareMarker(byte[] array, int imageWidth, IReadOnlyList<Point> pointList, double rate)
        // Tegner rektangel rundt point
        {
            var color = Color.DeepPink;
            if (rate <= 1 && rate >= 0)
            {
                color = Colors.HitRate3(rate);
            }

            for (var i = 0; i < pointList.Count; i += 2)
            {
                if (pointList[i] == pointList[i + 1]) continue;

                for (var xx = pointList[i].X; xx <= pointList[i + 1].X; xx++)
                {
                    var byteOffset = (pointList[i].Y * imageWidth + xx) * 4;

                    array[byteOffset + 0] = color.B;
                    array[byteOffset + 1] = color.G;
                    array[byteOffset + 2] = color.R;
                    array[byteOffset + 3] = 255;

                    byteOffset = (pointList[i + 1].Y * imageWidth + xx) * 4;

                    array[byteOffset + 0] = color.B;
                    array[byteOffset + 1] = color.G;
                    array[byteOffset + 2] = color.R;
                    array[byteOffset + 3] = 255;
                }

                for (var yy = pointList[i].Y; yy <= pointList[i + 1].Y; yy++)
                {
                    var byteOffset = (yy * imageWidth + pointList[i].X) * 4;

                    array[byteOffset + 0] = color.B;
                    array[byteOffset + 1] = color.G;
                    array[byteOffset + 2] = color.R;
                    array[byteOffset + 3] = 255;

                    byteOffset = (yy * imageWidth + pointList[i + 1].X) * 4;

                    array[byteOffset + 0] = color.B;
                    array[byteOffset + 1] = color.G;
                    array[byteOffset + 2] = color.R;
                    array[byteOffset + 3] = 255;
                }

            }

            return array;
        }

        public static List<Point> ShapeRectList(Bitmap bmp, int edgeLimit)
        // Gir en liste rektangler med shapes fra et bilde (Point, Point)
        {
            var edgePoints = EdgeOrientation.EdgeDetectList(bmp, Filters.Sobel3x3Horizontal,
                Filters.Sobel3x3Vertical, edgeLimit);

            var inShape = new List<Point>();
            var inSquare = new List<Point>();

            foreach (var point in edgePoints)
            {
                if (inShape.Contains(point)) continue;
                var shape = AdjacentPointsShaper(edgePoints, point);
                var square = ShapeRect(shape);
                if (shape.Count < 10) continue;
                inShape.AddRange(shape);
                inSquare.AddRange(square);
            }

            return inSquare;
        }

        public static Bitmap ShapeComparison(Bitmap subjectBitmap, Bitmap logoBitmap, int edgeLimitSub, int edgeLimitLogo, 
            double hitRate, bool grayscale)
        {
            var rectListSub = ShapeRectList(subjectBitmap, edgeLimitSub);
            var rectListLogo = ShapeRectList (logoBitmap, edgeLimitLogo);

            var pixelsSubject = TypesAndFiles.BitmapToByteArray(subjectBitmap);
            var pixelsLogo = TypesAndFiles.BitmapToByteArray(logoBitmap);
            var pixelResult = pixelsSubject;

            if (grayscale)
            {
                pixelsSubject = TypesAndFiles.GrayscaleByteArray(pixelsSubject);
                pixelsLogo = TypesAndFiles.GrayscaleByteArray(pixelsLogo);
            }

            //var compareList = new List<Point>();
            var previousHits = new List<double>();

            //FileReader.ExtractArray(pixelsSubject, rectListSub, subjectBitmap.Width, 0);

            for (var i = 0; i < rectListSub.Count / 2; i++)
            {
                previousHits.Add(0.0);
            }

            // Comparison by subtraction
            for (var j = 0; j < rectListLogo.Count; j += 2)
            {
                for (var i = 0; i < rectListSub.Count; i += 2)
                {
                    var subShapeWidth = rectListSub[i + 1].X - rectListSub[i].X + 1;
                    var subShapeHeight = rectListSub[i + 1].Y - rectListSub[i].Y + 1;

                    var logoShapeWidth = rectListLogo[j + 1].X - rectListLogo[j].X + 1;
                    var logoShapeHeight = rectListLogo[j + 1].Y - rectListLogo[j].Y + 1;

                    var newArrSub = TypesAndFiles.ExtractArray(pixelsSubject, rectListSub, subjectBitmap.Width, i);
                    var newArrLogo = TypesAndFiles.ExtractArray(pixelsLogo, rectListLogo, logoBitmap.Width, j);

                    var rate = 0.0;
                    var b = 0;

                    if (logoShapeHeight == subShapeHeight && logoShapeWidth == subShapeWidth)
                    {
                        var db = 0.0;
                        var dg = 0.0;
                        var dr = 0.0;

                        for (var yy = 0; yy < logoShapeHeight; yy++)
                        {
                            for (var xx = 0; xx < logoShapeWidth; xx++)
                            {
                                var byteOffset = (yy * logoShapeWidth + xx) * 4;

                                db += Math.Abs(newArrSub[byteOffset + 0] - newArrLogo[byteOffset + 0]);
                                dg += Math.Abs(newArrSub[byteOffset + 1] - newArrLogo[byteOffset + 1]);
                                dr += Math.Abs(newArrSub[byteOffset + 2] - newArrLogo[byteOffset + 2]);
                                b++;
                            }
                        }

                        db = db / (b * 255);
                        dg = dg / (b * 255);
                        dr = dr / (b * 255);
                        rate = 1 - (db + dg + dr) / 3;

                        if (previousHits[i / 2] > rate) continue;
                        previousHits[i / 2] = rate;
                    }

                    else
                    {
                        var newArrSubScaled = Transformations.TransScaling(newArrSub, subShapeHeight, subShapeWidth, logoShapeHeight, logoShapeWidth);

                        var db = 0.0;
                        var dg = 0.0;
                        var dr = 0.0;

                        for (var yy = 0; yy < logoShapeHeight; yy++)
                        {
                            for (var xx = 0; xx < logoShapeWidth; xx++)
                            {
                                var byteOffset = (yy * logoShapeWidth + xx) * 4;

                                db += Math.Abs(newArrSubScaled[byteOffset + 0] - newArrLogo[byteOffset + 0]);
                                dg += Math.Abs(newArrSubScaled[byteOffset + 1] - newArrLogo[byteOffset + 1]);
                                dr += Math.Abs(newArrSubScaled[byteOffset + 2] - newArrLogo[byteOffset + 2]);
                                b++;
                            }
                        }

                        db = db / (b * 255);
                        dg = dg / (b * 255);
                        dr = dr / (b * 255);
                        rate = 1 - (db + dg + dr) / 3;

                        if (previousHits[i / 2] > rate) continue;
                        previousHits[i / 2] = rate;
                    }
                }
            }

            // Marking every shape wwith a hitrate above hitcap 
            for (var j = 0; j < rectListSub.Count; j += 2)
            {
                if (previousHits[j / 2] < hitRate) continue;
                var shapelist = new List<Point> { rectListSub[j], rectListSub[j + 1] };
                pixelResult = SquareMarker(pixelResult, subjectBitmap.Width, shapelist, previousHits[j / 2]);
            }

            // Kunne brukt pixelSubject, men god huskeregel å ikke endre data man leser av.

            MainWindow.HitRatesPercent = previousHits;

            var resultBitmap = TypesAndFiles.ByteArrayToBitmap(pixelResult, subjectBitmap.Width, subjectBitmap.Height);
            
            return resultBitmap;
        }

        public static Bitmap SquaredImage(Bitmap bmp, int edgeLimit, bool grayscale)
        {
            var shapes = ShapeRectList(bmp, edgeLimit);
            var bmpData = TypesAndFiles.BitmapToByteArray(bmp);
            if (grayscale) bmpData = TypesAndFiles.GrayscaleByteArray(bmpData);
            var bmpSquares = SquareMarker(bmpData, bmp.Width, shapes, 2);
            return TypesAndFiles.ByteArrayToBitmap(bmpSquares, bmp.Width, bmp.Height);
        }

        private static void CodePieces(Bitmap bmp)
        // Kode som kan være nyttig copypasta
        {
            var rect = new Rectangle(0, 0, bmp.Width, bmp.Height);
            var sourceData = bmp.LockBits(rect, ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);

            var pixelSource = new byte[sourceData.Stride * bmp.Height];
            var pixelsResult = new byte[sourceData.Stride * bmp.Height];

            // Copying data from bmp into pixelSource array
            Marshal.Copy(sourceData.Scan0, pixelSource, 0, pixelSource.Length);

            // BGRA
            for (var i = 0; i < pixelSource.Length; i += 4)
            {
                pixelSource[i + 0] = 0;
                pixelSource[i + 1] = 0;
                pixelSource[i + 2] = 0;
                pixelSource[i + 3] = 255;
            }

            var resultBitmap = new Bitmap(bmp.Width, bmp.Height);

            var resultData = resultBitmap.LockBits(rect, ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);

            Marshal.Copy(pixelsResult, 0, resultData.Scan0, pixelsResult.Length);

            bmp.UnlockBits(sourceData);
            resultBitmap.UnlockBits(resultData);
        }
    }
}