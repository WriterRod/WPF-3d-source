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

namespace HexDonut
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
            Point3D[] generator = G3.MakePolygonPoints(6, D3.Origin, D3.XVector(1), D3.ZVector(1));

            // Repeat the first point at the end to make it a closed tube.
            int numGen = generator.Length + 1;
            Array.Resize(ref generator, numGen);
            generator[numGen - 1] = generator[0];

            // Make the path.
            Point3D[] path = G3.MakePolygonPoints(8, D3.Origin, D3.XVector(2), D3.YVector(2));

            // Mark the path. (Only visible if you hide the surface.)
            HashSet<Edge> edges = new HashSet<Edge>();
            MeshGeometry3D mesh3 = new MeshGeometry3D();
            mesh3.AddPolygonEdges(edges, 0.2, path);
            for (int i = 0; i < path.Length; i++)
                mesh3.AddSphere(path[i], 0.2, 20, 10, true);
            group.Children.Add(mesh3.MakeModel(Brushes.Red));

            // Repeat the first three points to close the tube.
            //int numPath = path.Length + 3;
            //Array.Resize(ref path, numPath);
            //path[numPath - 3] = path[0];
            //path[numPath - 2] = path[1];
            //path[numPath - 1] = path[2];

            // Make the surface.
            MeshGeometry3D mesh1 = new MeshGeometry3D();
            mesh1.AddPathSurface(generator, path, D3.ZVector(), true, true, false);
            //group.Children.Add(mesh1.MakeModel(Brushes.LightBlue));

            // Make a wireframe.
            MeshGeometry3D mesh2 = new MeshGeometry3D();
            mesh2.AddPathSurface(generator, path, D3.ZVector(),
                false, false, false, edges, 0.02);
            group.Children.Add(mesh2.MakeModel(Brushes.Blue));
        }
    }
}
