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

namespace ImageScaling
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
            group.Children.Add(new AmbientLight(Color.FromArgb(255, 128, 128, 128)));
            group.Children.Add(new AmbientLight(Color.FromArgb(255, 128, 128, 128)));
        }

        // Define the model.
        private void DefineModel(Model3DGroup group)
        {
            // The whole thing.
            MeshGeometry3D mesh1 = new MeshGeometry3D();
            AddRectangle(mesh1,
                new Point3D(0, 3.25, 3.25),
                new Point3D(0, 1.25, 3.25),
                new Point3D(0, 1.25, 1.25),
                new Point3D(0, 3.25, 1.25),
                0, 1, 0, 1);
            ImageBrush brush1 = new ImageBrush(
                new BitmapImage(new Uri("Smiley.png", UriKind.Relative)));
            GeometryModel3D model1 = new GeometryModel3D(mesh1, new DiffuseMaterial(brush1));
            group.Children.Add(model1);

            // Viewbox = upper left corner of image
            MeshGeometry3D mesh2 = new MeshGeometry3D();
            AddRectangle(mesh2,
                new Point3D(0, 3.25, 1),
                new Point3D(0, 1.25, 1),
                new Point3D(0, 1.25, -1),
                new Point3D(0, 3.25, -1),
                0, 1, 0, 1);
            ImageBrush brush2 = new ImageBrush(
                new BitmapImage(new Uri("Smiley.png", UriKind.Relative)));
            brush2.Viewbox = new Rect(0, 0, 0.5, 0.5);
            GeometryModel3D model2 = new GeometryModel3D(mesh2, new DiffuseMaterial(brush2));
            group.Children.Add(model2);

            // Viewbox = the whole image
            // Viewport = the upper half of the graphic
            // Tile
            MeshGeometry3D mesh3 = new MeshGeometry3D();
            AddRectangle(mesh3,
                new Point3D(0, 3.25, -1.25),
                new Point3D(0, 1.25, -1.25),
                new Point3D(0, 1.25, -3.25),
                new Point3D(0, 3.25, -3.25),
                0, 1, 0, 1);
            ImageBrush brush3 = new ImageBrush(
                new BitmapImage(new Uri("Smiley.png", UriKind.Relative)));
            brush3.Viewport = new Rect(0, 0, 0.5, 0.5);
            brush3.TileMode = TileMode.Tile;
            GeometryModel3D model3 = new GeometryModel3D(mesh3, new DiffuseMaterial(brush3));
            group.Children.Add(model3);

            // Viewbox = the whole image
            // Viewport = the upper half of the graphic
            // Stretch and tile
            MeshGeometry3D mesh4 = new MeshGeometry3D();
            AddRectangle(mesh4,
                new Point3D(0, 1, 3.25),
                new Point3D(0, -1, 3.25),
                new Point3D(0, -1, 1.25),
                new Point3D(0, 1, 1.25),
                0, 1, 0, 1);
            ImageBrush brush4 = new ImageBrush(
                new BitmapImage(new Uri("Smiley.png", UriKind.Relative)));
            brush4.Viewport = new Rect(0, 0, 1, 0.5);
            brush4.Stretch = Stretch.Fill;
            brush4.TileMode = TileMode.Tile;
            GeometryModel3D model4 = new GeometryModel3D(mesh4, new DiffuseMaterial(brush4));
            group.Children.Add(model4);

            // Viewbox = the whole image
            // Viewport = the graphic's upper left corner
            // Tile = FlipXY
            MeshGeometry3D mesh5 = new MeshGeometry3D();
            AddRectangle(mesh5,
                new Point3D(0, 1, 1),
                new Point3D(0, -1, 1),
                new Point3D(0, -1, -1),
                new Point3D(0, 1, -1),
                0, 1, 0, 1);
            ImageBrush brush5 = new ImageBrush(
                new BitmapImage(new Uri("Smiley.png", UriKind.Relative)));
            brush5.Viewport = new Rect(0, 0, 0.5, 0.5);
            brush5.TileMode = TileMode.FlipXY;
            GeometryModel3D model5 = new GeometryModel3D(mesh5, new DiffuseMaterial(brush5));
            group.Children.Add(model5);

            // Viewbox = the middle of the image
            // Viewport = the lower right corner of the graphic
            // No tile
            MeshGeometry3D mesh6 = new MeshGeometry3D();
            AddRectangle(mesh6,
                new Point3D(0, 1, -1.25),
                new Point3D(0, -1, -1.25),
                new Point3D(0, -1, -3.25),
                new Point3D(0, 1, -3.25),
                0, 1, 0, 1);
            ImageBrush brush6 = new ImageBrush(
                new BitmapImage(new Uri("Smiley.png", UriKind.Relative)));
            brush6.Viewbox = new Rect(0.25, 0.25, 0.5, 0.5);
            brush6.Viewport = new Rect(0.5, 0.5, 0.5, 0.5);
            GeometryModel3D model6 = new GeometryModel3D(mesh6, new DiffuseMaterial(brush6));
            group.Children.Add(model6);

            // The simple case for two pieces.
            MeshGeometry3D mesh7 = new MeshGeometry3D();
            AddRectangle(mesh7,
                new Point3D(0, -1.25, 3.25),
                new Point3D(0, -2.25, 3.25),
                new Point3D(0, -2.25, 2.25),
                new Point3D(0, -1.25, 2.25),
                0, 0.5, 0, 0.5);
            AddRectangle(mesh7,
                new Point3D(0, -2.25, 2.25),
                new Point3D(0, -3.25, 2.25),
                new Point3D(0, -3.25, 1.25),
                new Point3D(0, -2.25, 1.25),
                0.5, 1, 0.5, 1);
            ImageBrush brush7 = new ImageBrush(
                new BitmapImage(new Uri("Smiley.png", UriKind.Relative)));
            GeometryModel3D model7 = new GeometryModel3D(mesh7, new DiffuseMaterial(brush7));
            group.Children.Add(model7);

            // Strange default behavior
            // One piece with texture coordinates (0, 0)-(0.5, 0.5)
            MeshGeometry3D mesh8 = new MeshGeometry3D();
            AddRectangle(mesh8,
                new Point3D(0, -2.25, 0),
                new Point3D(0, -3.25, 0),
                new Point3D(0, -3.25, -1),
                new Point3D(0, -2.25, -1),
                0.5, 1, 0.5, 1);
            ImageBrush brush8 = new ImageBrush(
                new BitmapImage(new Uri("Smiley.png", UriKind.Relative)));
            GeometryModel3D model8 = new GeometryModel3D(mesh8, new DiffuseMaterial(brush8));
            group.Children.Add(model8);

            // One piece with texture coordinates (0, 0)-(0.5, 0.5)
            // ViewportUnits = Absolute
            MeshGeometry3D mesh9 = new MeshGeometry3D();
            AddRectangle(mesh9,
                new Point3D(0, -2.25, -2.25),
                new Point3D(0, -3.25, -2.25),
                new Point3D(0, -3.25, -3.25),
                new Point3D(0, -2.25, -3.25),
                0.5, 1, 0.5, 1);
            ImageBrush brush9 = new ImageBrush(
                new BitmapImage(new Uri("Smiley.png", UriKind.Relative)));
            brush9.ViewportUnits = BrushMappingMode.Absolute;
            GeometryModel3D model9 = new GeometryModel3D(mesh9, new DiffuseMaterial(brush9));
            group.Children.Add(model9);
        }

        // Add a rectangle with texture coordinates to the mesh.
        private void AddRectangle(MeshGeometry3D mesh,
            Point3D p1, Point3D p2, Point3D p3, Point3D p4,
            double umin, double umax, double vmin, double vmax)
        {
            int index = mesh.Positions.Count;
            mesh.Positions.Add(p1);
            mesh.Positions.Add(p2);
            mesh.Positions.Add(p3);
            mesh.Positions.Add(p4);

            mesh.TextureCoordinates.Add(new Point(umin, vmin));
            mesh.TextureCoordinates.Add(new Point(umin, vmax));
            mesh.TextureCoordinates.Add(new Point(umax, vmax));
            mesh.TextureCoordinates.Add(new Point(umax, vmin));

            mesh.TriangleIndices.Add(index);
            mesh.TriangleIndices.Add(index + 1);
            mesh.TriangleIndices.Add(index + 2);

            mesh.TriangleIndices.Add(index);
            mesh.TriangleIndices.Add(index + 2);
            mesh.TriangleIndices.Add(index + 3);
        }
    }
}
