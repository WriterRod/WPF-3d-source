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

namespace Moonscape
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
            int numX = 40;
            int numZ = 40;
            Point3D[,] surface = G3.InitSurface(0, numX, -3, 3, numZ, -3, 3);

            // Add some craters.
            AddCrater(surface, -1, 0, 1.5, 0.3);
            AddCrater(surface, 1.6, -1.5, 0.5, 0.4);
            AddCrater(surface, 1.5, 1.5, 0.75, 0.3);
            AddCrater(surface, 0.5, -0.6, 1.25, 0.1);

            // Add some relatively large-scale randomness to random points.
            Random rand = new Random(0);
            for (int i = 0; i < 10; i++)
            {
                int ix = rand.Next(0, numX);
                int iz = rand.Next(0, numZ);
                surface[ix, iz].Y += rand.NextDouble(-0.1, 0.1);
            }

            // Fractalize.
            surface = G3.FractalizeSurface(surface, 2, 1, -0.05, 0.05);

            // Translate to center better.
            TranslateTransform3D trans = new TranslateTransform3D(0, 1, 0);
            numX = surface.GetUpperBound(0) + 1;
            numZ = surface.GetUpperBound(1) + 1;
            for (int ix = 0; ix < numX; ix++)
                for (int iz = 0; iz < numZ; iz++)
                    surface[ix, iz] = trans.Transform(surface[ix, iz]);

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
            stops.Add(new GradientStop(Colors.Gray, 0));
            stops.Add(new GradientStop(Colors.LightGray, 0.25));
            stops.Add(new GradientStop(Colors.LightGray, 0.75));
            stops.Add(new GradientStop(Colors.White, 1));
            LinearGradientBrush brush =
                new LinearGradientBrush(stops, new Point(0, 0), new Point(1, 1));
            group.Children.Add(mesh1.MakeModel(brush));
        }

        // Add a crater.
        // If a point is within radius of the X and Z coordinates (cx, cz)
        // and the point's Y coordinate is greater than y, set it to y.
        private void AddCrater(Point3D[,] surface, double cx, double cz,
            double radius, double yscale)
        {
            int numX = surface.GetUpperBound(0) + 1;
            int numZ = surface.GetUpperBound(1) + 1;
            for (int ix = 0; ix < numX; ix++)
            {
                for (int iz = 0; iz < numZ; iz++)
                {
                    double dx = surface[ix, iz].X - cx;
                    double dz = surface[ix, iz].Z - cz;
                    double y = -yscale * Math.Sqrt(radius * radius - (dx * dx + dz * dz));
                    if (surface[ix, iz].Y > y) surface[ix, iz].Y = y;
                }
            }
        }
    }
}
