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

namespace Oblique
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
            // Viewport 1.
            ModelVisual3D visual3d1 = new ModelVisual3D();
            Model3DGroup group3d1 = new Model3DGroup();
            visual3d1.Content = group3d1;
            viewport1.Children.Add(visual3d1);
            DefineCamera(viewport1, 0, 0, 3);
            DefineLights(group3d1);
            MeshGeometry3D mesh1;
            DefineModel(group3d1, out mesh1);
            MakeOblique(mesh1, 30, 1);

            // Viewport 2.
            ModelVisual3D visual3d2 = new ModelVisual3D();
            Model3DGroup group3d2 = new Model3DGroup();
            visual3d2.Content = group3d2;
            viewport2.Children.Add(visual3d2);
            DefineCamera(viewport2, 0, 0, 3);
            DefineLights(group3d2);
            MeshGeometry3D mesh2;
            DefineModel(group3d2, out mesh2);
            MakeOblique(mesh2, 30, 0.5);
        }

        // Transform the points for a cavalier or cabinet projection.
        private void MakeOblique(MeshGeometry3D mesh, double angle, double scale)
        {
            angle *= Math.PI / 180;
            double sin = Math.Sin(angle) * scale;
            double cos = Math.Cos(angle) * scale;

            for (int i = 0; i < mesh.Positions.Count; i++)
            {
                double x = mesh.Positions[i].X;
                double y = mesh.Positions[i].Y;
                double z = mesh.Positions[i].Z;
                mesh.Positions[i] =
                    new Point3D(x - cos * z, y - sin * z, z);
            }
        }

        // Define the camera.
        private void DefineCamera(Viewport3D viewport, double x, double y, double z)
        {
            Point3D position = new Point3D(x, y, z);

            // Look back toward the origin.
            Vector3D lookDirection = new Vector3D(
                -position.X,
                -position.Y,
                -position.Z);

            Vector3D upDirection = new Vector3D(0, 1, 0);
            double width = 4;
            OrthographicCamera camera =
                new OrthographicCamera(position, lookDirection, upDirection, width);

            viewport.Camera = camera;
        }

        // Define the lights.
        private void DefineLights(Model3DGroup group)
        {
            group.Children.Add(new AmbientLight(Colors.Gray));

            Vector3D direction = new Vector3D(1, -2, -3);
            group.Children.Add(new DirectionalLight(Colors.Gray, direction));

            Vector3D direction2 = new Vector3D(0, -1, 0);
            group.Children.Add(new DirectionalLight(Colors.Gray, direction2));
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
