using GraphKI.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphKI.GraphSuite
{
    public class HyperGraph
    {
        #region fields
        /// <summary>
        /// Vertices of this graph.
        /// </summary>
        public List<Vertex> Vertices { get; private set; }

        /// <summary>
        /// Edges of this graph.
        /// </summary>
        public List<HyperEdge> Edges { get; private set; }
        #endregion

        #region ctor
        /// <summary>
        /// Instantiates a new object of this class.
        /// </summary>
        /// <param name="vertices">All vertices of this graph.</param>
        /// <param name="edges">All edges of this graph.</param>
        //public HyperGraph(IEnumerable<Vertex> vertices, IEnumerable<HyperEdge> edges)
        //{
        //    this.Vertices = vertices.ToList();
        //    this.Edges = edges.ToList();
        //}

        /// <summary>
        /// Instantiates a new object of this class.
        /// </summary>
        /// <param name="vertices">All vertices of this graph.</param>
        public HyperGraph(IEnumerable<Vertex> vertices)
        {
            this.Vertices = vertices.ToList();
        }

        /// <summary>
        /// Instantiates a new object of this class.
        /// </summary>
        public HyperGraph()
        {
            this.Vertices = new List<Vertex>();
            this.Edges = new List<HyperEdge>();
        }
        #endregion

        #region AddVertex
        /// <summary>
        /// Adds a new Vertex to this graph, if it isn't already part of it.
        /// </summary>
        /// <param name="vertex">Vertex to be added.</param>
        public bool AddVertex(Vertex vertex)
        {
            if (this.HasVertex(vertex) || this.HasVertex(vertex.Value))
            {
                return false;
            }

            this.Vertices.Add(vertex);
            return true;
        }

        /// <summary>
        /// Adds a new Vertex to this graph based on its value, if it isn't already part of it.
        /// </summary>
        /// <param name="vertex">Value of Vertex to be added.</param>
        public Vertex AddVertex(string vertexValue)
        {
            if (this.HasVertex(vertexValue))
            {
                return this.Vertices.Where(v => v.Value.Equals(vertexValue)).First();
            }

            Vertex vertex = new Vertex(vertexValue);
            this.AddVertex(vertex);
            return vertex;
        }
        #endregion

        #region AddEdge
        /// <summary>
        /// Adds a new Edge to this graph, if it isn't already part of it.
        /// </summary>
        /// <param name="vertex1">Vertex1 of the edge to be added.</param>
        /// <param name="vertex2">Vertex2 of the edge to be added.</param>
        /// <param name="vertex3">Vertex3 of the edge to be added.</param>
        /// <returns></returns>
        public HyperEdge AddEdge(string vertex1, string vertex2, string vertex3 = null)
        {
            if (!this.HasVertex(vertex1))
            {
                throw new ArgumentException($"This graph does not conatin a vertex with value '{vertex1}'.");
            }

            if (!this.HasVertex(vertex2))
            {
                throw new ArgumentException($"This graph does not conatin a vertex with value '{vertex2}'.");
            }

            if (vertex3 != null && !this.HasVertex(vertex3))
            {
                throw new ArgumentException($"This graph does not conatin a vertex with value '{vertex3}'.");
            }

            Vertex v1 = Vertex.CreateFromVertex(this.Vertices.Where(v => v.Value.Equals(vertex1)).First());
            Vertex v2 = Vertex.CreateFromVertex(this.Vertices.Where(v => v.Value.Equals(vertex2)).First());
            Vertex v3 = vertex3 != null ? Vertex.CreateFromVertex(this.Vertices.Where(v => v.Value.Equals(vertex3)).First()) : null;

            HyperEdge edge = new HyperEdge(v1, v2, v3);

            if (this.HasEdge(edge))
            {
                throw new ArgumentException($"This Graph has already an edge '{edge}'.");
            }

            this.Edges.Add(edge);
            return edge;
        }

        /// <summary>
        /// /// Adds a new Edge to this graph, if it isn't already part of it.
        /// </summary>
        /// <param name="vertex1">Vertex1 of the edge to be added.</param>
        /// <param name="vertex2">Vertex2 of the edge to be added.</param>
        /// <param name="vertex3">Vertex3 of the edge to be added.</param>
        /// <returns></returns>
        public HyperEdge AddEdge(Vertex vertex1, Vertex vertex2, Vertex vertex3 = null)
        {
            if (vertex1.EqualsOnEdgeBasis(vertex2) || vertex1.EqualsOnEdgeBasis(vertex3) || vertex2.EqualsOnEdgeBasis(vertex3))
            {
                throw new ArgumentException("Vertices must not be equal.");
            }

            if (!this.HasVertex(vertex1.Value))
            {
                throw new ArgumentException($"This graph does not conatin a vertex with value '{vertex1}'.");
            }

            if (!this.HasVertex(vertex2.Value))
            {
                throw new ArgumentException($"This graph does not conatin a vertex with value '{vertex2}'.");
            }

            if (vertex3 != null && !this.HasVertex(vertex3.Value))
            {
                throw new ArgumentException($"This graph does not conatin a vertex with value '{vertex3}'.");
            }

            Vertex v1 = Vertex.CreateFromVertex(this.Vertices.Where(v => v.Value.Equals(vertex1.Value)).First());
            Vertex v2 = Vertex.CreateFromVertex(this.Vertices.Where(v => v.Value.Equals(vertex2.Value)).First());
            Vertex v3 = vertex3 != null ? Vertex.CreateFromVertex(this.Vertices.Where(v => v.Value.Equals(vertex3.Value)).First()) : null;

            HyperEdge edge = new HyperEdge(v1, v2, v3);

            if (this.HasEdge(edge))
            {
                throw new ArgumentException($"This Graph has already an edge '{edge}'.");
            }

            this.Edges.Add(edge);
            return edge;
        }
        #endregion

        #region HasVertex
        /// <summary>
        /// Verifys, if this graph contains a specific vertex.
        /// </summary>
        /// <param name="vertex">Vertex to be verified.</param>
        /// <returns>True, if this graph contains vertex, false if not.</returns>
        public bool HasVertex(Vertex vertex)
        {
            foreach (Vertex thisVertex in this.Vertices)
            {
                if (thisVertex.EqualsOnVertexBasis(vertex))
                {
                    return true;
                }
            }

            return false;                
        }

        /// <summary>
        /// Verifys, if this graph contains a vertex with a specific value.
        /// </summary>
        /// <param name="vertexValue">Vertex value to be verified.</param>
        /// <returns>True, if this graph contains vertex value, false if not.</returns>
        public bool HasVertex(string vertexValue)
        {
            foreach(Vertex thisVertex in this.Vertices)
            {
                if (thisVertex.Value == vertexValue)
                {
                    return true;
                }
            }

            return false;
        }
        #endregion

        #region HasEdge
        /// <summary>
        /// Verifies, if this graph contains a specific edge.
        /// </summary>
        /// <param name="edge">Edge to be verified.</param>
        /// <returns>True, if this graph contains edge, false if not.</returns>
        public bool HasEdge(HyperEdge edge)
        {
            foreach (HyperEdge thisEdge in this.Edges)
            {
                if (thisEdge.EqualsOnValueBasis(edge))
                {
                    return true;
                }
            }
            return false;
        }
        #endregion

        #region GetVertex
        /// <summary>
        /// Determines the vertex instance of a vertex by its value.
        /// </summary>1
        /// <param name="vertexValue">value of the vertex.</param>
        /// <returns>instance of the vertex.</returns>
        public Vertex GetVertex(string vertexValue)
        {
            if (!this.HasVertex(vertexValue))
            {
                throw new ArgumentException($"This Graph does not contain a vertex with value '{vertexValue}'.");
            }

            Vertex vertex = this.Vertices.Where(v => v.Value.Equals(vertexValue)).Single();
            return vertex;
        }
        #endregion

        #region GetAdjacentEdges
        /// <summary>
        /// Determines all edges within this graph, which are adjacent to a specific vertex.
        /// If edgeVertexCount is defined, only those edges with the number of vertices given in edgeVertexCount are considered.
        /// </summary>
        /// <param name="vertex">vertex for which the adjacent edges are sought.</param>
        /// <param name="edgeVertexCount">number of vertices to which the adjacent edges are limited.</param>
        /// <returns>All Edges which fulfill the given requirements.</returns>
        public IEnumerable<HyperEdge> GetAdjacentEdges(Vertex vertex, int edgeVertexCount = 0)
        {
            if (!this.HasVertex(vertex))
            {
                throw new ArgumentException($"This Graph does not contain a vertex '{vertex}'.");
            }

            if (edgeVertexCount != 0)
            {
                return this.Edges.Where(e => e.VertexCount() == edgeVertexCount && e.ContainsOnVertexBasis(vertex));
            }

            return this.Edges.Where(e => e.ContainsOnVertexBasis(vertex));
        }
        #endregion

        #region GetVertexEulerDegrees
        /// <summary>
        /// Counts for a vertex the simple number of edges, which are ajdacent to this vertex.
        /// (if one edge is adjacent multiple times to one vertex, this counts also only as one)
        /// </summary>
        /// <returns>VertexDegree per vertex.</returns>
        public Tuple<string, int>[] GetVertexEulerDegrees()
        {
            List<Tuple<string, int>> degrees = new List<Tuple<string, int>>();
            foreach (Vertex vertex in this.Vertices)
            {
                int count = 0;
                foreach (HyperEdge edge in this.Edges)
                {
                    if (edge.ContainsOnVertexBasis(vertex))
                    {
                        count++;
                    }
                }

                degrees.Add(new Tuple<string, int>(vertex.Value, count));
            }

            return degrees.ToArray();
        }
        #endregion

        public List<List<Tuple<HyperEdge, Vertex>>> GetAllSimpleCycles()
        {
            // Alle Kanten mit 3 Knoten ermitteln
            IEnumerable<HyperEdge> threeSidedHyperEdges = this.Edges.Where(e => e.VertexCount() == 3);

            // Für die Kanten mit 3 Knoten das Dictionary mit der Color füllen
            Dictionary<HyperEdge, int> color = threeSidedHyperEdges.ToDictionary(e => e, e => 0);

            // Für jede Kante die Vorgängerkante, sowie der Knoten über welchen die akutelle Kante erreicht wurde speichern.
            Dictionary<HyperEdge, Tuple<HyperEdge, Vertex>> parent = threeSidedHyperEdges.ToDictionary(e => e, e => new Tuple<HyperEdge, Vertex>(null, null));

            // Liste mit den Zyklen erstellen
            List<List<Tuple<HyperEdge, Vertex>>> cycles = new List<List<Tuple<HyperEdge, Vertex>>>();

            this.GetSimpleCycle(threeSidedHyperEdges.First(), null, new List<Tuple<HyperEdge, Vertex>>(), color, cycles);

            //foreach (HyperEdge edge in threeSidedHyperEdges)
            //{
            //    foreach (Vertex vertex in edge.Vertices)
            //    {
            //        this.GetSimpleCycle(edge, vertex, new Dictionary<HyperEdge, Tuple<HyperEdge, Vertex>>(), color, cycles);
            //    }
            //}


            return cycles;
        }

        /// <summary>
        /// Werte wie Colors, Parent etc. als value, nicht referenz übergben?!
        /// Cycles als Referenz übergben.
        /// </summary>
        public void GetSimpleCycle(
            HyperEdge u,
            HyperEdge p,
            List<Tuple<HyperEdge, Vertex>> parent,
            Dictionary<HyperEdge, int> color,
            List<List<Tuple<HyperEdge, Vertex>>> cycles)
        {
            // edge was completly visited before
            //if (color.ContainsKey(u) && color[u] == 2)
            //{
            //    return;
            //}

            IEnumerable<HyperEdge> threeSidedParents = parent.Select(x => x.Item1).Where(e => e.IsThreeSidedEdge());

            //Hier greift if noch nicht an der richtigen stelle, da immer noch drei kanten doppelt vorkommen. Warum?
            if (u.IsThreeSidedEdge() && threeSidedParents.Count() >= 6 && parent.First().Item1 != u)
            {
                return;
            }

            // possible cycle detected
            // if (color.ContainsKey(u) && color[u] == 1)
            if (u.IsThreeSidedEdge() && threeSidedParents.Count() == 6 && u.EqualsOnEdgeBasis(parent.First().Item1))
            {

                //// Ein Zyklus im Triominograph muss immer min. die Länge 6 (nur 3er Kanten) bzw. 12 (inkl. 2er Kanten haben)
                //int cycleBegin = parent.Count - 12;
                //if (cycleBegin >= 0)
                //{
                //    // Suche den Anfang des Zyklus (soweit vorhanden)
                //    while (parent[cycleBegin].Item1 != u && cycleBegin > 0)
                //    {
                //        cycleBegin--;
                //    }

                int cycleBegin = 0;
                // Wenn anfang gefunden wurde, den Zyklus speichern.
                if (parent[cycleBegin].Item1.Equals(u))
                {
                    List<Tuple<HyperEdge, Vertex>> cycle = new List<Tuple<HyperEdge, Vertex>>();
                    for (int i = cycleBegin; i < parent.Count; i++)
                    {
                        cycle.Add(parent[i]);
                    }

                    cycles.Add(cycle);
                }
                //}
            }

            //if (color.ContainsKey(u))
            //{
            //    color[u] = 1;
            //}

            // Den Knoten ermitteln, über welchen die Kante u erreicht wurde.
            IEnumerable<Vertex> adjacentVertices = null;
            if (p != null)
            {
                //die Knoten der Kante u ermitteln, die nicht dem eingehenden Knoten entsprechen(diese müssen noch besucht werden)
                adjacentVertices = u.GetNeighborVertices(parent.Last().Item2);
            }
            else
            {
                // Beim ersten aufruf sind noch keine Parentknoten vorhanden, und jeder Knoten der ersten Kante muss besucht werden.
                adjacentVertices = u.Vertices;
            }

            // Die Anzahl der Knoten welche die nächste Kante besitzen muss 
            // ermitteln (Kanten mit 2 und 3 Knoten werden immer im Wechsel durchlaufen).
            int vCount = (u.VertexCount() == 2) ? 3 : 2;

            // die weiteren Knoten der Kante u besuchen 
            foreach (Vertex vertex in adjacentVertices)
            {
                // Alle möglichen Kanten von dem aktuellen Knoten aus ermitteln und die bisherigen Parents ausschließen
                IEnumerable<HyperEdge> edges = this.GetAdjacentEdges(vertex, vCount);
                if (threeSidedParents.Count() >= 5)
                {
                    edges = edges.Except(threeSidedParents.ToList().GetRange(threeSidedParents.Count() - 4, 4));
                }
                else
                {
                    edges = edges.Except(threeSidedParents);
                }

                //// (und dann die 5 zuletzt besuchten Kanten wieder entfernen) da erst ab 6 ein Cycle möglich ist.
                //IEnumerable<HyperEdge> edges = this.GetAdjacentEdges(vertex, vCount);
                //IEnumerable<HyperEdge> threeSidedParents = parent.Where(x => x.Item1.IsThreeSidedEdge()).Select(x => x.Item1);
                //if (threeSidedParents.Count() > 5)
                //{
                //    threeSidedParents = threeSidedParents.ToList().GetRange(threeSidedParents.Count() - 5, 5);
                //}
                //edges = edges.Except(threeSidedParents);

                foreach (HyperEdge edge in edges)
                {
                    // List<Tuple<HyperEdge, Vertex>> newParent = u.IsThreeSidedEdge() ? new List<Tuple<HyperEdge, Vertex>>(parent) : parent;
                    List<Tuple<HyperEdge, Vertex>> newParent = new List<Tuple<HyperEdge, Vertex>>(parent);
                    // Parent Kante und Knoten für die nächste Kante hinzfügen.
                    newParent.Add(new Tuple<HyperEdge, Vertex>(u, edge.GetEdgeVertexInstance(vertex)));

                    string edgeName = u.ToString();
                    string vertexName = vertex.ToString();

                    this.GetSimpleCycle(edge, u, newParent, color, cycles);
                }
            }

            //if (color.ContainsKey(u))
            //{
            //    color[u] = 2;
            //}
        }
    }
}
