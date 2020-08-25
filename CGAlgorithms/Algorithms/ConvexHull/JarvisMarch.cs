using CGUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CGAlgorithms.Algorithms.ConvexHull
{
    public class JarvisMarch : Algorithm
    {
        public static double CalcAngle(Point p1, Point p2, Point p3)
        {
            double angle = 0.0;
            Point a = new Point(0.0, 0.0);
            Point b = new Point(0.0, 0.0);

            Line l1 = new Line(p1, p2);
            Line l2 = new Line(p1, p3);

            a = HelperMethods.GetVector(l1);
            b = HelperMethods.GetVector(l2);
            double dotProduct = (a.X * b.X + a.Y * b.Y);
            double crossProduct = HelperMethods.CrossProduct(a, b);

            angle = Math.Atan2(dotProduct, crossProduct) * 180 / Math.PI;

            return angle;
        }
        public override void Run(List<Point> points, List<Line> lines, List<Polygon> polygons, ref List<Point> outPoints, ref List<Line> outLines, ref List<Polygon> outPolygons)
        {
            points = points.Distinct().ToList();

            if (points.Count <= 2)
            {
                for (int i = 0; i < points.Count; i++)
                {
                    outPoints.Add(points[i]);
                }
            }
            else
            {
                double minY = 100000;
                int index = -1;
                for (int i = 0; i < points.Count; i++)
                {
                    if (points[i].Y < minY)
                    {
                        index = i;
                        minY = points[i].Y;
                    }
                }
                Point minPoint = points[index];
                Point mnPoint = points[index];
                outPoints.Add(minPoint);
                double x1 = 100000000;
                double y1 = minPoint.Y;
                Point virtualPoint1 = new Point(x1, y1);

                List<KeyValuePair<double, Point>> sortedAngles1 = new List<KeyValuePair<double, Point>>();
                for (int i = 0; i < points.Count; i++)
                {
                    if (points[i] != minPoint)
                    {
                        double angle = CalcAngle(minPoint, virtualPoint1, points[i]);

                        sortedAngles1.Add(new KeyValuePair<double, Point>(angle, points[i]));
                    }
                }
                sortedAngles1.Sort((s, ss) => s.Key.CompareTo(ss.Key));
                outPoints.Add(sortedAngles1[0].Value);

                while (true)
                {

                    virtualPoint1 = outPoints[outPoints.Count - 1];
                    minPoint = outPoints[outPoints.Count - 2];
                    List<KeyValuePair<double, Point>> sortedAngles = new List<KeyValuePair<double, Point>>();
                    for (int i = 0; i < points.Count; i++)
                    {
                        if (virtualPoint1 != points[i] && minPoint != points[i])
                        {
                            double angle = CalcAngle(virtualPoint1, points[i], minPoint);
                            if (angle < 0)
                                angle += 360;
                            sortedAngles.Add(new KeyValuePair<double, Point>(angle, points[i]));
                        }
                    }

                    sortedAngles.Sort((s, ss) => s.Key.CompareTo(ss.Key));
                    if (sortedAngles[sortedAngles.Count - 1].Value != mnPoint)
                    {
                        outPoints.Add(sortedAngles[sortedAngles.Count - 1].Value);
                    }
                    else
                        break;
                }

            }


        }
        public override string ToString()
        {
            return "Convex Hull - Jarvis March";
        }
    }
}
