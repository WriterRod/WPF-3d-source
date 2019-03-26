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

namespace LabelPoints
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

        // The point labels and their locations in world coordinates.
        private Point3D[] LabelLocations =
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
        private Label[] PointLabels;

        // Position the point labels.
        private void CameraController_CameraPositionChanged(object sender, EventArgs e)
        {
            PositionLabels();
        }
        private void PositionLabels()
        {
            Point[] points = Cameras.Convert3DPoints(LabelLocations, mainViewport);
            for (int i = 0; i < LabelLocations.Length; i++)
            {
                Canvas.SetLeft(PointLabels[i], points[i].X);
                Canvas.SetTop(PointLabels[i], points[i].Y);
            }
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
        private void DefineModel()
        {
            // Make a cube.
            MeshGeometry3D mesh = new MeshGeometry3D();
            mesh.AddBox(new Point3D(-2, -2, -2), D3.XVector(4), D3.YVector(4), D3.ZVector(4));
            MainGroup.Children.Add(mesh.MakeModel(Brushes.LightBlue));

            // Create labels for the points.
            PointLabels = new Label[LabelLocations.Length];
            for (int i = 0; i < LabelLocations.Length; i++)
            {
                PointLabels[i] = new Label();
                foreCanvas.Children.Add(PointLabels[i]);
                PointLabels[i].Content = string.Format("({0}, {1}, {2})",
                    LabelLocations[i].X,
                    LabelLocations[i].Y,
                    LabelLocations[i].Z);
                PointLabels[i].Background =
                    new SolidColorBrush(Color.FromArgb(64, 255, 255, 255));
                PointLabels[i].FontWeight = FontWeights.Bold;
            }
            PositionLabels();
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (this.IsLoaded) PositionLabels();
        }
    }
}
