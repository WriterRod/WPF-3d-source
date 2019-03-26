using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Windows.Media.Media3D;

namespace Axes
{
    public static class D3
    {
        // Make a transformation for rotation around an arbitrary axis.
        public static RotateTransform3D Rotate(Vector3D axis, Point3D center, double angle)
        {
            Rotation3D rotation = new AxisAngleRotation3D(axis, angle);
            return new RotateTransform3D(rotation, center);
        }
    }
}
