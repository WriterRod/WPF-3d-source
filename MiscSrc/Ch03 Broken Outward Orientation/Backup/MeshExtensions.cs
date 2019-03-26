using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Windows.Media;
using System.Windows.Media.Media3D;
using System.Windows;
using System.Windows.Media.Imaging;

namespace Interlocked
{
    public static partial class MeshExtensions
    {
        // Precision for point comparison.
        private const double Precision = 0.0001;

        // Add a normal for a polygon.
        // Assumes the points are coplanar.
        // Assumes the points are outwardly oriented.
        public static void AddPolygonNormal(this MeshGeometry3D mesh,
            double length, double thickness, params Point3D[] points)
        {
            // Find the "center" point.
            double x = 0, y = 0, z = 0;
            foreach (Point3D point in points)
            {
                x += point.X;
                y += point.Y;
                z += point.Z;
            }
            Point3D endpoint1 = new Point3D(
                x / points.Length,
                y / points.Length,
                z / points.Length);

            // Get the polygon's normal.
            Vector3D n = FindTriangleNormal(
                points[0], points[1], points[2]);

            // Set the length.
            n = n.Scale(length);

            // Find the segment's other end point.
            Point3D endpoint2 = endpoint1 + n;

            // Create the segment.
            AddSegment(mesh, endpoint1, endpoint2, thickness);
        }

        // Add a polygon's normal by using the point names A, B, C, etc.
        // This is used to make dodecahedron normals.
        public static void AddPolygonNormal(this MeshGeometry3D mesh,
            string point_names, double length, double thickness, Point3D[] points)
        {
            Point3D[] polygon_points = new Point3D[point_names.Length];
            for (int i = 0; i < point_names.Length; i++)
            {
                polygon_points[i] = points[ToIndex(point_names[i])];
            }
            AddPolygonNormal(mesh, length, thickness, polygon_points);
        }

        // Return a MeshGeometry3D representing this mesh's triangle normals.
        public static MeshGeometry3D ToTriangleNormals(this MeshGeometry3D mesh,
            double length, double thickness)
        {
            // Make a mesh to hold the normals.
            MeshGeometry3D normals = new MeshGeometry3D();

            // Loop through the mesh's triangles.
            for (int triangle = 0; triangle < mesh.TriangleIndices.Count; triangle += 3)
            {
                // Get the triangle's vertices.
                Point3D point1 = mesh.Positions[mesh.TriangleIndices[triangle]];
                Point3D point2 = mesh.Positions[mesh.TriangleIndices[triangle + 1]];
                Point3D point3 = mesh.Positions[mesh.TriangleIndices[triangle + 2]];

                // Make the triangle's normal
                AddTriangleNormal(mesh, normals,
                    point1, point2, point3, length, thickness);
            }

            return normals;
        }

        // Add a segment representing the triangle's normal to the normals mesh.
        private static void AddTriangleNormal(MeshGeometry3D mesh,
            MeshGeometry3D normals, Point3D point1, Point3D point2, Point3D point3,
            double length, double thickness)
        {
            // Get the triangle's normal.
            Vector3D n = FindTriangleNormal(point1, point2, point3);

            // Set the length.
            n = n.Scale(length);

            // Find the center of the triangle.
            Point3D endpoint1 = new Point3D(
                (point1.X + point2.X + point3.X) / 3.0,
                (point1.Y + point2.Y + point3.Y) / 3.0,
                (point1.Z + point2.Z + point3.Z) / 3.0);

            // Find the segment's other end point.
            Point3D endpoint2 = endpoint1 + n;

            // Create the segment.
            AddSegment(normals, endpoint1, endpoint2, thickness);
        }

        // Calculate a triangle's normal vector.
        public static Vector3D FindTriangleNormal(Point3D point1, Point3D point2, Point3D point3)
        {
            // Get two edge vectors.
            Vector3D v1 = point2 - point1;
            Vector3D v2 = point3 - point2;

            // Get the cross product.
            Vector3D n = Vector3D.CrossProduct(v1, v2);

            // Normalize.
            n.Normalize();

            return n;
        }

