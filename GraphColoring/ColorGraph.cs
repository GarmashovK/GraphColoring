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

        //private List<int> curColor { get; set; }

        private int lvl = 0;
        private int curNumOfColors = 0;

        public ColorGraph(bool[,] graph)
        {
            Graph = graph;
            n = (int)Math.Sqrt(graph.Length);
            ResultGraph = null;

            bestColors = new List<List<int>>();
            tmpColors = new List<List<int>>();


            mainSupSet = CreateStartSupportSet(n);
            lvlVariants = new List<Variants>();
            
            //curColor = new List<int>();

            lvlVariants.Add(Variants.CreateFromMatrix(n, Graph, mainSupSet));

            if (mainSupSet.Count != 0)
                Build(mainSupSet, new List<int>());            
        }


        private List<int> CreateStartSupportSet(int n)
        {
            var result = new List<int>();

            for(var i=0; i < n; i++) { result.Add(i); }

            return result;
        }
        
        private List<int> CreateSupportSet()
        {
            var result = new List<int>(mainSupSet);

            for(var i=0; i< tmpColors.Count; i++)
            {
                result = result.Except(tmpColors[i]).ToList();
            }

            return null;
        }
        //построение с ненулевым множеством возможных продолжений
        private void BuildNotNullVariants(Variants variants, List<int> uni, List<int> curTempSet)
        {
            for (var i=0; i<variants.SetOfPairs.Count; i++)
            {
                var node = variants.SetOfPairs[i];
                var nextTempSet = new List<int>(curTempSet);

                nextTempSet.Add(node.Left);
                nextTempSet.Add(node.Right);

                var nextUni = uni.Intersect(node.Set).ToList();

                nextUni.Remove(node.Left);
                nextUni.Remove(node.Right);

                Build(nextUni, nextTempSet);
            }
        }

        //построение с нулевым множеством возможных продолжений, при ненулевом опорным множеством
        private void BuildEndByCenter(List<int> uni, List<int> curTempSet)
        {
            for (var i = 0; i < uni.Count; i++)
            {
                var nextTempSet = new List<int>(curTempSet);

                nextTempSet.Add(uni[i]);

                var nextUni = new List<int>(uni);
                nextUni.Remove(uni[i]);

                Build(nextUni, nextTempSet);
            }
        }

        private void Build(List<int> uni, List<int> curTempSet)
        {
            var centrElements = new List<int>();

            if(curTempSet.Count == 0)
            {
                uni = CreateSupportSet();
            }

            if (uni.Count == 0)
            {
                tmpColors.Add(curTempSet);
            }
            else
            {
                var variants = lvlVariants.Last();
                
                if (variants.Count != 0)
                {
                    BuildNotNullVariants(variants, uni, curTempSet);
                }else
                {
                    BuildEndByCenter(uni, curTempSet);
                }

            }
        }
    }
}

