using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using System.Windows.Media.Media3D;

namespace AllSolids
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        // The camera.
        private PerspectiveCamera TheCamera = null;

        // The camera controller.
        private SphericalCameraController CameraController = null;

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // Define WPF objects.
            ModelVisual3D visual3d = new ModelVisual3D();
            Model3DGroup modelGroup = new Model3DGroup();
            visual3d.Content = modelGroup;
            mainViewport.Children.Add(visual3d);

            // Define the camera, lights, and model.
            DefineCamera(mainViewport);
            DefineLights(modelGroup);
            DefineModel(modelGroup);
        }

        // Define the camera.
        private void DefineCamera(Viewport3D viewport)
        {
            TheCamera = new PerspectiveCamera();
            TheCamera.FieldOfView = 60;
            CameraController = new SphericalCameraController
                (TheCamera, viewport, this, mainGrid, mainGrid);
        }

        // Define the lights.
        private void DefineLights(Model3DGroup group)
        {
            Color darker = Color.FromArgb(255, 64, 64, 64);
            Color dark = Color.FromArgb(255, 96, 96, 96);

            group.Children.Add(new AmbientLight(darker));

            group.Children.Add(new DirectionalLight(dark, new Vector3D(0, -1, 0)));
            group.Children.Add(new DirectionalLight(dark, new Vector3D(1, -3, -2)));
            group.Children.Add(new DirectionalLight(dark, new Vector3D(-1, 3, 2)));
        }

        // Define the model.
        private void DefineModel(Model3DGroup group)
        {
            // Axes.
            MeshExtensions.AddAxes(group);

            MeshGeometry3D mesh1 = new MeshGeometry3D();
            MeshGeometry3D mesh2 = new MeshGeometry3D();
            MeshGeometry3D mesh3 = new MeshGeometry3D();
            MeshGeometry3D mesh4 = new MeshGeometry3D();

            double x0 = -5;
            Point3D center = new Point3D(x0, 0, 4.5);

            // Box.
            mesh1.AddBox(center + new Vector3D(-0.5, 0, -1),
                D3.XVector(1), D3.YVector(3), D3.ZVector(2));
            center.X += 2;

            // Box wrapped.
            mesh2.AddBoxWrapped(center + new Vector3D(-0.5, 0, -1),
                D3.XVector(1), D3.YVector(3), D3.ZVector(2));
            center.X += 2;

            // Cone.
            Point3D[] circle = G3.MakePolygonPoints(20, center, D3.XVector(1), D3.ZVector(-1));
            mesh1.AddCone(center, circle, D3.YVector(4));
            center.X += 2;

            // Cone frustum.
            circle = G3.MakePolygonPoints(20, center, D3.XVector(1), D3.ZVector(-1));
            mesh1.AddConeFrustum(center, circle, D3.YVector(4), 3);
            center.X += 2;

            // Cone frustum w/cutting plane.
            circle = G3.MakePolygonPoints(20, center, D3.XVector(1), D3.ZVector(-1));
            mesh1.AddConeFrustum(center, circle, D3.YVector(4),
                center + new Vector3D(0, 2, 0), new Vector3D(0, 1, 1));
            center.X += 2;

            // Start a new row.
            center.X = x0;
            center.Z -= 3;

            // Cube.
            MeshGeometry3D cubeMesh = new MeshGeometry3D();
            cubeMesh.AddCube();
            cubeMesh.ApplyTransformation(new ScaleTransform3D(1, 1.5, 0.5));
            cubeMesh.ApplyTransformation(
                new TranslateTransform3D(center.X, center.Y + 1.5, 0));
            mesh3.Merge(cubeMesh);
            center.X += 2;

            // Cylinder.
            Point3D[] polygon = G3.MakePolygonPoints(7, center, D3.XVector(1), D3.ZVector(-1));
            mesh1.AddCylinder(polygon, new Vector3D(0, 2, -1));
            center.X += 2;

            // Cylinder.
            circle = G3.MakePolygonPoints(20, center, D3.XVector(1), D3.ZVector(-1));
            mesh1.AddCylinder(circle, new Vector3D(0, 2, -1), true);
            center.X += 2;

            // Cylinder w/cutting planes.
            polygon = G3.MakePolygonPoints(7, center, D3.XVector(1), D3.ZVector(-1));
            mesh1.AddCylinder(polygon, new Vector3D(0, 1, 0),
                center + new Vector3D(0, 2, 0), new Vector3D(0, 2, 1),
                center + new Vector3D(0, -2, 0), new Vector3D(0, 2, -1));
            center.X += 2;

            // Cylinder w/cutting planes.
            polygon = G3.MakePolygonPoints(7, center, D3.XVector(1), D3.ZVector(-1));
            mesh1.AddCylinder(polygon, new Vector3D(0, 1, 0),
                center + new Vector3D(0, 2, 0), new Vector3D(0, 2, 1),
                center + new Vector3D(0, -2, 0), new Vector3D(0, 2, -1), true);
            center.X += 2;

            // Start a new row.
            center.X = x0;
            center.Z -= 3;

            // Dodecahedron.
            MeshGeometry3D dodMesh = new MeshGeometry3D();
            dodMesh.AddDodecahedron();
            double dodScale = 1 / G3.DodecahedronCircumradius();
            dodMesh.ApplyTransformation(new ScaleTransform3D(dodScale, dodScale, dodScale));
            dodMesh.ApplyTransformation(
                new TranslateTransform3D(center.X, center.Y + 0.5, center.Z));
            mesh3.Merge(dodMesh);
            center.X += 2;

            // Frustum.
            polygon = G3.MakePolygonPoints(5, center, D3.XVector(1), D3.ZVector(-1));
            mesh1.AddFrustum(center, polygon, D3.YVector(4), 2);
            center.X += 2;

            // Icosahedron.
            MeshGeometry3D icoMesh = new MeshGeometry3D();
            icoMesh.AddIcosahedron();
            double icoScale = 1 / G3.IcosahedronCircumradius();
            icoMesh.ApplyTransformation(new ScaleTransform3D(icoScale, icoScale, icoScale));
            icoMesh.ApplyTransformation(
                new TranslateTransform3D(center.X, center.Y + 0.5, center.Z));
            mesh3.Merge(icoMesh);
            center.X += 2;

            // Octahedron.
            MeshGeometry3D octMesh = new MeshGeometry3D();
            octMesh.AddOctahedron();
            double octScale = 1 / G3.OctahedronCircumradius();
            octMesh.ApplyTransformation(new ScaleTransform3D(octScale, octScale, octScale));
            octMesh.ApplyTransformation(
                new TranslateTransform3D(center.X, center.Y + 0.5, center.Z));
            mesh3.Merge(octMesh);
            center.X += 2;

            // Pyramid.
            polygon = G3.MakePolygonPoints(6, center, D3.XVector(1), D3.ZVector(-1));
            mesh1.AddPyramid(center, polygon, D3.YVector(3));
            center.X += 2;

            // Start a new row.
            center.X = x0;
            center.Z -= 3;

            // Sphere.
            mesh1.AddSphere(center + new Vector3D(0, 1, 0), 1, 20, 10);
            center.X += 2;

            // Tetrahedron.
            MeshGeometry3D tetMesh = new MeshGeometry3D();
            tetMesh.AddTetrahedron();
            double tetScale = 1 / G3.TetrahedronCircumradius();
            tetMesh.ApplyTransformation(new ScaleTransform3D(tetScale, tetScale, tetScale));
            tetMesh.ApplyTransformation(
                new TranslateTransform3D(center.X, center.Y + 0.5, center.Z));
            mesh3.Merge(tetMesh);
            center.X += 2;

            // Textured sphere.
            mesh4.Positions.Add(new Point3D());
            mesh4.TextureCoordinates.Add(new Point(1.01, 1.01));
            mesh4.AddTexturedSphere(center + new Vector3D(0, 1, 0), 1, 20, 10);
            center.X += 2;

            // Textured torus.
            mesh4.AddTexturedTorus(center + new Vector3D(0, 1, 0), 0.6, 0.4, 30, 15);
            center.X += 2;

            // Torus.
            mesh1.AddTorus(center + new Vector3D(0, 1, 0), 0.6, 0.4, 30, 15);
            center.X += 2;

            // Start a new row.
            center.X = x0;
            center.Z -= 3;

            group.Children.Add(mesh1.MakeModel(Brushes.LightGreen));
            group.Children.Add(mesh2.MakeModel("wrapper.png"));
            group.Children.Add(mesh3.MakeModel(Brushes.LightBlue));
            group.Children.Add(mesh4.MakeModel("world.jpg"));
        }
    }
}
