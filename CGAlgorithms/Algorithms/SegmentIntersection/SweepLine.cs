using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CGUtilities.DataStructures;
using CGUtilities;

namespace CGAlgorithms.Algorithms.SegmentIntersection
{
    public class EventPoint
    {
        public static List<Line> lines;
        public Point point;
        public int index;
        public int event_type;
        public EventPoint intersection_segment_1;
        public EventPoint intersection_segment_2;

        // 0 intersection, 1 start, -1 end
        public EventPoint(Point p, int index, int type)
        {
            this.point = p;
            this.index = index;
            this.event_type = type;
            this.intersection_segment_1 = null;
            this.intersection_segment_2 = null;
        }
        public EventPoint(Point p, EventPoint s1, EventPoint s2, int type)
        {
            this.point = p;
            this.event_type = type;
            this.intersection_segment_1 = s1;
            this.intersection_segment_2 = s2;
        }
    }
    class SweepLine : Algorithm
    {

        public int Compare_X(EventPoint E1, EventPoint E2)
        {
            if (E2.point.X < E1.point.X) return 1;
            else if (E2.point.X > E1.point.X) return -1;

            if (E2.point.Y < E1.point.Y) return 1;
            else if (E2.point.Y > E1.point.Y) return -1;

            if (EventPoint.lines[E2.index].End.X < EventPoint.lines[E1.index].End.X) return 1;
            else if (EventPoint.lines[E2.index].End.X > EventPoint.lines[E1.index].End.X) return -1;

            return 0;
        }

        public int Compare_Y(EventPoint E1, EventPoint E2)
        {
            if (E2.point.Y < E1.point.Y) return 1;
            if (E2.point.Y > E1.point.Y) return -1;

            if (E2.point.X < E1.point.X) return 1;
            if (E2.point.X > E1.point.X) return -1;

            return 0;
        }

        public bool ComparePoints(Point P1, Point P2)
        {
            if (P1.X < P2.X || (P1.X == P2.X && P1.Y < P2.Y)) return true;
            else return false;
        }


        public Point twoLinesIntersectionPoint(Line L1, Line L2)
        {
            double L1_slope = (L1.Start.Y - L1.End.Y) / (L1.Start.X - L1.End.X);
            double L2_slope = (L2.Start.Y - L2.End.Y) / (L2.Start.X - L2.End.X);
            double L1_intercept = L1.Start.Y - (L1_slope * L1.Start.X);
            double L2_intercept = L2.Start.Y - (L2_slope * L2.Start.X);
            double X, Y;

            if (L1_slope == Double.PositiveInfinity)
            {
                X = L1.Start.X;
                Y = (L2_slope * X) + L2_intercept;
            }
            else if (L2_slope == Double.PositiveInfinity)
            {
                X = L2.Start.X;
                Y = (L1_slope * X) + L1_intercept;
            }
            else
            {
                X = (L2_intercept - L1_intercept) / (L1_slope - L2_slope);
                Y = L1_slope * X + L1_intercept;
            }
            return new Point(X, Y);
        }
        public bool TwoSegmentsIntersectionTest(Line L1, Line L2)
        {
            Enums.TurnType first_orientation = HelperMethods.CheckTurn(L1, L2.Start);
            Enums.TurnType second_orientation = HelperMethods.CheckTurn(L1, L2.End);
            Enums.TurnType third_orientation = HelperMethods.CheckTurn(L2, L1.Start);
            Enums.TurnType fourth_orientation = HelperMethods.CheckTurn(L2, L1.End);

            if (first_orientation != second_orientation && third_orientation != fourth_orientation) return true;
            if (first_orientation == Enums.TurnType.Colinear && HelperMethods.PointOnSegment(L2.Start, L1.Start, L1.End)) return true;
            if (second_orientation == Enums.TurnType.Colinear && HelperMethods.PointOnSegment(L2.End, L1.Start, L1.End)) return true;
            if (third_orientation == Enums.TurnType.Colinear && HelperMethods.PointOnSegment(L1.Start, L2.Start, L2.End)) return true;
            if (fourth_orientation == Enums.TurnType.Colinear && HelperMethods.PointOnSegment(L1.End, L2.Start, L2.End)) return true;

            return false;
        }
        public bool HasValue(double value)
        {
            return !Double.IsNaN(value) && !Double.IsInfinity(value);
        }


