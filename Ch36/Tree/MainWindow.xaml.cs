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

namespace Tree
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
        private int NumBranches;

        // Define the model.
        private void DefineModel()
        {
            NumBranches = 0;

            // Show the axes.
            //MainGroup.Children.Add(MeshExtensions.XAxisModel(4));
            //MainGroup.Children.Add(MeshExtensions.YAxisModel(4));
            //MainGroup.Children.Add(MeshExtensions.ZAxisModel(4));
            //MainGroup.Children.Add(MeshExtensions.OriginModel());

            // Create the tree model.
            int depth = int.Parse(depthTextBox.Text);
            double length = double.Parse(lengthTextBox.Text);
            double radius = double.Parse(radiusTextBox.Text);
            double angle = double.Parse(angleTextBox.Text);
            double lengthFactor = double.Parse(lengthFactorTextBox.Text);
            double radiusFactor = double.Parse(radiusFactorTextBox.Text);
            int numBranches = int.Parse(numBranchesTextBox.Text);

            MeshGeometry3D treeMesh = new MeshGeometry3D();
            DrawBranch(treeMesh, depth, length, radius, angle,
                lengthFactor, radiusFactor, numBranches,
                new Point3D(0, -length, 0),
                new Vector3D(0, length, 0), 6);

            MainGroup.Children.Add(treeMesh.MakeModel(Brushes.LightBlue));

            Console.WriteLine("# Branches: " + NumBranches);
        }

        // Draw a branch in the indicated direction.
        private void DrawBranch(MeshGeometry3D mesh,
            int depth, double length, double radius, double angle,
            double lengthFactor, double radiusFactor, int numBranches,
            Point3D startPoint, Vector3D axis, int numSides)
        {
            // Set the branch's length.
            axis = axis.Scale(length);

            // Get two vectors perpendicular to the axis.
            Vector3D v1;
            if (Vector3D.AngleBetween(axis, D3.ZVector()) > 0.1)
                v1 = Vector3D.CrossProduct(axis, D3.ZVector());
            else
                v1 = Vector3D.CrossProduct(axis, D3.XVector());
            Vector3D v2 = Vector3D.CrossProduct(axis, v1);
            v1 = v1.Scale(radius);
            v2 = v2.Scale(radius);

            // Make the cone's base.
            Point3D[] pgon = G3.MakePolygonPoints(numSides, startPoint, v1, v2);

            // Make the branch.
            mesh.AddCylinder(pgon, axis, true);

            // Draw child branches.
            if (depth <= 0) return;

            // Move to the end of this segment.
            depth--;
            startPoint += axis;
            length *= lengthFactor;
            if (length < 0.01) length = 0.01;
            radius *= radiusFactor;
            if (radius < 0.001) radius = 0.001;

            // Find child vectors.
            List<Vector3D> children = axis.MakeFlowerVectors(
                D3.YVector(), D3.ZVector(), angle, numBranches);

            // Draw the child branches.
            foreach (Vector3D child in children)
            {
                DrawBranch(mesh, depth, length,
                    radius, angle, lengthFactor, radiusFactor, numBranches,
                    startPoint, child, numSides);
            }
        }

        // Generate the scene.
        private void generateButton_Click(object sender, RoutedEventArgs e)
        {
            DefineScene();
        }
    }
}
