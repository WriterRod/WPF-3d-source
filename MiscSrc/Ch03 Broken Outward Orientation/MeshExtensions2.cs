using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Windows.Media.Media3D;

namespace Interlocked
{
    public static partial class MeshExtensions
    {
        // Add a box representing a wall to the mesh.
        public static void AddWall(this MeshGeometry3D mesh,
            double length, double thickness, params Point3D[] points)
        {
            //            mesh.AddBox()
        }

        // Make a new mesh that is a reflection of this one across the X-Y plane.
        // Reverse the order of triangles to maintain outward orientation.
        public static MeshGeometry3D ReflectZ(this MeshGeometry3D mesh)
        {
            MeshGeometry3D new_mesh = new MeshGeometry3D();

            // Add the positions.
            int num_points = mesh.Positions.Count;
            for (int i = 0; i < num_points; i++)
            {
                new_mesh.Positions.Add(new Point3D(
                    mesh.Positions[i].X,
                    mesh.Positions[i].Y,
                    -mesh.Positions[i].Z));
            }

            // Add the triangles.
            int num_indices = mesh.TriangleIndices.Count;
            for (int i = 0; i < num_indices; i += 3)
            {
                new_mesh.TriangleIndices.Add(mesh.TriangleIndices[i + 2]);
                new_mesh.TriangleIndices.Add(mesh.TriangleIndices[i + 1]);
                new_mesh.TriangleIndices.Add(mesh.TriangleIndices[i + 0]);
            }

            return new_mesh;
        }

        // Add a cylinder.
        public static void AddCylinder(this MeshGeometry3D mesh, Point3D end_point, Vector3D axis, double radius, int num_sides)
        {
            // Get two vectors perpendicular to the axis.
            Vector3D v1;
            if ((axis.Z < -0.01) || (axis.Z > 0.01))
                v1 = new Vector3D(axis.Z, axis.Z, -axis.X - axis.Y);
            else
                v1 = new Vector3D(-axis.Y - axis.Z, axis.X, axis.X);
            Vector3D v2 = Vector3D.CrossProduct(v1, axis);

            // Make the vectors have length radius.
            v1 *= (radius / v1.Length);
            v2 *= (radius / v2.Length);

            // Make the top end cap.
            double theta = 0;
            double dtheta = 2 * Math.PI / num_sides;
            for (int i = 0; i < num_sides; i++)
            {
                Point3D p1 = end_point +
                    Math.Cos(theta) * v1 +
                    Math.Sin(theta) * v2;
                theta += dtheta;
                Point3D p2 = end_point +
                    Math.Cos(theta) * v1 +
                    Math.Sin(theta) * v2;
                mesh.AddTriangle(end_point, p1, p2);
            }

            // Make the bottom end cap.
            Point3D end_point2 = end_point + axis;
            theta = 0;
            for (int i = 0; i < num_sides; i++)
            {
                Point3D p1 = end_point2 +
                    Math.Cos(theta) * v1 +
                    Math.Sin(theta) * v2;
                theta += dtheta;
                Point3D p2 = end_point2 +
                    Math.Cos(theta) * v1 +
                    Math.Sin(theta) * v2;
                mesh.AddTriangle(end_point2, p2, p1);
            }

            // Make the sides.
            theta = 0;
            for (int i = 0; i < num_sides; i++)
            {
                Point3D p1 = end_point +
                    Math.Cos(theta) * v1 +
                    Math.Sin(theta) * v2;
                theta += dtheta;
                Point3D p2 = end_point +
                    Math.Cos(theta) * v1 +
                    Math.Sin(theta) * v2;

                Point3D p3 = p1 + axis;
                Point3D p4 = p2 + axis;

                mesh.AddTriangle(p1, p3, p2);
                mesh.AddTriangle(p2, p3, p4);
            }
        }

