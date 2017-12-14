using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using GraphColoring;

namespace GraphColoringTests {
    [TestClass]
    public class UndirectedGraphTests {
        [TestMethod]
        public void TestMISColoring() {
            int n = 10;
            bool[,] matrix = GraphGenerator.GenerateByDensity(n, 0.5);
                       
            var graph = new UndirectedGraph(n, matrix);

            var start = DateTime.Now;

            var colored = graph.GetMISChromaticNumber();            
        }

        [TestMethod]
        public void TestMIS() {
            int n = 35;
            bool[,] matrix = GraphGenerator.GenerateByDensity(n, 0.5);

            var graph = new UndirectedGraph(n, matrix);
            
            var mis = graph.MIS();
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
                    check &= result.AdjecencyMatrix[i, j] == resultCheck[i, j];
                }
            }

            Assert.IsTrue(check);
        }
    }
}
