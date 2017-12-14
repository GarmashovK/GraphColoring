using GraphColoring.Models.Bin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphColoring.ColorGraph {
    public class BinaryOlemskoyColorGraph {
        bool[,] adjacencyMatrix;
        int N;
        int maxColors;

        int numBlock;
        int numLevel;
        bool IsOver = false;

        BinarySet mainSupSet;

        List<BinaryVariants> lvlVariants;

        List<BinarySet> bestColors;

        List<BinarySet> tmpColors;

        List<BinarySet> lvlColored;

        List<BinarySet> watchedFirstBlocks;

        public BinaryOlemskoyColorGraph(bool[,] graph, int n) {
            adjacencyMatrix = graph;
            N = n;
            maxColors = N;

            numBlock = 1;
        }

        public List<List<int>> Calculate() {
            bestColors = new List<BinarySet>();

            Init();

            Build(mainSupSet, new BinarySet(N));

            return new List<List<int>>();
        }

        private bool ColoringIsOver() {
            var count = 0;
            tmpColors.ForEach(item => count += item.Count);

            return count == N;
        }

        private BinarySet GetPhi(int s) {                        
            return tmpColors.Last().Except(lvlColored[s]);
        }

        private bool BlockCheckA(int lenOfMax, int uniLen) {
            var val = tmpColors.Count + (double)uniLen / lenOfMax;
            return val >= maxColors;
        }

        private bool BlockCheckB(int curLvl, int ro) {
            return 2 * (curLvl - 1) + ro < Math.Round((double)N / maxColors);
        }

        private bool BlockCheckC(int curLvl, int uniLen, int ro) {
            return 2 * (curLvl - 1) + ro != uniLen;
        }

        private bool BlockCheckD(int ro) {
            if (tmpColors[0].Count < 2) return false;
            
            var tmp1 = N / ro;
            var tmp2 = (double)N / ro;


            if (tmp1 == tmp2) {
                return tmp1 == maxColors;
            } else {
                return tmp1 == maxColors - 1;
            }
        }

        private int GetRo(BinaryVariants variants, BinaryPair pair) {
            return variants.Count == 0 ? 1 : pair.Set.Count;
        }

        private void Build(BinarySet uni, BinarySet curTempSet) {

            if (curTempSet.Count == 0) {
                uni = CreateSupportSet();
            }
            
            lvlColored.Add(curTempSet);

            if (uni.Count == 0) {
                tmpColors.Add(curTempSet);

                Thinning();

                if (!ColoringIsOver()) {
                    Build(CreateSupportSet(), new BinarySet(N));
                } else {
                    if (bestColors.Count == 0 || tmpColors.Count < bestColors.Count) {
                        bestColors = tmpColors.Clone();
                        maxColors = bestColors.Count;
                        tmpColors.RemoveAt(tmpColors.Count - 1);
                    }
                }
                
                tmpColors.Remove(curTempSet);
            } else {
                CreateVariants(uni, curTempSet.Count == 0);

                var variants = lvlVariants.Last();

                if (variants.Count != 0) {
                    BuildNotNullVariants(variants, uni, curTempSet);
                } else {
                    BuildEndByCenter(uni, curTempSet);
                }

                lvlVariants.RemoveAt(lvlVariants.Count - 1);
            }
            lvlColored.RemoveAt(lvlColored.Count - 1);
        }

        private void BuildEndByCenter(BinarySet uni, BinarySet curTempSet) {
            var i = 0;
            while (uni.Count != 0) {
                if (!uni[i]) {
                    i++;
                    continue;
                }

                var nextTempSet = new BinarySet(curTempSet);

                nextTempSet[i] = true;

                var nextUni = new BinarySet(N);

                Build(nextUni, nextTempSet);
                uni[i] = false;
            }
        }

        private void BuildNotNullVariants(BinaryVariants variants, BinarySet uni, BinarySet curTempSet) {
            var lvl = curTempSet.Count / 2 + 1;
            var isFirstLvl = curTempSet.Count == 0;            

            if (curTempSet.Count == 0 
                && variants.Count == 0 &&
                uni.Count / GetRo(variants, null) >= maxColors)
                return;

            while (variants.SetOfPairs.Count != 0) {
                var node = variants.SetOfPairs.First();

                var tmpRo = GetRo(variants, node);                  

                if (isFirstLvl &&
                    BlockCheckA(tmpRo, uni.Count)
                    ||
                    tmpColors.Count == 0 &&
                    BlockCheckB(lvl, node.Set.Count)
                    ||
                    tmpColors.Count + 2 == maxColors &&
                    BlockCheckC(curTempSet.Count / 2, uni.Count, tmpRo)
                    || isFirstLvl && tmpColors.Count == 0 &&
                    BlockCheckD(tmpRo))
                    return;

                var nextTempSet = new BinarySet(curTempSet);
                nextTempSet[node.Left] = true;
                nextTempSet[node.Right] = true;

                var nextUni = uni.Intersect(node.Set);
                nextUni[node.Left] = false;
                nextUni[node.Right] = false;

                Build(nextUni, nextTempSet);

                variants.SetOfPairs.Remove(node);
            }
        }

        //прореживание
        private void Thinning() {
            var tmp = tmpColors.Last().Count;
            var lastColorLen = tmp / 2 - tmp % 2 != 0 ? 1 : 0;
            var lastVariants = lvlVariants.Count;

            if (lastVariants % 2 == 1)
                lastColorLen++;

            for (var i = lastColorLen; i >= 0; i--) {
                var phi = GetPhi(i);
                //удаляем полностью совпадающие ветви
                lvlVariants[lastVariants - i].Sift(phi);
            }
            if (tmpColors.Count == 1) {
                watchedFirstBlocks.Add(tmpColors[0]);
            }
        }

        private BinarySet CreateSupportSet() {
            var result = new BinarySet(mainSupSet);

            for (var i = 0; i < tmpColors.Count; i++) {
                result = result.Except(tmpColors[i]);
            }

            return result;
        }

        private void Init() {
            mainSupSet = new BinarySet(N);
            mainSupSet.Invert();

            lvlColored = new List<BinarySet>();
            watchedFirstBlocks = new List<BinarySet>();

            CreateVariants(mainSupSet, false);            
        }

        private void CreateVariants(BinarySet uni, bool newColor) {
            BinaryVariants result;

            if (!newColor) {//Если не строится новый цвет то просеиваем из предыдущего
                result = lvlVariants.Last().Sieve(uni);
            } else {//Если новый то тут 2 варианта: либо ещё не строился ни один цвет,
                //либо уже был построен как минимум один
                result = lvlVariants.Count != 0 ?
                    lvlVariants.First().Sieve(uni) :
                    new BinaryVariants(N, adjacencyMatrix);
            }
            result.SetOfPairs = result.SetOfPairs.OrderByDescending(item => item.Set.Count).ToList();
            //Добавляем в рассмотриваемые
            lvlVariants.Add(result);
        }
    }
}
