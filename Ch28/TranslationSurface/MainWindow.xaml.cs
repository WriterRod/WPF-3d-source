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

namespace TranslationSurface
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
            Color darker = Color.FromArgb(255, 64, 64, 64);
            Color dark = Color.FromArgb(255, 96, 96, 96);

            group.Children.Add(new AmbientLight(darker));

            group.Children.Add(new DirectionalLight(dark, new Vector3D(0, -1, 0)));
            group.Children.Add(new DirectionalLight(dark, new Vector3D(1, -3, -2)));
            group.Children.Add(new DirectionalLight(dark, new Vector3D(-1, 3, 2)));
        }

        // Define the model.
        private void DefineModel(Model3DGroup group)
        {
            // Axes.
            MeshExtensions.AddAxes(group);

            // Make the generator.
            int numTrans = 6;
            double dy = 0.75;
            int numPoints = 6;
            Point3D[] generator = G3.MakePolygonPoints(numPoints,
                new Point3D(0, -numTrans * dy / 2, 0),
                D3.XVector(2), -D3.ZVector(2));

            // Repeat the first point at the end.
            numPoints++;
            Array.Resize(ref generator, numPoints);
            generator[numPoints - 1] = generator[0];

            // Make the transformation.
            Transform3DGroup transGroup = new Transform3DGroup();
            transGroup.Children.Add(new ScaleTransform3D(0.8, 0.8, 0.8));
            transGroup.Children.Add(D3.Rotate(D3.YVector(), D3.Origin, 20));
            transGroup.Children.Add(new TranslateTransform3D(0, dy, 0));

            // Make the surface of transformation.
            MeshGeometry3D mesh1 = new MeshGeometry3D();
            mesh1.AddTransformSurface(generator, transGroup, numTrans,
                true, true, false, false,
                false);
            group.Children.Add(mesh1.MakeModel(Brushes.LightBlue));

            // Make a wireframe.
            HashSet<Edge> edges = new HashSet<Edge>();
            MeshGeometry3D mesh2 = new MeshGeometry3D();
            mesh2.AddTransformSurface(generator, transGroup, numTrans,
                false, false, false, false,
                false, edges, 0.02);
            group.Children.Add(mesh2.MakeModel(Brushes.Blue));

            // Display the generator.
            edges = new HashSet<Edge>();
            MeshGeometry3D mesh3 = new MeshGeometry3D();
            mesh3.AddPolygonEdges(edges, 0.1, generator);
            group.Children.Add(mesh3.MakeModel(Brushes.Black));
        }
    }
}
