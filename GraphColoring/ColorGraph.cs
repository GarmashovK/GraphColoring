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
        private int N{ get; set; }
        public ColoredGraph ResultGraph { get; private set; }
        //главное опорное множество (1,..n)
        private List<int> mainSupSet;
        
        //Ds_ij для каждого уровня
        private List<Variants> lvlVariants { get; set; }
        //лучшая раскраска
        private List<List<int>> bestColors { get; set; }

        private List<List<int>> tmpColors { get; set;}

        private List<List<int>> watchedFirstBlocks { get; set; }

        //private List<int> curColor { get; set; }
        private int lvl = 0;
        private int maxColors;
        public ColorGraph(bool[,] graph)
        {
            Graph = graph;
            N = (int)Math.Sqrt(graph.Length);
            ResultGraph = null;
            maxColors = N;

            bestColors = new List<List<int>>();
            tmpColors = new List<List<int>>();
            watchedFirstBlocks = new List<List<int>>();

            mainSupSet = CreateStartSupportSet(N);
            lvlVariants = new List<Variants>();
            
            //curColor = new List<int>();
            
            lvl++;

            if (mainSupSet.Count != 0)
                Build(null, new List<int>());            
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

        private bool BlockCheckA(int lenOfMax, int uniLen)
        {
            return (double)tmpColors.Count + (double)uniLen / lenOfMax >= maxColors;
        }

        private bool BlockCheckB(int ro)
        {
            return tmpColors.Count * 2 + ro < Math.Round((double)N / maxColors);
        }
        //построение с ненулевым множеством возможных продолжений
        private void BuildNotNullVariants(Variants variants, List<int> uni, List<int> curTempSet)
        {
            var tmp = variants.SetOfPairs[0].Set.Count;
            if (BlockCheckA(tmp != 0 ? tmp : 1, uni.Count))
                return;

            for (var i=0; i<variants.SetOfPairs.Count; i++)
            {
                var node = variants.SetOfPairs[i];

                if (BlockCheckB(node.Set.Count))
                    return;

                var nextTempSet = new List<int>(curTempSet);

                nextTempSet.Add(node.Left);
                nextTempSet.Add(node.Right);

                var nextUni = uni.Intersect(node.Set).ToList();

                nextUni.Remove(node.Left);
                nextUni.Remove(node.Right);

                Build(nextUni, nextTempSet);
            }
        }

        //построение с нулевым множеством возможных продолжений, 
        //при ненулевом опорным множеством
        private void BuildEndByCenter(List<int> uni, List<int> curTempSet)
        {
            for (var i = 0; i < uni.Count; i++)
            {
                var nextTempSet = new List<int>(curTempSet);

                nextTempSet.Add(uni[i]);

                var nextUni = new List<int>();

                Build(nextUni, nextTempSet);
            }
        }
        
        //Phi_js для прорежиания
        private List<int> GetPhi(int s)
        {
            var result = new List<int>(tmpColors.Last());
            var numOfIters = result.Count - s - 1;

            if (result.Count == s - 1)
            {
                for (var i = 0; i < numOfIters; i++)
                {
                    result.RemoveRange(0, 2);
                }

                if (result.Count % 2 != 0)
                {
                    result.RemoveAt(0);
                }
            }
            return result;
        }
        
        //прореживание
        private void Thinning()
        {
            var lastColor = tmpColors.Last();
            var lastVariants = lvlVariants.Count;
            
            for (var i= 1; i < lastColor.Count; i++)
            {
                var phi = GetPhi(i);
                //удаляем полностью совпадающие ветви
                lvlVariants[lastVariants - i].Sift(phi);
            }
        }

        private void BlockCheckC(List<int> uni)
        {
            var lastColor = tmpColors.Count;


        }

        private bool ColoringIsOver()
        {
            var count = 0;
            tmpColors.ForEach(item => count += item.Count);

            return count == N;
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

                Thinning();

                if (!ColoringIsOver())
                {
                    Build(CreateSupportSet(), new List<int>());
                }else
                {
                    if (bestColors.Count != 0)
                    {
                        if (tmpColors.Count < bestColors.Count)
                        {
                            bestColors = new List<List<int>>(tmpColors);
                        }
                    }
                }
            }
            else
            {
                CreateVariants(uni, curTempSet.Count == 0);

                var variants = lvlVariants.Last();
                
                if (variants.Count != 0)
                {
                    BuildNotNullVariants(variants, uni, curTempSet);
                }
                else
                {
                    BuildEndByCenter(uni, curTempSet);
                }

            }
        }

        private void CreateVariants(List<int> uni, bool newColor)
        {
            Variants result;
            
            if (!newColor)
            {//Если не строится новый цвет то просеиваем из предыдущего
                result = lvlVariants.Last().Sieve(uni);
            }
            else
            {//Если новый то тут 2 варианта: либо ещё не строился ни один цвет,
                //либо уже был построен как минимум один
                result = lvlVariants.Count != 0 ?
                    lvlVariants.First().Sieve(uni) :
                    Variants.CreateFromMatrix(N, Graph, mainSupSet);
            }
            //Добавляем в рассмотриваемые
            lvlVariants.Add(result);
        }
    }
}

