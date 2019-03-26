using System;
using System.Collections.Generic;

using System.Windows.Media.Media3D;

namespace Interlocked
{
    public static class PlatonicSolids
    {
        #region Stellate Sphere

        // Add a stellate sphere to the mesh.
        public static void AddStellateSphere(this MeshGeometry3D mesh,
            int geodesic_level, double side_length, double stellate_r,
            Transform3D transform)
        {
            // Get triangles to define the solid.
            List<Triangle> triangles = StellateSphereTriangles(
                geodesic_level, side_length, stellate_r, transform);

            // Add the Triangles to the mesh.
            mesh.AddTriangles(triangles);
        }

        // Return triangles that define a stellate sphere.
        public static List<Triangle> StellateSphereTriangles(int geodesic_level,
            double side_length, double stellate_r, Transform3D transform)
        {
            // Get triangles that define a geodesic
            // sphere of the desired level.
            List<Triangle> triangles = GeodesicSphereTriangles(
                geodesic_level, side_length, transform);

            // Stellate.
            List<Triangle> stellate_triangles = new List<Triangle>();
            Point3D center = new Point3D(0, 0, 0);
            if (transform != null) center = transform.Transform(center);
            foreach (Triangle triangle in triangles)
            {
                triangle.Stellate(stellate_triangles, center, stellate_r);
            }
            triangles = stellate_triangles;

            return triangles;
        }

        #endregion Stellate Sphere

        #region Geodesic Sphere

        // Add a geodesic sphere to the mesh.
        public static void AddGeodesicSphere(this MeshGeometry3D mesh,
            int geodesic_level, double side_length, Transform3D transform)
        {
            // Get triangles to define the solid.
            List<Triangle> triangles = GeodesicSphereTriangles(
                geodesic_level, side_length, transform);

            // Add the Triangles to the mesh.
            mesh.AddTriangles(triangles);
        }

        // Return triangles that define a geodesic sphere.
        public static List<Triangle> GeodesicSphereTriangles(int geodesic_level,
            double side_length, Transform3D transform)
        {
            // Get the icosahedron triangles.
            List<Triangle> triangles = IcoshedronTriangles(side_length, transform);

            // The radius is the distance from
            // any point to the center of the sphere.
            Point3D center = new Point3D(0, 0, 0);
            if (transform != null) center = transform.Transform(center);
            Vector3D v = triangles[0].Points[0] - center;
            double radius = v.Length;

            // Divide the triangles if desired
            // to make the geodesic sphere.
            for (int i = 0; i < geodesic_level; i++)
            {
                List<Triangle> new_triangles = new List<Triangle>();
                foreach (Triangle triangle in triangles)
                {
                    triangle.Subdivide(new_triangles, center, radius);
                }
                triangles = new_triangles;
            }

            return triangles;
        }

        #endregion Geodesic Sphere

        #region Icosahedron

        // Add an icosahedron to the mesh.
        public static void AddIcosahedron(this MeshGeometry3D mesh,
            double side_length, Transform3D transform)
        {
            // Get triangles to define the solid.
            List<Triangle> triangles = IcoshedronTriangles(side_length, transform);

            // Add the Triangles to the mesh.
            mesh.AddTriangles(triangles);
        }

