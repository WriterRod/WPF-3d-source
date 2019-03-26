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

namespace FractalLandscape
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
            // Make the initial surface.
            int numX = 10;
            int numZ = 10;
            Point3D[,] surface = G3.InitSurface(0, numX, -3, 3, numZ, -3, 3);

            // Add some interest.
            Random rand = new Random(0);
            for (int ix = 0; ix < numX; ix++)
            {
                for (int iz = 0; iz < numZ; iz++)
                {
                    double x = surface[ix, iz].X;
                    double z = surface[ix, iz].Z;
                    surface[ix, iz].Y = 1 - Math.Sin(x) + Math.Sin(z) / 5;
                }
            }

            // Fractalize.
            surface = G3.FractalizeSurface(surface, 4, 12345, -0.4, 0.4);

            // Limit Y.
            G3.LimitY(surface, minY: 0.15);

            // Make the mesh.
            MeshGeometry3D mesh1 = new MeshGeometry3D();
            mesh1.AddSurface(surface);

            // Apply a height map.
            double minY = surface[0, 0].Y;
            double maxY = minY;
            foreach (Point3D point in surface)
            {
                if (minY > point.Y) minY = point.Y;
                if (maxY < point.Y) maxY = point.Y;
            }
            mesh1.ApplyHeightMap(0, 1, minY, maxY);

            GradientStopCollection stops = new GradientStopCollection();
            stops.Add(new GradientStop(Colors.Blue, 0));
            stops.Add(new GradientStop(Colors.Blue, 0.001));
            stops.Add(new GradientStop(Colors.Chartreuse, 0.002));
            stops.Add(new GradientStop(Colors.Chartreuse, 0.2));
            stops.Add(new GradientStop(Colors.OliveDrab, 0.4));
            stops.Add(new GradientStop(Colors.Gray, 0.7));
            stops.Add(new GradientStop(Colors.White, 0.9));
            stops.Add(new GradientStop(Colors.White, 1));
            LinearGradientBrush brush =
                new LinearGradientBrush(stops, new Point(0, 0), new Point(1, 1));

            GeometryModel3D model = mesh1.MakeModel(brush);
            model.BackMaterial = new DiffuseMaterial(Brushes.Gray);
            group.Children.Add(model);
        }
    }
}
