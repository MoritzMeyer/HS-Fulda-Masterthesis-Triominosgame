using GraphKI.GameManagement;
using GraphKI.GraphSuite;
using GraphKI.Extensions;
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
        //[Ignore]
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

        #region HyperGraph_IsPartOfHexagon_has_to_work
        /// <summary>
        /// Verifies functionality of method IsPartOfHexagon.
        /// </summary>
        [TestMethod]
        public void HyperGraph_IsPartOfHexagon_has_to_work()
        {
            List<Vertex> vertices = new List<Vertex>();
            List<HyperEdge> twoSidedEdges = new List<HyperEdge>();
            for (int i = 0; i < 6; i++)
            {
                for (int j = i; j < 6; j++)
                {
                    vertices.Add(new Vertex(i + "" + j));
                    if (i != j)
                    {
                        vertices.Add(new Vertex(j + "" + i));
                    }

                    twoSidedEdges.Add(new HyperEdge(i + "" + j, j + "" + i));
                }
            }

            HyperGraph hyperGraph = new HyperGraph(vertices);
            foreach(HyperEdge twoSidedEdge in twoSidedEdges)
            {
                hyperGraph.AddEdge(twoSidedEdge);
            }

            HyperEdge edge0 = new HyperEdge("00", "00", "00", TileOrientation.Straight);
            HyperEdge edge1 = new HyperEdge("00", "02", "20", TileOrientation.Flipped);
            HyperEdge edge2 = new HyperEdge("02", "22", "20", TileOrientation.DoubleTiltLeft);
            HyperEdge edge3 = new HyperEdge("01", "12", "20", TileOrientation.Flipped);
            HyperEdge edge4 = new HyperEdge("01", "11", "10", TileOrientation.DoubleTiltRight);
            HyperEdge edge5 = new HyperEdge("00", "01", "10", TileOrientation.TiltRight);

            List<Hexagon> hexagons = new List<Hexagon>();

            HyperEdge addedEdge0 = hyperGraph.AddEdge(edge0);
            Assert.IsFalse(hyperGraph.IsPartOfHexagon(edge0, out hexagons));
            Assert.AreEqual(0, hexagons.Count);

            Assert.IsTrue(hyperGraph.AddEdgeWithDirectNeighbor(edge1, edge0, out HyperEdge addedEdge1));
            Assert.IsFalse(hyperGraph.IsPartOfHexagon(edge1, out hexagons));
            Assert.AreEqual(0, hexagons.Count);

            Assert.IsTrue(hyperGraph.AddEdgeWithDirectNeighbor(edge2, edge1, out HyperEdge addedEdge2));
            Assert.IsFalse(hyperGraph.IsPartOfHexagon(edge2, out hexagons));
            Assert.AreEqual(0, hexagons.Count);

            Assert.IsTrue(hyperGraph.AddEdgeWithDirectNeighbor(edge3, edge2, out HyperEdge addedEdge3));
            Assert.IsFalse(hyperGraph.IsPartOfHexagon(edge3, out hexagons));
            Assert.AreEqual(0, hexagons.Count);

            Assert.IsTrue(hyperGraph.AddEdgeWithDirectNeighbor(edge4, edge3, out HyperEdge addedEdge4));
            Assert.IsFalse(hyperGraph.IsPartOfHexagon(edge4, out hexagons));
            Assert.AreEqual(0, hexagons.Count);

            Assert.IsTrue(hyperGraph.AddEdgeWithDirectNeighbor(edge5, edge4, out HyperEdge addedEdge5));
            addedEdge5.AddDirectNeighbor(addedEdge0);
            Assert.IsTrue(hyperGraph.IsPartOfHexagon(edge5, out hexagons));
            Assert.AreEqual(1, hexagons.Count);

            //-----------------------------------------------------------------------------------
            hyperGraph = new HyperGraph(vertices);
            foreach (HyperEdge twoSidedEdge in twoSidedEdges)
            {
                hyperGraph.AddEdge(twoSidedEdge);
            }

            addedEdge1 = hyperGraph.AddEdge(edge1);
            Assert.IsFalse(hyperGraph.IsPartOfHexagon(edge1, out hexagons));
            Assert.AreEqual(0, hexagons.Count);

            Assert.IsTrue(hyperGraph.AddEdgeWithDirectNeighbor(edge2, edge1, out addedEdge2));
            Assert.IsFalse(hyperGraph.IsPartOfHexagon(edge2, out hexagons));
            Assert.AreEqual(0, hexagons.Count);

            Assert.IsTrue(hyperGraph.AddEdgeWithDirectNeighbor(edge3, edge2, out addedEdge3));
            Assert.IsFalse(hyperGraph.IsPartOfHexagon(edge3, out hexagons));
            Assert.AreEqual(0, hexagons.Count);

            Assert.IsTrue(hyperGraph.AddEdgeWithDirectNeighbor(edge4, edge3, out addedEdge4));
            Assert.IsFalse(hyperGraph.IsPartOfHexagon(edge4, out hexagons));
            Assert.AreEqual(0, hexagons.Count);

            Assert.IsTrue(hyperGraph.AddEdgeWithDirectNeighbor(edge5, edge4, out addedEdge5));
            Assert.IsFalse(hyperGraph.IsPartOfHexagon(edge5, out hexagons));
            Assert.AreEqual(0, hexagons.Count);

            Assert.IsTrue(hyperGraph.AddEdgeWithDirectNeighbor(edge0, edge5, out addedEdge0));
            addedEdge0.AddDirectNeighbor(addedEdge1);
            Assert.IsTrue(hyperGraph.IsPartOfHexagon(edge0, out hexagons));
            Assert.AreEqual(1, hexagons.Count);

            //-----------------------------------------------------------------------------------
            hyperGraph = new HyperGraph(vertices);
            foreach (HyperEdge twoSidedEdge in twoSidedEdges)
            {
                hyperGraph.AddEdge(twoSidedEdge);
            }

            addedEdge2 = hyperGraph.AddEdge(edge2);
            Assert.IsFalse(hyperGraph.IsPartOfHexagon(edge2, out hexagons));
            Assert.AreEqual(0, hexagons.Count);

            Assert.IsTrue(hyperGraph.AddEdgeWithDirectNeighbor(edge3, edge2, out addedEdge3));
            Assert.IsFalse(hyperGraph.IsPartOfHexagon(edge3, out hexagons));
            Assert.AreEqual(0, hexagons.Count);

            Assert.IsTrue(hyperGraph.AddEdgeWithDirectNeighbor(edge4, edge3, out addedEdge4));
            Assert.IsFalse(hyperGraph.IsPartOfHexagon(edge4, out hexagons));
            Assert.AreEqual(0, hexagons.Count);

            Assert.IsTrue(hyperGraph.AddEdgeWithDirectNeighbor(edge5, edge4, out addedEdge5));
            Assert.IsFalse(hyperGraph.IsPartOfHexagon(edge5, out hexagons));
            Assert.AreEqual(0, hexagons.Count);

            Assert.IsTrue(hyperGraph.AddEdgeWithDirectNeighbor(edge0, edge5, out addedEdge0));
            Assert.IsFalse(hyperGraph.IsPartOfHexagon(edge0, out hexagons));
            Assert.AreEqual(0, hexagons.Count);

            Assert.IsTrue(hyperGraph.AddEdgeWithDirectNeighbor(edge1, edge0, out addedEdge1));
            addedEdge1.AddDirectNeighbor(addedEdge2);
            Assert.IsTrue(hyperGraph.IsPartOfHexagon(edge1, out hexagons));
            Assert.AreEqual(1, hexagons.Count);

            //-----------------------------------------------------------------------------------
            hyperGraph = new HyperGraph(vertices);
            foreach (HyperEdge twoSidedEdge in twoSidedEdges)
            {
                hyperGraph.AddEdge(twoSidedEdge);
            }

            addedEdge3 = hyperGraph.AddEdge(edge3);
            Assert.IsFalse(hyperGraph.IsPartOfHexagon(edge3, out hexagons));
            Assert.AreEqual(0, hexagons.Count);

            Assert.IsTrue(hyperGraph.AddEdgeWithDirectNeighbor(edge4, edge3, out addedEdge4));
            Assert.IsFalse(hyperGraph.IsPartOfHexagon(edge4, out hexagons));
            Assert.AreEqual(0, hexagons.Count);

            Assert.IsTrue(hyperGraph.AddEdgeWithDirectNeighbor(edge5, edge4, out addedEdge5));
            Assert.IsFalse(hyperGraph.IsPartOfHexagon(edge5, out hexagons));
            Assert.AreEqual(0, hexagons.Count);

            Assert.IsTrue(hyperGraph.AddEdgeWithDirectNeighbor(edge0, edge5, out addedEdge0));
            Assert.IsFalse(hyperGraph.IsPartOfHexagon(edge0, out hexagons));
            Assert.AreEqual(0, hexagons.Count);

            Assert.IsTrue(hyperGraph.AddEdgeWithDirectNeighbor(edge1, edge0, out addedEdge1));
            Assert.IsFalse(hyperGraph.IsPartOfHexagon(edge1, out hexagons));
            Assert.AreEqual(0, hexagons.Count);

            Assert.IsTrue(hyperGraph.AddEdgeWithDirectNeighbor(edge2, edge1, out addedEdge2));
            addedEdge2.AddDirectNeighbor(addedEdge3);
            Assert.IsTrue(hyperGraph.IsPartOfHexagon(edge2, out hexagons));
            Assert.AreEqual(1, hexagons.Count);

            //-----------------------------------------------------------------------------------
            hyperGraph = new HyperGraph(vertices);
            foreach (HyperEdge twoSidedEdge in twoSidedEdges)
            {
                hyperGraph.AddEdge(twoSidedEdge);
            }

            addedEdge4 = hyperGraph.AddEdge(edge4);
            Assert.IsFalse(hyperGraph.IsPartOfHexagon(edge4, out hexagons));
            Assert.AreEqual(0, hexagons.Count);

            Assert.IsTrue(hyperGraph.AddEdgeWithDirectNeighbor(edge5, edge4, out addedEdge5));
            Assert.IsFalse(hyperGraph.IsPartOfHexagon(edge5, out hexagons));
            Assert.AreEqual(0, hexagons.Count);

            Assert.IsTrue(hyperGraph.AddEdgeWithDirectNeighbor(edge0, edge5, out addedEdge0));
            Assert.IsFalse(hyperGraph.IsPartOfHexagon(edge0, out hexagons));
            Assert.AreEqual(0, hexagons.Count);

            Assert.IsTrue(hyperGraph.AddEdgeWithDirectNeighbor(edge1, edge0, out addedEdge1));
            Assert.IsFalse(hyperGraph.IsPartOfHexagon(edge1, out hexagons));
            Assert.AreEqual(0, hexagons.Count);

            Assert.IsTrue(hyperGraph.AddEdgeWithDirectNeighbor(edge2, edge1, out addedEdge2));
            Assert.IsFalse(hyperGraph.IsPartOfHexagon(edge2, out hexagons));
            Assert.AreEqual(0, hexagons.Count);

            Assert.IsTrue(hyperGraph.AddEdgeWithDirectNeighbor(edge3, edge2, out addedEdge3));
            addedEdge3.AddDirectNeighbor(addedEdge4);
            Assert.IsTrue(hyperGraph.IsPartOfHexagon(edge3, out hexagons));
            Assert.AreEqual(1, hexagons.Count);

            //-----------------------------------------------------------------------------------
            hyperGraph = new HyperGraph(vertices);
            foreach (HyperEdge twoSidedEdge in twoSidedEdges)
            {
                hyperGraph.AddEdge(twoSidedEdge);
            }

            addedEdge5 = hyperGraph.AddEdge(edge5);
            Assert.IsFalse(hyperGraph.IsPartOfHexagon(edge5, out hexagons));
            Assert.AreEqual(0, hexagons.Count);

            Assert.IsTrue(hyperGraph.AddEdgeWithDirectNeighbor(edge0, edge5, out addedEdge0));
            Assert.IsFalse(hyperGraph.IsPartOfHexagon(edge0, out hexagons));
            Assert.AreEqual(0, hexagons.Count);

            Assert.IsTrue(hyperGraph.AddEdgeWithDirectNeighbor(edge1, edge0, out addedEdge1));
            Assert.IsFalse(hyperGraph.IsPartOfHexagon(edge1, out hexagons));
            Assert.AreEqual(0, hexagons.Count);

            Assert.IsTrue(hyperGraph.AddEdgeWithDirectNeighbor(edge2, edge1, out addedEdge2));
            Assert.IsFalse(hyperGraph.IsPartOfHexagon(edge2, out hexagons));
            Assert.AreEqual(0, hexagons.Count);

            Assert.IsTrue(hyperGraph.AddEdgeWithDirectNeighbor(edge3, edge2, out addedEdge3));
            Assert.IsFalse(hyperGraph.IsPartOfHexagon(edge3, out hexagons));
            Assert.AreEqual(0, hexagons.Count);

            Assert.IsTrue(hyperGraph.AddEdgeWithDirectNeighbor(edge4, edge3, out addedEdge4));
            addedEdge4.AddDirectNeighbor(addedEdge5);
            Assert.IsTrue(hyperGraph.IsPartOfHexagon(edge4, out hexagons));
            Assert.AreEqual(1, hexagons.Count);

            //----------------------------------------------------------------------------------
            HyperEdge edge6 = new HyperEdge("02", "24", "40", TileOrientation.DoubleTiltLeft);
            HyperEdge edge7 = new HyperEdge("04", "45", "50", TileOrientation.TiltLeft);
            HyperEdge edge8 = new HyperEdge("05", "55", "50", TileOrientation.Straight);
            HyperEdge edge9 = new HyperEdge("00", "05", "50", TileOrientation.TiltLeft);

            hyperGraph = new HyperGraph(vertices);
            foreach (HyperEdge twoSidedEdge in twoSidedEdges)
            {
                hyperGraph.AddEdge(twoSidedEdge);
            }

            addedEdge1 = hyperGraph.AddEdge(edge1);
            Assert.IsFalse(hyperGraph.IsPartOfHexagon(edge1, out hexagons));
            Assert.AreEqual(0, hexagons.Count);

            Assert.IsTrue(hyperGraph.AddEdgeWithDirectNeighbor(edge2, edge1, out addedEdge2));
            Assert.IsFalse(hyperGraph.IsPartOfHexagon(edge2, out hexagons));
            Assert.AreEqual(0, hexagons.Count);

            Assert.IsTrue(hyperGraph.AddEdgeWithDirectNeighbor(edge3, edge2, out addedEdge3));
            Assert.IsFalse(hyperGraph.IsPartOfHexagon(edge3, out hexagons));
            Assert.AreEqual(0, hexagons.Count);

            Assert.IsTrue(hyperGraph.AddEdgeWithDirectNeighbor(edge4, edge3, out addedEdge4));
            Assert.IsFalse(hyperGraph.IsPartOfHexagon(edge4, out hexagons));
            Assert.AreEqual(0, hexagons.Count);

            Assert.IsTrue(hyperGraph.AddEdgeWithDirectNeighbor(edge5, edge4, out addedEdge5));
            Assert.IsFalse(hyperGraph.IsPartOfHexagon(edge5, out hexagons));
            Assert.AreEqual(0, hexagons.Count);

            Assert.IsTrue(hyperGraph.AddEdgeWithDirectNeighbor(edge6, edge1, out HyperEdge addedEdge6));
            Assert.IsFalse(hyperGraph.IsPartOfHexagon(edge6, out hexagons));
            Assert.AreEqual(0, hexagons.Count);

            Assert.IsTrue(hyperGraph.AddEdgeWithDirectNeighbor(edge7, edge6, out HyperEdge addedEdge7));
            Assert.IsFalse(hyperGraph.IsPartOfHexagon(edge7, out hexagons));
            Assert.AreEqual(0, hexagons.Count);

            Assert.IsTrue(hyperGraph.AddEdgeWithDirectNeighbor(edge8, edge7, out HyperEdge addedEdge8));
            Assert.IsFalse(hyperGraph.IsPartOfHexagon(edge8, out hexagons));
            Assert.AreEqual(0, hexagons.Count);

            Assert.IsTrue(hyperGraph.AddEdgeWithDirectNeighbor(edge9, edge8, out HyperEdge addedEdge9));
            Assert.IsFalse(hyperGraph.IsPartOfHexagon(edge9, out hexagons));
            Assert.AreEqual(0, hexagons.Count);

            Assert.IsTrue(hyperGraph.AddEdgeWithDirectNeighbor(edge0, edge5, out addedEdge0));
            addedEdge0.AddDirectNeighbor(addedEdge1);
            addedEdge0.AddDirectNeighbor(addedEdge9);
            Assert.IsTrue(hyperGraph.IsPartOfHexagon(edge0, out hexagons));
            Assert.AreEqual(2, hexagons.Count);
        }
        #endregion

        [TestMethod]
        public void GetTileSideStructure()
        {
            string path = Path.Combine(Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.FullName, "TestFiles", "tEdgePieces.txt");
            //string path = Path.Combine(Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName, "TestFiles", "tEdgePieces.txt");

            HyperGraph hyperGraph = GraphGenerator.LoadHyperGraphFromFile(path);

            Dictionary<string, int> tileSideStructure = new Dictionary<string, int>();

            int structureCount = 0;
            foreach (HyperEdge edge in hyperGraph.Edges)
            {
                structureCount = edge.GreenSideCount() * 100 + edge.BlueSideCount() * 10 + edge.RedSideCount();

                tileSideStructure.Add(edge.ToString(), structureCount);
            }

            //Dictionary<string, int> onlyblue = tileSideStructure.Where(kv => kv.Value % 10 == 0).ToDictionary(kv => kv.Key, kv => kv.Value);
            //Dictionary<string, int> onlyGreen = tileSideStructure.Where(kv => kv.Value % 100 == 0).ToDictionary(kv => kv.Key, kv => kv.Value);
            //Dictionary<string, int> containsBlueAndGreen = tileSideStructure.Where(kv => kv.Value > 100 && (kv.Value % 100) >= 10).ToDictionary(kv => kv.Key, kv => kv.Value);
            //Dictionary<string, int> rest = tileSideStructure.Where(kv => !onlyblue.Keys.Contains(kv.Key) && !containsBlueAndGreen.Keys.Contains(kv.Key)).ToDictionary(kv => kv.Key, kv => kv.Value);
            //Dictionary<string, int> redAndBlue = tileSideStructure.Where(kv => kv.Value % 2 == 1).ToDictionary(kv => kv.Key, kv => kv.Value);

            HyperEdge[] onlyBlue = hyperGraph.Edges.Where(e => e.BlueSideCount() == 3).ToArray();
            HyperEdge[] onlyGreen = hyperGraph.Edges.Where(e => e.GreenSideCount() == 3).ToArray();
            HyperEdge[] onlyRed = hyperGraph.Edges.Where(e => e.RedSideCount() == 3).ToArray();
            HyperEdge[] twoBlueOneGreen = hyperGraph.Edges.Where(e => e.BlueSideCount() == 2 && e.GreenSideCount() == 1).ToArray();
            HyperEdge[] twoBlueOneRed = hyperGraph.Edges.Where(e => e.BlueSideCount() == 2 && e.RedSideCount() == 1).ToArray();
            HyperEdge[] twoGreenOneBlue = hyperGraph.Edges.Where(e => e.BlueSideCount() == 1 && e.GreenSideCount() == 2).ToArray();
            HyperEdge[] twoGreenOneRed = hyperGraph.Edges.Where(e => e.RedSideCount() == 1 && e.GreenSideCount() == 2).ToArray();
            HyperEdge[] greenRedBlue = hyperGraph.Edges.Where(e => e.BlueSideCount() == 1 && e.GreenSideCount() == 1 && e.RedSideCount() == 1).ToArray();
            HyperEdge[] moreThanOneRed = hyperGraph.Edges.Where(e => e.RedSideCount() > 1).ToArray();

            //30 = 6
            //201 = 20
            //111 = 30
        }

        [TestMethod]
        public void GetTileProbabilities()
        {
            string path = Path.Combine(Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.FullName, "TestFiles", "tEdgePieces.txt");
            //string path = Path.Combine(Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName, "TestFiles", "tEdgePieces.txt");

            HyperGraph hyperGraph = GraphGenerator.LoadHyperGraphFromFile(path);
            //Tuple<string, int>[] degrees = hyperGraph.GetVertexDegrees();
            Tuple<string, int>[] degrees = hyperGraph.GetVertexEulerDegrees();

            Dictionary<HyperEdge, float> probabilities = new Dictionary<HyperEdge, float>();

            int blueBlueBlueValues, greenGreenRedValues, greenBlueRedValues;
            blueBlueBlueValues = greenGreenRedValues = greenBlueRedValues = 0;

            foreach (HyperEdge edge in hyperGraph.Edges)
            {
                int tileSideWs = 0;
                foreach (Vertex vertex in edge.Vertices)
                {
                    string reverse = vertex.Value.ReverseString();
                    int sideWS = degrees.Where(t => t.Item1.Equals(reverse)).Single().Item2;
                    if (edge.Vertices.Select(v => v.Value).Contains(reverse))
                    {
                        sideWS--;
                    }
                    tileSideWs += sideWS;
                }

                string[] vertexCounterParts = edge.Vertices.Select(v => v.Value.ReverseString()).ToArray();
                List<HyperEdge> duplicate = hyperGraph.Edges.Where(e => !e.Equals(edge) && e.Vertices.Select(v => v.Value).Intersect(vertexCounterParts).Count() == 2).ToList();

                // The number for tileSideWs has to be corrected about the number of those tiles, 
                // which hold more than one 'counterparts' (max counterparts on one tile is 2)
                // of the actual tile sides.
                tileSideWs = tileSideWs - duplicate.Count;

                //float ws = (float)tileSideWs/168.0f;
                float ws = (float)tileSideWs / 56.0f;
                probabilities.Add(edge, ws);

                switch (edge.Vertices.Select(v => v.Value.GetVertexColor()).Distinct().Count())
                {
                    case 1:
                        blueBlueBlueValues += edge.Vertices.Select(v => Int32.Parse(v.Value[0].ToString())).Sum();
                        break;
                    case 2:
                        greenGreenRedValues += edge.Vertices.Select(v => Int32.Parse(v.Value[0].ToString())).Sum();
                        break;
                    case 3:
                        greenBlueRedValues += edge.Vertices.Select(v => Int32.Parse(v.Value[0].ToString())).Sum();
                        break;
                    default:
                        throw new InvalidOperationException($"Cannot determine SideType for Tile: {edge.Vertices.Select(v => v.Value[0].ToString()).Aggregate((a, b) => a + " - " + b)}");
                }

                //45 / 6 => 7.5
                //150 / 20 => 7.5
                //225 / 30 => 7.5
            }

            string outputPath = @"C:\Users\Moritz\Dropbox\Studium\Fulda\5_SS19\Project\Triominos\Graphsuite\GraphSuite\Files";
            string outputFile = "tileProbabilitiesAndValues.txt";

            if (File.Exists(Path.Combine(outputPath, outputFile)))
            {
                File.Delete(Path.Combine(outputPath, outputFile));
            }

            File.WriteAllLines(
                Path.Combine(outputPath, outputFile),
                probabilities.OrderBy(kv => kv.Key.GreenSideCount()).Select(
                    kv => kv.Key.Vertices.Select(v => v.Value[0].ToString()).Aggregate((a, b) => a + "-" + b) + " | " +
                    kv.Key.Vertices.Select(v => Int32.Parse(v.Value[0].ToString())).Sum() + " | " +
                    kv.Key.Vertices.Select(v => v.Value.GetVertexColor().ToString()).Aggregate((a, b) => a + "-" + b) + " | " +
                    kv.Value));
        }
    }
}