        // Add a cylinder.
        public static void AddSmoothCylinder(this MeshGeometry3D mesh,
            Dictionary<Point3D, int> point_dict,
            Point3D end_point, Vector3D axis, double radius, int num_sides)
        {
            // Get two vectors perpendicular to the axis.
            Vector3D v1;
            if ((axis.Z < -0.01) || (axis.Z > 0.01))
                v1 = new Vector3D(axis.Z, axis.Z, -axis.X - axis.Y);
            else
                v1 = new Vector3D(-axis.Y - axis.Z, axis.X, axis.X);
            Vector3D v2 = Vector3D.CrossProduct(v1, axis);

            // Make the vectors have length radius.
            v1 *= (radius / v1.Length);
            v2 *= (radius / v2.Length);

            // Make the top end cap.
            double theta = 0;
            double dtheta = 2 * Math.PI / num_sides;
            for (int i = 0; i < num_sides; i++)
            {
                Point3D p1 = end_point +
                    Math.Cos(theta) * v1 +
                    Math.Sin(theta) * v2;
                theta += dtheta;
                Point3D p2 = end_point +
                    Math.Cos(theta) * v1 +
                    Math.Sin(theta) * v2;
                mesh.AddSmoothTriangle(point_dict, end_point, p1, p2);
            }

            // Make the bottom end cap.
            Point3D end_point2 = end_point + axis;
            theta = 0;
            for (int i = 0; i < num_sides; i++)
            {
                Point3D p1 = end_point2 +
                    Math.Cos(theta) * v1 +
                    Math.Sin(theta) * v2;
                theta += dtheta;
                Point3D p2 = end_point2 +
                    Math.Cos(theta) * v1 +
                    Math.Sin(theta) * v2;
                mesh.AddSmoothTriangle(point_dict, end_point2, p2, p1);
            }

            // Make the sides.
            theta = 0;
            for (int i = 0; i < num_sides; i++)
            {
                Point3D p1 = end_point +
                    Math.Cos(theta) * v1 +
                    Math.Sin(theta) * v2;
                theta += dtheta;
                Point3D p2 = end_point +
                    Math.Cos(theta) * v1 +
                    Math.Sin(theta) * v2;

                Point3D p3 = p1 + axis;
                Point3D p4 = p2 + axis;

                mesh.AddSmoothTriangle(point_dict, p1, p3, p2);
                mesh.AddSmoothTriangle(point_dict, p2, p3, p4);
            }
        }

        // Add a cone.
        public static void AddCone(this MeshGeometry3D mesh, Point3D end_point,
            Vector3D axis, double radius1, double radius2, int num_sides)
        {
            // Get two vectors perpendicular to the axis.
            Vector3D top_v1;
            if ((axis.Z < -0.01) || (axis.Z > 0.01))
                top_v1 = new Vector3D(axis.Z, axis.Z, -axis.X - axis.Y);
            else
                top_v1 = new Vector3D(-axis.Y - axis.Z, axis.X, axis.X);
            Vector3D top_v2 = Vector3D.CrossProduct(top_v1, axis);

            Vector3D bot_v1 = top_v1;
            Vector3D bot_v2 = top_v2;

            // Make the vectors have length radius.
            top_v1 *= (radius1 / top_v1.Length);
            top_v2 *= (radius1 / top_v2.Length);

            bot_v1 *= (radius2 / bot_v1.Length);
            bot_v2 *= (radius2 / bot_v2.Length);

            // Make the top end cap.
            double theta = 0;
            double dtheta = 2 * Math.PI / num_sides;
            for (int i = 0; i < num_sides; i++)
            {
                Point3D p1 = end_point +
                    Math.Cos(theta) * top_v1 +
                    Math.Sin(theta) * top_v2;
                theta += dtheta;
                Point3D p2 = end_point +
                    Math.Cos(theta) * top_v1 +
                    Math.Sin(theta) * top_v2;
                mesh.AddTriangle(end_point, p1, p2);
            }

            // Make the bottom end cap.
            Point3D end_point2 = end_point + axis;
            theta = 0;
            for (int i = 0; i < num_sides; i++)
            {
                Point3D p1 = end_point2 +
                    Math.Cos(theta) * bot_v1 +
                    Math.Sin(theta) * bot_v2;
                theta += dtheta;
                Point3D p2 = end_point2 +
                    Math.Cos(theta) * bot_v1 +
                    Math.Sin(theta) * bot_v2;
                mesh.AddTriangle(end_point2, p2, p1);
            }

            // Make the sides.
            theta = 0;
            for (int i = 0; i < num_sides; i++)
            {
                Point3D p1 = end_point +
                    Math.Cos(theta) * top_v1 +
                    Math.Sin(theta) * top_v2;
                Point3D p3 = end_point + axis +
                    Math.Cos(theta) * bot_v1 +
                    Math.Sin(theta) * bot_v2;
                theta += dtheta;
                Point3D p2 = end_point +
                    Math.Cos(theta) * top_v1 +
                    Math.Sin(theta) * top_v2;
                Point3D p4 = end_point + axis +
                    Math.Cos(theta) * bot_v1 +
                    Math.Sin(theta) * bot_v2;

                mesh.AddTriangle(p1, p3, p2);
                mesh.AddTriangle(p2, p3, p4);
            }
        }