        // Return a MeshGeometry3D representing this mesh's wireframe.
        public static MeshGeometry3D ToWireframe(this MeshGeometry3D mesh, double thickness)
        {
            // Make a mesh to hold the wireframe.
            MeshGeometry3D wireframe = new MeshGeometry3D();

            // Make a HashSet to keep track of edges
            // that have already been added.
            HashSet<Edge> already_added = new HashSet<Edge>();

            // Loop through the mesh's triangles.
            for (int triangle = 0; triangle < mesh.TriangleIndices.Count; triangle += 3)
            {
                // Get the triangle's corner indices.
                int index1 = mesh.TriangleIndices[triangle];
                int index2 = mesh.TriangleIndices[triangle + 1];
                int index3 = mesh.TriangleIndices[triangle + 2];
                Point3D point1 = mesh.Positions[index1];
                Point3D point2 = mesh.Positions[index2];
                Point3D point3 = mesh.Positions[index3];

                // Make the triangle's three segments.
                wireframe.AddEdge(already_added, point1, point2, thickness);
                wireframe.AddEdge(already_added, point2, point3, thickness);
                wireframe.AddEdge(already_added, point3, point1, thickness);
            }

            return wireframe;
        }

        // Add an outline wireframe for a polygon.
        public static void AddPolygonWireframe(this MeshGeometry3D mesh,
            double thickness, params Point3D[] points)
        {
            // Make a HashSet to keep track of edges
            // that have already been added.
            HashSet<Edge> already_added = new HashSet<Edge>();

            // Loop through the polygon's vertices.
            for (int i = 0; i < points.Length - 1; i++)
            {
                mesh.AddEdge(already_added, points[i], points[i + 1], thickness);
            }
            mesh.AddEdge(already_added, points[points.Length - 1], points[0], thickness);
        }

        // Add a polygon's wireframe by using the point names A, B, C, etc.
        // This is used to make dodecahedron wireframes.
        public static void AddPolygonWireframe(this MeshGeometry3D mesh,
            string point_names, double thickness, Point3D[] points)
        {
            Point3D[] polygon_points = new Point3D[point_names.Length];
            for (int i = 0; i < point_names.Length; i++)
            {
                polygon_points[i] = points[ToIndex(point_names[i])];
            }
            AddPolygonWireframe(mesh, thickness, polygon_points);
        }

        // Return a MeshGeometry3D representing this mesh's vertices as boxes.
        public static MeshGeometry3D ToVertexBoxes(this MeshGeometry3D mesh, double thickness)
        {
            // Make a mesh to hold the result.
            MeshGeometry3D boxes = new MeshGeometry3D();

            // Make vectors of the desired lengths.
            Vector3D ux = new Vector3D(thickness, 0, 0);
            Vector3D uy = new Vector3D(0, thickness, 0);
            Vector3D uz = new Vector3D(0, 0, thickness);
            Vector3D ux2 = ux / 2;
            Vector3D uy2 = uy / 2;
            Vector3D uz2 = uz / 2;

            // Loop through the mesh's vertices.
            foreach (Point3D vertex in mesh.Positions)
            {
                // Add a box for this vertex.
                boxes.AddBox(vertex - ux2 - uy2 - uz2, ux, uy, uz);
            }
            return boxes;
        }

        // Add an edge between the two points unless it has already been added.
        private static void AddEdge(this MeshGeometry3D mesh,
            HashSet<Edge> already_added, Point3D point1, Point3D point2, double thickness)
        {
            // Make an Edge.
            Edge edge = new Edge(point1, point2);

            // See if the Edge has already been added.
            if (already_added.Contains(edge)) return;

            // Add the edge.
            already_added.Add(edge);
            mesh.AddSegment(point1, point2, thickness);
        }

