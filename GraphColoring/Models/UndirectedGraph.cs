using GraphColoring.ColorGraph;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphColoring {
    public class UndirectedGraph {
        public int NumOfVertices { get; private set; }
        public List<int> VerticesNames { get; private set; }
        public bool[,] AdjecencyMatrix { get; private set; }

        public UndirectedGraph(int verticesCount, bool[,] graph, List<int> names) {
            this.AdjecencyMatrix = (bool[,])graph.Clone();
            NumOfVertices = verticesCount;
            VerticesNames = names;
        }

        public UndirectedGraph(int verticesCount, bool[,] graph, int[] names) {
            if (graph.Length / verticesCount != verticesCount || names.Length != verticesCount)
                throw new Exception("Number of peaks is not equal with the graph demension!");
            this.AdjecencyMatrix = (bool[,])graph.Clone();
            NumOfVertices = verticesCount;
            VerticesNames = names.ToList();
        }

        public UndirectedGraph(int verticesCount, bool[,] graph) {
            if (graph.Length / verticesCount != verticesCount)
                throw new Exception("Number of peaks is not equal with the graph demension!");
            this.AdjecencyMatrix = (bool[,])graph.Clone();
            NumOfVertices = verticesCount;
            VerticesNames = GetStandartNames(NumOfVertices);
        }

        public UndirectedGraph(int verticesCount, double edgeDensity)
            : this(verticesCount, edgeDensity, GetStandartNames(verticesCount)) { }

        public UndirectedGraph(int verticesCount, double edgeDensity, List<int> names) {

            if (edgeDensity > 1 || edgeDensity < 0)
                throw new Exception("Density isn't in range of 0 to 1!");

            NumOfVertices = verticesCount;
            VerticesNames = names;
            int maxNumOfPeaks = (int)(verticesCount * (verticesCount - 1) / 2);
            int edgesNum = Convert.ToInt32(Math.Round(edgeDensity * maxNumOfPeaks));

            AdjecencyMatrix = GetEmptyGraph(NumOfVertices);

            var edges = GetAllPossibleEdges();
            var rand = new Random();

            for (var i = 0; i < edgesNum; i++) {
                int pos = rand.Next(edges.Count);
                AdjecencyMatrix[edges[pos].Key, edges[pos].Value] = AdjecencyMatrix[edges[pos].Value, edges[pos].Key] = true;
                edges.RemoveAt(pos);
            }
        }

        private List<KeyValuePair<int, int>> GetAllPossibleEdges() {
            var _edges = new List<KeyValuePair<int, int>>();
            for (var i = 0; i < NumOfVertices; i++)
                for (var j = i + 1; j < NumOfVertices; j++)
                    _edges.Add(new KeyValuePair<int, int>(i, j));

            return _edges;
        }

        private static List<int> GetStandartNames(int count) {
            var names = new int[count];
            for (var i = 0; i < count; i++)
                names[i] = i;
            return names.ToList();
        }

        private static bool[,] GetEmptyGraph(int count) {
            var graph = new bool[count, count];
            for (var i = 0; i < count; i++)
                for (var j = 0; j < count; j++)
                    graph[i, j] = false;
            return graph;
        }

        public override string ToString() {
            var result = "";

            //for (var i = 0; i < NumOfVertices; i++)
            //{
            //    result += VerticesNames[i];
            //}
            //result += "\n";
            for (var i = 0; i < NumOfVertices; i++) {
                for (var j = 0; j < NumOfVertices; j++) {
                    result += AdjecencyMatrix[i, j] ? 1 : 0;
                }
                if (i != NumOfVertices - 1) result += "\n";
            }
            return result;
        }

        public List<UndirectedGraph> GetComponents() {
            var components = new List<UndirectedGraph>();
            int[] remainder = new int[NumOfVertices];
            int[] foundPeaks = null;
            int root = 0;
            for (var i = 0; i < NumOfVertices; i++) remainder[i] = i;

            do {
                root = remainder.FirstOrDefault();
                foundPeaks = DeepSearch(root);
                remainder = remainder.Except(foundPeaks).ToArray();

                var tmpGraph = new bool[foundPeaks.Length, foundPeaks.Length];
                int strCount = 0;
                int colCount = 0;
                foreach (var i in foundPeaks) {
                    colCount = 0;
                    foreach (var j in foundPeaks)
                        tmpGraph[strCount, colCount++] = AdjecencyMatrix[i, j];
                    strCount++;
                }
                components.Add(new UndirectedGraph(foundPeaks.Length, tmpGraph, GetNamesAtPositions(foundPeaks)));
            } while (remainder.Count() != 0);

            return components;
        }

        private int[] GetNamesAtPositions(int[] pos) {
            var names = new int[pos.Length];
            for (var i = 0; i < pos.Length; i++) {
                names[i] = VerticesNames[pos[i]];
            }
            return names;
        }

        public int[] DeepSearch(int root) {
            var n = NumOfVertices;
            var traversal = new bool[n];
            var peaks = new List<int>();
            var stack = new Stack<int>();

            for (var i = 0; i < n; i++) traversal[i] = false;

            stack.Push(root);
            traversal[root] = true;

            do {
                var u = stack.Pop();
                peaks.Add(u);

                for (var i = 0; i < n; i++) {
                    if (AdjecencyMatrix[u, i] && !traversal[i]) {
                        stack.Push(i);
                        traversal[i] = true;
                    }
                }

            } while (stack.Count != 0);

            return peaks.OrderBy(x => x).ToArray();
        }

        public bool IsEdge(int a, int b) {
            int posA = 0, posB = 0;
            byte check = 0;
            for (var i = 0; i < VerticesNames.Count && check < 2; i++)
                if (VerticesNames[i] == a) { posA = i; check++; } else if (VerticesNames[i] == b) { posB = i; check++; }

            return AdjecencyMatrix[posA, posB];
        }

        public override bool Equals(object obj) {
            var other = (UndirectedGraph)obj;
            if (other.AdjecencyMatrix.Length != this.AdjecencyMatrix.Length)
                return false;
            var n = this.NumOfVertices;

            for (var i = 0; i < n; i++) {
                for (var j = 0; j < n; j++) {
                    if (other.AdjecencyMatrix[i, j] != this.AdjecencyMatrix[i, j])
                        return false;
                }
            }
            var inter = other.VerticesNames.Intersect(this.VerticesNames).Count();
            if (other.VerticesNames.Intersect(this.VerticesNames).Count() != NumOfVertices)
                return false;

            return true;
        }

        public override int GetHashCode() {
            return base.GetHashCode();
        }

        public List<int> MISBacktracking() {
            List<int> result = null;
            int m = 0;
            Func<List<int>, int, bool> isIndependent = (set, v) => {
                var posV = GetPosByName(v);
                for (var i = 0; i < set.Count; i++) {
                    var posU = GetPosByName(set[i]);
                    if (AdjecencyMatrix[posV, posU]) return false;
                }
                return true;
            };
            Action<List<int>, List<int>> IS = null;
            IS = (S, T) => {
                if (S.Count > m) {
                    result = S.ToList();
                    m = S.Count;
                }

                for (var i = 0; i < T.Count; i++) {
                    if (isIndependent(S, T[i])) {
                        var neigbors = GetNeighbors(T[i]);
                        neigbors.Add(T[i]);
                        S.Add(T[i]);
                        IS(S, T.Except(neigbors).ToList());
                        S.RemoveAt(S.Count - 1);
                    }
                }
            };

            IS(new List<int>(), VerticesNames.ToList());

            return result;
        }

        private List<int> RemoveElems(List<int> list, List<int> elems) {
            var result = new List<int>(list);
            for (var i = 0; i < elems.Count; i++) {
                result.Remove(elems[i]);
            }
            return result;
        }

        public List<List<int>> MIS() {
            var result = new List<List<int>>();

            if (NumOfVertices == 0) return result;

            var currentIS = new List<int>();
            var Q_used = new List<List<int>>();
            var Q_not_used = new List<List<int>>();
            int lvl = 0;

            Q_used.Add(new List<int>());
            Q_not_used.Add(new List<int>(VerticesNames));

            if (Q_not_used.Count == 0) return result;

            Action stepBackward = null,
                   stepForward = null,
                   check = null;

            stepForward = () => {
                var selected = Q_not_used[lvl].First();
                var neighbors = GetNeighbors(selected);
                currentIS.Add(selected);
                Q_used.Add(RemoveElems(Q_used[lvl], neighbors));
                Q_not_used.Add(RemoveElems(Q_not_used[lvl], neighbors));
                Q_not_used[++lvl].Remove(selected);

                check();
            };

            check = () => {
                if (lvl != 0) {
                    for (var i = 0; i < Q_used[lvl].Count; i++) {
                        var neighbors = GetNeighbors(Q_used[lvl][i]);
                        if (neighbors.Intersect(Q_not_used[lvl]).Count() == 0) {
                            stepBackward();
                            return;
                        }
                    }
                }
                if (Q_not_used[lvl].Count == 0) {
                    if (Q_used[lvl].Count == 0)
                        result.Add(new List<int>(currentIS));
                    stepBackward();
                } else stepForward();
            };

            stepBackward = () => {
                var last = currentIS.LastOrDefault();

                if (lvl != 0) {
                    currentIS.Remove(last);

                    Q_used.RemoveAt(lvl);
                    Q_not_used.RemoveAt(lvl);

                    lvl--;
                    Q_used[lvl].Add(last);
                    Q_not_used[lvl].Remove(last);
                }

                if (lvl == 0 && Q_not_used[lvl].Count == 0)
                    return;
                else
                    check();
            };

            stepForward();

            return result;
        }

        public List<int> GetNeighbors(int v) {
            var result = new List<int>();
            var posV = GetPosByName(v);
            for (var i = 0; i < NumOfVertices; i++) {
                if (i == posV) continue;
                if (AdjecencyMatrix[i, posV]) result.Add(VerticesNames[i]);
            }
            return result;
        }

        public int GetPosByName(int name) {
            if (!VerticesNames.Contains(name))
                throw new ArgumentException();
            for (var i = 0; i < VerticesNames.Count; i++)
                if (name == VerticesNames[i])
                    return i;
            return -1;
        }

        public void Complement() {
            for (var i = 0; i < NumOfVertices; i++) {
                for (var j = i + 1; j < NumOfVertices; j++) {
                    AdjecencyMatrix[i, j] = AdjecencyMatrix[j, i] = !AdjecencyMatrix[i, j];
                }
            }
        }

        public UndirectedGraph Union(UndirectedGraph other) {
            var len = other.NumOfVertices + this.NumOfVertices;
            var _newGraph = GetEmptyGraph(len);

            for (var i = 0; i < this.NumOfVertices; i++) {
                for (var j = i + 1; j < this.NumOfVertices; j++) {
                    _newGraph[i, j] = _newGraph[j, i] = this.AdjecencyMatrix[i, j];
                }
            }

            for (var i = 0; i < other.NumOfVertices; i++) {
                for (var j = i + 1; j < other.NumOfVertices; j++) {
                    _newGraph[i + this.NumOfVertices, j + this.NumOfVertices]
                        = _newGraph[j + this.NumOfVertices, i + this.NumOfVertices]
                        = other.AdjecencyMatrix[i, j];
                }
            }

            return new UndirectedGraph(len, _newGraph);
        }

        public UndirectedGraph Intersect(UndirectedGraph other) {
            var verInter = this.VerticesNames.Intersect(other.VerticesNames).ToArray();
            var result = new bool[verInter.Length, verInter.Length];
            int tmpI1 = 0, tmpI2 = 0;

            for (var i = 0; i < verInter.Length; i++, tmpI1++, tmpI2++) {
                int tmpJ1 = 0, tmpJ2 = 0;
                while (!verInter.Contains(this.VerticesNames[tmpI1])) tmpI1++;
                while (!verInter.Contains(other.VerticesNames[tmpI2])) tmpI2++;

                for (var j = 0; j < verInter.Length; j++, tmpJ1++, tmpJ2++) {
                    while (!verInter.Contains(this.VerticesNames[tmpJ1])) tmpJ1++;
                    while (!verInter.Contains(other.VerticesNames[tmpJ2])) tmpJ2++;

                    result[i, j] = result[j, i] = this.AdjecencyMatrix[tmpI1, tmpJ1] & other.AdjecencyMatrix[tmpI2, tmpJ2];
                }
            }
            return new UndirectedGraph(verInter.Length, result, verInter);
        }

        public void RemoveEdge(int from, int to) {
            Func<int, int> getPos = (x) => {
                for (var i = 0; i < NumOfVertices; i++)
                    if (VerticesNames[i] == x) return i;
                throw new ArgumentException();
            };

            int pos1 = getPos(from),
                pos2 = getPos(to);

            AdjecencyMatrix[pos1, pos2] = AdjecencyMatrix[pos2, pos1] = false;
        }

        public void RemoveVertice(int name) {
            if (!VerticesNames.Contains(name))
                throw new ArgumentException();

            var _newGraph = new bool[NumOfVertices - 1, NumOfVertices - 1];
            int pos = 0;
            for (var i = 0; i < VerticesNames.Count; i++) {
                if (VerticesNames[i] == name) {
                    pos = i;
                    break;
                }
            }
            int tempV = 0, tempH = 0;

            for (var i = 0; i < NumOfVertices; i++) {
                tempH = 0;
                if (i == pos) {
                    tempV = 1;
                    continue;
                }
                for (var j = i + 1; j < NumOfVertices; j++) {
                    if (j >= pos) tempH = 1;
                    _newGraph[i - tempV, j - tempH] = _newGraph[j - tempH, i - tempV] = AdjecencyMatrix[i, j];
                }
                _newGraph[i - tempV, i - tempV] = false;
            }

            NumOfVertices--;
            AdjecencyMatrix = _newGraph;
        }

        public UndirectedGraph RemoveVertices(IList<int> vertices) {
            var tmpNames = VerticesNames.Except(vertices).ToList();
            var positions = GetPositions(tmpNames);
            var len = tmpNames.Count();
            var matrix = new bool[len, len];

            for (var i = 0; i < tmpNames.Count; i++) {
                for (var j = i + 1; j < tmpNames.Count; j++) {
                    matrix[i, j] = AdjecencyMatrix[positions[i], positions[j]];
                    matrix[j, i] = AdjecencyMatrix[positions[i], positions[j]];
                }
            }

            return new UndirectedGraph(len, matrix, tmpNames);
        }

        private int[] GetPositions(IList<int> vertexes) {
            var result = new int[vertexes.Count()];

            for (var i = 0; i < result.Length; i++) {
                result[i] = VerticesNames.IndexOf(vertexes[i]);
                if (result[i] < 0) throw new ArgumentOutOfRangeException() { Source = "Graph" };
            }

            return result;
        }

        public static UndirectedGraph operator +(UndirectedGraph graph1, UndirectedGraph graph2) {
            var len = graph1.NumOfVertices + graph2.NumOfVertices;
            var _newGraph = new bool[len, len];

            for (var i = 0; i < graph1.NumOfVertices; i++) {
                for (var j = i + 1; j < graph1.NumOfVertices; j++) {
                    _newGraph[i, j] = _newGraph[j, i] = graph1.AdjecencyMatrix[i, j];
                }
                _newGraph[i, i] = false;
            }

            for (var i = 0; i < graph2.NumOfVertices; i++) {
                for (var j = i + 1; j < graph2.NumOfVertices; j++) {
                    _newGraph[i + graph1.NumOfVertices, j + graph1.NumOfVertices]
                        = _newGraph[j + graph1.NumOfVertices, i + graph1.NumOfVertices]
                        = graph2.AdjecencyMatrix[i, j];
                }
                _newGraph[i + graph1.NumOfVertices, i + graph1.NumOfVertices] = false;
            }

            for (var i = 0; i < graph1.NumOfVertices; i++) {
                for (var j = graph1.NumOfVertices; j < len; j++) {
                    _newGraph[i, j] = _newGraph[j, i] = true;
                }
            }

            return new UndirectedGraph(len, _newGraph);
        }

        public List<List<int>> GetMISChromaticNumber() {
            var result = new List<List<int>>();
            var tmp = new List<List<int>>();
            var max = NumOfVertices;

            Action<UndirectedGraph> chromaticFunc = null;

            chromaticFunc =
                (G) => {
                    var all_MIS = G.MIS();

                    if (all_MIS.Count == 0) {
                        if (tmp.Count < max) {
                            result = new List<List<int>>(tmp);
                            max = result.Count;
                            return;
                        }
                    }

                    foreach (List<int> IS in all_MIS) {
                        tmp.Add(IS);
                        chromaticFunc(G.RemoveVertices(IS));
                        tmp.RemoveAt(tmp.Count - 1);
                    }
                };

            chromaticFunc(this);

            return result;
        }

        public List<List<int>> GetOlemskoyChromaticNumber() {
            var olemGraph = new OlemskoyColorGraph(this.AdjecencyMatrix);
            
            return olemGraph.ResultColorNodes;
        }

        public List<List<int>> GetOlemskoyBinChromaticNumber() {
            var olemGraph = new BinaryOlemskoyColorGraph(AdjecencyMatrix, NumOfVertices);

            return olemGraph.Calculate();
        }

        private IList<int> BuildPermutation(List<List<int>> colored) {
            var tmp = new List<int>();

            foreach (List<int> color in colored) {
                tmp.AddRange(color);
            }

            return GetPositions(tmp);
        }

        public string PrintColors(List<List<int>> coloredV) {
            var result = "";
            result += string.Format("Кол-во цветов: {0}\n", coloredV.Count);

            result += "Раскраска: \n";

            for (var i = 0; i < coloredV.Count; i++) {
                result += string.Format("{0} - ", i + 1);

                for (var j = 0; j < coloredV[i].Count; j++) {
                    result += string.Format("{0} ", coloredV[i][j]);
                }
                result += "\n";
            }

            return result;
        }

        public string PrintGraph(bool[,] graph) {
            string result = "";
            var n = NumOfVertices;
            
            result = ("Граф: \n");

            for (var i = 0; i < n; i++) {
                for (var j = 0; j < n; j++) {
                    result += string.Format("{0} ", graph[i, j] ? 1 : 0);
                }
                result += "\n";
            }

            return result;
        }

        public string OutColoredGraph(List<List<int>> coloredV) {
            var result = "";
            var permutation = BuildPermutation(coloredV);
            var n = NumOfVertices;
            var tmp = new bool[n, n];


            for (var i = 0; i < n; i++) {
                for (var j = 0; j < n; j++) {
                    tmp[i, j] = AdjecencyMatrix[permutation[i], permutation[j]];
                }
            }

            result += PrintColors(coloredV);
            result += PrintGraph(tmp);


            return result;
        }
    }
}
