using GraphColoring;
using GraphColoring.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphColoringConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            //bool[,] matrix = new bool[,]
            //{
            //    {false,true,false,false,true,false,false,false },
            //    {false,false,true,false,false,false,false,false },
            //    {false,false,false,true,true,false,false,false },
            //    {true,false,false,false,false,true,true,false },
            //    {false,false,false,false,false,true,true,true },
            //    {false,false,false,true,true,false,false,false },
            //    {false,false,false,false,true,false,false,false },
            //    {false,true,false,false,false,false,false,false }
            //};

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

            var start = DateTime.Now;
            var colored = new ColorGraph(matrix);
            var time = DateTime.Now - start;

            Console.WriteLine(time.TotalMilliseconds);
            Console.WriteLine(printGraph(colored, n));
            
            Console.Read();
        }

        static string printGraph(ColorGraph graph, int n)
        {
            var result = "";
            result += string.Format("Кол-во цветов: {0}\n", graph.ResultNumOfColors);

            result += "Раскраска: \n";

            for (var i = 0; i < graph.ResultColorNodes.Count; i++)
            {
                result += string.Format("{0} - ", i);

                for (var j = 0; j < graph.ResultColorNodes[i].Count; j++)
                {
                    result += string.Format("{0} ", graph.ResultColorNodes[i][j]);
                }
                result += "\n";
            }

            result += ("Граф: \n");
            for (var i = 0; i < n; i++)
            {
                for(var j=0; j < n; j++)
                {
                    result += string.Format("{0} ", graph.ResultGraph[i, j] ? 1 : 0);
                }
                result += "\n";
            }
            return result;
        }
    }
}
