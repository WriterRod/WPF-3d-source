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

namespace MengerSponge
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
        private int NumCubes;

        // Define the model.
        private void DefineModel()
        {
            NumCubes = 0;

            // Get parameters.
            int depth = int.Parse(depthTextBox.Text);
            int width = int.Parse(widthTextBox.Text);

            // Get the volume where we will put it.
            double x = -width / 2;
            Rect3D rect = new Rect3D(x, x, x, width, width, width);

            // Make the sponge.
            MeshGeometry3D mesh = new MeshGeometry3D();
            MakeSponge(mesh, depth, rect);
            MainGroup.Children.Add(mesh.MakeModel(Brushes.Yellow));

            Console.WriteLine("# Cubes: " + NumCubes);
        }

        // Make a Menger sponge.
        private void MakeSponge(MeshGeometry3D mesh, int depth, Rect3D rect)
        {
            // See if we are at the end of the recursion.
            if (depth == 0)
            {
                // Just draw the box.
                Vector3D vx = D3.XVector(rect.SizeX);
                Vector3D vy = D3.YVector(rect.SizeY);
                Vector3D vz = D3.ZVector(rect.SizeZ);
                mesh.AddBox(rect.Location, vx, vy, vz);
            }
            else
            {
                // Divide the volume.
                depth--;
                double dx = rect.SizeX / 3.0;
                double dy = rect.SizeY / 3.0;
                double dz = rect.SizeZ / 3.0;
                for (int ix = 0; ix < 3; ix++)
                {
                    for (int iy = 0; iy < 3; iy++)
                    {
                        if ((ix == 1) && (iy == 1)) continue;
                        for (int iz = 0; iz < 3; iz++)
                        {
                            if ((iz == 1) &&
                                ((ix == 1) || (iy == 1))) continue;
                            Rect3D newRect = new Rect3D(
                                rect.X + dx * ix,
                                rect.Y + dy * iy,
                                rect.Z + dz * iz,
                                dx, dy, dz);
                            MakeSponge(mesh, depth, newRect);
                        }
                    }
                }
            }
        }

        // Generate the scene.
        private void generateButton_Click(object sender, RoutedEventArgs e)
        {
            DefineScene();
        }
    }
}
