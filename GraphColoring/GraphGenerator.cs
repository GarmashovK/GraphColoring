using GraphColoring.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphColoring {
    public class GraphGenerator {
        private static List<KeyValuePair<int,int>> GetPossibleVertexes(int n) {
            var result = new List<KeyValuePair<int, int>>();
            
            for(var i=0; i < n; i++) {
                for (var j = i + 1; j < n; j++) {
                    result.Add(new KeyValuePair<int, int>(i, j));
                }
            }
            return result;
        }

        public static bool[,] GenerateByDensity(int n, double density) {
            var result = new bool[n, n];
            var selection = GetPossibleVertexes(n);
            var random = new Random(DateTime.Now.Millisecond);
            var numOfEdges = (int)(density * selection.Count);

            while (numOfEdges != 0) {
                var num = random.Next(selection.Count);
                var elem = selection.ElementAt(num);

                result[elem.Key, elem.Value] = true;
                result[elem.Value, elem.Key] = true;

                selection.RemoveAt(num);
                numOfEdges--;
            }

            return result;
        }
    }
}
