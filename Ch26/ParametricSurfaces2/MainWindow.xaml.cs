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

namespace ParametricSurfaces2
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
            double pi = Math.PI;
            if (bonbonRadioButton.IsChecked.Value)
            {
                mesh1.AddSurface(Bonbon, -pi, pi, 20, -pi, pi, 20, true);
                if (wireframeCheckBox.IsChecked.Value)
                    mesh1a.AddSurface(Bonbon, -pi, pi, 20, -pi, pi, 20, false, edges, 0.02);
            }
            else if (catalanRadioButton.IsChecked.Value)
            {
                mesh1.AddSurface(Catalan, -pi, pi, 40, -2, 2, 20, true);
                if (wireframeCheckBox.IsChecked.Value)
                    mesh1a.AddSurface(Catalan, -pi, pi, 40, -2, 2, 20, false, edges, 0.02);
            }
            else if (catenoidRadioButton.IsChecked.Value)
            {
                mesh1.AddSurface(Catenoid, -pi, pi, 25, -pi, pi, 25, true);
                if (wireframeCheckBox.IsChecked.Value)
                    mesh1a.AddSurface(Catenoid, -pi, pi, 25, -pi, pi, 25, false, edges, 0.02);
            }
            else if (cosinusRadioButton.IsChecked.Value)
            {
                mesh1.AddSurface(Cosinus, -1, 1, 25, -1, 1, 25, true);
                if (wireframeCheckBox.IsChecked.Value)
                    mesh1a.AddSurface(Cosinus, -1, 1, 25, -1, 1, 25, false, edges, 0.02);
            }
            else if (enneperRadioButton.IsChecked.Value)
            {
                mesh1.AddSurface(Enneper, -2, 2, 50, -2, 2, 50, true);
                if (wireframeCheckBox.IsChecked.Value)
                    mesh1a.AddSurface(Enneper, -2, 2, 50, -2, 2, 50, false, edges, 0.02);
            }
            else if (helicoidalRadioButton.IsChecked.Value)
            {
                mesh1.AddSurface(Helicoidal, -pi, pi, 50, -pi, pi, 50, true);
                if (wireframeCheckBox.IsChecked.Value)
                    mesh1a.AddSurface(Helicoidal, -pi, pi, 50, -pi, pi, 50, false, edges, 0.02);
            }
            else if (helixRadioButton.IsChecked.Value)
            {
                mesh1.AddSurface(Helix, 0, 6 * pi, 100, 0, 2 * pi, 10, true);
                if (wireframeCheckBox.IsChecked.Value)
                    mesh1a.AddSurface(Helix, 0, 6 * pi, 100, 0, 2 * pi, 10, false, edges, 0.02);
            }
            else if (hexaedronRadioButton.IsChecked.Value)
            {
                mesh1.AddSurface(Hexaedron, -pi / 2, pi / 2, 20, 0, 2 * pi, 30, true);
                if (wireframeCheckBox.IsChecked.Value)
                    mesh1a.AddSurface(Hexaedron, -pi / 2, pi / 2, 20, 0, 2 * pi, 30, false, edges, 0.02);
            }
            else if (hyperhelicoidalRadioButton.IsChecked.Value)
            {
                mesh1.AddSurface(Hyperhelicoidal, -2 * pi, 2 * pi, 100, -pi, pi, 30, true);
                if (wireframeCheckBox.IsChecked.Value)
                    mesh1a.AddSurface(Hyperhelicoidal, -2 * pi, 2 * pi, 100, -pi, pi, 30, false, edges, 0.02);
            }
            else if (kleinRadioButton.IsChecked.Value)
            {
                mesh1.AddSurface(Klein, 0, 2 * pi, 20, 0, 2 * pi, 30, true);
                if (wireframeCheckBox.IsChecked.Value)
                    mesh1a.AddSurface(Klein, 0, 2 * pi, 20, 0, 2 * pi, 30, false, edges, 0.02);
            }
            else if (shellRadioButton.IsChecked.Value)
            {
                mesh1.AddSurface(Shell, 0, pi, 20, -pi, 2.5 * pi, 30, true);
                if (wireframeCheckBox.IsChecked.Value)
                    mesh1a.AddSurface(Shell, 0, pi, 20, -pi, 2.5 * pi, 30, false, edges, 0.02);
            }
            else if (torusRadioButton.IsChecked.Value)
            {
                mesh1.AddSurface(Torus, 0, 2 * pi, 20, 0, 2 * pi, 30, true);
                if (wireframeCheckBox.IsChecked.Value)
                    mesh1a.AddSurface(Torus, 0, 2 * pi, 20, 0, 2 * pi, 30, false, edges, 0.02);
            }
            else if (sphereRadioButton.IsChecked.Value)
            {
                mesh1.AddSurface(Sphere, 0, pi, 10, 0, 2 * pi, 20, true);
                if (wireframeCheckBox.IsChecked.Value)
                    mesh1a.AddSurface(Sphere, 0, pi, 10, 0, 2 * pi, 20, false, edges, 0.02);
            }
            else if (breatherRadioButton.IsChecked.Value)
            {
                mesh1.AddSurface(Breather, -13.2, 13.2, 50, -37.4, 37.4, 60, true);
                if (wireframeCheckBox.IsChecked.Value)
                    mesh1a.AddSurface(Breather, -13.2, 13.2, 50, -37.4, 37.4, 60, false, edges, 0.02);
            }

            GeometryModel3D model = new GeometryModel3D(mesh1, null);
            if (frontfacesCheckBox.IsChecked.Value)
                model.Material = new DiffuseMaterial(Brushes.LightBlue);
            if (backfacesCheckBox.IsChecked.Value)
                model.BackMaterial = new DiffuseMaterial(Brushes.Gray);
            ModelGroup.Children.Add(model);

            if (wireframeCheckBox.IsChecked.Value)
                ModelGroup.Children.Add(mesh1a.MakeModel(Brushes.Blue));
        }

        // The surface-generating methods mostly from:
        // https://wiki.blender.org/index.php/Extensions:2.6/Py/Scripts/Add_Mesh/Add_3d_Function_Surface
        private Point3D Bonbon(double u, double v)
        {
            double x = u;
            double y = Math.Cos(u) * Math.Sin(v);
            double z = Math.Cos(u) * Math.Cos(v);

            // Invert y for outward orientation.
            return new Point3D(x, -y, z);
        }

        private Point3D Catalan(double u, double v)
        {
            double x = u - Math.Sin(u) * Math.Cosh(v);
            double y = 4 * Math.Sin(u / 2) * Math.Sinh(v / 2);
            double z = 1 - Math.Cos(u) * Math.Cosh(v);

            return new Point3D(x / 2, y / 2, z / 2);
        }

        private Point3D Catenoid(double u, double v)
        {
            double x = 2 * Math.Cosh(v / 2) * Math.Cos(u);
            double y = v;
            double z = 2 * Math.Cosh(v / 2) * Math.Sin(u);

            return new Point3D(x / 2, y / 2, z / 2);
        }

        private Point3D Cosinus(double u, double v)
        {
            double x = u;
            double y = Math.Sin(Math.PI * (u * u + v * v)) / 2;
            double z = v;

            return new Point3D(x * 2, y * 2, z * 2);
        }

        private static Point3D Enneper(double u, double v)
        {
            double x = u - u * u * u / 3 + u * v * v;
            double y = u * u - v * v;
            double z = v - v * v * v / 3 + v * u * u;

            return new Point3D(x / 3, y / 3, z / 3);
        }

        private static Point3D Helicoidal(double u, double v)
        {
            double x = Math.Sinh(v) * Math.Sin(u);
            double y = 3 * u;
            double z = -Math.Sinh(v) * Math.Cos(u);

            return new Point3D(x / 4, y / 4, z / 4);
        }

        private static Point3D Helix(double u, double v)
        {
            double x = (1 - 0.1 * Math.Cos(v)) * Math.Cos(u);
            double y = 0.1 * (Math.Sin(v) + u / 1.7 - 5);
            double z = (1 - 0.1 * Math.Cos(v)) * Math.Sin(u);

            // Invert y for outward orientation.
            return new Point3D(x * 3, -y * 3, z * 3);
        }

        private static Point3D Hexaedron(double u, double v)
        {
            double cosu = Math.Cos(u);
            double sinu = Math.Sin(u);
            double cosv = Math.Cos(v);
            double sinv = Math.Sin(v);
            double x = cosv * cosv * cosv * cosu * cosu * cosu;
            double y = sinu * sinu * sinu;
            double z = sinv * sinv * sinv * cosu * cosu * cosu;

            // Invert y for outward orientation.
            return new Point3D(x * 3, -y * 3, z * 3);
        }

        private static Point3D Hyperhelicoidal(double u, double v)
        {
            double x = (Math.Sinh(v) * Math.Cos(3 * u)) / (1 + Math.Cosh(u) * Math.Cosh(v));
            double y = (Math.Cosh(v) * Math.Sinh(u)) / (1 + Math.Cosh(u) * Math.Cosh(v));
            double z = (Math.Sinh(v) * Math.Sin(3 * u)) / (1 + Math.Cosh(u) * Math.Cosh(v));

            return new Point3D(x * 3, y * 3, z * 3);
        }

        private static Point3D Klein(double u, double v)
        {
            double x = (3 * (1 + Math.Sin(v)) + 2 * (1 - Math.Cos(v) / 2) * Math.Cos(u)) * Math.Cos(v);
            double y = (-2 * (1 - Math.Cos(v) / 2) * Math.Sin(u));
            double z = (4 + 2 * (1 - Math.Cos(v) / 2) * Math.Cos(u)) * Math.Sin(v);

            return new Point3D(x / 2, y / 2, z / 2);
        }

        private static Point3D Shell(double u, double v)
        {
            double sinu2 = Math.Sin(u) * Math.Sin(u);
            double x = Math.Pow(1.2, v) * (sinu2 * Math.Sin(v));
            double y = Math.Pow(1.2, v) * (Math.Sin(u) * Math.Cos(u));
            double z = Math.Pow(1.2, v) * (sinu2 * Math.Cos(v));

            // Invert y for outward orientation.
            return new Point3D(x, -y, z);
        }

        private static Point3D Torus(double u, double v)
        {
            double x = (1 + 0.5 * Math.Cos(u)) * Math.Cos(v);
            double y = 0.5 * Math.Sin(u);
            double z = (1 + 0.5 * Math.Cos(u)) * Math.Sin(v);

            // Invert y for outward orientation.
            return new Point3D(x * 2, -y * 2, z * 2);
        }

        private static Point3D Sphere(double u, double v)
        {
            const double r = 2;
            double y = r * Math.Cos(u);
            double h = r * Math.Sin(u);
            double x = h * Math.Sin(v);
            double z = h * Math.Cos(v);

            return new Point3D(x, -y, z);
        }

        // From http://xahlee.info/surface/breather_p/breather_p.html
        private static Point3D Breather(double u, double v)
        {
            const double b = 0.4;
            double r = 1 - b * b;
            double w = Math.Sqrt(r);
            double denom = b * (
                (w * Math.Cosh(b * u)) * (w * Math.Cosh(b * u)) +
                (b * Math.Sin(w * v)) * (b * Math.Sin(w * v)));

            double x = -u + (2 * r * Math.Cosh(b * u) * Math.Sinh(b * u)) / denom;
            double y = (2 * w * Math.Cosh(b * u) * (-(w * Math.Cos(v) * Math.Cos(w * v)) - Math.Sin(v) * Math.Sin(w * v))) / denom;
            double z = (2 * w * Math.Cosh(b * u) * (-(w * Math.Sin(v) * Math.Cos(w * v)) + Math.Cos(v) * Math.Sin(w * v))) / denom;

            return new Point3D(x / 2, y / 2, z / 2);
        }

        // The user changed an option. Display the selected surface.
        private void Option_Click(object sender, RoutedEventArgs e)
        {
            DefineModel();
        }
    }
}
