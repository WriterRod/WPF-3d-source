using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using System.Windows.Controls;

namespace CirclePoints
{
    public static class Cameras
    {
        // Convert a single point from 3D to 2D.
        public static Point Convert3DPoint(Point3D point3d, Viewport3D vp)
        {
            // Get the combined transformation matrix.
            Viewport3DVisual visual =
                VisualTreeHelper.GetParent(vp.Children[0]) as Viewport3DVisual;
            Matrix3D matrix = GetWorldToViewportMatrix(visual);

            // Transform the point.
            point3d = matrix.Transform(point3d);
            return new Point(point3d.X, point3d.Y);
        }

        // Convert an array of points from 3D to 2D.
        public static Point[] Convert3DPoints(Point3D[] points3d, Viewport3D vp)
        {
            // Get the combined transformation matrix.
            Viewport3DVisual visual =
                VisualTreeHelper.GetParent(vp.Children[0]) as Viewport3DVisual;
            Matrix3D matrix = GetWorldToViewportMatrix(visual);

            // Copy the points.
            int numPoints = points3d.Length;
            Point3D[] copiedPoints = new Point3D[numPoints];
            Array.Copy(points3d, copiedPoints, numPoints);

            // Transform the copied points.
            matrix.Transform(copiedPoints);

            // Convert the Point3Ds into Points.
            Point[] points2d = new Point[numPoints];
            for (int i = 0; i < numPoints; i++)
            {
                points2d[i] = new Point(copiedPoints[i].X, copiedPoints[i].Y);
            }
            return points2d;
        }

        // Return a matrix that combines the view, projection, and viewport transformations.
        public static Matrix3D GetWorldToViewportMatrix(Viewport3DVisual visual)
        {
            Camera camera = visual.Camera;
            Rect rect = visual.Viewport;
            Matrix3D result = Matrix3D.Identity;

            // If the camera has a transform, add its inverse to the result.
            Transform3D transform = camera.Transform;
            if ((transform != null) && (!transform.Value.IsIdentity))
            {
                Matrix3D matrix = transform.Value;
                matrix.Invert();
                result.Append(matrix);
            }

            // Add the view, projection, and viewport transformations.
            result.Append(GetViewMatrix(camera));
            result.Append(GetProjectionMatrix(camera, rect.Width / rect.Height));
            result.Append(GetViewportMatrix(rect));

            return result;
        }

        // Return the view matrix for a projection camera.
        // (Projection cameras include perspective and orthographic cameras.)
        public static Matrix3D GetViewMatrix(Point3D position,
            Vector3D lookDirection, Vector3D upDirection)
        {
            Vector3D zaxis = -lookDirection;
            zaxis.Normalize();
            Vector3D xaxis = Vector3D.CrossProduct(upDirection, zaxis);
            xaxis.Normalize();
            Vector3D yaxis = Vector3D.CrossProduct(zaxis, xaxis);

            Vector3D positionDirection = (Vector3D)position;
            double offsetX = -Vector3D.DotProduct(xaxis, positionDirection);
            double offsetY = -Vector3D.DotProduct(yaxis, positionDirection);
            double offsetZ = -Vector3D.DotProduct(zaxis, positionDirection);

            return new Matrix3D(
                xaxis.X, yaxis.X, zaxis.X, 0,
                xaxis.Y, yaxis.Y, zaxis.Y, 0,
                xaxis.Z, yaxis.Z, zaxis.Z, 0,
                offsetX, offsetY, offsetZ, 1);
        }
        public static Matrix3D GetViewMatrix(ProjectionCamera camera)
        {
            return GetViewMatrix(camera.Position, camera.LookDirection, camera.UpDirection);
        }

        // Return a view matrix for a camera that is either
        // a projection camera or a matrix camera.
        public static Matrix3D GetViewMatrix(Camera camera)
        {
            if (camera is ProjectionCamera)
                return GetViewMatrix(camera as ProjectionCamera);
            if (camera is MatrixCamera)
                return (camera as MatrixCamera).ViewMatrix;
            throw new ArgumentException("Unknown camera type " +
                camera.GetType().FullName, "camera");
        }

        // Return a projection matrix for an orthographic camera.
        // This is the same matrix returned by the D3DXMatrixOrthoRH function described at:
        // https://msdn.microsoft.com/library/windows/desktop/bb205349.aspx
        private static Matrix3D GetOrthographicProjectionMatrix(double width,
            double nearPlaneDistance, double farPlaneDistance, double aspectRatio)
        {
            double w = width;
            double h = w / aspectRatio;
            double zn = nearPlaneDistance;
            double zf = farPlaneDistance;
            double m33 = 1 / (zn - zf);
            double m43 = zn * m33;
            return new Matrix3D(
                2 / w, 0, 0, 0,
                    0, 2 / h, 0, 0,
                    0, 0, m33, 0,
                    0, 0, m43, 1);
        }
        private static Matrix3D GetProjectionMatrix(OrthographicCamera camera, double aspectRatio)
        {
            return GetOrthographicProjectionMatrix(camera.Width,
                camera.NearPlaneDistance, camera.FarPlaneDistance, aspectRatio);
        }

        // Return a projection matrix for a perspective camera.
        // This is the same matrix returned by the D3DXMatrixPerspectiveRH function described at:
        // https://msdn.microsoft.com/library/windows/desktop/bb205355.aspx
        public static Matrix3D GetPerspectiveProjectionMatrix(double fieldOfView,
            double nearPlaneDistance, double farPlaneDistance, double aspectRatio)
        {
            fieldOfView = DegreesToRadians(fieldOfView);
            double zn = nearPlaneDistance;
            double zf = farPlaneDistance;
            double xScale = 1 / Math.Tan(fieldOfView / 2);
            double yScale = aspectRatio * xScale;
            double m33 = (zf == double.PositiveInfinity) ? -1 : (zf / (zn - zf));
            double m43 = zn * m33;
            return new Matrix3D(
                xScale, 0, 0, 0,
                     0, yScale, 0, 0,
                     0, 0, m33, -1,
                     0, 0, m43, 0);
        }
        public static Matrix3D GetProjectionMatrix(PerspectiveCamera camera, double aspectRatio)
        {
            return GetPerspectiveProjectionMatrix(camera.FieldOfView,
                camera.NearPlaneDistance, camera.FarPlaneDistance, aspectRatio);
        }

        // Return a projection matrix for a perspective, orthographic, or matrix camera.
        public static Matrix3D GetProjectionMatrix(Camera camera, double aspectRatio)
        {
            if (camera is PerspectiveCamera)
                return GetProjectionMatrix(camera as PerspectiveCamera, aspectRatio);
            if (camera is OrthographicCamera)
                return GetProjectionMatrix(camera as OrthographicCamera, aspectRatio);
            if (camera is MatrixCamera)
                return (camera as MatrixCamera).ProjectionMatrix;
            throw new ArgumentException("Unknown camera type " +
                camera.GetType().FullName, "camera");
        }

        // Return a matrix to map from projected coordinates to the viewport rectangle.
        public static Matrix3D GetViewportMatrix(Rect rect)
        {
            double scaleX = rect.Width / 2;
            double scaleY = rect.Height / 2;
            double offsetX = rect.X + scaleX;
            double offsetY = rect.Y + scaleY;
            return new Matrix3D(
                 scaleX,       0, 0, 0,
                      0, -scaleY, 0, 0,
                      0,       0, 1, 0,
                offsetX, offsetY, 0, 1);
        }

        public static double DegreesToRadians(double degrees)
        {
            return degrees * (Math.PI / 180.0);
        }
    }
}
