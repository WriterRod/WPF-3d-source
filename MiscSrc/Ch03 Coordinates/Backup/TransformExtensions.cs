using System.Windows.Media.Media3D;

namespace Interlocked
{
    public static class TransformExtensions
    {
        public static void Transform(this Transform3D transform, Triangle triangle)
        {
            transform.Transform(triangle.Points);
        }

        // Transform a MeshGeometry3D.
        public static void Transform(this Transform3D transform, MeshGeometry3D mesh)
        {
            Point3D[] points = new Point3D[mesh.Positions.Count];
            mesh.Positions.CopyTo(points, 0);
            transform.Transform(points);
            mesh.Positions = new Point3DCollection(points);
        }
    }
}