        // Add a triangle to the indicated mesh.
        // Do not reuse points so triangles don't share normals.
        // Assumes the points are outwardly oriented.
        public static void AddTriangle(this MeshGeometry3D mesh,
            Point3D point1, Point3D point2, Point3D point3)
        {
            // Create the points.
            int index1 = mesh.Positions.Count;
            mesh.Positions.Add(point1);
            mesh.Positions.Add(point2);
            mesh.Positions.Add(point3);

            // Create the triangle.
            mesh.TriangleIndices.Add(index1++);
            mesh.TriangleIndices.Add(index1++);
            mesh.TriangleIndices.Add(index1);
        }

        // Add a textured triangle to the indicated mesh.
        // Do not reuse points so triangles don't share normals.
        // Assumes the points are outwardly oriented.
        public static void AddTexturedTriangle(this MeshGeometry3D mesh,
            Point3D point1, Point3D point2, Point3D point3,
            Point u1, Point u2, Point u3)
        {
            // Create the points.
            int index1 = mesh.Positions.Count;
            mesh.Positions.Add(point1);
            mesh.Positions.Add(point2);
            mesh.Positions.Add(point3);

            // Create the triangle.
            mesh.TriangleIndices.Add(index1++);
            mesh.TriangleIndices.Add(index1++);
            mesh.TriangleIndices.Add(index1);

            // Set the texture coordinates for the triangle's vertices.
            mesh.TextureCoordinates.Add(u1);
            mesh.TextureCoordinates.Add(u2);
            mesh.TextureCoordinates.Add(u3);
        }

        // Add a triangle to the indicated mesh.
        // Reuse points so triangles share normals.
        // Assumes the points are outwardly oriented.
        public static void AddSmoothTriangle(this MeshGeometry3D mesh,
            Dictionary<Point3D, int> point_dict,
            Point3D point1, Point3D point2, Point3D point3)
        {
            // Find or create the points.
            int index1 = mesh.FindOrCreatePoint(point_dict, point1);
            int index2 = mesh.FindOrCreatePoint(point_dict, point2);
            int index3 = mesh.FindOrCreatePoint(point_dict, point3);

            // Create the triangle.
            mesh.TriangleIndices.Add(index1);
            mesh.TriangleIndices.Add(index2);
            mesh.TriangleIndices.Add(index3);
        }

        // Find or create a point and return its index.
        private static int FindOrCreatePoint(this MeshGeometry3D mesh,
            Dictionary<Point3D, int> point_dict,
            Point3D point)
        {
            // If the point is in the point dictionary,
            // return its saved index.
            if (point_dict.ContainsKey(point))
                return point_dict[point];

            // We didn't find the point. Create it.
            mesh.Positions.Add(point);
            point_dict.Add(point, mesh.Positions.Count - 1);
            return mesh.Positions.Count - 1;
        }

        // Return true if the points are the same.
        private static bool PointEquals(Point3D point1, Point3D point2, double precision)
        {
            Vector3D v = point1 - point2;
            return (v.Length <= precision);
        }

        // Make a thin rectangular prism between the two points.
        // If extend is true, extend the segment by half the
        // thickness so segments with the same end points meet nicely.
        // If up is missing, create a perpendicular vector to use.
        public static void AddSegment(this MeshGeometry3D mesh,
            Point3D point1, Point3D point2, double thickness, bool extend)
        {
            // Find an up vector that is not collinear with the segment.
            // Start with a vector parallel to the Y axis.
            Vector3D up = new Vector3D(0, 1, 0);

            // If the segment and up vector point in more or less the
            // same direction, use an up vector parallel to the X axis.
            Vector3D segment = point2 - point1;
            segment.Normalize();
            if (Math.Abs(Vector3D.DotProduct(up, segment)) > 0.9)
                up = new Vector3D(1, 0, 0);

            // Add the segment.
            AddSegment(mesh, point1, point2, up, thickness, extend);
        }
        public static void AddSegment(this MeshGeometry3D mesh,
            double x1, double y1, double z1,
            double x2, double y2, double z2, double thickness)
        {
            AddSegment(mesh, new Point3D(x1, y1, z1),
                new Point3D(x2, y2, z2), thickness, false);
        }
        public static void AddSegment(this MeshGeometry3D mesh,
            Point3D point1, Point3D point2, double thickness)
        {
            AddSegment(mesh, point1, point2, thickness, false);
        }
        public static void AddSegment(this MeshGeometry3D mesh,
            Point3D point1, Point3D point2, Vector3D up, double thickness)
        {
            AddSegment(mesh, point1, point2, up, thickness, false);
        }
        public static void AddSegment(this MeshGeometry3D mesh,
            Point3D point1, Point3D point2, Vector3D up, double thickness,
            bool extend)
        {
            // Get the segment's vector.
            Vector3D v = point2 - point1;

            if (extend)
            {
                // Increase the segment's length on both ends by thickness / 2.
                Vector3D n = v.Scale(thickness / 2.0);
                point1 -= n;
                point2 += n;
                v += 2 * n;
            }

            // Get the scaled up vector.
            Vector3D n1 = up.Scale(thickness / 2.0);

            // Get another scaled perpendicular vector.
            Vector3D n2 = Vector3D.CrossProduct(v, n1);
            n2 = n2.Scale(thickness / 2.0);

            // Make a skinny box.
            mesh.AddBox(point1 - n1 - n2, v, 2 * n1, 2 * n2);
        }

