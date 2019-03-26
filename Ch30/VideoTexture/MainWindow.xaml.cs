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

namespace VideoTexture
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

            bearMediaElement.Play();
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
            group.Children.Add(new AmbientLight(Colors.White));
        }

        // Define the model.
        private void DefineModel()
        {
            MeshGeometry3D mesh = new MeshGeometry3D();
            mesh.AddSurface(Quadratic,
                -2, 2, 0, 1, 20,
                -2, 2, 0, 1, 20, true);

            VisualBrush brush = new VisualBrush(bearMediaElement);
            DiffuseMaterial mat = new DiffuseMaterial(brush);
            GeometryModel3D model = new GeometryModel3D(mesh, mat);
            MainGroup.Children.Add(model);
        }

        // Return a function F(x, z);
        private Point3D Quadratic(double x, double z)
        {
            double y = 3.0 - (x * x + z * z) / 5.0;
            return new Point3D(x, y, z);
        }

        private void bearMediaElement_MediaEnded(object sender, RoutedEventArgs e)
        {
            bearMediaElement.Position = TimeSpan.Zero;
            bearMediaElement.Play();
        }
    }
}
