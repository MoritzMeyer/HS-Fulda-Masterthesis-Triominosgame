using System;
using System.Collections.Generic;
using System.Linq;

namespace GraphSuite
{
    public class HyperGraph
    {
        #region ctor
        /// <summary>
        /// Erzeugt eine neue Instanz der Klasse.
        /// </summary>
        public HyperGraph(IEnumerable<string> vertices, IEnumerable<HyperEdge> edges)
        {
            this.Vertices = vertices.ToList();
            this.Edges = edges.ToList();
        }

        public HyperGraph(IEnumerable<string> vertices)
        {
            this.Vertices = vertices.ToList();
        }

        public HyperGraph()
        {
            this.Vertices = new List<string>();
            this.Edges = new List<HyperEdge>();
        }
        #endregion

        #region Vertices
        /// <summary>
        /// Die Knoten des Graphen
        /// </summary>
        public List<string> Vertices { get; private set; }
        #endregion

        #region Edges
        /// <summary>
        /// Die Kanten des Graphen.
        /// </summary>
        public List<HyperEdge> Edges { get; private set; }
        #endregion

        #region AddVertex
        /// <summary>
        /// Fügt dem Graphen einen weiteren Knoten hinzu.
        /// </summary>
        /// <param name="vertex"></param>
        public void AddVertex(string vertex)
        {
            if (this.Vertices.Contains(vertex))
            {
                return;
            }

            this.Vertices.Add(vertex);
        }
        #endregion

        #region AddEdge
        /// <summary>
        /// Fügt dem Graphen eine weitere Kante hinzu.
        /// </summary>
        /// <param name="edge"></param>
        public void AddEdge(HyperEdge edge)
        {
            if (this.Edges.Contains(edge))
            {
                throw new ArgumentException("Diese Kante existiert bereits im Graphen.");
            }

            this.Edges.Add(edge);
        }
        #endregion

        #region HasVertex
        /// <summary>
        /// Prüft für einen Knoten, ob dieser bereits im Graphen vorhanden ist.
        /// </summary>
        /// <param name="vertex">Der zu überprüfende Knoten.</param>
        /// <returns>True, wenn der Knoten vorhanden ist, False wenn nicht.</returns>
        public bool HasVertex(string vertex)
        {
            return this.Vertices.Contains(vertex);
        }
        #endregion

        #region HasEdge
        /// <summary>
        /// Überprüft für eine Kante, ob diese bereits im Graphen existiert.
        /// </summary>
        /// <param name="edge">Die zu überprüfende Kante.</param>
        /// <returns>True, wenn die Kante im Graphen vorhanden ist, False wenn nicht.</returns>
        public bool HasEdge(HyperEdge edge)
        {
            return this.Edges.Contains(edge);
        }
        #endregion

        #region GetAdjacentEdges
        /// <summary>
        /// Liefert eine Aufzählung mit allen angrenzenden Kanten zu einem Knoten.
        /// </summary>
        /// <param name="vertex">Der Knoten zu dem die angrenzenden Kanten gesucht werden.</param>
        /// <returns>Die Auflistung mit angrezenden Kanten.</returns>
        public IEnumerable<HyperEdge> GetAdjacentEdges(string vertex, int edgeVertexCount = 0)
        {
            if (edgeVertexCount != 0)
            {
                return this.Edges.Where(e => e.VertexCount == edgeVertexCount && e.Contains(vertex));
            }

            return this.Edges.Where(e => e.Contains(vertex));
        }
        #endregion

        #region GetVertexDegrees
        /// <summary>
        /// Liefert ein Tuple-Array mit allen Knoten und deren Edge-Degrees.
        /// </summary>
        /// <returns>Das Tuple-Array</returns>
        public Tuple<string, int>[] GetVertexDegrees()
        {
            List<Tuple<string, int>> degrees = new List<Tuple<string, int>>();
            foreach (string vertex in this.Vertices)
            {
                int count = 0;
                foreach (HyperEdge edge in this.Edges)
                {
                    count += edge.Vertices.Where(v => v.GetVertexValue().Equals(vertex)).Count();
                }

                degrees.Add(new Tuple<string, int>(vertex, count));
            }

            return degrees.ToArray();
        }
        #endregion