        // Make a box.
        // The vectors should be oriented so v1 x v2 = v3.
        // For example, for a unit box parallel to the coordinate axes,
        // the corner could be the one with the minimum X, Y, and Z
        // coordinates and the axes could be unit vectors along the
        // coordinate axes.
        public static void AddBox(this MeshGeometry3D mesh,
            Point3D corner, Vector3D v1, Vector3D v2, Vector3D v3)
        {
            AddRectangle(mesh, corner, v2, v1);         // Back
            AddRectangle(mesh, corner + v3, v1, v2);    // Front
            AddRectangle(mesh, corner, v3, v2);         // Left
            AddRectangle(mesh, corner + v1, v2, v3);    // Right
            AddRectangle(mesh, corner + v2, v3, v1);    // Top
            AddRectangle(mesh, corner, v1, v3);         // Bottom
        }

        // Make a box with sides parallel to the coordinate axes
        // centered at the given point with the give side lengths.
        public static void AddBox(this MeshGeometry3D mesh,
            Point3D center, double dx, double dy, double dz)
        {
            mesh.AddBox(
                new Point3D(
                    center.X - dx * 0.5,
                    center.Y - dy * 0.5,
                    center.Z - dz * 0.5),
                new Vector3D(dx, 0, 0),
                new Vector3D(0, dy, 0),
                new Vector3D(0, 0, dz));
        }

        // Make a textured box.
        // The vectors should be oriented so v1 x v2 = v3.
        // For example, for a unit box parallel to the coordinate axes,
        // the corner could be the one with the minimum X, Y, and Z
        // coordinates and the axes could be unit vectors along the
        // coordinate axes.
        public static void AddTexturedBox(this MeshGeometry3D mesh,
            Point3D bottom_back_corner, Vector3D v1, Vector3D v2, Vector3D v3,
            Point back_u, Vector back_u1, Vector back_u2,
            Point front_u, Vector front_u1, Vector front_u2,
            Point left_u, Vector left_u1, Vector left_u2,
            Point right_u, Vector right_u1, Vector right_u2,
            Point top_u, Vector top_u1, Vector top_u2,
            Point bottom_u, Vector bottom_u1, Vector bottom_u2)
        {
            AddTexturedRectangle(mesh,      // Back
                bottom_back_corner + v1 + v2, -v2, -v1,
                back_u, back_u2, back_u1);
            AddTexturedRectangle(mesh,      // Front
                bottom_back_corner + v2 + v3, -v2, v1,
                front_u, front_u2, front_u1);
            AddTexturedRectangle(mesh,      // Left
                bottom_back_corner + v2, -v2, v3,
                left_u, left_u2, left_u1);
            AddTexturedRectangle(mesh,      // Right
                bottom_back_corner + v1 + v2 + v3, -v2, -v3,
                right_u, right_u2, right_u1);
            AddTexturedRectangle(mesh,      // Top
                bottom_back_corner + v2, v3, v1,
                top_u, top_u2, top_u1);
            AddTexturedRectangle(mesh,      // Bottom
                bottom_back_corner + v3, -v3, v1,
                bottom_u, bottom_u2, bottom_u1);
        }