        // Return triangles that define an icosahedron.
        public static List<Triangle> IcoshedronTriangles(double side_length, Transform3D transform)
        {
            // Get the vertices.
            Point3D[] points = IcosahedronVertices(side_length).ToArray();
            if (transform != null) transform.Transform(points);

            // Make the triangles.
            List<Triangle> triangles = new List<Triangle>();
            triangles.Add(new Triangle(points[0], points[2], points[1]));
            triangles.Add(new Triangle(points[0], points[3], points[2]));
            triangles.Add(new Triangle(points[0], points[4], points[3]));
            triangles.Add(new Triangle(points[0], points[5], points[4]));
            triangles.Add(new Triangle(points[0], points[1], points[5]));

            triangles.Add(new Triangle(points[1], points[2], points[9]));
            triangles.Add(new Triangle(points[2], points[3], points[10]));
            triangles.Add(new Triangle(points[3], points[4], points[6]));
            triangles.Add(new Triangle(points[4], points[5], points[7]));
            triangles.Add(new Triangle(points[5], points[1], points[8]));

            triangles.Add(new Triangle(points[6], points[4], points[7]));
            triangles.Add(new Triangle(points[7], points[5], points[8]));
            triangles.Add(new Triangle(points[8], points[1], points[9]));
            triangles.Add(new Triangle(points[9], points[2], points[10]));
            triangles.Add(new Triangle(points[10], points[3], points[6]));

            triangles.Add(new Triangle(points[11], points[6], points[7]));
            triangles.Add(new Triangle(points[11], points[7], points[8]));
            triangles.Add(new Triangle(points[11], points[8], points[9]));
            triangles.Add(new Triangle(points[11], points[9], points[10]));
            triangles.Add(new Triangle(points[11], points[10], points[6]));
            return triangles;
        }

        // Return the vertices for an icosahedron.
        public static List<Point3D> IcosahedronVertices(double side_length)
        {
            double S = side_length;
            //double t1 = 2.0 * Math.PI / 5;        // Not used.
            double t2 = Math.PI / 10.0;
            double t4 = Math.PI / 5.0;
            //double t3 = -3.0 * Math.PI / 10.0;    // Not used.
            double R = (S / 2.0) / Math.Sin(t4);
            double H = Math.Cos(t4) * R;
            double Cx = R * Math.Sin(t2);
            double Cz = R * Math.Cos(t2);
            double H1 = Math.Sqrt(S * S - R * R);
            double H2 = Math.Sqrt((H + R) * (H + R) - H * H);
            double Y2 = (H2 - H1) / 2.0;
            double Y1 = Y2 + H1;

            List<Point3D> points = new List<Point3D>();
            points.Add(new Point3D(0, Y1, 0));
            points.Add(new Point3D(R, Y2, 0));
            points.Add(new Point3D(Cx, Y2, Cz));
            points.Add(new Point3D(-H, Y2, S / 2));
            points.Add(new Point3D(-H, Y2, -S / 2));
            points.Add(new Point3D(Cx, Y2, -Cz));
            points.Add(new Point3D(-R, -Y2, 0));
            points.Add(new Point3D(-Cx, -Y2, -Cz));
            points.Add(new Point3D(H, -Y2, -S / 2));
            points.Add(new Point3D(H, -Y2, S / 2));
            points.Add(new Point3D(-Cx, -Y2, Cz));
            points.Add(new Point3D(0, -Y1, 0));

            return points;
        }

        #endregion Icosahedron

        #region Dodecahedron

        // Add a dodecahedron to the mesh.
        public static void AddDodecahedron(this MeshGeometry3D mesh,
            double side_length, Transform3D transform)
        {
            // Get the dodecahedron's points.
            Point3D[] points = DodecahedronVertices(side_length).ToArray();
            if (transform != null) transform.Transform(points);

            // Create the solid dodecahedron.
            mesh.AddPolygon("EDCBA", points);
            mesh.AddPolygon("ABGKF", points);
            mesh.AddPolygon("AFOJE", points);
            mesh.AddPolygon("EJNID", points);
            mesh.AddPolygon("DIMHC", points);
            mesh.AddPolygon("CHLGB", points);
            mesh.AddPolygon("KPTOF", points);
            mesh.AddPolygon("OTSNJ", points);
            mesh.AddPolygon("NSRMI", points);
            mesh.AddPolygon("MRQLH", points);
            mesh.AddPolygon("LQPKG", points);
            mesh.AddPolygon("PQRST", points);
        }

