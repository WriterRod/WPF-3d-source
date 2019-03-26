//#define MONOCHROME
//#define AXES

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

namespace Interlocked
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class Window1 : Window
    {
        public Window1()
        {
            InitializeComponent();
        }

        // The main object model group.
        private Model3DGroup MainModel3Dgroup = new Model3DGroup();

        // The camera.
//        private PerspectiveCamera TheCamera;

        //// The camera's current location.
        //private double CameraPhi = Math.PI / 6.0;       // 30 degrees
        //private double CameraTheta = Math.PI / 6.0;     // 30 degrees
        //private double CameraR = 8.0;

        //// The change in CameraPhi when you press the up and down arrows.
        //private const double CameraDPhi = 0.1;

        //// The change in CameraTheta when you press the left and right arrows.
        //private const double CameraDTheta = 0.1;

        //// The change in CameraR when you press + or -.
        //private const double CameraDR = 0.1;

        // Create the scene.
        // MainViewport is the Viewport3D defined
        // in the XAML code that displays everything.
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //// Give the camera its initial position.
            //TheCamera = new PerspectiveCamera();
            //TheCamera.FieldOfView = 60;
            //MainViewport.Camera = TheCamera;
            //PositionCamera();

            // Define lights.
            DefineLights();

            // Create the model.
            DefineModel();

            // Add the group of models to a ModelVisual3D.
            ModelVisual3D model_visual = new ModelVisual3D();
            model_visual.Content = MainModel3Dgroup;

            // Display the main visual to the viewport.
            MainViewport.Children.Add(model_visual);
        }

        // Define the lights.
        private void DefineLights()
        {
            AmbientLight ambient_light = new AmbientLight(Colors.Gray);
            Color gray = Color.FromArgb(255, 64, 64, 64);
            DirectionalLight directional_light =
                new DirectionalLight(Colors.Gray,
                    new Vector3D(1, -2, -3));
            DirectionalLight directional_light2 =
                new DirectionalLight(Color.FromArgb(255, 64, 64, 64),
                    new Vector3D(0, -1, 0));
            DirectionalLight directional_light3 =
                new DirectionalLight(Color.FromArgb(255, 64, 64, 64),
                    new Vector3D(0, 0, -1));
            MainModel3Dgroup.Children.Add(ambient_light);
            MainModel3Dgroup.Children.Add(directional_light);
            //MainModel3Dgroup.Children.Add(directional_light2);
            //MainModel3Dgroup.Children.Add(directional_light3);
        }

        // Add the model to the Model3DGroup.
        private void DefineModel()
        {
            // Brushes.
#if MONOCHROME
            SolidColorBrush pink_brush = new SolidColorBrush(Color.FromArgb(255, 224, 224, 224));
            SolidColorBrush lightgreen_brush = new SolidColorBrush(Color.FromArgb(255, 150, 150, 150));
            SolidColorBrush lightergreen_brush = new SolidColorBrush(Color.FromArgb(255, 200, 200, 200));
            SolidColorBrush lightblue_brush = new SolidColorBrush(Color.FromArgb(255, 100, 100, 100));
            SolidColorBrush red_brush = new SolidColorBrush(Color.FromArgb(255, 60, 60, 60));
            SolidColorBrush green_brush = new SolidColorBrush(Color.FromArgb(255, 80, 80, 80));
            SolidColorBrush blue_brush = new SolidColorBrush(Color.FromArgb(255, 160, 160, 160));
            SolidColorBrush seg_brush = new SolidColorBrush(Color.FromArgb(255, 20, 20, 20));
#else
            SolidColorBrush pink_brush = Brushes.Pink;
            SolidColorBrush lightgreen_brush = Brushes.LightGreen;
            SolidColorBrush lightblue_brush = Brushes.LightBlue;
            SolidColorBrush red_brush = Brushes.Red;
            SolidColorBrush green_brush = Brushes.Green;
            SolidColorBrush blue_brush = Brushes.Blue;
            SolidColorBrush seg_brush = Brushes.Red;
#endif

            // Cube.
            MeshGeometry3D cube_mesh = new MeshGeometry3D();
            cube_mesh.AddCube(2, null);
            DiffuseMaterial cube_material = new DiffuseMaterial(lightblue_brush);
            GeometryModel3D cube_model = new GeometryModel3D(cube_mesh, cube_material);
            MainModel3Dgroup.Children.Add(cube_model);

            MeshGeometry3D wire_mesh = cube_mesh.ToWireframe(0.05);
            DiffuseMaterial wire_material = new DiffuseMaterial(red_brush);
            GeometryModel3D wire_model = new GeometryModel3D(wire_mesh, wire_material);
            MainModel3Dgroup.Children.Add(wire_model);

#if AXES
            // Axes.
            Point3D origin = new Point3D();
            const double axis_thickness = 0.05;
            const double axis_length = 3;
            MeshGeometry3D xaxis_mesh = new MeshGeometry3D();
            xaxis_mesh.AddSegment(origin, new Point3D(axis_length, 0, 0), axis_thickness, true);
            DiffuseMaterial xaxis_material = new DiffuseMaterial(red_brush);
            GeometryModel3D xaxis_model = new GeometryModel3D(xaxis_mesh, xaxis_material);
            MainModel3Dgroup.Children.Add(xaxis_model);

            MeshGeometry3D yaxis_mesh = new MeshGeometry3D();
            yaxis_mesh.AddSegment(origin, new Point3D(0, axis_length, 0), axis_thickness, true);
            DiffuseMaterial yaxis_material = new DiffuseMaterial(green_brush);
            GeometryModel3D yaxis_model = new GeometryModel3D(yaxis_mesh, yaxis_material);
            MainModel3Dgroup.Children.Add(yaxis_model);

            MeshGeometry3D zaxis_mesh = new MeshGeometry3D();
            zaxis_mesh.AddSegment(origin, new Point3D(0, 0, axis_length), axis_thickness, true);
            DiffuseMaterial zaxis_material = new DiffuseMaterial(blue_brush);
            GeometryModel3D zaxis_model = new GeometryModel3D(zaxis_mesh, zaxis_material);
            MainModel3Dgroup.Children.Add(zaxis_model);
#endif

        }

    }
}
