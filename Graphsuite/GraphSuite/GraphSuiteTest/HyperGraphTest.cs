using GraphSuite;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace GraphSuiteTest
{
    [TestClass]
    public class HyperGraphTest
    {
        [TestMethod]
        public void HyperGraph_must_work()
        {
            string path = Path.Combine(Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.FullName, "TestFiles", "tEdgePieces.txt");
            //string path = Path.Combine(Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName, "TestFiles", "tEdgePieces.txt");

            HyperGraph hyperGraph = GraphGenerator.LoadHyperGraphFromFile(path);
            //Tuple<string, int>[] degrees = hyperGraph.GetVertexDegrees();
            Tuple<string, int>[] degrees = hyperGraph.GetVertexEulerDegrees();

            int greenEdgeDegree = 0;
            int redEdgeDegree = 0;
            int redEdgeCount = 0;
            int greenEdgeCount = 0;
            int evenEdgeDegree = 0;
            int evenEdgeCount = 0;
            int maxDegree = degrees.Select(d => d.Item2).Max();
            Tuple<string, int>[] maxDegrees = degrees.Where(d => d.Item2 == 6).ToArray();

            foreach(Tuple<string, int> tuple in degrees)
            {
                int i = Convert.ToInt32(tuple.Item1[0]);
                int j = Convert.ToInt32(tuple.Item1[1]);

                if (i < j)
                {
                    greenEdgeDegree += tuple.Item2;
                    greenEdgeCount++;
                }
                else if (i > j)
                {
                    redEdgeDegree += tuple.Item2;
                    redEdgeCount++;
                }
                else
                {
                    evenEdgeDegree += tuple.Item2;
                    evenEdgeCount++;
                }
            }

            Tuple<string, int>[] orderedDegrees = degrees.OrderBy(t => t.Item2).ThenBy(t => t.Item1).ToArray();

            string outputPath = @"C:\Users\Moritz\Dropbox\Studium\Fulda\5_SS19\Project\Triominos\Graphsuite\GraphSuite\Files";
            string outputFile = "tEdgePiecesEulerDegrees.txt";

            if (File.Exists(Path.Combine(outputPath, outputFile)))
            {
                File.Delete(Path.Combine(outputPath, outputFile));
            }

            File.WriteAllLines(Path.Combine(outputPath, outputFile), degrees.OrderBy(x => x.Item1).Select(d => d.Item1 + " => " + d.Item2));
            //Tuple<int, Stack<Tuple<string, HyperEdge<string>>>>[] cycles = hyperGraph.GetAllSimpleCycles();
        }

        [TestMethod]
        public void GetAllCycles_must_work()
        {
            string path = Path.Combine(Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.FullName, "TestFiles", "tEdgeGraph.txt");
            HyperGraph hyperGraph = GraphGenerator.LoadHyperGraphFromFile(path);

            List<List<Tuple<HyperEdge, string>>> cycles = hyperGraph.GetAllSimpleCycles2();

        }

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
            
            foreach(HyperEdge edge in hyperGraph.Edges)
            {
                int tileSideWs = 0;
                foreach(string vertex in edge.Vertices)
                {
                    string reverse = vertex.GetVertexValue().Reverse();
                    int sideWS = degrees.Where(t => t.Item1.Equals(reverse)).Single().Item2;
                    if (edge.Vertices.Select(v => v.GetVertexValue()).Contains(reverse))
                    {
                        sideWS--;
                    }
                    tileSideWs += sideWS;
                }

                string[] vertexCounterParts = edge.Vertices.Select(v => v.GetVertexValue().Reverse()).ToArray();
                List<HyperEdge> duplicate = hyperGraph.Edges.Where(e => !e.Equals(edge) && e.Vertices.Select(v => v.GetVertexValue()).Intersect(vertexCounterParts).Count() == 2).ToList();

                // The number for tileSideWs has to be corrected about the number of those tiles, 
                // which hold more than one 'counterparts' (max counterparts on one tile is 2)
                // of the actual tile sides.
                tileSideWs = tileSideWs - duplicate.Count;

                //float ws = (float)tileSideWs/168.0f;
                float ws = (float)tileSideWs/56.0f;
                probabilities.Add(edge, ws);
            }

            string outputPath = @"C:\Users\Moritz\Dropbox\Studium\Fulda\5_SS19\Project\Triominos\Graphsuite\GraphSuite\Files";
            string outputFile = "tileProbabilitiesOnTileBasisCorrect.txt";

            if (File.Exists(Path.Combine(outputPath, outputFile)))
            {
                File.Delete(Path.Combine(outputPath, outputFile));
            }

            File.WriteAllLines(
                Path.Combine(outputPath, outputFile), 
                probabilities.OrderBy(kv => kv.Key.GreenSideCount()).Select(
                    kv => kv.Key.Vertices.Select(v => v.GetVertexValue()[0].ToString()).Aggregate((a, b) => a + "-" + b) + " | " + 
                    kv.Key.Vertices.Select(v => v.GetVertexColor().ToString()).Aggregate((a, b) => a + "-" + b) + " | " + 
                    kv.Value));
        }
    }
}