        // Add a cone with smooth sides.
        public static void AddSmoothCone(this MeshGeometry3D mesh, Point3D end_point,
            Vector3D axis, double radius1, double radius2, int num_sides)
        {
            // Get two vectors perpendicular to the axis.
            Vector3D top_v1;
            if ((axis.Z < -0.01) || (axis.Z > 0.01))
                top_v1 = new Vector3D(axis.Z, axis.Z, -axis.X - axis.Y);
            else
                top_v1 = new Vector3D(-axis.Y - axis.Z, axis.X, axis.X);
            Vector3D top_v2 = Vector3D.CrossProduct(top_v1, axis);

            Vector3D bot_v1 = top_v1;
            Vector3D bot_v2 = top_v2;

            // Make the top vectors have length radius1.
            top_v1 *= (radius1 / top_v1.Length);
            top_v2 *= (radius1 / top_v2.Length);

            // Make the bottom vectors have length radius2.
            bot_v1 *= (radius2 / bot_v1.Length);
            bot_v2 *= (radius2 / bot_v2.Length);

            // Make the top end cap.
            // Make the end point.
            int pt0 = mesh.Positions.Count;     // Index of end_point.
            mesh.Positions.Add(end_point);

            // Make the top points.
            double theta = 0;
            double dtheta = 2 * Math.PI / num_sides;
            for (int i = 0; i < num_sides; i++)
            {
                mesh.Positions.Add(end_point +
                    Math.Cos(theta) * top_v1 +
                    Math.Sin(theta) * top_v2);
                theta += dtheta;
            }

            // Make the top triangles.
            int pt1 = mesh.Positions.Count - 1; // Index of last point.
            int pt2 = pt0 + 1;                  // Index of first point in this cap.
            for (int i = 0; i < num_sides; i++)
            {
                mesh.TriangleIndices.Add(pt0);
                mesh.TriangleIndices.Add(pt1);
                mesh.TriangleIndices.Add(pt2);
                pt1 = pt2++;
            }

            // Make the bottom end cap.
            // Make the end point.
            pt0 = mesh.Positions.Count;     // Index of end_point2.
            Point3D end_point2 = end_point + axis;
            mesh.Positions.Add(end_point2);

            // Make the bottom points.
            theta = 0;
            for (int i = 0; i < num_sides; i++)
            {
                mesh.Positions.Add(end_point2 +
                    Math.Cos(theta) * bot_v1 +
                    Math.Sin(theta) * bot_v2);
                theta += dtheta;
            }

            // Make the bottom triangles.
            theta = 0;
            pt1 = mesh.Positions.Count - 1; // Index of last point.
            pt2 = pt0 + 1;                  // Index of first point in this cap.
            for (int i = 0; i < num_sides; i++)
            {
                mesh.TriangleIndices.Add(pt0);
                mesh.TriangleIndices.Add(pt2);
                mesh.TriangleIndices.Add(pt1);
                pt1 = pt2++;
            }

            // Make the sides.
            // Add the points to the mesh.
            int first_side_point = mesh.Positions.Count;
            theta = 0;
            for (int i = 0; i < num_sides; i++)
            {
                Point3D p1 = end_point +
                    Math.Cos(theta) * top_v1 +
                    Math.Sin(theta) * top_v2;
                mesh.Positions.Add(p1);
                Point3D p2 = end_point + axis +
                    Math.Cos(theta) * bot_v1 +
                    Math.Sin(theta) * bot_v2;
                mesh.Positions.Add(p2);
                theta += dtheta;
            }

            // Make the side triangles.
            pt1 = mesh.Positions.Count - 2;
            pt2 = pt1 + 1;
            int pt3 = first_side_point;
            int pt4 = pt3 + 1;
            for (int i = 0; i < num_sides; i++)
            {
                mesh.TriangleIndices.Add(pt1);
                mesh.TriangleIndices.Add(pt2);
                mesh.TriangleIndices.Add(pt4);

                mesh.TriangleIndices.Add(pt1);
                mesh.TriangleIndices.Add(pt4);
                mesh.TriangleIndices.Add(pt3);

                pt1 = pt3;
                pt3 += 2;
                pt2 = pt4;
                pt4 += 2;
            }
        }

