using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Windows.Media;
using System.Windows.Media.Media3D;
using System.Windows.Media.Imaging;
using System.Windows;

namespace PyramidCone
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

        #region PointSharing

        // If the point is already in the dictionary, return its index in the mesh.
        // If the point isn't in the dictionary, create it in the mesh and add its
        // index to the dictionary.
        private static int PointIndex(this MeshGeometry3D mesh,
            Point3D point, Dictionary<Point3D, int> pointDict = null)
        {
            // See if the point already exists.
            if ((pointDict != null) && (pointDict.ContainsKey(point)))
            {
                // The point is already in the dictionary. Return its index.
                return pointDict[point];
            }

            // Create the point.
            int index = mesh.Positions.Count;
            mesh.Positions.Add(point);

            // Add the point's index to the dictionary.
            if (pointDict != null) pointDict.Add(point, index);

            // Return the index.
            return index;
        }

        // If the point is already in the dictionary, return its index in the mesh.
        // If the point isn't in the dictionary, create it and its texture coordinates
        // in the mesh and add its index to the dictionary.
        private static int PointIndex(this MeshGeometry3D mesh,
            Point3D point, Point textureCoord,
            Dictionary<Point3D, int> pointDict = null)
        {
            // See if the point already exists.
            if ((pointDict != null) && (pointDict.ContainsKey(point)))
            {
                // The point is already in the dictionary. Return its index.
                return pointDict[point];
            }

            // Create the point.
            int index = mesh.Positions.Count;
            mesh.Positions.Add(point);

            // Add the point's texture coordinates.
            mesh.TextureCoordinates.Add(textureCoord);

            // Add the point's index to the dictionary.
            if (pointDict != null) pointDict.Add(point, index);

            // Return the index.
            return index;
        }

        #endregion PointSharing

        #region Polygon

        // Add a polygon with points stored in an array.
        // Texture coordinates are optional.
        public static void AddPolygon(this MeshGeometry3D mesh,
            Point3D[] points, Point[] textureCoords = null)
        {
            // Make a point dictionary.
            Dictionary<Point3D, int> pointDict = new Dictionary<Point3D, int>();

            // Get the first two point indices.
            int indexA, indexB, indexC;

            if (textureCoords == null)
                indexA = mesh.PointIndex(points[0].Round(), pointDict);
            else
                indexA = mesh.PointIndex(points[0].Round(), textureCoords[0], pointDict);

            if (textureCoords == null)
                indexC = mesh.PointIndex(points[1].Round(), pointDict);
            else
                indexC = mesh.PointIndex(points[1].Round(), textureCoords[1], pointDict);

            // Make triangles.
            for (int i = 2; i < points.Length; i++)
            {
                indexB = indexC;

                if (textureCoords == null)
                    indexC = mesh.PointIndex(points[i].Round(), pointDict);
                else
                    indexC = mesh.PointIndex(points[i].Round(), textureCoords[i], pointDict);

                if ((indexA != indexB) &&
                    (indexB != indexC) &&
                    (indexC != indexA))
                {
                    mesh.TriangleIndices.Add(indexA);
                    mesh.TriangleIndices.Add(indexB);
                    mesh.TriangleIndices.Add(indexC);
                }
            }
        }

        // Add a polygon with a variable argument list of points
        // and no texture coordinates.
        public static void AddPolygon(this MeshGeometry3D mesh,
            params Point3D[] points)
        {
            mesh.AddPolygon(points, null);
        }

        // Add a regular polygon with optional texture coordinates.
        public static void AddRegularPolygon(this MeshGeometry3D mesh,
            int numSides, Point3D center, Vector3D vx, Vector3D vy,
            Point[] textureCoords = null)
        {
            // Generate the points.
            Point3D[] points = G3.MakePolygonPoints(numSides, center, vx, vy);

            // Make the polygon.
            mesh.AddPolygon(points, textureCoords);
        }

        #endregion Polygon

        #region Models

        // Make a model with a diffuse brush.
        public static GeometryModel3D MakeModel(this MeshGeometry3D mesh, Brush brush)
        {
            Material material = new DiffuseMaterial(brush);
            return new GeometryModel3D(mesh, material);
        }

        // Make a model with a texture brush.
        public static GeometryModel3D MakeModel(this MeshGeometry3D mesh, string uri)
        {
            ImageBrush brush = new ImageBrush();
            brush.ImageSource = new BitmapImage(new Uri(uri, UriKind.Relative));
            return mesh.MakeModel(brush);
        }

        #endregion Models

        #region Parallelogram

        // Add a parallelogram defined by a corner point and two edge vectors.
        // Texture coordinates and the point dictionary are optional.
        public static void AddParallelogram(this MeshGeometry3D mesh,
            Point3D corner, Vector3D v1, Vector3D v2,
            Point[] textureCoords = null)
        {
            // Find the parallelogram's corners.
            Point3D[] points =
            {
                corner,
                corner + v1,
                corner + v1 + v2,
                corner + v2,
            };

            // Make it.
            mesh.AddPolygon(points, textureCoords);
        }

        #endregion Parallelogram

        #region Boxes

        // Add a parallelepiped defined by a corner point and three edge vectors.
        // The vectors should have more or less the orientation of the X, Y, and Z axes.
        // The corner point should be the back, lower, left corner
        // analogous to the smallest X, Y, and Z coordinates.
        // Texture coordinates are optional.
        // Points are shared on each face and not between faces.
        public static void AddBox(this MeshGeometry3D mesh,
            Point3D corner, Vector3D vx, Vector3D vy, Vector3D vz,
            Point[] textureCoords = null)
        {
            mesh.AddBox(corner, vx, vy, vz,
                textureCoords, textureCoords, textureCoords,
                textureCoords, textureCoords, textureCoords);
        }

        // Add a parallelepiped with different texture coordinates for each face.
        public static void AddBox(this MeshGeometry3D mesh,
            Point3D corner, Vector3D vx, Vector3D vy, Vector3D vz,
            Point[] frontCoords, Point[] leftCoords, Point[] rightCoords,
            Point[] backCoords, Point[] topCoords, Point[] bottomCoords)
        {
            mesh.AddParallelogram(corner + vz, vx, vy, frontCoords);        // Front
            mesh.AddParallelogram(corner, vz, vy, leftCoords);              // Left
            mesh.AddParallelogram(corner + vx + vz, -vz, vy, rightCoords);  // Right
            mesh.AddParallelogram(corner + vx, -vx, vy, backCoords);        // Back
            mesh.AddParallelogram(corner + vy + vz, vx, -vz, topCoords);    // Top
            mesh.AddParallelogram(corner, vx, vz, bottomCoords);            // Bottom
        }

        // Add a parallelepiped with wrapped texture coordinates.
        public static void AddBoxWrapped(this MeshGeometry3D mesh,
            Point3D corner, Vector3D vx, Vector3D vy, Vector3D vz)
        {
            // Get texture coordinates for the pieces.
            Point[] frontCoords =
            {
                new Point(0.25, 0.75),
                new Point(0.50, 0.75),
                new Point(0.50, 0.50),
                new Point(0.25, 0.50),
            };
            Point[] leftCoords =
            {
                new Point(0.00, 0.25),
                new Point(0.00, 0.50),
                new Point(0.25, 0.50),
                new Point(0.25, 0.25),
            };
            Point[] rightCoords =
            {
                new Point(0.75, 0.50),
                new Point(0.75, 0.25),
                new Point(0.50, 0.25),
                new Point(0.50, 0.50),
            };
            Point[] backCoords =
            {
                new Point(0.50, 0.00),
                new Point(0.25, 0.00),
                new Point(0.25, 0.25),
                new Point(0.50, 0.25),
            };
            Point[] topCoords =
            {
                new Point(0.25, 0.50),
                new Point(0.50, 0.50),
                new Point(0.50, 0.25),
                new Point(0.25, 0.25),
            };
            Point[] bottomCoords =
            {
                new Point(0.25, 1.00),
                new Point(0.50, 1.00),
                new Point(0.50, 0.75),
                new Point(0.25, 0.75),
            };

            // Add a point to use all texture coordinates in the area (0, 0) - (1, 1).
            mesh.Positions.Add(new Point3D());
            mesh.TextureCoordinates.Add(new Point(1, 1));

            // Add the box.
            mesh.AddBox(corner, vx, vy, vz,
                frontCoords, leftCoords, rightCoords,
                backCoords, topCoords, bottomCoords);
        }

        #endregion Boxes

        #region Axes

        // Make models for the coordinate axes.
        public static void AddXAxis(Model3DGroup group,
            double length = 4, double thickness = 0.1)
        {
            MeshGeometry3D mesh = new MeshGeometry3D();
            Point3D origin = D3.Origin -
                D3.XVector(thickness / 2) -
                D3.YVector(thickness / 2) -
                D3.ZVector(thickness / 2);
            mesh.AddBox(origin,
                D3.XVector(length), D3.YVector(thickness), D3.ZVector(thickness));
            group.Children.Add(mesh.MakeModel(Brushes.Red));
        }

        public static void AddYAxis(Model3DGroup group,
            double length = 4, double thickness = 0.1)
        {
            MeshGeometry3D mesh = new MeshGeometry3D();
            Point3D origin = D3.Origin -
                D3.XVector(thickness / 2) -
                D3.YVector(thickness / 2) -
                D3.ZVector(thickness / 2);
            mesh.AddBox(origin,
                D3.XVector(thickness), D3.YVector(length), D3.ZVector(thickness));
            group.Children.Add(mesh.MakeModel(Brushes.Green));
        }

        public static void AddZAxis(Model3DGroup group,
            double length = 4, double thickness = 0.1)
        {
            MeshGeometry3D mesh = new MeshGeometry3D();
            Point3D origin = D3.Origin -
                D3.XVector(thickness / 2) -
                D3.YVector(thickness / 2) -
                D3.ZVector(thickness / 2);
            mesh.AddBox(origin,
                D3.XVector(thickness), D3.YVector(thickness), D3.ZVector(length));
            group.Children.Add(mesh.MakeModel(Brushes.Blue));
        }

        // Make a cube at the origin.
