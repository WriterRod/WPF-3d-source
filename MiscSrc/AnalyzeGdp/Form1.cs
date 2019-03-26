using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace AnalyzeGdp
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // Calculate the data.
            Bitmap bm = (Bitmap)picData.Image;
            double min_x = 103;
            double max_x = 771;
            double x = min_x;
            double min_y = 99;
            double max_y = 622;
            double min_value = 0;
            double max_value = 450;
            double yscale = (max_value - min_value) / (max_y - min_y);
            int min_year = 1968;
            int max_year = 2008;
            int num_years = max_year - min_year + 1;
            double dx = (max_x - min_x) / (num_years - 1);
            double[,] values = new double[num_years, 4];
            for (int year = min_year; year <= max_year; year++)
            {
                int ix = (int)x;
                int last_y = 622;
                int iy = 580;
                for (int q = 0; q < 4; q++)
                {
                    Color color = bm.GetPixel(ix, iy);
                    while (bm.GetPixel(ix, iy) == color) iy--;

                    double value = min_value + (last_y - iy) * yscale;
                    values[year - min_year, q] = value;

                    bm.SetPixel(ix, iy + 1, Color.Yellow);//@
                    last_y = iy;
                }
                x += dx;
            }

            // Display the data.
            Console.WriteLine("double[,] values =");
            Console.WriteLine("{");
            for (int year = 0; year < num_years; year++)
            {
                Console.Write("    { ");
                for (int q = 0; q < 4; q++)
                {
                    Console.Write(values[year, q] + ", ");
                }
                Console.WriteLine("},");
            }
            Console.WriteLine("};");

            // Draw the data.
            int wid = 600;
            int hgt = 600;
            Bitmap new_bm = new Bitmap(wid, hgt);
            using (Graphics gr = Graphics.FromImage(new_bm))
            {
                gr.Clear(Color.White);
                for (int q = 0; q < 4; q++)
                {
                    PointF[] points = new PointF[num_years];
                    for (int year = 0; year < num_years; year++)
                    {
                        float fx = year * wid / (float)num_years;
                        float fy = hgt - (float)(values[year, q] * hgt / max_value);
                        points[year] = new PointF(fx, fy);
                    }
                    gr.DrawLines(Pens.Black, points);
                }
            }
            picGraph.Image = new_bm;
        }
    }
}
