using GraphColoring.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphColoringConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            int n = 4;

            bool[,] matrix= new bool[,]
            {
                {false,true,false,true },
                {true,false,false,false },
                {false,false,false,true },
                {true,false,true,false }
            };

            var res = Variants.CreateFromMatrix(n, matrix, new int[] { 0, 1, 2, 3 }.ToList());
            var tmp = "Hello";
            if (true)
            {
                var list = new List<string>();
                list.Add(tmp);
            }

            Console.Read();
        }
    }
}
