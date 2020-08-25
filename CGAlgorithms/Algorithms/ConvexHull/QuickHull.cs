using CGUtilities;
using System;
using System.Collections.Generic;

namespace CGAlgorithms.Algorithms.ConvexHull
{
    public class QuickHull : Algorithm
    {

        public double dist(Point a, Point b, Point c)
        {
            return Math.Abs((c.Y - a.Y) * (b.X - a.X) -
                       (b.Y - a.Y) * (c.X - a.X));
        }
        public List<Point> quickHull(List<Point> a, Point min_x, Point max_x, string ss)
        {
            var s = Enums.TurnType.Left;
            if (ss == "Right")
            {
                s = Enums.TurnType.Right;
            }
            else
            {
                s = Enums.TurnType.Left;
            }
            int ind = -1;
            double max = -1;
            List<Point> ans = new List<Point>();
            if (a.Count == 0)
                return ans;
            for (int i = 0; i < a.Count; i++)
            {
                double x = dist(min_x, max_x, a[i]);
                if (CGUtilities.HelperMethods.CheckTurn(new Line(min_x.X, min_x.Y, max_x.X, max_x.Y), a[i]) == s && x > max)
                {
                    ind = i;
                    max = x;
                }
            }


            if (ind == -1)
            {
                ans.Add(min_x);
                ans.Add(max_x);

                return ans;
            }

            List<Point> p1, p2;
            if (CGUtilities.HelperMethods.CheckTurn(new Line(a[ind].X, a[ind].Y, min_x.X, min_x.Y), max_x) ==
                Enums.TurnType.Right)
            {
                p1 = quickHull(a, a[ind], min_x, "Left");

            }
            else
            {
                p1 = quickHull(a, a[ind], min_x, "Right");

            }

            if (CGUtilities.HelperMethods.CheckTurn(new Line(a[ind].X, a[ind].Y, max_x.X, max_x.Y), min_x) ==
                Enums.TurnType.Right)
            {
                p2 = quickHull(a, a[ind], max_x, "Left");

            }
            else
            {
                p2 = quickHull(a, a[ind], max_x, "Right");

            }
            for (int i = 0; i < p2.Count; ++i)
                p1.Add(p2[i]);
            return p1;

        }
        public override void Run(List<Point> points, List<Line> lines, List<Polygon> polygons, ref List<Point> outPoints, ref List<Line> outLines, ref List<Polygon> outPolygons)
        {
            Point max_x = new Point(-100000, 0);
            Point min_x = new Point(100000, 0);
            for (int i = 0; i < points.Count; i++)
            {
                if (points[i].X < min_x.X)
                    min_x = points[i];
                if (points[i].X > max_x.X)
                    max_x = points[i];
            }
            List<Point> right = quickHull(points, min_x, max_x, "Right");
            List<Point> left = quickHull(points, min_x, max_x, "Left");
            for (int i = 0; i < left.Count; ++i)
                right.Add(left[i]);

            for (int i = 0; i < right.Count; i++)
            {
                if (!outPoints.Contains(right[i]))
                    outPoints.Add(right[i]);
            }
        }

        public override string ToString()
        {
            return "Convex Hull - Quick Hull";
        }
    }
}
