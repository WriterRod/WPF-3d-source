using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Windows;
using System.Windows.Media.Media3D;

// Miscellaneous 3D extensions.
namespace Horn
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
        public static Point3D Origin
        {
            get { return new Point3D(); }
        }

        // Return vectors along the coordinate axes.
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

        // Return an array containing unit texture coordinates.
        // Points are ordered: LL, LR, UR, UL.
        public static Point[] UnitTextures
        {
            get
            {
                return new Point[]
                {
                    new Point(0, 1),
                    new Point(1, 1),
                    new Point(1, 0),
                    new Point(0, 0),
                };
            }
        }

        // Divide the unit texture coordinates into even sections.
        public static Point[][] SectionTextureCoords(int numRows, int numCols)
        {
            int numTotal = numRows * numCols;
            Point[][] result = new Point[numTotal][];

            double dr = 1.0 / numRows;
            double dc = 1.0 / numCols;
            for (int r = 0; r < numRows; r++)
            {
                for (int c = 0; c < numCols; c++)
                {
                    int i = r * numCols + c;
                    result[i] = new Point[4];
                    result[i][0] = new Point(c * dc, (r + 1) * dr);
                    result[i][1] = new Point((c + 1) * dc, (r + 1) * dr);
                    result[i][2] = new Point((c + 1) * dc, r * dr);
                    result[i][3] = new Point(c * dc, r * dr);
                }
            }
            return result;
        }

        // Make a MaterialGroup from a list of materials.
        public static MaterialGroup MakeMaterialGroup(params Material[] materials)
        {
            MaterialGroup group = new MaterialGroup();
            foreach (Material material in materials)
                group.Children.Add(material);
            return group;
        }
    }
}