        // Add a rectangle.
        // Vectors v1 and v2 should be outwardly oriented so
        // v1 x v2 points outward.
        public static void AddRectangle(this MeshGeometry3D mesh,
            Point3D corner, Vector3D v1, Vector3D v2)
        {
            AddTriangle(mesh, corner, corner + v1, corner + v1 + v2);
            AddTriangle(mesh, corner, corner + v1 + v2, corner + v2);
        }

        // Add a textured rectangle.
        // Vectors v1 and v2 should be outwardly oriented so
        // v1 x v2 points outward.
        public static void AddTexturedRectangle(this MeshGeometry3D mesh,
            Point3D corner, Vector3D v1, Vector3D v2,
            Point u_corner, Vector u1, Vector u2)
        {
            AddTexturedTriangle(mesh,
                corner, corner + v1, corner + v1 + v2,
                u_corner, u_corner + u1, u_corner + u1 + u2);
            AddTexturedTriangle(mesh,
                corner, corner + v1 + v2, corner + v2,
                u_corner, u_corner + u1 + u2, u_corner + u2);
        }

        // Add X, Y, and Z axes.
        public static void AddAxes(this MeshGeometry3D mesh, double xmax, double thickness)
        {
            mesh.AddAxes(xmax, thickness, true, true, 2, 8);
        }
        public static void AddAxes(this MeshGeometry3D mesh,
            double xmax, double thickness, bool start_at_origin,
            bool show_tick_marks, double tick_thickness, double tick_width)
        {
            double xmin, ymin, zmin;
            if (start_at_origin)
            {
                xmin = 0;
                ymin = 0;
                zmin = 0;
            }
            else
            {
                xmin = -xmax;
                ymin = -xmax;
                zmin = -xmax;
            }
            mesh.AddSegment(xmin, 0, 0, xmax, 0, 0, thickness);
            mesh.AddSegment(0, ymin, 0, 0, xmax, 0, thickness);
            mesh.AddSegment(0, 0, zmin, 0, 0, xmax, thickness);

            if (show_tick_marks)
            {
                Vector3D ux = new Vector3D(thickness, 0, 0);
                Vector3D uy = new Vector3D(0, thickness, 0);
                Vector3D uz = new Vector3D(0, 0, thickness);
                for (int x = (int)xmin; x <= (int)xmax; x++)
                {
                    Point3D corner = new Point3D(
                        x - tick_thickness * 0.5 * thickness,
                        -tick_width * 0.5 * thickness,
                        -tick_width * 0.5 * thickness);
                    mesh.AddBox(corner, tick_thickness * ux, tick_width * uy, tick_width * uz);
                }
                for (int y = (int)ymin; y <= (int)xmax; y++)
                {
                    Point3D corner = new Point3D(
                        -tick_width * 0.5 * thickness,
                        y - tick_thickness * 0.5 * thickness,
                        -tick_width * 0.5 * thickness);
                    mesh.AddBox(corner, tick_width * ux, tick_thickness * uy, tick_width * uz);
                }
                for (int z = (int)zmin; z <= (int)xmax; z++)
                {
                    Point3D corner = new Point3D(
                        -tick_width * 0.5 * thickness,
                        -tick_width * 0.5 * thickness,
                        z - tick_thickness * 0.5 * thickness);
                    mesh.AddBox(corner, tick_width * ux, tick_width * uy, tick_thickness * uz);
                }
            }
        }

        // Add triangles from a List<Triangle>.
        public static void AddTriangles(this MeshGeometry3D mesh,
            List<Triangle> triangles)
        {
            foreach (Triangle triangle in triangles)
            {
                mesh.AddTriangle(
                    triangle.Points[0],
                    triangle.Points[1],
                    triangle.Points[2]);
            }
        }

