using CGUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CGAlgorithms.Algorithms.ConvexHull
{
    public class Incremental : Algorithm
    {
        public static double angleBetween2Lines(Line line1, Line line2)
        {
            return CalculateAngle(line1.Start, line1.End, line2.Start, line2.End);
        }
        public static double CalculateAngle(Point A1, Point A2, Point B1, Point B2)
        {
            double angle1 = Math.Atan2(A2.Y - A1.Y, A1.X - A2.X);
            double angle2 = Math.Atan2(B2.Y - B1.Y, B1.X - B2.X);
            double degrees = (angle1 - angle2) * (180 / Math.PI);

            double calculatedAngle = degrees;
            if (calculatedAngle < 0) calculatedAngle += 360;
            return calculatedAngle;
        }


        public override void Run(List<Point> points, List<Line> lines, List<Polygon> polygons, ref List<Point> outPoints, ref List<Line> outLines, ref List<Polygon> outPolygons)
        {
            if(points.Count < 3)
            {
                outPoints = points;
                return;
            }
            double K = 1000.0f;
            CGUtilities.DataStructures.OrderedSet<Tuple<double, int>> CH = new CGUtilities.DataStructures.OrderedSet<Tuple<double, int>>();

            Point B = new Point((points[0].X + points[1].X) / 2.0f, (points[0].Y + points[1].Y) / 2.0f);
            B.X = (B.X + points[2].X) / 2.0f;
            B.Y= (B.Y + points[2].Y) / 2.0f;


            Point NB = new Point(B.X + K, B.Y);

            Line base_line = new Line(B, NB);

            double point_zero_angle = angleBetween2Lines(base_line, new Line(B, points[0]));
            double point_one_angle = angleBetween2Lines(base_line, new Line(B, points[1]));
            double point_two_angle = angleBetween2Lines(base_line, new Line(B, points[2]));
            CH.Add(new Tuple<double, int>(point_zero_angle, 0));
            CH.Add(new Tuple<double, int>(point_one_angle, 1));
            CH.Add(new Tuple<double, int>(point_two_angle, 2));


            for(int i = 3; i <points.Count; ++i)
            {
                Point P = points[i];
                KeyValuePair<Tuple<double, int>, Tuple<double, int>> P_pair = new KeyValuePair<Tuple<double, int>, Tuple<double, int>>();
                double point_P_angle = angleBetween2Lines(base_line, new Line(B, P));
                P_pair = CH.DirectUpperAndLower(new Tuple<double, int>(point_P_angle, i));
                Tuple<double, int> Pre = P_pair.Value;
                Tuple<double, int> Next = P_pair.Key;
                if (Pre == null)
                    Pre = CH.GetLast();
                if(Next == null)
                    Next = CH.GetFirst();
                if (HelperMethods.CheckTurn(new Line(points[Pre.Item2], points[Next.Item2]), P) == Enums.TurnType.Right)
                {
                    KeyValuePair<Tuple<double, int>, Tuple<double, int>> Pre_pair = new KeyValuePair<Tuple<double, int>, Tuple<double, int>>();
                    double point_Pre_angle = angleBetween2Lines(base_line, new Line(B, points[Pre.Item2]));
                    Pre_pair = CH.DirectUpperAndLower(new Tuple<double, int>(point_Pre_angle, Pre.Item2));
                    Tuple<double, int> newPre = Pre_pair.Value;
                    if (newPre == null)
                        newPre = CH.GetLast();
                    while (HelperMethods.CheckTurn(new Line(P, points[Pre.Item2]), points[newPre.Item2]) == Enums.TurnType.Left || HelperMethods.CheckTurn(new Line(P, points[Pre.Item2]), points[newPre.Item2]) == Enums.TurnType.Colinear)
                    {
                        CH.Remove(Pre);
                        Pre = newPre;
                        point_Pre_angle = angleBetween2Lines(base_line, new Line(B, points[Pre.Item2]));
                        Pre_pair = CH.DirectUpperAndLower(new Tuple<double, int>(point_Pre_angle, Pre.Item2));
                        newPre = Pre_pair.Value;
                        if (newPre == null)
                            newPre = CH.GetLast();
                    }
                    KeyValuePair<Tuple<double, int>, Tuple<double, int>> Next_pair = new KeyValuePair<Tuple<double, int>, Tuple<double, int>>();
                    double point_Next_angle = angleBetween2Lines(base_line, new Line(B, points[Next.Item2]));
                    Next_pair = CH.DirectUpperAndLower(new Tuple<double, int>(point_Next_angle, Next.Item2));
                    Tuple<double, int> newNext = Next_pair.Key;
                    if (newNext == null)
                        newNext = CH.GetFirst();
                    while (HelperMethods.CheckTurn(new Line(P, points[Next.Item2]), points[newNext.Item2]) == Enums.TurnType.Right || HelperMethods.CheckTurn(new Line(P, points[Next.Item2]), points[newNext.Item2]) == Enums.TurnType.Colinear)
                    {
                        CH.Remove(Next);
                        Next = newNext;
                        point_Next_angle = angleBetween2Lines(base_line, new Line(B, points[Next.Item2]));
                        Next_pair = CH.DirectUpperAndLower(new Tuple<double, int>(point_Next_angle, Next.Item2));
                        newNext = Next_pair.Key;
                        if (newNext == null)
                            newNext = CH.GetFirst();
                    }
                    for(int element = 0; element < CH.Count; ++element)
                    {
                        if(HelperMethods.PointInTriangle(points[CH.ElementAt(element).Item2], points[i], points[Pre.Item2], points[Next.Item2]) == Enums.PointInPolygon.Inside)
                        {
                            CH.Remove(CH.ElementAt(element));
                        }
                    }
                    CH.Add(new Tuple<double, int>(point_P_angle, i));
                }

            }
            for(int i =0; i < CH.Count;++i)
            {
                outPoints.Add(points[CH.ElementAt(i).Item2]);
            }

        }

        public override string ToString()
        {
            return "Convex Hull - Incremental";
        }
    }
}
