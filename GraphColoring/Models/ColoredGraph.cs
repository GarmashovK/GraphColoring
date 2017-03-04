using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphColoring.Models
{
    public class ColoredGraph
    {
        public int NumOfNodes { get; set; }

        public bool[,] Graph { get; set; }

        public List<int[]> ColoredNodes { get; set; } 
    }
}
