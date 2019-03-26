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

namespace CircleChart
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
            Color dark = Color.FromArgb(255, 128, 128, 128);

            group.Children.Add(new AmbientLight(darker));

            group.Children.Add(new DirectionalLight(darker, new Vector3D(-1, 0, 0)));
            group.Children.Add(new DirectionalLight(darker, new Vector3D(0, -1, 0)));
            group.Children.Add(new DirectionalLight(dark, new Vector3D(0, 0, -1)));
        }

        private const double YMax = 3100000;
        private const double YScale = 5.0 / YMax;

        // Define the model.
        private void DefineModel(Model3DGroup group)
        {
            // Axes.
            //MeshExtensions.AddAxes(group);

            // General positioning parameters.
            Point3D origin = new Point3D(0, -1, 0);

            double[] values = LoadData();
            Brush[] brushes =
            {
                Brushes.Red, Brushes.Green, Brushes.Blue,
                Brushes.Pink, Brushes.LightGreen, Brushes.LightBlue,
            };
            double radius1 = 2;
            double radius2 = 4;
            double height = 1;
            MakeCircleChart(origin, radius1, radius2, height, values, brushes, 60, group);

            // Title.
            FontFamily ff = new FontFamily("Franklin Gothic Demi");
            Point3D ll = new Point3D(-radius2, origin.Y + height + 2, radius2);
            double fontSize = 0.9;
            MakeLabel("Bachelor's", ll, D3.ZVector(-2 * radius2), D3.YVector(1.25),
                Brushes.Transparent, Brushes.Blue, fontSize, ff,
                HorizontalAlignment.Center, VerticalAlignment.Center, group);
            ll.Y -= 1;
            MakeLabel("Degrees", ll, D3.ZVector(-2 * radius2), D3.YVector(1.25),
                Brushes.Transparent, Brushes.Blue, fontSize, ff,
                HorizontalAlignment.Center, VerticalAlignment.Center, group);

            // Key.
            const double patchWid = 1;
            const double labelWid = 3;
            const double hgt = 0.6;
            const double xgap = 0.1;
            const double ygap = 0.1;
            string[] labels =
            {
                "Business", "Health", "Soc. Science",
                "Psychology", "Bio. Science", "Engineering",
            };
            int numLabels = labels.Length;
            double keyWid = patchWid + xgap + labelWid;
            double keyHgt = numLabels * hgt + (numLabels - 1) * ygap;
            double y = origin.Y + height + keyHgt;
            fontSize = 0.4;
            for (int row = 0; row < numLabels; row++)
            {
                double x = -keyWid / 2;
                ll = new Point3D(x, y, -radius2);
                MeshGeometry3D mesh = new MeshGeometry3D();
                Point3D[] points =
                {
                    ll,
                    ll + D3.XVector(patchWid),
                    ll + D3.XVector(patchWid) + D3.YVector(hgt),
                    ll + D3.YVector(hgt),
                };
                mesh.AddPolygon(points);
                group.Children.Add(mesh.MakeModel(brushes[row]));

                ll.X += patchWid + xgap;
                MakeLabel(labels[row], ll, D3.XVector(labelWid), D3.YVector(hgt),
                    Brushes.Transparent, Brushes.Black, fontSize, ff,
                    HorizontalAlignment.Left, VerticalAlignment.Center, group);
                y -= hgt + ygap;
            }
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

        // Make pie slices.
        private void MakePieSlices(Point3D center, double r, double height,
            double[] values, Brush[] brushes, int numPieces, Model3DGroup group)
        {
            // Calculate percentages.
            double total = values.Sum();
            int numValues = values.Length;
            double[] percents = new double[numValues];
            for (int i = 0; i < numValues; i++) percents[i] = values[i] / total;

            // Draw slices.
            double minTheta = 0;
            for (int i = 0; i < numValues; i++)
            {
                double maxTheta = minTheta + 360.0 * percents[i];
                MeshGeometry3D mesh = new MeshGeometry3D();
                int slicePieces = (int)(numPieces * percents[i]);
                if (slicePieces < 2) slicePieces = 2;
                mesh.AddPieSlice(center, height, minTheta, maxTheta,
                    r, slicePieces);
                group.Children.Add(mesh.MakeModel(brushes[i]));

                minTheta = maxTheta;
            }
        }

        // Make stacked pie slices.
        private void MakeStackedPieSlices(Point3D center, double r, double height,
            double[,] values, Brush[,] brushes, int numPieces, Model3DGroup group)
        {
            // Calculate percentages.
            int numLevels = values.GetUpperBound(0) + 1;
            int numWedges = values.GetUpperBound(1) + 1;
            int numValues = values.Length;
            double total = 0;
            foreach (double value in values) total += value;
            double[,] percents = new double[2, numWedges];
            for (int level = 0; level < numLevels; level++)
                for (int slice = 0; slice < numWedges; slice++)
                    percents[level, slice] = values[level, slice] / total;

            // Draw slices.
            double minTheta = 0;
            for (int wedge = 0; wedge < numWedges; wedge++)
            {
                // Get this wedge's total percent.
                double wedgePercent = 0;
                for (int level = 0; level < numLevels; level++)
                    wedgePercent += percents[level, wedge];

                // Calculate the size of the wedge in degrees.
                double maxTheta = minTheta + 360.0 * wedgePercent;

                int wedgePieces = (int)(numPieces * wedgePercent);
                if (wedgePieces < 2) wedgePieces = 2;

                // Make the slices.
                double y = center.Y;
                for (int level = 0; level < numLevels; level++)
                {
                    // Calculate the slice's height.
                    double sliceHgt = height * percents[level, wedge] / wedgePercent;

                    MeshGeometry3D mesh = new MeshGeometry3D();
                    Point3D sliceCenter = new Point3D(center.X, y, center.Z);
                    mesh.AddPieSlice(sliceCenter, sliceHgt,
                        minTheta, maxTheta, r, wedgePieces);
                    group.Children.Add(mesh.MakeModel(brushes[level, wedge]));

                    y += sliceHgt;
                }
                minTheta = maxTheta;
            }
        }

        // Make ragged pie slices.
        private void MakeRaggedPieSlices(Point3D center, double r, double[] values,
            double[] heights, Brush[] brushes, int numPieces, Model3DGroup group)
        {
            // Calculate percentages.
            double total = values.Sum();
            int numValues = values.Length;
            double[] percents = new double[numValues];
            for (int i = 0; i < numValues; i++) percents[i] = values[i] / total;

            // Draw slices.
            double minTheta = 0;
            for (int i = 0; i < numValues; i++)
            {
                double maxTheta = minTheta + 360.0 * percents[i];
                MeshGeometry3D mesh = new MeshGeometry3D();
                int slicePieces = (int)(numPieces * percents[i]);
                if (slicePieces < 2) slicePieces = 2;
                mesh.AddPieSlice(center, heights[i], minTheta, maxTheta,
                    r, slicePieces);
                group.Children.Add(mesh.MakeModel(brushes[i]));

                minTheta = maxTheta;
            }
        }

        // Make circle chart pie slices.
        private void MakeCircleChart(Point3D center, double r1, double r2,
            double height, double[] values, Brush[] brushes, int numPieces, Model3DGroup group)
        {
            // Calculate percentages.
            double total = values.Sum();
            int numValues = values.Length;
            double[] percents = new double[numValues];
            for (int i = 0; i < numValues; i++) percents[i] = values[i] / total;

            // Draw slices.
            double minTheta = 0;
            for (int i = 0; i < numValues; i++)
            {
                double maxTheta = minTheta + 360.0 * percents[i];
                MeshGeometry3D mesh = new MeshGeometry3D();
                int slicePieces = (int)(numPieces * percents[i]);
                if (slicePieces < 2) slicePieces = 2;
                mesh.AddCircleSlice(center, height, minTheta, maxTheta,
                    r1, r2, slicePieces);
                group.Children.Add(mesh.MakeModel(brushes[i]));

                minTheta = maxTheta;
            }
        }

        // Load data.
        // Source: https://nces.ed.gov/programs/coe/indicator_cta.asp
        // Figure 2, 2014-2015.
        private double[] LoadData()
        {
            return new double[] { 363800, 216200, 166900, 117600, 109900, 97900 };
        }
    }
}
