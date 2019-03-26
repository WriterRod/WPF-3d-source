using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Windows.Media.Media3D;

namespace RandomSurfaces
{
    // Hold points that make up a polygon.
    public class Polygon
    {
        public List<Point3D> Points = null;

        // Copy the points into the Points list.
        public Polygon(params Point3D[] points)
        {
            Points = new List<Point3D>(points);
        }

        // Create triangles for stellation.
        public List<Triangle> MakeStellateTriangles(Point3D center, double radius)
        {
            // Find the polygon's center.
            Point3D pgonCenter = Center;

            // Find the unit vector from the stellar center to the polygon's center.
            Vector3D v = pgonCenter - center;

            // Find the pyramid's apex.
            Point3D apex = center + v / v.Length * radius;

            // Make a pyramid with this polygon as its base and the calculated apex.
            List<Triangle> triangles = new List<Triangle>();
            int numPoints = Points.Count;
            for (int i = 0; i < numPoints; i++)
            {
                int i1 = (i + 1) % numPoints;
                triangles.Add(new Triangle(Points[i], Points[i1], apex));
            }
            return triangles;
        }

        // Find the polygon's center by averaging its vertices.
        public Point3D Center
        {
            get
            {
                Point3D center = new Point3D();
                foreach (Point3D point in Points)
                {
                    center.X += point.X;
                    center.Y += point.Y;
                    center.Z += point.Z;
                }
                int numPoints = Points.Count;
                center.X /= numPoints;
                center.Y /= numPoints;
                center.Z /= numPoints;
                return center;
            }
        }
    }
}
