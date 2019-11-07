using GraphKI.Extensions;
using GraphKI.GameManagement;
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
            this.Edges = new List<HyperEdge>();
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
        /// Adds an edge to this graph (it creates a new Instance of edge, based on the graphs vertices).
        /// </summary>
        /// <param name="edge">Edge to be added.</param>
        /// <returns>The new edge.</returns>
        public HyperEdge AddEdge(HyperEdge edge)
        {
            if (edge.IsTwoSidedEdge())
            {
                return this.AddEdge(edge.Vertices[0], edge.Vertices[1]);
            }

            return this.AddEdge(edge.Vertices[0], edge.Vertices[1], edge.Vertices[2], edge.Orientation);
        }

        /// <summary>
        /// Adds a new Edge to this graph, if it isn't already part of it.
        /// </summary>
        /// <param name="vertex1">Vertex1 of the edge to be added.</param>
        /// <param name="vertex2">Vertex2 of the edge to be added.</param>
        /// <param name="vertex3">Vertex3 of the edge to be added.</param>
        /// <returns></returns>
        public HyperEdge AddEdge(string vertex1, string vertex2, string vertex3 = null, TileOrientation tileOrientation = TileOrientation.None)
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

            HyperEdge edge = new HyperEdge(v1, v2, v3, tileOrientation);

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
        public HyperEdge AddEdge(Vertex vertex1, Vertex vertex2, Vertex vertex3 = null, TileOrientation tileOrientation = TileOrientation.None)
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

            HyperEdge edge = new HyperEdge(v1, v2, v3, tileOrientation);

            if (this.HasEdge(edge))
            {
                throw new ArgumentException($"This Graph has already an edge '{edge}'.");
            }

            this.Edges.Add(edge);
            return edge;
        }
        #endregion

        #region AddEdgeWithDirectNeighbor
        /// <summary>
        /// Adds an Edge to this graph and sets a directNeighbor for this edge.
        /// </summary>
        /// <param name="vertex1">First vertex of new Edge.</param>
        /// <param name="vertex2">Second vertex of new Edge.</param>
        /// <param name="vertex3">Third vertex of new Edge.</param>
        /// <param name="directNeighbor">The direct Neighbor of new edge.</param>
        /// <param name="edge">The added edge.</param>
        /// <returns>True if edge with neighbor could be added, false if not.</returns>
        public bool AddEdgeWithDirectNeighbor(HyperEdge edge, HyperEdge directNeighbor, out HyperEdge addedEdge)
        {
            return this.AddEdgeWithDirectNeighbors(edge, new List<HyperEdge>() { directNeighbor }, out addedEdge);
        }
        #endregion

        public bool AddEdgeWithDirectNeighbors(HyperEdge edge, IEnumerable<HyperEdge> directNeighbors, out HyperEdge addedEdge)
        {
            addedEdge = null;
            List<HyperEdge> directNeighborInstances = new List<HyperEdge>();
            foreach (HyperEdge directNeighbor in directNeighbors)
            {
                if (!directNeighbor.IsThreeSidedEdge())
                {
                    throw new ArgumentException($"DirectNeighbor has to be three-sided ('{directNeighbor}' isn't.");
                }

                if (!this.HasEdge(directNeighbor))
                {
                    return false;
                }

                if (!this.TryGetEdge(out HyperEdge directNeighborInstance, directNeighbor.Vertices[0], directNeighbor.Vertices[1], directNeighbor.Vertices[2]))
                {
                    throw new ArgumentException($"Could not get Graph-Edge-Instance of '{directNeighbor}', although it exists within this graph.");
                }

                directNeighborInstances.Add(directNeighborInstance);
            }
            

            addedEdge = this.AddEdge(edge);

            foreach (HyperEdge directNeighborInstance in directNeighborInstances)
            {
                addedEdge.AddDirectNeighbor(directNeighborInstance);
                directNeighborInstance.AddDirectNeighbor(addedEdge);
            }            

            return true;
        }

        #region TryGetEdge
        /// <summary>
        /// Returns an edge from this graph with specific vertices, if existing.
        /// </summary>
        /// <param name="matchingEdge">The edge with specific vertices (null if none was existing)</param>
        /// <param name="vertex1">First vertex of edge.</param>
        /// <param name="vertex2">Second vertex of edge.</param>
        /// <param name="vertex3">Third vertex of edge (null if edge is twosided)</param>
        /// <returns>True, if edge could be found, false if not.</returns>
        public bool TryGetEdge(out HyperEdge matchingEdge, string vertex1, string vertex2, string vertex3 = null)
        {
            if (vertex3 == null)
            {
                return this.TryGetEdge(out matchingEdge, new Vertex(vertex1), new Vertex(vertex2));
            }

            return this.TryGetEdge(out matchingEdge, new Vertex(vertex1), new Vertex(vertex2), new Vertex(vertex3));
        }

        /// <summary>
        /// Returns an edge from this graph with specific vertices, if existing.
        /// </summary>
        /// <param name="matchingEdge">The edge with specific vertices (null if none was existing)</param>
        /// <param name="vertex1">First vertex of edge.</param>
        /// <param name="vertex2">Second vertex of edge.</param>
        /// <param name="vertex3">Third vertex of edge (null if edge is twosided)</param>
        /// <returns>True, if edge could be found, false if not.</returns>
        public bool TryGetEdge(out HyperEdge matchingEdge, Vertex vertex1, Vertex vertex2, Vertex vertex3 = null)
        {
            IEnumerable<HyperEdge> possibleEdges = (vertex3 == null) ? this.Edges.Where(e => e.IsTwoSidedEdge()) : this.Edges.Where(e => e.IsThreeSidedEdge());

            matchingEdge = null;
            IEnumerable<HyperEdge> matchingEdges = null;
            if (vertex3 == null)
            {
                matchingEdges = possibleEdges.Where(e => e.Vertices[0].EqualsOnValueBasis(vertex1) && e.Vertices[1].EqualsOnValueBasis(vertex2));
            }
            else
            {
                matchingEdges = possibleEdges.Where(e => e.Vertices[0].EqualsOnValueBasis(vertex1) && e.Vertices[1].EqualsOnValueBasis(vertex2) && e.Vertices[2].EqualsOnValueBasis(vertex3));
            }

            if (matchingEdges.Count() > 1)
            {
                throw new ArgumentException($"More than one Edge with Vertices ({vertex1}, {vertex2}, {vertex3}) where found.");
            }

            if (matchingEdges.Count() < 1)
            {
                return false;
            }

            // At this point it is clear, that only one edge was found.
            matchingEdge = matchingEdges.First();
            return true;
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

            // Für jede Kante die Vorgängerkante, sowie der Knoten über welchen die akutelle Kante erreicht wurde speichern.
            Dictionary<HyperEdge, Tuple<HyperEdge, Vertex>> parent = threeSidedHyperEdges.ToDictionary(e => e, e => new Tuple<HyperEdge, Vertex>(null, null));

            // Liste mit den Zyklen erstellen
            List<List<Tuple<HyperEdge, Vertex>>> cycles = new List<List<Tuple<HyperEdge, Vertex>>>();

            this.GetSimpleCycle(threeSidedHyperEdges.First(), null, new List<Tuple<HyperEdge, Vertex>>(), new List<Tuple<HyperEdge, TileOrientation>>(), cycles);

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
            List<Tuple<HyperEdge, TileOrientation>> orientations,
            List<List<Tuple<HyperEdge, Vertex>>> cycles)
        {
            IEnumerable<TileOrientation> threeSidedOrientations = orientations.Where(x => x.Item1.IsThreeSidedEdge()).Select(x => x.Item2);
            if (threeSidedOrientations.Distinct().Count() != threeSidedOrientations.Count())
            {
                return;
            }

            IEnumerable<HyperEdge> threeSidedParents = parent.Select(x => x.Item1).Where(e => e.IsThreeSidedEdge());

            //Hier greift if noch nicht an der richtigen stelle, da immer noch drei kanten doppelt vorkommen. Warum?
            if (u.IsThreeSidedEdge() && threeSidedParents.Count() >= 6 && parent.First().Item1 != u)
            {
                return;
            }

            // possible cycle detected
            if (u.IsThreeSidedEdge() && threeSidedParents.Count() == 6 && u.EqualsOnEdgeBasis(parent.First().Item1))
            {
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
            }

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

                foreach (HyperEdge edge in edges)
                {
                    // List<Tuple<HyperEdge, Vertex>> newParent = u.IsThreeSidedEdge() ? new List<Tuple<HyperEdge, Vertex>>(parent) : parent;
                    List<Tuple<HyperEdge, Vertex>> newParent = new List<Tuple<HyperEdge, Vertex>>(parent);
                    // Parent Kante und Knoten für die nächste Kante hinzfügen.
                    newParent.Add(new Tuple<HyperEdge, Vertex>(u, edge.GetEdgeVertexInstance(vertex)));

                    List<Tuple<HyperEdge, TileOrientation>> newOrientations = new List<Tuple<HyperEdge, TileOrientation>>(orientations);
                    TileOrientation actualOrientation = TileOrientation.None;

                    if (u.IsThreeSidedEdge())
                    {
                        actualOrientation = TileOrientation.Straight;

                        if (threeSidedParents.Count() > 0)
                        {
                            Tuple<HyperEdge, Vertex> lastThreeSidedParent = parent.Where(x => x.Item1.IsThreeSidedEdge()).Last();
                            TileFace previousFace = lastThreeSidedParent.Item1.GetTileFaceFromVertex(lastThreeSidedParent.Item2);
                            TileFace actualFace = u.GetTileFaceFromVertex(parent.Last().Item2);

                            actualOrientation = GameBoard.GetTileOrienationFromOtherTileOrientationAndFaces(
                                orientations.Where(x => x.Item1.IsThreeSidedEdge()).Last().Item2,
                                previousFace,
                                actualFace);
                        }
                    }

                    newOrientations.Add(new Tuple<HyperEdge, TileOrientation>(u, actualOrientation));

                    string edgeName = u.ToString();
                    string vertexName = vertex.ToString();

                    this.GetSimpleCycle(edge, u, newParent, newOrientations, cycles);
                }
            }
        }

        public bool IsPartOfHexagon(HyperEdge edge, out List<Hexagon> hexagons)
        {
            if (!edge.IsThreeSidedEdge())
            {
                throw new ArgumentException($"Edge: '{edge}' is not three sided.");
            }

            if (!this.HasEdge(edge) || !this.TryGetEdge(out HyperEdge edgeGraphInstance, edge.Vertices[0], edge.Vertices[1], edge.Vertices[2]))
            {
                throw new ArgumentException($"This Graph does not conatain an edge '{edge}'");
            }

            hexagons = new List<Hexagon>();
            List<TileFace> faces = new List<TileFace>() { TileFace.Left, TileFace.Right, TileFace.Bottom };
            for (int i = 0; i < faces.Count; i++)
            {
                //if (this.TryBuildHexagon(edgeGraphInstance, faces[i], out Hexagon hexagon))
                //{
                //    hexagons.Add(hexagon);
                //}

                Hexagon hexagon = null;
                if (this.TryBuildHexagonRec(edgeGraphInstance, faces[i], ref hexagon))
                {
                    hexagons.Add(hexagon);
                }
            }

            if (hexagons.Count() < 1)
            {
                return false;
            }

            return true;
        }

        private bool TryBuildHexagon(HyperEdge edge, TileFace startingFace, out Hexagon hexagon)
        {
            Vertex outgoingVertex = edge.GetVertexOnSpecificSide(startingFace);
            HyperEdge outgoingConnector = this.Edges.Where(e => e.IsTwoSidedEdge() && e.ContainsVertexOnValueBasis(outgoingVertex)).First();

            hexagon = new Hexagon(edge, startingFace, outgoingConnector);
            if (edge.DirectNeighbors.Count < 1)
            {
                return false;
            }

            IEnumerable<HyperEdge> possibleNextTiles = edge.DirectNeighbors
                .Where(e => e.ContainsVertexOnValueBasis(outgoingConnector.GetNeighborVertices(outgoingConnector.GetEdgeVertexInstance(outgoingVertex)).First()));
                //.FirstOrDefault();

            if (possibleNextTiles.Count() < 1)
            {
                return false;
            }

            bool tileAdded = true;
            while (!hexagon.IsComplete && tileAdded)
            {
                tileAdded = false;
                foreach (HyperEdge possibleNextTile in possibleNextTiles)
                {
                    TileFace nextOutgoingFace = TileFace.None;
                    try
                    {
                        nextOutgoingFace = hexagon.GetOutgoingFace(hexagon.GetNextPointerValue(), possibleNextTile.Orientation);
                    }
                    catch(ArgumentException argEx)
                    {
                        continue;
                    }

                    if (!tileAdded && nextOutgoingFace != TileFace.None)
                    {
                        Vertex nextOutgoingVertex = possibleNextTile.GetVertexOnSpecificSide(nextOutgoingFace);
                        HyperEdge nextOutgiongConnector = this.Edges.Where(e => e.IsTwoSidedEdge() && e.ContainsVertexOnValueBasis(nextOutgoingVertex)).First();

                        if (hexagon.TryAddToHexagon(possibleNextTile, nextOutgiongConnector))
                        {
                            //HyperEdge previousTile = hexagon.Triominos[hexagon.GetPreviousPointerValue()];
                            List<HyperEdge> possibleTileNeigbors = new List<HyperEdge>();
                            foreach (HyperEdge neighbor in possibleNextTile.DirectNeighbors)
                            {
                                if (!hexagon.Triominos.Contains(neighbor))
                                {
                                    possibleTileNeigbors.Add(neighbor);
                                }
                            }

                            possibleNextTiles = possibleTileNeigbors
                                .Where(e => e.ContainsVertexOnValueBasis(nextOutgiongConnector.GetNeighborVertices(nextOutgiongConnector.GetEdgeVertexInstance(nextOutgoingVertex)).First()));

                            //possibleNextTiles = possibleNextTile.DirectNeighbors
                            //    .Where(e => !e.Equals(previousTile) && e.ContainsVertexOnValueBasis(nextOutgiongConnector.GetNeighborVertices(nextOutgiongConnector.GetEdgeVertexInstance(nextOutgoingVertex)).First()));
                            //.FirstOrDefault();
                            tileAdded = true;
                        }
                    }                    
                }
            }

            return hexagon.IsComplete;
        }



        public bool TryBuildHexagonRec(HyperEdge nextTile, TileFace nextOutgoingFace, ref Hexagon hexagon)
        {
            Vertex nextOutgoingVertex = nextTile.GetVertexOnSpecificSide(nextOutgoingFace);
            HyperEdge nextOutgoingConnector = this.Edges.Where(e => e.IsTwoSidedEdge() && e.ContainsVertexOnValueBasis(nextOutgoingVertex)).First();

            if (hexagon == null || hexagon.Triominos.Count < 1)
            {
                hexagon = new Hexagon(nextTile, nextOutgoingFace, nextOutgoingConnector);
            }
            else
            {
                if (!hexagon.TryAddToHexagon(nextTile, nextOutgoingConnector))
                {
                    return false;
                }

                if (hexagon.IsComplete)
                {
                    return true;
                }
            }

            List<HyperEdge> possibleTileNeigbors = new List<HyperEdge>();
            foreach (HyperEdge neighbor in nextTile.DirectNeighbors)
            {
                if (!hexagon.Triominos.Contains(neighbor))
                {
                    possibleTileNeigbors.Add(neighbor);
                }
            }

            IEnumerable<HyperEdge> possibleNextTiles = possibleTileNeigbors
                .Where(e => e.ContainsVertexOnValueBasis(nextOutgoingConnector.GetNeighborVertices(nextOutgoingConnector.GetEdgeVertexInstance(nextOutgoingVertex)).First()));

            foreach (HyperEdge possibleNextTile in possibleNextTiles)
            {
                TileFace nextPossibleOutgoingFace = TileFace.None;
                try
                {
                    nextPossibleOutgoingFace = hexagon.GetOutgoingFace(hexagon.GetNextPointerValue(), possibleNextTile.Orientation);
                }
                catch (ArgumentException argEx)
                {
                    continue;
                }

                Hexagon memHexagon = hexagon.DeepCopy();
                if (this.TryBuildHexagonRec(possibleNextTile, nextPossibleOutgoingFace, ref memHexagon))
                {
                    hexagon = memHexagon;
                    return true;
                }
            }

            return false;
        }
    }
}
