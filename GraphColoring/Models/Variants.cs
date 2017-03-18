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

        public Variants(Variants toClone)
        {
            this.SetOfPairs = new List<Pair>();
            toClone.SetOfPairs.ForEach(pair =>
            this.SetOfPairs.Add(new Pair(pair)));
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

        // просеивание по двум элементам
        public Variants Sieve(int left, int right)
        {
            var result = new Variants(this);
            for(var i=0; i<SetOfPairs.Count; i++)
            {
                var pair = new Pair(result.SetOfPairs[i]);

                if (pair.Left != left && pair.Right != right)
                {
                    pair.Set.Remove(left);
                    pair.Set.Remove(right);
                }else
                {
                    result.SetOfPairs.RemoveAt(i);
                    i--;
                }
            }

            return result;
        }

        public Variants Sieve(List<int> supSet)
        {
            var result = new Variants(this);

            for(var i=0; i< result.SetOfPairs.Count; i++)
            {
                var pair = result.SetOfPairs[i];

                if (!supSet.Contains(pair.Left) ||
                    !supSet.Contains(pair.Right))
                {
                    result.SetOfPairs.RemoveAt(i);
                    i--;
                }else
                {
                    pair.Set = pair.Set.Intersect(supSet).ToList();
                }
            }

            return result;
        }

        public void Sift(List<int> supSet)
        {
            for (var i = 0; i < this.SetOfPairs.Count; i++)
            {
                var pair = this.SetOfPairs[i];
                if (pair.Set.Except(supSet).Count() == 0)
                {
                    this.SetOfPairs.RemoveAt(i);
                    i--;
                }
            }
        }
    }
}
