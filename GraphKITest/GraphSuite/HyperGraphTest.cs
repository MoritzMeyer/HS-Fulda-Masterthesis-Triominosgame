using GraphKI.GraphSuite;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace GraphKITest.GraphSuite
{
    [TestClass]
    public class HyperGraphTest
    {
        [TestMethod]
        [Ignore]
        public void HyperGraph_has_to_work()
        {
            string path = Path.Combine(Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.FullName, "TestFiles", "tEdgeGraph.txt");
            //string path = Path.Combine(Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName, "TestFiles", "tEdgePieces.txt");

            HyperGraph hyperGraph = GraphGenerator.LoadHyperGraphFromFile(path);
            //Tuple<string, int>[] degrees = hyperGraph.GetVertexDegrees();
            Tuple<string, int>[] degrees = hyperGraph.GetVertexEulerDegrees();

            string outputPath = @"C:\Users\Moritz\source\repos\GraphSuite\Files";
            string outputFile = "tEdgeEulerDegrees.txt";

            if (File.Exists(Path.Combine(outputPath, outputFile)))
            {
                File.Delete(Path.Combine(outputPath, outputFile));
            }

            File.WriteAllLines(Path.Combine(outputPath, outputFile), degrees.OrderBy(x => x.Item1).Select(d => d.Item1 + " => " + d.Item2));
            //Tuple<int, Stack<Tuple<string, HyperEdge<string>>>>[] cycles = hyperGraph.GetAllSimpleCycles();
        }

        [TestMethod]
        public void HyperGraph_GetAllCycles_has_to_work()
        {
            string path = Path.Combine(Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.FullName, "TestFiles", "tEdgeGraph.txt");
            HyperGraph hyperGraph = GraphGenerator.LoadHyperGraphFromFile(path);

            List<List<Tuple<HyperEdge, Vertex>>> cycles = hyperGraph.GetAllSimpleCycles();
        }

        #region HyperGraph_AddVertex_has_to_work
        /// <summary>
        /// Verifies behaviour of MethodGroup AddVertex
        /// </summary>
        [TestMethod]
        public void HyperGraph_AddVertex_has_to_work()
        {
            HyperGraph hyperGraph = new HyperGraph();
            Vertex v1 = hyperGraph.AddVertex("00");

            Assert.AreEqual(v1, hyperGraph.Vertices.First());
            Assert.IsFalse(hyperGraph.AddVertex(new Vertex("00")));
            Assert.AreEqual(v1, hyperGraph.AddVertex("00"));
        }
        #endregion

        #region HyperGraph_AddEdge_has_to_work
        /// <summary>
        /// Verifies behaviour of MethodGroup AddEdge
        /// </summary>
        [TestMethod]
        public void HyperGraph_AddEdge_has_to_work()
        {
            HyperGraph hyperGraph = new HyperGraph();
            Assert.ThrowsException<ArgumentException>(() => { hyperGraph.AddEdge("00", "00", "00"); }, "This graph does not conatin a vertex with value '00'.");

            Vertex v1 = hyperGraph.AddVertex("00");
            HyperEdge h1 = hyperGraph.AddEdge("00", "00", "00");
            Assert.AreEqual(3, h1.VertexCount());
            Assert.IsTrue(h1.ContainsOnVertexBasis(v1));

            HyperEdge h2 = new HyperEdge("00", "01", "10");
            Assert.ThrowsException<ArgumentException>(() => { hyperGraph.AddEdge(new Vertex("00"), new Vertex("01"), new Vertex("10")); }, $"This graph does not conatin a vertex with value '01'");

            Vertex v2 = hyperGraph.AddVertex("11");
            Vertex v3 = hyperGraph.AddVertex("01");
            Vertex v4 = hyperGraph.AddVertex("10");
            Assert.ThrowsException<ArgumentException>(() => hyperGraph.AddEdge(v2, v2, v2), "Vertices must not be equal.");
            HyperEdge h3 = hyperGraph.AddEdge(v1, v3, v4);

            Assert.IsTrue(h3.ContainsOnVertexBasis(v1));
            Assert.IsTrue(h3.ContainsOnVertexBasis(v3));
            Assert.IsTrue(h3.ContainsOnVertexBasis(v4));

            Assert.IsFalse(h3.ContainsOnEdgeBasis(v1));
            Assert.IsFalse(h3.ContainsOnEdgeBasis(v3));
            Assert.IsFalse(h3.ContainsOnEdgeBasis(v4));

            HyperEdge h4 = new HyperEdge(new Vertex("00"), new Vertex("01"), new Vertex("10"));
            Assert.IsTrue(hyperGraph.HasEdge(h4));
            Assert.ThrowsException<ArgumentException>(() => { hyperGraph.AddEdge(new Vertex("00"), new Vertex("01"), new Vertex("10")); }, $"This Graph has already an edge '-00-01-10->'.");
            Assert.ThrowsException<ArgumentException>(() => { hyperGraph.AddEdge("00", "01", "10"); }, $"This Graph has already an edge '-00-01-10->'.");
        }
        #endregion

        #region HyperGraph_GetAdjacentEdges_has_to_work
        /// <summary>
        /// Verifies Output of GetAdjacentEdges
        /// </summary>
        [TestMethod]
        public void HyperGraph_GetAdjacentEdges_has_to_work()
        {
            HyperGraph hyperGraph = new HyperGraph();
            Vertex v1 = hyperGraph.AddVertex("00");
            Vertex v2 = hyperGraph.AddVertex("01");
            Vertex v3 = hyperGraph.AddVertex("10");

            HyperEdge h1 = hyperGraph.AddEdge("00", "00", "00");
            HyperEdge h2 = hyperGraph.AddEdge("00", "01", "10");
            HyperEdge h3 = hyperGraph.AddEdge("00", "00");
            HyperEdge h4 = hyperGraph.AddEdge("01", "10");

            List<HyperEdge> neighbors = hyperGraph.GetAdjacentEdges(v1).ToList();
            Assert.AreEqual(3, neighbors.Count);
            Assert.IsTrue(neighbors.Contains(h1));
            Assert.IsTrue(neighbors.Contains(h2));
            Assert.IsTrue(neighbors.Contains(h3));

            neighbors = hyperGraph.GetAdjacentEdges(v2).ToList();
            Assert.AreEqual(2, neighbors.Count);
            Assert.IsTrue(neighbors.Contains(h2));
            Assert.IsTrue(neighbors.Contains(h4));

            neighbors = hyperGraph.GetAdjacentEdges(v2, 2).ToList();
            Assert.AreEqual(1, neighbors.Count);
            Assert.IsTrue(neighbors.Contains(h4));

            neighbors = hyperGraph.GetAdjacentEdges(v2, 3).ToList();
            Assert.AreEqual(1, neighbors.Count);
            Assert.IsTrue(neighbors.Contains(h2));
        }
        #endregion      
    }
}