        // Add smooth triangles from a List<Triangle>.
        public static void AddSmoothTriangles(this MeshGeometry3D mesh,
            Dictionary<Point3D, int> point_dict,
            List<Triangle> triangles)
        {
            foreach (Triangle triangle in triangles)
            {
                mesh.AddSmoothTriangle(point_dict,
                    triangle.Points[0],
                    triangle.Points[1],
                    triangle.Points[2]);
            }
        }

        // Add a polygon to the indicated mesh.
        // Do not reuse old points but reuse these points.
        // Assumes the points are outwardly oriented.
        // Assumes the points are coplanar.
        public static void AddPolygon(this MeshGeometry3D mesh, params Point3D[] points)
        {
            // Create the points.
            int index1 = mesh.Positions.Count;
            foreach (Point3D point in points)
                mesh.Positions.Add(point);

            // Create the triangles.
            for (int i = 1; i < points.Length - 1; i++)
            {
                mesh.TriangleIndices.Add(index1);
                mesh.TriangleIndices.Add(index1 + i);
                mesh.TriangleIndices.Add(index1 + i + 1);
            }
        }

        // Add a polygon by using the point names A, B, C, etc.
        // This is used to make dodecahedrons.
        public static void AddPolygon(this MeshGeometry3D mesh, string point_names, Point3D[] points)
        {
            Point3D[] polygon_points = new Point3D[point_names.Length];
            for (int i = 0; i < point_names.Length; i++)
            {
                polygon_points[i] = points[ToIndex(point_names[i])];
            }
            AddPolygon(mesh, polygon_points);
        }

        // Find a point's index from its letter.
        private static int ToIndex(char ch)
        {
            return ch - 'A';
        }

        // Give the mesh a solid colored material.
        public static GeometryModel3D MakeModel(this MeshGeometry3D mesh, Color color)
        {
            SolidColorBrush brush = new SolidColorBrush(color);
            DiffuseMaterial material = new DiffuseMaterial(brush);
            return new GeometryModel3D(mesh, material);
        }

        // Give the mesh a solid textured material.
        public static GeometryModel3D MakeTexturedModel(this MeshGeometry3D mesh, BitmapImage bitmap)
        {
            ImageBrush texture_brush = new ImageBrush();
            texture_brush.ImageSource = bitmap;
            DiffuseMaterial material = new DiffuseMaterial(texture_brush);
            return new GeometryModel3D(mesh, material);
        }

        // Merge another mesh into this one.
        public static void MergeWith(this MeshGeometry3D mesh, MeshGeometry3D other)
        {
            int offset = mesh.Positions.Count;
            foreach (Point3D point in other.Positions)
                mesh.Positions.Add(point);
            foreach (int index in other.TriangleIndices)
                mesh.TriangleIndices.Add(index + offset);
        }

        // Add an ellipse to the mesh.
        public static void AddEllipse(this MeshGeometry3D mesh, Point3D center,
            Vector3D v1, Vector3D v2, int num_sides)
        {
            double theta = 0;
            double dtheta = 2.0 * Math.PI / num_sides;
            for (int i = 0; i < num_sides; i++)
            {
                Point3D point1 = center +
                    v1 * Math.Cos(theta) +
                    v2 * Math.Sin(theta);
                Point3D point2 = center +
                    v1 * Math.Cos(theta + dtheta) +
                    v2 * Math.Sin(theta + dtheta);
                mesh.AddTriangle(center, point1, point2);

                theta += dtheta;
            }
        }

