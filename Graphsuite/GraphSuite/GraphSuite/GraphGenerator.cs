using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace GraphSuite
{
    public static class GraphGenerator
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
                HyperEdge hyperEdge = new HyperEdge(lineData);
                if (!hyperGraph.HasEdge(hyperEdge))
                {
                    hyperGraph.AddEdge(hyperEdge);
                }

                lineNumber++;
            }

            return hyperGraph;
        }
        #endregion
    }
}
