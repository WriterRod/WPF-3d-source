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

// Add a reference to System.Numerics.
using System.Numerics;
using System.Windows.Media.Media3D;

namespace ParametricSurfaces
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
            if (ellipsoidRadioButton.IsChecked.Value)
            {
                const double thetaMax = Math.PI / 2;
                const double phiMax = Math.PI;
                mesh1.AddSurface(Ellipsoid, -thetaMax, thetaMax, 10, -phiMax, phiMax, 25, true);
                if (wireframeCheckBox.IsChecked.Value)
                    mesh1a.AddSurface(Ellipsoid, -thetaMax, thetaMax, 10, -phiMax, phiMax, 25, false, edges, 0.02);
            }
            else if (mobiusRadioButton.IsChecked.Value)
            {
                mesh1.AddSurface(Mobius, 0, 2 * Math.PI, 30, -1, 1, 5, true);
                if (wireframeCheckBox.IsChecked.Value)
                    mesh1a.AddSurface(Mobius, 0, 2 * Math.PI, 30, -1, 1, 5, false, edges, 0.02);
            }
            else if (kleinBottleRadioButton.IsChecked.Value)
            {
                mesh1.AddSurface(KleinBottle, 0, Math.PI, 50, 0, 2 * Math.PI, 25, true);
                if (wireframeCheckBox.IsChecked.Value)
                    mesh1a.AddSurface(KleinBottle, 0, Math.PI, 50, 0, 2 * Math.PI, 25, false, edges, 0.02);
            }
            else if (boysSurfaceRadioButton.IsChecked.Value)
            {
                mesh1.AddSurface(BoysSurface, 0, 1, 25, 0, 2 * Math.PI, 100, true);
                if (wireframeCheckBox.IsChecked.Value)
                    mesh1a.AddSurface(BoysSurface, 0, 1, 25, 0, 2 * Math.PI, 100, false, edges, 0.02);
            }

            GeometryModel3D model = new GeometryModel3D(mesh1, null);
            if (frontfacesCheckBox.IsChecked.Value)
                model.Material = new DiffuseMaterial(Brushes.LightBlue);
            if (backfacesCheckBox.IsChecked.Value)
                model.BackMaterial = new DiffuseMaterial(Brushes.LightBlue);
            ModelGroup.Children.Add(model);

            if (wireframeCheckBox.IsChecked.Value)
                ModelGroup.Children.Add(mesh1a.MakeModel(Brushes.Blue));
        }

        // The surface-generating methods.
        private Point3D Ellipsoid(double theta, double phi)
        {
            double x = 3 * Math.Cos(theta) * Math.Cos(phi);
            double y = 1 * Math.Sin(theta);
            double z = 2 * Math.Cos(theta) * Math.Sin(phi);

            // Invert y for outward orientation.
            return new Point3D(x, -y, z);
        }

        private Point3D Mobius(double u, double v)
        {
            double x = (1 + v / 2 * Math.Cos(u / 2)) * Math.Cos(u);
            double y = (1 + v / 2 * Math.Cos(u / 2)) * Math.Sin(u);
            double z = v / 2 * Math.Sin(u / 2);

            return new Point3D(x, y, z);
        }

        // See https://en.wikipedia.org/wiki/Klein_bottle
        private Point3D KleinBottle(double u, double v)
        {
            double sinV = Math.Sin(v);
            double cosV = Math.Cos(v);
            double sinU = Math.Sin(u);
            double cosU = Math.Cos(u);
            double cosU2 = cosU * cosU;
            double cosU3 = cosU2 * cosU;
            double cosU4 = cosU3 * cosU;
            double cosU5 = cosU4 * cosU;
            double cosU6 = cosU5 * cosU;
            double cosU7 = cosU6 * cosU;

            double x = -2.0 / 15 * cosU * (3 * cosV - 30 * sinU + 90 * cosU4 * sinU -
                60 * cosU6 * sinU + 5 * cosU * cosV * sinU);
            double y = -1.0 / 15 * sinU * (3 * cosV - 3 * cosU2 * cosV -
                48 * cosU4 * cosV + 48 * cosU6 * cosV -
                60 * sinU + 5 * cosU * cosV * sinU - 5 * cosU3 * cosV * sinU -
                80 * cosU5 * cosV * sinU + 80 * cosU7 * cosV * sinU);
            double z = 2.0 / 15 * (3 + 5 * cosU * sinU) * sinV;

            // Note: Move y up a bit and invert.
            // Invert x to orient the "outer" parts of the bottle outwardly.
            // If you don't use a BackMaterial, then parts inside the opening are culled.
            return new Point3D(-x, 2 - y, z);
        }

        // Requires the Complex class in the System.Numerics namespace.
        // For the basics, see:
        //      https://en.wikipedia.org/wiki/Boy%27s_surface
        //      http://mathworld.wolfram.com/BoySurface.html
        // Here 0 <= i <= 1, 0 <= v <= 2*pi. This is stated at:
        //      https://mathcurve.com/surfaces/boy/boy.shtml
        // The real number is w = u*e^(iv). To see how to solve that, see:
        //      https://mathcurve.com/surfaces/boy/boy.shtml
        private static Point3D BoysSurface(double u, double v)
        {
            double sqrt5 = Math.Sqrt(5);

            // w = u*e^(iv)
            double wr = Math.Cos(v);
            double wi = Math.Sin(v);
            Complex w = u * new Complex(wr, wi);
            Complex w3 = w * w * w;
            Complex w4 = w3 * w;
            Complex w6 = w3 * w3;

            Complex d = w6 + sqrt5 * w3 - 1;
            Complex wa = w * (1 - w4) / d;
            Complex wb = w * (1 + w4) / d;
            Complex wc = (1 + w6) / d;

            double g1 = -1.5 * wa.Imaginary;
            double g2 = -1.5 * wb.Real;
            double g3 = wc.Imaginary - 0.5;
            double l2 = g1 * g1 + g2 * g2 + g3 * g3;

            double x = g1 / l2;
            double y = g2 / l2;
            double z = g3 / l2;

            // Note: Switch y and z and move y up a bit.
            // Invert x to orient the "outer" parts of the bottle outwardly.
            // If you don't use a BackMaterial, then parts inside the openings are culled.
            return new Point3D(-x, 0.75 + z, -y);
        }

        // The user changed an option. Display the selected surface.
        private void Option_Click(object sender, RoutedEventArgs e)
        {
            DefineModel();
        }
    }
}
