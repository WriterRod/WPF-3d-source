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
using System.IO;

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
                (TheCamera, viewport, this, mainViewbox, mainViewbox);
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
            const double xmin = -5;
            const double xmax = 5;
            const double zmin = -10;
            const double zmax = 5;
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
                    mesh1.AddSurface(Splash, xmin, xmax, 60, zmin, zmax, 90, true);
                    mesh1a.AddSurface(Splash, xmin, xmax, 60, zmin, zmax, 90, false, edges, thickness);

                    mesh1.ApplyTransformation(new TranslateTransform3D(-1, -0.5, 2));
                    mesh1a.ApplyTransformation(new TranslateTransform3D(-1, -0.5, 2));
                }
                else
                    mesh1.AddSurface(Splash, xmin, xmax, 100, zmin, zmax, 100, true);
            }
            else if (strangeRadioButton.IsChecked.Value)
            {
                if (wireframeCheckBox.IsChecked.Value)
                {
                    mesh1.AddSurface(Strange, xmin, xmax, 35, zmin, zmax, 35, true);
                    mesh1a.AddSurface(Strange, xmin, xmax, 35, zmin, zmax, 35, false, edges, thickness);
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

            ModelGroup.Children.Add(mesh1.MakeModel(color1.Background));

            if (wireframeCheckBox.IsChecked.Value)
                ModelGroup.Children.Add(mesh1a.MakeModel(color2.Background));
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
            double y = 1 + 4 * Math.Cos(r2) / (1.5 + r2);
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

        private void printButton_Click(object sender, RoutedEventArgs e)
        {
            SaveControlImage(mainBorder, "Test.png");
        }

        // Save a control's image.
        private void SaveControlImage(FrameworkElement control,
            string filename)
        {
            // Get the size of the Visual and its descendants.
            Rect rect = VisualTreeHelper.GetDescendantBounds(control);

            // Make a DrawingVisual to make a screen
            // representation of the control.
            DrawingVisual dv = new DrawingVisual();

            // Fill a rectangle the same size as the control
            // with a brush containing images of the control.
            using (DrawingContext ctx = dv.RenderOpen())
            {
                VisualBrush brush = new VisualBrush(control);
                ctx.DrawRectangle(brush, null, new Rect(rect.Size));
            }

            // Make a bitmap and draw on it.
            int width = (int)control.ActualWidth;
            int height = (int)control.ActualHeight;
            RenderTargetBitmap rtb = new RenderTargetBitmap(
                width, height, 96, 96, PixelFormats.Pbgra32);
            rtb.Render(dv);
            
            // Make a PNG encoder.
            PngBitmapEncoder encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(rtb));
            
            // Save the file.
            using (FileStream fs = new FileStream(filename,
                FileMode.Create, FileAccess.Write, FileShare.None))
            {
                encoder.Save(fs);
            }
        }
    }
}
