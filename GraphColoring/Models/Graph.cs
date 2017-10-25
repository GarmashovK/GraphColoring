using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphColoring.Models {
    public class Graph {
        public int NumberOfVertexes { get; private set; }
        public List<int> NamesOfVertexes { get; private set; }
        public bool[,] MAdjacency { get; private set; }

        public Graph(Graph other) {
            this.NumberOfVertexes = other.NumberOfVertexes;
            this.NamesOfVertexes = new List<int>(other.NamesOfVertexes);
            this.MAdjacency = (bool[,])other.MAdjacency.Clone();
        }

        public Graph(bool[,] adjacency, int numOfVertex) {
            MAdjacency = (bool[,])adjacency.Clone();
            NumberOfVertexes = numOfVertex;

            NamesOfVertexes = new List<int>();
            for(var i=0; i < NumberOfVertexes; i++) {
                NamesOfVertexes.Add(i + 1);
            }
        }

        public Graph(bool[,] adjacency, 
                int numOfVertexes,
                List<int> namesOfVertexes) {
            MAdjacency = (bool[,])adjacency.Clone();
            NumberOfVertexes = numOfVertexes;
            NamesOfVertexes = namesOfVertexes;
        }

        public Graph RemoveVertex(int vertexName) {
            var pos = NamesOfVertexes.ToList().IndexOf(vertexName);
            if (pos < 0)
                return new Graph(this);

            var tmpNames = new List<int>(NamesOfVertexes);
            tmpNames.RemoveAt(pos);
            var tmpAdjacency = 
                new bool[NumberOfVertexes - 1
                , NumberOfVertexes - 1];

            for(var i=0; i< NumberOfVertexes; i++) {
                if (i == pos) continue;
                for (var j = i + 1; j<NumberOfVertexes; j++) {
                    if (j == pos) continue;

                    if (i > pos) {
                        if(j > pos) {
                            tmpAdjacency[i - 1, j - 1] = tmpAdjacency[j - 1, i - 1] = MAdjacency[i, j];
                        }

                        tmpAdjacency[i - 1, j] = tmpAdjacency[j, i - 1] = MAdjacency[i, j];
                    }
                }
            }

            return new Graph(tmpAdjacency, tmpNames.Count, tmpNames);
        }

        public Graph RemoveVertexes(int[] vertexes) {
            var tmpNames = NamesOfVertexes.Except(vertexes).ToList();
            var positions = GetPositions(tmpNames);
            var len = tmpNames.Count();
            var matrix = new bool[len, len];
            
            for (var i=0; i < tmpNames.Count; i++) {
                for(var j = i + 1; j < tmpNames.Count; j++) {
                    matrix[i, j] = MAdjacency[positions[i], positions[j]];
                }
            }

            return new Graph(matrix, len, tmpNames);
        }

        private int[] GetPositions(IList<int> vertexes) {
            var result = new int[vertexes.Count()];

            for(var i=0; i < result.Length; i++) {
                result[i] = NamesOfVertexes.IndexOf(vertexes[i]);
                if (result[i] < 0) throw new ArgumentOutOfRangeException() { Source = "Graph" };
            }

            return result;
        }

        private List<int> GetFullSet() {
            var result = new List<int>();

            for (var i = 0; i < NamesOfVertexes.Count; i++)
                result.Add(i);

            return result;
        }
        
        //получить максимально независимое множество
        //public List<List<int>> GetMIS() {
        //    int k = 0;
        //    var indSet = new List<int>();
        //    var used = new List<int>();
        //    var canBeUsed = GetFullSet();

        //    var vertex = canBeUsed.First();
        //    indSet.Add(vertex);
        //    var nextUsed = new List<int>(used);
        //    var nextCanBeUsed = new List<int>(canBeUsed);
        //    foreach
        //}
    }
}
