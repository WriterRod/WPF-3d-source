using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Windows.Media.Media3D;

namespace Spinner
{
    public static class MeshExtensions
    {
        #region Transformation

        // Apply a transformation Matrix3D or transformation class.
        public static void ApplyTransformation(this MeshGeometry3D mesh, Matrix3D transformation)
        {
            Point3D[] points = mesh.Positions.ToArray();
            transformation.Transform(points);
            mesh.Positions = new Point3DCollection(points);
        }

        public static void ApplyTransformation(this MeshGeometry3D mesh, Transform3D transformation)
        {
            Point3D[] points = mesh.Positions.ToArray();
            transformation.Transform(points);
            mesh.Positions = new Point3DCollection(points);
        }

        #endregion Transformation
    }
}
