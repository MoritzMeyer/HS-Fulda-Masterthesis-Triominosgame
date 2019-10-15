using GraphKI.GraphSuite;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GraphKITest.GraphSuite
{
    [TestClass]
    public class GraphGeneratorTest
    {
        [TestMethod]
        public void GraphGenerator_Create_Triomino_Graph_has_to_work()
        {
            HyperGraph triominoGraph = GraphGenerator.CreateTriominoGraph();
            Assert.AreEqual(56, triominoGraph.Edges.Where(e => e.IsThreeSidedEdge()).Count());
            Assert.AreEqual(21, triominoGraph.Edges.Where(e => e.IsTwoSidedEdge()).Count());
        }
    }
}
