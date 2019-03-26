using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Windows.Media.Media3D;

namespace Interlocked
{
    public static class ThreeDTools
    {
        // A two-dimensional point.
        private class Point2D
        {
            public double X, Y;
            public Point2D(double x, double y)
            {
                X = x;
                Y = y;
            }
            public Point3D ToPoint3D(double z)
            {
                return new Point3D(X, Y, z);
            }
            public override string ToString()
            {
                return "(" +
                    X.ToString() + ", " +
                    Y.ToString() + ")";
            }
        };

        // A rectangle point.
        private class Rect2D
        {
            public double X, Y, Width, Height;
            public Rect2D(double x, double y, double width, double height)
            {
                X = x;
                Y = y;
                Width = width;
                Height = height;
            }
        };

        #region MakeXmasTree

        // Make an Xmas tree.
        public static MeshGeometry3D MakeXmasTree(Transform3D transform)
        {
            MeshGeometry3D mesh = new MeshGeometry3D();

            // Make the bottom.
            Point3D[] points =
            {
                new Point3D(1, 0, 0),
                new Point3D(1.2, 0, 0),
                new Point3D(1, 0.5, 0)
            };
            int num_points = points.Length;
            mesh.AddPolygon(points);

            // Make a temporary array of points.
            Point3D[] new_points = new Point3D[num_points];

            // Make a transformation that rotates around the
            // origin, scales smaller, and translates vertically.
            const int num_laps = 6;
            const int frames_per_lap = 20;

            Transform3DGroup group = new Transform3DGroup();

            const double theta = 360.0 / frames_per_lap;
            Vector3D axis = new Vector3D(0, 1, 0);
            Rotation3D rotation = new AxisAngleRotation3D(axis, theta);
            Point3D center = new Point3D(0, 0, 0);
            group.Children.Add(new RotateTransform3D(rotation, center));

            const double scale = 0.98;
            group.Children.Add(new ScaleTransform3D(scale, 1, scale));

            const double vert = 2.0 / frames_per_lap / num_laps;
            group.Children.Add(new TranslateTransform3D(0, vert, 0));

            // Make a transformation that rotates around the
            // origin, scales smaller, and translates vertically.
            for (int lap = 0; lap < num_laps; lap++)
            {
                for (int frame = 0; frame < frames_per_lap; frame++)
                {
                    // Transform the points.
                    Array.Copy(points, new_points, num_points);
                    group.Transform(new_points);

                    // Make the new triangles.
                    Dictionary<Point3D, int> point_dict = new Dictionary<Point3D, int>();
                    for (int i = 0; i < num_points; i++)
                    {
                        int i1 = (i + 1) % num_points;
                        mesh.AddSmoothTriangle(point_dict, points[i], new_points[i], new_points[i1]);
                        mesh.AddSmoothTriangle(point_dict, points[i], new_points[i1], points[i1]);
                    }

                    // Update the points.
                    Point3D[] temp = points;
                    points = new_points;
                    new_points = temp;
                }
            }

            // Transform the mesh's points if desired.
            if (transform != null)
                transform.Transform(mesh);

            return mesh;
        }

        #endregion MakeXmasTree

        #region MakeStar

        // Make a star.
        public static MeshGeometry3D MakeStar(double radius, double thickness,
            Transform3D transform)
        {
            MeshGeometry3D mesh = new MeshGeometry3D();

            // Get the points for a 2-D star.
            const int num_tips = 5;
            const int skip = 2;
            Rect2D rect = new Rect2D(-0.25, -0.25, 0.5, 0.5);
            Point2D[] star_points =
                MakeStarPoints(Math.PI / 2.0, num_tips, skip, rect);

            // Convert to 3D points.
            int num_points = star_points.Length;
            double z = thickness / 2.0;

            // Make the star's tips.
            for (int i0 = 1; i0 < num_points; i0 += skip)
            {
                int i1 = (i0 + skip / 2) % num_points;
                int i2 = (i0 + skip) % num_points;

                // Front.
                mesh.AddTriangle(
                    star_points[i0].ToPoint3D(z),
                    star_points[i2].ToPoint3D(z),
                    star_points[i1].ToPoint3D(0));

                // Back.
                mesh.AddTriangle(
                    star_points[i0].ToPoint3D(-z),
                    star_points[i1].ToPoint3D(0),
                    star_points[i2].ToPoint3D(-z));

                // Sides.
                mesh.AddTriangle(
                    star_points[i0].ToPoint3D(z),
                    star_points[i1].ToPoint3D(0),
                    star_points[i0].ToPoint3D(-z));
                mesh.AddTriangle(
                    star_points[i2].ToPoint3D(z),
                    star_points[i2].ToPoint3D(-z),
                    star_points[i1].ToPoint3D(0));
            }

            // Make the back face.
            List<Point3D> face_points = new List<Point3D>();
            for (int i0 = 1; i0 < num_points; i0 += skip)
                face_points.Add(star_points[i0].ToPoint3D(-z));
            mesh.AddPolygon(face_points.ToArray());

            // Make the front face.
            Point3D[] front_points = face_points.ToArray();
            Array.Reverse(front_points);
            for (int i = 0; i < front_points.Length; i++)
                front_points[i].Z = z;
            mesh.AddPolygon(front_points);

            // Transform the mesh's points if desired.
            if (transform != null)
                transform.Transform(mesh);

            return mesh;
        }

