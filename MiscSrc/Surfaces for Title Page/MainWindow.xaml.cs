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

namespace Surfaces
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
        private Model3DGroup ModelGroup = null;

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // Define WPF objects.
            ModelVisual3D visual3d = new ModelVisual3D();
            ModelGroup = new Model3DGroup();
            visual3d.Content = ModelGroup;
            mainViewport.Children.Add(visual3d);

            // Define the camera, lights, and model.
            DefineCamera(mainViewport);
            DefineLights(ModelGroup);
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
            group.Children.Add(new DirectionalLight(dark, new Vector3D(1, -3, -2)));
            group.Children.Add(new DirectionalLight(dark, new Vector3D(-1, 3, 2)));
        }

        // Define the model.
        private void DefineModel()
        {
            // Remove non-light models.
            for (int i = ModelGroup.Children.Count - 1; i >= 0; i--)
            {
                if (!(ModelGroup.Children[i] is Light))
                    ModelGroup.Children.RemoveAt(i);
            }

            // Axes.
            if (axesCheckBox.IsChecked.Value)
                MeshExtensions.AddAxes(ModelGroup);

            // Add the selected surface.
            HashSet<Edge> edges = new HashSet<Edge>();
            MeshGeometry3D mesh1 = new MeshGeometry3D();
            MeshGeometry3D mesh1a = new MeshGeometry3D();
            const double xmin = -3.5;
            const double xmax = 3.5;
            const double zmin = -3.5;
            const double zmax = 3.5;
            const double thickness = 0.01;
            if (quadraticRadioButton.IsChecked.Value)
            {
                mesh1.AddSurface(Quadratic, xmin, xmax, 20, zmin, zmax, 20, true);
                mesh1a.AddSurface(Quadratic, xmin, xmax, 20, zmin, zmax, 20, false, edges, thickness);
            }
            else if (splashRadioButton.IsChecked.Value)
            {
                if (wireframeCheckBox.IsChecked.Value)
                {
                    mesh1.AddSurface(Splash, xmin, xmax, 40, zmin, zmax, 40, true);
                    mesh1a.AddSurface(Splash, xmin, xmax, 40, zmin, zmax, 40, false, edges, thickness);
                }
                else
                    mesh1.AddSurface(Splash, xmin, xmax, 100, zmin, zmax, 100, true);
            }
            else if (strangeRadioButton.IsChecked.Value)
            {
                if (wireframeCheckBox.IsChecked.Value)
                {
                    mesh1.AddSurface(Strange, xmin, xmax, 40, zmin, zmax, 40, true);
                    mesh1a.AddSurface(Strange, xmin, xmax, 40, zmin, zmax, 40, false, edges, thickness);
                }
                else
                    mesh1.AddSurface(Strange, xmin, xmax, 60, zmin, zmax, 60, true);
            }
            else if (twisterRadioButton.IsChecked.Value)
            {
                if (wireframeCheckBox.IsChecked.Value)
                {
                    mesh1.AddSurface(Twister, xmin, xmax, 20, zmin, zmax, 20, true);
                    mesh1a.AddSurface(Twister, xmin, xmax, 20, zmin, zmax, 20, false, edges, thickness);
                }
                else
                    mesh1.AddSurface(Twister, xmin, xmax, 20, zmin, zmax, 20, true);
            }

            ModelGroup.Children.Add(mesh1.MakeModel(Brushes.LightBlue));
            if (wireframeCheckBox.IsChecked.Value)
                ModelGroup.Children.Add(mesh1a.MakeModel(Brushes.Blue));
        }

        // The surface-generating methods.
        private Point3D Quadratic(double x, double z)
        {
            double y = 3.0 - (x * x + z * z) / 5.0;
            return new Point3D(x, y, z);
        }

        private Point3D Splash(double x, double z)
        {
            double r2 = x * x + z * z;
            double y = 1 + 4 * Math.Cos(r2) / (2 + r2);
            return new Point3D(x, y, z);
        }

        private Point3D Strange(double x, double z)
        {
            double r2 = (x * x + z * z) / 4;
            double r = Math.Sqrt(r2);

            double theta = Math.Atan2(z, x);
            double y = 3 * Math.Exp(-r2) * Math.Sin(2 * Math.PI * r) * Math.Cos(3 * theta);
            return new Point3D(x, y, z);
        }

        private Point3D Twister(double u, double v)
        {
            double r2 = (u * u + v * v) / 4;
            double r = Math.Sqrt(r2);

            double y = 2 - 1 / (r2 + 0.0001);
            double x = u * Math.Cos(r) - v * Math.Sin(r);
            double z = u * Math.Sin(r) + v * Math.Cos(r);
            return new Point3D(x, y, z);
        }

        // The user changed an option. Display the selected surface.
        private void Option_Click(object sender, RoutedEventArgs e)
        {
            DefineModel();
        }
    }
}
