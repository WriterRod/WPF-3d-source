using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Windows.Media.Media3D;

namespace StackedAreaGraph
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

        #region Planes

        // Find the intersection between three planes defined by three points on each.
        public static Point3D Intersect3Planes(
            Point3D p1a, Point3D p1b, Point3D p1c,
            Point3D p2a, Point3D p2b, Point3D p2c,
            Point3D p3a, Point3D p3b, Point3D p3c)
        {
            // Get the plane equations.
            double
                A1, B1, C1, D1,
                A2, B2, C2, D2,
                A3, B3, C3, D3;
            GetPlaneEquation(out A1, out B1, out C1, out D1, p1a, p1b, p1c);
            GetPlaneEquation(out A2, out B2, out C2, out D2, p2a, p2b, p2c);
            GetPlaneEquation(out A3, out B3, out C3, out D3, p3a, p3b, p3c);

            // Find the point of intersection.
            return Intersect3Planes(
                A1, B1, C1, D1,
                A2, B2, C2, D2,
                A3, B3, C3, D3);
        }

        // Find the equation of plane through the three points.
        private static void GetPlaneEquation(
            out double A, out double B, out double C, out double D,
            Point3D p1, Point3D p2, Point3D p3)
        {
            // Find two vectors in the plane.
            Vector3D v12 = p2 - p1;
            Vector3D v23 = p3 - p2;

            // Take the cross product to get a normal vector.
            Vector3D n = Vector3D.CrossProduct(v12, v23);
            n.Normalize();

            // Calculate the plane equation's coefficients.
            A = n.X;
            B = n.Y;
            C = n.Z;
            D = -(A * p1.X + B * p1.Y + C * p1.Z);
        }

        // Find the intersection between three planes defined by plane equations.
        private static Point3D Intersect3Planes(
            double A1, double B1, double C1, double D1,
            double A2, double B2, double C2, double D2,
            double A3, double B3, double C3, double D3)
        {
            return Gaussian(
                A1, B1, C1, -D1,
                A2, B2, C2, -D2,
                A3, B3, C3, -D3);
        }

        // Use Gaussian elimination to solve three equations with three unknowns.
        private static Point3D Gaussian(
            double A1, double B1, double C1, double D1,
            double A2, double B2, double C2, double D2,
            double A3, double B3, double C3, double D3)
        {
            // Build the array.
            double[,] arr =
            {
                {A1, B1, C1, D1, 0},
                {A2, B2, C2, D2, 0},
                {A3, B3, C3, D3, 0},
            };

            // Solve.
            const double tiny = 0.00001;
            const int numRows = 3;
            const int numCols = 3;
            for (int r = 0; r < numRows - 1; r++)
            {
                // Zero out all entries in column r after this row.
                // See if this row has a non-zero entry in column r.
                if (Math.Abs(arr[r, r]) < tiny)
                {
                    // Too close to zero. Try to swap with a later row.
                    for (int r2 = r + 1; r2 < numRows; r2++)
                    {
                        if (Math.Abs(arr[r2, r]) > tiny)
                        {
                            // This row will work. Swap them.
                            for (int c = 0; c <= numCols; c++)
                            {
                                double tmp = arr[r, c];
                                arr[r, c] = arr[r2, c];
                                arr[r2, c] = tmp;
                            }
                            break;
                        }
                    }
                }

                // If this row has a non-zero entry in column r, use it.
                if (Math.Abs(arr[r, r]) > tiny)
                {
                    // Zero out this column in later rows.
                    for (int r2 = r + 1; r2 < numRows; r2++)
                    {
                        double factor = -arr[r2, r] / arr[r, r];
                        for (int c = r; c <= numCols; c++)
                        {
                            arr[r2, c] = arr[r2, c] + factor * arr[r, c];
                        }
                    }
                }
            }

            // Backsolve.
            for (int r = numRows - 1; r >= 0; r--)
            {
                double tmp = arr[r, numCols];
                for (int r2 = r + 1; r2 < numRows; r2++)
                {
                    tmp -= arr[r, r2] * arr[r2, numCols + 1];
                }
                arr[r, numCols + 1] = tmp / arr[r, r];
            }

            // Return the result.
            return new Point3D(arr[0, numCols + 1], arr[1, numCols + 1], arr[2, numCols + 1]);
        }

        #endregion Planes

        #region Surfaces

        // Initialize points to define a flat surface with a given Y value.
        // Values numX and numZ give the number of points not the number of sections between points.
        public static Point3D[,] InitSurface(double y,
            int numX, double xmin, double xmax,
            int numZ, double zmin, double zmax)
        {
            double dx = (xmax - xmin) / (numX - 1);
            double dz = (zmax - zmin) / (numZ - 1);
            Point3D[,] surface = new Point3D[numX, numZ];
            for (int ix = 0; ix < numX; ix++)
            {
                double x = xmin + ix * dx;
                for (int iz = 0; iz < numX; iz++)
                {
                    double z = zmin + iz * dz;
                    surface[ix, iz] = new Point3D(x, y, z);
                }
            }
            return surface;
        }

        // Fractalize the surface by using midpoint displacement.
        public static Point3D[,] FractalizeSurface(Point3D[,] surface, int iterations,
            int seed, double minDy, double maxDy)
        {
            // Initialize the random number generator with the seed.
            Random rand;
            if (seed < 0) rand = new Random();
            else rand = new Random(seed);

            // Get the middle dy value.
            double midDy = (minDy + maxDy) / 2;
            double dySpread = maxDy - minDy;

            // Repeat for the desired number of iterations.
            for (int iteration = 0; iteration < iterations; iteration++)
            {
                // Get the array's current and new size.
                int numX1 = surface.GetUpperBound(0) + 1;
                int numZ1 = surface.GetUpperBound(1) + 1;
                int numX2 = numX1 * 2 - 1;
                int numZ2 = numZ1 * 2 - 1;

                // Expand the array.
                Point3D[,] surface2 = new Point3D[numX2, numZ2];

                // Copy the old values into the new array.
                for (int ix = 0; ix < numX1; ix++)
                    for (int iz = 0; iz < numZ1; iz++)
                        surface2[ix * 2, iz * 2] = surface[ix, iz];

                // Position midpoints.
                for (int ix = 1; ix < numX2; ix += 2)
                {
                    for (int iz = 0; iz < numZ2; iz += 2)
                    {
                        double x = surface2[ix - 1, iz].X + surface2[ix + 1, iz].X;
                        double y = surface2[ix - 1, iz].Y + surface2[ix + 1, iz].Y +
                            rand.NextDouble(minDy, maxDy);
                        double z = surface2[ix - 1, iz].Z + surface2[ix + 1, iz].Z;
                        surface2[ix, iz] = new Point3D(x / 2, y / 2, z / 2);
                    }
                }
                for (int iz = 1; iz < numZ2; iz += 2)
                {
                    for (int ix = 0; ix < numX2; ix += 2)
                    {
                        double x = surface2[ix, iz - 1].X + surface2[ix, iz + 1].X;
                        double y = surface2[ix, iz - 1].Y + surface2[ix, iz + 1].Y +
                            rand.NextDouble(minDy, maxDy);
                        double z = surface2[ix, iz - 1].Z + surface2[ix, iz + 1].Z;
                        surface2[ix, iz] = new Point3D(x / 2, y / 2, z / 2);
                    }
                }

                // Position center points.
                for (int ix = 1; ix < numX2; ix += 2)
                {
                    for (int iz = 1; iz < numZ2; iz += 2)
                    {
                        double x =
                            surface2[ix, iz - 1].X + surface2[ix, iz + 1].X +
                            surface2[ix - 1, iz].X + surface2[ix + 1, iz].X;
                        double y =
                            surface2[ix, iz - 1].Y + surface2[ix, iz + 1].Y +
                            surface2[ix - 1, iz].Y + surface2[ix + 1, iz].Y +
                                rand.NextDouble(minDy, maxDy);
                        double z =
                            surface2[ix, iz - 1].Z + surface2[ix, iz + 1].Z +
                            surface2[ix - 1, iz].Z + surface2[ix + 1, iz].Z;
                        surface2[ix, iz] = new Point3D(x / 4, y / 4, z / 4);
                    }
                }

                // Replace surface with surface2.
                surface = surface2;

                // Reduce the spread of random values.
                dySpread /= 2;
                minDy = midDy - dySpread / 2;
                maxDy = midDy + dySpread / 2;
            } // End looping for iterations.

            // Return the latest surface.
            return surface;
        }

        // Ensure that the surface's Y coordinates are within the given bounds.
        public static void LimitY(Point3D[,] surface,
            double minY = double.MinValue, double maxY = double.MaxValue)
        {
            int numX = surface.GetUpperBound(0) + 1;
            int numZ = surface.GetUpperBound(1) + 1;
            for (int ix = 0; ix < numX; ix++)
            {
                for (int iz = 0; iz < numZ; iz++)
                {
                    double y = surface[ix, iz].Y;
                    if (y < minY) y = minY;
                    if (y > maxY) y = maxY;
                    surface[ix, iz].Y = y;
                }
            }
        }

        #endregion Surfaces

    }
}
