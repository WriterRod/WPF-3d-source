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

namespace Cylinders
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
            // Define a polygon in the XZ plane.
            Point3D center = new Point3D(0, -1.5, 2);
            Point3D[] polygon = G3.MakePolygonPoints(20, center,
                new Vector3D(0.5, 0, 0), new Vector3D(0, 0, -0.5));

            // Transform to move the polygon.
            TranslateTransform3D xTranslate = new TranslateTransform3D(-2, 0, 0);

            // Make a transform to move the polygon in the -Z direction.
            TranslateTransform3D xzTranslate = new TranslateTransform3D(2, 0, -2);

            // Make a smooth skewed cylinder.
            MeshGeometry3D mesh1 = new MeshGeometry3D();
            mesh1.AddCylinder(polygon, new Vector3D(0, 2, -1), true);
            group.Children.Add(mesh1.MakeModel(Brushes.Pink));

            // Make a skewed cylinder.
            MeshGeometry3D mesh2 = new MeshGeometry3D();
            xTranslate.Transform(polygon);
            mesh2.AddCylinder(polygon, new Vector3D(0, 3, -1));
            group.Children.Add(mesh2.MakeModel(Brushes.Pink));

            // Make a smooth right cylinder.
            MeshGeometry3D mesh3 = new MeshGeometry3D();
            xzTranslate.Transform(polygon);
            mesh3.AddCylinder(polygon, new Vector3D(0, 2, 0), true);
            group.Children.Add(mesh3.MakeModel(Brushes.LightGreen));

            // Make a right cylinder.
            MeshGeometry3D mesh4 = new MeshGeometry3D();
            xTranslate.Transform(polygon);
            mesh4.AddCylinder(polygon, new Vector3D(0, 3, 0));
            group.Children.Add(mesh4.MakeModel(Brushes.LightGreen));

            // Make a cylinder defined by cutting planes.
            MeshGeometry3D mesh5 = new MeshGeometry3D();
            xzTranslate.Transform(polygon);
            mesh5.AddCylinder(polygon, new Vector3D(0, 3, 0),
                center + new Vector3D(0, 1, 0), new Vector3D(0, 2, 1),
                center + new Vector3D(0, -0.5, 0), new Vector3D(1, -1, 0),
                true);
            group.Children.Add(mesh5.MakeModel(Brushes.LightBlue));

            // Make a smooth cylinder defined by cutting planes.
            MeshGeometry3D mesh6 = new MeshGeometry3D();
            xTranslate.Transform(polygon);
            mesh6.AddCylinder(polygon, new Vector3D(0, 3, 0),
                center + new Vector3D(0, 2, 0), new Vector3D(0, 2, 1),
                center + new Vector3D(0, -0.5, 0), new Vector3D(1, -1, 0));
            group.Children.Add(mesh6.MakeModel(Brushes.LightBlue));

            // Show the axes.
            MeshExtensions.AddAxes(group);
        }
    }
}
