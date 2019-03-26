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

namespace SierpinskiGasket
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

            group.Children.Add(new PointLight(Colors.White, new Point3D(0, 1, 1)));
        }

        // Counts.
        private int NumTetrahedrons;

        // Define the model.
        private void DefineModel()
        {
            // Show the axes.
            //MainGroup.Children.Add(MeshExtensions.XAxisModel(4));
            //MainGroup.Children.Add(MeshExtensions.YAxisModel(4));
            //MainGroup.Children.Add(MeshExtensions.ZAxisModel(4));
            //MainGroup.Children.Add(MeshExtensions.OriginModel());

            NumTetrahedrons = 0;

            // Get parameters.
            int depth = int.Parse(depthTextBox.Text);
            int height = int.Parse(heightTextBox.Text);

            // Get the volume where we will put it.
            Point3D A, B, C, D;
            G3.TetrahedronPoints(out A, out B, out C, out D, true);
            double scale = height / (A.Y - B.Y);
            A = new Point3D(A.X * scale, A.Y * scale, A.Z * scale);
            B = new Point3D(B.X * scale, B.Y * scale, B.Z * scale);
            C = new Point3D(C.X * scale, C.Y * scale, C.Z * scale);
            D = new Point3D(D.X * scale, D.Y * scale, D.Z * scale);

            // Make the gasket.
            MeshGeometry3D mesh = new MeshGeometry3D();
            MakeGasket(mesh, depth, A, B, C, D);
            MainGroup.Children.Add(mesh.MakeModel(Brushes.Yellow));

            Console.WriteLine("# Tetrahedrons: " + NumTetrahedrons);
        }

        // Make a 3D Sierpinski gasket.
        private void MakeGasket(MeshGeometry3D mesh, int depth,
            Point3D A, Point3D B, Point3D C, Point3D D)
        {
            // See if we are at the end of the recursion.
            if (depth == 0)
            {
                // Just draw the tetrahedron.
                mesh.AddPolygon(A, B, C);
                mesh.AddPolygon(A, C, D);
                mesh.AddPolygon(A, D, B);
                mesh.AddPolygon(D, C, B);
                NumTetrahedrons++;
            }
            else
            {
                // Divide the volume.
                depth--;
                Point3D AB = new Point3D(
                    (A.X + B.X) / 2,
                    (A.Y + B.Y) / 2,
                    (A.Z + B.Z) / 2);
                Point3D AC = new Point3D(
                    (A.X + C.X) / 2,
                    (A.Y + C.Y) / 2,
                    (A.Z + C.Z) / 2);
                Point3D AD = new Point3D(
                    (A.X + D.X) / 2,
                    (A.Y + D.Y) / 2,
                    (A.Z + D.Z) / 2);
                Point3D BC = new Point3D(
                    (B.X + C.X) / 2,
                    (B.Y + C.Y) / 2,
                    (B.Z + C.Z) / 2);
                Point3D CD = new Point3D(
                    (C.X + D.X) / 2,
                    (C.Y + D.Y) / 2,
                    (C.Z + D.Z) / 2);
                Point3D DB = new Point3D(
                    (D.X + B.X) / 2,
                    (D.Y + B.Y) / 2,
                    (D.Z + B.Z) / 2);
                MakeGasket(mesh, depth, A, AB, AC, AD);
                MakeGasket(mesh, depth, AB, B, BC, DB);
                MakeGasket(mesh, depth, AC, BC, C, CD);
                MakeGasket(mesh, depth, AD, DB, CD, D);
            }
        }

        // Generate the scene.
        private void generateButton_Click(object sender, RoutedEventArgs e)
        {
            DefineScene();
        }
    }
}
