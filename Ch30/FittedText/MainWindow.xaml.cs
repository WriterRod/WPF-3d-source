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

namespace FittedText
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
        private Model3DGroup MainGroup;

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // Define WPF objects.
            ModelVisual3D visual3d = new ModelVisual3D();
            MainGroup = new Model3DGroup();
            visual3d.Content = MainGroup;
            mainViewport.Children.Add(visual3d);

            // Define the camera, lights, and model.
            DefineCamera(mainViewport);
            DefineLights(MainGroup);
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
            group.Children.Add(new AmbientLight(Colors.White));
        }

        // Define the model.
        private void DefineModel()
        {
            // Make some text.
            DrawTextWithBorder("3D Text",
                new Point3D(0, -2, 1), new Point3D(0, -2, -1),
                new Point3D(0, 0, -1), new Point3D(0, 0, 1),
                Brushes.Transparent, Brushes.Black, TextAlignment.Center);
            DrawTextWithBorder("3D Text",
                new Point3D(1, -2, 1), new Point3D(1, -2, -1),
                new Point3D(1, 0, -1), new Point3D(1, 0, 1),
                Brushes.Transparent, Brushes.Black, TextAlignment.Center);
            DrawTextWithBorder("3D Text",
                new Point3D(-1, -2, 1), new Point3D(-1, -2, -1),
                new Point3D(-1, 0, -1), new Point3D(-1, 0, 1),
                Brushes.Transparent, Brushes.Black, TextAlignment.Center);

            FontFamily fontFamily = new FontFamily("Times New Roman");
            DrawTextWithBorder("Two-line\nText",
                new Point3D(0, 0.5, 2.25), new Point3D(0, 0.5, 0.25),
                new Point3D(0, 1.5, 0.25), new Point3D(0, 1.5, 2.25),
                Brushes.LightBlue, Brushes.Black, TextAlignment.Center, fontFamily);
            DrawTextWithBorder("Two-line\nText",
                new Point3D(0, 0.5, -0.25), new Point3D(0, 0.5, -2),
                new Point3D(0, 2.5, -2), new Point3D(0, 2.5, -0.25),
                Brushes.LightBlue, Brushes.Black, TextAlignment.Center, fontFamily);
        }

        // Draw text with a border of segments.
        private void DrawTextWithBorder(string text,
            Point3D ll, Point3D lr, Point3D ur, Point3D ul,
            Brush bgBrush, Brush fgBrush,
            TextAlignment textAlign, FontFamily fontFamily = null)
        {
            MeshGeometry3D mesh1 = new MeshGeometry3D();
            MainGroup.Children.Add(mesh1.AddFittedText(text,
                ll, lr, ur, ul,
                bgBrush, fgBrush, textAlign, fontFamily));

            MeshGeometry3D mesh2 = new MeshGeometry3D();
            HashSet<Edge> edges = new HashSet<Edge>();
            mesh2.AddPolygon(edges, 0.01, ll, lr, ur, ul);
            MainGroup.Children.Add(mesh2.MakeModel(Brushes.Red));
        }
    }
}
