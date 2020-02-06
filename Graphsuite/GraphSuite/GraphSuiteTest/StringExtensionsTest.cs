using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using GraphSuite;
using System.Collections.Generic;
using System.Text;

namespace GraphSuiteTest
{
    [TestClass]
    public class StringExtensionsTest
    {
        [TestMethod]
        public void Reverse_has_to_work()
        {
            string test = "test";
            Assert.AreEqual("tset", test.Reverse());
        }

        [TestMethod]
        public void GetTileValue_has_to_work()
        {
            string test = "1_2_3";
            Assert.AreEqual(6, test.GetTileValue());
        }
    }
}
