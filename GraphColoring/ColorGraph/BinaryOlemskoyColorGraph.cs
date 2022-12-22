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
            Init();

            Build(mainSupSet, new BinarySet(N));
            
            return GetResult();
        }

        private List<List<int>> GetResult() {
            var result = new List<List<int>>();

            for(var i=0; i<bestColors.Count; i++) {
                var tmp = new List<int>();
                for(var j=0; j<N; j++) {
                    if (bestColors[i][j])
                        tmp.Add(j);
                }
                result.Add(tmp);
            }
            return result;
        }

        private bool ColoringIsOver() {
            var count = 0;
            tmpColors.ForEach(item => count += item.Count);

            return count == N;
        }

        private BinarySet GetPhi(int s) {                        
            return tmpColors.Last()
                .Except(lvlColored[lvlColored.Count - s - 1]);
        }

        private bool BlockCheckA(int lenOfMax, int uniLen) {
            var val = tmpColors.Count + (double)uniLen / lenOfMax;
            return val >= maxColors;
        }

        private bool BlockCheckB(int curLvl, int ro) {
            return 2 * (curLvl - 1) + ro < N / maxColors;
        }

        private bool BlockCheckC(int curLvl, int uniLen, int ro) {
            return 2 * (curLvl - 1) + ro != uniLen;
        }

        private bool BlockCheckD(int ro) {            
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
                if (tmpColors.Count == 1) {
                    if (checkFirstBlock(curTempSet)) {
                        return;
                    } else {
                        watchedFirstBlocks.Add(curTempSet);
                    }
                }

                tmpColors.Add(curTempSet);

                Thinning();

                if (!ColoringIsOver()) {
                    Build(CreateSupportSet(), new BinarySet(N));
                } else {
                    if (bestColors.Count == 0 || tmpColors.Count < bestColors.Count) {
                        bestColors = tmpColors.Clone();
                        maxColors = bestColors.Count;
                    }
                }

                tmpColors.RemoveAt(tmpColors.Count - 1);
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

        private bool checkFirstBlock(BinarySet binarySet) {
            if (watchedFirstBlocks.Count == 0) return false;

            for (var i=0; i< watchedFirstBlocks.Count; i++) {
                if (watchedFirstBlocks[i] == binarySet)
                    return true;
            }

            return false;
        }

        private void BuildEndByCenter(BinarySet uni, BinarySet curTempSet) {
            if (tmpColors.Count + uni.Count >= maxColors
                || tmpColors.Count + 1 == maxColors - 1)
                return;

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
            
            while (variants.SetOfPairs.Count != 0) {
                var node = variants.SetOfPairs.First();

                var tmpRo = GetRo(variants, node);                  

                if (checkNextStep(isFirstLvl,tmpRo,uni,node,lvl))
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

        private bool checkNextStep
            (bool isFirstLvl, int tmpRo,
            BinarySet uni, BinaryPair node, int lvl) {
            return isFirstLvl &&
                    BlockCheckA(tmpRo, uni.Count)
                    || tmpColors.Count == 0 &&
                    (uni.Count / tmpRo >= maxColors || BlockCheckB(lvl, node.Set.Count))
                    ||
                    lvl + 1 == maxColors &&
                    BlockCheckC(lvl, uni.Count, tmpRo)
                    ||
                    isFirstLvl && tmpColors.Count == 0 &&
                    BlockCheckD(tmpRo);
        }

        //прореживание
        private void Thinning() {
            var count = tmpColors.Last().Count;
            var tmp = count % 2 == 1 ? 1 : 0;
            var lastColorLen = count / 2 + tmp;
            var lastVariants = lvlVariants.Count;
            
            for (var i = 1; i <= lastColorLen; i++) {
                var phi = GetPhi(i);
                //удаляем полностью совпадающие ветви
                lvlVariants[lastVariants - i].Sift(phi);
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
            tmpColors = new List<BinarySet>();
            bestColors = new List<BinarySet>();

            mainSupSet = new BinarySet(N);
            mainSupSet.Invert();

            lvlColored = new List<BinarySet>();
            watchedFirstBlocks = new List<BinarySet>();
            lvlVariants = new List<BinaryVariants>();
      
        }

        private void CreateVariants(BinarySet uni, bool newColor) {
            BinaryVariants result;

            if (!newColor) {
                //Если не строится новый цвет то просеиваем из предыдущего
                result = lvlVariants.Last().Sieve(uni);
            } else {
                //Если новый то тут 2 варианта: либо ещё не строился ни один цвет,
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
