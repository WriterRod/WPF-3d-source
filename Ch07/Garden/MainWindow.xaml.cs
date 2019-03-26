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

namespace Garden
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
            group.Children.Add(new AmbientLight(Color.FromArgb(255, 64, 64, 64)));

            group.Children.Add(new DirectionalLight(Colors.LightGray, new Vector3D(1, -3, -2)));
            group.Children.Add(new DirectionalLight(Colors.LightGray, new Vector3D(-1, 3, 2)));
        }

        // Define the model.
        private void DefineModel(Model3DGroup group)
        {
            // Rock sections.
            MeshGeometry3D rockMesh = new MeshGeometry3D();
            AddRectangle(rockMesh,
                new Point3D(-3, 0, -1),
                new Point3D(-3, 0, +1),
                new Point3D(-1, 0, +1),
                new Point3D(-1, 0, -1));
            AddRectangle(rockMesh,
                new Point3D(+1, 0, -1),
                new Point3D(+1, 0, +1),
                new Point3D(+3, 0, +1),
                new Point3D(+3, 0, -1));
            AddRectangle(rockMesh,
                new Point3D(-1, 0, +1),
                new Point3D(-1, 0, +3),
                new Point3D(+1, 0, +3),
                new Point3D(+1, 0, +1));

            ImageBrush rockBrush = new ImageBrush();
            rockBrush.ImageSource = new BitmapImage(new Uri("rocks.jpg", UriKind.Relative));
            Material rockMaterial = new DiffuseMaterial(rockBrush);
            GeometryModel3D rockModel = new GeometryModel3D(rockMesh, rockMaterial);
            group.Children.Add(rockModel);

            // Grass sections.
            MeshGeometry3D grassMesh = new MeshGeometry3D();
            AddRectangle(grassMesh,
                new Point3D(-3, 0, -3),
                new Point3D(-3, 0, -1),
                new Point3D(-1, 0, -1),
                new Point3D(-1, 0, -3));
            AddRectangle(grassMesh,
                new Point3D(-3, 0, +1),
                new Point3D(-3, 0, +3),
                new Point3D(-1, 0, +3),
                new Point3D(-1, 0, +1));
            AddRectangle(grassMesh,
                new Point3D(+1, 0, -3),
                new Point3D(+1, 0, -1),
                new Point3D(+3, 0, -1),
                new Point3D(+3, 0, -3));
            AddRectangle(grassMesh,
                new Point3D(+1, 0, +1),
                new Point3D(+1, 0, +3),
                new Point3D(+3, 0, +3),
                new Point3D(+3, 0, +1));

            ImageBrush grassBrush = new ImageBrush();
            grassBrush.ImageSource = new BitmapImage(new Uri("grass.jpg", UriKind.Relative));
            Material grassMaterial = new DiffuseMaterial(grassBrush);
            GeometryModel3D grassModel = new GeometryModel3D(grassMesh, grassMaterial);
            group.Children.Add(grassModel);

            // Water.
            MeshGeometry3D waterMesh = new MeshGeometry3D();
            AddRectangle(waterMesh,
                new Point3D(-1, 0, -1),
                new Point3D(-1, 0, +1),
                new Point3D(+1, 0, +1),
                new Point3D(+1, 0, -1));
            ImageBrush waterBrush = new ImageBrush();
            waterBrush.ImageSource = new BitmapImage(new Uri("water.jpg", UriKind.Relative));
            Material waterMaterial = new DiffuseMaterial(waterBrush);
            GeometryModel3D waterModel = new GeometryModel3D(waterMesh, waterMaterial);
            group.Children.Add(waterModel);

            // Cube brick face.
            MeshGeometry3D brickMesh = new MeshGeometry3D();
            AddRectangle(brickMesh,
                new Point3D(-1, 2, -1),
                new Point3D(-1, 0, -1),
                new Point3D(+1, 0, -1),
                new Point3D(+1, 2, -1));
            ImageBrush brickBrush = new ImageBrush();
            brickBrush.ImageSource = new BitmapImage(new Uri("bricks.jpg", UriKind.Relative));
            Material brickMaterial = new DiffuseMaterial(brickBrush);
            GeometryModel3D brickModel = new GeometryModel3D(brickMesh, brickMaterial);
            group.Children.Add(brickModel);

            // Cube metal face.
            MeshGeometry3D metalMesh = new MeshGeometry3D();
            AddRectangle(metalMesh,
                new Point3D(+1, 2, -1),
                new Point3D(+1, 0, -1),
                new Point3D(+1, 0, -3),
                new Point3D(+1, 2, -3));
            ImageBrush metalBrush = new ImageBrush();
            metalBrush.ImageSource = new BitmapImage(new Uri("metal.jpg", UriKind.Relative));
            Material metalMaterial = new DiffuseMaterial(metalBrush);
            GeometryModel3D metalModel = new GeometryModel3D(metalMesh, metalMaterial);
            group.Children.Add(metalModel);

            // Cube wood face.
            MeshGeometry3D woodMesh = new MeshGeometry3D();
            AddRectangle(woodMesh,
                new Point3D(-1, 2, -3),
                new Point3D(-1, 2, -1),
                new Point3D(+1, 2, -1),
                new Point3D(+1, 2, -3));
            ImageBrush woodBrush = new ImageBrush();
            woodBrush.ImageSource = new BitmapImage(new Uri("wood.jpg", UriKind.Relative));
            Material woodMaterial = new DiffuseMaterial(woodBrush);
            GeometryModel3D woodModel = new GeometryModel3D(woodMesh, woodMaterial);
            group.Children.Add(woodModel);

            // Cube fire face.
            MeshGeometry3D fireMesh = new MeshGeometry3D();
            AddRectangle(fireMesh,
                new Point3D(-1, 2, -3),
                new Point3D(-1, 0, -3),
                new Point3D(-1, 0, -1),
                new Point3D(-1, 2, -1));
            ImageBrush fireBrush = new ImageBrush();
            fireBrush.ImageSource = new BitmapImage(new Uri("fire.jpg", UriKind.Relative));
            Material fireMaterial = new DiffuseMaterial(fireBrush);
            GeometryModel3D fireModel = new GeometryModel3D(fireMesh, fireMaterial);
            group.Children.Add(fireModel);

            // Cube cloth face.
            MeshGeometry3D clothMesh = new MeshGeometry3D();
            AddRectangle(clothMesh,
                new Point3D(+1, 2, -3),
                new Point3D(+1, 0, -3),
                new Point3D(-1, 0, -3),
                new Point3D(-1, 2, -3));
            ImageBrush clothBrush = new ImageBrush();
            clothBrush.ImageSource = new BitmapImage(new Uri("cloth.jpg", UriKind.Relative));
            Material clothMaterial = new DiffuseMaterial(clothBrush);
            GeometryModel3D clothModel = new GeometryModel3D(clothMesh, clothMaterial);
            group.Children.Add(clothModel);

            // Skybox meshes.
            MeshGeometry3D sky1Mesh = new MeshGeometry3D();
            AddRectangle(sky1Mesh,
                new Point3D(-6, +7, +6),
                new Point3D(-6, -5, +6),
                new Point3D(-6, -5, -6),
                new Point3D(-6, +7, -6));
            ImageBrush sky1Brush = new ImageBrush();
            sky1Brush.ImageSource = new BitmapImage(new Uri("clouds.jpg", UriKind.Relative));
            MaterialGroup sky1Group = new MaterialGroup();
            sky1Group.Children.Add(new DiffuseMaterial(sky1Brush));
            sky1Group.Children.Add(new EmissiveMaterial(new SolidColorBrush(
                Color.FromArgb(255, 128, 128, 128))));
            GeometryModel3D sky1Model = new GeometryModel3D(sky1Mesh, sky1Group);
            group.Children.Add(sky1Model);

            MeshGeometry3D sky2Mesh = new MeshGeometry3D();
            AddRectangle(sky2Mesh,
                new Point3D(-6, +7, -6),
                new Point3D(-6, -5, -6),
                new Point3D(+6, -5, -6),
                new Point3D(+6, +7, -6));
            ImageBrush sky2Brush = new ImageBrush();
            sky2Brush.ImageSource = new BitmapImage(new Uri("clouds.jpg", UriKind.Relative));
            MaterialGroup sky2Group = new MaterialGroup();
            sky2Group.Children.Add(new DiffuseMaterial(sky2Brush));
            sky2Group.Children.Add(new EmissiveMaterial(new SolidColorBrush(
                Color.FromArgb(255, 64, 64, 64))));
            GeometryModel3D sky2Model = new GeometryModel3D(sky2Mesh, sky2Group);
            group.Children.Add(sky2Model);

            MeshGeometry3D sky3Mesh = new MeshGeometry3D();
            AddRectangle(sky3Mesh,
                new Point3D(-6, -5, +6),
                new Point3D(+6, -5, +6),
                new Point3D(+6, -5, -6),
                new Point3D(-6, -5, -6));
            ImageBrush sky3Brush = new ImageBrush();
            sky3Brush.ImageSource = new BitmapImage(new Uri("clouds.jpg", UriKind.Relative));
            Material sky3Material = new DiffuseMaterial(sky3Brush);
            GeometryModel3D sky3Model = new GeometryModel3D(sky3Mesh, sky3Material);
            group.Children.Add(sky3Model);
        }

        // Add a rectangle with texture coordinates to the mesh.
        private void AddRectangle(MeshGeometry3D mesh,
            Point3D p1, Point3D p2, Point3D p3, Point3D p4)
        {
            int index = mesh.Positions.Count;
            mesh.Positions.Add(p1);
            mesh.Positions.Add(p2);
            mesh.Positions.Add(p3);
            mesh.Positions.Add(p4);

            mesh.TextureCoordinates.Add(new Point(0, 0));
            mesh.TextureCoordinates.Add(new Point(0, 1));
            mesh.TextureCoordinates.Add(new Point(1, 1));
            mesh.TextureCoordinates.Add(new Point(1, 0));

            mesh.TriangleIndices.Add(index);
            mesh.TriangleIndices.Add(index + 1);
            mesh.TriangleIndices.Add(index + 2);

            mesh.TriangleIndices.Add(index);
            mesh.TriangleIndices.Add(index + 2);
            mesh.TriangleIndices.Add(index + 3);
        }
    }
}
