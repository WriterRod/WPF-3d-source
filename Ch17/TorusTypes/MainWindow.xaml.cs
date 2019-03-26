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

namespace TorusTypes
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
            const int numTheta = 60;
            const int numPhi = 30;

            // Make a smooth torus.
            MeshGeometry3D mesh1 = new MeshGeometry3D();
            Point3D center = new Point3D(1.75, 1, 3.25);
            mesh1.AddTorus(center, 0.9, 0.6, numTheta, numPhi, true, true);
            GeometryModel3D model1 = (GeometryModel3D)mesh1.MakeModel(Brushes.LightBlue);
            model1.BackMaterial = model1.Material;
            group.Children.Add(model1);

            // Make a smooth torus.
            MeshGeometry3D mesh2 = new MeshGeometry3D();
            center = new Point3D(1.75, 1, 0);
            mesh2.AddTorus(center, 0.75, 0.75, numTheta, numPhi, true, true);
            GeometryModel3D model2 = (GeometryModel3D)mesh2.MakeModel(Brushes.LightBlue);
            model2.BackMaterial = model2.Material;
            group.Children.Add(model2);

            // Make a smooth torus.
            MeshGeometry3D mesh3 = new MeshGeometry3D();
            center = new Point3D(1.75, 1, -3.25);
            mesh3.AddTorus(center, 0.6, 0.9, numTheta, numPhi, true, true);
            GeometryModel3D model3 = (GeometryModel3D)mesh3.MakeModel(Brushes.LightBlue);
            model3.BackMaterial = model3.Material;
            group.Children.Add(model3);

            // Half tori.
            // Make a smooth torus.
            MeshGeometry3D mesh4 = new MeshGeometry3D();
            center = new Point3D(-1.75, 1, 3.25);
            mesh4.AddTorus(center, 1, 0.5, numTheta, numPhi, true, false);
            GeometryModel3D model4 = (GeometryModel3D)mesh4.MakeModel(Brushes.LightBlue);
            model4.BackMaterial = model4.Material;
            group.Children.Add(model4);

            // Make a smooth torus.
            MeshGeometry3D mesh5 = new MeshGeometry3D();
            center = new Point3D(-1.75, 1, 0);
            mesh5.AddTorus(center, 0.75, 0.75, numTheta, numPhi, true, false);
            GeometryModel3D model5 = (GeometryModel3D)mesh5.MakeModel(Brushes.LightBlue);
            model5.BackMaterial = model5.Material;
            group.Children.Add(model5);

            // Make a smooth torus.
            MeshGeometry3D mesh6 = new MeshGeometry3D();
            center = new Point3D(-1.75, 1, -3.25);
            mesh6.AddTorus(center, 0.6, 0.9, numTheta, numPhi, true, false);
            GeometryModel3D model6 = (GeometryModel3D)mesh6.MakeModel(Brushes.LightBlue);
            model6.BackMaterial = model6.Material;
            group.Children.Add(model6);

            // Show the axes.
            MeshExtensions.AddAxes(group);
        }
    }
}