        // Generate the points for a star.
        private static Point2D[] MakeStarPoints(double start_theta,
            int num_points, int skip, Rect2D rect)
        {
            double theta, dtheta;
            Point2D[] result;
            double cx = rect.X + rect.Width / 2.0;
            double cy = rect.Y + rect.Height / 2.0;
            double rx = rect.Width / 2.0;
            double ry = rect.Height / 2.0;

            // If this is a polygon, don't bother with concave points.
            if (skip == 1)
            {
                result = new Point2D[num_points];
                theta = start_theta;
                dtheta = 2 * Math.PI / num_points;
                for (int i = 0; i < num_points; i++)
                {
                    result[i] = new Point2D(
                        (float)(cx + rx * Math.Cos(theta)),
                        (float)(cy + ry * Math.Sin(theta)));
                    theta += dtheta;
                }
                return result;
            }

            // Find the radius for the concave vertices.
            double concave_radius =
                CalculateConcaveRadius(num_points, skip);

            // Make the points.
            result = new Point2D[2 * num_points];
            theta = start_theta;
            dtheta = -Math.PI / num_points;
            for (int i = 0; i < num_points; i++)
            {
                result[2 * i] = new Point2D(
                    (float)(cx + rx * Math.Cos(theta)),
                    (float)(cy + ry * Math.Sin(theta)));
                theta += dtheta;
                result[2 * i + 1] = new Point2D(
                    (float)(cx + rx * Math.Cos(theta) * concave_radius),
                    (float)(cy + ry * Math.Sin(theta) * concave_radius));
                theta += dtheta;
            }
            return result;
        }

        // Calculate the inner star radius.
        private static double CalculateConcaveRadius(int num_tips, int skip)
        {
            // For really small numbers of points.
            if (num_tips < 5) return 0.33;

            // Calculate angles to key points.
            double dtheta = 2 * Math.PI / num_tips;
            double theta00 = -Math.PI / 2;
            double theta01 = theta00 + dtheta * skip;
            double theta10 = theta00 + dtheta;
            double theta11 = theta10 - dtheta * skip;

            // Find the key points.
            Point2D pt00 = new Point2D(
                (float)Math.Cos(theta00),
                (float)Math.Sin(theta00));
            Point2D pt01 = new Point2D(
                (float)Math.Cos(theta01),
                (float)Math.Sin(theta01));
            Point2D pt10 = new Point2D(
                (float)Math.Cos(theta10),
                (float)Math.Sin(theta10));
            Point2D pt11 = new Point2D(
                (float)Math.Cos(theta11),
                (float)Math.Sin(theta11));

            // See where the segments connecting the points intersect.
            bool lines_intersect, segments_intersect;
            Point2D intersection, close_p1, close_p2;
            FindIntersection(pt00, pt01, pt10, pt11,
                out lines_intersect, out segments_intersect,
                out intersection, out close_p1, out close_p2);

            // Calculate the distance between the
            // point of intersection and the center.
            return Math.Sqrt(
                intersection.X * intersection.X +
                intersection.Y * intersection.Y);
        }