        public override void Run(List<CGUtilities.Point> points, List<CGUtilities.Line> lines, List<CGUtilities.Polygon> polygons, ref List<CGUtilities.Point> outPoints, ref List<CGUtilities.Line> outLines, ref List<CGUtilities.Polygon> outPolygons)
        {
            OrderedSet<Tuple<int, int>> Intersected_lines = new OrderedSet<Tuple<int, int>>();
            EventPoint.lines = lines;

            OrderedSet<EventPoint> Q = new OrderedSet<EventPoint>(Compare_X);
            OrderedSet<EventPoint> L = new OrderedSet<EventPoint>(Compare_Y);

            for (int i = 0; i < lines.Count; ++i)
            {
                if (ComparePoints(lines[i].Start, lines[i].End) == false)
                    lines[i] = new Line(lines[i].End, lines[i].Start);
                Q.Add(new EventPoint(lines[i].Start, i, 1));
                Q.Add(new EventPoint(lines[i].End, i, -1));
            }
            int counter = lines.Count;
            while (Q.Count != 0)
            {
                EventPoint current_event = Q.GetFirst();
                Q.RemoveFirst();

                if (current_event.event_type == 1)
                {
                    L.Add(current_event);
                    if (lines[current_event.index].Start.X == lines[current_event.index].End.Y)
                    {
                        IEnumerable<EventPoint> vertical__ = L;
                        foreach (EventPoint e in vertical__)
                        {
                            if (TwoSegmentsIntersectionTest(lines[e.index], lines[current_event.index]))
                            {

                                Point point_of_intersection = twoLinesIntersectionPoint(lines[e.index], lines[current_event.index]);

                                if (HasValue(point_of_intersection.X) && HasValue(point_of_intersection.Y) && !Intersected_lines.Contains(new Tuple<int, int>(current_event.index, e.index)))
                                {
                                    outPoints.Add(point_of_intersection);
                                    Intersected_lines.Add(new Tuple<int, int>(current_event.index, e.index));
                                    Intersected_lines.Add(new Tuple<int, int>(current_event.index, e.index));

                                }
                            }
                        }
                        continue;
                    }

                    EventPoint pre = L.DirectUpperAndLower(current_event).Key;
                    EventPoint next = L.DirectUpperAndLower(current_event).Value;
                    if (pre != null)
                    {
                        if (TwoSegmentsIntersectionTest(lines[pre.index], lines[current_event.index]))
                        {
                            Point point_of_intersection = twoLinesIntersectionPoint(lines[pre.index], lines[current_event.index]);
                            if (!Q.Contains(new EventPoint(point_of_intersection, pre, current_event, 0)) && !Intersected_lines.Contains(new Tuple<int, int>(pre.index, current_event.index)))
                                Q.Add(new EventPoint(point_of_intersection, pre, current_event, 0));
                        }
                    }
                    if (next != null)
                    {
                        if (TwoSegmentsIntersectionTest(lines[current_event.index], lines[next.index]))
                        {
                            Point point_of_intersection = twoLinesIntersectionPoint(lines[current_event.index], lines[next.index]);
                            if (!Q.Contains(new EventPoint(point_of_intersection, current_event, next, 0)) && !Intersected_lines.Contains(new Tuple<int, int>(current_event.index, next.index)))
                                Q.Add(new EventPoint(point_of_intersection, current_event, next, 0));

                        }
                    }
                }
                else if (current_event.event_type == -1)
                {
                    EventPoint pre = L.DirectUpperAndLower(current_event).Key;
                    EventPoint next = L.DirectUpperAndLower(current_event).Value;
                    if (pre != null && next != null)
                    {
                        if (TwoSegmentsIntersectionTest(lines[pre.index], lines[next.index]))
                        {
                            Point point_of_intersection = twoLinesIntersectionPoint(lines[pre.index], lines[next.index]);
                            if (!Q.Contains(new EventPoint(point_of_intersection, pre, next, 0)) && !Intersected_lines.Contains(new Tuple<int, int>(pre.index, next.index)))
                                Q.Add(new EventPoint(point_of_intersection, pre, next, 0));

                        }
                    }
                    if (current_event.index != 0)
                    {
                        List<EventPoint> LL = L.RemoveAll(p => p.index == current_event.index).ToList();
                        for (int i = 0; i < LL.Count; ++i)
                            L.Remove(LL[i]);
                    }
                    else
                        L.Remove(current_event);

                }
                else
                {
                    EventPoint s1 = current_event.intersection_segment_1;
                    EventPoint s2 = current_event.intersection_segment_2;

                    if (HasValue(current_event.point.X) && HasValue(current_event.point.Y) && !Intersected_lines.Contains(new Tuple<int, int>(s1.index, s2.index)))
                    {
                        outPoints.Add(current_event.point);
                        Intersected_lines.Add(new Tuple<int, int>(s1.index, s2.index));
                        Intersected_lines.Add(new Tuple<int, int>(s2.index, s1.index));

                    }
                    EventPoint s1_prev = L.DirectUpperAndLower(s1).Key;
                    EventPoint s2_next = L.DirectUpperAndLower(s2).Value;
                    if (s1_prev != null)
                    {
                        if (TwoSegmentsIntersectionTest(lines[s1_prev.index], lines[s2.index]))
                        {
                            Point point_of_intersection = twoLinesIntersectionPoint(lines[s1_prev.index], lines[s2.index]);
                            if (!Q.Contains(new EventPoint(point_of_intersection, s1_prev, s2, 0)) && !Intersected_lines.Contains(new Tuple<int, int>(s1_prev.index, s2.index)))
                                Q.Add(new EventPoint(point_of_intersection, s1_prev, s2, 0));
                        }
                    }

                    if (s2_next != null)
                    {
                        if (TwoSegmentsIntersectionTest(lines[s1.index], lines[s2_next.index]))
                        {
                            Point point_of_intersection = twoLinesIntersectionPoint(lines[s1.index], lines[s2_next.index]);
                            if (!Q.Contains(new EventPoint(point_of_intersection, s1, s2_next, 0)) && !Intersected_lines.Contains(new Tuple<int, int>(s1.index, s2_next.index)))
                                Q.Add(new EventPoint(point_of_intersection, s1, s2_next, 0));
                        }
                    }
                    L.Remove(s1);
                    L.Remove(s2);
                    double PX = current_event.point.X + 0.3;
                    double PY1 = current_event.point.Y + 10000;
                    double PY2 = current_event.point.Y - 10000;
                    Line sweepL = new Line(new Point(PX, PY1), new Point(PX, PY2));
                    Point P1 = twoLinesIntersectionPoint(sweepL, lines[s1.index]);
                    Point P2 = twoLinesIntersectionPoint(sweepL, lines[s2.index]);
                    s1.point = P1;
                    s2.point = P2;
                    L.Add(s1);
                    L.Add(s2);
                }
            }
        }

        public override string ToString()
        {
            return "Sweep Line";
        }
    }
}
