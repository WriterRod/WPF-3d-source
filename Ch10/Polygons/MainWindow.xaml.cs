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

namespace Polygons
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
            group.Children.Add(new AmbientLight(Color.FromArgb(255, 255, 255, 255)));

            //group.Children.Add(new DirectionalLight(Colors.LightGray, new Vector3D(1, -3, -2)));
            //group.Children.Add(new DirectionalLight(Colors.LightGray, new Vector3D(-1, 3, 2)));
        }

        // Define the model.
        private void DefineModel(Model3DGroup group)
        {
            // Make a textured pentagon in the XZ plane.
            MeshGeometry3D mesh1 = new MeshGeometry3D();
            mesh1.AddRegularPolygon(5, D3.Origin,
                D3.XVector(2), D3.ZVector(-2),
                D3.MakePolygonTextureCoords(5));
            group.Children.Add(mesh1.MakeModel("Smiley.png"));

            // Clone and move down 2 units.
            MeshGeometry3D mesh2 = mesh1.Clone();
            mesh2.ApplyTransformation(new TranslateTransform3D(0, -2, 0));
            group.Children.Add(mesh2.MakeModel(Brushes.LightBlue));

            // Make a triangle with X = -2.
            MeshGeometry3D mesh3 = new MeshGeometry3D();
            mesh3.AddPolygon(
                new Point3D(-2, -2, -2),
                new Point3D(-2, +2,  0),
                new Point3D(-2, -2, +2)
            );
            group.Children.Add(mesh3.MakeModel(Brushes.Pink));

            // Make a rectangle with Z = -2.
            MeshGeometry3D mesh4 = new MeshGeometry3D();
            mesh4.AddPolygon(
                new Point3D(-2, -2, -2),
                new Point3D(+2, -2, -2),
                new Point3D(+2, +2, -2),
                new Point3D(-2, +2, -2)
            );
            group.Children.Add(mesh4.MakeModel(Brushes.LightGreen));
        }
    }
}
