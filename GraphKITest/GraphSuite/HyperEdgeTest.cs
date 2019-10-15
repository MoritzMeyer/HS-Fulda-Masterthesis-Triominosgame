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
                    Assert.IsFalse(triominioEdges[i].EqualsOnValueBasis(triominioEdges[j]));
                    Assert.IsFalse(triominioEdges[i].EqualsOnVertexBasis(triominioEdges[j]));
                }

                Assert.AreEqual(triominioEdges[i], triominioEdges[i]);
                Assert.IsTrue(triominioEdges[i].EqualsOnValueBasis(triominioEdges[i]));
            }

            Vertex vertex1 = new Vertex("00");
            Vertex vertex2 = Vertex.CreateFromVertex(vertex1);
            Vertex vertex3 = Vertex.CreateFromVertex(vertex1);
            Vertex vertex4 = Vertex.CreateFromVertex(vertex1);

            HyperEdge test1 = new HyperEdge(vertex1, vertex2, vertex3);
            HyperEdge test2 = new HyperEdge(vertex1, vertex2, vertex3);
            HyperEdge test3 = new HyperEdge(vertex1, vertex2, vertex4);

            Assert.AreEqual(test1, test1);
            Assert.AreNotEqual(test1, test2);
            Assert.AreNotEqual(test1, test3);
            Assert.AreNotEqual(test2, test3);
            Assert.IsTrue(test1.EqualsOnVertexBasis(test2));
            Assert.IsTrue(test1.EqualsOnVertexBasis(test3));
            Assert.IsTrue(test2.EqualsOnVertexBasis(test3));

            HyperEdge h1 = new HyperEdge("11", "11", "11");
            HyperEdge h2 = new HyperEdge("01", "11", "10");
            Assert.IsFalse(h1.EqualsOnValueBasis(h2));

            HyperEdge h3 = new HyperEdge("12", "23", "31");
            HyperEdge h4 = new HyperEdge("12", "23", "31");
            Assert.IsTrue(h3.EqualsOnValueBasis(h4));
        }

        [TestMethod]
        public void Contains_Methods_has_to_work()
        {
            Vertex vertex1 = new Vertex("00");

            HyperEdge test2 = new HyperEdge(vertex1, vertex1, vertex1);

            Assert.IsTrue(test2.ContainsOnVertexBasis(vertex1));
            Assert.IsFalse(test2.ContainsOnEdgeBasis(vertex1));
        }

        #region GetNeighborVertices_has_to_work
        /// <summary>
        /// Verifies behaviour of method 'GetNeighborVetices'
        /// </summary>
        [TestMethod]
        public void GetNeighborVertices_has_to_work()
        {
            Vertex v1 = new Vertex("00");
            Vertex v2 = new Vertex("00");
            Vertex v3 = new Vertex("00");

            HyperEdge h1 = new HyperEdge(v1, v2, v3);
            Vertex v11 = h1.GetEdgeVertexInstance(v1);
            Vertex v21 = h1.GetEdgeVertexInstance(v2);
            Vertex v31 = h1.GetEdgeVertexInstance(v3);
            Vertex v41 = h1.GetEdgeVertexInstance(v3);

            Assert.AreNotEqual(v11, v21);
            Assert.AreNotEqual(v11, v31);
            Assert.AreNotEqual(v21, v31);
            Assert.AreEqual(v11, v41);

            IEnumerable<Vertex> neigbors = h1.GetNeighborVertices(v11);
            Assert.AreEqual(2, neigbors.Count());
            Assert.AreEqual(1, neigbors.Where(v => v.EqualsOnVertexBasis(v2)).Count());
            Assert.AreEqual(1, neigbors.Where(v => v.EqualsOnVertexBasis(v3)).Count());
            Assert.AreEqual(1, neigbors.Where(v => v.EqualsOnEdgeBasis(v21)).Count());
            Assert.AreEqual(1, neigbors.Where(v => v.EqualsOnEdgeBasis(v31)).Count());
        }
        #endregion
    }
}