        // Smooth the entire mesh.
        public static void Smooth(this MeshGeometry3D mesh)
        {
            // Copy the points into a new Point3DCollection.
            // Use a dictionary to prevent duplicates and
            // remember the points' indices.
            Point3DCollection points = new Point3DCollection();
            Dictionary<ClosePoint3D, int> point_dict =
                new Dictionary<ClosePoint3D, int>();

            const int digits = 3;
            foreach (Point3D point in mesh.Positions)
            {
                // See if we've already stored this point.
                int index;
                ClosePoint3D close_point = new ClosePoint3D(digits, point);
                if (point_dict.ContainsKey(close_point))
                    index = point_dict[close_point];
                else
                {
                    // Remember this point's index.
                    point_dict.Add(close_point, points.Count);

                    // Add the point to the Point3DCollection.
                    points.Add(point);
                }
            }

            // Reload the mesh's triangle indices.
            for (int i = 0; i < mesh.TriangleIndices.Count; i++)
            {
                // Get the original point.
                Point3D point = mesh.Positions[mesh.TriangleIndices[i]];

                // Find the stored ClosePoint3D.
                ClosePoint3D close_point = new ClosePoint3D(digits, point);
                int index = point_dict[close_point];

                // Update the triangle index to use the new index.
                mesh.TriangleIndices[i] = index;
            }

            // Make the mesh use the new Point3DCollection.
            mesh.Positions = points;
        }

        // Add a surface to the mesh.
        public static void AddSurface(this MeshGeometry3D mesh,
            Func<double, double, double> F, double xmin, double xmax,
            double zmin, double zmax, double dx, double dz)
        {
            // Make the surface's points and triangles.
            for (double x = xmin; x <= xmax - dx; x += dx)
            {
                for (double z = zmin; z <= zmax - dz; z += dx)
                {
                    // Make points at the corners of the surface
                    // over (x, z) - (x + dx, z + dz).
                    Point3D p00 = new Point3D(x, F(x, z), z);
                    Point3D p10 = new Point3D(x + dx, F(x + dx, z), z);
                    Point3D p01 = new Point3D(x, F(x, z + dz), z + dz);
                    Point3D p11 = new Point3D(x + dx, F(x + dx, z + dz), z + dz);

                    // Add the triangles.
                    AddTriangle(mesh, p00, p01, p11);
                    AddTriangle(mesh, p00, p11, p10);
                }
            }
        }

        // Add a smooth surface to the mesh.
        public static void AddSmoothSurface(this MeshGeometry3D mesh,
            Dictionary<Point3D, int> point_dict,
            Func<double, double, double> F, double xmin, double xmax,
            double zmin, double zmax, double dx, double dz)
        {
            // Make the surface's points and triangles.
            for (double x = xmin; x <= xmax - dx; x += dx)
            {
                for (double z = zmin; z <= zmax - dz; z += dx)
                {
                    // Make points at the corners of the surface
                    // over (x, z) - (x + dx, z + dz).
                    Point3D p00 = new Point3D(x, F(x, z), z);
                    Point3D p10 = new Point3D(x + dx, F(x + dx, z), z);
                    Point3D p01 = new Point3D(x, F(x, z + dz), z + dz);
                    Point3D p11 = new Point3D(x + dx, F(x + dx, z + dz), z + dz);

                    // Add the triangles.
                    mesh.AddSmoothTriangle(point_dict, p00, p01, p11);
                    mesh.AddSmoothTriangle(point_dict, p00, p11, p10);
                }
            }
        }

        // Create a model containing text.
        public static GeometryModel3D MakeTextModel(
            string text, string font_name, double em_size, TextAlignment text_align,
            Brush bg_brush, Pen rect_pen, Brush text_brush,
            Point3D ul_point, Vector3D right, Vector3D down)
        {
            // Make the new mesh.
            MeshGeometry3D mesh = new MeshGeometry3D();

            // Create a rectangle to hold the 3D text.
            AddTriangle(mesh, ul_point, ul_point + down, ul_point + right + down);
            AddTriangle(mesh, ul_point, ul_point + right + down, ul_point + right);

            // Set the texture coordinates.
            mesh.TextureCoordinates.Add(new Point(0, 0));
            mesh.TextureCoordinates.Add(new Point(0, 1));
            mesh.TextureCoordinates.Add(new Point(1, 1));

            mesh.TextureCoordinates.Add(new Point(0, 0));
            mesh.TextureCoordinates.Add(new Point(1, 1));
            mesh.TextureCoordinates.Add(new Point(1, 0));

            // Create a RenderTargetBitmap holding the text.
            RenderTargetBitmap bm =
                DrawingContextExtensions.RenderTextOntoBitmap(
                    text, 96, 96, font_name, em_size, text_align,
                    bg_brush, rect_pen, text_brush, new Thickness(0));

            // Create the material.
            ImageBrush image_brush = new ImageBrush(bm);
            DiffuseMaterial material = new DiffuseMaterial(image_brush);
            GeometryModel3D model = new GeometryModel3D(mesh, material);
            model.BackMaterial = material;

            return model;
        }

