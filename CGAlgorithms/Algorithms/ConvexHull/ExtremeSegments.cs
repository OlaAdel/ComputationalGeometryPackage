using CGUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CGAlgorithms.Algorithms.ConvexHull
{
    public class ExtremeSegments : Algorithm
    {
        public override void Run(List<Point> points, List<Line> lines, List<Polygon> polygons, ref List<Point> outPoints, ref List<Line> outLines, ref List<Polygon> outPolygons)
        {
            if (points.Count == 1)
            {
                outPoints = points;
                return;
            }
            for (int i = 0; i < points.Count; ++i)
            {
                for (int j = i + 1; j < points.Count; ++j)
                {
                    int r_dir = 0, l_dir = 0;

                    for (int p = 0; p < points.Count; ++p)
                    {
                        if (i == p || j == p) continue;

                        if (HelperMethods.CheckTurn(new Line(points[i], points[j]), points[p]) == Enums.TurnType.Right)
                            ++r_dir;

                        if (HelperMethods.CheckTurn(new Line(points[i], points[j]), points[p]) == Enums.TurnType.Left)
                            ++l_dir;                        
                    }
                    if (l_dir == points.Count - 2 || r_dir == points.Count - 2)
                    {
                        outLines.Add(new Line(points[i], points[j]));
                        if (!outPoints.Contains(points[i])) outPoints.Add(points[i]);
                        if (!outPoints.Contains(points[j])) outPoints.Add(points[j]);
                    }
                }
            }
        }
        public override string ToString()
        {
            return "Convex Hull - Extreme Segments";
        }
    }
}