        #region GetVertexEulerDegrees
        /// <summary>
        /// Ermittelt für jeden Knoten die einfache Anzahl an Kanten, die mit diesem Knoten verbunden sind 
        /// (auch wenn eine Kante mehrfach mit einem Knoten verbunden ist, wird diese nur einmal gezählt).
        /// </summary>
        /// <returns>Der Kantengrad pro Knoten.</returns>
        public Tuple<string, int>[] GetVertexEulerDegrees()
        {
            List<Tuple<string, int>> degrees = new List<Tuple<string, int>>();
            foreach (string vertex in this.Vertices)
            {
                int count = 0;
                foreach (HyperEdge edge in this.Edges)
                {
                    if (edge.Vertices.Select(v => v.GetVertexValue()).Contains(vertex))
                    {
                        count++;
                    }
                }

                degrees.Add(new Tuple<string, int>(vertex, count));
            }

            return degrees.ToArray();
        }
        #endregion

        /*
        // in Anlehnung an https://www.geeksforgeeks.org/print-all-the-cycles-in-an-undirected-graph/
        public void GetAllSimpleCycles()
        {
            List<Tuple<HyperEdge, string>> cycles = new List<Tuple<HyperEdge, string>>();
            Dictionary<HyperEdge, int> edgeColors = this.Edges.ToDictionary(e => e, e => 0);
            int cycleNumber = 0;

            IEnumerable<HyperEdge> threeSidedEdges = this.Edges.Where(e => e.VertexCount == 3);
            if (threeSidedEdges.Count() < 1)
            {
                return;
            }

            HyperEdge startEdge = this.Edges.Where(e => e.VertexCount == 3).First();
            foreach (string vertex in startEdge.Vertices)
            {

            }            
        }


        public void GetSimpleCylce(
            HyperEdge u, 
            string p, 
            Dictionary<HyperEdge, int> edgeColors,
            List<Tuple<HyperEdge, string>> cycles, 
            Dictionary<HyperEdge, string> parent,
            int cycleNumber)
        {
            // already completly visited edge
            if (edgeColors[u] == 2)
            {
                return;
            }

            // seen edge, but was not completly visited -> cycle detected
            if (edgeColors[u] == 1)
            {
                cycleNumber++;
                // follow and add cycle to cycles.
            }

            parent[u] = p;
            edgeColors[u] = 1;

            IEnumerable<string> neighbourVertices = u.Vertices.Where(v => !object.Equals(u, v));
            foreach (string vertex in neighbourVertices)
            {
                if (neighbourVertices.Count() > 2 || neighbourVertices.Count() < 1)
                {
                    throw new IndexOutOfRangeException("Number of Vertices in one Edge of a Triominograph has to be 2 or 3");
                }

                if (neighbourVertices.Count() == 1)
                {
                    string nextParent = neighbourVertices.ElementAt(0);
                    IEnumerable<HyperEdge> adjacentEdges = this.GetAdjacentEdges(nextParent).Where(e => e.VertexCount == 3);
                    if (adjacentEdges.Count() < 1)
                    {
                        return;
                    }

                    foreach (HyperEdge edge in adjacentEdges)
                    {
                        if ()
                        this.GetSimpleCylce(edge, nextParent, edgeColors, cycles, parent, cycleNumber);
                    }
                }
            }
        }
        */

        public List<List<Tuple<HyperEdge, string>>> GetAllSimpleCycles2()
        {
            // Alle Kanten mit 3 Knoten ermitteln
            IEnumerable<HyperEdge> threeSidedHyperEdges = this.Edges.Where(e => e.VertexCount == 3);

            // Für die Kanten mit 3 Knoten das Dictionary mit der Color füllen
            Dictionary<HyperEdge, int> color = threeSidedHyperEdges.ToDictionary(e => e, e => 0);

            // Für jede Kante die Vorgängerkante, sowie der Knoten über welchen die akutelle Kante erreicht wurde speichern.
            Dictionary<HyperEdge, Tuple<HyperEdge, string>> parent = threeSidedHyperEdges.ToDictionary(e => e, e => new Tuple<HyperEdge, string>(null, null));

            // Liste mit den Zyklen erstellen
            List<List<Tuple<HyperEdge, string>>> cycles = new List<List<Tuple<HyperEdge, string>>>();

            this.GetSimpleCycle2(threeSidedHyperEdges.First(), null, new List<Tuple<HyperEdge, string>>(), color, cycles);
            /*
            foreach (HyperEdge edge in threeSidedHyperEdges)
            {
                foreach(string vertex in edge.Vertices)
                {
                    this.GetSimpleCycle2(edge, vertex, new Dictionary<HyperEdge, Tuple<HyperEdge, string>>(), color, cycles);
                }
            }
            */

            return cycles;
        }

