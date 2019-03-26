using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace Interlocked
{
    public static class ArchimedeanSolids
    {
        // Precision for point comparison.
        private const double Precision = 0.0001;

        // Return two meshes, one holding truncated
        // corners and one holding modified faces.
        // The frac parameter tells how far along
        // each edge the corners should be truncated and
        // should be between 0.0 and 0.5
        public static void TruncateSolid(List<Point3D> vertices,
            List<Polygon> faces, double frac,
            out MeshGeometry3D corner_mesh, out MeshGeometry3D face_mesh)
        {
            // Make the corner mesh.
            corner_mesh = MakeTruncatedSolidCornerMesh(vertices, faces, frac);

            // Make the face mesh.
            face_mesh = MakeTruncatedSolidFaceMesh(vertices, faces, frac);
        }

        // Make a mesh for the corner faces of a truncated solid.
        private static MeshGeometry3D MakeTruncatedSolidCornerMesh(
            List<Point3D> vertices,
            List<Polygon> faces, double frac)
        {
            // Get the corner polygons.
            List<Polygon> corner_polygons =
                MakeTruncatedSolidCornerPolygons(vertices, faces, frac);

            // Make the corner mesh.
            MeshGeometry3D corner_mesh = new MeshGeometry3D();
            foreach (Polygon polygon in corner_polygons)
                corner_mesh.AddPolygon(polygon.Points);

            return corner_mesh;
        }

        // Make a list of polygons representing corner faces for a truncated solid.
        private static List<Polygon> MakeTruncatedSolidCornerPolygons(
            List<Point3D> vertices,
            List<Polygon> faces, double frac)
        {
            // Make a list to hold the new polygons.
            List<Polygon> corner_polygons = new List<Polygon>();

            // Consider each vertex.
            foreach (Point3D vertex in vertices)
            {
                // Find the faces containing this vertex.
                // Make a list of edges across those faces.
                List<Edge> edges = new List<Edge>();
                foreach (Polygon face in faces)
                {
                    // See if the vertex is in this face.
                    int i1 = PointIndex(face.Points, vertex, Precision);
                    if (i1 >= 0)
                    {
                        // This face contains the vertex.
                        // Find the indices of the points
                        // that are adjacent to the vertex.
                        int i0 = i1 - 1;
                        if (i0 < 0) i0 += face.Points.Length;
                        int i2 = (i1 + 1) % face.Points.Length;

                        // Calculate the edge end points.
                        Vector3D v10 = face.Points[i0] - face.Points[i1];
                        Point3D p0 = face.Points[i1] + frac * v10;
                        Vector3D v12 = face.Points[i2] - face.Points[i1];
                        Point3D p2 = face.Points[i1] + frac * v12;

                        edges.Add(new Edge(p0, p2));
                    } // End finding faces that contain this vertex.
                }

                // Order the edges so they form a polygon.
                Polygon polygon = EdgesToPolygon(edges);

                // Orient the polygon's points outwardly
                // with respect to the vertex.
                OrientPolygon(polygon.Points, vertex);

                // Add the polygon to the list of corner polygons.
                corner_polygons.Add(polygon);
            } // End examining each vertex.

            return corner_polygons;
        }

        // Order the edges so they form a non-intersecting
        // polygon around the vertex.
        private static Polygon EdgesToPolygon(List<Edge> edges)
        {
            // Make the result point list.
            List<Point3D> points = new List<Point3D>();

            // Add the first edge to the new list.
            Edge edge = edges[0];
            Point3D last_point = edge.Point2;
            points.Add(last_point);
            edges.RemoveAt(0);

            // Save the last point.

            // Add the other edges.
            while (edges.Count > 0)
            {
                bool found_edge = false;
                for (int i = 0; i < edges.Count; i++)
                {
                    // Check this edge.
                    edge = edges[i];

                    // See if the edge's first point matches
                    // the last point in the polygon.
                    if (PointEquals(last_point, edge.Point1, Precision))
                    {
                        // Add edge.Point2 to the polygon.
                        last_point = edge.Point2;
                        points.Add(last_point);
                        edges.RemoveAt(i);
                        found_edge = true;
                        break;
                    }

                    // See if the edge's second point matches
                    // the last point in the polygon.
                    if (PointEquals(last_point, edge.Point2, Precision))
                    {
                        // Add edge.Point1 to the polygon.
                        last_point = edge.Point1;
                        points.Add(last_point);
                        edges.RemoveAt(i);
                        found_edge = true;
                        break;
                    }

                    // Continue looking for the next edge.                
                }

                // Make sure we found an edge.
                if (!found_edge)
                    throw new Exception("Could not find a next edge for the polygon.");
            }

            // Return the polygon.
            return new Polygon(points.ToArray());
        }

        // Orient the points so the polygon is outwardly
        // oriented with respect to the given point.
        private static void OrientPolygon(Point3D[] points, Point3D outside)
        {
            // See if the points are already correctly oriented.
            Vector3D v01 = points[1] - points[0];
            Vector3D v12 = points[2] - points[1];
            Vector3D cross = Vector3D.CrossProduct(v01, v12);

            Vector3D vOutside = outside - points[0];
            if (Vector3D.DotProduct(vOutside, cross) < 0)
            {
                // The points are inwardly oriented.
                // Reverse their order.
                Array.Reverse(points);
            }
        }

        // Return the index of a point within an array.
        private static int PointIndex(Point3D[] points, Point3D target, double precision)
        {
            for (int i = 0; i < points.Length; i++)
                if (PointEquals(target, points[i], precision))
                    return i;
            return -1;
        }

        // Return true if the points are the same.
        private static bool PointEquals(Point3D point1, Point3D point2, double precision)
        {
            Vector3D v = point1 - point2;
            return (v.Length <= precision);
        }

        // Make a mesh for the faces of a truncated solid.
        private static MeshGeometry3D MakeTruncatedSolidFaceMesh(
            List<Point3D> vertices,
            List<Polygon> faces, double frac)
        {
            // Get the modified face polygons.
            List<Polygon> face_polygons =
                MakeTruncatedSolidFacePolygons(vertices, faces, frac);

            // Make the face mesh.
            MeshGeometry3D face_mesh = new MeshGeometry3D();
            foreach (Polygon polygon in face_polygons)
                face_mesh.AddPolygon(polygon.Points);

            return face_mesh;
        }

        // Make polygons for the existing faces of a truncated solid.
        private static List<Polygon> MakeTruncatedSolidFacePolygons(
            List<Point3D> vertices,
            List<Polygon> faces, double frac)
        {
            List<Polygon> polygons = new List<Polygon>();

            // Consider each of the original faces.
            foreach (Polygon face in faces)
            {
                // Make a list to hold the new polygon's points.
                List<Point3D> points = new List<Point3D>();

                // Consider each edge in the polygon.
                for (int i0 = 0; i0 < face.Points.Length; i0++)
                {
                    // Consider the edge from point i0 to
                    // point i1 = i + 1.
                    int i1 = (i0 + 1) % face.Points.Length;
                    Vector3D v = face.Points[i1] - face.Points[i0];

                    // Add the points along the edge.
                    points.Add(face.Points[i0] + v * frac);
                    if (frac < 0.5)
                        points.Add(face.Points[i0] + v * (1 - frac));
                }

                // Make the new points into a polygon.
                polygons.Add(new Polygon(points.ToArray()));
            } // End looping through the faces.

            // Return the polygons.
            return polygons;
        }

        // Make a stellate truncated solid.
        private delegate void MakePolygonsMethod(
            double side_length, double frac,
            Transform3D transform,
            out List<Point3D> vertices, out List<Polygon> faces);
        private static void MakeStellateTruncatedSolid(
            double side_length, double frac,
            double corner_radius, double face_radius,
            Transform3D transform,
            out MeshGeometry3D corner_mesh, out MeshGeometry3D face_mesh,
            MakePolygonsMethod make_polygons)
        {
            // Get the solid's polygons.
            List<Point3D> vertices;
            List<Polygon> faces;
            make_polygons(
                side_length, frac, null,
                out vertices, out faces);

            // Truncate.
            // Get the corner polygons.
            List<Polygon> corner_polygons =
                MakeTruncatedSolidCornerPolygons(vertices, faces, frac);

            // Get the modified face polygons.
            List<Polygon> face_polygons =
                MakeTruncatedSolidFacePolygons(vertices, faces, frac);

            // Stellate.
            Point3D center = new Point3D(0, 0, 0);
            corner_mesh = new MeshGeometry3D();
            foreach (Polygon polygon in corner_polygons)
            {
                corner_mesh.AddTriangles(
                    StellatePolygon(polygon.Points, center, corner_radius));
            }
            if (transform != null) transform.Transform(corner_mesh);

            face_mesh = new MeshGeometry3D();
            foreach (Polygon polygon in face_polygons)
            {
                face_mesh.AddTriangles(
                    StellatePolygon(polygon.Points, center, face_radius));
            }
            if (transform != null) transform.Transform(face_mesh);
        }

        #region Truncated Tetrahedron

        // Make a truncated tetrahedron.
        public static void MakeTruncatedTetrahedron(
            double side_length, double frac, Transform3D transform,
            out MeshGeometry3D corner_mesh, out MeshGeometry3D face_mesh)
        {
            // Get the polygons.
            List<Point3D> vertices;
            List<Polygon> faces;
            MakeTruncatedTetrahedronPolygons(side_length, frac, transform,
                out vertices, out faces);

            // Truncate.
            TruncateSolid(
                vertices, faces, frac,
                out corner_mesh, out face_mesh);
        }

        // Make a truncated tetrahedron's polygons.
        public static void MakeTruncatedTetrahedronPolygons(
            double side_length, double frac, Transform3D transform,
            out List<Point3D> vertices, out List<Polygon> faces)
        {
            // Get the vertices.
            vertices = PlatonicSolids.TetrahedronVertices(side_length, true);

            // Transform the points if desired.
            if (transform != null)
            {
                Point3D[] point_array = vertices.ToArray();
                transform.Transform(point_array);
                vertices = new List<Point3D>(point_array);
            }

            // Make the faces.
            faces = new List<Polygon>();
            faces.Add(new Polygon(vertices[0], vertices[1], vertices[2]));
            faces.Add(new Polygon(vertices[0], vertices[2], vertices[3]));
            faces.Add(new Polygon(vertices[0], vertices[3], vertices[1]));
            faces.Add(new Polygon(vertices[3], vertices[2], vertices[1]));
        }

        // Make a stellate truncated tetrahedron.
        public static void MakeStellateTruncatedTetrahedron(
            double side_length, double frac,
            double corner_radius, double face_radius,
            Transform3D transform,
            out MeshGeometry3D corner_mesh, out MeshGeometry3D face_mesh)
        {
            MakeStellateTruncatedSolid(side_length, frac,
                corner_radius, face_radius, transform,
                out corner_mesh, out face_mesh,
                MakeTruncatedTetrahedronPolygons);
        }

        #endregion Truncated Tetrahedron

        #region Truncated Cube

        // Make a truncated cube.
        public static void MakeTruncatedCube(
            double side_length, double frac, Transform3D transform,
            out MeshGeometry3D corner_mesh, out MeshGeometry3D face_mesh)
        {
            // Get the polygons.
            List<Point3D> vertices;
            List<Polygon> faces;
            MakeTruncatedCubePolygons(side_length, frac, transform, out vertices, out faces);

            // Truncate.
            TruncateSolid(
                vertices, faces, frac,
                out corner_mesh, out face_mesh);
        }

        // Make a truncated cube's polygons.
        public static void MakeTruncatedCubePolygons(
            double side_length, double frac, Transform3D transform,
            out List<Point3D> vertices, out List<Polygon> faces)
        {
            // Get the vertices.
            vertices = PlatonicSolids.CubeVertices(side_length);

            // Transform the points if desired.
            if (transform != null)
            {
                Point3D[] point_array = vertices.ToArray();
                transform.Transform(point_array);
                vertices = new List<Point3D>(point_array);
            }

            // Make the faces.
            faces = new List<Polygon>();
            faces.Add(new Polygon(vertices[0], vertices[1], vertices[2], vertices[3]));
            faces.Add(new Polygon(vertices[0], vertices[4], vertices[5], vertices[1]));
            faces.Add(new Polygon(vertices[1], vertices[5], vertices[6], vertices[2]));
            faces.Add(new Polygon(vertices[2], vertices[6], vertices[7], vertices[3]));
            faces.Add(new Polygon(vertices[3], vertices[7], vertices[4], vertices[0]));
            faces.Add(new Polygon(vertices[7], vertices[6], vertices[5], vertices[4]));
        }

        // Make a stellate truncated cube.
        public static void MakeStellateTruncatedCube(
            double side_length, double frac,
            double corner_radius, double face_radius,
            Transform3D transform,
            out MeshGeometry3D corner_mesh, out MeshGeometry3D face_mesh)
        {
            MakeStellateTruncatedSolid(side_length, frac,
                corner_radius, face_radius, transform,
                out corner_mesh, out face_mesh,
                MakeTruncatedCubePolygons);
        }

        #endregion Truncated Cube

        #region Truncated Octahedron

        // Make a truncated octahedron.
        public static void MakeTruncatedOctahedron(
            double side_length, double frac, Transform3D transform,
            out MeshGeometry3D corner_mesh, out MeshGeometry3D face_mesh)
        {
            // Get the polygons.
            List<Point3D> vertices = PlatonicSolids.OctahedronVertices(side_length);
            List<Polygon> faces = new List<Polygon>();
            MakeTruncatedOctahedronPolygons(side_length, frac, transform,
                out vertices, out faces);

            // Truncate.
            TruncateSolid(
                vertices, faces, frac,
                out corner_mesh, out face_mesh);
        }

        // Make a truncated octahedron's polygons.
        public static void MakeTruncatedOctahedronPolygons(
            double side_length, double frac, Transform3D transform,
            out List<Point3D> vertices, out List<Polygon> faces)
        {
            // Get the vertices.
            vertices = PlatonicSolids.OctahedronVertices(side_length);

            // Transform the points if desired.
            if (transform != null)
            {
                Point3D[] point_array = vertices.ToArray();
                transform.Transform(point_array);
                vertices = new List<Point3D>(point_array);
            }

            // Make the faces.
            faces = new List<Polygon>();
            faces.Add(new Polygon(vertices[0], vertices[1], vertices[2]));
            faces.Add(new Polygon(vertices[0], vertices[2], vertices[3]));
            faces.Add(new Polygon(vertices[0], vertices[3], vertices[4]));
            faces.Add(new Polygon(vertices[0], vertices[4], vertices[1]));
            faces.Add(new Polygon(vertices[5], vertices[4], vertices[3]));
            faces.Add(new Polygon(vertices[5], vertices[3], vertices[2]));
            faces.Add(new Polygon(vertices[5], vertices[2], vertices[1]));
            faces.Add(new Polygon(vertices[5], vertices[1], vertices[4]));
        }

        // Make a stellate truncated octahedron.
        public static void MakeStellateTruncatedOctahedron(
            double side_length, double frac,
            double corner_radius, double face_radius,
            Transform3D transform,
            out MeshGeometry3D corner_mesh, out MeshGeometry3D face_mesh)
        {
            MakeStellateTruncatedSolid(side_length, frac,
                corner_radius, face_radius, transform,
                out corner_mesh, out face_mesh,
                MakeTruncatedOctahedronPolygons);
        }

        #endregion Truncated Octahedron

        #region Truncated Dodecahedron

        // Make a truncated dodecahedron.
        public static void MakeTruncatedDodecahedron(
            double side_length, double frac, Transform3D transform,
            out MeshGeometry3D corner_mesh, out MeshGeometry3D face_mesh)
        {
            // Get the vertices.
            List<Point3D> vertices;
            List<Polygon> faces;
            MakeTruncatedDodecahedronPolygons(
                side_length, frac, transform, out vertices, out faces);

            // Truncate.
            TruncateSolid(
                vertices, faces, frac,
                out corner_mesh, out face_mesh);
        }

        // Make a truncated dodecahedron's polygons.
        public static void MakeTruncatedDodecahedronPolygons(
            double side_length, double frac, Transform3D transform,
            out List<Point3D> vertices, out List<Polygon> faces)
        {
            // Get the vertices.
            vertices = PlatonicSolids.DodecahedronVertices(side_length);

            // Transform the points if desired.
            if (transform != null)
            {
                Point3D[] point_array = vertices.ToArray();
                transform.Transform(point_array);
                vertices = new List<Point3D>(point_array);
            }

            // Make the faces.
            faces = new List<Polygon>();
            faces.Add(new Polygon("EDCBA", vertices));
            faces.Add(new Polygon("ABGKF", vertices));
            faces.Add(new Polygon("AFOJE", vertices));
            faces.Add(new Polygon("EJNID", vertices));
            faces.Add(new Polygon("DIMHC", vertices));
            faces.Add(new Polygon("CHLGB", vertices));
            faces.Add(new Polygon("KPTOF", vertices));
            faces.Add(new Polygon("OTSNJ", vertices));
            faces.Add(new Polygon("NSRMI", vertices));
            faces.Add(new Polygon("MRQLH", vertices));
            faces.Add(new Polygon("LQPKG", vertices));
            faces.Add(new Polygon("PQRST", vertices));
        }

        // Make a stellate truncated dodecahedron.
        public static void MakeStellateTruncatedDodecahedron(
            double side_length, double frac,
            double corner_radius, double face_radius,
            Transform3D transform,
            out MeshGeometry3D corner_mesh, out MeshGeometry3D face_mesh)
        {
            MakeStellateTruncatedSolid(side_length, frac,
                corner_radius, face_radius, transform,
                out corner_mesh, out face_mesh,
                MakeTruncatedDodecahedronPolygons);
        }

        #endregion Truncated Dodecahedron

        #region Truncated Icosahedron

        // Make a truncated icosahedron.
        public static void MakeTruncatedIcosahedron(
            double side_length, double frac, Transform3D transform,
            out MeshGeometry3D corner_mesh, out MeshGeometry3D face_mesh)
        {
            // Get the polygons.
            List<Point3D> vertices;
            List<Polygon> faces;
            MakeTruncatedIcosahedronPolygons(
                side_length, frac, transform,
                out vertices, out faces);

            // Truncate.
            TruncateSolid(
                vertices, faces, frac,
                out corner_mesh, out face_mesh);
        }

        // Return the vertices and polygon faces for a truncated icosahedron.
        public static void MakeTruncatedIcosahedronPolygons(
            double side_length, double frac, Transform3D transform,
            out List<Point3D> vertices, out List<Polygon> faces)
        {
            // Get the vertices.
            vertices = PlatonicSolids.IcosahedronVertices(side_length);

            // Transform the points if desired.
            if (transform != null)
            {
                Point3D[] point_array = vertices.ToArray();
                transform.Transform(point_array);
                vertices = new List<Point3D>(point_array);
            }

            // Make the faces.
            faces = new List<Polygon>();
            faces.Add(new Polygon(vertices[0], vertices[2], vertices[1]));
            faces.Add(new Polygon(vertices[0], vertices[3], vertices[2]));
            faces.Add(new Polygon(vertices[0], vertices[4], vertices[3]));
            faces.Add(new Polygon(vertices[0], vertices[5], vertices[4]));
            faces.Add(new Polygon(vertices[0], vertices[1], vertices[5]));

            faces.Add(new Polygon(vertices[1], vertices[2], vertices[9]));
            faces.Add(new Polygon(vertices[2], vertices[3], vertices[10]));
            faces.Add(new Polygon(vertices[3], vertices[4], vertices[6]));
            faces.Add(new Polygon(vertices[4], vertices[5], vertices[7]));
            faces.Add(new Polygon(vertices[5], vertices[1], vertices[8]));

            faces.Add(new Polygon(vertices[6], vertices[4], vertices[7]));
            faces.Add(new Polygon(vertices[7], vertices[5], vertices[8]));
            faces.Add(new Polygon(vertices[8], vertices[1], vertices[9]));
            faces.Add(new Polygon(vertices[9], vertices[2], vertices[10]));
            faces.Add(new Polygon(vertices[10], vertices[3], vertices[6]));

            faces.Add(new Polygon(vertices[11], vertices[6], vertices[7]));
            faces.Add(new Polygon(vertices[11], vertices[7], vertices[8]));
            faces.Add(new Polygon(vertices[11], vertices[8], vertices[9]));
            faces.Add(new Polygon(vertices[11], vertices[9], vertices[10]));
            faces.Add(new Polygon(vertices[11], vertices[10], vertices[6]));
        }

        // Make a stellate truncated icosahedron.
        public static void MakeStellateTruncatedIcosahedron(
            double side_length, double frac,
            double corner_radius, double face_radius,
            Transform3D transform,
            out MeshGeometry3D corner_mesh, out MeshGeometry3D face_mesh)
        {
            MakeStellateTruncatedSolid(side_length, frac,
                corner_radius, face_radius, transform,
                out corner_mesh, out face_mesh,
                MakeTruncatedIcosahedronPolygons);
        }

        #endregion Truncated Icosahedron

        #region Stellate

        // Make triangles to stellate this polygon.
        public static List<Triangle> StellatePolygon(Point3D[] points, Point3D center, double radius)
        {
            // Find the point in the middle of the polygon.
            double x = 0;
            double y = 0;
            double z = 0;
            foreach (Point3D point in points)
            {
                x += point.X;
                y += point.Y;
                z += point.Z;
            }
            int num_points = points.Length;
            Point3D peak = new Point3D(
                x / num_points,
                y / num_points,
                z / num_points);

            // Give the peak its desired radius.
            NormalizePoint(ref peak, center, radius);

            // Make the new triangles.
            List<Triangle> result = new List<Triangle>();
            for (int i = 0; i < num_points; i++)
                result.Add(new Triangle(points[i], points[(i + 1) % num_points], peak));

            return result;
        }

        // Make the point the indicated distance away from the center.
        private static void NormalizePoint(ref Point3D point, Point3D center, double distance)
        {
            Vector3D vector = point - center;
            point = center + vector / vector.Length * distance;
        }

        #endregion Stellate
    }
}
