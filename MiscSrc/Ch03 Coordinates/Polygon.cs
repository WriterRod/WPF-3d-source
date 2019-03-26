using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Windows.Media.Media3D;

namespace Interlocked
{
    // A 3D polygon.
    // Assumes the points are coplanar.
    public class Polygon
    {
        public Point3D[] Points;
        public Polygon(params Point3D[] points)
        {
            Points = points;
        }

        // Make a polygon from an array of points
        // and the vertex names A, B, C, etc.
        public Polygon(string names, List<Point3D> points)
        {
            Points = new Point3D[names.Length];
            for (int i = 0; i < names.Length; i++)
                Points[i] = points[ToIndex(names[i])];
        }

        // Find a point's index from its letter.
        private static int ToIndex(char ch)
        {
            return ch - 'A';
        }
    }
}
