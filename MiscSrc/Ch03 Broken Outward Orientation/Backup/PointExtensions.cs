using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Windows.Media.Media3D;

namespace Interlocked
{
    public static class PointExtensions
    {
        public static Point3D Scale(this Point3D point, double scale)
        {
            return new Point3D(
                scale * point.X,
                scale * point.Y,
                scale * point.Z);
        }
    }
}
