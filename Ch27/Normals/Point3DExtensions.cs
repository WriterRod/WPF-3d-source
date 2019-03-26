using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Windows.Media.Media3D;

namespace Normals
{
    public static class Point3DExtensions
    {
        // Return a rounded Point3D so close points match.
        public static Point3D Round(this Point3D point, int decimals = 3)
        {
            double x = Math.Round(point.X, decimals);
            double y = Math.Round(point.Y, decimals);
            double z = Math.Round(point.Z, decimals);
            return new Point3D(x, y, z);
        }

        // Move this point along the vector to the center
        // so it has the given distance from the center.
        public static Point3D SetDistanceFrom(this Point3D point, Point3D center, double distance)
        {
            Vector3D v = point - center;
            return center + v / v.Length * distance;
        }
    }
}
