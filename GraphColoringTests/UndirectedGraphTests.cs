using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using GraphColoring;

namespace GraphColoringTests {
    [TestClass]
    public class UndirectedGraphTests {
        [TestMethod]
        public void TestMISColoring() {
            bool[,] matrix = new bool[,]
            {
                {false,false,false,false,false,true,true,false,false,true,false,true,false,false,false,true },
                {false,false,false,false,false,true,true,true,false,false,false,false,true,false,false,true },
                {false,false,false,false,false,false,true,true,true,false,false,false,false,true,false,true },
                {false,false,false,false,false,false,false,true,true,true,false,false,false,false,true,true },
                {false,false,false,false,false,true,false,false,true,true,true,false,false,false,false,true },
                {true,true,false,false,true,false,false,false,false,false,false,false,false,true,false,false },
                {true,true,true,false,false,false,false,false,false,false,false,false,false,false,true,false },
                {false,true,true,true,false,false,false,false,false,false,true,false,false,false,false,false },
                {false,false,true,true,true,false,false,false,false,false,false,true,false,false,false,false },
                {true,false,false,true,true,false,false,false,false,false,false,false,true,false,false,false },
                {false,false,false,false,true,false,false,true,false,false,false,true,false,false,true,false },
                {true,false,false,false,false,false,false,false,true,false,true,false,true,false,false,false },
                {false,true,false,false,false,false,false,false,false,true,false,true,false,true,false,false },
                {false,false,true,false,false,true,false,false,false,false,false,false,true,false,true,false },
                {false,false,false,true,false,false,true,false,false,false,true,false,false,true,false,false },
                {true,true,true,true,true,false,false,false,false,false,false,false,false,false,false,false }
            };
            int n = (int)Math.Sqrt((double)matrix.Length);

            Console.WriteLine("Graph is being colored...");
            var graph = new UndirectedGraph(n, matrix);

            var start = DateTime.Now;

            var colored = graph.GetChromaticNumber();

            var time = DateTime.Now - start;
        }

        [TestMethod]
        public void TestMIS() {
            bool[,] matrix = new bool[,]
            {
                {false,false,false,false,false,true,true,false,false,true,false,true,false,false,false,true },
                {false,false,false,false,false,true,true,true,false,false,false,false,true,false,false,true },
                {false,false,false,false,false,false,true,true,true,false,false,false,false,true,false,true },
                {false,false,false,false,false,false,false,true,true,true,false,false,false,false,true,true },
                {false,false,false,false,false,true,false,false,true,true,true,false,false,false,false,true },
                {true,true,false,false,true,false,false,false,false,false,false,false,false,true,false,false },
                {true,true,true,false,false,false,false,false,false,false,false,false,false,false,true,false },
                {false,true,true,true,false,false,false,false,false,false,true,false,false,false,false,false },
                {false,false,true,true,true,false,false,false,false,false,false,true,false,false,false,false },
                {true,false,false,true,true,false,false,false,false,false,false,false,true,false,false,false },
                {false,false,false,false,true,false,false,true,false,false,false,true,false,false,true,false },
                {true,false,false,false,false,false,false,false,true,false,true,false,true,false,false,false },
                {false,true,false,false,false,false,false,false,false,true,false,true,false,true,false,false },
                {false,false,true,false,false,true,false,false,false,false,false,false,true,false,true,false },
                {false,false,false,true,false,false,true,false,false,false,true,false,false,true,false,false },
                {true,true,true,true,true,false,false,false,false,false,false,false,false,false,false,false }
            };
            int n = (int)Math.Sqrt((double)matrix.Length);
            var graph = new UndirectedGraph(n, matrix);

            Console.WriteLine("Graph is being colored...");

            var start = DateTime.Now;

            var mis = graph.MIS();

            var time = DateTime.Now - start;
        }

        [TestMethod]
        public void RemoveVerticesTest() {
            var matrix = new bool[,]
            {
                {false,false,false,false,false,true,true,false,false,true,false,true,false,false,false,true },
                {false,false,false,false,false,true,true,true,false,false,false,false,true,false,false,true },
                {false,false,false,false,false,false,true,true,true,false,false,false,false,true,false,true },
                {false,false,false,false,false,false,false,true,true,true,false,false,false,false,true,true },
                {false,false,false,false,false,true,false,false,true,true,true,false,false,false,false,true },
                {true,true,false,false,true,false,false,false,false,false,false,false,false,true,false,false },
                {true,true,true,false,false,false,false,false,false,false,false,false,false,false,true,false },
                {false,true,true,true,false,false,false,false,false,false,true,false,false,false,false,false },
                {false,false,true,true,true,false,false,false,false,false,false,true,false,false,false,false },
                {true,false,false,true,true,false,false,false,false,false,false,false,true,false,false,false },
                {false,false,false,false,true,false,false,true,false,false,false,true,false,false,true,false },
                {true,false,false,false,false,false,false,false,true,false,true,false,true,false,false,false },
                {false,true,false,false,false,false,false,false,false,true,false,true,false,true,false,false },
                {false,false,true,false,false,true,false,false,false,false,false,false,true,false,true,false },
                {false,false,false,true,false,false,true,false,false,false,true,false,false,true,false,false },
                {true,true,true,true,true,false,false,false,false,false,false,false,false,false,false,false }
            };
            var resultCheck = new bool[,] {
                {false,false,false,false,false,false,false,false,true,false,false },
                {false,false,false,false,false,false,false,false,false,true,false },
                {false,false,false,false,false,true,false,false,false,false,false },
                {false,false,false,false,false,false,true,false,false,false,false },
                {false,false,false,false,false,false,false,true,false,false,false },
                {false,false,true,false,false,false,true,false,false,true,false },
                {false,false,false,true,false,true,false,true,false,false,false },
                {false,false,false,false,true,false,true,false,true,false,false },
                {true,false,false,false,false,false,false,true,false,true,false },
                {false,true,false,false,false,true,false,false,true,false,false },
                {false,false,false,false,false,false,false,false,false,false,false }
            };

            int n = (int)Math.Sqrt((double)matrix.Length);
            int l = (int)Math.Sqrt((double)resultCheck.Length);

            var graph = new UndirectedGraph(n, matrix);
            var vertices = new int[] { 0, 1, 2, 3, 4 };

            var result = graph.RemoveVertices(vertices);
            var check = true;

            for (var i=0; i<l; i++) {
                for (var j = 0; j < l; j++) {
                    check &= result._graph[i, j] == resultCheck[i, j];
                }
            }

            Assert.IsTrue(check);
        }
    }
}
