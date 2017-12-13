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

        private BinarySet mainSupSet;

        private List<BinaryVariants> lvlVariants;

        private List<BinarySet> bestColors;

        private List<BinarySet> tmpColors;

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

        private void Build(BinarySet uni, BinarySet curTempSet) {

            if (curTempSet.Count == 0) {

            }

            if (uni.Count == 0) {

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
        }

        private void BuildEndByCenter(BinarySet uni, BinarySet curTempSet) {
            throw new NotImplementedException();
        }

        private void BuildNotNullVariants(BinaryVariants variants, BinarySet uni, BinarySet curTempSet) {
            
            for(var i=0; i<variants.SetOfPairs.Count; i++) {
                var node = variants.SetOfPairs[i];

                var nextTempSet = new BinarySet(N);

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
