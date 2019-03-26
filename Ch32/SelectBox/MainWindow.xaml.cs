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

namespace SelectBox
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

        // The currently selected model and mesh.
        private GeometryModel3D SelectedModel = null;

        // The materials for selected and deselected models.
        private Material SelectedMaterial, DeselectedMaterial;

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

            group.Children.Add(new PointLight(Colors.White, new Point3D(0, 1, 1)));
        }

        // Define the model.
        private void DefineModel()
        {
            // Show the axes.
            //MainGroup.AddAxisModels();

            // Make the materials for selected and deselected models.
            SelectedMaterial = new DiffuseMaterial(Brushes.Fuchsia);
            DeselectedMaterial = new DiffuseMaterial(Brushes.LightBlue);
            //SelectedMaterial = new DiffuseMaterial(Brushes.Indigo);
            //DeselectedMaterial = new DiffuseMaterial(Brushes.Lavender);

            // Make a bunch of cubes.
            const int xmax = 2;
            const double wid = 0.25;
            const double radius = wid / 2;
            Vector3D vx = D3.XVector(wid);
            Vector3D vy = D3.YVector(wid);
            Vector3D vz = D3.ZVector(wid);
            for (int ix = -xmax; ix <= xmax; ix++)
            {
                for (int iy = -xmax; iy <= xmax; iy++)
                {
                    for (int iz = -xmax; iz <= xmax; iz++)
                    {
                        MeshGeometry3D mesh = new MeshGeometry3D();
                        mesh.AddBox(new Point3D(ix - radius, iy - radius, iz - radius), vx, vy, vz);
                        MainGroup.Children.Add(new GeometryModel3D(mesh, DeselectedMaterial));
                    }
                }
            }
            int num = 2 * xmax + 1;
            num = num * num * num;
            Console.WriteLine(num + " cubes");
        }

        // See if the user clicked on anything.
        private void mainBorder_MouseDown(object sender, MouseButtonEventArgs e)
        {
            // Only respond to left mouse down.
            if (e.LeftButton == MouseButtonState.Released) return;

            // Get the mouse's position relative to the viewport.
            Point mousePos = e.GetPosition(mainViewport);

            // Perform the hit test.
            HitTestResult result =
                VisualTreeHelper.HitTest(mainViewport, mousePos);
            RayMeshGeometry3DHitTestResult meshResult =
                result as RayMeshGeometry3DHitTestResult;
            GeometryModel3D model = null;
            if ((meshResult != null) && (meshResult.ModelHit is GeometryModel3D))
                model = meshResult.ModelHit as GeometryModel3D;

            // Select the new model.
            SelectModel(model);
        }

        // Select a model.
        private void SelectModel(GeometryModel3D model)
        {
            // Deselect any previously selected model.
            if (SelectedModel != null) SelectedModel.Material = DeselectedMaterial;

            // Select the new model.
            SelectedModel = model;
            if (model != null) model.Material = SelectedMaterial;
        }
    }
}
