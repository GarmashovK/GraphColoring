using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphColoring.Models
{
    public class Variants
    {
        public List<Pair> SetOfPairs { get; private set; }

        public int Count { get { return SetOfPairs.Count; } }
        public Variants()
        {
            SetOfPairs = new List<Pair>();
        }
        // создание вариантов перетановок по симметричной матрице графов и опорному множеству
        public static Variants CreateFromMatrix(int dim, bool[,] matrix, List<int> w)
        {
            var result = new Variants();

            for (var i = 0; i < dim; i++)
            {
                for (var j = i + 1; j < dim; j++)
                {
                    if (matrix[i, j]) continue;

                    var pair = new Pair();
                    pair.Left = i;
                    pair.Right = j;
                    pair.Set = new List<int>();

                    for (var k =0; k<dim; k++)
                    {
                        if (!matrix[i, k] && !matrix[k, j])
                        {
                            if (w.Contains(k))
                                pair.Set.Add(k);
                        }
                    }
                    result.SetOfPairs.Add(pair);
                }
            }

            result.SetOfPairs.OrderBy(p => p.Set.Count);
            return result;
        }

        
    }
}
