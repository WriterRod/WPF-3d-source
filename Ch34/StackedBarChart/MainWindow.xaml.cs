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

namespace StackedBarChart
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

        private const double YMax = 3100000;
        private const double YScale = 5.0 / YMax;

        // Define the model.
        private void DefineModel(Model3DGroup group)
        {
            // Axes.
            //MeshExtensions.AddAxes(group);

            // General positioning parameters.
            Point3D origin = new Point3D(-2, -3, 0);
            double barDx = 1;
            double barGap = 0.25;

            // Load the data.
            double[,] values = LoadData();
            int numX = values.GetUpperBound(0) + 1;
            int numZ = values.GetUpperBound(1) + 1;
            Brush[] barBrushes = { Brushes.Green, Brushes.Blue, Brushes.White, Brushes.Red };
            string[] frontLabels =
                { " 1960", " 1970", " 1980", " 1990", " 2000", " 2010" };

            // See how big the bars will be.
            double widX = numX * barDx + (numX - 1) * barGap;
            double widZ = numZ * barDx + (numZ - 1) * barGap;
            double hgt = YMax * YScale;
            double barXmin = origin.X - widX / 2;
            double barXmax = barXmin + widX;
            double barYmin = origin.Y;
            double barYmax = barYmin + hgt;
            double barZmin = origin.Z - barDx / 2;
            double barZmax = barZmin + barDx;

            // Bars.
            FontFamily ff = new FontFamily("Franklin Gothic Demi");
            MakeStackedBars(barXmin, barYmin, barZmin, barDx, barGap, values,
                frontLabels, barBrushes, Brushes.Transparent, Brushes.Black,
                ff, group);

            // Title.
            MakeLabel("US Military",
                new Point3D(barXmin, barYmax + 1, barZmin - 0.1),
                D3.XVector(widX), D3.YVector(1),
                Brushes.Transparent, Brushes.Green, 0.9, ff,
                HorizontalAlignment.Center, VerticalAlignment.Center, group);
            MakeLabel("Composition",
                new Point3D(barXmin, barYmax, barZmin - 0.1),
                D3.XVector(widX), D3.YVector(1),
                Brushes.Transparent, Brushes.Green, 0.9, ff,
                HorizontalAlignment.Center, VerticalAlignment.Center, group);

            // Lines behind.
            MeshGeometry3D mesh = new MeshGeometry3D();
            Point3D p1 = new Point3D(barXmin - barGap / 2, barYmin, barZmin - 0.1);
            Point3D p2 = new Point3D(barXmax + barGap / 2, barYmin, barZmin - 0.1);
            for (double y = 0; y < YMax; y += 500000)
            {
                p1.Y = barYmin + y * YScale;
                p2.Y = p1.Y;
                mesh.AddSegment(0.05, p1, p2);
            }
            group.Children.Add(mesh.MakeModel(Brushes.Red));

            // Line labels.
            p1 = new Point3D(barXmax + barGap, barYmin, barZmin - 0.1);
            for (double y = 0; y < YMax; y += 1000000)
            {
                p1.Y = barYmin + y * YScale - 0.25;
                string text = ((int)(y / 1000000)).ToString() + " M";
                MakeLabel(text, p1,
                    D3.XVector(0.5), D3.YVector(0.5),
                    Brushes.Transparent, Brushes.Black, 0.25, ff,
                    HorizontalAlignment.Left, VerticalAlignment.Center, group);
            }

            // Key.
            const double textWidth = 1.5;
            Point3D point = new Point3D(barXmax + 2 * barGap + 0.5, barYmin, barZmin);
            MakeLabel("", point,
                D3.XVector(0.5), D3.YVector(0.5),
                Brushes.Green, Brushes.Black, 0.3, ff,
                HorizontalAlignment.Left, VerticalAlignment.Center, group);
            point.X += 0.5;
            MakeLabel(" Army", point,
                D3.XVector(textWidth), D3.YVector(0.5),
                Brushes.Transparent, Brushes.Black, 0.3, ff,
                HorizontalAlignment.Left, VerticalAlignment.Center, group);
            point.X -= 0.5;
            point.Y += 0.75;
            MakeLabel("", point,
                D3.XVector(0.5), D3.YVector(0.5),
                Brushes.Blue, Brushes.Black, 0.3, ff,
                HorizontalAlignment.Left, VerticalAlignment.Center, group);
            point.X += 0.5;
            MakeLabel(" Navy", point,
                D3.XVector(textWidth), D3.YVector(0.5),
                Brushes.Transparent, Brushes.Black, 0.3, ff,
                HorizontalAlignment.Left, VerticalAlignment.Center, group);
            point.X -= 0.5;
            point.Y += 0.75;
            MakeLabel("", point,
                D3.XVector(0.5), D3.YVector(0.5),
                Brushes.White, Brushes.Black, 0.3, ff,
                HorizontalAlignment.Left, VerticalAlignment.Center, group);
            point.X += 0.5;
            MakeLabel(" Air Force", point,
                D3.XVector(textWidth), D3.YVector(0.5),
                Brushes.Transparent, Brushes.Black, 0.3, ff,
                HorizontalAlignment.Left, VerticalAlignment.Center, group);
            point.Y += 0.75;
            point.X -= 0.5;
            MakeLabel("", point,
                D3.XVector(0.5), D3.YVector(0.5),
                Brushes.Red, Brushes.Black, 0.3, ff,
                HorizontalAlignment.Left, VerticalAlignment.Center, group);
            point.X += 0.5;
            MakeLabel(" Marines", point,
                D3.XVector(textWidth), D3.YVector(0.5),
                Brushes.Transparent, Brushes.Black, 0.3, ff,
                HorizontalAlignment.Left, VerticalAlignment.Center, group);
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

        // Make rows of bars in the X and Z directions.
        private void MakeRowColumnBars(double xmin, double ymin,
            double zmin, double dx, double gap,
            double[,] values, string[] frontLabels, string[] sideLabels,
            Brush[,] barBrushes, Brush[] bgBrushes, Brush[] fgBrushes,
            FontFamily ff, Model3DGroup group)
        {
            int numX = values.GetUpperBound(0) + 1;
            int numZ = values.GetUpperBound(1) + 1;
            double x = xmin;
            double z;
            double fontSize = 0.3;
            for (int ix = 0; ix < numX; ix++)
            {
                z = zmin;
                for (int iz = 0; iz < numZ; iz++)
                {
                    // Make the bar.
                    MeshGeometry3D barMesh = new MeshGeometry3D();
                    Point3D corner = new Point3D(x, ymin, z);
                    barMesh.AddBox(corner,
                        D3.XVector(dx),
                        D3.YVector(YScale * values[ix, iz]),
                        D3.ZVector(dx));
                    group.Children.Add(barMesh.MakeModel(barBrushes[ix, iz]));

                    z += dx + gap;
                }

                // Display the front label.
                const double textWid = 2;
                MakeLabel(frontLabels[ix],
                    new Point3D(x + dx, ymin, z + textWid),
                    D3.ZVector(-textWid), D3.XVector(-dx),
                    bgBrushes[ix], fgBrushes[ix], fontSize, ff,
                    HorizontalAlignment.Left, VerticalAlignment.Center, group);

                x += dx + gap;
            }

            // Display the side labels.
            z = zmin + dx;
            double xmax = xmin + numX * dx + (numX - 1) * gap;
            for (int iz = 0; iz < numZ; iz++)
            {
                const double textWid = 2;
                MakeLabel(sideLabels[iz],
                    new Point3D(xmax + gap, ymin, z),
                    D3.XVector(textWid), D3.ZVector(-dx),
                    Brushes.Transparent, Brushes.Black, fontSize, ff,
                    HorizontalAlignment.Left, VerticalAlignment.Center, group);
                //MakeLabel(sideLabels[iz],
                //    new Point3D(xmin - textWid - gap, ymin, z),
                //    D3.XVector(textWid), D3.ZVector(-dx),
                //    Brushes.Transparent, Brushes.Black, fontSize, ff,
                //    HorizontalAlignment.Left, VerticalAlignment.Center, group);
                z += dx + gap;
            }
        }

        // Make a row of stacked bars in the X direction.
        private void MakeStackedBars(double xmin, double ymin,
            double zmin, double dx, double gap,
            double[,] values, string[] frontLabels,
            Brush[] barBrushes, Brush bgBrush, Brush fgBrush,
            FontFamily ff, Model3DGroup group)
        {
            double x = xmin;
            int numX = values.GetUpperBound(0) + 1;
            int numZ = values.GetUpperBound(1) + 1;
            double fontSize = 0.45;
            for (int ix = 0; ix < numX; ix++)
            {
                double y = ymin;
                for (int iz = 0; iz < numZ; iz++)
                {
                    // Make this piece of the bar.
                    MeshGeometry3D barMesh = new MeshGeometry3D();
                    Point3D corner = new Point3D(x, y, zmin);
                    barMesh.AddBox(corner,
                        D3.XVector(dx),
                        D3.YVector(YScale * values[ix, iz]),
                        D3.ZVector(dx));
                    group.Children.Add(barMesh.MakeModel(barBrushes[iz]));
                    y += YScale * values[ix, iz];
                }

                // Display the front label.
                const double textWid = 1.5;
                MakeLabel(frontLabels[ix],
                    new Point3D(x + dx, ymin, textWid + zmin + dx + gap),
                    D3.ZVector(-textWid), D3.XVector(-dx),
                    bgBrush, fgBrush, fontSize, FontFamily,
                    HorizontalAlignment.Left, VerticalAlignment.Center, group);

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

        // Load data.
        // Source: https://historyinpieces.com/research/us-military-personnel-1954-2014
        private double[,] LoadData()
        {
            return new double[,]
            {
                { 873078, 617984, 814752, 170621, },
                { 1322548, 692660, 791349, 259737, },
                { 777036, 527153, 557969, 188469, },
                { 732403, 579417, 535233, 196652, },
                { 482170, 373193, 355654, 173321, },
                { 566045, 328303, 334196, 202441, },
            };
        }
    }
}
