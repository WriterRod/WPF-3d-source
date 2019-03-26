using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Windows.Media.Media3D;

namespace VectorArithmetic
{
    class Program
    {
        static void Main(string[] args)
        {
            Vector3D u = new Vector3D(0, 3, 4);
            Vector3D v = new Vector3D(0, 4, -3);
            Point3D a = new Point3D(10, 12, 15);
            Point3D b = a + u;      // Point + Vector = Point
            Vector3D w = b - a;     // Point - Point = Vector
            Vector3D t = u + v;     // Vector + Vector = Vector

            double angle = Vector3D.AngleBetween(u, v);
            Vector3D cross = Vector3D.CrossProduct(u, v);
            double dot = Vector3D.DotProduct(u, v);

            Vector3D normal = cross;
            normal.Normalize();
            Vector3D scaled = normal * 10;

            Console.WriteLine("u:\t" + u.ToString());
            Console.WriteLine("v:\t" + v.ToString());
            Console.WriteLine("w:\t" + w.ToString());
            Console.WriteLine("a:\t" + a.ToString());
            Console.WriteLine("b:\t" + b.ToString());
            Console.WriteLine("angle:\t" + angle.ToString());
            Console.WriteLine("cross:\t" + cross.ToString());
            Console.WriteLine("dot:\t" + dot.ToString());
            Console.WriteLine("normal:\t" + normal.ToString());
            Console.WriteLine("scaled:\t" + scaled.ToString());
            Console.ReadLine();
        }
    }
}