        // Add a dodecahedron's wireframe to the mesh.
        public static void AddDodecahedronWireframe(this MeshGeometry3D mesh,
            double side_length, double thickness, Transform3D transform)
        {
            // Get the dodecahedron's points.
            Point3D[] points = DodecahedronVertices(side_length).ToArray();
            if (transform != null) transform.Transform(points);

            // Create the wireframe.
            mesh.AddPolygonWireframe("EDCBA", thickness, points);
            mesh.AddPolygonWireframe("ABGKF", thickness, points);
            mesh.AddPolygonWireframe("AFOJE", thickness, points);
            mesh.AddPolygonWireframe("EJNID", thickness, points);
            mesh.AddPolygonWireframe("DIMHC", thickness, points);
            mesh.AddPolygonWireframe("CHLGB", thickness, points);
            mesh.AddPolygonWireframe("KPTOF", thickness, points);
            mesh.AddPolygonWireframe("OTSNJ", thickness, points);
            mesh.AddPolygonWireframe("NSRMI", thickness, points);
            mesh.AddPolygonWireframe("MRQLH", thickness, points);
            mesh.AddPolygonWireframe("LQPKG", thickness, points);
            mesh.AddPolygonWireframe("PQRST", thickness, points);
        }

        // Add a dodecahedron's normals to the mesh.
        public static void AddDodecahedronNormals(this MeshGeometry3D mesh,
            double side_length, double length, double thickness,
            Transform3D transform)
        {
            // Get the dodecahedron's points.
            Point3D[] points = DodecahedronVertices(side_length).ToArray();
            if (transform != null) transform.Transform(points);

            // Create the normals.
            mesh.AddPolygonNormal("EDCBA", length, thickness, points);
            mesh.AddPolygonNormal("ABGKF", length, thickness, points);
            mesh.AddPolygonNormal("AFOJE", length, thickness, points);
            mesh.AddPolygonNormal("EJNID", length, thickness, points);
            mesh.AddPolygonNormal("DIMHC", length, thickness, points);
            mesh.AddPolygonNormal("CHLGB", length, thickness, points);
            mesh.AddPolygonNormal("KPTOF", length, thickness, points);
            mesh.AddPolygonNormal("OTSNJ", length, thickness, points);
            mesh.AddPolygonNormal("NSRMI", length, thickness, points);
            mesh.AddPolygonNormal("MRQLH", length, thickness, points);
            mesh.AddPolygonNormal("LQPKG", length, thickness, points);
            mesh.AddPolygonNormal("PQRST", length, thickness, points);
        }

        // Return the vertices for an dodecahedron.
        public static List<Point3D> DodecahedronVertices(double side_length)
        {
            double s = side_length;
            //double t1 = 2.0 * Math.PI / 5.0;      // Not used.
            double t2 = Math.PI / 10.0;
            double t3 = 3.0 * Math.PI / 10.0;
            double t4 = Math.PI / 5.0;
            double d1 = s / 2.0 / Math.Sin(t4);
            double d2 = d1 * Math.Cos(t4);
            double d3 = d1 * Math.Cos(t2);
            double d4 = d1 * Math.Sin(t2);
            double Fx =
                (s * s - (2.0 * d3) * (2.0 * d3) - (d1 * d1 - d3 * d3 - d4 * d4)) /
                (2.0 * (d4 - d1));
            double d5 = Math.Sqrt(0.5 *
                (s * s + (2.0 * d3) * (2.0 * d3) -
                    (d1 - Fx) * (d1 - Fx) -
                        (d4 - Fx) * (d4 - Fx) - d3 * d3));
            double Fy = (Fx * Fx - d1 * d1 - d5 * d5) / (2.0 * d5);
            double Ay = d5 + Fy;

            Point3D A = new Point3D(d1, Ay, 0);
            Point3D B = new Point3D(d4, Ay, d3);
            Point3D C = new Point3D(-d2, Ay, s / 2);
            Point3D D = new Point3D(-d2, Ay, -s / 2);
            Point3D E = new Point3D(d4, Ay, -d3);
            Point3D F = new Point3D(Fx, Fy, 0);
            Point3D G = new Point3D(Fx * Math.Sin(t2), Fy, Fx * Math.Cos(t2));
            Point3D H = new Point3D(-Fx * Math.Sin(t3), Fy, Fx * Math.Cos(t3));
            Point3D I = new Point3D(-Fx * Math.Sin(t3), Fy, -Fx * Math.Cos(t3));
            Point3D J = new Point3D(Fx * Math.Sin(t2), Fy, -Fx * Math.Cos(t2));
            Point3D K = new Point3D(Fx * Math.Sin(t3), -Fy, Fx * Math.Cos(t3));
            Point3D L = new Point3D(-Fx * Math.Sin(t2), -Fy, Fx * Math.Cos(t2));
            Point3D M = new Point3D(-Fx, -Fy, 0);
            Point3D N = new Point3D(-Fx * Math.Sin(t2), -Fy, -Fx * Math.Cos(t2));
            Point3D O = new Point3D(Fx * Math.Sin(t3), -Fy, -Fx * Math.Cos(t3));
            Point3D P = new Point3D(d2, -Ay, s / 2);
            Point3D Q = new Point3D(-d4, -Ay, d3);
            Point3D R = new Point3D(-d1, -Ay, 0);
            Point3D S = new Point3D(-d4, -Ay, -d3);
            Point3D T = new Point3D(d2, -Ay, -s / 2);

            List<Point3D> points = new List<Point3D>();
            points.Add(A);
            points.Add(B);
            points.Add(C);
            points.Add(D);
            points.Add(E);
            points.Add(F);
            points.Add(G);
            points.Add(H);
            points.Add(I);
            points.Add(J);
            points.Add(K);
            points.Add(L);
            points.Add(M);
            points.Add(N);
            points.Add(O);
            points.Add(P);
            points.Add(Q);
            points.Add(R);
            points.Add(S);
            points.Add(T);

            return points;
        }

