#define MONOCHROME
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
        private PerspectiveCamera TheCamera;

        // The camera's current location.
        private double CameraPhi = Math.PI / 6.0;       // 30 degrees
        private double CameraTheta = Math.PI / 6.0;     // 30 degrees
        private double CameraR = 8.0;

        // The change in CameraPhi when you press the up and down arrows.
        private const double CameraDPhi = 0.1;

        // The change in CameraTheta when you press the left and right arrows.
        private const double CameraDTheta = 0.1;

        // The change in CameraR when you press + or -.
        private const double CameraDR = 0.1;

        // Create the scene.
        // MainViewport is the Viewport3D defined
        // in the XAML code that displays everything.
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // Give the camera its initial position.
            TheCamera = new PerspectiveCamera();
            TheCamera.FieldOfView = 60;
            MainViewport.Camera = TheCamera;
            PositionCamera();

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
            AmbientLight ambient_light = new AmbientLight(Color.FromArgb(255, 128, 128, 128));
            Color gray = Color.FromArgb(255, 64, 64, 64);
            DirectionalLight directional_light =
                new DirectionalLight(Color.FromArgb(255, 64, 64, 64),
                    new Vector3D(1, -1, -1));
            DirectionalLight directional_light2 =
                new DirectionalLight(Color.FromArgb(255, 64, 64, 64),
                    new Vector3D(0, -1, 0));
            DirectionalLight directional_light3 =
                new DirectionalLight(Color.FromArgb(255, 64, 64, 64),
                    new Vector3D(0, 0, -1));
            MainModel3Dgroup.Children.Add(ambient_light);
            MainModel3Dgroup.Children.Add(directional_light);
            MainModel3Dgroup.Children.Add(directional_light2);
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

            // Octahedron.
            MeshGeometry3D octo_mesh = new MeshGeometry3D();
            octo_mesh.AddOctahedron(3, null);
            DiffuseMaterial octo_material = new DiffuseMaterial(lightergreen_brush);
            GeometryModel3D octo_model = new GeometryModel3D(octo_mesh, octo_material);
            MainModel3Dgroup.Children.Add(octo_model);

            // Cube.
            double s = octo_mesh.Positions[0].Y;
            MeshGeometry3D cube_mesh = new MeshGeometry3D();
            cube_mesh.AddCube(s, null);
            DiffuseMaterial cube_material = new DiffuseMaterial(pink_brush);
            GeometryModel3D cube_model = new GeometryModel3D(cube_mesh, cube_material);
            MainModel3Dgroup.Children.Add(cube_model);

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

        // Adjust the camera's position.
        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Up:
                    CameraPhi += CameraDPhi;
                    if (CameraPhi > Math.PI / 2.0) CameraPhi = Math.PI / 2.0;
                    break;
                case Key.Down:
                    CameraPhi -= CameraDPhi;
                    if (CameraPhi < -Math.PI / 2.0) CameraPhi = -Math.PI / 2.0;
                    break;
                case Key.Left:
                    CameraTheta += CameraDTheta;
                    break;
                case Key.Right:
                    CameraTheta -= CameraDTheta;
                    break;
                //case Key.Add:
                //case Key.OemPlus:
                //    CameraR -= CameraDR;
                //    if (CameraR < CameraDR) CameraR = CameraDR;
                //    break;
                //case Key.Subtract:
                //case Key.OemMinus:
                //    CameraR += CameraDR;
                //    break;
            }

            // Update the camera's position.
            PositionCamera();
        }

        // Position the camera.
        private void PositionCamera()
        {
            // Calculate the camera's position in Cartesian coordinates.
            double y = CameraR * Math.Sin(CameraPhi);
            double hyp = CameraR * Math.Cos(CameraPhi);
            double x = hyp * Math.Cos(CameraTheta);
            double z = hyp * Math.Sin(CameraTheta);
            TheCamera.Position = new Point3D(x, y, z);

            // Look toward the origin.
            TheCamera.LookDirection = new Vector3D(-x, -y, -z);

            // Set the Up direction.
            TheCamera.UpDirection = new Vector3D(0, 1, 0);
        }

        // Zoom in or out.
        private void Window_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            CameraR += Math.Sign(e.Delta) * CameraDR;
            PositionCamera();
        }

        // Let the user change CameraPhi and CameraTheta.
        private Point LastPoint;
        private void MainBorder_MouseDown(object sender, MouseButtonEventArgs e)
        {
            MainBorder.MouseMove += new MouseEventHandler(MainBorder_MouseMove);
            MainBorder.MouseUp += new MouseButtonEventHandler(MainBorder_MouseUp);
            LastPoint = e.GetPosition(MainBorder);
        }

        private void MainBorder_MouseMove(object sender, MouseEventArgs e)
        {
            const double x_scale = 0.1;
            const double y_scale = 0.1;

            Point new_point = e.GetPosition(MainBorder);
            double dx = new_point.X - LastPoint.X;
            double dy = new_point.Y - LastPoint.Y;

            CameraTheta += dx * CameraDTheta * x_scale;
            CameraPhi += dy * CameraDPhi * y_scale;

            LastPoint = new_point;
            PositionCamera();
        }

        private void MainBorder_MouseUp(object sender, MouseButtonEventArgs e)
        {
            MainBorder.MouseMove -= new MouseEventHandler(MainBorder_MouseMove);
            MainBorder.MouseUp -= new MouseButtonEventHandler(MainBorder_MouseUp);
        }
    }
}
