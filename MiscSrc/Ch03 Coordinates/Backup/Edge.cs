using System;

using System.Windows.Media.Media3D;

namespace Interlocked
{
    public class Edge : IEquatable<Edge>
    {
        public Point3D Point1, Point2;
        public Edge(Point3D point1, Point3D point2)
        {
            Point1 = point1;
            Point2 = point2;
        }

        public bool Equals(Edge other)
        {
            Vector3D v1 = this.Point1 - other.Point1;
            Vector3D v2 = this.Point2 - other.Point2;
            if ((v1.Length < 0.001) && (v2.Length < 0.001)) return true;

            v1 = this.Point2 - other.Point1;
            v2 = this.Point1 - other.Point2;
            if ((v1.Length < 0.001) && (v2.Length < 0.001)) return true;
            return false;
        }
    }
}
