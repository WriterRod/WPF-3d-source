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

namespace SameSizedText
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
            FontFamily fontFamily = new FontFamily("Franklin Gothic Demi");
            const double fontSize = 0.4;
            double matWidth = 2;
            double matHeight = 2;
            const string text = "3D Text";
            double y = 1.25;
            double z = 1.25;

            MeshGeometry3D mesh1 = new MeshGeometry3D();
            MainGroup.Children.Add(mesh1.AddSizedText(text,
                fontSize, matWidth, matHeight,
                new Point3D(-2, y, z + matHeight), new Point3D(-2, y, z),
                new Point3D(-2, y + matWidth, z), new Point3D(-2, y + matWidth, z + matHeight),
                Brushes.LightGreen, Brushes.Black,
                HorizontalAlignment.Left, VerticalAlignment.Top, fontFamily));
            z -= 2.25;

            MeshGeometry3D mesh2 = new MeshGeometry3D();
            MainGroup.Children.Add(mesh2.AddSizedText(text,
                fontSize, matWidth, matHeight,
                new Point3D(-2, y, z + matHeight), new Point3D(-2, y, z),
                new Point3D(-2, y + matWidth, z), new Point3D(-2, y + matWidth, z + matHeight),
                Brushes.LightGreen, Brushes.Black,
                HorizontalAlignment.Center, VerticalAlignment.Top, fontFamily));
            z -= 2.25;

            MeshGeometry3D mesh3 = new MeshGeometry3D();
            MainGroup.Children.Add(mesh3.AddSizedText(text,
                fontSize, matWidth, matHeight,
                new Point3D(-2, y, z + matHeight), new Point3D(-2, y, z),
                new Point3D(-2, y + matWidth, z), new Point3D(-2, y + matWidth, z + matHeight),
                Brushes.LightGreen, Brushes.Black,
                HorizontalAlignment.Right, VerticalAlignment.Top, fontFamily));

            y -= 2.25;
            z = 1.25;
            MeshGeometry3D mesh4 = new MeshGeometry3D();
            MainGroup.Children.Add(mesh4.AddSizedText(text,
                fontSize, matWidth, matHeight,
                new Point3D(-2, y, z + matHeight), new Point3D(-2, y, z),
                new Point3D(-2, y + matWidth, z), new Point3D(-2, y + matWidth, z + matHeight),
                Brushes.LightGreen, Brushes.Black,
                HorizontalAlignment.Left, VerticalAlignment.Center, fontFamily));
            z -= 2.25;

            MeshGeometry3D mesh5 = new MeshGeometry3D();
            MainGroup.Children.Add(mesh5.AddSizedText(text,
                fontSize, matWidth, matHeight,
                new Point3D(-2, y, z + matHeight), new Point3D(-2, y, z),
                new Point3D(-2, y + matWidth, z), new Point3D(-2, y + matWidth, z + matHeight),
                Brushes.LightGreen, Brushes.Black,
                HorizontalAlignment.Center, VerticalAlignment.Center, fontFamily));
            z -= 2.25;

            MeshGeometry3D mesh6 = new MeshGeometry3D();
            MainGroup.Children.Add(mesh6.AddSizedText(text,
                fontSize, matWidth, matHeight,
                new Point3D(-2, y, z + matHeight), new Point3D(-2, y, z),
                new Point3D(-2, y + matWidth, z), new Point3D(-2, y + matWidth, z + matHeight),
                Brushes.LightGreen, Brushes.Black,
                HorizontalAlignment.Right, VerticalAlignment.Center, fontFamily));

            y -= 2.25;
            z = 1.25;
            MeshGeometry3D mesh7 = new MeshGeometry3D();
            MainGroup.Children.Add(mesh7.AddSizedText(text,
                fontSize, matWidth, matHeight,
                new Point3D(-2, y, z + matHeight), new Point3D(-2, y, z),
                new Point3D(-2, y + matWidth, z), new Point3D(-2, y + matWidth, z + matHeight),
                Brushes.LightGreen, Brushes.Black,
                HorizontalAlignment.Left, VerticalAlignment.Bottom, fontFamily));
            z -= 2.25;

            MeshGeometry3D mesh8 = new MeshGeometry3D();
            MainGroup.Children.Add(mesh8.AddSizedText(text,
                fontSize, matWidth, matHeight,
                new Point3D(-2, y, z + matHeight), new Point3D(-2, y, z),
                new Point3D(-2, y + matWidth, z), new Point3D(-2, y + matWidth, z + matHeight),
                Brushes.LightGreen, Brushes.Black,
                HorizontalAlignment.Center, VerticalAlignment.Bottom, fontFamily));
            z -= 2.25;

            MeshGeometry3D mesh9 = new MeshGeometry3D();
            MainGroup.Children.Add(mesh9.AddSizedText(text,
                fontSize, matWidth, matHeight,
                new Point3D(-2, y, z + matHeight), new Point3D(-2, y, z),
                new Point3D(-2, y + matWidth, z), new Point3D(-2, y + matWidth, z + matHeight),
                Brushes.LightGreen, Brushes.Black,
                HorizontalAlignment.Right, VerticalAlignment.Bottom, fontFamily));


            double x = -2;
            y = 2.25;
            MeshGeometry3D mesh10 = new MeshGeometry3D();
            matWidth = 4;
            matHeight = 1;
            MainGroup.Children.Add(mesh10.AddSizedText(text,
                fontSize, matWidth, matHeight,
                new Point3D(x, y, -3.5), new Point3D(x + matWidth, y, -3.5),
                new Point3D(x + matWidth, y + matHeight, -3.5),
                new Point3D(x, y + matHeight, -3.5),
                Brushes.LightBlue, Brushes.Black,
                HorizontalAlignment.Center, VerticalAlignment.Center, fontFamily));

            y = 0.5;
            MeshGeometry3D mesh11 = new MeshGeometry3D();
            matWidth = 6;
            matHeight = 1;
            MainGroup.Children.Add(mesh11.AddSizedText(text,
                fontSize, matWidth, matHeight,
                new Point3D(x, y, -3.5), new Point3D(x + matWidth, y, -3.5),
                new Point3D(x + matWidth, y + matHeight, -3.5),
                new Point3D(x, y + matHeight, -3.5),
                Brushes.LightBlue, Brushes.Black,
                HorizontalAlignment.Center, VerticalAlignment.Center, fontFamily));

            y = -3.25;
            MeshGeometry3D mesh12 = new MeshGeometry3D();
            matWidth = 2;
            matHeight = 3;
            MainGroup.Children.Add(mesh12.AddSizedText(text,
                fontSize, matWidth, matHeight,
                new Point3D(x, y, -3.5), new Point3D(x + matWidth, y, -3.5),
                new Point3D(x + matWidth, y + matHeight, -3.5),
                new Point3D(x, y + matHeight, -3.5),
                Brushes.LightBlue, Brushes.Black,
                HorizontalAlignment.Center, VerticalAlignment.Center, fontFamily));
        }
    }
}
