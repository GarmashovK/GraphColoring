using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using GraphColoring.Models.Bin;
using System.Collections.Generic;
using System.Linq;

namespace GraphColoringTests {
   

    [TestClass]
    public class BinaryTests {
        [TestMethod]
        public void TestUnion() {
            var len = 10;
            var binset1 = new BinarySet(len);
            var binset2 = new BinarySet(len);

            binset1[4] = true;
            binset1[8] = true;

            binset2[2] = true;
            binset2[6] = true;

            var result = binset1.Union(binset2);

            Assert.AreEqual(result[2], binset2[2]);
            Assert.AreEqual(result[4], binset1[4]);
            Assert.AreEqual(result[6], binset2[6]);
            Assert.AreEqual(result[8], binset1[8]);
        }

        [TestMethod]
        public void TestIntersect() {
            var len = 10;
            var binset1 = new BinarySet(len);
            var binset2 = new BinarySet(len);

            binset1[4] = true;
            binset1[8] = true;

            binset2[2] = true;
            binset2[8] = true;

            var result = binset1.Intersect(binset2);
            var tmp = result[6];

            Assert.AreEqual(result[2], false);
            Assert.AreEqual(result[4], false);
            Assert.AreEqual(result[8], true);
        }

        [TestMethod]
        public void TestInvert() {
            var len = 10;
            var binset = new BinarySet(len);

            binset.Invert();

            for (var i = 0; i < len; i++)
                Assert.IsTrue(binset[i]);
        }

        [TestMethod]
        public void TestCopy() {
            var len = 10;
            var binset1 = new BinarySet(len);
            var binset2 = new BinarySet(len);

            binset1[4] = true;
            binset1[6] = true;

            binset2[2] = true;
            binset2[6] = true;

            var list = new List<BinarySet>();

            list.Add(binset1);
            list.Add(binset2);

            var copylist = list.Clone();
            
            Assert.IsTrue(list[0] == copylist[0]);
            Assert.IsTrue(list[1] == copylist[1]);

            binset2[3] = true;

            Assert.IsTrue(list[1] != copylist[1]);

            var copybinset2 = new BinarySet(binset2);
            list.Remove(copybinset2);
        }

        [TestMethod]
        public void TestDelete() {
            var binset = new BinarySet(10);

            binset[4] = true;
            binset[5] = true;

            Assert.IsTrue(binset.Count == 2);
            binset[4] = false;

            Assert.IsTrue(binset.Count == 1);
        }
    }
}
