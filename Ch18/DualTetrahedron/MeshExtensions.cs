using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Windows.Media;
using System.Windows.Media.Media3D;
using System.Windows.Media.Imaging;
using System.Windows;

namespace DualTetrahedron
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

            Vector3D[] normals = mesh.Normals.ToArray();
            transformation.Transform(normals);
            mesh.Normals = new Vector3DCollection(normals);
        }

        public static void ApplyTransformation(this MeshGeometry3D mesh, Transform3D transformation)
        {
            Point3D[] points = mesh.Positions.ToArray();
            transformation.Transform(points);
            mesh.Positions = new Point3DCollection(points);

            Vector3D[] normals = mesh.Normals.ToArray();
            transformation.Transform(normals);
            mesh.Normals = new Vector3DCollection(normals);
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
            mesh.AddPolygon(points, null, textureCoords);
        }
        public static void AddPolygon(this MeshGeometry3D mesh,
            Point3D[] points, Dictionary<Point3D, int> pointDict = null,
            Point[] textureCoords = null)
        {
            // Make a point dictionary.
            if (pointDict == null) pointDict = new Dictionary<Point3D, int>();

            // Get the first two point indices.
            int indexA, indexB, indexC;

            Point3D roundedA = points[0].Round();
            if (textureCoords == null)
                indexA = mesh.PointIndex(roundedA, pointDict);
            else
                indexA = mesh.PointIndex(roundedA, textureCoords[0], pointDict);

            Point3D roundedC = points[1].Round();
            if (textureCoords == null)
                indexC = mesh.PointIndex(roundedC, pointDict);
            else
                indexC = mesh.PointIndex(roundedC, textureCoords[1], pointDict);

            // Make triangles.
            Point3D roundedB;
            for (int i = 2; i < points.Length; i++)
            {
                indexB = indexC;
                roundedB = roundedC;

                // Get the next point.
                roundedC = points[i].Round();
                if (textureCoords == null)
                    indexC = mesh.PointIndex(points[i].Round(), pointDict);
                else
                    indexC = mesh.PointIndex(points[i].Round(), textureCoords[i], pointDict);

                // If two of the points are the same, skip this triangle.
                if ((roundedA != roundedB) &&
                    (roundedB != roundedC) &&
                    (roundedC != roundedA))
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
            Dictionary<Point3D, int> pointDict = null,
            params Point3D[] points)
        {
            mesh.AddPolygon(points, pointDict, null);
        }
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
            mesh.AddRegularPolygon(numSides, center, vx, vy, null, textureCoords);
        }
        public static void AddRegularPolygon(this MeshGeometry3D mesh,
            int numSides, Point3D center, Vector3D vx, Vector3D vy,
            Dictionary<Point3D, int> pointDict = null,
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

        // Make a model with a material group.
        public static GeometryModel3D MakeModel(this MeshGeometry3D mesh, MaterialGroup material)
        {
            return new GeometryModel3D(mesh, material);
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
            Point3D center, Point3D[] polygon, Vector3D axis,
            bool smoothSides = false)
        {
            // Find the apex.
            Point3D apex = center + axis;

            // If we should smooth the sides, make the point dictionary.
            Dictionary<Point3D, int> pointDict = null;
            if (smoothSides) pointDict = new Dictionary<Point3D, int>();

            // Make the sides.
            int numPoints = polygon.Length;
            for (int i = 0; i < polygon.Length; i++)
            {
                int i1 = (i + 1) % numPoints;
                mesh.AddPolygon(pointDict, polygon[i], polygon[i1], apex);
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
            Point3D center, Point3D[] polygon, Vector3D axis, double length,
            bool smoothSides = false)
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

            // If we should smooth the sides, make the point dictionary.
            Dictionary<Point3D, int> pointDict = null;
            if (smoothSides) pointDict = new Dictionary<Point3D, int>();

            // Make the sides.
            for (int i = 0; i < polygon.Length; i++)
            {
                int i1 = (i + 1) % numPoints;
                mesh.AddPolygon(pointDict, polygon[i], polygon[i1], top[i1], top[i]);
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
            Point3D planePt, Vector3D n, bool smoothSides = false)
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

            // If we should smooth the sides, make the point dictionary.
            Dictionary<Point3D, int> pointDict = null;
            if (smoothSides) pointDict = new Dictionary<Point3D, int>();

            // Make the sides.
            for (int i = 0; i < polygon.Length; i++)
            {
                int i1 = (i + 1) % numPoints;
                mesh.AddPolygon(pointDict, polygon[i], polygon[i1], top[i1], top[i]);
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
            Point3D planePt, Vector3D n, bool smoothSides = false)
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

        #region Cones

        // These methods delegate their work to pyramid and frustum methods.
        public static void AddCone(this MeshGeometry3D mesh,
            Point3D center, Point3D[] polygon, Vector3D axis,
            bool smoothSides = false)
        {
            mesh.AddPyramid(center, polygon, axis, true);
        }
        public static void AddConeFrustum(this MeshGeometry3D mesh,
            Point3D center, Point3D[] polygon, Vector3D axis, double length,
            bool smoothSides = false)
        {
            mesh.AddFrustum(center, polygon, axis, length, true);
        }
        public static void AddConeFrustum(this MeshGeometry3D mesh,
            Point3D center, Point3D[] polygon, Vector3D axis,
            Point3D planePt, Vector3D n, bool smoothSides = false)
        {
            mesh.AddFrustum(center, polygon, axis, planePt, n, true);
        }

        #endregion Cones

        #region Cylinders

        // Add a cylinder defined by a center point, a polygon, and an axis vector.
        // The cylinder should be oriented toward its axis.
        public static void AddCylinder(this MeshGeometry3D mesh,
            Point3D[] polygon, Vector3D axis, bool smoothSides = false)
        {
            // If we should smooth the sides, make the point dictionary.
            Dictionary<Point3D, int> pointDict = null;
            if (smoothSides) pointDict = new Dictionary<Point3D, int>();

            // Make the top.
            int numPoints = polygon.Length;
            Point3D[] top = new Point3D[numPoints];
            for (int i = 0; i < polygon.Length; i++)
            {
                top[i] = polygon[i] + axis;
            }
            mesh.AddPolygon(top);

            // Make the sides.
            for (int i = 0; i < polygon.Length; i++)
            {
                int i1 = (i + 1) % numPoints;
                mesh.AddPolygon(pointDict, polygon[i], polygon[i1], top[i1], top[i]);
            }

            // Make the bottom.
            Point3D[] bottom = new Point3D[numPoints];
            Array.Copy(polygon, bottom, numPoints);
            Array.Reverse(bottom);
            mesh.AddPolygon(bottom);
        }

        // Add a cylinder defined by a polygon, two axis, and two cutting planes.
        public static void AddCylinder(this MeshGeometry3D mesh,
            Point3D[] polygon, Vector3D axis,
            Point3D topPlanePt, Vector3D topN,
            Point3D bottomPlanePt, Vector3D bottomN,
            bool smoothSides = false)
        {
            // Make the top.
            int numPoints = polygon.Length;
            Point3D[] top = new Point3D[numPoints];
            for (int i = 0; i < polygon.Length; i++)
            {
                // See where this vector intersects the top cutting plane.
                top[i] = IntersectPlaneLine(polygon[i], axis, topPlanePt, topN);
            }
            mesh.AddPolygon(top);

            // Make the bottom.
            Point3D[] bottom = new Point3D[numPoints];
            for (int i = 0; i < polygon.Length; i++)
            {
                // See where this vector intersects the bottom cutting plane.
                bottom[i] = IntersectPlaneLine(polygon[i], axis, bottomPlanePt, bottomN);
            }

            // If we should smooth the sides, make the point dictionary.
            Dictionary<Point3D, int> pointDict = null;
            if (smoothSides) pointDict = new Dictionary<Point3D, int>();

            // Make the sides.
            for (int i = 0; i < polygon.Length; i++)
            {
                int i1 = (i + 1) % numPoints;
                mesh.AddPolygon(pointDict, bottom[i], bottom[i1], top[i1], top[i]);
            }

            // Make the bottom.
            Array.Reverse(bottom);
            mesh.AddPolygon(bottom);
        }

        #endregion Cylinders

        #region Spheres

        // Add a sphere without texture coordinates.
        public static void AddSphere(this MeshGeometry3D mesh,
            Point3D center, double radius, int numTheta, int numPhi,
            bool smooth = false)
        {
            // Make a point dictionary if needed.
            Dictionary<Point3D, int> pointDict = null;
            if (smooth) pointDict = new Dictionary<Point3D, int>();

            // Generate the points.
            double dtheta = 2 * Math.PI / numTheta;
            double dphi = Math.PI / numPhi;
            double theta = 0;
            for (int t = 0; t < numTheta; t++)
            {
                double phi = 0;
                for (int p = 0; p < numPhi; p++)
                {
                    // Find this piece's points.
                    Point3D[] points =
                    {
                        G3.SpherePoint(center, radius, theta, phi),
                        G3.SpherePoint(center, radius, theta, phi + dphi),
                        G3.SpherePoint(center, radius, theta + dtheta, phi + dphi),
                        G3.SpherePoint(center, radius, theta + dtheta, phi),
                    };

                    // Make the polygon.
                    mesh.AddPolygon(pointDict, points);

                    phi += dphi;
                }
                theta += dtheta;
            }
        }

        // Add a sphere with texture coordinates.
        public static void AddTexturedSphere(this MeshGeometry3D mesh,
            Point3D center, double radius, int numTheta, int numPhi,
            bool smooth = false)
        {
            double dtheta = 2 * Math.PI / numTheta;
            double dphi = Math.PI / numPhi;
            double theta = 0;
            for (int t = 0; t < numTheta; t++)
            {
                double phi = 0;
                for (int p = 0; p < numPhi; p++)
                {
                    // Find this piece's points.
                    Point3D point1 = G3.SpherePoint(center, radius, theta, phi).Round();
                    Point3D point2 = G3.SpherePoint(center, radius, theta, phi + dphi).Round();
                    Point3D point3 = G3.SpherePoint(center, radius, theta + dtheta, phi + dphi).Round();
                    Point3D point4 = G3.SpherePoint(center, radius, theta + dtheta, phi).Round();

                    // Find this piece's texture coordinates.
                    Point coords1 = new Point((double)t / numTheta, (double)p / numPhi);
                    Point coords2 = new Point((double)t / numTheta, (double)(p + 1) / numPhi);
                    Point coords3 = new Point((double)(t + 1) / numTheta, (double)(p + 1) / numPhi);
                    Point coords4 = new Point((double)(t + 1) / numTheta, (double)p / numPhi);

                    // Find this piece's normals.
                    Vector3D normal1 = (Vector3D)G3.SpherePoint(D3.Origin, 1, theta, phi).Round();
                    Vector3D normal2 = (Vector3D)G3.SpherePoint(D3.Origin, 1, theta, phi + dphi).Round();
                    Vector3D normal3 = (Vector3D)G3.SpherePoint(D3.Origin, 1, theta + dtheta, phi + dphi).Round();
                    Vector3D normal4 = (Vector3D)G3.SpherePoint(D3.Origin, 1, theta + dtheta, phi).Round();

                    // Make the first triangle.
                    int index = mesh.Positions.Count;
                    mesh.Positions.Add(point1);
                    if (smooth) mesh.Normals.Add(normal1);
                    mesh.TextureCoordinates.Add(coords1);

                    mesh.Positions.Add(point2);
                    if (smooth) mesh.Normals.Add(normal2);
                    mesh.TextureCoordinates.Add(coords2);

                    mesh.Positions.Add(point3);
                    if (smooth) mesh.Normals.Add(normal3);
                    mesh.TextureCoordinates.Add(coords3);

                    mesh.TriangleIndices.Add(index++);
                    mesh.TriangleIndices.Add(index++);
                    mesh.TriangleIndices.Add(index++);

                    // Make the second triangle.
                    mesh.Positions.Add(point1);
                    if (smooth) mesh.Normals.Add(normal1);
                    mesh.TextureCoordinates.Add(coords1);

                    mesh.Positions.Add(point3);
                    if (smooth) mesh.Normals.Add(normal3);
                    mesh.TextureCoordinates.Add(coords3);

                    mesh.Positions.Add(point4);
                    if (smooth) mesh.Normals.Add(normal4);
                    mesh.TextureCoordinates.Add(coords4);

                    mesh.TriangleIndices.Add(index++);
                    mesh.TriangleIndices.Add(index++);
                    mesh.TriangleIndices.Add(index++);

                    phi += dphi;
                }
                theta += dtheta;
            }
        }

        #endregion Spheres

        #region Tori

        // Make a torus without texture coordinates.
        public static void AddTorus(this MeshGeometry3D mesh,
            Point3D center, double R, double r, int numTheta, int numPhi,
            bool smooth = false)
        {
            // Make a point dictionary if needed.
            Dictionary<Point3D, int> pointDict = null;
            if (smooth) pointDict = new Dictionary<Point3D, int>();

            // Generate the points.
            double dtheta = 2 * Math.PI / numTheta;
            double dphi = 2 * Math.PI / numPhi;
            double theta = 0;
            for (int t = 0; t < numTheta; t++)
            {
                double phi = 0;
                for (int p = 0; p < numPhi; p++)
                {
                    // Find this piece's points.
                    Point3D[] points =
                    {
                        G3.TorusPoint(center, R, r, theta + dtheta, phi),
                        G3.TorusPoint(center, R, r, theta + dtheta, phi + dphi),
                        G3.TorusPoint(center, R, r, theta, phi + dphi),
                        G3.TorusPoint(center, R, r, theta, phi),
                    };

                    // Make the polygon.
                    mesh.AddPolygon(pointDict, points);

                    phi += dphi;
                }
                theta += dtheta;
            }
        }

        // Add a textured torus.
        public static void AddTexturedTorus(this MeshGeometry3D mesh,
            Point3D center, double R, double r, int numTheta, int numPhi,
            bool smooth = false)
        {
            double dtheta = 2 * Math.PI / numTheta;
            double dphi = 2 * Math.PI / numPhi;
            double theta = Math.PI;         // Puts the texture's top/bottom on the inside.
            for (int t = 0; t < numTheta; t++)
            {
                double phi = 0;
                for (int p = 0; p < numPhi; p++)
                {
                    // Find this piece's points.
                    Point3D point1 = G3.TorusPoint(center, R, r, theta, phi).Round();
                    Point3D point2 = G3.TorusPoint(center, R, r, theta + dtheta, phi).Round();
                    Point3D point3 = G3.TorusPoint(center, R, r, theta + dtheta, phi + dphi).Round();
                    Point3D point4 = G3.TorusPoint(center, R, r, theta, phi + dphi).Round();

                    // Find this piece's normals.
                    Vector3D normal1 = G3.TorusNormal(D3.Origin, R, r, theta, phi);
                    Vector3D normal2 = G3.TorusNormal(D3.Origin, R, r, theta + dtheta, phi);
                    Vector3D normal3 = G3.TorusNormal(D3.Origin, R, r, theta + dtheta, phi + dphi);
                    Vector3D normal4 = G3.TorusNormal(D3.Origin, R, r, theta, phi + dphi);

                    // Find this piece's texture coordinates.
                    Point coords1 = new Point(1 - (double)p / numPhi, 1 - (double)t / numTheta);
                    Point coords2 = new Point(1 - (double)p / numPhi, 1 - (double)(t + 1) / numTheta);
                    Point coords3 = new Point(1 - (double)(p + 1) / numPhi, 1 - (double)(t + 1) / numTheta);
                    Point coords4 = new Point(1 - (double)(p + 1) / numPhi, 1 - (double)t / numTheta);

                    // Make the first triangle.
                    int index = mesh.Positions.Count;
                    mesh.Positions.Add(point1);
                    if (smooth) mesh.Normals.Add(normal1);
                    mesh.TextureCoordinates.Add(coords1);

                    mesh.Positions.Add(point2);
                    if (smooth) mesh.Normals.Add(normal2);
                    mesh.TextureCoordinates.Add(coords2);

                    mesh.Positions.Add(point3);
                    if (smooth) mesh.Normals.Add(normal3);
                    mesh.TextureCoordinates.Add(coords3);

                    mesh.TriangleIndices.Add(index++);
                    mesh.TriangleIndices.Add(index++);
                    mesh.TriangleIndices.Add(index++);

                    // Make the second triangle.
                    mesh.Positions.Add(point1);
                    if (smooth) mesh.Normals.Add(normal1);
                    mesh.TextureCoordinates.Add(coords1);

                    mesh.Positions.Add(point3);
                    if (smooth) mesh.Normals.Add(normal3);
                    mesh.TextureCoordinates.Add(coords3);

                    mesh.Positions.Add(point4);
                    if (smooth) mesh.Normals.Add(normal4);
                    mesh.TextureCoordinates.Add(coords4);

                    mesh.TriangleIndices.Add(index++);
                    mesh.TriangleIndices.Add(index++);
                    mesh.TriangleIndices.Add(index++);

                    phi += dphi;
                }
                theta += dtheta;
            }

            // Add texture coordinates 1.01 to prevent "seams."
            mesh.Positions.Add(new Point3D());
            mesh.TextureCoordinates.Add(new Point(1.01, 1.01));
        }

        #endregion Tori

        #region Platonic Solids

        // Make a tetrahedron without texture coordinates or smoothing.
        public static void AddTetrahedron(this MeshGeometry3D mesh, bool centered = true)
        {
            // Get the points.
            Point3D A, B, C, D;
            G3.TetrahedronPoints(out A, out B, out C, out D, centered);

            // Make the triangles.
            mesh.AddPolygon(A, B, C);
            mesh.AddPolygon(A, C, D);
            mesh.AddPolygon(A, D, B);
            mesh.AddPolygon(D, C, B);
        }
        public static void VerifyTetrahedron()
        {
            // Get the points.
            Point3D A, B, C, D;
            G3.TetrahedronPoints(out A, out B, out C, out D, true);

            // Verify the points.
            G3.VerifyPoints(A, B, C, D);

            // Verify the faces.
            G3.VerifyPolygon(A, B, C);
            G3.VerifyPolygon(A, C, D);
            G3.VerifyPolygon(A, D, B);
            G3.VerifyPolygon(D, C, B);
        }

        #endregion Platonic Solids

    }
}
