using GraphColoring;
using System;

namespace GraphColoringConsole {
    class Program {
        static void Main(string[] args) {
            int n = 13;
            bool[,] matrix = GraphGenerator.GenerateByDensity(n, 0.3);

            //bool[,] matrix = new bool[,]
            //{
            //    {false,false,false,false,false,true,true,false,false,true,false,true,false,false,false,true },
            //    {false,false,false,false,false,true,true,true,false,false,false,false,true,false,false,true },
            //    {false,false,false,false,false,false,true,true,true,false,false,false,false,true,false,true },
            //    {false,false,false,false,false,false,false,true,true,true,false,false,false,false,true,true },
            //    {false,false,false,false,false,true,false,false,true,true,true,false,false,false,false,true },
            //    {true,true,false,false,true,false,false,false,false,false,false,false,false,true,false,false },
            //    {true,true,true,false,false,false,false,false,false,false,false,false,false,false,true,false },
            //    {false,true,true,true,false,false,false,false,false,false,true,false,false,false,false,false },
            //    {false,false,true,true,true,false,false,false,false,false,false,true,false,false,false,false },
            //    {true,false,false,true,true,false,false,false,false,false,false,false,true,false,false,false },
            //    {false,false,false,false,true,false,false,true,false,false,false,true,false,false,true,false },
            //    {true,false,false,false,false,false,false,false,true,false,true,false,true,false,false,false },
            //    {false,true,false,false,false,false,false,false,false,true,false,true,false,true,false,false },
            //    {false,false,true,false,false,true,false,false,false,false,false,false,true,false,true,false },
            //    {false,false,false,true,false,false,true,false,false,false,true,false,false,true,false,false },
            //    {true,true,true,true,true,false,false,false,false,false,false,false,false,false,false,false }
            //};
            //int n = (int)Math.Sqrt((double)matrix.Length);
            var graph = new UndirectedGraph(n, matrix);

            Console.WriteLine(graph.PrintGraph(matrix));
            
            Console.WriteLine("MIS alg");

            var start = DateTime.Now;
            //var colored = new OlemskoyColorGraph(matrix);
            var misChromatic = graph.GetMISChromaticNumber();

            var time = DateTime.Now - start;

            Console.WriteLine(graph.OutColoredGraph(misChromatic));
            Console.WriteLine("Time: {0}", time.TotalSeconds);
            Console.WriteLine();

            //------------------------------------
            Console.WriteLine("Olemskoy alg");

            start = DateTime.Now;
            //var colored = new OlemskoyColorGraph(matrix);
            var olemskoyChromatic = graph.GetOlemskoyChromaticNumber();

            time = DateTime.Now - start;
            //Console.WriteLine(printGraph(colored, n));
            Console.WriteLine(graph.OutColoredGraph(olemskoyChromatic));
            Console.WriteLine("Time: {0}", time.TotalSeconds);
            Console.WriteLine();

            Console.Read();
        }

        //static string printGraph(OlemskoyColorGraph graph, int n) {
        //    var result = "";
        //    result += string.Format("Кол-во цветов: {0}\n", graph.ResultNumOfColors);

        //    result += "Раскраска: \n";

        //    for (var i = 0; i < graph.ResultColorNodes.Count; i++) {
        //        result += string.Format("{0} - ", i);

        //        for (var j = 0; j < graph.ResultColorNodes[i].Count; j++) {
        //            result += string.Format("{0} ", graph.ResultColorNodes[i][j]);
        //        }
        //        result += "\n";
        //    }

        //    result += PrintGraph(graph.ResultGraph, n);
        //    return result;
        //}

        //static string PrintGraph(bool[,] graph, int n) {
        //    string result = "";

        //    result = ("Граф: \n");

        //    for (var i = 0; i < n; i++) {
        //        for (var j = 0; j < n; j++) {
        //            result += string.Format("{0} ", graph[i, j] ? 1 : 0);
        //        }
        //        result += "\n";
        //    }

        //    return result;
        //}
    }

}
