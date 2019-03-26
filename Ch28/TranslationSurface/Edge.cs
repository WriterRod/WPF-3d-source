using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Windows.Media.Media3D;

namespace TranslationSurface
{
    // An object to prevent duplicate wireframe edges.
    public class Edge : IEquatable<Edge>, IComparable<Edge>
    {
        public Point3D Point1, Point2;
        public Edge(Point3D point1, Point3D point2)
        {
            // Put them in order so Point1 <= Point2.
            bool p1smaller =
                (point1.X < point2.X) ||
                ((point1.X == point2.X) && (point1.Y < point2.Y)) ||
                ((point1.X == point2.X) && (point1.Y == point2.Y) && (point1.Z < point2.Z));
            if (p1smaller)
            {
                Point1 = point1;
                Point2 = point2;
            }
            else
            {
                Point1 = point2;
                Point2 = point1;
            }
        }

        public bool Equals(Edge other)
        {
            if (ReferenceEquals(other, null)) return false;
            if ((Point1 == other.Point1) && (Point2 == other.Point2)) return true;
            return false;
        }
        public static bool operator ==(Edge edge1, Edge edge2)
        {
            if (ReferenceEquals(edge1, edge2)) return true;
            if ((edge1 == null)) return false;
            return edge1.Equals(edge2);
        }
        public static bool operator !=(Edge edge1, Edge edge2)
        {
            return !(edge1 == edge2);
        }
        public override bool Equals(object obj)
        {
            if (!(obj is Edge)) return false;
            return Equals(obj as Edge);
        }

        public override int GetHashCode()
        {
            return Point1.GetHashCode() ^ Point2.GetHashCode();
        }

        public override string ToString()
        {
            return Point1.ToString() + " --> " + Point2.ToString();
        }

        // Return:
        //      -1 if Point1 < Point2
        //       0 if Point1 == Point2
        //       1 if Point1 > Point2
        public int CompareTo(Edge other)
        {
            if (Point1.X < Point2.X) return -1;
            if (Point1.X > Point2.X) return 1;
            if (Point1.Y < Point2.Y) return -1;
            if (Point1.Y > Point2.Y) return 1;
            if (Point1.Z < Point2.Z) return -1;
            if (Point1.Z > Point2.Z) return 1;
            return 0;
        }
    }
}