        #endregion Dodecahedron

        #region Octahedron

        // Add an octahedron to the mesh.
        public static void AddOctahedron(this MeshGeometry3D mesh,
            double side_length, Transform3D transform)
        {
            Point3D[] points = OctahedronVertices(side_length).ToArray();
            if (transform != null) transform.Transform(points);

            mesh.AddTriangle(points[0], points[1], points[2]);
            mesh.AddTriangle(points[0], points[2], points[3]);
            mesh.AddTriangle(points[0], points[3], points[4]);
            mesh.AddTriangle(points[0], points[4], points[1]);
            mesh.AddTriangle(points[5], points[4], points[3]);
            mesh.AddTriangle(points[5], points[3], points[2]);
            mesh.AddTriangle(points[5], points[2], points[1]);
            mesh.AddTriangle(points[5], points[1], points[4]);
        }

        // Return the vertices for an octahedron.
        public static List<Point3D> OctahedronVertices(double side_length)
        {
            double y = side_length / Math.Sqrt(2);
            List<Point3D> points = new List<Point3D>();
            points.Add(new Point3D(0, y, 0));
            points.Add(new Point3D(y, 0, 0));
            points.Add(new Point3D(0, 0, -y));
            points.Add(new Point3D(-y, 0, 0));
            points.Add(new Point3D(0, 0, y));
            points.Add(new Point3D(0, -y, 0));
            return points;
        }

        #endregion Octahedron

        #region Cube

        // Add a cube to the mesh.
        public static void AddCube(this MeshGeometry3D mesh,
            double side_length, Transform3D transform)
        {
            Point3D[] points = CubeVertices(side_length).ToArray();
            if (transform != null) transform.Transform(points);

            mesh.AddPolygon(points[0], points[1], points[2], points[3]);
            mesh.AddPolygon(points[0], points[4], points[5], points[1]);
            mesh.AddPolygon(points[1], points[5], points[6], points[2]);
            mesh.AddPolygon(points[2], points[6], points[7], points[3]);
            mesh.AddPolygon(points[3], points[7], points[4], points[0]);
            mesh.AddPolygon(points[7], points[6], points[5], points[4]);
        }

        // Add a cube's wireframe to the mesh.
        public static void AddCubeWireframe(this MeshGeometry3D mesh,
            double side_length, double thickness, Transform3D transform)
        {
            // Get the cube's points.
            Point3D[] points = CubeVertices(side_length).ToArray();
            if (transform != null) transform.Transform(points);

            // Create the wireframe.
            mesh.AddSegment(points[0], points[1], thickness);
            mesh.AddSegment(points[1], points[2], thickness);
            mesh.AddSegment(points[2], points[3], thickness);
            mesh.AddSegment(points[3], points[0], thickness);

            mesh.AddSegment(points[0], points[4], thickness);
            mesh.AddSegment(points[1], points[5], thickness);
            mesh.AddSegment(points[2], points[6], thickness);
            mesh.AddSegment(points[3], points[7], thickness);

            mesh.AddSegment(points[4], points[5], thickness);
            mesh.AddSegment(points[5], points[6], thickness);
            mesh.AddSegment(points[6], points[7], thickness);
            mesh.AddSegment(points[7], points[4], thickness);
        }

