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

namespace BadNormals
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

            group.Children.Add(new DirectionalLight(Colors.White, new Vector3D(0, -1, 0)));
        }

        // Define the model.
        private void DefineModel(Model3DGroup group)
        {
            const int numTheta = 30;
            const int numPhi = 15;

            MeshGeometry3D mesh1 = new MeshGeometry3D();
            Point3D center = new Point3D(0, 0, 1.75);
            mesh1.AddTexturedSphere(center, 1.5, numTheta, numPhi, true);
            group.Children.Add(mesh1.MakeModel("world.jpg"));
            // Add a point to redefine the texture area to hide the "seam."
            mesh1.Positions.Add(new Point3D());
            mesh1.TextureCoordinates.Add(new Point(1.01, 1.01));

            MeshGeometry3D mesh3 = new MeshGeometry3D();
            center = new Point3D(0, 0, -1.75);
            mesh3.AddTexturedSphere(center, 1.5, numTheta, numPhi, true);
            mesh3.ApplyTransformation(D3.Rotate(D3.XVector(), center, 90));
            group.Children.Add(mesh3.MakeModel("world.jpg"));
            // Add a point to redefine the texture area to hide the "seam."
            mesh3.Positions.Add(new Point3D());
            mesh3.TextureCoordinates.Add(new Point(1.01, 1.01));

            // Show the axes.
            MeshExtensions.AddAxes(group);
        }
    }
}