public static void AddOrigin(Model3DGroup group,
    double cubeThickness = 0.102)
{
    MeshGeometry3D mesh = new MeshGeometry3D();
    Point3D origin = D3.Origin -
        D3.XVector(cubeThickness / 2) -
        D3.YVector(cubeThickness / 2) -
        D3.ZVector(cubeThickness / 2);
    mesh.AddBox(origin,
        D3.XVector(cubeThickness),
        D3.YVector(cubeThickness),
        D3.ZVector(cubeThickness));
    group.Children.Add(mesh.MakeModel(Brushes.Black));
}

        // Make X, Y, and Z axes, and the origin cube.
        public static void AddAxes(Model3DGroup group,
            double length = 4, double thickness = 0.1,
            double cubeThickness = 0.102)
        {
            AddXAxis(group, length, thickness);
            AddYAxis(group, length, thickness);
            AddZAxis(group, length, thickness);
            AddOrigin(group, cubeThickness);
        }

        #endregion Axes

        #region Pyramids

        // Add a pyramid defined by a center point, a polygon, and an axis vector.
        // The polygon should be oriented toward its axis.
        public static void AddPyramid(this MeshGeometry3D mesh,
            Point3D center, Point3D[] polygon, Vector3D axis)
        {
            // Find the apex.
            Point3D apex = center + axis;

            // Make the sides.
            int numPoints = polygon.Length;
            for (int i = 0; i < polygon.Length; i++)
            {
                int i1 = (i + 1) % numPoints;
                mesh.AddPolygon(polygon[i], polygon[i1], apex);
            }

            // Make the bottom.
            Point3D[] bottom = new Point3D[numPoints];
            Array.Copy(polygon, bottom, numPoints);
            Array.Reverse(bottom);
            mesh.AddPolygon(bottom);
        }

        // Add a frustum.
        // Length is the length measured along the axis.
        public static void AddFrustum(this MeshGeometry3D mesh,
            Point3D center, Point3D[] polygon, Vector3D axis, double length)
        {
            // Find the length ratio.
            double ratio = length / axis.Length;

            // See where the apex would be.
            Point3D apex = center + axis;

            // Make the top.
            int numPoints = polygon.Length;
            Point3D[] top = new Point3D[numPoints];
            for (int i = 0; i < polygon.Length; i++)
            {
                Vector3D vector = apex - polygon[i];
                vector *= ratio;
                top[i] = polygon[i] + vector;
            }
            mesh.AddPolygon(top);

            // Make the sides.
            for (int i = 0; i < polygon.Length; i++)
            {
                int i1 = (i + 1) % numPoints;
                mesh.AddPolygon(polygon[i], polygon[i1], top[i1], top[i]);
            }

            // Make the bottom.
            Point3D[] bottom = new Point3D[numPoints];
            Array.Copy(polygon, bottom, numPoints);
            Array.Reverse(bottom);
            mesh.AddPolygon(bottom);
        }

        // Add a frustum where the top is determined by a plane of intersection.
        // The plane is determined by the point planePt and the normal vector n.
        public static void AddFrustum(this MeshGeometry3D mesh,
            Point3D center, Point3D[] polygon, Vector3D axis,
            Point3D planePt, Vector3D n)
        {
            // See where the apex would be.
            Point3D apex = center + axis;

            // Make the top.
            int numPoints = polygon.Length;
            Point3D[] top = new Point3D[numPoints];
            for (int i = 0; i < polygon.Length; i++)
            {
                // Get the vector from the point to the apex.
                Vector3D vector = apex - polygon[i];

                // See where this vector intersects the plane.
                top[i] = IntersectPlaneLine(polygon[i], vector, planePt, n);
            }
            mesh.AddPolygon(top);

            // Make the sides.
            for (int i = 0; i < polygon.Length; i++)
            {
                int i1 = (i + 1) % numPoints;
                mesh.AddPolygon(polygon[i], polygon[i1], top[i1], top[i]);
            }

            // Make the bottom.
            Point3D[] bottom = new Point3D[numPoints];
            Array.Copy(polygon, bottom, numPoints);
            Array.Reverse(bottom);
            mesh.AddPolygon(bottom);
        }

        // Find the intersection of a plane and a line.
        // The line is given by point linePt and vector v.
        // The plane is given by point planePt and normal vector n.
        private static Point3D IntersectPlaneLine(Point3D linePt, Vector3D v,
            Point3D planePt, Vector3D n)
        {
            // Get the equation for the plane.
            // For information on getting the plane equation, see:
            // http://www.songho.ca/math/plane/plane.html
            double A = n.X;
            double B = n.Y;
            double C = n.Z;
            double D = -(A * planePt.X + B * planePt.Y + C * planePt.Z);

            // Find the intersection parameter t.
            // For information on finding the intersection, see:
            // http://www.ambrsoft.com/TrigoCalc/Plan3D/PlaneLineIntersection_.htm
            double t = -(A * linePt.X + B * linePt.Y + C * linePt.Z + D) /
                (A * v.X + B * v.Y + C * v.Z);

            // Find the point of intersection.
            return linePt + t * v;
        }

        #endregion Pyramids
    }
}
