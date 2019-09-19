using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Controls;

namespace ImageAnalysis
{
    public class VectorsAndPoints
    {
        public static double GetDistance(Point p1, Point p2)
        // Calculates the distance between two points.
        {
            double dx = p2.X - p1.X;
            double dy = p2.Y - p1.Y;

            var dist = Math.Sqrt(dx * dx + dy * dy);

            return dist;
        }

        public static Tuple<double, Point> MinimumDistance(List<Point> pointList, Point checkingPoint)
        // Returns minimum distance between one point and a list of points, and a list with the two points.
        // Never used in this program
        {
            var minDistance = 0.0;
            var minDistPoint = new Point();
            var first = true;

            foreach (var point in pointList)
            {
                if (checkingPoint != point)
                {
                    var pointDistance = GetDistance(checkingPoint, point);

                    if (pointDistance < minDistance || first)
                    {
                        minDistance = pointDistance;
                        minDistPoint = point;
                        first = false;
                    }
                }
            }

            var result = new Tuple<double, Point>(minDistance, minDistPoint);

            return result;
        }
    }
}