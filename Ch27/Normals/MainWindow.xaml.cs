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

namespace Normals
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
            if (quadraticRadioButton.IsChecked.Value)
            {
                mesh1.AddSurface(Quadratic, -3, 3, 20, -3, 3, 20, true);
                if (wireframeCheckBox.IsChecked.Value)
                    mesh1a.AddSurface(Quadratic, -3, 3, 20, -3, 3, 20, false, edges, 0.02);
            }
            else if (splashRadioButton.IsChecked.Value)
            {
                mesh1.AddSurface(Splash, -3, 3, 40, -3, 3, 40, true);
                if (wireframeCheckBox.IsChecked.Value)
                    mesh1a.AddSurface(Splash, -3, 3, 40, -3, 3, 40, false, edges, 0.02);
            }
            else if (strangeRadioButton.IsChecked.Value)
            {
                mesh1.AddSurface(Strange, -3, 3, 35, -3, 3, 35, true);
                if (wireframeCheckBox.IsChecked.Value)
                    mesh1a.AddSurface(Strange, -3, 3, 35, -3, 3, 35, false, edges, 0.02);
            }
            else if (twisterRadioButton.IsChecked.Value)
            {
                mesh1.AddSurface(Twister, -3, 3, 20, -3, 3, 20, true);
                if (wireframeCheckBox.IsChecked.Value)
                    mesh1a.AddSurface(Twister, -3, 3, 20, -3, 3, 20, false, edges, 0.02);
            }

            GeometryModel3D model = new GeometryModel3D(mesh1, null);
            if (frontfacesCheckBox.IsChecked.Value)
                model.Material = new DiffuseMaterial(Brushes.LightBlue);
            if (backfacesCheckBox.IsChecked.Value)
                model.BackMaterial = new DiffuseMaterial(Brushes.Gray);
            ModelGroup.Children.Add(model);

            if (wireframeCheckBox.IsChecked.Value)
                ModelGroup.Children.Add(mesh1a.MakeModel(Brushes.Blue));

            if (normalsCheckBox.IsChecked.Value)
            {
                MeshGeometry3D mesh1b = mesh1.ToNormals(0.01, 1);
                ModelGroup.Children.Add(mesh1b.MakeModel(Brushes.Red));
            }
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
