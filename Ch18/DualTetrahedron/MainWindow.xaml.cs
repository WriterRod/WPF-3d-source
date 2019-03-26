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

namespace DualTetrahedron
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
            group.Children.Add(new DirectionalLight(dark, new Vector3D(1, -1, 0)));
            group.Children.Add(new DirectionalLight(dark, new Vector3D(1, -1, -2)));
        }

        // Define the model.
        private void DefineModel(Model3DGroup group)
        {
            // Show the axes.
            MeshExtensions.AddAxes(group);

            const double scale = 2;

            // Make a solid tetrahedron.
            MeshGeometry3D mesh1 = new MeshGeometry3D();
            mesh1.AddTetrahedron(true);
            mesh1.ApplyTransformation(new ScaleTransform3D(scale, scale, scale));
            mesh1.ApplyTransformation(D3.Rotate(D3.YVector(), D3.Origin, 180));
            mesh1.ApplyTransformation(D3.Rotate(D3.XVector(), D3.Origin, 180));
            group.Children.Add(mesh1.MakeModel(Brushes.LightBlue));

            // Make a translucent tetrahedron.
            MeshGeometry3D mesh2 = new MeshGeometry3D();
            mesh2.AddTetrahedron(true);
            mesh2.ApplyTransformation(new ScaleTransform3D(scale * 3, scale * 3, scale * 3));
            Brush brush = new SolidColorBrush(Color.FromArgb(128, 128, 255, 128));
            MaterialGroup group3 = D3.MakeMaterialGroup(
                new DiffuseMaterial(brush),
                new SpecularMaterial(Brushes.White, 100));
            group.Children.Add(mesh2.MakeModel(group3));

            // Mark both tetrahedrons' vertices.
            MeshGeometry3D mesh3 = new MeshGeometry3D();
            foreach (Point3D point in mesh1.Positions)
                mesh3.AddSphere(point, 0.1, 20, 10, true);
            foreach (Point3D point in mesh2.Positions)
                mesh3.AddSphere(point, 0.1, 20, 10, true);
            group.Children.Add(mesh3.MakeModel(Brushes.Red));
        }
    }
}