        // Add a cone with smoother sides.
        public static void AddSmootherCone(this MeshGeometry3D mesh, Point3D end_point,
            Vector3D axis, double radius1, double radius2, int num_sides,
            int num_slices)
        {
            // Get two vectors perpendicular to the axis.
            Vector3D top_v1;
            if ((axis.Z < -0.01) || (axis.Z > 0.01))
                top_v1 = new Vector3D(axis.Z, axis.Z, -axis.X - axis.Y);
            else
                top_v1 = new Vector3D(-axis.Y - axis.Z, axis.X, axis.X);
            Vector3D top_v2 = Vector3D.CrossProduct(top_v1, axis);

            Vector3D bot_v1 = top_v1;
            Vector3D bot_v2 = top_v2;

            // Make the top vectors have length radius1.
            top_v1 *= (radius1 / top_v1.Length);
            top_v2 *= (radius1 / top_v2.Length);

            // Make the bottom vectors have length radius2.
            bot_v1 *= (radius2 / bot_v1.Length);
            bot_v2 *= (radius2 / bot_v2.Length);

            // Make the top end cap.
            // Make the end point.
            int pt0 = mesh.Positions.Count;     // Index of end_point.
            mesh.Positions.Add(end_point);

            // Make the top points.
            double theta = 0;
            double dtheta = 2 * Math.PI / num_sides;
            for (int i = 0; i < num_sides; i++)
            {
                mesh.Positions.Add(end_point +
                    Math.Cos(theta) * top_v1 +
                    Math.Sin(theta) * top_v2);
                theta += dtheta;
            }

            // Make the top triangles.
            int pt1 = mesh.Positions.Count - 1; // Index of last point.
            int pt2 = pt0 + 1;                  // Index of first point in this cap.
            for (int i = 0; i < num_sides; i++)
            {
                mesh.TriangleIndices.Add(pt0);
                mesh.TriangleIndices.Add(pt1);
                mesh.TriangleIndices.Add(pt2);
                pt1 = pt2++;
            }

            // Make the bottom end cap.
            // Make the end point.
            pt0 = mesh.Positions.Count;     // Index of end_point2.
            Point3D end_point2 = end_point + axis;
            mesh.Positions.Add(end_point2);

            // Make the bottom points.
            theta = 0;
            for (int i = 0; i < num_sides; i++)
            {
                mesh.Positions.Add(end_point2 +
                    Math.Cos(theta) * bot_v1 +
                    Math.Sin(theta) * bot_v2);
                theta += dtheta;
            }

            // Make the bottom triangles.
            theta = 0;
            pt1 = mesh.Positions.Count - 1; // Index of last point.
            pt2 = pt0 + 1;                  // Index of first point in this cap.
            for (int i = 0; i < num_sides; i++)
            {
                mesh.TriangleIndices.Add(pt0);
                mesh.TriangleIndices.Add(pt2);
                mesh.TriangleIndices.Add(pt1);
                pt1 = pt2++;
            }

            // Make the points for the sides.
            Vector3D slice_axis = axis / num_slices;
            double dradius = (radius2 - radius1) / num_slices;
            int first_side_point = mesh.Positions.Count;
            for (int slice = 0; slice <= num_slices; slice++)
            {
                // Scale the perpendicular axes.
                double rad1 = radius1 + slice * dradius;
                double rad2 = radius1 + (slice + 1) * dradius;

                // Make the vectors have length rad1.
                Vector3D side_v1 = top_v1 * (rad1 / top_v1.Length);
                Vector3D side_v2 = top_v2 * (rad1 / top_v2.Length);

                // Add the points to the mesh.
                theta = 0;
                for (int i = 0; i < num_sides; i++)
                {
                    Point3D p1 = end_point +
                        Math.Cos(theta) * side_v1 +
                        Math.Sin(theta) * side_v2;
                    mesh.Positions.Add(p1);
                    theta += dtheta;
                }

                // Move to the next end point, which is
                // in the middle of the slice's bottom edge.
                end_point += slice_axis;
            }

            // Make the side triangles.
            pt1 = first_side_point;
            for (int slice = 0; slice < num_slices; slice++)
            {
                for (int side = 0; side < num_sides; side++)
                {
                    if (side == num_sides - 1)
                        pt2 = pt1 - num_sides + 1;
                    else
                        pt2 = pt1 + 1;
                    int pt3 = pt1 + num_sides;
                    int pt4 = pt2 + num_sides;

                    mesh.TriangleIndices.Add(pt1);
                    mesh.TriangleIndices.Add(pt3);
                    mesh.TriangleIndices.Add(pt2);

                    mesh.TriangleIndices.Add(pt2);
                    mesh.TriangleIndices.Add(pt3);
                    mesh.TriangleIndices.Add(pt4);

                    // Move to the next set of points.
                    pt1++;
                }
            }
        }

