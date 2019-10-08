using GraphKI.GraphSuite;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;

namespace GraphKITest.GraphSuite
{
    [TestClass]
    public class VertexTest
    {
        #region VertexEquals_and_EdgeEquals_has_to_work
        /// <summary>
        /// Verifies Methods EqualsOnVertexBasis and EqualsOnEdgeBasis.
        /// </summary>
        [TestMethod]
        public void VertexEquals_and_EdgeEquals_has_to_work()
        {
            Vertex vertex1 = new Vertex("00");
            Vertex vertex2 = Vertex.CreateFromVertex(vertex1);
            Vertex vertex3 = new Vertex("00");
            Vertex vertex4 = new Vertex("01", vertex1.VertexGuid);

            Assert.IsTrue(vertex1.EqualsOnVertexBasis(vertex2));
            Assert.IsFalse(vertex1.EqualsOnEdgeBasis(vertex2));

            Assert.IsFalse(vertex1.EqualsOnVertexBasis(vertex3));
            Assert.IsFalse(vertex1.EqualsOnEdgeBasis(vertex3));

            Assert.IsFalse(vertex1.EqualsOnVertexBasis(vertex4));
            Assert.IsFalse(vertex1.EqualsOnEdgeBasis(vertex4));

            Assert.IsTrue(vertex1.EqualsOnValueBasis(vertex3));
            Assert.IsFalse(vertex1.EqualsOnValueBasis(vertex4));

            Assert.AreNotEqual(vertex1, vertex2);
            Assert.AreNotEqual(vertex1, vertex3);
            Assert.AreNotEqual(vertex1, vertex4);
            Assert.AreNotEqual(vertex2, vertex3);
            Assert.AreEqual(vertex1, vertex1);

        }
        #endregion
    }
}
