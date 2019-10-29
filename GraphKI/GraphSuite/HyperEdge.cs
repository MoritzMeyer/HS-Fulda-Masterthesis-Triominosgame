using GraphKI.Extensions;
using GraphKI.GameManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphKI.GraphSuite
{
    public class HyperEdge
    {
        #region fields
        /// <summary>
        /// All vertices within this edge.
        /// </summary>
        public List<Vertex> Vertices { get; }

        /// <summary>
        /// Orientation of Tile in TriominoGame.
        /// </summary>
        public TileOrientation Orientation { get; set; }

        public List<HyperEdge> DirectNeighbors { get; private set; }

        /// <summary>
        /// Dictionary which stores the isVisited property for each vertex.
        /// </summary>
        private Dictionary<Vertex, bool> VertexIsVisited { get; set; }
        #endregion

        #region ctor
        /// <summary>
        /// Intantiates a new Object of this class.
        /// </summary>
        /// <param name="vertex1">vertex1</param>
        /// <param name="vertex2">vertex2</param>
        /// <param name="vertex3">vertex3 (can be null, if edge has only two vertices)</param>
        public HyperEdge(string vertex1, string vertex2, string vertex3 = null, TileOrientation orientation = TileOrientation.None)
            : this(new Vertex(vertex1), new Vertex(vertex2), (vertex3 != null) ? new Vertex(vertex3) : null, orientation)
        {
        }

        /// <summary>
        /// Intantiates a new Object of this class.
        /// </summary>
        /// <param name="vertex1">vertex1</param>
        /// <param name="vertex2">vertex2</param>
        /// <param name="vertex3">vertex3 (can be null, if edge has only two vertices)</param>
        public HyperEdge(Vertex vertex1, Vertex vertex2, Vertex vertex3 = null, TileOrientation orientation = TileOrientation.None)
        {
            if (vertex1 == null)
            {
                throw new ArgumentException("Vertex1 cannot be null");
            }

            if (vertex2 == null)
            {
                throw new ArgumentException("Vertex2 cannot be null");
            }

            Vertex v1 = Vertex.CreateFromVertex(vertex1);
            Vertex v2 = Vertex.CreateFromVertex(vertex2);

            this.Vertices = new List<Vertex>() { v1, v2 };

            if (vertex3 != null)
            {
                Vertex v3 = Vertex.CreateFromVertex(vertex3);
                this.Vertices.Add(v3);
            }

            this.VertexIsVisited = Enumerable
                .Range(0, this.Vertices.Count)
                .ToDictionary(i => this.Vertices[i], i => false);

            this.Orientation = orientation;
            this.DirectNeighbors = new List<HyperEdge>();
        }
        #endregion

        #region IsThreeSidedEdge
        /// <summary>
        /// Returns true, if this edge connects three vertices.
        /// </summary>
        /// <returns>Three if it is three sided, false if not.</returns>
        public bool IsThreeSidedEdge()
        {
            return this.Vertices.Count == 3;
        }
        #endregion

        #region IsTwoSidedEdge
        /// <summary>
        /// Returns true, if this edge connects two vertices.
        /// </summary>
        /// <returns>True if it is two sided, false if not.</returns>
        public bool IsTwoSidedEdge()
        {
            return this.Vertices.Count == 2;
        }
        #endregion

        #region VertexCount
        /// <summary>
        /// Number of vertices within this edge.
        /// </summary>
        /// <returns>Number of vertices within this edge.</returns>
        public int VertexCount()
        {
            return this.Vertices.Count;
        }
        #endregion

        #region IsVisited
        /// <summary>
        /// Verifys, if all vertices where visited.
        /// </summary>
        /// <returns>True if all verticies where visited, false if not.</returns>
        public bool IsVisited()
        {
            return !this.VertexIsVisited.Values.Where(v => !v).Any();
        }
        #endregion

        #region VisitVertex
        /// <summary>
        /// Sets the value for vertex in 'VertexIsVisited' to true
        /// </summary>
        /// <param name="vertex">vertex to be visited.</param>
        public void VisitVertex(Vertex vertex)
        {
            if (!this.Vertices.Contains(vertex))
            {
                new ArgumentException("Diese Kante enthält keinen Knoten " + vertex);
            }

            this.VertexIsVisited[vertex] = true;
        }
        #endregion

        #region ContainsOnVertexBasis
        /// <summary>
        /// Verifies, if this edge contains a vertex (on VertexBasis, EdgeGuid for vertices will not be considered).
        /// </summary>
        /// <param name="vertex">vertex to be checked</param>
        /// <returns>True if this edge contains vertex.</returns>
        public bool ContainsOnVertexBasis(Vertex vertex)
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
        #endregion

        #region ContainsVertexOnValueBasis
        /// <summary>
        /// Verifies, if this edge contains a vertex (on ValueBasis).
        /// </summary>
        /// <param name="vertex">vertex to be checked</param>
        /// <returns>True if this edge contains vertex.</returns>
        public bool ContainsVertexOnValueBasis(Vertex vertex)
        {
            foreach (Vertex thisVertex in this.Vertices)
            {
                if (thisVertex.EqualsOnValueBasis(vertex))
                {
                    return true;
                }
            }

            return false;
        }
        #endregion

        #region ContainsOnEdgeBasis
        /// <summary>
        /// Verifies, if this edge contains a vertex (on EdgeBasis, EdgeGuid for vertices will be considered).
        /// </summary>
        /// <param name="vertex">vertex to be checked</param>
        /// <returns>True if this edge contains vertex.</returns>
        public bool ContainsOnEdgeBasis(Vertex vertex)
        {
            foreach (Vertex thisVertex in this.Vertices)
            {
                if (thisVertex.EqualsOnEdgeBasis(vertex))
                {
                    return true;
                }
            }

            return false;
        }
        #endregion

        #region GetNeighborVertices
        /// <summary>
        /// Determines the neighbor vertices for one vertex within this edge.
        /// </summary>
        /// <param name="vertex">Vertex for which the neighbors should be determined.</param>
        /// <returns>Neighbor vertices</returns>
        public Vertex[] GetNeighborVertices(Vertex vertex)
        {
            if (!this.ContainsOnEdgeBasis(vertex))
            {
                throw new ArgumentException($"This edge does'nt contains a vertex {vertex}.");
            }

            return this.Vertices.Where(v => !v.EqualsOnEdgeBasis(vertex)).ToArray();
        }
        #endregion

        #region AddDirectNeighbor
        /// <summary>
        ///  Adds a direct neigbor for this edge.
        /// </summary>
        /// <param name="edge"></param>
        /// <returns>True if neighbor could be added, false if not.</returns>
        public bool AddDirectNeighbor(HyperEdge edge)
        {
            if (!edge.IsThreeSidedEdge())
            {
                throw new ArgumentException($"Only three-sided edges are allowed as DirectNeighbors. '{edge}' isn't three sided.");
            }

            if (this.DirectNeighbors.Contains(edge))
            {
                throw new ArgumentException($"'{edge}' is already set as direct neighbor.");
            }

            if (this.DirectNeighbors.Count() >= 3)
            {
                throw new ArgumentException($"This edge contains already three direct neighbors.");
            }

            this.DirectNeighbors.Add(edge);
            return true;
        }
        #endregion

        public Vertex GetEdgeVertexInstance(Vertex otherVertexInstance)
        {
            Vertex thisVertexInstance = this.Vertices.Where(v => v.EqualsOnValueBasis(otherVertexInstance)).First();
            //thisVertexInstance.VisitVertex();

            return thisVertexInstance;
        }

        public Vertex GetVertexOnSpecificSide(TileFace tileFace)
        {
            switch(tileFace)
            {
                case TileFace.Right:
                    return this.Vertices[0];
                case TileFace.Bottom:
                    return this.Vertices[1];
                case TileFace.Left:
                    return this.Vertices[2];
            }

            throw new ArgumentException($"No vertex for Side '{tileFace}'.");
        }

        internal TileFace GetTileFaceFromVertex(Vertex vertex)
        {
            if (!this.IsThreeSidedEdge())
            {
                throw new ArgumentException("Only for three-sided-edges the TileFace can be determined.");
            }

            List<char> edgeChars = new List<char>();
            foreach (Vertex v in this.Vertices)
            {
                edgeChars.Add(v.Value[0]);
            }

            string firstSide = edgeChars[0] + "" + edgeChars[1];
            string secondSide = edgeChars[1] + "" + edgeChars[2];
            string thirdSide = edgeChars[2] + "" + edgeChars[0];

            if (vertex.Value.Equals(firstSide))
            {
                return TileFace.Right;
            }

            if (vertex.Value.Equals(secondSide))
            {
                return TileFace.Bottom;
            }

            if (vertex.Value.Equals(thirdSide))
            {
                return TileFace.Left;
            }

            throw new Exception($"Could not determine the TileFace for Edge '{this}'");
        }

        #region EqualsOnVertexBasis
        /// <summary>
        /// Verifies, if two HyperEdges are really the same (Vertex comparision on VertexBasis).
        /// </summary>
        /// <param name="obj">other edge</param>
        /// <returns>True, if they are the same, false if not</returns>
        public bool EqualsOnVertexBasis(object obj)
        {
            if (obj == null || !obj.GetType().Equals(this.GetType()))
            {
                return false;
            }

            HyperEdge other = (HyperEdge)obj;

            if (other.VertexCount() != this.VertexCount() || other.IsVisited() != this.IsVisited())
            {
                return false;
            }

            foreach(Vertex otherVertex in other.Vertices)
            {
                bool hasCounterPartInThis = false;
                foreach(Vertex thisVertex in this.Vertices)
                {
                    if (thisVertex.EqualsOnVertexBasis(otherVertex))
                    {
                        hasCounterPartInThis = true;
                    }
                }

                if (!hasCounterPartInThis)
                {
                    return false;
                }
            }

            return true;
        }
        #endregion

        #region EqualsOnEdgeBasis
        /// <summary>
        /// Verifies, if two HyperEdges are really the same (Vertex comparision on EdgeBasis).
        /// </summary>
        /// <param name="obj">other edge</param>
        /// <returns>True, if they are the same, false if not</returns>
        public bool EqualsOnEdgeBasis(object obj)
        {
            return this.Equals(obj);
        }
        #endregion

        #region EqualsOnValueBasis
        /// <summary>
        /// /// Verifies, if two HyperEdges are the same (Vertex comparision on Valu basis).
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public bool EqualsOnValueBasis(object obj)
        {
            if (obj == null || !obj.GetType().Equals(this.GetType()))
            {
                return false;
            }

            HyperEdge other = (HyperEdge)obj;

            if (other.VertexCount() != this.VertexCount() || other.IsVisited() != this.IsVisited())
            {
                return false;
            }

            IEnumerable<string> thisVertices = this.Vertices.Select(v => v.Value);
            IEnumerable<string> otherVertices = other.Vertices.Select(v => v.Value);

            return !thisVertices.Except(otherVertices).Union(otherVertices.Except(thisVertices)).Any();
        }
        #endregion

        #region Equals
        /// <summary>
        /// Überschreibt die Equals Methode für diese Klasse.
        /// </summary>
        /// <param name="obj">Das Objekt mit dem verglichen werden soll.</param>
        /// <returns>True, wenn das Objekt und diese Instanz der Klasse gleich sind, false wenn nicht.</returns>
        public override bool Equals(object obj)
        {
            if (obj == null || !obj.GetType().Equals(this.GetType()))
            {
                return false;
            }

            HyperEdge other = (HyperEdge)obj;

            if (other.VertexCount() != this.VertexCount() || other.IsVisited() != this.IsVisited())
            {
                return false;
            }

            foreach (Vertex otherVertex in other.Vertices)
            {
                bool hasCounterPartInThis = false;
                foreach (Vertex thisVertex in this.Vertices)
                {
                    if (thisVertex.EqualsOnEdgeBasis(otherVertex))
                    {
                        hasCounterPartInThis = true;
                    }
                }

                if (!hasCounterPartInThis)
                {
                    return false;
                }
            }

            return true;
        }
        #endregion

        #region GetHashCode
        /// <summary>
        /// Überschreibt die Berechnung des HashCodes für diese Klasse.
        /// </summary>
        /// <returns>Den HashCode.</returns>
        public override int GetHashCode()
        {
            return this.VertexCount().GetHashCode() ^ this.Vertices.GetHashCode() ^ this.IsVisited().GetHashCode();
        }
        #endregion        

        #region ToString
        /// <summary>
        /// Overrides the toString mehtod for this class.
        /// </summary>
        /// <returns>String representation of this class.</returns>
        public override string ToString()
        {
            return "-" + this.Vertices.Select(v => v.Value).Aggregate((a, b) => a + "-" + b) + "->";
        }
        #endregion
    }
}
