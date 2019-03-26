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

namespace Tori
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
            Color dark = Color.FromArgb(255, 96, 96, 96);

            group.Children.Add(new AmbientLight(dark));

            group.Children.Add(new DirectionalLight(dark, new Vector3D(0, -1, 0)));
            group.Children.Add(new DirectionalLight(dark, new Vector3D(1, -1, -1)));
        }

        // Define the model.
        private void DefineModel(Model3DGroup group)
        {
            const int numTheta = 15;
            const int numPhi = 30;

            // Make a non-smooth torus.
            MeshGeometry3D mesh1 = new MeshGeometry3D();
            Point3D center = new Point3D(-1, 1, 2);
            mesh1.AddTorus(center, 1, 0.5, numTheta, numPhi);
            group.Children.Add(mesh1.MakeModel(Brushes.LightGreen));

            // Make a smooth torus.
            MeshGeometry3D mesh2 = new MeshGeometry3D();
            center = new Point3D(1, 0, 2);
            mesh2.AddTorus(center, 1, 0.5, numTheta, numPhi, true);
            group.Children.Add(mesh2.MakeModel(Brushes.LightGreen));

            // Make a non-smooth textured torus.
            MeshGeometry3D mesh3 = new MeshGeometry3D();
            center = new Point3D(-1, 1, -2);
            mesh3.AddTexturedTorus(center, 1, 0.5, numTheta, numPhi);
            mesh3.ApplyTransformation(D3.Rotate(D3.YVector(), center, -150));
            mesh3.Positions.Add(new Point3D());
            mesh3.TextureCoordinates.Add(new Point(1.01, 1.01));
            group.Children.Add(mesh3.MakeModel("world.jpg"));

            // Make a smooth textured torus.
            MeshGeometry3D mesh4 = new MeshGeometry3D();
            center = new Point3D(1, 0, -2);
            mesh4.AddTexturedTorus(center, 1, 0.5, numTheta, numPhi, true);
            mesh4.ApplyTransformation(D3.Rotate(D3.YVector(), center, -150));
            mesh4.Positions.Add(new Point3D());
            mesh4.TextureCoordinates.Add(new Point(1.01, 1.01));
            group.Children.Add(mesh4.MakeModel("world.jpg"));

            // Show the axes.
            MeshExtensions.AddAxes(group);
        }
    }
}
