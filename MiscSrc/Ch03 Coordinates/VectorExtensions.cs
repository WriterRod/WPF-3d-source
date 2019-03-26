using System.Windows.Media.Media3D;
using System.Collections.Generic;
using System;
using System.Diagnostics;

namespace Interlocked
{
    public static class VectorExtensions
    {
        public static Vector3D Scale(this Vector3D vector, double length)
        {
            double scale = length / vector.Length;
            return vector * scale;
        }

        // Make vectors radiating in a cone angle degrees from the vector.
        public static List<Vector3D> MakeFlowerVectors(this Vector3D vector,
            Vector3D pref1, Vector3D pref2, double angle, int num)
        {
            // Find perpendicular vectors.
            Vector3D stem = vector;
            stem.Normalize();
            Vector3D n1 = pref1;
            n1.Normalize();

            Vector3D n2 = Vector3D.CrossProduct(n1, stem);
            if (n2.Length < 0.1)
            {
                n1 = pref2;
                n1.Normalize();
                n2 = Vector3D.CrossProduct(n1, stem);
            }
            n2.Normalize();
            n1 = Vector3D.CrossProduct(stem, n2);
            Debug.Assert(Math.Abs(n1.Length - 1) < 0.1);

            // Make the branch vectors.
            List<Vector3D> children = new List<Vector3D>();
            double angle_radians = angle * Math.PI / 180;
            double theta = 0;
            double dtheta = 2 * Math.PI / num;
            for (int i = 0; i < num; i++)
            {
                Vector3D v =
                    n1 * Math.Cos(theta) +
                    n2 * Math.Sin(theta);
                Vector3D child =
                    stem * Math.Cos(angle_radians) +
                    v * Math.Sin(angle_radians);
                children.Add(child);
                theta += dtheta;
            }

            return children;
        }
    }
}
