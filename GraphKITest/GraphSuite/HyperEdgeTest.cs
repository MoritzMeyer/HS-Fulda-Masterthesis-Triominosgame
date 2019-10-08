using GraphKI.GraphSuite;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GraphKITest.GraphSuite
{
    [TestClass]
    public class HyperEdgeTest
    {
        [TestMethod]
        public void Equals_hast_to_work()
        {
            List<HyperEdge> triominioEdges = GraphGenerator.GetAllTriominoEdges().ToList();
            for (int i = 0; i < triominioEdges.Count; i++)
            {
                for (int j = i + 1; j < triominioEdges.Count; j++)
                {
                    Assert.AreNotEqual(triominioEdges[i], triominioEdges[j]);
                }

                Assert.AreEqual(triominioEdges[i], triominioEdges[i]);
            }

            Vertex vertex1 = new Vertex("00");
            Vertex vertex2 = Vertex.CreateFromVertex(vertex1);
            Vertex vertex3 = Vertex.CreateFromVertex(vertex1);
            Vertex vertex4 = Vertex.CreateFromVertex(vertex1);
            
            HyperEdge test1 = new HyperEdge(vertex1, vertex2, vertex3);
            HyperEdge test2 = new HyperEdge(vertex1, vertex2, vertex3);
            HyperEdge test3 = new HyperEdge(vertex1, vertex2, vertex4);

            Assert.AreEqual(test1, test2);
            Assert.AreNotEqual(test1, test3);
            Assert.AreNotEqual(test2, test3);
            Assert.IsTrue(test1.EqualsOnVertexBasis(test2));
            Assert.IsTrue(test1.EqualsOnVertexBasis(test3));
            Assert.IsTrue(test2.EqualsOnVertexBasis(test3));
        }

        [TestMethod]
        public void Contains_Methods_has_to_work()
        {
            Vertex vertex1 = new Vertex("00");
            Vertex vertex2 = Vertex.CreateFromVertex(vertex1);
            Vertex vertex3 = Vertex.CreateFromVertex(vertex1);
            Vertex vertex4 = Vertex.CreateFromVertex(vertex1);

            HyperEdge test2 = new HyperEdge(vertex1, vertex2, vertex3);

            Assert.IsTrue(test2.ContainsOnVertexBasis(vertex4));
            Assert.IsFalse(test2.ContainsOnEdgeBasis(vertex4));

            Assert.IsTrue(test2.ContainsOnEdgeBasis(vertex1));
            Assert.IsTrue(test2.ContainsOnEdgeBasis(vertex2));
            Assert.IsTrue(test2.ContainsOnEdgeBasis(vertex3));
        }
    }
}
