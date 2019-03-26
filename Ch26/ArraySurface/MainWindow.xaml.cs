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

namespace ArraySurface
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
            //MeshExtensions.AddAxes(group);

            // Make some points to define a surface.
            const double xmin = -4;
            const double xmax = -xmin;
            const double zmin = xmin;
            const double zmax = xmax;
            int numX = 20;
            int numZ = 20;
            Point3D[,] points = new Point3D[numX + 1, numZ + 1];
            double dx = (xmax - xmin) / numX;
            double dz = (zmax - zmin) / numZ;
            double x = xmin;
            for (int ix = 0; ix <= numX; ix++)
            {
                double z = zmin;
                for (int iz = 0; iz <= numZ; iz++)
                {
                    points[ix, iz] = new Point3D(x, 1 + Math.Sin(2 * x) / 2 + Math.Sin(2 * z) / 2, z);
                    z += dz;
                }
                x += dx;
            }

            // Add a surface.
            MeshGeometry3D mesh1 = new MeshGeometry3D();
            mesh1.AddSurface(points, true);
            group.Children.Add(mesh1.MakeModel(Brushes.LightBlue));

            // Add a wireframe.
            HashSet<Edge> edges = new HashSet<Edge>();
            MeshGeometry3D mesh2 = new MeshGeometry3D();
            mesh2.AddSurface(points, true, edges, 0.02);
            group.Children.Add(mesh2.MakeModel(Brushes.Blue));
        }
    }
}
