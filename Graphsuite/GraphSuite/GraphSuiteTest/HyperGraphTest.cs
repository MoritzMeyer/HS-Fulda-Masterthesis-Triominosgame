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
        public void GetAllCycles_must_work()
        {
            string path = Path.Combine(Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.FullName, "TestFiles", "tEdgeGraph.txt");
            HyperGraph hyperGraph = GraphGenerator.LoadHyperGraphFromFile(path);

            List<List<Tuple<HyperEdge, string>>> cycles = hyperGraph.GetAllSimpleCycles2();

        }
    }
}
