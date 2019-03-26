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

namespace Spheres
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
                (TheCamera, viewport, this, MainGrid, MainGrid);
        }

        // Define the lights.
        private void DefineLights(Model3DGroup group)
        {
            Color dark = Color.FromArgb(255, 96, 96, 96);

            group.Children.Add(new AmbientLight(dark));
            
            group.Children.Add(new DirectionalLight(Colors.White, new Vector3D(0, -1, 0)));
        }

        // Define the model.
        private void DefineModel(Model3DGroup group)
        {
            MeshGeometry3D mesh1 = new MeshGeometry3D();
            Point3D center = new Point3D(-2, 0, 2);
            mesh1.AddTexturedSphere(center, 1.5, 20, 10, true);
            group.Children.Add(mesh1.MakeModel("world.jpg"));
            // Add a point to redefine the texture area to hide the "seam."
            mesh1.Positions.Add(new Point3D());
            mesh1.TextureCoordinates.Add(new Point(1.01, 1.01));

            MeshGeometry3D mesh2 = new MeshGeometry3D();
            center = new Point3D(-2, 0, -2);
            mesh2.AddTexturedSphere(center, 1.5, 20, 10, true);
            group.Children.Add(mesh2.MakeModel("world.jpg"));
            // Add a point to redefine the texture area to hide the "seam."
            mesh2.Positions.Add(new Point3D());
            mesh2.TextureCoordinates.Add(new Point(1.01, 1.01));

            // Modify the normals.
            for (int i = 0; i < mesh2.Normals.Count; i++)
            {
                if (mesh2.Normals[i].X > 0)
                    mesh2.Normals[i] *= 1000;
                else
                    mesh2.Normals[i] /= 1000;
                // Console.WriteLine(mesh2.Normals[i].Length);
            }

            MeshGeometry3D mesh3 = new MeshGeometry3D();
            center = new Point3D(2, 0, 2);
            mesh3.AddTexturedSphere(center, 1.5, 20, 10, true);
            //mesh3.ApplyTransformation(new ScaleTransform3D(new Vector3D(1, 0.5, 1), center));
            mesh3.ApplyTransformation(D3.Rotate(D3.XVector(), center, 90));
            group.Children.Add(mesh3.MakeModel("world.jpg"));
            // Add a point to redefine the texture area to hide the "seam."
            mesh3.Positions.Add(new Point3D());
            mesh3.TextureCoordinates.Add(new Point(1.01, 1.01));

            MeshGeometry3D mesh4 = new MeshGeometry3D();
            center = new Point3D(2, 0, -2);
            mesh4.AddTexturedSphere(center, 1.5, 20, 10, true);
            //mesh4.ApplyTransformation(new ScaleTransform3D(new Vector3D(1, 0.5, 1), center));
            mesh4.ApplyTransformation(D3.Rotate(D3.XVector(), center, 90));
            group.Children.Add(mesh4.MakeModel("world.jpg"));
            // Add a point to redefine the texture area to hide the "seam."
            mesh4.Positions.Add(new Point3D());
            mesh4.TextureCoordinates.Add(new Point(1.01, 1.01));

            // Modify the normals.
            for (int i = 0; i < mesh4.Normals.Count; i++)
            {
                if (mesh4.Normals[i].X > 0)
                    mesh4.Normals[i] *= 1000;
                else
                    mesh4.Normals[i] /= 1000;
                // Console.WriteLine(mesh4.Normals[i].Length);
            }

            // Show the axes.
            MeshExtensions.AddAxes(group);
        }
    }
}