        // Add a cube's normals to the mesh.
        public static void AddCubeNormals(this MeshGeometry3D mesh,
            double side_length, double length, double thickness,
            Transform3D transform)
        {
            // Get the cube's points.
            Point3D[] points = CubeVertices(side_length).ToArray();
            if (transform != null) transform.Transform(points);

            // Create the normals.
            mesh.AddPolygonNormal(length, thickness,
                points[0], points[1], points[2], points[3]);
            mesh.AddPolygonNormal(length, thickness,
                points[0], points[4], points[5], points[1]);
            mesh.AddPolygonNormal(length, thickness,
                points[1], points[5], points[6], points[2]);
            mesh.AddPolygonNormal(length, thickness,
                points[2], points[6], points[7], points[3]);
            mesh.AddPolygonNormal(length, thickness,
                points[3], points[7], points[4], points[1]);
            mesh.AddPolygonNormal(length, thickness,
                points[7], points[6], points[5], points[4]);
        }

        // Return the vertices for an Cube.
        public static List<Point3D> CubeVertices(double side_length)
        {
            List<Point3D> points = new List<Point3D>();
            double y = side_length * 0.5;
            points.Add(new Point3D(y, y, y));
            points.Add(new Point3D(y, y, -y));
            points.Add(new Point3D(-y, y, -y));
            points.Add(new Point3D(-y, y, y));

            points.Add(new Point3D(y, -y, y));
            points.Add(new Point3D(y, -y, -y));
            points.Add(new Point3D(-y, -y, -y));
            points.Add(new Point3D(-y, -y, y));

            return points;
        }

        #endregion Cube

        #region Tetrahedron

        // Add a Tetrahedron to the mesh.
        public static void AddTetrahedron(this MeshGeometry3D mesh,
            double side_length, bool centered, Transform3D transform)
        {
            Point3D[] points = TetrahedronVertices(side_length, centered).ToArray();
            if (transform != null) transform.Transform(points);

            // Create the solid tetrahedron.
            MeshGeometry3D solid_mesh = new MeshGeometry3D();
            mesh.AddTriangle(points[0], points[1], points[2]);
            mesh.AddTriangle(points[0], points[2], points[3]);
            mesh.AddTriangle(points[0], points[3], points[1]);
            mesh.AddTriangle(points[3], points[2], points[1]);
        }

        // Return the vertices for an Tetrahedron.
        public static List<Point3D> TetrahedronVertices(double side_length, bool centered)
        {
            List<Point3D> points = new List<Point3D>();
            points.Add(new Point3D(0, Math.Sqrt(2.0 / 3.0), 0));
            points.Add(new Point3D(1 / Math.Sqrt(3.0), 0, 0));
            points.Add(new Point3D(-1.0 / (2.0 * Math.Sqrt(3)), 0, -0.5));
            points.Add(new Point3D(-1.0 / (2.0 * Math.Sqrt(3)), 0, 0.5));

            // Move the center to the origin.
            if (centered)
            {
                Vector3D center = new Vector3D(
                    (points[0].X + points[1].X + points[2].X + points[3].X) / 4,
                    (points[0].Y + points[1].Y + points[2].Y + points[3].Y) / 4,
                    (points[0].Z + points[1].Z + points[2].Z + points[3].Z) / 4);
                for (int i = 0; i < points.Count; i++)
                    points[i] = points[i] - center;
            }

            // Scale.
            for (int i = 0; i < points.Count; i++)
                points[i] = new Point3D(
                    side_length * points[i].X,
                    side_length * points[i].Y,
                    side_length * points[i].Z);

            return points;
        }

        #endregion Tetrahedron
    }
}
