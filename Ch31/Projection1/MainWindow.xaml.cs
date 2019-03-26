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

namespace Projection1
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

        // The main model group.
        private Model3DGroup MainGroup;

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // Define WPF objects.
            ModelVisual3D visual3d = new ModelVisual3D();
            MainGroup = new Model3DGroup();
            visual3d.Content = MainGroup;
            mainViewport.Children.Add(visual3d);

            // Define the camera, lights, and model.
            DefineCamera(mainViewport);
            DefineLights(MainGroup);
            DefineModel();
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
            group.Children.Add(new DirectionalLight(dark, new Vector3D(-1, -1, -1)));
            group.Children.Add(new DirectionalLight(dark, new Vector3D(-1, 0, 1)));
            group.Children.Add(new DirectionalLight(dark, new Vector3D(0, 0, -1)));
        }

        // Define the model.
        private void DefineModel()
        {
            // Axes.
            MeshExtensions.AddAxes(MainGroup);

            // Make the objects.
            MeshGeometry3D cameraMesh, boxMesh, edgeMesh;
            Point3D cameraCenter;
            MakeObjects(out cameraMesh, out boxMesh, out edgeMesh, out cameraCenter);
        }

        private void MakeObjects(out MeshGeometry3D cameraMesh,
            out MeshGeometry3D boxMesh, out MeshGeometry3D edgeMesh,
            out Point3D cameraCenter)
        {
            // Make a "camera."
            cameraMesh = new MeshGeometry3D();
            cameraMesh.AddBox(new Point3D(-0.6, -0.5, -0.2),
                D3.XVector(1.2), D3.YVector(1), D3.ZVector(0.4));
            Point3D[] points = G3.MakePolygonPoints(20, new Point3D(0, 0, -0.7),
                D3.XVector(0.3), D3.YVector(0.3));
            cameraMesh.AddCylinder(points, D3.ZVector(0.7), true);
            points = G3.MakePolygonPoints(20, new Point3D(0, 0, -0.8),
                D3.XVector(0.4), D3.YVector(0.4));
            cameraMesh.AddCylinder(points, D3.ZVector(0.2), true);
            cameraMesh.AddBox(new Point3D(0.3, 0.5, -0.1),
                D3.XVector(0.2), D3.YVector(0.2), D3.ZVector(0.2));
            MainGroup.Children.Add(cameraMesh.MakeModel(Brushes.LightBlue));

            // Transform the camera and vector.
            Transform3DGroup trans = new Transform3DGroup();
            RotateTransform3D r1 = D3.Rotate(D3.XVector(), D3.Origin, -45);
            trans.Children.Add(r1);
            RotateTransform3D r2 = D3.Rotate(D3.YVector(), D3.Origin, -45);
            trans.Children.Add(r2);

            // See where we need to translate to make the camera point at the origin.
            Point3D lookAtPoint = new Point3D(0, 0, -3.5);
            lookAtPoint = trans.Transform(lookAtPoint);

            TranslateTransform3D t1 = new TranslateTransform3D(
                -lookAtPoint.X, -lookAtPoint.Y, -lookAtPoint.Z);
            trans.Children.Add(t1);
            cameraMesh.ApplyTransformation(trans);

            cameraCenter = trans.Transform(D3.Origin);

            // Make a target box.
            boxMesh = new MeshGeometry3D();
            boxMesh.AddBox(new Point3D(-0.75, -0.75, -0.75),
                D3.XVector(1.5), D3.YVector(1.5), D3.ZVector(1.5));
            MainGroup.Children.Add(boxMesh.MakeModel(Brushes.LightGreen));

            // Make the box's edges.
            edgeMesh = new MeshGeometry3D();
            HashSet<Edge> edges = new HashSet<Edge>();
            edgeMesh.AddBox(new Point3D(-0.75, -0.75, -0.75),
                D3.XVector(1.5), D3.YVector(1.5), D3.ZVector(1.5), edges: edges);
            MainGroup.Children.Add(edgeMesh.MakeModel(Brushes.Black));
        }
    }
}
