using CGUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CGAlgorithms.Algorithms.ConvexHull
{
    public class GrahamScan : Algorithm
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

            if (points.Count == 1)
            {
                outPoints.Add(points[0]);
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
                double x = minPoint.X + 100000000;
                double y = minPoint.Y;
                Point virtualPoint = new Point(x, y);
                points.RemoveAt(index);
                List<KeyValuePair<double, Point>> sortedAngles = new List<KeyValuePair<double, Point>>();
                for (int i = 0; i < points.Count; i++)
                {

                    double angle = CalcAngle(minPoint, virtualPoint, points[i]);

                    sortedAngles.Add(new KeyValuePair<double, Point>(angle, points[i]));
                }

                sortedAngles.Sort((s, ss) => s.Key.CompareTo(ss.Key));
                Stack<Point> myStack = new Stack<Point>();
                myStack.Push(minPoint);
                myStack.Push(sortedAngles[0].Value);

                for (int i = 1; i < points.Count; i++)
                {
                    Point p1 = myStack.Pop();
                    Point p2 = myStack.Pop();
                    myStack.Push(p2);
                    myStack.Push(p1);
                    Line l = new Line(p1, p2);
                    Enums.TurnType t = HelperMethods.CheckTurn(l, sortedAngles[i].Value);

                    while (myStack.Count > 2 && t != Enums.TurnType.Left)
                    {
                        myStack.Pop();

                        p1 = myStack.Pop();
                        p2 = myStack.Pop();

                        myStack.Push(p2);
                        myStack.Push(p1);
                        l = new Line(p1, p2);
                        t = HelperMethods.CheckTurn(l, sortedAngles[i].Value);

                    }
                    if (t == Enums.TurnType.Colinear)
                    {
                        double d1 = Math.Sqrt(Math.Pow((sortedAngles[i].Value.X - p1.X), 2) + Math.Pow((sortedAngles[i].Value.Y - p1.Y), 2));
                        double d2 = Math.Sqrt(Math.Pow((p2.X - p1.X), 2) + Math.Pow((p2.Y - p1.Y), 2));
                        if (d1 > d2)
                        {
                            myStack.Pop();
                        }
                    }
                    myStack.Push(sortedAngles[i].Value);
                }


                while (myStack.Count != 0)
                {
                    outPoints.Add(myStack.Pop());
                }
            }


        }
        public override string ToString()
        {
            return "Convex Hull - Graham Scan";
        }
    }
}
