using CGUtilities;
using CGUtilities.DataStructures;
using System;
using System.Collections.Generic;
using System.Linq;


namespace CGAlgorithms.Algorithms.PolygonTriangulation
{


    public class EventPoint
    {
        public Point point;
        public int index;
        public string event_type;
        public Point edge;

        public EventPoint(Point p, int index, string type, Point e)
        {
            this.point = p;
            this.index = index;
            this.event_type = type;
            this.edge = e;
        }

    }
    class MonotonePartitioning : Algorithm
    {
        public double Y = 0.0;

        public static double CalcAngle(Point p1, Point p, Point p2)
        {

            double angle = 0.0;

            Line l1 = new Line(p1, p);
            Line l2 = new Line(p, p2);

            Point a = HelperMethods.GetVector(l1);
            Point b = HelperMethods.GetVector(l2);
            double dotProduct = (a.X * b.X + a.Y * b.Y);
            double crossProduct = HelperMethods.CrossProduct(a, b);

            angle = Math.Atan2(crossProduct, dotProduct) * 180 / Math.PI;
            if (angle < 0)
                angle += 360;

            return angle;
        }



        public int Compare_Y(EventPoint E1, EventPoint E2)
        {
            if (E2.point.Y < E1.point.Y) return -1;
            if (E2.point.Y > E1.point.Y) return 1;

            if (E2.point.X < E1.point.X) return 1;
            if (E2.point.X > E1.point.X) return -1;

            return 0;
        }
        public int Compare_X(EventPoint E1, EventPoint E2)
        {
            double m1 = (E1.edge.Y - E1.point.Y) / (E1.edge.X - E1.point.X);
            double b1 = E1.point.Y - (m1 * E1.point.X);

            double m2 = (E2.edge.Y - E2.point.Y) / (E2.edge.X - E2.point.X);
            double b2 = E2.point.Y - (m2 * E2.point.X);

            double x1 = (Y - b1) / m1;
            double x2 = (Y - b2) / m2;

            if (x1 > x2) return 1;
            else if (x1 < x2) return -1;

            return 0;
        }




        public override void Run(List<CGUtilities.Point> points, List<CGUtilities.Line> lines, List<CGUtilities.Polygon> polygons, ref List<CGUtilities.Point> outPoints, ref List<CGUtilities.Line> outLines, ref List<CGUtilities.Polygon> outPolygons)
        {
            OrderedSet<EventPoint> Q = new OrderedSet<EventPoint>(Compare_Y);
            Dictionary<int, EventPoint> prev_event = new Dictionary<int, EventPoint>();
            Dictionary<int, EventPoint> helper = new Dictionary<int, EventPoint>();
            OrderedSet<EventPoint> T = new OrderedSet<EventPoint>(Compare_X);
            string type;



            for (int i = 0; i < polygons[0].lines.Count; ++i)
            {
                Point n1 = polygons[0].lines[(polygons[0].lines.Count + i - 1) % polygons[0].lines.Count].Start;
                Point p = polygons[0].lines[i].Start;
                Point n2 = polygons[0].lines[(i + 1) % polygons[0].lines.Count].Start;
                double angle = CalcAngle(n1, p, n2);

                if (((n1.Y < p.Y) || ((n1.Y == p.Y) && (n1.X > p.Y))) && ((n2.Y < p.Y) || ((n2.Y == p.Y) && (n2.X > p.X))))
                {
                    if (angle < 180)
                    {

                        type = "Start";

                    }
                    else
                    {

                        type = "Split";

                    }
                }
                else if (((p.Y < n1.Y) || ((p.Y == n1.Y) && (p.X > n1.Y))) && ((p.Y < n2.Y) || ((p.Y == n2.Y) && (p.X > n2.X))))
                {
                    if (angle < 180)
                    {

                        type = "End";

                    }
                    else
                    {

                        type = "Merge";

                    }
                }
                else
                {

                    type = "Regular";

                }

                EventPoint ev = new EventPoint(polygons[0].lines[i].Start, i, type, polygons[0].lines[i].End);
                Q.Add(ev);
                EventPoint prev;
                if (i != (polygons[0].lines.Count - 1))
                {
                    prev = new EventPoint(polygons[0].lines[i].Start, i, type, polygons[0].lines[i].End);
                    prev_event.Add(i + 1, prev);

                }
                else
                {
                    prev = new EventPoint(polygons[0].lines[i].Start, i, type, polygons[0].lines[i].End);
                    prev_event.Add(0, prev);
                }


            }

            while (Q.Count != 0)
            {
                EventPoint ev = Q.First();
                Y = ev.point.Y;
                if (ev.event_type == "Start")
                {
                    helper.Add(ev.index, ev);
                    T.Add(ev);
                }

                else if (ev.event_type == "End")
                {
                    if (ev.index - 1 >= 0)
                    {
                        if (helper[ev.index - 1].event_type == "Merge")
                        {
                            outLines.Add(new Line(ev.point, helper[ev.index - 1].point));
                        }
                        T.Remove(helper[ev.index - 1]);
                    }

                }

                else if (ev.event_type == "Merge")
                {
                    // if (ev.index - 1 >= 0)
                    //{
                    EventPoint ev2 = helper[ev.index - 1];
                    if (ev2.event_type == "Merge")
                    {
                        outLines.Add(new Line(ev.point, ev2.point));
                    }
                    EventPoint prev = prev_event[ev.index];
                    T.Remove(prev);
                    //}
                    KeyValuePair<EventPoint, EventPoint> temp = new KeyValuePair<EventPoint, EventPoint>(T.DirectUpperAndLower(ev).Key, T.DirectUpperAndLower(ev).Value);
                    if (temp.Value != null)
                    {
                        if (helper[temp.Value.index].event_type == "Merge")
                        {
                            outLines.Add(new Line(ev.point, helper[temp.Value.index].point));
                        }
                        helper[temp.Value.index] = ev;
                    }

                }

                else if (ev.event_type == "Split")
                {
                    KeyValuePair<EventPoint, EventPoint> temp = new KeyValuePair<EventPoint, EventPoint>(T.DirectUpperAndLower(ev).Key, T.DirectUpperAndLower(ev).Value);
                    if (temp.Value != null)
                    {
                        outLines.Add(new Line(ev.point, helper[temp.Value.index].point));
                        helper[temp.Value.index] = ev;
                    }
                    helper[ev.index] = ev;
                    T.Add(ev);
                }
                else
                {
                    var y = ev.edge.Y;
                    if (ev.point.Y > y)
                    {
                        if (ev.index - 1 >= 0)
                        {
                            if (helper[ev.index - 1].event_type == "Merge")
                            {
                                outLines.Add(new Line(ev.point, helper[ev.index - 1].point));
                            }

                            T.Remove(prev_event[ev.index]);
                        }
                        helper.Add(ev.index, ev);
                        T.Add(ev);
                    }
                    else
                    {
                        KeyValuePair<EventPoint, EventPoint> temp = new KeyValuePair<EventPoint, EventPoint>(T.DirectUpperAndLower(ev).Key, T.DirectUpperAndLower(ev).Value);
                        if (temp.Value != null)
                        {
                            if (helper[temp.Value.index].event_type == "Merge")
                            {
                                outLines.Add(new Line(ev.point, helper[temp.Value.index].point));
                                helper[temp.Value.index] = ev;
                            }

                        }
                    }
                }
                Q.Remove(ev);

            }

        }

        public override string ToString()
        {
            return "Monotone Partitioning";
        }
    }
}