using GraphColoring.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphColoring
{
    public class ColorGraph
    {
        private bool[,] Graph {get;set;}
        private int n{ get; set; }
        public ColoredGraph ResultGraph { get; private set; }
        //главное опорное множество (1,..n)
        private List<int> mainSupSet;
        
        //Ds_ij для каждого уровня
        private List<Variants> lvlVariants { get; set; }
        //лучшая раскраска
        private List<List<int>> bestColors { get; set; }

        private List<List<int>> tmpColors { get; set;}

        private int lvl = 0;
        private int curNumOfColors = 0;

        public ColorGraph(bool[,] graph)
        {
            Graph = graph;
            n = (int)Math.Sqrt(graph.Length);
            ResultGraph = null;

            mainSupSet = CreateSupportSet(n);
            lvlVariants = new List<Variants>();

            Build(lvlVariants.Last(), mainSupSet, new List<int>());
            
        }

        private List<int> CreateSupportSet(int n)
        {
            var result = new List<int>();

            for(var i=0; i < n; i++) { result.Add(i); }

            return result;
        }
        

        private void Build(Variants sets, List<int> uni, List<int> curTempSet)
        {
            if (uni.Count != 0)
            {
                if (lvl == 0)
                {
                    var nextSupSet = uni.Except(curTempSet);

                    if (curTempSet.Count == 0)
                    {
                        var variants = Variants.CreateFromMatrix(n, Graph, uni);

                        if (variants.Count != 0)
                        {
                            foreach (var node in variants.SetOfPairs)
                            {
                                
                            }
                        }
                    }
                }
            }
        }
    }
}
