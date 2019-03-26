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

namespace RotationSurface
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
            const int numPoints = 20;
            const double tmin = 1.1 * Math.PI;
            const double tmax = tmin + 1.8 * Math.PI;
            Point3D[] generator = new Point3D[numPoints];
            double t = tmin;
            double dt = (tmax - tmin) / (numPoints - 1);
            for (int i = 0; i < numPoints; i++)
            {
                generator[i] = new Point3D(1 + 0.5 * Math.Sin(t), 3 - t * 0.5, 0);
                t += dt;
            }

            // Make the transformation.
            const int numRot = 20;
            const double dangle = 360.0 / numRot;
            RotateTransform3D rot = D3.Rotate(D3.YVector(), D3.Origin, dangle);

            // Make the surface of transformation.
            MeshGeometry3D mesh1 = new MeshGeometry3D();
            mesh1.AddTransformSurface(generator, rot, numRot,
                false, false, true, true,
                false);
            group.Children.Add(mesh1.MakeModel(Brushes.LightBlue));

            // Make a wireframe.
            HashSet<Edge> edges = new HashSet<Edge>();
            MeshGeometry3D mesh2 = new MeshGeometry3D();
            mesh2.AddTransformSurface(generator, rot, numRot,
                false, false, true, true,
                false, edges, 0.02);
            group.Children.Add(mesh2.MakeModel(Brushes.Blue));

            // Display the generator.
            edges = new HashSet<Edge>();
            MeshGeometry3D mesh3 = new MeshGeometry3D();
            for (int i = 1; i < numPoints; i++)
                mesh3.AddEdge(edges, 0.1, generator[i - 1], generator[i]);
            group.Children.Add(mesh3.MakeModel(Brushes.Black));
        }
    }
}
