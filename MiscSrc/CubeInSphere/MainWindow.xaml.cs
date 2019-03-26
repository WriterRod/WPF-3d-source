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

namespace CubeInSphere
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
                (TheCamera, viewport, this, MainGrid, MainGrid);
        }

        // Define the lights.
        private void DefineLights(Model3DGroup group)
        {
            Color dark = Color.FromArgb(255, 96, 96, 96);

            group.Children.Add(new AmbientLight(dark));

            group.Children.Add(new DirectionalLight(dark, new Vector3D(0, -1, 0)));
            group.Children.Add(new DirectionalLight(dark, new Vector3D(1, -1, -1)));
        }

        // Define the model.
        private void DefineModel(Model3DGroup group)
        {
            // Show the axes.
            MeshExtensions.AddAxes(group);


            MeshGeometry3D mesh1 = new MeshGeometry3D();
            double dx = 3 / Math.Sqrt(3);
            mesh1.AddBox(new Point3D(-dx, -dx, -dx),
                D3.XVector(2 * dx), D3.YVector(2 * dx), D3.ZVector(2 * dx));
            group.Children.Add(mesh1.MakeModel(new SolidColorBrush(Colors.LightBlue)));

            const int numTheta = 60;
            const int numPhi = 30;
            MeshGeometry3D mesh2 = new MeshGeometry3D();
            Point3D center = new Point3D(0, 0, 0);
            mesh2.AddSphere(center, 3, numTheta, numPhi, true);

            MaterialGroup material = new MaterialGroup();
            Color color = Color.FromArgb(64, 128, 128, 128);
            material.Children.Add(new DiffuseMaterial(new SolidColorBrush(color)));
            material.Children.Add(new SpecularMaterial(Brushes.White, 100));

            group.Children.Add(mesh2.MakeModel(material));
        }
    }
}
