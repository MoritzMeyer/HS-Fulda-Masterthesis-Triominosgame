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

        #region GetAllTriominoEdges
        /// <summary>
        /// Creates all Edges for a TriominoEdgeGraph.
        /// </summary>
        /// <returns>the edges.</returns>
        public static IEnumerable<HyperEdge> GetAllTriominoEdges()
        {
            List<HyperEdge> triominoEdges = new List<HyperEdge>();
            //List<string> test = new List<string>();

            for (int i = 0; i < 6; i++)
            {
                for (int j = i; j < 6; j++)
                {
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
    }
}
