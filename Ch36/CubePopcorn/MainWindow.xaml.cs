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

namespace CubePopcorn
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

        // The main model group.
        private Model3DGroup MainGroup;

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // Define WPF objects.
            ModelVisual3D visual3d = new ModelVisual3D();
            MainGroup = new Model3DGroup();
            visual3d.Content = MainGroup;
            mainViewport.Children.Add(visual3d);

            // Define the camera, lights, and model.
            DefineCamera(mainViewport);

            // Define the lights and model.
            DefineScene();
        }

        // Define the lights and model.
        private void DefineScene()
        {
            Cursor = Cursors.Wait;
            MainGroup.Children.Clear();
            DefineLights(MainGroup);
            DefineModel();
            Cursor = null;
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
            Color darker = Color.FromArgb(255, 64, 64, 64);
            Color dark = Color.FromArgb(255, 96, 96, 96);

            group.Children.Add(new AmbientLight(darker));

            group.Children.Add(new DirectionalLight(dark, new Vector3D(0, -1, 0)));
            group.Children.Add(new DirectionalLight(dark, new Vector3D(1, -3, -2)));
            group.Children.Add(new DirectionalLight(dark, new Vector3D(-1, 3, 2)));
        }

        // Counts.
        private int NumBoxes;

        // Define the model.
        private void DefineModel()
        {
            NumBoxes = 0;

            // Show the axes.
            //MainGroup.Children.Add(MeshExtensions.XAxisModel(4));
            //MainGroup.Children.Add(MeshExtensions.YAxisModel(4));
            //MainGroup.Children.Add(MeshExtensions.ZAxisModel(4));
            //MainGroup.Children.Add(MeshExtensions.OriginModel());

            // Make locations for recursive popcorn.
            double df = double.Parse(distFactorTextBox.Text);
            List<Vector3D> directions = new List<Vector3D>();
            directions.Add(new Vector3D(-df, -df, -df));
            directions.Add(new Vector3D(-df, -df, df));
            directions.Add(new Vector3D(-df, df, -df));
            directions.Add(new Vector3D(-df, df, df));
            directions.Add(new Vector3D(df, -df, -df));
            directions.Add(new Vector3D(df, -df, df));
            directions.Add(new Vector3D(df, df, -df));
            directions.Add(new Vector3D(df, df, df));

            // Get parameters.
            int depth = int.Parse(depthTextBox.Text);

            double r = double.Parse(rTextBox.Text);
            double g = double.Parse(gTextBox.Text);
            double b = double.Parse(bTextBox.Text);
            double colorFactor = double.Parse(colorFactorTextBox.Text);

            double radius = double.Parse(radiusTextBox.Text);
            double radiusFactor = double.Parse(radiusFactorTextBox.Text);

            // Make boxes.
            MeshGeometry3D mesh = new MeshGeometry3D();
            MakeBoxPopcorn(mesh, depth, D3.Origin, directions,
                r, g, b, colorFactor, radius, radiusFactor);
            MainGroup.Children.Add(mesh.MakeModel(Brushes.Orange));

            Console.WriteLine("# Boxes: " + NumBoxes);
        }

        // Make cube popcorn.
        private void MakeBoxPopcorn(MeshGeometry3D mesh, int depth,
            Point3D center, List<Vector3D> directions,
            double r, double g, double b, double colorFactor,
            double radius, double radiusFactor)
        {
            // Add a cube to the mesh.
            Point3D corner = center + new Vector3D(-radius, -radius, -radius);
            mesh.AddBox(corner, D3.XVector(2 * radius), D3.YVector(2 * radius), D3.ZVector(2 * radius));
            NumBoxes++;

            // See if we're done.
            if (--depth < 0) return;

            // Make smaller boxes.
            double newRadius = radius * radiusFactor;
            r *= colorFactor;
            g *= colorFactor;
            b *= colorFactor;
            foreach (Vector3D direction in directions)
            {
                Point3D newCenter = center + (radius + newRadius) * direction;
                MakeBoxPopcorn(mesh, depth, newCenter, directions,
                    r, g, b, colorFactor, newRadius, radiusFactor);
            }
        }

        // Generate the scene.
        private void generateButton_Click(object sender, RoutedEventArgs e)
        {
            DefineScene();
        }
    }
}
