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

namespace BarChart
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
            Color darker = Color.FromArgb(255, 96, 96, 96);
            Color dark = Color.FromArgb(255, 129, 129, 129);

            group.Children.Add(new AmbientLight(darker));

            group.Children.Add(new DirectionalLight(dark, new Vector3D(0, -1, 0)));
            group.Children.Add(new DirectionalLight(dark, new Vector3D(1, -3, -2)));
            group.Children.Add(new DirectionalLight(dark, new Vector3D(-1, 3, 2)));
        }

        private const double YScale = 8;

        // Define the model.
        private void DefineModel(Model3DGroup group)
        {
            // Axes.
            //MeshExtensions.AddAxes(group);

            // General positioning parameters.
            Point3D origin = new Point3D(0, -1, 0);
            double barDx = 1;
            double barGap = 0.25;

            // Load the data.
            double[] values = LoadData();
            int numValues = values.Length;
            string[] frontLabels =
                { " Sofa", " Bathroom", " Dresser", " Fridge or\n Freezer", " Outside", " Other" };
            string[] topLabels = new string[numValues];
            for (int i = 0; i < numValues; i++)
                topLabels[i] = values[i].ToString("P0");
            Brush[] barBrushes =
            {
                Brushes.Red, Brushes.Green, Brushes.Blue,
                Brushes.Yellow, Brushes.Orange, Brushes.Fuchsia,
            };
            Brush[] bgBrushes =
            {
                Brushes.HotPink, Brushes.LightGreen, Brushes.LightBlue,
                new SolidColorBrush(Color.FromArgb(255, 255, 255, 100)),
                new SolidColorBrush(Color.FromArgb(255, 255, 220, 100)),
                new SolidColorBrush(Color.FromArgb(255, 255, 128, 255)),
            };
            Brush[] fgBrushes =
            {
                Brushes.Black, Brushes.Black, Brushes.Black,
                Brushes.Black, Brushes.Black, Brushes.Black,
            };

            // See how big the bars will be.
            double wid = numValues * barDx + (numValues - 1) * barGap;
            double hgt = values.Max() * YScale;
            double barXmin = origin.X - wid / 2;
            double barXmax = barXmin + wid;
            double barYmin = origin.Y;
            double barYmax = barYmin + hgt;
            double barZmin = origin.Z - barDx / 2;
            double barZmax = barZmin + barDx;

            // Bars.
            FontFamily ff = new FontFamily("Franklin Gothic Demi");
            MakeBars(barXmin, barYmin, barZmin, barDx, barGap, values,
                frontLabels, topLabels, barBrushes, bgBrushes, fgBrushes,
                ff, group);

            // Title.
            double fontSize = 0.75;
            MakeLabel("Where's My",
                new Point3D(barXmin, barYmax - 1, barZmin - 0.1),
                D3.XVector(wid), D3.YVector(1),
                Brushes.Transparent, Brushes.DarkBlue, fontSize, ff,
                HorizontalAlignment.Center, VerticalAlignment.Center, group);
            MakeLabel("Remote?",
                new Point3D(barXmin, barYmax - 0.75 - 1, barZmin - 0.1),
                D3.XVector(wid), D3.YVector(1),
                Brushes.Transparent, Brushes.DarkBlue, fontSize, ff,
                HorizontalAlignment.Center, VerticalAlignment.Center, group);
        }

        // Make a row of bars in the X direction.
        private void MakeBars(double xmin, double ymin,
            double zmin, double dx, double gap,
            double[] values, string[] frontLabels, string[] topLabels,
            Brush[] barBrushes, Brush[] bgBrushes, Brush[] fgBrushes,
            FontFamily ff, Model3DGroup group)
        {
            double x = xmin;
            double fontSize = 0.4;
            for (int i = 0; i < values.Length; i++)
            {
                // Make the bar.
                MeshGeometry3D barMesh = new MeshGeometry3D();
                Point3D corner = new Point3D(x, ymin, zmin);
                barMesh.AddBox(corner,
                    D3.XVector(dx),
                    D3.YVector(YScale * values[i]),
                    D3.ZVector(dx));
                group.Children.Add(barMesh.MakeModel(barBrushes[i]));

                // Display the front label.
                const double textWid = 1.8;
                MakeLabel(frontLabels[i],
                    new Point3D(x + dx, ymin, textWid + zmin + dx + gap),
                    D3.ZVector(-textWid), D3.XVector(-dx),
                    bgBrushes[i], fgBrushes[i], fontSize, ff,
                    HorizontalAlignment.Left, VerticalAlignment.Center, group);

                // Display the top label.
                MakeLabel(topLabels[i],
                    new Point3D(x,
                        ymin + YScale * values[i],
                        zmin - 0.1),
                    D3.XVector(dx), D3.YVector(dx),
                    Brushes.Transparent, fgBrushes[i], fontSize, ff,
                    HorizontalAlignment.Center, VerticalAlignment.Bottom, group);

                x += dx + gap;
            }
        }
        
        // Make a label.
        private void MakeLabel(string text, Point3D ll,
            Vector3D vRight, Vector3D vUp,
            Brush bgBrush, Brush fgBrush,
            double fontSize, FontFamily fontFamily,
            HorizontalAlignment hAlign, VerticalAlignment vAlign,
            Model3DGroup group)
        {
            double wid = vRight.Length;
            double hgt = vUp.Length;
            MeshGeometry3D mesh = new MeshGeometry3D();
            group.Children.Add(
                mesh.AddSizedText(text,
                    fontSize, wid, hgt,
                    ll, ll + vRight, ll + vRight + vUp, ll + vUp,
                    bgBrush, fgBrush, hAlign, vAlign, fontFamily));
        }

        // Load the data.
        // Source: http://blog.logitech.com/2011/03/17/where-is-that-remote/
        private double[] LoadData()
        {
            // Initialize the values. The last entry is 1.
            double[] values =
            {
                0.49, 0.08, 0.08, 0.04, 0.02, 1,
            };

            // Subtract the other values from the last entry.
            int last = values.Length - 1;
            for (int i = 0; i < last; i++) values[last] -= values[i];
            return values;
        }
    }
}