        // Find the point of intersection between
        // the lines p1 --> p2 and p3 --> p4.
        private static void FindIntersection(
            Point2D p1, Point2D p2, Point2D p3, Point2D p4,
            out bool lines_intersect, out bool segments_intersect,
            out Point2D intersection,
            out Point2D close_p1, out Point2D close_p2)
        {
            // Get the segments' parameters.
            double dx12 = p2.X - p1.X;
            double dy12 = p2.Y - p1.Y;
            double dx34 = p4.X - p3.X;
            double dy34 = p4.Y - p3.Y;

            // Solve for t1 and t2
            double denominator = (dy12 * dx34 - dx12 * dy34);

            double t1 =
                ((p1.X - p3.X) * dy34 + (p3.Y - p1.Y) * dx34)
                    / denominator;
            if (double.IsInfinity(t1))
            {
                // The lines are parallel (or close enough to it).
                lines_intersect = false;
                segments_intersect = false;
                intersection = new Point2D(double.NaN, double.NaN);
                close_p1 = new Point2D(double.NaN, double.NaN);
                close_p2 = new Point2D(double.NaN, double.NaN);
                return;
            }
            lines_intersect = true;

            double t2 =
                ((p3.X - p1.X) * dy12 + (p1.Y - p3.Y) * dx12)
                    / -denominator;

            // Find the point of intersection.
            intersection = new Point2D(p1.X + dx12 * t1, p1.Y + dy12 * t1);

            // The segments intersect if t1 and t2 are between 0 and 1.
            segments_intersect =
                ((t1 >= 0) && (t1 <= 1) &&
                 (t2 >= 0) && (t2 <= 1));

            // Find the closest points on the segments.
            if (t1 < 0)
            {
                t1 = 0;
            }
            else if (t1 > 1)
            {
                t1 = 1;
            }

            if (t2 < 0)
            {
                t2 = 0;
            }
            else if (t2 > 1)
            {
                t2 = 1;
            }

            close_p1 = new Point2D(p1.X + dx12 * t1, p1.Y + dy12 * t1);
            close_p2 = new Point2D(p3.X + dx34 * t2, p3.Y + dy34 * t2);
        }

        #endregion MakeStar

        #region MakeCylinder

        // Make a right circular cylinder.
        public static MeshGeometry3D MakeCylinder(Point3D base_point,
            double radius, int num_sides, Vector3D axis, Transform3D transform)
        {
            // Normalize the axis.
            Vector3D v0 = axis / axis.Length;

            // Find two normal vectors.
            Vector3D n0 = new Vector3D(0, 1, 0);
            if (Vector3D.DotProduct(v0, n0) > 0.999)
                n0 = new Vector3D(1, 0, 0);
            Vector3D n1 = Vector3D.CrossProduct(v0, n0);
            n0 = Vector3D.CrossProduct(v0, n1);

            // Define the circular base.
            double theta = 0;
            double dtheta = 2.0 * Math.PI / num_sides;
            List<Point3D> points = new List<Point3D>();
            for (int i = 0; i < num_sides; i++)
            {
                Point3D point = base_point +
                    n0 * radius * Math.Cos(theta) +
                    n1 * radius * Math.Sin(theta);
                points.Add(point);
                theta += dtheta;
            }

            // Create the cylinder.
            return MakeCylinder(points.ToArray(), axis, transform);
        }

        // Make a cylinder with the indicated base.
        // Assumes base_points forms an outwardly oriented convex polygon.
        public static MeshGeometry3D MakeCylinder(Point3D[] base_points,
            Vector3D axis, Transform3D transform)
        {
            MeshGeometry3D mesh = new MeshGeometry3D();

            // Make the bottom.
            mesh.AddPolygon(base_points);

            // Make the top.
            int num_points = base_points.Length;
            Point3D[] top_points = new Point3D[num_points];
            for (int i = 0; i < num_points; i++)
                top_points[i] = base_points[num_points - 1 - i] + axis;
            mesh.AddPolygon(top_points);

            // Make the sides.
            for (int i0 = 0; i0 < num_points; i0++)
            {
                int i1 = (i0 + 1) % num_points;
                mesh.AddTriangle(
                    base_points[i0],
                    base_points[i0] + axis,
                    base_points[i1] + axis);
                mesh.AddTriangle(
                    base_points[i0],
                    base_points[i1] + axis,
                    base_points[i1]);
            }

            // Transform the mesh's points if desired.
            if (transform != null)
                transform.Transform(mesh);

            return mesh;
        }

        #endregion MakeCylinder

    }
}
