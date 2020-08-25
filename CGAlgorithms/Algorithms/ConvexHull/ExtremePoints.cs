using CGUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CGAlgorithms.Algorithms.ConvexHull
{
    public class ExtremePoints : Algorithm
    {
        public override void Run(List<Point> points, List<Line> lines, List<Polygon> polygons, ref List<Point> outPoints, ref List<Line> outLines, ref List<Polygon> outPolygons)
        {
            points = points.Distinct().ToList();

            List<int> plist = new List<int>();
            for (int i = 0; i < points.Count; ++i)
            {
                for (int j = 0; j < points.Count; ++j)
                {
                    if (i == j) continue;
                    for (int k = 0; k < points.Count; ++k)
                    {
                        if (j == k) continue;
                        for (int l = 0; l < points.Count; ++l)
                        {
                            if (k == l) continue;
                            if (HelperMethods.PointInTriangle(points[i], points[j], points[k], points[l]) == Enums.PointInPolygon.Inside)
                            {
                                plist.Add(i);
                                break;

                            }
                        }
                    }
                }
            }
            for (int i = 0; i < points.Count; ++i)
            {
                if (!plist.Contains(i))
                    outPoints.Add(points[i]);
            }

        }

        public override string ToString()
        {
            return "Convex Hull - Extreme Points";
        }
    }
}
