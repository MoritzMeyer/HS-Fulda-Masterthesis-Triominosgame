using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphKI.GraphSuite
{
    public class GraphGenerator
    {
        #region LoadHyperGraphFromFile
        /// <summary>
        /// Lädt einen HyperGraphen aus einer Datei.
        /// </summary>
        /// <param name="path">Der Pfad zu der Datei.</param>
        /// <returns>Den geladenen Graphen.</returns>
        public static HyperGraph LoadHyperGraphFromFile(string path)
        {
            // Die Daten aus der Datei laden.
            IEnumerable<string> lines = File.ReadLines(path);
            if (lines.Count() < 1)
            {
                throw new InvalidDataException("The given File is empty");
            }

            // Den Graphen erstellen.
            HyperGraph hyperGraph = new HyperGraph();

            // Die einzelnen Zeilen der Datei auslesen.
            int lineNumber = 0;
            IEnumerator<string> linesEnumerator = lines.GetEnumerator();
            while (linesEnumerator.MoveNext())
            {
                string[] lineData = linesEnumerator.Current.Split(' ');
                if (lineData.Count() < 2)
                {
                    continue;
                    //throw new ArgumentException("One edge must connect at least two vertices (Line: " + lineNumber + ").");
                }

                // Die Knoten hinzufügen, falls noch nicht vorhanden.
                foreach (string vertex in lineData)
                {
                    if (!hyperGraph.HasVertex(vertex))
                    {
                        hyperGraph.AddVertex(vertex);
                    }
                }

                // Die Kante hinzufügen, falls noch nicht vorhanden
                HyperEdge hyperEdge = (lineData.Length == 2) ? new HyperEdge(lineData[0], lineData[1]) : new HyperEdge(lineData[0], lineData[1], lineData[2]);
                if (!hyperGraph.HasEdge(hyperEdge))
                {
                    if (hyperEdge.IsThreeSidedEdge())
                    {
                        hyperGraph.AddEdge(hyperEdge.Vertices[0], hyperEdge.Vertices[1], hyperEdge.Vertices[2]);
                    }
                    else
                    {
                        hyperGraph.AddEdge(hyperEdge.Vertices[0], hyperEdge.Vertices[1]);
                    }                    
                }

                lineNumber++;
            }

            return hyperGraph;
        }
        #endregion

        #region GetAllTriominoVertices
        /// <summary>
        /// Returns all Vertices for an Triomino graph.
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<string> GetAllTriominoVertices()
        {
            List<string> vertices = new List<string>();
            for (int i = 0; i < 6; i++)
            {
                for (int j = i; j < 6; j++)
                {
                    vertices.Add($"{i}{j}");
                    if (i != j)
                    {
                        vertices.Add($"{j}{i}");
                    }
                }
            }

            return vertices;
        }
        #endregion

        #region GetAllTriominoEdges
        /// <summary>
        /// Creates all Edges for a TriominoEdgeGraph.
        /// </summary>
        /// <returns>the edges.</returns>
        public static IEnumerable<HyperEdge> GetAllTriominoEdges()
        {
            List<HyperEdge> triominoEdges = new List<HyperEdge>();
            //List<string> test = new List<string>();

            // Threesided edges
            for (int i = 0; i < 6; i++)
            {
                for (int j = i; j < 6; j++)
                {
                    //test.Add($"{i}-{j}: {i}{j}-{j}{i}");
                    triominoEdges.Add(new HyperEdge($"{i}{j}", $"{j}{i}"));
                    for (int k = j; k < 6; k++)
                    {
                        //test.Add($"{i}-{j}-{k}: {i}{j}-{j}{k}-{k}{i}");
                        triominoEdges.Add(new HyperEdge($"{i}{j}", $"{j}{k}", $"{k}{i}"));
                    }
                }
            }

            return triominoEdges;
        }
        #endregion

        public static HyperGraph CreateTriominoGraph()
        {
            IEnumerable<string> vertices = GraphGenerator.GetAllTriominoVertices();
            IEnumerable<HyperEdge> edges = GraphGenerator.GetAllTriominoEdges();
            HyperGraph triomionGraph = new HyperGraph();

            // Add all vertices
            foreach(string vertexValue in vertices)
            {
                triomionGraph.AddVertex(vertexValue);
            }

            // Add all Edges
            foreach(HyperEdge edge in edges)
            {
                if (edge.IsTwoSidedEdge())
                {
                    triomionGraph.AddEdge(edge.Vertices[0].Value, edge.Vertices[1].Value);
                }
                else
                {
                    triomionGraph.AddEdge(edge.Vertices[0].Value, edge.Vertices[1].Value, edge.Vertices[2].Value);
                }
            }

            return triomionGraph;
        }
    }
}
