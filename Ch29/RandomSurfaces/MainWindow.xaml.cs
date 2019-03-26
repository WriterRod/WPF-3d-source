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

namespace RandomSurfaces
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
            DefineLights(MainGroup);
            DefineModel();
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
        private void DefineModel()
        {
            // Remove non-light models.
            for (int i = MainGroup.Children.Count - 1; i >= 0; i--)
                if (!(MainGroup.Children[i] is Light))
                    MainGroup.Children.RemoveAt(i);

            // Axes.
            MeshExtensions.AddAxes(MainGroup);

            // Make the initial surface.
            Point3D[,] surface =
            {
                {
                    new Point3D(-3, 2, -3),
                    new Point3D(-3, 0, 3),
                },
                {
                    new Point3D(3, 0, -3),
                    new Point3D(3, -1, 3),
                }
            };

            // Fractalize.
            surface = G3.FractalizeSurface(surface, 5, -1, -2, 2);

            // Make the mesh.
            MeshGeometry3D mesh1 = new MeshGeometry3D();
            mesh1.AddSurface(surface);
            MainGroup.Children.Add(mesh1.MakeModel(Brushes.LightBlue));

            // Make a wireframe.
            HashSet<Edge> edges = new HashSet<Edge>();
            MeshGeometry3D mesh2 = new MeshGeometry3D();
            mesh2.AddSurface(surface, false, edges, 0.005);
            MainGroup.Children.Add(mesh2.MakeModel(Brushes.Blue));
        }

        // Make a new surface.
        private void GenerateButton_Click(object sender, RoutedEventArgs e)
        {
            DefineModel();
        }
    }
}
