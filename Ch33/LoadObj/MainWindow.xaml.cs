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

// Add a reference to System.Windows.Forms.
using System.Windows.Forms;

using System.IO;
using System.Windows.Media.Media3D;

namespace LoadObj
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

        // The main model group.
        private Model3DGroup MainGroup = null;

        // The camera.
        private PerspectiveCamera TheCamera = null;

        // The camera controller.
        private SphericalCameraController CameraController = null;

        // Dictionary of mesh names.
        private Dictionary<MeshGeometry3D, string> MeshNames = null;

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // Define WPF objects.
            ModelVisual3D visual3d = new ModelVisual3D();
            MainGroup = new Model3DGroup();
            visual3d.Content = MainGroup;
            mainViewport.Children.Add(visual3d);

            // Define the camera and lights.
            DefineCamera(mainViewport);
            DefineLights(MainGroup);
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
            Color darker = Color.FromArgb(255, 96, 96, 96);
            Color dark = Color.FromArgb(255, 128, 128, 128);

            group.Children.Add(new AmbientLight(darker));

            group.Children.Add(new DirectionalLight(dark, new Vector3D(0, -1, 0)));
            group.Children.Add(new DirectionalLight(dark, new Vector3D(1, -3, -2)));
            group.Children.Add(new DirectionalLight(dark, new Vector3D(-1, 3, 2)));
        }

        // Close.
        private void ExitMenu_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        // Let the user select and open a model.
        private void OpenCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Open();
        }
        private void Open()
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "OBJ files (*.obj)|*.obj|All files (*.*)|*.*";
            dlg.AddExtension = true;
            dlg.DefaultExt = "obj";
            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                // Hide the viewport and display the wait cursor.
                mainViewport.Visibility = Visibility.Hidden;
                Cursor = System.Windows.Input.Cursors.Wait;

                // Get the directory and file.
                FileInfo fileinfo = new FileInfo(dlg.FileName);
                string dir = fileinfo.DirectoryName;
                if (!dir.EndsWith("\\")) dir += "\\";
                string objfile = fileinfo.Name;

                // Load the model.
                LoadModel(dir, objfile,
                    invertTexturesCheckBox.IsChecked.Value,
                    zisUpCheckBox.IsChecked.Value);

                // Show the viewport and display the default cursor.
                Cursor = null;
                mainViewport.Visibility = Visibility.Visible;
            }
        }

        // Load a model.
        private void LoadModel(string dir, string objfile,
            bool invertTextures = false, bool zIsUp = false)
        {
            // Remove any previously loaded models.
            for (int i = MainGroup.Children.Count - 1; i >= 0; i--)
                if (!(MainGroup.Children[i] is Light))
                    MainGroup.Children.RemoveAt(i);

            // Add axes if desired.
            if (axesCheckBox.IsChecked.Value)
            {
                MeshExtensions.AddAxes(MainGroup);
            }

            // Load the file.
            ObjLoader loader = new ObjLoader(dir, objfile, invertTextures, zIsUp);

            // Fit in the box -4 <= {x, y, z} <= 4.
            loader.FitToBox(new Rect3D(-4, -4, -4, 2 * 4, 2 * 4, 2 * 4));

            // Make a new list for the mesh names.
            MeshNames = new Dictionary<MeshGeometry3D, string>();

            // Display the meshes.
            int totalPoints = 0;
            int totalTriangles = 0;
            for (int i = 0; i < loader.Meshes.Count; i++)
            {
                totalPoints += loader.Meshes[i].Positions.Count;
                totalTriangles += loader.Meshes[i].TriangleIndices.Count;
                Console.WriteLine("Mesh " + loader.MeshNames[i] +
                    ", Material: " + loader.MaterialNames[i] +
                    ", Points: " + loader.Meshes[i].Positions.Count +
                    ", Triangles: " + loader.Meshes[i].TriangleIndices.Count);

                // Get the mesh.
                MeshGeometry3D mesh = loader.Meshes[i];

                // Get the mesh's material.
                MaterialGroup matGroup;
                string matName = loader.MaterialNames[i];
                if (loader.MtlMaterials.ContainsKey(matName))
                    matGroup = loader.MtlMaterials[matName].MatGroup;
                else
                {
                    // Use a default material.
                    matGroup = new MaterialGroup();
                    matGroup.Children.Add(new DiffuseMaterial(Brushes.LightBlue));
                }

                // Make the model.
                GeometryModel3D model = mesh.MakeModel(matGroup);
                model.BackMaterial = model.Material;
                MainGroup.Children.Add(model);

                // Save the mesh's name.
                MeshNames.Add(mesh, loader.MeshNames[i]);
            }
            Console.WriteLine("Total meshes:    " + loader.Meshes.Count);
            Console.WriteLine("Total points:    " + totalPoints);
            Console.WriteLine("Total triangles: " + totalTriangles);
        }

        // On right-mouse down, display hit mesh information.
        private void mainViewport_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.RightButton == MouseButtonState.Released) return;

            // Get the mouse's position relative to the viewport.
            Point mouse_pos = e.GetPosition(mainViewport);

            // Perform the hit test.
            HitTestResult result =
                VisualTreeHelper.HitTest(mainViewport, mouse_pos);

            // Display information about the hit.
            RayMeshGeometry3DHitTestResult mesh_result =
                result as RayMeshGeometry3DHitTestResult;
            if (mesh_result != null)
            {
                // Display the name of the mesh.
                Console.WriteLine("Hit mesh " + MeshNames[mesh_result.MeshHit]);
            }
        }
    }
}
