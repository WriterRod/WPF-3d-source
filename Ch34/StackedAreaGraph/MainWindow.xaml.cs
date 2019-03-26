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

namespace StackedAreaGraph
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
            group.Children.Add(new DirectionalLight(darker, new Vector3D(1, -2, -1)));
            group.Children.Add(new DirectionalLight(dark, new Vector3D(0, 0, -1)));
        }

        private const double YMax = 3100000;
        private const double YScale = 5.0 / YMax;

        // Define the model.
        private void DefineModel(Model3DGroup group)
        {
            // Axes.
            //MeshExtensions.AddAxes(group);

            // Make a line graph.
            double[,] values = LoadData();
            string[] xlabels = { "Jan", "Feb", "Mar", "Apr", "May" };
            string[] ylabels = { "", "10", "20", "30", "40", "50", "60", "70", "80" };
            Brush[] valuesBrushes = { null, Brushes.Red, Brushes.Green, Brushes.Blue, Brushes.LightGreen };
            const double labelWid = 2;
            const double labelFontSize = 0.7;
            const double yoff = 0;
            Point3D corner = new Point3D(-4, -4 + yoff, -3);
            MakeStackedAreaGraph(group,
                values, valuesBrushes, corner, 8, 8, 1,
                0.05, Brushes.Blue, labelWid, labelFontSize, Brushes.Black,
                0, 4, "Month", xlabels,
                0, 100, "Units Sold", ylabels,
                true);

            // Title.
            FontFamily ff = new FontFamily("Franklin Gothic Demi");
            Point3D ll = new Point3D(-4, 3 + yoff, -3);
            double fontSize = 1.1;
            MakeLabel("Monthly Sales", ll,
                D3.XVector(8), D3.YVector(1.25),
                Brushes.Transparent, Brushes.Black, fontSize, ff,
                HorizontalAlignment.Center, VerticalAlignment.Center, group);

            // Key.
            string[] products = { "Prod 1", "Prod 2", "Prod 3", "Prod 4" };
            ll = new Point3D(-4, -2 + yoff, 5);
            fontSize = 0.6;
            for (int i = 0; i < products.Length; i++)
            {
                MakeLabel(products[i], ll,
                    D3.ZVector(-3), D3.YVector(1.25),
                    Brushes.Transparent, Brushes.Black, fontSize, ff,
                    HorizontalAlignment.Center, VerticalAlignment.Center, group);

                MeshGeometry3D mesh = new MeshGeometry3D();
                Point3D[] points =
                {
                    new Point3D(ll.X, ll.Y + 0.1, ll.Z - 4 + 1.0),
                    new Point3D(ll.X, ll.Y + 0.1, ll.Z - 4 + 0.1),
                    new Point3D(ll.X, ll.Y + 0.9, ll.Z - 4 + 0.1),
                    new Point3D(ll.X, ll.Y + 0.9, ll.Z - 4 + 1.0),
                };
                mesh.AddPolygon(points);
                group.Children.Add(mesh.MakeModel(valuesBrushes[i + 1]));

                ll.Y += 1;
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

        // Make pie slices.
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

        // Make a scatter plot.
        public void MakeScatterPlot(Model3DGroup group,
            Point3D corner, double xLength, double yLength, double zLength,
            Point3D[] values, Brush valuesBrush,
            double boxThickness, double lineThickness,
            Brush boxBrush, Brush lineBrush, Brush tickBrush,
            double labelWid, double labelHgt, double labelGap, double labelFontSize, Brush labelBrush,
            double titleHgt, double titleFontSize, Brush titleBrush,
            double xmin, double xmax, double tickDx, double labelDx, string xtitle, string[] xlabels,
            double ymin, double ymax, double tickDy, double labelDy, string ytitle, string[] ylabels,
            double zmin, double zmax, double tickDz, double labelDz, string ztitle, string[] zlabels)
        {
            // Make and label the plot box.
            MakePlotBox(group, corner, xLength, yLength, zLength,
                boxThickness, lineThickness,
                boxBrush, lineBrush, tickBrush,
                labelWid, labelHgt, labelGap, labelFontSize, labelBrush,
                titleHgt, titleFontSize, titleBrush,
                xmin, xmax, tickDx, labelDx, xtitle, xlabels,
                ymin, ymax, tickDy, labelDy, ytitle, ylabels,
                zmin, zmax, tickDz, labelDz, ztitle, zlabels);

            // Minimum X, Y, and Z values, and scales.
            double px1 = corner.X;
            double py1 = corner.Y;
            double pz1 = corner.Z;
            double xScale = xLength / (xmax - xmin);
            double yScale = yLength / (ymax - ymin);
            double zScale = zLength / (zmax - zmin);

            // Plot the data values.
            MeshGeometry3D valuesMesh = new MeshGeometry3D();
            foreach (Point3D value in values)
            {
                Point3D p = new Point3D(
                    px1 + (value.X - xmin) * xScale,
                    py1 + (value.Y - ymin) * yScale,
                    pz1 + (value.Z - zmin) * zScale);
                valuesMesh.AddSphere(p, 0.2, 10, 5, true);
            }
            group.Children.Add(valuesMesh.MakeModel(valuesBrush));
        }

        // Make a box with the back XY, YZ, and ZX planes labeled with lines,
        // tick marks, and labels every 1 unit.
        public void MakePlotBox(Model3DGroup group,
            Point3D corner, double xLength, double yLength, double zLength,
            double boxThickness, double lineThickness,
            Brush boxBrush, Brush lineBrush, Brush tickBrush,
            double labelWid, double labelHgt, double labelGap, double labelFontSize, Brush labelBrush,
            double titleHgt, double titleFontSize, Brush titleBrush,
            double xmin, double xmax, double tickDx, double labelDx, string xtitle, string[] xlabels,
            double ymin, double ymax, double tickDy, double labelDy, string ytitle, string[] ylabels,
            double zmin, double zmax, double tickDz, double labelDz, string ztitle, string[] zlabels,
            bool shiftZ = false)
        {
            const double tickWid = 0.2;
            FontFamily ff = new FontFamily("Franklin Gothic Demi");

            // Draw the box outline.
            Vector3D vx = D3.XVector(xLength);
            Vector3D vy = D3.YVector(yLength);
            Vector3D vz = D3.ZVector(zLength);
            MeshGeometry3D boxMesh = new MeshGeometry3D();
            boxMesh.AddSegment(boxThickness, corner, corner + vx);
            boxMesh.AddSegment(boxThickness, corner, corner + vy);
            boxMesh.AddSegment(boxThickness, corner, corner + vz);
            boxMesh.AddSegment(boxThickness, corner + vy, corner + vy + vx);
            boxMesh.AddSegment(boxThickness, corner + vy, corner + vy + vz);
            boxMesh.AddSegment(boxThickness, corner + vx, corner + vx + vy);
            boxMesh.AddSegment(boxThickness, corner + vx, corner + vx + vz);
            boxMesh.AddSegment(boxThickness, corner + vz, corner + vz + vx);
            boxMesh.AddSegment(boxThickness, corner + vz, corner + vz + vy);
            group.Children.Add(boxMesh.MakeModel(boxBrush));

            // Minimum X, Y, and Z values, and scales.
            double px1 = corner.X;
            double px2 = corner.X + xLength;
            double py1 = corner.Y;
            double py2 = corner.Y + yLength;
            double pz1 = corner.Z;
            double pz2 = corner.Z + zLength;
            double xScale = xLength / (xmax - xmin);
            double yScale = yLength / (ymax - ymin);
            double zScale = zLength / (zmax - zmin);

            // Lines and labels.
            MeshGeometry3D lineMesh = new MeshGeometry3D();

            // Lines with different X values.
            Point3D ll;
            int i = 0;
            for (double x = xmin; x < xmax + labelDx / 2; x += labelDx)
            {
                double px = px1 + (x - xmin) * xScale;

                // Label the line.
                ll = new Point3D(px + labelHgt / 2, py1, pz2 + labelWid + labelGap);
                MakeLabel(xlabels[i++], ll, D3.ZVector(-labelWid), D3.XVector(-labelHgt),
                    Brushes.Transparent, labelBrush, labelFontSize, ff,
                    HorizontalAlignment.Right, VerticalAlignment.Center, group);

                // Draw the line.
                if ((x > xmin) && (x < xmax - labelDx / 2))
                {
                    lineMesh.AddSegment(lineThickness,
                        new Point3D(px, py1, pz1),
                        new Point3D(px, py2, pz1));
                    lineMesh.AddSegment(lineThickness,
                        new Point3D(px, py1, pz1),
                        new Point3D(px, py1, pz2));
                }
            }

            // Lines with different Y values.
            i = 0;
            for (double y = ymin; y < ymax + labelDy / 2; y += labelDy)
            {
                double py = py1 + (y - ymin) * yScale;

                // Label the line.
                if (y > ymin)
                {
                    ll = new Point3D(px1, py - labelHgt / 2, pz2 + labelWid + labelGap);
                    MakeLabel(ylabels[i], ll, D3.ZVector(-labelWid), D3.YVector(labelHgt),
                        Brushes.Transparent, labelBrush, labelFontSize, ff,
                        HorizontalAlignment.Right, VerticalAlignment.Center, group);
                }
                i++;

                // Draw the line.
                if ((y > ymin) && (y < ymax - labelDy / 2))
                {
                    lineMesh.AddSegment(lineThickness,
                        new Point3D(px1, py, pz1),
                        new Point3D(px2, py, pz1));
                    lineMesh.AddSegment(lineThickness,
                        new Point3D(px1, py, pz1),
                        new Point3D(px1, py, pz2));
                }
            }

            // Lines with different Z values.
            double zshift = 0;
            if (shiftZ) zshift -= (labelDz / 2) * zScale;
             i = 0;
            for (double z = zmin; z < zmax + labelDz / 2; z += labelDz)
            {
                double pz = pz1 + (z - zmin) * zScale;

                // Label the line.
                ll = new Point3D(px2 + labelGap, py1, pz + labelHgt / 2 + zshift);
                MakeLabel(zlabels[i++], ll, D3.XVector(labelWid), D3.ZVector(-labelHgt),
                    Brushes.Transparent, labelBrush, labelFontSize, ff,
                    HorizontalAlignment.Left, VerticalAlignment.Center, group);

                // Draw the line.
                if ((z > zmin) && (z < zmax - labelDz / 2))
                {
                    lineMesh.AddSegment(lineThickness,
                        new Point3D(px1, py1, pz),
                        new Point3D(px2, py1, pz));
                    lineMesh.AddSegment(lineThickness,
                        new Point3D(px1, py1, pz),
                        new Point3D(px1, py2, pz));
                }
            }
            group.Children.Add(lineMesh.MakeModel(lineBrush));

            // Tick marks.
            MeshGeometry3D tickMesh = new MeshGeometry3D();

            // Lines with different X values.
            for (double x = xmin + tickDx; x < xmax + tickDx / 2; x += tickDx)
            {
                double px = px1 + (x - xmin) * xScale;
                tickMesh.AddSegment(lineThickness,
                    new Point3D(px, py1, pz2 - tickWid),
                    new Point3D(px, py1, pz2 + tickWid));
            }

            // Lines with different Y values.
            for (double y = ymin + tickDy; y < ymax + tickDy / 2; y += tickDy)
            {
                double py = py1 + (y - ymin) * yScale;
                tickMesh.AddSegment(lineThickness,
                    new Point3D(px1, py, pz2 - tickWid),
                    new Point3D(px1, py, pz2 + tickWid));
            }

            // Lines with different Z values.
            for (double z = zmin; z < zmax + tickDz / 2; z += tickDz)
            {
                double pz = pz1 + (z - zmin) * zScale;
                tickMesh.AddSegment(lineThickness,
                    new Point3D(px2 - tickWid, py1, pz),
                    new Point3D(px2 + tickWid, py1, pz));
            }
            group.Children.Add(tickMesh.MakeModel(lineBrush));

            // Draw titles.
            ll = new Point3D(
                px1,
                py1,
                pz2 + labelWid + labelGap + titleHgt);
            MakeLabel(xtitle, ll, D3.XVector(xLength), D3.ZVector(-titleHgt),
                Brushes.Transparent, titleBrush, titleFontSize, ff,
                HorizontalAlignment.Center, VerticalAlignment.Top, group);

            ll = new Point3D(
                px1,
                py2,
                pz2 + labelWid + labelGap + titleHgt);
            MakeLabel(ytitle, ll, D3.YVector(-yLength), D3.ZVector(-titleHgt),
                Brushes.Transparent, titleBrush, titleFontSize, ff,
                HorizontalAlignment.Center, VerticalAlignment.Top, group);

            ll = new Point3D(
                px2 + labelWid + labelGap + titleHgt,
                py1,
                pz2);
            MakeLabel(ztitle, ll, D3.ZVector(-zLength), D3.XVector(-titleHgt),
                Brushes.Transparent, titleBrush, titleFontSize, ff,
                HorizontalAlignment.Center, VerticalAlignment.Top, group);
        }

        // Make a line graph.
        public void MakeLineGraph(Model3DGroup group,
            double[,] values, Brush[] valuesBrushes,
            Point3D corner, double xLength, double yLength, double zLength,
            double boxThickness, double lineThickness, double curveThickness,
            Brush boxBrush, Brush lineBrush, Brush tickBrush,
            double labelWid, double labelHgt, double labelGap, double labelFontSize, Brush labelBrush,
            double titleHgt, double titleFontSize, Brush titleBrush,
            double xmin, double xmax, double tickDx, double labelDx, string xtitle, string[] xlabels,
            double ymin, double ymax, double tickDy, double labelDy, string ytitle, string[] ylabels,
            double zmin, double zmax, double tickDz, double labelDz, string ztitle, string[] zlabels)
        {
            // Make and label the plot box.
            MakePlotBox(group,
                corner, xLength, yLength, zLength,
                boxThickness, lineThickness,
                boxBrush, lineBrush, tickBrush,
                labelWid, labelHgt, labelGap, labelFontSize, labelBrush,
                titleHgt, titleFontSize, titleBrush,
                xmin, xmax, tickDx, labelDx, xtitle, xlabels,
                ymin, ymax, tickDy, labelDy, ytitle, ylabels,
                zmin, zmax, tickDz, labelDz, ztitle, zlabels);

            // Minimum X, Y, and Z values, and scales.
            double px1 = corner.X;
            double py1 = corner.Y;
            double pz1 = corner.Z;
            double xScale = xLength / (xmax - xmin);
            double yScale = yLength / (ymax - ymin);
            double zScale = zLength / (zmax - zmin);

            // Plot the data values.
            int numItems = values.GetUpperBound(0) + 1;
            int numEntries = values.GetUpperBound(1) + 1;
            Point3D point1;
            Point3D point2 = new Point3D();
            double radius = 2 * curveThickness;
            for (int iz = 0; iz < numItems; iz++)
            {
                double pz = pz1 + iz * zScale;
                MeshGeometry3D mesh = new MeshGeometry3D();
                for (int ix = 0; ix < numEntries; ix++)
                {
                    // Save the previous point.
                    point1 = point2;

                    // Find the new point.
                    double px = px1 + ix * xScale;
                    double py = py1 + (values[iz, ix] - ymin) * yScale;
                    point2 = new Point3D(px, py, pz);

                    // Plot the point.
                    mesh.AddSphere(point2, radius, 10, 5, true);

                    // Draw the line.
                    if (ix > 0)
                        mesh.AddSegment(curveThickness, point1, point2);
                }
                group.Children.Add(mesh.MakeModel(valuesBrushes[iz]));
            }
        }

        // Make a ribbon graph.
        public void MakeRibbonGraph(Model3DGroup group,
            double[,] values, Brush[] valuesBrushes,
            Point3D corner, double xLength, double yLength, double zLength,
            double boxThickness, double lineThickness,
            Brush boxBrush, Brush lineBrush, Brush tickBrush,
            double labelWid, double labelHgt, double labelGap, double labelFontSize, Brush labelBrush,
            double titleHgt, double titleFontSize, Brush titleBrush,
            double xmin, double xmax, double tickDx, double labelDx, string xtitle, string[] xlabels,
            double ymin, double ymax, double tickDy, double labelDy, string ytitle, string[] ylabels,
            double zmin, double zmax, double tickDz, double labelDz, string ztitle, string[] zlabels,
            bool shiftZ = false)
        {
            // Make and label the plot box.
            MakePlotBox(group,
                corner, xLength, yLength, zLength,
                boxThickness, lineThickness,
                boxBrush, lineBrush, tickBrush,
                labelWid, labelHgt, labelGap, labelFontSize, labelBrush,
                titleHgt, titleFontSize, titleBrush,
                xmin, xmax, tickDx, labelDx, xtitle, xlabels,
                ymin, ymax, tickDy, labelDy, ytitle, ylabels,
                zmin, zmax, tickDz, labelDz, ztitle, zlabels,
                shiftZ);

            // Minimum X, Y, and Z values, and scales.
            double px1 = corner.X;
            double py1 = corner.Y;
            double pz1 = corner.Z;
            double xScale = xLength / (xmax - xmin);
            double yScale = yLength / (ymax - ymin);
            double zScale = zLength / (zmax - zmin);

            // Plot the data values.
            int numItems = values.GetUpperBound(0) + 1;
            int numEntries = values.GetUpperBound(1) + 1;
            for (int iz = 0; iz < numItems; iz++)
            {
                Point3D[] points = new Point3D[4];
                double z1 = pz1 + (iz - 1) * zScale;
                double z2 = pz1 + iz * zScale;
                MeshGeometry3D mesh = new MeshGeometry3D();
                for (int ix = 0; ix < numEntries; ix++)
                {
                    // Save the previous points.
                    points[0] = points[1];
                    points[3] = points[2];

                    // Find the new points.
                    double px = px1 + ix * xScale;
                    double py = py1 + (values[iz, ix] - ymin) * yScale;
                    points[1] = new Point3D(px, py, z2);
                    points[2] = new Point3D(px, py, z1);

                    // Draw the ribbon.
                    if (ix > 0)
                    {
                        mesh.AddPolygon(points);
                    }
                }
                GeometryModel3D model = mesh.MakeModel(valuesBrushes[iz]);
                model.BackMaterial = model.Material;
                group.Children.Add(model);
            }
        }

        // Make an area graph.
        public void MakeAreaGraph(Model3DGroup group,
            double[,] values, Brush[] valuesBrushes,
            Point3D corner, double xLength, double yLength, double zLength,
            double boxThickness, double lineThickness,
            Brush boxBrush, Brush lineBrush, Brush tickBrush,
            double labelWid, double labelHgt, double labelGap, double labelFontSize, Brush labelBrush,
            double titleHgt, double titleFontSize, Brush titleBrush,
            double xmin, double xmax, double tickDx, double labelDx, string xtitle, string[] xlabels,
            double ymin, double ymax, double tickDy, double labelDy, string ytitle, string[] ylabels,
            double zmin, double zmax, double tickDz, double labelDz, string ztitle, string[] zlabels,
            bool shiftZ = false)
        {
            // Make and label the plot box.
            MakePlotBox(group,
                corner, xLength, yLength, zLength,
                boxThickness, lineThickness,
                boxBrush, lineBrush, tickBrush,
                labelWid, labelHgt, labelGap, labelFontSize, labelBrush,
                titleHgt, titleFontSize, titleBrush,
                xmin, xmax, tickDx, labelDx, xtitle, xlabels,
                ymin, ymax, tickDy, labelDy, ytitle, ylabels,
                zmin, zmax, tickDz, labelDz, ztitle, zlabels,
                shiftZ);

            // Minimum X, Y, and Z values, and scales.
            double px1 = corner.X;
            double py1 = corner.Y;
            double pz1 = corner.Z;
            double px2 = px1 + xLength;
            double py2 = py1 + yLength;
            double pz2 = pz1 + zLength;
            double xScale = xLength / (xmax - xmin);
            double yScale = yLength / (ymax - ymin);
            double zScale = zLength / (zmax - zmin);

            // Plot the data values.
            int numItems = values.GetUpperBound(0) + 1;
            int numEntries = values.GetUpperBound(1) + 1;
            for (int iz = 0; iz < numItems; iz++)
            {
                MeshGeometry3D mesh = new MeshGeometry3D();
                double z1 = pz1 + (iz - 1) * zScale;
                double z2 = pz1 + iz * zScale;

                // Bottom.
                Point3D[] botPoints =
                {
                    new Point3D(px1, py1, z1),
                    new Point3D(px2, py1, z1),
                    new Point3D(px2, py1, z2),
                    new Point3D(px1, py1, z2),
                };
                mesh.AddPolygon(botPoints);

                // Left.
                double py = py1 + (values[iz, 0] - ymin) * yScale;
                Point3D[] leftPoints =
                {
                    new Point3D(px1, py, z2),
                    new Point3D(px1, py, z1),
                    new Point3D(px1, py1, z1),
                    new Point3D(px1, py1, z2),
                };
                mesh.AddPolygon(leftPoints);

                // Right.
                py = py1 + (values[iz, numEntries - 1] - ymin) * yScale;
                Point3D[] rightPoints =
                {
                    new Point3D(px2, py1, z2),
                    new Point3D(px2, py1, z1),
                    new Point3D(px2, py, z1),
                    new Point3D(px2, py, z2),
                };
                mesh.AddPolygon(rightPoints);

                Point3D[] topPoints = new Point3D[4];
                Point3D[] frontPoints = new Point3D[4];
                Point3D[] backPoints = new Point3D[4];
                for (int ix = 0; ix < numEntries; ix++)
                {
                    // Save the previous points.
                    topPoints[0] = topPoints[1];
                    topPoints[3] = topPoints[2];
                    frontPoints[0] = frontPoints[1];
                    frontPoints[3] = frontPoints[2];
                    backPoints[0] = backPoints[1];
                    backPoints[3] = backPoints[2];

                    // Find the new points.
                    double px = px1 + ix * xScale;
                    py = py1 + (values[iz, ix] - ymin) * yScale;
                    topPoints[1] = new Point3D(px, py, z2);
                    topPoints[2] = new Point3D(px, py, z1);
                    frontPoints[1] = new Point3D(px, py1, z2);
                    frontPoints[2] = new Point3D(px, py, z2);
                    backPoints[1] = new Point3D(px, py, z1);
                    backPoints[2] = new Point3D(px, py1, z1);

                    // Draw the area's sides.
                    if (ix > 0)
                    {
                        mesh.AddPolygon(topPoints);
                        mesh.AddPolygon(frontPoints);
                        mesh.AddPolygon(backPoints);
                    }
                }
                group.Children.Add(mesh.MakeModel(valuesBrushes[iz]));
            }
        }

        // Make a stacked area graph.
        public void MakeStackedAreaGraph(Model3DGroup group,
            double[,] values, Brush[] valuesBrushes,
            Point3D corner, double xLength, double yLength, double zLength,
            double lineThickness, Brush lineBrush, 
            double labelWid, double labelFontSize, Brush labelBrush,
            double xmin, double xmax, string xtitle, string[] xlabels,
            double ymin, double ymax, string ytitle, string[] ylabels,
            bool shiftZ = false)
        {
            // Minimum X, Y, and Z values, and scales.
            double px1 = corner.X;
            double py1 = corner.Y;
            double pz1 = corner.Z;
            double px2 = px1 + xLength;
            double py2 = py1 + yLength;
            double pz2 = pz1 + zLength;
            double xScale = xLength / (xmax - xmin);
            double yScale = yLength / (ymax - ymin);
            double zScale = zLength;

            // The Z coordinates remain constant.
            double z1 = pz1;
            double z2 = pz1 + zScale;

            // See how many values we have.
            int numStrips = values.GetUpperBound(0) + 1;
            int numPieces = values.GetUpperBound(1) + 1;

            // Keep track of the running totals for each piece.
            double[] totals = new double[numPieces];

            // Plot the data values.
            for (int strip = 0; strip < numStrips; strip++)
            {
                // Make the iz strip in the area graph.
                MeshGeometry3D mesh = new MeshGeometry3D();

                // Track the pieces' starting X coordinate.
                double x1 = px1;
                double x2 = x1 + xScale;

                // Left end.
                double y1 = py1 + yScale * (totals[0]);
                double y2 = y1 + yScale * (values[strip, 0]);
                Point3D[] points =
                {
                    new Point3D(x1, y1, z1),
                    new Point3D(x1, y1, z2),
                    new Point3D(x1, y2, z2),
                    new Point3D(x1, y2, z1),
                };
                mesh.AddPolygon(points);

                // Right end.
                y1 = py1 + yScale * (totals[numPieces - 1]);
                y2 = y1 + yScale * (values[strip, numPieces - 1]);
                points = new Point3D[]
                {
                    new Point3D(px2, y1, z2),
                    new Point3D(px2, y1, z1),
                    new Point3D(px2, y2, z1),
                    new Point3D(px2, y2, z2),
                };
                mesh.AddPolygon(points);

                // Draw the pieces of the strip.
                for (int piece = 0; piece < numPieces - 1; piece++)
                {
                    y1 = py1 + yScale * (totals[piece]);
                    y2 = py1 + yScale * (totals[piece + 1]);
                    Point3D bot1 = new Point3D(x1, y1, z2);
                    Point3D bot2 = new Point3D(x2, y2, z2);
                    Point3D bot3 = new Point3D(x2, y2, z1);
                    Point3D bot4 = new Point3D(x1, y1, z1);

                    y1 = py1 + yScale * (totals[piece] + values[strip, piece]);
                    y2 = py1 + yScale * (totals[piece + 1] + values[strip, piece + 1]);
                    Point3D top1 = new Point3D(x1, y1, z2);
                    Point3D top2 = new Point3D(x2, y2, z2);
                    Point3D top3 = new Point3D(x2, y2, z1);
                    Point3D top4 = new Point3D(x1, y1, z1);

                    // Top.
                    points = new Point3D[] { top1, top2, top3, top4 };
                    mesh.AddPolygon(points);

                    // Front side.
                    points = new Point3D[] { bot1, bot2, top2, top1 };
                    mesh.AddPolygon(points);

                    // Back side.
                    points = new Point3D[] { top4, top3, bot3, bot4 };
                    mesh.AddPolygon(points);

                    // Move to the next piece.
                    x1 = x2;
                    x2 += xScale;
                }

                // Update the running totals.
                for (int piece = 0; piece < numPieces; piece++)
                    totals[piece] += values[strip, piece];

                // Bottom.
                if (strip == 1)
                {
                    points = new Point3D[]
                    {
                        new Point3D(px1, py1, z1),
                        new Point3D(px2, py1, z1),
                        new Point3D(px2, py1, z2),
                        new Point3D(px1, py1, z2),
                    };
                    mesh.AddPolygon(points);
                }

                // Make the model.
                group.Children.Add(mesh.MakeModel(valuesBrushes[strip]));
            }

            // Front labels.
            double x = px1;
            for (int piece = 0; piece < numPieces; piece++)
            {
                MakeLabel(xlabels[piece],
                    new Point3D(x + xScale / 2, py1, labelWid + pz2),
                    D3.ZVector(-labelWid), D3.XVector(-xScale),
                    Brushes.Transparent, labelBrush, labelFontSize, FontFamily,
                    HorizontalAlignment.Left, VerticalAlignment.Center, group);
                x += xScale;
            }

            // Side labels and lines.
            MeshGeometry3D lineMesh = new MeshGeometry3D();
            double dy = 10 * yScale;
            double y = py1;
            for (int i = 0; i < ylabels.Length; i++)
            {
                // Make the label.
                Point3D ll = new Point3D(px1, y - dy / 2, labelWid + pz2 + 0.5 * zScale);
                MakeLabel(ylabels[i], ll, D3.ZVector(-labelWid), D3.YVector(dy),
                    Brushes.Transparent, labelBrush, labelFontSize, FontFamily,
                    HorizontalAlignment.Center, VerticalAlignment.Center, group);

                // Draw the line.
                Point3D p1 = new Point3D(px1, y, pz1);
                Point3D p2 = new Point3D(px2, y, pz1);
                lineMesh.AddSegment(lineThickness, p1, p2);

                y += dy;
            }
            group.Children.Add(lineMesh.MakeModel(lineBrush));

            //@@@
        }

        // Load data.
        // Source: Made up.
        // Values hold sales for [item, month].
        private double[,] LoadData()
        {
            return new double[,]
            {
                { 0, 0, 0, 0, 0, },
                { 16, 14, 17, 21, 13, },
                { 12, 13, 16, 20, 17, },
                { 14, 15, 18, 16, 19, },
                { 20, 18, 27, 25, 18, },
            };
        }
    }
}
