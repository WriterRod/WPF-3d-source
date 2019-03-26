using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Windows.Media.Media3D;

namespace BarChart
{
    // Hold three points that make up a triangle.
    public class Triangle
    {
        public List<Point3D> Points = null;

        // Copy the points into the Points list.
        public Triangle(params Point3D[] points)
        {
            Points = new List<Point3D>(points);
        }

        // Divide this triangle into triangles for use in geodesic spheres.
        public List<Triangle> DivideGeodesic(Point3D center, double radius, int numRows)
        {
            // Make vectors 1/numDivisions of the length along the triangle's edges.
            Vector3D vAB = (Points[1] - Points[0]) / numRows;
            Vector3D vBC = (Points[2] - Points[1]) / numRows;

            // Use vector arithmetic to create the points.
            List<Triangle> triangles = new List<Triangle>();
            for (int row = 0; row < numRows; row++)
            {
                for (int col = 0; col <= row; col++)
                {
                    // Make the points we will need.
                    Point3D p0 = Points[0] + row * vAB + col * vBC;
                    Point3D p1 = p0 + vAB;
                    Point3D p2 = p1 + vBC;
                    Point3D p3 = p0 + vBC;

                    // Project the points onto the sphere.
                    p0 = p0.SetDistanceFrom(center, radius);
                    p1 = p1.SetDistanceFrom(center, radius);
                    p2 = p2.SetDistanceFrom(center, radius);
                    p3 = p3.SetDistanceFrom(center, radius);

                    // Make the lower triangle.
                    triangles.Add(new Triangle(p0, p1, p2));
                    if (col == row) break;

                    // Make the upper triangle.
                    triangles.Add(new Triangle(p0, p2, p3));
                }
            }
            return triangles;
        }

        // Return the triangle's angles, sorted.
        public List<double> Angles()
        {
            int numPoints = Points.Count;
            List<double> angles = new List<double>();
            for (int i = 0; i < numPoints; i++)
            {
                int i1 = (i + 1) % numPoints;
                int i2 = (i + 2) % numPoints;
                Vector3D v1 = Points[i1] - Points[i];
                Vector3D v2 = Points[i1] - Points[i2];
                angles.Add(Vector3D.AngleBetween(v1, v2));
            }

            angles.Sort();
            return angles;
        }
    }
}
