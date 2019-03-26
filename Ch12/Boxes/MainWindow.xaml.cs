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

namespace Boxes
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
            Color dark = Color.FromArgb(255, 128, 128, 128);

            group.Children.Add(new AmbientLight(dark));

            group.Children.Add(new DirectionalLight(dark, new Vector3D(0,-1, 0)));
            group.Children.Add(new DirectionalLight(dark, new Vector3D(1, -3, -2)));
            group.Children.Add(new DirectionalLight(dark, new Vector3D(-1, 3, 2)));
        }

        // Define the model.
        private void DefineModel(Model3DGroup group)
        {
            // Make a box.
            MeshGeometry3D mesh1 = new MeshGeometry3D();
            mesh1.AddBox(new Point3D(1, -1, 0),
                D3.XVector(2), D3.YVector(2), D3.ZVector(2),
                D3.UnitTextures);
            group.Children.Add(mesh1.MakeModel("wood.jpg"));

            // Make a cube with every face different.
            MeshGeometry3D mesh2 = new MeshGeometry3D();
            Point[][] textureCoords = D3.SectionTextureCoords(2, 3);
            mesh2.AddBox(new Point3D(0, 1, -2),
                D3.XVector(2), D3.YVector(2), D3.ZVector(2),
                textureCoords[0], textureCoords[1], textureCoords[2],
                textureCoords[3], textureCoords[4], textureCoords[5]);
            group.Children.Add(mesh2.MakeModel("all.jpg"));

            // Make a green parallelepiped.
            MeshGeometry3D mesh3 = new MeshGeometry3D();
            mesh3.AddBox(new Point3D(-4, -1, -3),
                new Vector3D(2, 0.5, -0.5),
                new Vector3D(0, 2, -0.5),
                new Vector3D(-0.5, 0, 2));
            group.Children.Add(mesh3.MakeModel(Brushes.LightGreen));

            // Make a wrapped box.
            MeshGeometry3D mesh4 = new MeshGeometry3D();
            mesh4.AddBoxWrapped(new Point3D(-3, -1, 0),
                D3.XVector(2), D3.YVector(2), D3.ZVector(2));
            group.Children.Add(mesh4.MakeModel("wrapper.png"));

            // Show the axes.
            MeshExtensions.AddAxes(group);
        }
    }
}
