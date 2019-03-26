using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Windows.Media.Media3D;

namespace Tree
{
    public static class VectorExtensions
    {
        // Return the vector scaled.
        public static Vector3D Scale(this Vector3D vector, double length)
        {
            return vector * length / vector.Length;
        }

        // Make vectors radiating in a cone "angle" degrees from the vector.
        // Vectors pref1 and pref2 give preferred up directions.
        public static List<Vector3D> MakeFlowerVectors(this Vector3D vector,
            Vector3D pref1, Vector3D pref2, double angle, int num)
        {
            // Find perpendicular vectors n1 and n2.
            Vector3D stem = vector;
            stem.Normalize();

            Vector3D up = pref1;
            if (Vector3D.AngleBetween(stem, up) < 0.1) up = pref2;

            Vector3D n1 = Vector3D.CrossProduct(up, stem);
            Vector3D n2 = Vector3D.CrossProduct(n1, stem);
            n1.Normalize();
            n2.Normalize();

            // Make the branch vectors.
            List<Vector3D> children = new List<Vector3D>();
            double radians = angle * Math.PI / 180;
            double theta = 0;
            double dtheta = 2 * Math.PI / num;
            for (int i = 0; i < num; i++)
            {
                Vector3D v = n1 * Math.Cos(theta) + n2 * Math.Sin(theta);
                Vector3D child = stem * Math.Cos(radians) + v * Math.Sin(radians);
                children.Add(child);
                theta += dtheta;
            }

            return children;
        }
    }
}
