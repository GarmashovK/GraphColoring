using GraphColoring.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphColoring.ColorGraph
{
    public class OlemskoyColorGraph
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


        private bool IsOver = false;

        //private List<int> curColor { get; set; }
        private int maxColors;
        public OlemskoyColorGraph(bool[,] graph)
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
        }

        private void Init()
        {
            bestColors = new List<List<int>>();
            tmpColors = new List<List<int>>();
            watchedFirstBlocks = new List<List<int>>();

            maxColors = N;
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
            return 2 * (curLvl - 1) + ro < N / maxColors;
        }

        private bool BlockCheckC(int curLvl, int uniLen, int ro)
        {
            return 2 * (curLvl - 1) + ro == uniLen;
        }


        private int GetRo(Variants variants, Pair pair)
        {
            if (variants.Count == 0) return 1;
            int tmp = pair.Set.Count;
            return tmp != 0 ? tmp : 1;
        }

        //построение с ненулевым множеством возможных продолжений
        private void BuildNotNullVariants(Variants variants, List<int> uni, List<int> curTempSet)
        {
            var lvl = curTempSet.Count / 2 + 1;
            var isFirstLvl = curTempSet.Count == 0;
                        
            while (variants.SetOfPairs.Count != 0) {
                var node = variants.SetOfPairs.First();
                var tmpRo = GetRo(variants, node);
                
                if (isFirstLvl &&
                    BlockCheckA(tmpRo, uni.Count)
                    || tmpColors.Count == 0 &&
                    (uni.Count / tmpRo >= maxColors || BlockCheckB(lvl, node.Set.Count))
                    ||
                    tmpColors.Count + 1 == maxColors &&
                    !BlockCheckC(curTempSet.Count / 2, uni.Count, tmpRo)
                    || 
                    isFirstLvl && tmpColors.Count == 0 &&
                    BlockCheckD(tmpRo))
                    return;

                var nextTempSet = new List<int>(curTempSet);

                nextTempSet.Add(node.Left);
                nextTempSet.Add(node.Right);

                var nextUni = uni.Intersect(node.Set).ToList();

                nextUni.Remove(node.Left);
                nextUni.Remove(node.Right);

                Build(nextUni, nextTempSet);
                variants.SetOfPairs.Remove(node);

                if (IsOver) return;
            }
        }

        //построение с нулевым множеством возможных продолжений, 
        //при ненулевом опорным множеством
        private void BuildEndByCenter(List<int> uni, List<int> curTempSet)
        {
            if (tmpColors.Count + uni.Count >= maxColors
                || tmpColors.Count + 1 == maxColors - 1)
                return;

            for (var i = 0; i < uni.Count; i++)
            {
                var nextTempSet = new List<int>(curTempSet);

                nextTempSet.Add(uni[i]);

                var nextUni = new List<int>();

                Build(nextUni, nextTempSet);

                if (IsOver) return;
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
            var lastColorLen = tmpColors.Last().Count / 2;
            var lastVariants = lvlVariants.Count;

            if (lastVariants % 2 == 1)
                lastColorLen++;

            for (var i = 1; i < lastColorLen; i++)
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

        private bool BlockCheckD(int ro)
        {            
            var tmp1 = N / ro;
            var tmp2 = (double)N / ro;
            

            if(tmp1 == tmp2)
            {
                return tmp1 == maxColors;
            }
            else
            {
                return tmp1 == maxColors - 1;
            }
        }
        //построение дерева перебора
        private void Build(List<int> uni, List<int> curTempSet)
        {
            if (IsOver) return;
            var centrElements = new List<int>();

            if (curTempSet.Count == 0)
            {                
                if (tmpColors.Count == 1) {
                    if (checkFirstBlock(tmpColors[0])) {
                        return;
                    } else {
                        watchedFirstBlocks.Add(tmpColors[0]);
                    }
                }
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
                lvlVariants.RemoveAt(lvlVariants.Count - 1);
            }

            if (IsOver) return;
        }

        private bool checkFirstBlock(List<int> list) {
            if (watchedFirstBlocks.Count == 0) return false;

            for (var i=0; i<watchedFirstBlocks.Count; i++) {
                if (watchedFirstBlocks[i].Except(list).Count() == 0)
                    return true;
            }
            return false;
        }

        //создаёт множество возможных продолжений
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

