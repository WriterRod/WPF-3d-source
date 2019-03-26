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

namespace Pyramids
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
            Point3D center = new Point3D(0, 0, 4.5);
            Point3D[] polygon = G3.MakePolygonPoints(6, center,
                new Vector3D(1, 0, 0), new Vector3D(0, 0, -1));

            // Make a transform to move the polygon in the -Z direction.
            TranslateTransform3D translate = new TranslateTransform3D(0, 0, -3);

            // Make a right pyramid.
            MeshGeometry3D mesh1 = new MeshGeometry3D();
            mesh1.AddPyramid(center, polygon, new Vector3D(0, 3, 0));
            group.Children.Add(mesh1.MakeModel(Brushes.Pink));

            // Make a skewed pyramid.
            translate.Transform(polygon);
            center = translate.Transform(center);
            MeshGeometry3D mesh2 = new MeshGeometry3D();
            mesh2.AddPyramid(center, polygon, new Vector3D(0, 3, 2));
            group.Children.Add(mesh2.MakeModel(Brushes.LightBlue));

            // Make a frustum with cutting plane parallel to the base.
            translate.Transform(polygon);
            center = translate.Transform(center);
            MeshGeometry3D mesh3 = new MeshGeometry3D();
            mesh3.AddFrustum(center, polygon, new Vector3D(0, 4, 0), 2);
            group.Children.Add(mesh3.MakeModel(Brushes.LightGreen));

            // Make a frustum with cutting plane not parallel to the base.
            translate.Transform(polygon);
            center = translate.Transform(center);
            MeshGeometry3D mesh4 = new MeshGeometry3D();
            Point3D planePt = center + new Vector3D(0, 2, 0);
            Vector3D planeN = new Vector3D(1, 1, 1);
            mesh4.AddFrustum(center, polygon, new Vector3D(0, 4, 0), planePt, planeN);
            group.Children.Add(mesh4.MakeModel(Brushes.Orange));

            // Show the axes.
            MeshExtensions.AddAxes(group);
        }
    }
}
