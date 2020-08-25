using CGUtilities;
using System.Collections.Generic;
using System.Linq;

namespace CGAlgorithms.Algorithms.ConvexHull
{
    public class DivideAndConquer : Algorithm
    {

        public List<Point> merge(List<Point> aa, List<Point> bb)
        {
           
            //a = a.Distinct().ToList();
            //b = b.Distinct().ToList();
            List<Point> a = new List<Point>();
            List<Point> b = new List<Point>();


            for (int i = 0; i < aa.Count; ++i)
            {
                if (!a.Contains(aa[i]))

                    a.Add(aa[i]);
            }

            for (int i = 0; i < bb.Count; ++i)
            {
                if (!b.Contains(bb[i]))

                    b.Add(bb[i]);
            }
            //a = a.OrderBy(x => x.X).ThenBy(x => x.Y).ToList();
            //o = outPoints.OrderBy(x => x.X).ThenBy(x => x.Y).ToList();

            int n1 = a.Count;
            int n2 = b.Count;
            int ind1 = 0;
            int ind2 = 0;

            for (int i = 1; i < n1; i++)
            {
                if (a[i].X > a[ind1].X)
                    ind1 = i;
                else if (a[i].X == a[ind1].X)
                {
                    if (a[i].Y > a[ind1].Y)
                        ind1 = i;
                }

            }

            // ind2  leftmost point of b 
            for (int i = 1; i < n2; i++)
            {
                if (b[i].X < b[ind2].X)
                    ind2 = i;
                else if (b[i].X == b[ind2].X)
                {
                    if (b[i].Y < b[ind2].Y)
                        ind2 = i;
                }

            }

            //  upper tangent 
            int uppera  = ind1;
            int upperb = ind2;
            bool found = false;
            int num=0;

            while (!found)
            {
                found = true;
                while (CGUtilities.HelperMethods.CheckTurn(new Line(b[upperb].X,
                           b[upperb].Y, a[uppera].X, a[uppera].Y), 
                           a[(uppera + 1) % n1]) == Enums.TurnType.Right)
                {
                    uppera = (uppera + 1) % n1;
                    found = false;
                }
                if (found == true &&
                    (CGUtilities.HelperMethods.CheckTurn(new Line(b[upperb].X, b[upperb].Y, a[uppera].X, a[uppera].Y),
                         a[(uppera + 1) % n1]) == Enums.TurnType.Colinear))
                    uppera = (uppera + 1) % n1;

                while (CGUtilities.HelperMethods.CheckTurn(new Line(a[uppera].X, a[uppera].Y, b[upperb].X, b[upperb].Y), b[(n2 + upperb - 1) % n2]) == Enums.TurnType.Left)
                {
                    upperb = (n2 + upperb - 1) % n2;
                    found = false;

                }
                if (found == true && (CGUtilities.HelperMethods.CheckTurn(new Line(a[uppera].X, a[uppera].Y, b[upperb].X, b[upperb].Y), b[(upperb + n2 - 1) % n2]) == Enums.TurnType.Colinear))
                    upperb = (upperb + n2 - 1) % n2;


            }

            int lowera = ind1;
            int  lowerb = ind2;
            found = false;



            //lower tangent 
            while (!found)
            {
                found = true;
                while (CGUtilities.HelperMethods.CheckTurn(new Line(b[lowerb].X, b[lowerb].Y, a[lowera].X, a[lowera].Y), a[(lowera + n1 - 1) % n1]) == Enums.TurnType.Left)
                {
                    lowera = (lowera + n1 - 1) % n1;
                    found = false;
                }

                if (found == true &&
                    (CGUtilities.HelperMethods.CheckTurn(new Line(b[lowerb].X, b[lowerb].Y, a[lowera].X, a[lowera].Y),
                         a[(lowera + n1 - 1) % n1]) == Enums.TurnType.Colinear))
                    lowera = (lowera + n1 - 1) % n1;

                while (CGUtilities.HelperMethods.CheckTurn(new Line(a[lowera].X, a[lowera].Y, b[lowerb].X, b[lowerb].Y), b[(lowerb + 1) % n2]) == Enums.TurnType.Right)
                {
                    lowerb = (lowerb + 1) % n2;
                    found = false;

                }
                if (found == true && (CGUtilities.HelperMethods.CheckTurn(new Line(a[lowera].X, a[lowera].Y, b[lowerb].X, b[lowerb].Y), b[(lowerb + 1) % n2]) == Enums.TurnType.Colinear))
                    lowerb = (lowerb + 1) % n2;


            }

            List<Point> out_points = new List<Point>();


            int ind = uppera;
            if (!out_points.Contains(a[uppera]))
                out_points.Add(a[uppera]);

            while (ind != lowera)
            {
                ind = (ind + 1) % n1;


                if (!out_points.Contains(a[ind]))
                {
                    out_points.Add(a[ind]);

                }




            }

            ind = lowerb;
            if (!out_points.Contains(b[lowerb]))

                out_points.Add(b[lowerb]);

            while (ind != upperb)
            {
                ind = (ind + 1) % n2;

                if (!out_points.Contains(b[ind]))

                    out_points.Add(b[ind]);

            }




            return out_points;
        }
 
        public List<Point> divide(List<Point> a)
        {
            

            if (a.Count == 1)
            {
                return a;
            }

          
            List<Point> left = new List<Point>();
            List<Point> right = new List<Point>();
            for (int i = 0; i < a.Count / 2; i++)
                left.Add(a[i]);
            for (int i = a.Count / 2; i < a.Count; i++)
                right.Add(a[i]);

            List<Point> left_new = divide(left);
            List<Point> right_new = divide(right);

            return merge(left_new, right_new);
        }
        public override void Run(List<Point> points, List<Line> lines, List<Polygon> polygons, ref List<Point> outPoints, ref List<Line> outLines, ref List<Polygon> outPolygons)
        {

            points = points.OrderBy(x => x.X).ThenBy(x => x.Y).ToList();
            outPoints=new List<Point>();
            List<Point> new_po = divide(points);
            for (int i =0  ;i<new_po.Count; ++i)
                if (!outPoints.Contains(new_po[i]))
                {
                    outPoints.Add(new_po[i]);

                }

        }

        public override string ToString()
        {
            return "Convex Hull - Divide & Conquer";
        }

    }
}
