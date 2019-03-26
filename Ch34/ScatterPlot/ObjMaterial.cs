using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Windows.Media.Media3D;

namespace ScatterPlot
{
    // Hold information for an mtl file material.
    public class ObjMaterial
    {
        public string Name = "";
        public string Filename = null;
        public double Ns, Alpha;
        public Point3D Kd, Ks, Ka;
        public MaterialGroup MatGroup;
    }
}
