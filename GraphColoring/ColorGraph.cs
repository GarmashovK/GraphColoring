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
        public bool[,] ResultGraph { get; private set; }

        public List<List<int>> ResultColorNodes { get; private set; }

        public int ResultNumOfColors { get; private set; }

        private bool[,] Graph;
        private int N;
        //главное опорное множество (1,..n)
        private List<int> mainSupSet;

        //Ds_ij для каждого уровня
        private List<Variants> lvlVariants;
        //лучшая раскраска
        private List<List<int>> bestColors;

        private List<List<int>> tmpColors;

        private List<List<int>> watchedFirstBlocks;

        //private List<int> curColor { get; set; }
        private int lvl = 0;
        private int maxColors;
        public ColorGraph(bool[,] graph)
        {
            Graph = graph;
            N = (int)Math.Sqrt(graph.Length);
            ResultGraph = null;
            maxColors = N;

            Init();

            if (mainSupSet.Count != 0)
            {
                Build(new List<int>(), new List<int>());

                InitResult();
            }
        }

        private void InitResult()
        {
            ResultColorNodes = bestColors;
            ResultNumOfColors = bestColors.Count;
            var tmp = new List<int>();
            bestColors.ForEach(item => tmp = tmp.Union(item).ToList());

            ResultGraph = new bool[N, N];

            for (var i = 0; i < N; i++)
            {
                for (var j = 0; j < N; j++)
                {
                    ResultGraph[i, j] = Graph[tmp[i], tmp[j]];
                }
            }
        }

        private void Init()
        {
            bestColors = new List<List<int>>();
            tmpColors = new List<List<int>>();
            watchedFirstBlocks = new List<List<int>>();

            mainSupSet = CreateStartSupportSet(N);
            lvlVariants = new List<Variants>();
        }

        private List<int> CreateStartSupportSet(int n)
        {
            var result = new List<int>();

            for (var i = 0; i < n; i++) { result.Add(i); }

            return result;
        }

        private List<int> CreateSupportSet()
        {
            var result = new List<int>(mainSupSet);

            for (var i = 0; i < tmpColors.Count; i++)
            {
                result = result.Except(tmpColors[i]).ToList();
            }

            return result;
        }

        private bool BlockCheckA(int lenOfMax, int uniLen)
        {
            var val = (double)tmpColors.Count + (double)uniLen / lenOfMax;
            return val >= maxColors;
        }

        private bool BlockCheckB(int curLvl, int ro)
        {
            return 2 * curLvl + ro < Math.Round((double)N / maxColors);
        }

        private bool BlockCheckC(int curLvl, int uniLen, int ro)
        {
            return 2 * curLvl + ro == uniLen;
        }
        //построение с ненулевым множеством возможных продолжений
        private void BuildNotNullVariants(Variants variants, List<int> uni, List<int> curTempSet)
        {
            var tmp = variants.SetOfPairs[0].Set.Count;
            var tmpRo = tmp != 0 ? tmp : 1;
            if (curTempSet.Count == 0 && 
                BlockCheckA(tmpRo, uni.Count))
                return;

            for (var i = 0; i < variants.SetOfPairs.Count; i++)
            {
                var node = variants.SetOfPairs[i];

                if (tmpColors.Count == 0 &&
                    BlockCheckB(curTempSet.Count / 2, node.Set.Count)
                    ||
                    tmpColors.Count - 1 == maxColors &&
                    !BlockCheckC(curTempSet.Count / 2, uni.Count, tmpRo))
                    return;

                var nextTempSet = new List<int>(curTempSet);

                nextTempSet.Add(node.Left);
                nextTempSet.Add(node.Right);

                var nextUni = uni.Intersect(node.Set).ToList();

                nextUni.Remove(node.Left);
                nextUni.Remove(node.Right);

                Build(nextUni, nextTempSet);
            }
            lvlVariants.RemoveAt(lvlVariants.Count - 1);
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
            lvlVariants.RemoveAt(lvlVariants.Count - 1);
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
            var lastColor = tmpColors.Last().Count / 2;
            var lastVariants = lvlVariants.Count;

            if (lastVariants % 2 == 1)
                lastColor++;

            for (var i = 1; i <= lastColor; i++)
            {
                var phi = GetPhi(i);
                //удаляем полностью совпадающие ветви
                lvlVariants[lastVariants - i].Sift(phi);
            }
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

            if (curTempSet.Count == 0)
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
                }
                else
                {
                    if (bestColors.Count == 0 || tmpColors.Count < bestColors.Count)
                    {
                        bestColors = new List<List<int>>(tmpColors);
                        maxColors = bestColors.Count;
                        tmpColors.RemoveAt(tmpColors.Count - 1);
                    }
                }
                tmpColors.Remove(curTempSet);
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
            result.SetOfPairs = result.SetOfPairs.OrderByDescending(item => item.Set.Count).ToList();
            //Добавляем в рассмотриваемые
            lvlVariants.Add(result);
        }
    }
}

