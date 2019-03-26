using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Windows.Media;
using System.Windows.Media.Media3D;
using System.Windows.Media.Imaging;
using System.Windows;

namespace Moonscape
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

        #region Merging

        // Merge a mesh into this one.
        // Do not copy texture coordinates or normals.
        public static void Merge(this MeshGeometry3D mesh, MeshGeometry3D other)
        {
            // Copy the positions. Save their new indices in an indices array.
            int index = mesh.Positions.Count;
            int[] indices = new int[other.Positions.Count];
            for (int i = 0; i < other.Positions.Count; i++)
            {
                mesh.Positions.Add(other.Positions[i]);
                indices[i] = index++;
            }

            // Copy the triangles.
            for (int t = 0; t < other.TriangleIndices.Count; t++)
            {
                int i = other.TriangleIndices[t];
                mesh.TriangleIndices.Add(indices[i]);
            }
        }

        #endregion Merging

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

        // Add a simple polygon with no texture coordinates, smoothing, or wireframe.
        public static void AddPolygon(this MeshGeometry3D mesh,
            HashSet<Edge> edges, double thickness, params Point3D[] points)
        {
            mesh.AddPolygon(pointDict: null, points: points, edges: edges, thickness: thickness);
        }
        public static void AddPolygon(this MeshGeometry3D mesh, params Point3D[] points)
        {
            mesh.AddPolygon(pointDict: null, points: points);
        }

        // Add a polygon.
        public static void AddPolygon(this MeshGeometry3D mesh,
            Dictionary<Point3D, int> pointDict = null,
            HashSet<Edge> edges = null, double thickness = 0.1,
            Point[] textureCoords = null, params Point3D[] points)
        {
            if (edges != null)
            {
                // Make a wireframe polygon.
                mesh.AddPolygonEdges(edges, thickness, points);
            }
            else
            {
                // Make a wireframe polygon.
                mesh.AddPolygonTriangles(pointDict, textureCoords, points);
            }
        }

        // Make a polygon's triangles.
        public static void AddPolygonTriangles(this MeshGeometry3D mesh,
            Dictionary<Point3D, int> pointDict = null,
            Point[] textureCoords = null, params Point3D[] points)
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

        // Add a regular polygon with optional texture coordinates.
        public static void AddRegularPolygon(this MeshGeometry3D mesh,
            int numSides, Point3D center, Vector3D vx, Vector3D vy,
            Dictionary<Point3D, int> pointDict = null,
            HashSet<Edge> edges = null, double thickness = 0.1,
            Point[] textureCoords = null)
        {
            // Generate the points.
            Point3D[] points = G3.MakePolygonPoints(numSides, center, vx, vy);

            // Make the polygon.
            mesh.AddPolygon(pointDict, edges, thickness, textureCoords, points);
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
            Point[] textureCoords = null,
            HashSet<Edge> edges = null, double thickness = 0.1)
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
            mesh.AddPolygon(points: points, textureCoords: textureCoords,
                edges: edges, thickness: thickness);
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
            Point[] textureCoords = null,
            HashSet<Edge> edges = null, double thickness = 0.1)
        {
            mesh.AddBox(corner, vx, vy, vz,
                textureCoords, textureCoords, textureCoords,
                textureCoords, textureCoords, textureCoords,
                edges, thickness);
        }

        // Add a parallelepiped with different texture coordinates for each face.
        public static void AddBox(this MeshGeometry3D mesh,
            Point3D corner, Vector3D vx, Vector3D vy, Vector3D vz,
            Point[] frontCoords, Point[] leftCoords, Point[] rightCoords,
            Point[] backCoords, Point[] topCoords, Point[] bottomCoords,
            HashSet<Edge> edges = null, double thickness = 0.1)
        {
            mesh.AddParallelogram(corner + vz, vx, vy, frontCoords, edges, thickness);        // Front
            mesh.AddParallelogram(corner, vz, vy, leftCoords, edges, thickness);              // Left
            mesh.AddParallelogram(corner + vx + vz, -vz, vy, rightCoords, edges, thickness);  // Right
            mesh.AddParallelogram(corner + vx, -vx, vy, backCoords, edges, thickness);        // Back
            mesh.AddParallelogram(corner + vy + vz, vx, -vz, topCoords, edges, thickness);    // Top
            mesh.AddParallelogram(corner, vx, vz, bottomCoords, edges, thickness);            // Bottom
        }

        // Add a parallelepiped with wrapped texture coordinates.
        public static void AddBoxWrapped(this MeshGeometry3D mesh,
            Point3D corner, Vector3D vx, Vector3D vy, Vector3D vz,
            HashSet<Edge> edges = null, double thickness = 0.1)
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
                backCoords, topCoords, bottomCoords,
                edges, thickness);
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
            bool smoothSides = false, HashSet<Edge> edges = null, double thickness = 0.1)
        {
            // Find the apex.
            Point3D apex = center + axis;

            // If we should smooth the sides, make the point dictionary.
            Dictionary<Point3D, int> pointDict = null;
            if (smoothSides) pointDict = new Dictionary<Point3D, int>();

            // Make the sides.
            int numPoints = polygon.Length;
            for (int i = 0; i < numPoints; i++)
            {
                int i1 = (i + 1) % numPoints;
                mesh.AddPolygon(pointDict, edges, thickness, null,
                    polygon[i], polygon[i1], apex);
            }

            // Make the bottom.
            Point3D[] bottom = new Point3D[numPoints];
            Array.Copy(polygon, bottom, numPoints);
            Array.Reverse(bottom);
            mesh.AddPolygon(pointDict, edges, thickness, null, bottom);
        }

        // Add a frustum.
        // Length is the length measured along the axis.
        public static void AddFrustum(this MeshGeometry3D mesh,
            Point3D center, Point3D[] polygon, Vector3D axis, double length,
            bool smoothSides = false, HashSet<Edge> edges = null, double thickness = 0.1)
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
            mesh.AddPolygon(points: top, edges: edges, thickness: thickness);

            // If we should smooth the sides, make the point dictionary.
            Dictionary<Point3D, int> pointDict = null;
            if (smoothSides) pointDict = new Dictionary<Point3D, int>();

            // Make the sides.
            for (int i = 0; i < polygon.Length; i++)
            {
                int i1 = (i + 1) % numPoints;
                mesh.AddPolygon(pointDict, edges, thickness, null,
                    polygon[i], polygon[i1], top[i1], top[i]);
            }

            // Make the bottom.
            Point3D[] bottom = new Point3D[numPoints];
            Array.Copy(polygon, bottom, numPoints);
            Array.Reverse(bottom);
            mesh.AddPolygon(points: bottom, edges: edges, thickness: thickness);
        }

        // Add a frustum where the top is determined by a plane of intersection.
        // The plane is determined by the point planePt and the normal vector n.
        public static void AddFrustum(this MeshGeometry3D mesh,
            Point3D center, Point3D[] polygon, Vector3D axis,
            Point3D planePt, Vector3D n, bool smoothSides = false,
            HashSet<Edge> edges = null, double thickness = 0.1)
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
            mesh.AddPolygon(points: top, edges: edges, thickness: thickness);

            // If we should smooth the sides, make the point dictionary.
            Dictionary<Point3D, int> pointDict = null;
            if (smoothSides) pointDict = new Dictionary<Point3D, int>();

            // Make the sides.
            for (int i = 0; i < polygon.Length; i++)
            {
                int i1 = (i + 1) % numPoints;
                mesh.AddPolygon(pointDict, edges, thickness, null,
                    polygon[i], polygon[i1], top[i1], top[i]);
            }

            // Make the bottom.
            Point3D[] bottom = new Point3D[numPoints];
            Array.Copy(polygon, bottom, numPoints);
            Array.Reverse(bottom);
            mesh.AddPolygon(points: bottom, edges: edges, thickness: thickness);
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
            HashSet<Edge> edges = null, double thickness = 0.1)
        {
            mesh.AddPyramid(center, polygon, axis, true, edges, thickness);
        }
        public static void AddConeFrustum(this MeshGeometry3D mesh,
            Point3D center, Point3D[] polygon, Vector3D axis, double length,
            HashSet<Edge> edges = null, double thickness = 0.1)
        {
            mesh.AddFrustum(center, polygon, axis, length, true, edges, thickness);
        }
        public static void AddConeFrustum(this MeshGeometry3D mesh,
            Point3D center, Point3D[] polygon, Vector3D axis,
            Point3D planePt, Vector3D n,
            HashSet<Edge> edges = null, double thickness = 0.1)
        {
            mesh.AddFrustum(center, polygon, axis, planePt, n, true, edges, thickness);
        }

        #endregion Cones

        #region Cylinders

        // Add a cylinder defined by a center point, a polygon, and an axis vector.
        // The cylinder should be oriented toward its axis.
        public static void AddCylinder(this MeshGeometry3D mesh,
            Point3D[] polygon, Vector3D axis, bool smoothSides = false,
            HashSet<Edge> edges = null, double thickness = 0.1)
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
            mesh.AddPolygon(points: top, edges: edges, thickness: thickness);

            // Make the sides.
            for (int i = 0; i < polygon.Length; i++)
            {
                int i1 = (i + 1) % numPoints;
                mesh.AddPolygon(pointDict, edges, thickness, null,
                    polygon[i], polygon[i1], top[i1], top[i]);
            }

            // Make the bottom.
            Point3D[] bottom = new Point3D[numPoints];
            Array.Copy(polygon, bottom, numPoints);
            Array.Reverse(bottom);
            mesh.AddPolygon(points: bottom, edges: edges, thickness: thickness);
        }

        // Add a cylinder defined by a polygon, two axis, and two cutting planes.
        public static void AddCylinder(this MeshGeometry3D mesh,
            Point3D[] polygon, Vector3D axis,
            Point3D topPlanePt, Vector3D topN,
            Point3D bottomPlanePt, Vector3D bottomN,
            bool smoothSides = false,
            HashSet<Edge> edges = null, double thickness = 0.1)
        {
            // Make the top.
            int numPoints = polygon.Length;
            Point3D[] top = new Point3D[numPoints];
            for (int i = 0; i < polygon.Length; i++)
            {
                // See where this vector intersects the top cutting plane.
                top[i] = IntersectPlaneLine(polygon[i], axis, topPlanePt, topN);
            }
            mesh.AddPolygon(points: top, edges: edges, thickness: thickness);

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
                mesh.AddPolygon(pointDict, edges, thickness, null,
                    bottom[i], bottom[i1], top[i1], top[i]);
            }

            // Make the bottom.
            Array.Reverse(bottom);
            mesh.AddPolygon(points: bottom, edges: edges, thickness: thickness);
        }

        #endregion Cylinders

        #region Spheres

        // Add a sphere without texture coordinates.
        public static void AddSphere(this MeshGeometry3D mesh,
            Point3D center, double radius, int numTheta, int numPhi,
            bool smooth = false,
            HashSet<Edge> edges = null, double thickness = 0.1)
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
                    mesh.AddPolygon(pointDict: pointDict,
                        edges: edges, thickness: thickness, points: points);

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
            bool smooth = false,
            HashSet<Edge> edges = null, double thickness = 0.1)
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
                    mesh.AddPolygon(pointDict: pointDict, points: points,
                        edges: edges, thickness: thickness);

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
        public static void AddTetrahedron(this MeshGeometry3D mesh, bool centered = true,
            HashSet<Edge> edges = null, double thickness = 0.1)
        {
            // Get the points.
            Point3D A, B, C, D;
            G3.TetrahedronPoints(out A, out B, out C, out D, centered);

            // Make the faces.
            mesh.AddPolygon(edges, thickness, A, B, C);
            mesh.AddPolygon(edges, thickness, A, C, D);
            mesh.AddPolygon(edges, thickness, A, D, B);
            mesh.AddPolygon(edges, thickness, D, C, B);
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

        // Make a cube without texture coordinates or smoothing.
        public static void AddCube(this MeshGeometry3D mesh,
            HashSet<Edge> edges = null, double thickness = 0.1)
        {
            // Get the points.
            Point3D A, B, C, D, E, F, G, H;
            G3.CubePoints(out A, out B, out C, out D, out E, out F, out G, out H);

            // Make the faces.
            mesh.AddPolygon(edges, thickness, A, B, C, D);
            mesh.AddPolygon(edges, thickness, A, D, H, E);
            mesh.AddPolygon(edges, thickness, A, E, F, B);
            mesh.AddPolygon(edges, thickness, G, C, B, F);
            mesh.AddPolygon(edges, thickness, G, F, E, H);
            mesh.AddPolygon(edges, thickness, G, H, D, C);
        }
        public static void VerifyCube()
        {
            // Get the points.
            Point3D A, B, C, D, E, F, G, H;
            G3.CubePoints(out A, out B, out C, out D, out E, out F, out G, out H);

            // Verify the points.
            G3.VerifyPoints(A, B, C, D, E, F, G, H);

            // Verify the faces.
            G3.VerifyPolygon(A, B, C, D);
            G3.VerifyPolygon(A, D, H, E);
            G3.VerifyPolygon(A, E, F, B);
            G3.VerifyPolygon(G, C, B, F);
            G3.VerifyPolygon(G, F, E, H);
            G3.VerifyPolygon(G, H, D, C);
        }

        // Make an octahedron without texture coordinates or smoothing.
        public static void AddOctahedron(this MeshGeometry3D mesh,
            HashSet<Edge> edges = null, double thickness = 0.1)
        {
            // Get the points.
            Point3D A, B, C, D, E, F;
            G3.OctahedronPoints(out A, out B, out C, out D, out E, out F);

            // Make the faces.
            mesh.AddPolygon(edges, thickness, A, B, C);
            mesh.AddPolygon(edges, thickness, A, C, D);
            mesh.AddPolygon(edges, thickness, A, D, E);
            mesh.AddPolygon(edges, thickness, A, E, B);
            mesh.AddPolygon(edges, thickness, F, B, E);
            mesh.AddPolygon(edges, thickness, F, C, B);
            mesh.AddPolygon(edges, thickness, F, D, C);
            mesh.AddPolygon(edges, thickness, F, E, D);
        }
        public static void VerifyOctahedron()
        {
            // Get the points.
            Point3D A, B, C, D, E, F;
            G3.OctahedronPoints(out A, out B, out C, out D, out E, out F);

            // Verify the points.
            G3.VerifyPoints(A, B, C, D);

            // Verify the faces.
            G3.VerifyPolygon(A, B, C);
            G3.VerifyPolygon(A, C, D);
            G3.VerifyPolygon(A, D, E);
            G3.VerifyPolygon(A, E, B);
            G3.VerifyPolygon(F, B, E);
            G3.VerifyPolygon(F, C, B);
            G3.VerifyPolygon(F, D, C);
            G3.VerifyPolygon(F, E, D);
        }

        // Make a dodecahedron without texture coordinates or smoothing.
        public static void AddDodecahedron(this MeshGeometry3D mesh,
            HashSet<Edge> edges = null, double thickness = 0.1)
        {
            // Get the points.
            Point3D A, B, C, D, E, F, G, H, I, J, K, L, M, N, O, P, Q, R, S, T;
            G3.DodecahedronPoints(
                out A, out B, out C, out D, out E,
                out F, out G, out H, out I, out J,
                out K, out L, out M, out N, out O,
                out P, out Q, out R, out S, out T);

            // Make the faces.
            mesh.AddPolygon(edges, thickness, E, D, C, B, A);
            mesh.AddPolygon(edges, thickness, A, B, G, K, F);
            mesh.AddPolygon(edges, thickness, A, F, O, J, E);
            mesh.AddPolygon(edges, thickness, E, J, N, I, D);
            mesh.AddPolygon(edges, thickness, D, I, M, H, C);
            mesh.AddPolygon(edges, thickness, C, H, L, G, B);
            mesh.AddPolygon(edges, thickness, K, P, T, O, F);
            mesh.AddPolygon(edges, thickness, O, T, S, N, J);
            mesh.AddPolygon(edges, thickness, N, S, R, M, I);
            mesh.AddPolygon(edges, thickness, M, R, Q, L, H);
            mesh.AddPolygon(edges, thickness, L, Q, P, K, G);
            mesh.AddPolygon(edges, thickness, P, Q, R, S, T);
        }
        public static void VerifyDodecahedron()
        {
            // Get the points.
            Point3D A, B, C, D, E, F, G, H, I, J, K, L, M, N, O, P, Q, R, S, T;
            G3.DodecahedronPoints(
                out A, out B, out C, out D, out E,
                out F, out G, out H, out I, out J,
                out K, out L, out M, out N, out O,
                out P, out Q, out R, out S, out T);

            // Verify the points.
            G3.VerifyPoints(A, B, C, D, E, F, G, H, I, J, K, L, M, N, O, P, Q, R, S, T);

            // Verify the faces.
            G3.VerifyPolygon(E, D, C, B, A);
            G3.VerifyPolygon(A, B, G, K, F);
            G3.VerifyPolygon(A, F, O, J, E);
            G3.VerifyPolygon(E, J, N, I, D);
            G3.VerifyPolygon(D, I, M, H, C);
            G3.VerifyPolygon(C, H, L, G, B);
            G3.VerifyPolygon(K, P, T, O, F);
            G3.VerifyPolygon(O, T, S, N, J);
            G3.VerifyPolygon(N, S, R, M, I);
            G3.VerifyPolygon(M, R, Q, L, H);
            G3.VerifyPolygon(L, Q, P, K, G);
            G3.VerifyPolygon(P, Q, R, S, T);
        }

        // Make an icosahedron without texture coordinates or smoothing.
        public static void AddIcosahedron(this MeshGeometry3D mesh,
            HashSet<Edge> edges = null, double thickness = 0.1)
        {
            // Get the points.
            Point3D A, B, C, D, E, F, G, H, I, J, K, L;
            G3.IcosahedronPoints(
                out A, out B, out C, out D, out E, out F,
                out G, out H, out I, out J, out K, out L);

            // Make the faces.
            mesh.AddPolygon(edges, thickness, A, C, B);
            mesh.AddPolygon(edges, thickness, A, D, C);
            mesh.AddPolygon(edges, thickness, A, E, D);
            mesh.AddPolygon(edges, thickness, A, F, E);
            mesh.AddPolygon(edges, thickness, A, B, F);
            mesh.AddPolygon(edges, thickness, D, K, C);
            mesh.AddPolygon(edges, thickness, C, K, J);
            mesh.AddPolygon(edges, thickness, C, J, B);
            mesh.AddPolygon(edges, thickness, B, J, I);
            mesh.AddPolygon(edges, thickness, B, I, F);
            mesh.AddPolygon(edges, thickness, F, I, H);
            mesh.AddPolygon(edges, thickness, F, H, E);
            mesh.AddPolygon(edges, thickness, E, H, G);
            mesh.AddPolygon(edges, thickness, E, G, D);
            mesh.AddPolygon(edges, thickness, D, G, K);
            mesh.AddPolygon(edges, thickness, L, J, K);
            mesh.AddPolygon(edges, thickness, L, I, J);
            mesh.AddPolygon(edges, thickness, L, H, I);
            mesh.AddPolygon(edges, thickness, L, G, H);
            mesh.AddPolygon(edges, thickness, L, K, G);
        }
        public static void VerifyIcosahedron()
        {
            // Get the points.
            Point3D A, B, C, D, E, F, G, H, I, J, K, L;
            G3.IcosahedronPoints(
                out A, out B, out C, out D, out E, out F,
                out G, out H, out I, out J, out K, out L);

            // Verify the points.
            G3.VerifyPoints(A, B, C, D, E, F, G, H, I, J, K, L);

            // Verify the faces.
            G3.VerifyPolygon(A, C, B);
            G3.VerifyPolygon(A, D, C);
            G3.VerifyPolygon(A, E, D);
            G3.VerifyPolygon(A, F, E);
            G3.VerifyPolygon(A, B, F);
            G3.VerifyPolygon(D, K, C);
            G3.VerifyPolygon(C, K, J);
            G3.VerifyPolygon(C, J, B);
            G3.VerifyPolygon(B, J, I);
            G3.VerifyPolygon(B, I, F);
            G3.VerifyPolygon(F, I, H);
            G3.VerifyPolygon(F, H, E);
            G3.VerifyPolygon(E, H, G);
            G3.VerifyPolygon(E, G, D);
            G3.VerifyPolygon(D, G, K);
            G3.VerifyPolygon(L, J, K);
            G3.VerifyPolygon(L, I, J);
            G3.VerifyPolygon(L, H, I);
            G3.VerifyPolygon(L, G, H);
            G3.VerifyPolygon(L, K, G);
        }

        #endregion Platonic Solids

        #region Wireframe

        // Make a thin line segment.
        public static void AddSegment(this MeshGeometry3D mesh,
            double thickness, Point3D point1, Point3D point2)
        {
            // Get a vector between the points.
            Vector3D v = point2 - point1;

            // Get perpendicular vectors.
            Vector3D vz, vx;
            double angle = Vector3D.AngleBetween(v, D3.YVector());
            if ((angle > 10) && (angle < 170))
                vz = Vector3D.CrossProduct(v, D3.YVector());
            else
                vz = Vector3D.CrossProduct(v, D3.ZVector());
            vx = Vector3D.CrossProduct(v, vz);

            // Give the perpendicular vectors length thickness.
            vx *= thickness / vx.Length;
            vz *= thickness / vz.Length;

            // Make the box.
            mesh.AddBox(point1 - vx / 2 - vz / 2, vx, v, vz);
        }

        // Add a wireframe edge to this mesh.
        public static void AddEdge(this MeshGeometry3D mesh,
            HashSet<Edge> edges, double thickness, Point3D point1, Point3D point2)
        {
            // If the points are the same, skip it.
            if (point1 == point2) return;

            // See if the edge is already in the HashSet.
            Edge edge = new Edge(point1, point2);
            if (edges.Contains(edge)) return;

            // Add the edge.
            edges.Add(edge);
            mesh.AddSegment(thickness, point1, point2);
        }

        // Add a polygon's wireframe to this mesh.
        public static void AddPolygonEdges(this MeshGeometry3D mesh,
            HashSet<Edge> edges, double thickness, params Point3D[] points)
        {
            for (int i = 0; i < points.Length; i++)
            {
                int i1 = (i + 1) % points.Length;
                mesh.AddEdge(edges, thickness, points[i], points[i1]);
            }
        }

        // Convert a mesh into a new mesh containing a wireframe.
        public static MeshGeometry3D ToWireframe(this MeshGeometry3D mesh, double thickness)
        {
            // Make a dictionary of edges.
            HashSet<Edge> edges = new HashSet<Edge>();

            // Make the wireframe pieces.
            MeshGeometry3D result = new MeshGeometry3D();
            for (int i = 0; i < mesh.TriangleIndices.Count; i += 3)
            {
                Point3D point1 = mesh.Positions[mesh.TriangleIndices[i]];
                Point3D point2 = mesh.Positions[mesh.TriangleIndices[i + 1]];
                Point3D point3 = mesh.Positions[mesh.TriangleIndices[i + 2]];
                result.AddPolygonEdges(edges, thickness, point1, point2, point3);
            }
            return result;
        }

        #endregion Wireframe

        #region Geodesic Sphere

        // Add a geodesic sphere.
        public static void AddGeodesicSphere(this MeshGeometry3D mesh,
            Point3D center, double radius, int numDivisions,
            HashSet<Edge> edges = null, double thickness = 0.1)
        {
            // Create an icosahedron.
            // Get the points.
            Point3D A, B, C, D, E, F, G, H, I, J, K, L;
            G3.IcosahedronPoints(
                out A, out B, out C, out D, out E, out F,
                out G, out H, out I, out J, out K, out L);

            // Scale the icosahedron to the proper radius and center it.
            double scale = radius / G3.IcosahedronCircumradius();
            ScaleTransform3D scaleT = new ScaleTransform3D(scale, scale, scale);
            TranslateTransform3D translateT = new TranslateTransform3D(center.X, center.Y, center.Z);
            Transform3DGroup groupT = new Transform3DGroup();
            groupT.Children.Add(scaleT);
            groupT.Children.Add(translateT);
            A = groupT.Transform(A);
            B = groupT.Transform(B);
            C = groupT.Transform(C);
            D = groupT.Transform(D);
            E = groupT.Transform(E);
            F = groupT.Transform(F);
            G = groupT.Transform(G);
            H = groupT.Transform(H);
            I = groupT.Transform(I);
            J = groupT.Transform(J);
            K = groupT.Transform(K);
            L = groupT.Transform(L);

            // Make the icosahedron's faces.
            List<Triangle> triangles = new List<Triangle>();
            triangles.Add(new Triangle(A, C, B));
            triangles.Add(new Triangle(A, D, C));
            triangles.Add(new Triangle(A, E, D));
            triangles.Add(new Triangle(A, F, E));
            triangles.Add(new Triangle(A, B, F));
            triangles.Add(new Triangle(D, K, C));
            triangles.Add(new Triangle(C, K, J));
            triangles.Add(new Triangle(C, J, B));
            triangles.Add(new Triangle(B, J, I));
            triangles.Add(new Triangle(B, I, F));
            triangles.Add(new Triangle(F, I, H));
            triangles.Add(new Triangle(F, H, E));
            triangles.Add(new Triangle(E, H, G));
            triangles.Add(new Triangle(E, G, D));
            triangles.Add(new Triangle(D, G, K));
            triangles.Add(new Triangle(L, J, K));
            triangles.Add(new Triangle(L, I, J));
            triangles.Add(new Triangle(L, H, I));
            triangles.Add(new Triangle(L, G, H));
            triangles.Add(new Triangle(L, K, G));

            // Subdivide the faces as desired.
            List<Triangle> newTriangles = new List<Triangle>();
            foreach (Triangle triangle in triangles)
            {
                // Subdivide this triangle and add the results to newTriangles.
                newTriangles.AddRange(triangle.DivideGeodesic(center, radius, numDivisions));
            }

            // Create the geodesic sphere.
            foreach (Triangle triangle in newTriangles)
            {
                mesh.AddPolygon(edges, thickness, triangle.Points.ToArray());
            }

            //// Analysis.
            //Console.WriteLine("# Triangles: " + newTriangles.Count);
            //List<double> angles = new List<double>();
            //foreach (Triangle triangle in newTriangles) angles.AddRange(triangle.Angles());
            //var anglesQuery =
            //    from double angle in angles
            //    orderby angle
            //    select Math.Round(angle, 5);
            //Console.Write("Angles:");
            //foreach (double angle in anglesQuery.Distinct())
            //    Console.Write(" " + angle);
            //Console.WriteLine();
        }

        #endregion Geodesic Sphere

        #region Stellate Polyhedrons

        // Make a stellate octahedron without texture coordinates or smoothing.
        public static void AddStellateOctahedron(this MeshGeometry3D mesh,
            double starRadius, HashSet<Edge> edges = null, double thickness = 0.1)
        {
            // Get the octahedron's points.
            Point3D A, B, C, D, E, F;
            G3.OctahedronPoints(out A, out B, out C, out D, out E, out F);

            // Make face polygons.
            List<Polygon> polygons = new List<Polygon>();
            polygons.Add(new Polygon(A, B, C));
            polygons.Add(new Polygon(A, C, D));
            polygons.Add(new Polygon(A, D, E));
            polygons.Add(new Polygon(A, E, B));
            polygons.Add(new Polygon(F, B, E));
            polygons.Add(new Polygon(F, C, B));
            polygons.Add(new Polygon(F, D, C));
            polygons.Add(new Polygon(F, E, D));

            // Stellify the faces.
            List<Triangle> triangles = new List<Triangle>();
            foreach (Polygon polygon in polygons)
            {
                triangles.AddRange(polygon.MakeStellateTriangles(D3.Origin, starRadius));
            }

            // Add triangles to the mesh.
            foreach (Triangle triangle in triangles)
                mesh.AddPolygon(edges, thickness, triangle.Points.ToArray());
        }

        // Make a stellate dodecahedron texture coordinates or smoothing.
        public static void AddStellateDodecahedron(this MeshGeometry3D mesh,
            double starRadius, HashSet<Edge> edges = null, double thickness = 0.1)
        {
            // Get the points.
            Point3D A, B, C, D, E, F, G, H, I, J, K, L, M, N, O, P, Q, R, S, T;
            G3.DodecahedronPoints(
                out A, out B, out C, out D, out E,
                out F, out G, out H, out I, out J,
                out K, out L, out M, out N, out O,
                out P, out Q, out R, out S, out T);

            // Make face polygons.
            List<Polygon> polygons = new List<Polygon>();
            polygons.Add(new Polygon(E, D, C, B, A));
            polygons.Add(new Polygon(A, B, G, K, F));
            polygons.Add(new Polygon(A, F, O, J, E));
            polygons.Add(new Polygon(E, J, N, I, D));
            polygons.Add(new Polygon(D, I, M, H, C));
            polygons.Add(new Polygon(C, H, L, G, B));
            polygons.Add(new Polygon(K, P, T, O, F));
            polygons.Add(new Polygon(O, T, S, N, J));
            polygons.Add(new Polygon(N, S, R, M, I));
            polygons.Add(new Polygon(M, R, Q, L, H));
            polygons.Add(new Polygon(L, Q, P, K, G));
            polygons.Add(new Polygon(P, Q, R, S, T));

            // Stellify the faces.
            List<Triangle> triangles = new List<Triangle>();
            foreach (Polygon polygon in polygons)
            {
                triangles.AddRange(polygon.MakeStellateTriangles(D3.Origin, starRadius));
            }

            // Add triangles to the mesh.
            foreach (Triangle triangle in triangles)
                mesh.AddPolygon(edges, thickness, triangle.Points.ToArray());
        }

        // Make a stellate geodesic sphere without texture coordinates or smoothing.
        public static void AddStellateGeodesicSphere(this MeshGeometry3D mesh,
            Point3D center, double radius, int numDivisions, double starRadius,
            HashSet<Edge> edges = null, double thickness = 0.1)
        {
            // Create an icosahedron.
            // Get the points.
            Point3D A, B, C, D, E, F, G, H, I, J, K, L;
            G3.IcosahedronPoints(
                out A, out B, out C, out D, out E, out F,
                out G, out H, out I, out J, out K, out L);

            // Scale the icosahedron to the proper radius and center it.
            double scale = radius / G3.IcosahedronCircumradius();
            ScaleTransform3D scaleT = new ScaleTransform3D(scale, scale, scale);
            TranslateTransform3D translateT = new TranslateTransform3D(center.X, center.Y, center.Z);
            Transform3DGroup groupT = new Transform3DGroup();
            groupT.Children.Add(scaleT);
            groupT.Children.Add(translateT);
            A = groupT.Transform(A);
            B = groupT.Transform(B);
            C = groupT.Transform(C);
            D = groupT.Transform(D);
            E = groupT.Transform(E);
            F = groupT.Transform(F);
            G = groupT.Transform(G);
            H = groupT.Transform(H);
            I = groupT.Transform(I);
            J = groupT.Transform(J);
            K = groupT.Transform(K);
            L = groupT.Transform(L);

            // Make the icosahedron's faces.
            List<Triangle> triangles = new List<Triangle>();
            triangles.Add(new Triangle(A, C, B));
            triangles.Add(new Triangle(A, D, C));
            triangles.Add(new Triangle(A, E, D));
            triangles.Add(new Triangle(A, F, E));
            triangles.Add(new Triangle(A, B, F));
            triangles.Add(new Triangle(D, K, C));
            triangles.Add(new Triangle(C, K, J));
            triangles.Add(new Triangle(C, J, B));
            triangles.Add(new Triangle(B, J, I));
            triangles.Add(new Triangle(B, I, F));
            triangles.Add(new Triangle(F, I, H));
            triangles.Add(new Triangle(F, H, E));
            triangles.Add(new Triangle(E, H, G));
            triangles.Add(new Triangle(E, G, D));
            triangles.Add(new Triangle(D, G, K));
            triangles.Add(new Triangle(L, J, K));
            triangles.Add(new Triangle(L, I, J));
            triangles.Add(new Triangle(L, H, I));
            triangles.Add(new Triangle(L, G, H));
            triangles.Add(new Triangle(L, K, G));

            // Subdivide the faces as desired.
            List<Triangle> newTriangles = new List<Triangle>();
            foreach (Triangle triangle in triangles)
            {
                // Subdivide this triangle and add the results to newTriangles.
                newTriangles.AddRange(triangle.DivideGeodesic(center, radius, numDivisions));
            }

            // Convert the triangles into polygons.
            List<Polygon> polygons = new List<Polygon>();
            foreach (Triangle triangle in newTriangles)
                polygons.Add(new Polygon(triangle.Points.ToArray()));

            // Stellify the triangles.
            List<Triangle> stellateTriangles = new List<Triangle>();
            foreach (Polygon polygon in polygons)
            {
                stellateTriangles.AddRange(polygon.MakeStellateTriangles(center, starRadius));
            }

            // Add triangles to the mesh.
            foreach (Triangle triangle in stellateTriangles)
                mesh.AddPolygon(edges, thickness, triangle.Points.ToArray());
        }

        #endregion Stellate Polyhedrons

        #region Surfaces

        // Make a surface y = F(x, z).
        public static void AddSurface(this MeshGeometry3D mesh,
            Func<double, double, Point3D> F,
            double xmin, double xmax, int numX,
            double zmin, double zmax, int numZ,
            bool smooth = false,
            HashSet<Edge> edges = null, double thickness = 0.1,
            Point[] textureCoords = null)
        {
            // Make a point dictionary if desired.
            Dictionary<Point3D, int> pointDict = null;
            if (smooth) pointDict = new Dictionary<Point3D, int>();

            // Generate the surface's points.
            double dx = (xmax - xmin) / numX;
            double dz = (zmax - zmin) / numZ;
            double x = xmin;
            for (int ix = 0; ix < numX; ix++)
            {
                double z = zmin;
                for (int iz = 0; iz < numZ; iz++)
                {
                    Point3D p1 = F(x, z);
                    Point3D p2 = F(x, z + dz);
                    Point3D p3 = F(x + dx, z + dz);
                    Point3D p4 = F(x + dx, z);
                    mesh.AddPolygon(pointDict, edges, thickness, textureCoords, p1, p2, p3, p4);
                    z += dz;
                }
                x += dx;
            }
        }

        // Make a surface defined by a 2D array of points.
        public static void AddSurface(this MeshGeometry3D mesh,
            Point3D[,] points,
            bool smooth = false,
            HashSet<Edge> edges = null, double thickness = 0.1,
            Point[] textureCoords = null)
        {
            // Make a point dictionary if desired.
            Dictionary<Point3D, int> pointDict = null;
            if (smooth) pointDict = new Dictionary<Point3D, int>();

            // See how many pieces there are.
            int numX = points.GetUpperBound(0);
            int numZ = points.GetUpperBound(1);

            // Build the pieces.
            for (int ix = 0; ix < numX; ix++)
            {
                for (int iz = 0; iz < numZ; iz++)
                {
                    Point3D p1 = points[ix, iz];
                    Point3D p2 = points[ix, iz + 1];
                    Point3D p3 = points[ix + 1, iz + 1];
                    Point3D p4 = points[ix + 1, iz];
                    mesh.AddPolygon(pointDict, edges, thickness, textureCoords, p1, p2, p3, p4);
                }
            }
        }

        #endregion Surfaces
        
        #region Normals

        // Convert a mesh into a new mesh containing triangle normals.
        public static MeshGeometry3D ToNormals(this MeshGeometry3D mesh, double thickness,
            double length)
        {
            // Make a dictionary of edges.
            HashSet<Edge> edges = new HashSet<Edge>();

            // Loop through the triangles.
            MeshGeometry3D result = new MeshGeometry3D();
            for (int i = 0; i < mesh.TriangleIndices.Count; i += 3)
            {
                // Get vectors.
                Vector3D v01 =
                    mesh.Positions[mesh.TriangleIndices[i]] -
                    mesh.Positions[mesh.TriangleIndices[i + 1]];
                Vector3D v12 =
                    mesh.Positions[mesh.TriangleIndices[i + 1]] -
                    mesh.Positions[mesh.TriangleIndices[i + 2]];

                // Find the cross product.
                Vector3D n = Vector3D.CrossProduct(v01, v12);

                // Set the length.
                n = n * length / n.Length;

                // Find the triangle's center.
                Point3D center = new Point3D();
                for (int j = i; j < i + 3; j++)
                {
                    center.X += mesh.Positions[mesh.TriangleIndices[j]].X;
                    center.Y += mesh.Positions[mesh.TriangleIndices[j]].Y;
                    center.Z += mesh.Positions[mesh.TriangleIndices[j]].Z;
                }
                center.X /= 3.0;
                center.Y /= 3.0;
                center.Z /= 3.0;

                // Add the normal.
                result.AddEdge(edges, thickness, center, center + n);
            }
            return result;
        }

        #endregion Normals

        #region Surfaces of Transformation

        // Add a surface of transformation.
        //
        // The trans parameter can be a specific transformation such as
        // TranslateTransform3D or it can be a Transform3D.
        //
        // To treat the points as a closed figure, repeat the first point at the end.
        //
        // Points must be oriented properly for the given transformation.
        //
        // If closeStart, make a polygon out of the generating polygon.
        //
        // If closeEnd, make a polygon out of the final transformed version
        // of the generating polygon.
        //
        // If closeFirst, make a polygon by transforming the first point.
        // The points must be oriented so that polygon is inwardly oriented.
        //
        // If closeLast, make a polygon by transforming the last point.
        // The points must be oriented so that polygon is inwardly oriented.
        public static void AddTransformSurface(this MeshGeometry3D mesh,
            Point3D[] generator, Transform3D trans, int num,
            bool closeTransStart = false, bool closeTransEnd = false,
            bool closeRotStart = false, bool closeRotEnd = false,
            bool smooth = false,
            HashSet<Edge> edges = null, double thickness = 0.1)
        {
            // Make a point dictionary if needed.
            Dictionary<Point3D, int> pointDict = null;
            if (smooth) pointDict = new Dictionary<Point3D, int>();

            // Make two working arrays.
            int numPoints = generator.Length;
            Point3D[] pts1 = new Point3D[generator.Length];
            Point3D[] pts2 = new Point3D[generator.Length];

            // Copy the original points into pts2.
            Array.Copy(generator, pts2, numPoints);

            // Apply the transformation.
            for (int i = 0; i < num; i++)
            {
                // Copy the last batch of points into pts1.
                Array.Copy(pts2, pts1, numPoints);

                // Transform the points in pts2.
                trans.Transform(pts2);

                // Build the edges.
                for (int p = 1; p < numPoints; p++)
                {
                    Point3D[] sidePts =
                    {
                        pts1[p - 1], pts1[p], pts2[p], pts2[p - 1],
                    };
                    mesh.AddPolygon(pointDict: pointDict,
                        edges: edges, thickness: thickness, points:sidePts);
                }
            }

            // Close the ends of a surface of translation if desired.
            if (closeTransStart)
            {
                Point3D[] pts = new Point3D[numPoints];
                Array.Copy(generator, pts, numPoints);
                Array.Reverse(pts);
                mesh.AddPolygon(pointDict: pointDict,
                    edges: edges, thickness: thickness, points: pts);
            }
            if (closeTransEnd)
            {
                mesh.AddPolygon(pointDict: pointDict,
                    edges: edges, thickness: thickness, points: pts2);
            }

            // Close the ends of a surface of rotation if desired.
            if (closeRotStart)
            {
                Point3D[] pts = GetTransformPolygon(generator[0], trans, num);
                mesh.AddPolygon(pointDict: pointDict,
                    edges: edges, thickness: thickness, points: pts);
            }
            if (closeRotEnd)
            {
                Point3D[] pts = GetTransformPolygon(generator[numPoints - 1], trans, num);
                Array.Reverse(pts);
                mesh.AddPolygon(pointDict: pointDict,
                    edges: edges, thickness: thickness, points: pts);
            }
        }

        // Return an array containing a point and its transformed versions.
        public static Point3D[] GetTransformPolygon(
            Point3D point, Transform3D trans, int num)
        {
            // Make an array to hold the point and its transformations.
            Point3D[] points = new Point3D[num];

            // Transform the point.
            for (int i = 0; i < num; i++)
            {
                points[i] = point;
                point = trans.Transform(point);
            }

            // Return the points.
            return points;
        }

        #endregion Surfaces of Transformation

        #region Heightmaps

        // Map Y values (minY <= y <= maxY) to texture coordinates (minV <= y <= maxV).
        // Removes any previous texture coordinates.
        public static void ApplyHeightMap(this MeshGeometry3D mesh,
            double minV, double maxV, double minY, double maxY)
        {
            double ydiff = maxY - minY;
            double vdiff = maxV - minV;
            mesh.TextureCoordinates.Clear();
            for (int i = 0; i < mesh.Positions.Count; i++)
            {
                double v = minV + (mesh.Positions[i].Y - minY) * vdiff / ydiff;
                mesh.TextureCoordinates.Add(new Point(0, v));
            }
        }

        #endregion Heightmaps

    }
}
