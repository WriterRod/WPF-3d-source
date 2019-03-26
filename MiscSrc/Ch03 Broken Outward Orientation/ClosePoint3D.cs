using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Windows.Media.Media3D;

namespace Interlocked
{
    // A point class that makes nearby points equal.
    public class ClosePoint3D : IEquatable<ClosePoint3D>
    {
        public int Digits;
        public double X, Y, Z;
        public double RoundX, RoundY, RoundZ;

        public ClosePoint3D(int digits, double x, double y, double z)
        {
            Digits = digits;
            X = x;
            Y = y;
            Z = z;
            RoundX = Math.Round(X, Digits);
            RoundY = Math.Round(Y, Digits);
            RoundZ = Math.Round(Z, Digits);
        }
        public ClosePoint3D(int digits, Point3D point)
            : this(digits, point.X, point.Y, point.Z)
        {
        }

        public bool Equals(ClosePoint3D other)
        {
            if (other == null) return false;
            return (
                (RoundX == other.RoundX) &&
                (RoundY == other.RoundY) &&
                (RoundZ == other.RoundZ));
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;

            ClosePoint3D other = obj as ClosePoint3D;
            return (
                (RoundX == other.RoundX) &&
                (RoundY == other.RoundY) &&
                (RoundZ == other.RoundZ));
        }

        public override int GetHashCode()
        {
            return
                RoundX.GetHashCode() +
                RoundY.GetHashCode() +
                RoundZ.GetHashCode();
        }

        public static bool operator ==(ClosePoint3D point1, ClosePoint3D point2)
        {
            if (
                ((object)point1 == null ||
                ((object)point2 == null)))
                return Object.Equals(point1, point2);
            return point1.Equals(point2);
        }

        public static bool operator !=(ClosePoint3D point1, ClosePoint3D point2)
        {
            return (!(point1 == point2));
        }
    }
}