        // Add a cage.
        private static void AddCage(MeshGeometry3D mesh)
        {
            // Top.
            Vector3D up = new Vector3D(0, 1, 0);
            mesh.AddSegment(new Point3D(1, 1, 1), new Point3D(1, 1, -1), up, 0.05, true);
            mesh.AddSegment(new Point3D(1, 1, -1), new Point3D(-1, 1, -1), up, 0.05, true);
            mesh.AddSegment(new Point3D(-1, 1, -1), new Point3D(-1, 1, 1), up, 0.05, true);
            mesh.AddSegment(new Point3D(-1, 1, 1), new Point3D(1, 1, 1), up, 0.05, true);

            // Bottom.
            mesh.AddSegment(new Point3D(1, -1, 1), new Point3D(1, -1, -1), up, 0.05, true);
            mesh.AddSegment(new Point3D(1, -1, -1), new Point3D(-1, -1, -1), up, 0.05, true);
            mesh.AddSegment(new Point3D(-1, -1, -1), new Point3D(-1, -1, 1), up, 0.05, true);
            mesh.AddSegment(new Point3D(-1, -1, 1), new Point3D(1, -1, 1), up, 0.05, true);

            // Sides.
            Vector3D right = new Vector3D(1, 0, 0);
            mesh.AddSegment(new Point3D(1, -1, 1), new Point3D(1, 1, 1), right, 0.05, true);
            mesh.AddSegment(new Point3D(1, -1, -1), new Point3D(1, 1, -1), right, 0.05, true);
            mesh.AddSegment(new Point3D(-1, -1, 1), new Point3D(-1, 1, 1), right, 0.05, true);
            mesh.AddSegment(new Point3D(-1, -1, -1), new Point3D(-1, 1, -1), right, 0.05, true);
        }

        // Convert the Polygons into stellate triangles and add them to the mesh.
        public static void MakeStellate(this MeshGeometry3D mesh,
            List<Polygon> polygons, Point3D center, double radius)
        {
            foreach (Polygon polygon in polygons)
            {
                mesh.MakeStellate(polygon, center, radius);
            }
        }

        // Make triangles to make this triangle stellate.
        public static void MakeStellate(this MeshGeometry3D mesh,
            Polygon polygon, Point3D center, double radius)
        {
            // Find the point in the middle of the polygon.
            Point3D peak = new Point3D();
            foreach (Point3D point in polygon.Points)
            {
                peak.X += point.X;
                peak.Y += point.Y;
                peak.Z += point.Z;
            }
            int num_points = polygon.Points.Length;
            peak.X /= num_points;
            peak.Y /= num_points;
            peak.Z /= num_points;

            // Move the peak the desired distance from the center.
            NormalizePoint(ref peak, center, radius);

            // Make the new triangles.
            for (int i = 0; i < polygon.Points.Length; i++)
            {
                int i1 = (i + 1) % num_points;
                mesh.AddTriangle(peak, polygon.Points[i], polygon.Points[i1]);
            }
        }

        // Move the point to the indicated distance away from the center.
        private static void NormalizePoint(ref Point3D point, Point3D center, double radius)
        {
            Vector3D vector = point - center;
            point = center + vector.Scale(radius);
        }
    }
}