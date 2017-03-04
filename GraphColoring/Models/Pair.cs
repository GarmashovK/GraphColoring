using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphColoring.Models
{
    // Dij
    public class Pair
    {
        public int Left { get; set;}
        public int Right { get; set;}

        public List<int> Set { get; set; }

        public Pair() { }
    }
}
