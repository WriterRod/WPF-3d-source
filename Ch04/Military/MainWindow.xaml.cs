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

namespace Military
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

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // Define WPF objects.
            ModelVisual3D visual3d = new ModelVisual3D();
            Model3DGroup group3d = new Model3DGroup();
            visual3d.Content = group3d;
            mainViewport.Children.Add(visual3d);

            // Define the camera, lights, and model.
            DefineCamera(mainViewport);
            DefineLights(group3d);
            MeshGeometry3D mesh;
            DefineModel(group3d, out mesh);

            // Transform points for a military projection.
            MakeMilitary(mesh, 0.5);
        }

        // Transform points for a military projection.
        private void MakeMilitary(MeshGeometry3D mesh, double scale)
        {
            scale = scale / Math.Sqrt(2);
            for (int i = 0; i < mesh.Positions.Count; i++)
            {
                double x = mesh.Positions[i].X;
                double y = mesh.Positions[i].Y;
                double z = mesh.Positions[i].Z;
                mesh.Positions[i] = new Point3D(
                    x - scale * y,
                    y,
                    z - scale * y);
            }
        }

        // Define the camera.
        private void DefineCamera(Viewport3D viewport)
        {
            Point3D position = new Point3D(0, 3, 0);
            Vector3D lookDirection = new Vector3D(
                -position.X, -position.Y, -position.Z);
            Vector3D upDirection = new Vector3D(-1, 0, -1);
            double width = 5;
            OrthographicCamera camera =
                new OrthographicCamera(position, lookDirection, upDirection, width);

            viewport.Camera = camera;
        }

        // Define the lights.
        private void DefineLights(Model3DGroup group)
        {
            //group.Children.Add(new AmbientLight(Colors.Gray));

            Vector3D xdirection = new Vector3D(-1, 0, 0);
            group.Children.Add(new DirectionalLight(
                Color.FromArgb(255, 32, 32, 32),
                xdirection));

            Vector3D ydirection = new Vector3D(0, -1, 0);
            group.Children.Add(new DirectionalLight(
                Color.FromArgb(255, 255, 255, 255),
                ydirection));

            Vector3D zdirection = new Vector3D(0, 0, -1);
            group.Children.Add(new DirectionalLight(
                Color.FromArgb(255, 128, 128, 128),
                zdirection));
        }

        // Define the model.
        private void DefineModel(Model3DGroup group, out MeshGeometry3D mesh)
        {
            // Make a cube.
            mesh = MakeCubeMesh(0, 0, 0, 2);
            DiffuseMaterial material = new DiffuseMaterial(Brushes.LightBlue);
            GeometryModel3D model = new GeometryModel3D(mesh, material);
            group.Children.Add(model);
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
    }
}
