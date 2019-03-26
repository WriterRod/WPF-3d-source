using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Windows.Media.Media3D;

namespace TorusWireframe
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

        // Verify that the points are the same distance from the origin.
        public static void VerifyPoints(params Point3D[] points)
        {
            double d0 = (points[0] - D3.Origin).Length;
            for (int i = 1; i < points.Length; i++)
            {
                double d1 = (points[i] - D3.Origin).Length;
                if (Math.Abs(d1 - d0) > 0.001)
                    throw new Exception("VerifyPoints: Distance " +
                        d1 + " not close enough to " + d0);
            }
        }

        // Verify that the points in a polygon are the same distance apart.
        public static void VerifyPolygon(params Point3D[] points)
        {
            double d0 = (points[points.Length - 1] - points[0]).Length;
            for (int i = 1; i < points.Length; i++)
            {
                double d1 = (points[i] - points[i - 1]).Length;
                if (Math.Abs(d1 - d0) > 0.001)
                    throw new Exception("VerifyPolygon: Distance " +
                        d1 + " not close enough to " + d0);
            }
        }

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

        // Cube.
        public static void CubePoints(
            out Point3D A, out Point3D B, out Point3D C, out Point3D D,
            out Point3D E, out Point3D F, out Point3D G, out Point3D H)
        {
            A = new Point3D(-1, +1, +1);
            B = new Point3D(+1, +1, +1);
            C = new Point3D(+1, +1, -1);
            D = new Point3D(-1, +1, -1);
            E = new Point3D(-1, -1, +1);
            F = new Point3D(+1, -1, +1);
            G = new Point3D(+1, -1, -1);
            H = new Point3D(-1, -1, -1);
        }
        public static double CubeCircumradius()
        {
            return Math.Sqrt(3.0);
        }
        public static double CubeInradius()
        {
            return 1;
        }

        // Octahedron.
        public static void OctahedronPoints(out Point3D A, out Point3D B,
            out Point3D C, out Point3D D, out Point3D E, out Point3D F)
        {
            A = new Point3D(0, 1, 0);
            B = new Point3D(1, 0, 0);
            C = new Point3D(0, 0, -1);
            D = new Point3D(-1, 0, 0);
            E = new Point3D(0, 0, 1);
            F = new Point3D(0, -1, 0);
        }
        public static double OctahedronCircumradius()
        {
            return 1;
        }
        public static double OctahedronInradius()
        {
            return Math.Sqrt(1.0 / 3.0);
        }

        // Dodecahedron.
        // Dodecahedron intermediate values.
        private static double ds = 2;
        //private static double dt1 = 2 * Math.PI / 5;    // Not actually used.
        private static double dt2 = Math.PI / 10;
        private static double dt3 = 3 * Math.PI / 10;
        private static double dt4 = Math.PI / 5;
        private static double dd1 = ds / 2 / Math.Sin(dt4);
        private static double dd2 = dd1 * Math.Cos(dt4);
        private static double dd3 = dd1 * Math.Cos(dt2);
        private static double dd4 = dd1 * Math.Sin(dt2);
        private static double dFx =
            (ds * ds - (2 * dd3) * (2 * dd3) -
                (dd1 * dd1 - dd3 * dd3 - dd4 * dd4)) /
            (2 * (dd4 - dd1));
        private static double dd5 = Math.Sqrt(
            0.5 * (ds * ds + (2 * dd3) * (2 * dd3) -
                (dd1 - dFx) * (dd1 - dFx) -
                (dd4 - dFx) * (dd4 - dFx) - dd3 * dd3));
        private static double dFy = (dFx * dFx - dd1 * dd1 -
            dd5 * dd5) / (2 * dd5);
        private static double dAy = dd5 + dFy;

        // Calculate the dodecahedron vertices.
        public static void DodecahedronPoints(
            out Point3D A, out Point3D B, out Point3D C, out Point3D D,
            out Point3D E, out Point3D F, out Point3D G, out Point3D H,
            out Point3D I, out Point3D J, out Point3D K, out Point3D L,
            out Point3D M, out Point3D N, out Point3D O, out Point3D P,
            out Point3D Q, out Point3D R, out Point3D S, out Point3D T)
        {
            // Make the points.
            A = new Point3D(dd1, dAy, 0);
            B = new Point3D(dd4, dAy, dd3);
            C = new Point3D(-dd2, dAy, ds / 2);
            D = new Point3D(-dd2, dAy, -ds / 2);
            E = new Point3D(dd4, dAy, -dd3);
            F = new Point3D(dFx, dFy, 0);
            G = new Point3D(dFx * Math.Sin(dt2), dFy, dFx * Math.Cos(dt2));
            H = new Point3D(-dFx * Math.Sin(dt3), dFy, dFx * Math.Cos(dt3));
            I = new Point3D(-dFx * Math.Sin(dt3), dFy, -dFx * Math.Cos(dt3));
            J = new Point3D(dFx * Math.Sin(dt2), dFy, -dFx * Math.Cos(dt2));
            K = new Point3D(dFx * Math.Sin(dt3), -dFy, dFx * Math.Cos(dt3));
            L = new Point3D(-dFx * Math.Sin(dt2), -dFy, dFx * Math.Cos(dt2));
            M = new Point3D(-dFx, -dFy, 0);
            N = new Point3D(-dFx * Math.Sin(dt2), -dFy, -dFx * Math.Cos(dt2));
            O = new Point3D(dFx * Math.Sin(dt3), -dFy, -dFx * Math.Cos(dt3));
            P = new Point3D(dd2, -dAy, ds / 2);
            Q = new Point3D(-dd4, -dAy, dd3);
            R = new Point3D(-dd1, -dAy, 0);
            S = new Point3D(-dd4, -dAy, -dd3);
            T = new Point3D(dd2, -dAy, -ds / 2);
        }
        public static double DodecahedronCircumradius()
        {
            // Get intermediate values.
            return Math.Sqrt(dd1 * dd1 + dAy * dAy);
        }
        public static double DodecahedronInradius()
        {
            return dAy;
        }

        // Icosahedron.
        // Icosahedron intermediate values.
        private static double s = 2;
        //private static double t1 = 2 * Math.PI / 5;     // Not actually used.
        private static double t2 = Math.PI / 10;
        private static double t4 = Math.PI / 5;
        //private static double t3 = -3 * Math.PI / 10;   // Not actually used.
        private static double r = (s / 2) / Math.Sin(t4);
        private static double h = r * Math.Cos(t4);
        private static double h1 = Math.Sqrt(s * s - r * r);
        private static double h2 = Math.Sqrt((h + r) * (h + r) - h * h);
        private static double cx = r * Math.Sin(t2);
        private static double cz = r * Math.Cos(t2);
        private static double y2 = (h2 - h1) / 2;
        private static double y1 = y2 + h1;

        // Calculate the icosahedron vertices.
        public static void IcosahedronPoints(
            out Point3D A, out Point3D B, out Point3D C, out Point3D D,
            out Point3D E, out Point3D F, out Point3D G, out Point3D H,
            out Point3D I, out Point3D J, out Point3D K, out Point3D L)
        {
            // Make the points.

            A = new Point3D(0, y1, 0);
            B = new Point3D(r, y2, 0);
            C = new Point3D(cx, y2, cz);
            D = new Point3D(-h, y2, s / 2);
            E = new Point3D(-h, y2, -s / 2);
            F = new Point3D(cx, y2, -cz);
            G = new Point3D(-r, -y2, 0);
            H = new Point3D(-cx, -y2, -cz);
            I = new Point3D(h, -y2, -s / 2);
            J = new Point3D(h, -y2, s / 2);
            K = new Point3D(-cx, -y2, cz);
            L = new Point3D(0, -y1, 0);
        }
        public static double IcosahedronCircumradius()
        {
            // Get intermediate values.
            return y1;
        }
        public static double IcosahedronInradius()
        {
            return 1.0 / 3.0 * Math.Sqrt(
                (r + cx) * (r + cx) +
                (y1 + y2 + y2) * (y1 + y2 + y2) +
                (cz) * (cz));
        }

        #endregion Platonic Solids

    }
}
