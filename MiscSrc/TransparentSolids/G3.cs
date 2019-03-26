using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Windows.Media.Media3D;

namespace Tetrahedron
{
    public static class G3
    {
        #region Polygons

        // Make points to define a regular polygon.
        public static Point3D[] MakePolygonPoints(int numSides,
            Point3D center, Vector3D vx, Vector3D vy)
        {
            // Generate the points.
            Point3D[] points = new Point3D[numSides];
            double dtheta = 2 * Math.PI / numSides;
            double theta = Math.PI / 2;
            for (int i = 0; i < numSides; i++)
            {
                points[i] = center + vx * Math.Cos(theta) + vy * Math.Sin(theta);
                theta += dtheta;
            }
            return points;
        }

        #endregion Polygons

        #region Spheres

        // Return a point on a sphere.
        public static Point3D SpherePoint(Point3D center, double r, double theta, double phi)
        {
            double y = r * Math.Cos(phi);
            double h = r * Math.Sin(phi);
            double x = h * Math.Sin(theta);
            double z = h * Math.Cos(theta);
            return center + new Vector3D(x, y, z);
        }

        #endregion Spheres

        #region Tori

        // Return a point on a torus.
        public static Point3D TorusPoint(Point3D center, double R, double r, double theta, double phi)
        {
            return new Point3D(
                center.X + (R + r * Math.Cos(theta)) * Math.Cos(phi),
                center.Y + r * Math.Sin(theta),
                center.Z + (R + r * Math.Cos(theta)) * Math.Sin(phi));
        }

        // Return a normal on a torus.
        public static Vector3D TorusNormal(Point3D center, double R, double r, double theta, double phi)
        {
            return (Vector3D)TorusPoint(center, 0, r, theta, phi);
        }

        #endregion Tori

        #region Platonic Solids

        // Tetrahedron.
        public static void TetrahedronPoints(
            out Point3D A, out Point3D B, out Point3D C, out Point3D D,
            bool centered)
        {
            double dy = 0;
            if (centered) dy = 0.25 * Math.Sqrt(2.0 / 3.0);

            A = new Point3D(0, Math.Sqrt(2.0 / 3.0) - dy, 0);
            B = new Point3D(1.0 / Math.Sqrt(3.0), -dy, 0);
            C = new Point3D(-1.0 / (2 * Math.Sqrt(3.0)), -dy, -1.0 / 2.0);
            D = new Point3D(-1.0 / (2 * Math.Sqrt(3.0)), -dy, 1.0 / 2.0);
        }
        public static double TetrahedronCircumradius()
        {
            return Math.Sqrt(2.0 / 3.0) * 0.75;
        }
        public static double TetrahedronInradius()
        {
            return Math.Sqrt(2.0 / 3.0) * 0.25;
        }

        #endregion Platonic Solids

    }
}
