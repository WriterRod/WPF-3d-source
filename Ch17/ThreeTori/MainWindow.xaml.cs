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

namespace ThreeTori
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
            Color dark = Color.FromArgb(255, 128, 128, 128);

            group.Children.Add(new AmbientLight(dark));

            group.Children.Add(new DirectionalLight(dark, new Vector3D(0, -1, 0)));
        }

        // Define the model.
        private void DefineModel(Model3DGroup group)
        {
            const int numTheta = 30;
            const int numPhi = 60;

            // Make a smooth torus.
            MeshGeometry3D mesh1 = new MeshGeometry3D();
            Point3D center = new Point3D(1, 0, 1);
            mesh1.AddTorus(center, 1, 0.5, numTheta, numPhi, true);
            mesh1.ApplyTransformation(new TranslateTransform3D(0, -1, -1));
            group.Children.Add(mesh1.MakeModel(Brushes.Pink));

            // Make a smooth torus.
            MeshGeometry3D mesh2 = new MeshGeometry3D();
            center = new Point3D(0, 0.6, 2.5);
            mesh2.AddTorus(center, 1, 0.5, numTheta, numPhi, true);
            mesh2.ApplyTransformation(D3.Rotate(D3.ZVector(), center, 40));
            mesh2.ApplyTransformation(D3.Rotate(D3.YVector(), center, 90));
            mesh2.ApplyTransformation(new TranslateTransform3D(0, -1, -1));
            group.Children.Add(mesh2.MakeModel(Brushes.LightGreen));

            // Make a smooth torus.
            MeshGeometry3D mesh3 = new MeshGeometry3D();
            center = new Point3D(-0.75, 1, -0.75);
            mesh3.AddTorus(center, 1.1, 0.55, numTheta, numPhi, true);
            mesh3.ApplyTransformation(D3.Rotate(D3.XVector(), center, 90));
            mesh3.ApplyTransformation(D3.Rotate(D3.YVector(), center, 45));
            mesh3.ApplyTransformation(new TranslateTransform3D(0, -1, 0));
            group.Children.Add(mesh3.MakeModel(Brushes.LightBlue));

            // Show the axes.
            MeshExtensions.AddAxes(group);
        }
    }
}