        // Add a cage for 3D graphing.
        public static List<GeometryModel3D> AddCage(this MeshGeometry3D mesh,
            double wxmin, double wxmax, double wymin, double wymax, double wzmin, double wzmax,
            double dx, double dy, double dz, double line_thickness,
            string font_name, double em_size, TextAlignment text_alignment,
            Brush text_bg_brush, Pen text_rect_pen, Brush text_fg_brush,
            double text_width, double text_height, double h_offset)
        {
            // Make the list of text models.
            List<GeometryModel3D> text_models = new List<GeometryModel3D>();

            // Bottom.
            for (double x = wxmin; x <= wxmax + dx / 2; x += dx)
            {
                mesh.AddSegment(
                    new Point3D(x, wymin, wzmin),
                    new Point3D(x, wymin, wzmax),
                    line_thickness);

                // Label this X value.
                GeometryModel3D new_model = MakeTextModel(
                    x.ToString("0.00"), font_name, em_size, text_alignment,
                    text_bg_brush, text_rect_pen, text_fg_brush,
                    new Point3D(x - text_height / 2, wymin, wzmax + text_width + h_offset),
                    new Vector3D(0, 0, -text_width),
                    new Vector3D(text_height, 0, 0));
                text_models.Add(new_model);
            }
            for (double z = wzmin; z <= wzmax + dz / 2; z += dz)
            {
                mesh.AddSegment(
                    new Point3D(wxmin, wymin, z),
                    new Point3D(wxmax, wymin, z),
                    line_thickness);

                // Label this Z value.
                GeometryModel3D new_model = MakeTextModel(
                    z.ToString("0.00"), font_name, em_size, text_alignment,
                    text_bg_brush, text_rect_pen, text_fg_brush,
                    new Point3D(wxmax + h_offset, wymin, z - text_height / 2),
                    new Vector3D(text_width, 0, 0),
                    new Vector3D(0, 0, text_height));
                text_models.Add(new_model);
            }

            // Left.
            for (double y = wymin; y <= wymax + dy / 2; y += dy)
            {
                mesh.AddSegment(
                    new Point3D(wxmin, y, wzmin),
                    new Point3D(wxmin, y, wzmax),
                    line_thickness);

                // Label this Y value.
                if (y > wymin)
                {
                    GeometryModel3D new_model = MakeTextModel(
                        y.ToString("0.00"), font_name, em_size, text_alignment,
                        text_bg_brush, text_rect_pen, text_fg_brush,
                        new Point3D(wxmin, y + text_height / 2, wzmax + text_width + h_offset),
                        new Vector3D(0, 0, -text_width),
                        new Vector3D(0, -text_height, 0));
                    text_models.Add(new_model);
                }
            }
            for (double z = wzmin; z <= wzmax + dz / 2; z += dz)
                mesh.AddSegment(
                    new Point3D(wxmin, wymin, z),
                    new Point3D(wxmin, wymax, z),
                    line_thickness);

            // Right.
            for (double x = wxmin; x <= wxmax + dx / 2; x += dx)
                mesh.AddSegment(
                    new Point3D(x, wymin, wzmin),
                    new Point3D(x, wymax, wzmin),
                    line_thickness);
            for (double y = wymin; y <= wymax + dy / 2; y += dy)
                mesh.AddSegment(
                    new Point3D(wxmin, y, wzmin),
                    new Point3D(wxmax, y, wzmin),
                    line_thickness);

            return text_models;
        }
    }
}
