using System;

namespace GraphKI.GraphSuite
{
    public class Vertex
    {
        #region fields
        /// <summary>
        /// Value of this vertex
        /// </summary>
        public string Value { get; private set; }

        /// <summary>
        /// unique guid for this vertex (to copmare on vertex basis).
        /// </summary>
        public readonly Guid VertexGuid;

        /// <summary>
        /// unique guid for comparision on edge basis (even vertices with same value must be seen as different)
        /// </summary>
        public readonly Guid EdgeGuid;
        #endregion

        #region ctor
        /// <summary>
        /// Instantiates new Object for this class.
        /// </summary>
        /// <param name="value">Value of this vertex.</param>
        public Vertex(string value)
            : this(value, Guid.NewGuid())
        {
        }

        /// <summary>
        /// Instantiates new Object for this class.
        /// </summary>
        /// <param name="value">Value of this vertex.</param>
        /// <param name="vertexGuid">Guid to compare on vertex basis.</param>
        public Vertex(string value, Guid vertexGuid)
        {
            this.Value = value;
            this.VertexGuid = vertexGuid;
            this.EdgeGuid = Guid.NewGuid();
        }
        #endregion

        #region Equals
        /// <summary>
        /// Overrides equals-method for this class.
        /// </summary>
        /// <param name="obj">The other object to compare with.</param>
        /// <returns>True if other object and this are equal, false if not.</returns>
        public override bool Equals(object obj)
        {
            if (obj == null || !obj.GetType().Equals(this.GetType()))
            {
                return false;
            }

            Vertex other = (Vertex)obj;

            if (other.VertexGuid != this.VertexGuid || other.EdgeGuid != this.EdgeGuid || other.Value != this.Value)
            {
                return false;
            }

            return true;
        }
        #endregion

        #region EqualsOnVertexBasis
        /// <summary>
        /// Compares two vertices on vertex basis (the edge guid of both could be different)
        /// </summary>
        /// <param name="obj">other vertex.</param>
        /// <returns>true if value and vertex guid are equal, false if not.</returns>
        public bool EqualsOnVertexBasis(object obj)
        {
            if (obj == null || !obj.GetType().Equals(this.GetType()))
            {
                return false;
            }

            Vertex other = (Vertex)obj;

            if (other.VertexGuid != this.VertexGuid || other.Value != this.Value)
            {
                return false;
            }

            return true;
        }
        #endregion

        #region EqualsOnEdgeBasis
        /// <summary>
        /// Compares two verticves on edge basis (value, edgeguid and vertexguid has to be the same)
        /// </summary>
        /// <param name="obj">other vertex</param>
        /// <returns>true, if value, edgeguid and vertex guid are equal, false if not.</returns>
        public bool EqualsOnEdgeBasis(object obj)
        {
            return this.Equals(obj);
        }
        #endregion

        #region EqualsOnValueBasis
        /// <summary>
        /// Compares two vertices on value basis (only values of both verices are compared)
        /// </summary>
        /// <param name="obj">other vertex.</param>
        /// <returns>true if values are equal, false if not.</returns>
        public bool EqualsOnValueBasis(object obj)
        {
            if (obj == null || !obj.GetType().Equals(this.GetType()))
            {
                return false;
            }

            Vertex other = (Vertex)obj;

            if (other.Value != this.Value)
            {
                return false;
            }

            return true;
        }
        #endregion

        #region CreateFromVertex
        /// <summary>
        /// Creates a new Vertex on the basis of another vertex,
        /// so that value and vertexGuid of the new one are the same.
        /// </summary>
        /// <param name="otherVertex">the other vertex</param>
        /// <returns>the new vertex</returns>
        public static Vertex CreateFromVertex(Vertex otherVertex)
        {
            Vertex vertex = new Vertex(otherVertex.Value, otherVertex.VertexGuid);
            return vertex;
        }
        #endregion

        #region GetHashCode
        /// <summary>
        /// Overrides method for generating HashCode.
        /// </summary>
        /// <returns>The HashCode.</returns>
        public override int GetHashCode()
        {
            return EdgeGuid.GetHashCode() ^ VertexGuid.GetHashCode() ^ Value.GetHashCode();
        }
        #endregion

        #region ToString
        /// <summary>
        /// Overrides the toString mehtod for this class.
        /// </summary>
        /// <returns>String representation of this class.</returns>
        public override string ToString()
        {
            return this.Value + " (VertexGuid:'" + this.VertexGuid.ToString() + "', EdgeGuid:'" + this.EdgeGuid + "')";
        }
        #endregion
    }
}
