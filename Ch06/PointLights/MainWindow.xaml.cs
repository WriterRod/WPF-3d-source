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

namespace PointLights
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

        // The model group.
        private Model3DGroup MainGroup = null;

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // Define WPF objects.
            ModelVisual3D visual3d = new ModelVisual3D();
            MainGroup = new Model3DGroup();
            visual3d.Content = MainGroup;
            mainViewport.Children.Add(visual3d);

            // Define the camera, lights, and model.
            DefineCamera(mainViewport);
            DefineLights(MainGroup);
            DefineModel(MainGroup);
        }

        // Define the camera.
        private void DefineCamera(Viewport3D viewport)
        {
            TheCamera = new PerspectiveCamera();
            TheCamera.FieldOfView = 60;
            CameraController = new SphericalCameraController
                (TheCamera, viewport, this, mainGrid, mainGrid);
        }

        #region Lights

        // Define the lights.
        private void DefineLights(Model3DGroup group)
        {
            MakeAmbientLight(Color.FromArgb(255, 64, 64, 64));
            MakeDirectionalLight(Colors.DarkGray, 1, -3, -2);
            //MakeDirectionalLight(Colors.DarkGray, 0, -1, 0);
            MakeDirectionalLight(Colors.DarkGray, -1, 3, 2);
            MakePointLight(Colors.White, 0, 0, 0);
        }

        // Make an ambient light and give it a CheckBox.
        private void MakeAmbientLight(Color color)
        {
            MakeLightCheckbox(new AmbientLight(color), "Ambient");
        }

        // Make a new directional light and give it a CheckBox.
        private void MakeDirectionalLight(Color color, double x, double y, double z)
        {
            MakeLightCheckbox(
                new DirectionalLight(color, new Vector3D(x, y, z)),
                $"<{x}, {y}, {z}>");
        }

        // Make a new point light and give it a CheckBox.
        private void MakePointLight(Color color, double x, double y, double z)
        {
            MakeLightCheckbox(
                new PointLight(color, new Point3D(x, y, z)),
                $"Point({x}, {y}, {z})");
        }

        // Make a new spot light and give it a CheckBox.
        private void MakeSpotLight(Color color, double x, double y, double z,
            double vx, double vy, double vz,
            double outerAngle, double innerAngle)
        {
            SpotLight light = new SpotLight(color,
                new Point3D(x, y, z),
                new Vector3D(vx, vy, vz),
                outerAngle, innerAngle);
            MakeLightCheckbox(light, $"Spot({x}, {y}, {z})");
        }

        // Make a CheckBox for a light.
        private void MakeLightCheckbox(Light light, string caption)
        {
            CheckBox chk = new CheckBox();
            chk.Tag = light;
            chk.Content = caption;
            chk.Click += DisplayLights;

            lightStackPanel.Children.Add(chk);
        }

        #endregion Lights

        // Define the model.
        private void DefineModel(Model3DGroup group)
        {
            const double dist = 2;
            const double width = 1.5;

            // Make a cube on the positive X axis.
            MeshGeometry3D x1_mesh = MakeCubeMesh(dist, 0, 0, width);
            DiffuseMaterial x1_material = new DiffuseMaterial(Brushes.LightBlue);
            GeometryModel3D x1_model = new GeometryModel3D(x1_mesh, x1_material);
            group.Children.Add(x1_model);

            // Make a cube on the negative X axis.
            MeshGeometry3D x2_mesh = MakeCubeMesh(-dist, 0, 0, width);
            DiffuseMaterial x2_material = new DiffuseMaterial(Brushes.LightBlue);
            GeometryModel3D x2_model = new GeometryModel3D(x2_mesh, x2_material);
            group.Children.Add(x2_model);

            // Make a cube on the positive Z axis.
            MeshGeometry3D z1_mesh = MakeCubeMesh(0, 0, dist, width);
            DiffuseMaterial z1_material = new DiffuseMaterial(Brushes.LightBlue);
            GeometryModel3D z1_model = new GeometryModel3D(z1_mesh, z1_material);
            group.Children.Add(z1_model);

            // Make a cube on the negative Z axis.
            MeshGeometry3D z2_mesh = MakeCubeMesh(0, 0, -dist, width);
            DiffuseMaterial z2_material = new DiffuseMaterial(Brushes.LightBlue);
            GeometryModel3D z2_model = new GeometryModel3D(z2_mesh, z2_material);
            group.Children.Add(z2_model);
        }

        // Add a rectangle to the mesh.
        private void AddRectangle(MeshGeometry3D mesh, Point3D p1, Point3D p2, Point3D p3, Point3D p4)
        {
            int index = mesh.Positions.Count;
            mesh.Positions.Add(p1);
            mesh.Positions.Add(p2);
            mesh.Positions.Add(p3);
            mesh.Positions.Add(p4);

            mesh.TriangleIndices.Add(index);
            mesh.TriangleIndices.Add(index + 1);
            mesh.TriangleIndices.Add(index + 2);

            mesh.TriangleIndices.Add(index);
            mesh.TriangleIndices.Add(index + 2);
            mesh.TriangleIndices.Add(index + 3);
        }

        // Make a mesh containing a cube centered at this point.
        private MeshGeometry3D MakeCubeMesh(double x, double y, double z, double width)
        {
            // Create the geometry.
            MeshGeometry3D mesh = new MeshGeometry3D();

            // Define the positions.
            width /= 2;
            Point3D[] points =
            {
                new Point3D(x - width, y - width, z - width),
                new Point3D(x + width, y - width, z - width),
                new Point3D(x + width, y - width, z + width),
                new Point3D(x - width, y - width, z + width),
                new Point3D(x - width, y - width, z + width),
                new Point3D(x + width, y - width, z + width),
                new Point3D(x + width, y + width, z + width),
                new Point3D(x - width, y + width, z + width),
                new Point3D(x + width, y - width, z + width),
                new Point3D(x + width, y - width, z - width),
                new Point3D(x + width, y + width, z - width),
                new Point3D(x + width, y + width, z + width),
                new Point3D(x + width, y + width, z + width),
                new Point3D(x + width, y + width, z - width),
                new Point3D(x - width, y + width, z - width),
                new Point3D(x - width, y + width, z + width),
                new Point3D(x - width, y - width, z + width),
                new Point3D(x - width, y + width, z + width),
                new Point3D(x - width, y + width, z - width),
                new Point3D(x - width, y - width, z - width),
                new Point3D(x - width, y - width, z - width),
                new Point3D(x - width, y + width, z - width),
                new Point3D(x + width, y + width, z - width),
                new Point3D(x + width, y - width, z - width),
            };
            foreach (Point3D point in points) mesh.Positions.Add(point);

            // Define the triangles.
            Tuple<int, int, int>[] triangles =
            {
                 new Tuple<int, int, int>(0, 1, 2),
                 new Tuple<int, int, int>(2, 3, 0),
                 new Tuple<int, int, int>(4, 5, 6),
                 new Tuple<int, int, int>(6, 7, 4),
                 new Tuple<int, int, int>(8, 9, 10),
                 new Tuple<int, int, int>(10, 11, 8),
                 new Tuple<int, int, int>(12, 13, 14),
                 new Tuple<int, int, int>(14, 15, 12),
                 new Tuple<int, int, int>(16, 17, 18),
                 new Tuple<int, int, int>(18, 19, 16),
                 new Tuple<int, int, int>(20, 21, 22),
                 new Tuple<int, int, int>(22, 23, 20),
            };
            foreach (Tuple<int, int, int> tuple in triangles)
            {
                mesh.TriangleIndices.Add(tuple.Item1);
                mesh.TriangleIndices.Add(tuple.Item2);
                mesh.TriangleIndices.Add(tuple.Item3);
            }

            return mesh;
        }

        // Display the selected the lights.
        private void DisplayLights(object sender, RoutedEventArgs e)
        {
            // Display the selected lights.
            foreach (CheckBox chk in lightStackPanel.Children)
            {
                ShowOrHideLight(chk.Tag as Light, chk.IsChecked.Value);
            }
        }

        // Show or hide this light.
        private void ShowOrHideLight(Light light, bool show)
        {
            if (show)
            {
                // Show it if necessary.
                if (!MainGroup.Children.Contains(light))
                    MainGroup.Children.Add(light);
            }
            else
            {
                // Hide it if necessary.
                if (MainGroup.Children.Contains(light))
                    MainGroup.Children.Remove(light);
            }
        }
    }
}
