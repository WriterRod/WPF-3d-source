using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Moonscape
{
    public static class RandomExtensions
    {
        // Return a double between min inclusive and max exclusive.
        public static double NextDouble(this Random rand, double min, double max)
        {
            return min + rand.NextDouble() * (max - min);
        }
    }
}
