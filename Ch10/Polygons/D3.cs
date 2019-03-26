using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Windows;
using System.Windows.Media.Media3D;

namespace Polygons
{
    public static class D3
    {
        // Make a transformation for rotation around an arbitrary axis.
        public static RotateTransform3D Rotate(Vector3D axis, Point3D center, double angle)
        {
            Rotation3D rotation = new AxisAngleRotation3D(axis, angle);
            return new RotateTransform3D(rotation, center);
        }

        // Return the origin.
        public static Point3D Origin = new Point3D();

        // Return vectors along the coordinate axes.
        public static Vector3D XUnit = new Vector3D(1, 0, 0);
        public static Vector3D YUnit = new Vector3D(0, 1, 0);
        public static Vector3D ZUnit = new Vector3D(0, 0, 1);
        public static Vector3D XVector(double length = 1)
        {
            return new Vector3D(length, 0, 0);
        }
        public static Vector3D YVector(double length = 1)
        {
            return new Vector3D(0, length, 0);
        }
        public static Vector3D ZVector(double length = 1)
        {
            return new Vector3D(0, 0, length);
        }

        // Make texture coordinates for a polygon.
        // The first point is at the top.
        public static Point[] MakePolygonTextureCoords(int numSides)
        {
            double dtheta = 2 * Math.PI / numSides;
            double theta = Math.PI / 2;
            Point[] coords = new Point[numSides];
            for (int i = 0; i < numSides; i++)
            {
                coords[i] = new Point(
                    0.5 + Math.Cos(theta) / 2,
                    0.5 - Math.Sin(theta) / 2);
                theta += dtheta;
            }
            return coords;
        }
    }
}