        /// <summary>
        /// Werte wie Colors, Parent etc. als value, nicht referenz übergben?!
        /// Cycles als Referenz übergben.
        /// </summary>
        public void GetSimpleCycle2(
            HyperEdge u,
            HyperEdge p, 
            List<Tuple<HyperEdge, string>> parent, 
            Dictionary<HyperEdge, int> color,
            List<List<Tuple<HyperEdge, string>>> cycles)
        {
            // edge was completly visited before
            if (color.ContainsKey(u) && color[u] == 2)
            {
                return;
            }

            // possible cycle detected
            if (color.ContainsKey(u) && color[u] == 1)
            {
                // Ein Zyklus im Triominograph muss immer min. die Länge 6 (nur 3er Kanten) bzw. 12 (inkl. 2er Kanten haben)
                int cycleBegin = parent.Count - 12;
                if (cycleBegin >= 0)
                {
                    // Suche den Anfang des Zyklus (soweit vorhanden)
                    while (parent[cycleBegin].Item1 != u && cycleBegin > 0)
                    {
                        cycleBegin--;
                    }

                    // Wenn anfang gefunden wurde, den Zyklus speichern.
                    if (parent[cycleBegin].Item1.Equals(u))
                    {
                        List<Tuple<HyperEdge, string>> cycle = new List<Tuple<HyperEdge, string>>();
                        for (int i = cycleBegin; i < parent.Count; i++)
                        {
                            cycle.Add(parent[i]);
                        }

                        cycles.Add(cycle);
                    }
                }
            }

            if (color.ContainsKey(u))
            {
                color[u] = 1;
            }

            // Den Knoten ermitteln, über welchen die Kante u erreicht wurde.
            string parentVertex = string.Empty;
            if (parent.Count > 0)
            {
                parentVertex = parent.Last().Item2;
            }

            // die Knoten der Kante u ermitteln, die nicht dem eingehenden Knoten entsprechen (diese müssen noch besucht werden)
            IEnumerable<string> adjacentVertices = u.GetNeighborVertices(parentVertex);

            // Die Anzahl der Knoten welche die nächste Kante besitzen muss 
            //ermitteln (Kanten mit 2 und 3 Knoten werden immer im Wechsel durchlaufen).
            int vCount = (u.VertexCount == 2) ? 3 : 2;

            // die weiteren Knoten der Kante u besuchen 
            foreach (string vertex in adjacentVertices)
            {
                // Alle möglichen Kanten von dem aktuellen Knoten aus ermitteln.
                IEnumerable<HyperEdge> edges = this.GetAdjacentEdges(vertex, vCount);
                if (parent.Count - 2 >= 0)
                {
                    edges = edges.Where(e => !e.Equals(parent[parent.Count - 1].Item1));
                }

                foreach (HyperEdge edge in edges)
                {
                    // Parent Kante und Knoten für die nächste Kante hinzfügen.
                    // Für den nachfolgenden Knotenvergleich muss darauf geachtet werden, 
                    // dass hier der Wert des Knotens der nächsten Kante verwendet wird 
                    // (in diesem ist die Position des Knotens in der nächsten Kante gespeichert)
                    parent.Add(new Tuple<HyperEdge, string>(u, edge.GetVertexEdgeValue(vertex)));
                    this.GetSimpleCycle2(edge, u, parent, color, cycles);
                }
            }

            if (color.ContainsKey(u))
            {
                color[u] = 2;
            }
        }

        // Algoritmus:
        /*
         * 1. Für jede Kante Vorgänger Kante und Knoten in einem Stack (o.ä. merken)
         * 2. Die Color und den parent bei jedem Durchlauf wie in => https://www.geeksforgeeks.org/print-all-the-cycles-in-an-undirected-graph/
         *    setzen
         * 3. Wenn Color = 1 => prüfen ob es ein Zyklus sein kann (min. Kantenlänge = 6 (nur die 3er Kanten zählen))
         * 4. alle weiteren Knoten von 3er Kanten durchlaufen
         * 5. Auch zweier Kanten auf den Parentstack packen
         * 6. Mit 3er anfangen und dann immer mit 2er im Wechsel durchlaufen
         */
    }
}
