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

namespace CirclePoints
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

            // Catch the camera controller's CameraPositionChanged event.
            CameraController.CameraPositionChanged += CameraController_CameraPositionChanged;
        }

        // The points' locations and circles.
        private Point3D[] PointLocations =
        {
            new Point3D(2, 2, 2),
            new Point3D(2, 2, -2),
            new Point3D(2, -2, 2),
            new Point3D(2, -2, -2),
            new Point3D(-2, 2, 2),
            new Point3D(-2, 2, -2),
            new Point3D(-2, -2, 2),
            new Point3D(-2, -2, -2),
        };
        private Ellipse[] PointCircles;

        // Position the point labels.
        private void CameraController_CameraPositionChanged(object sender, EventArgs e)
        {
            PositionCircles();
        }
        private void PositionCircles()
        {
            Point[] points = Cameras.Convert3DPoints(PointLocations, mainViewport);
            for (int i = 0; i < PointLocations.Length; i++)
            {
                Canvas.SetLeft(PointCircles[i], points[i].X - 10);
                Canvas.SetTop(PointCircles[i], points[i].Y - 10);
            }
        }

        // Define the lights.
        private void DefineLights(Model3DGroup group)
        {
            Color darker = Color.FromArgb(255, 96, 96, 96);
            Color dark = Color.FromArgb(255, 128, 128, 128);

            group.Children.Add(new AmbientLight(darker));

            group.Children.Add(new DirectionalLight(dark, new Vector3D(0, -1, 0)));
            group.Children.Add(new DirectionalLight(dark, new Vector3D(1, -3, -2)));
            group.Children.Add(new DirectionalLight(dark, new Vector3D(-1, 3, 2)));
        }

        // Define the model.
        private void DefineModel()
        {
            // Make a cube.
            MeshGeometry3D mesh = new MeshGeometry3D();
            mesh.AddBox(new Point3D(-2, -2, -2), D3.XVector(4), D3.YVector(4), D3.ZVector(4));
            MainGroup.Children.Add(mesh.MakeModel(Brushes.LightBlue));

            // Create circles for the points.
            PointCircles = new Ellipse[PointLocations.Length];
            for (int i = 0; i < PointLocations.Length; i++)
            {
                PointCircles[i] = new Ellipse();
                foreCanvas.Children.Add(PointCircles[i]);
                PointCircles[i].Width = 20;
                PointCircles[i].Height = 20;
                PointCircles[i].Stroke = Brushes.Red;
                PointCircles[i].StrokeThickness = 5;
            }
            PositionCircles();
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (this.IsLoaded) PositionCircles();
        }
    }
}
