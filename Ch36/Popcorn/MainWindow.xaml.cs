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

namespace Popcorn
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
        private int NumSpheres;

        // Define the model.
        private void DefineModel()
        {
            NumSpheres = 0;

            // Show the axes.
            //MainGroup.Children.Add(MeshExtensions.XAxisModel(4));
            //MainGroup.Children.Add(MeshExtensions.YAxisModel(4));
            //MainGroup.Children.Add(MeshExtensions.ZAxisModel(4));
            //MainGroup.Children.Add(MeshExtensions.OriginModel());

            // Make locations for recursive popcorn.
            double distFactor = double.Parse(distFactorTextBox.Text);
            List<Vector3D> directions = GetIcosahedronDirections(distFactor);

            // Get parameters.
            int depth = int.Parse(depthTextBox.Text);
            int numTheta = int.Parse(numThetaTextBox.Text);
            int numPhi = int.Parse(numPhiTextBox.Text);

            double r = double.Parse(rTextBox.Text);
            double g = double.Parse(gTextBox.Text);
            double b = double.Parse(bTextBox.Text);
            double colorFactor = double.Parse(colorFactorTextBox.Text);

            double radius = double.Parse(radiusTextBox.Text);
            double radiusFactor = double.Parse(radiusFactorTextBox.Text);

            // Make spheres.
            MakePopcorn(depth, numTheta, numPhi, D3.Origin, directions,
                r, g, b, colorFactor, radius, radiusFactor);

            Console.WriteLine("# Spheres: " + NumSpheres);
        }

        // Make popcorn.
        private void MakePopcorn(int depth, int numTheta, int numPhi, 
            Point3D center, List<Vector3D> directions,
            double r, double g, double b, double colorFactor,
            double radius, double radiusFactor)
        {
            // Make the mesh.
            MeshGeometry3D mesh = new MeshGeometry3D();
            Color color = Color.FromArgb(255, (byte)r, (byte)g, (byte)b);
            SolidColorBrush brush = new SolidColorBrush(color);

            MainGroup.Children.Add(mesh.MakeModel(brush));
            //SolidColorBrush brush = new SolidColorBrush(color);
            //GeometryModel3D model = mesh.MakeModel(brush);
            //MainGroup.Children.Add(model);
            //model.BackMaterial = model.Material;

            // Add a sphere to the mesh.
            mesh.AddSphere(center, radius, numTheta, numPhi, true);
            NumSpheres++;

            // See if we're done.
            if (--depth < 0) return;

            // Make smaller spheres.
            double newR = radius * radiusFactor;
            r *= colorFactor;
            g *= colorFactor;
            b *= colorFactor;
            foreach (Vector3D direction in directions)
            {
                Point3D newCenter = center + (radius + newR) * direction;
                MakePopcorn(depth, numTheta, numPhi, newCenter, directions,
                    r, g, b, colorFactor, newR, radiusFactor);
            }
        }

        // Make vectors pointing to the vertices on an icosahedron.
        private List<Vector3D> GetIcosahedronDirections(double length)
        {
            Point3D A, B, C, D, E, F, G, H, I, J, K, L;
            G3.IcosahedronPoints(out A, out B, out C, out D, out E,
                out F, out G, out H, out I, out J, out K, out L);

            List<Vector3D> directions = new List<Vector3D>();
            directions.Add(A - D3.Origin);
            directions.Add(B - D3.Origin);
            directions.Add(C - D3.Origin);
            directions.Add(D - D3.Origin);
            directions.Add(E - D3.Origin);
            directions.Add(F - D3.Origin);
            directions.Add(G - D3.Origin);
            directions.Add(H - D3.Origin);
            directions.Add(I - D3.Origin);
            directions.Add(J - D3.Origin);
            directions.Add(K - D3.Origin);
            directions.Add(L - D3.Origin);
            for (int i = 0; i < directions.Count; i++)
                directions[i] *= length / directions[i].Length;
            return directions;
        }

        // Generate the scene.
        private void generateButton_Click(object sender, RoutedEventArgs e)
        {
            DefineScene();
        }
    }
}
