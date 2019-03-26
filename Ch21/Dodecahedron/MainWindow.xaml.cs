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

namespace Dodecahedron
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
            group.Children.Add(new DirectionalLight(dark, new Vector3D(1, -3, -2)));
            group.Children.Add(new DirectionalLight(dark, new Vector3D(-1, 3, 2)));
        }

        // Define the model.
        private void DefineModel(Model3DGroup group)
        {
            //// Verify the dodecahedron calculations.
            //MeshExtensions.VerifyDodecahedron();

            // Show the axes.
            MeshExtensions.AddAxes(group);

            const double scale = 1.25;

            // Make a solid insphere.
            MeshGeometry3D mesh1 = new MeshGeometry3D();
            mesh1.AddSphere(D3.Origin, G3.DodecahedronInradius(), 60, 30, true);
            mesh1.ApplyTransformation(new ScaleTransform3D(scale, scale, scale));
            group.Children.Add(mesh1.MakeModel(Brushes.Red));

            // Make a translucent dodecahedron.
            MeshGeometry3D mesh2 = new MeshGeometry3D();
            mesh2.AddDodecahedron();
            mesh2.ApplyTransformation(new ScaleTransform3D(scale, scale, scale));
            Brush brush = new SolidColorBrush(Color.FromArgb(128, 128, 255, 128));
            MaterialGroup group2 = D3.MakeMaterialGroup(
                new DiffuseMaterial(brush),
                new SpecularMaterial(Brushes.White, 100));
            group.Children.Add(mesh2.MakeModel(group2));

            // Make a translucent circumsphere.
            MeshGeometry3D mesh3 = new MeshGeometry3D();
            mesh3.AddSphere(D3.Origin, G3.DodecahedronCircumradius(), 60, 30, true);
            mesh3.ApplyTransformation(new ScaleTransform3D(scale, scale, scale));
            MaterialGroup group3 = D3.MakeMaterialGroup(
                new DiffuseMaterial(brush),
                new SpecularMaterial(Brushes.White, 100));
            group.Children.Add(mesh3.MakeModel(group3));
        }
    }
}
