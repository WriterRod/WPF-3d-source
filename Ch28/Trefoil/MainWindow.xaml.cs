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

namespace Trefoil
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
            Model3DGroup group = new Model3DGroup();
            visual3d.Content = group;
            mainViewport.Children.Add(visual3d);

            // Define the camera, lights, and model.
            DefineCamera(mainViewport);
            DefineLights(group);
            DefineModel(group);
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

            // Make the generator.
            double r = 0.5;
            Point3D[] generator = G3.MakePolygonPoints(25, D3.Origin, D3.XVector(r), D3.ZVector(r));
            int numGen = generator.Length + 1;
            Array.Resize(ref generator, numGen);
            generator[numGen - 1] = generator[0];

            // Make the path.
            Point3D[] path = G3.MakeTrefoilPoints(100, D3.Origin, D3.XVector(), D3.ZVector());

            // Repeat the first three points.
            int numPath = path.Length + 3;
            Array.Resize(ref path, numPath);
            path[numPath - 3] = path[0];
            path[numPath - 2] = path[1];
            path[numPath - 1] = path[2];

            // Make the surface.
            MeshGeometry3D mesh1 = new MeshGeometry3D();
            mesh1.AddPathSurface(generator, path, D3.ZVector(), false, false, false);
            group.Children.Add(mesh1.MakeModel(Brushes.LightBlue));

            // Make a wireframe.
            HashSet<Edge> edges = new HashSet<Edge>();
            MeshGeometry3D mesh2 = new MeshGeometry3D();
            mesh2.AddPathSurface(generator, path, D3.ZVector(),
                false, false, false, edges, 0.02);
            group.Children.Add(mesh2.MakeModel(Brushes.Blue));
        }
    }
}
