using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GraphSuite
{
    public class HyperEdge
    {
        #region ctor
        /// <summary>
        /// Erzeugt eine neue Instanz der Klasse.
        /// </summary>
        /// <param name="vertices">Aufzählung von Knoten.</param>
        public HyperEdge(IEnumerable<string> vertices)
        {
            this.Vertices = vertices.OrderBy(x => x).ToList();
            for (int i = 0; i < this.Vertices.Count(); i++)
            {
                this.Vertices[i] = this.Vertices[i].Insert(i, "_");
            }
            this.VertexIsVisited = 
                Enumerable
                    .Range(0, this.Vertices.Count)
                    .ToDictionary(i => this.Vertices[i], i => false);
        }
        #endregion

        #region VertexCount
        /// <summary>
        /// Liefert die Anzahl an Knoten, welche diese Kante verbindet
        /// </summary>
        public int VertexCount
        {
            get
            {
                return this.Vertices.Count();
            }
        }
        #endregion

        #region Vertices
        /// <summary>
        /// Die Knoten, welche diese Kante miteinander verbindet.
        /// </summary>
        public List<string> Vertices { get; set; }
        #endregion

        #region IsVisited
        /// <summary>
        /// Liefert/setzt die 'IsVisited' Eigenschaft.
        /// </summary>
        public bool IsVisited { get; set; }
        #endregion

        #region VertexIsVisited
        /// <summary>
        /// Array, welches die visited Eigenschaften für jeden Knoten der Kante enthält.
        /// </summary>
        public Dictionary<string, bool> VertexIsVisited { get; private set; }
        #endregion

        #region IsCompletlyVisited
        /// <summary>
        /// Gibt an, ob jeder Knoten dieser Kante bereits besucht wurde.
        /// </summary>
        public bool IsCompletlyVisited
        {
            get
            {
                return this.VertexIsVisited.Where(v => v.Value).Count() == this.VertexIsVisited.Count();
            }
        }
        #endregion

        #region VisitVertex
        /// <summary>
        /// Setzt die isVisited Eigenschaft für einen bestimmten Knoten dieser Kante auf true.
        /// </summary>
        /// <param name="vertex">Der Knoten dessen isVisited Eigenschaft gestzt werden soll.</param>
        public void VisitVertex(string vertex)
        {
            if (!this.Vertices.Contains(vertex))
            {
                new ArgumentException("Diese Kante enthält keinen Knoten " + vertex);
            }

            this.VertexIsVisited[vertex] = true;
        }
        #endregion

        #region Contains
        /// <summary>
        /// Überprüft, ob die Kante einen bestimmten Knoten enthält
        /// (Dazu werden die Positionsmarkierungen (_) der Knoten vorher entfernt)
        /// </summary>
        /// <param name="vertex"></param>
        /// <returns></returns>
        public bool Contains(string vertex)
        {
            string vertexValue = vertex.GetVertexValue();
            foreach(string v in this.Vertices)
            {
                if (v.GetVertexValue() == vertexValue)
                {
                    return true;
                }
            }

            return false;
        }
        #endregion

        #region GetNeighborVertices
        /// <summary>
        /// Ermittelt für einen Knoten alle weiteren Knoten, 
        /// welche diese Kante mit ersterem verbindet
        /// </summary>
        /// <param name="vertex">Der Knoten zu dem die Nachbarn gesucht werden.</param>
        /// <returns>Die Nachbarknoten.</returns>
        public string[] GetNeighborVertices(string vertex)
        {
            return this.Vertices.Where(v => !v.Equals(vertex)).ToArray();
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

            if (other.VertexCount != this.VertexCount || other.IsVisited != this.IsVisited)
            {
                return false;
            }

            for (int i = 0; i < this.VertexCount; i++)
            {
                if (!other.Vertices.Contains(this.Vertices[i]) || !this.Vertices.Contains(other.Vertices[i]))
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
            return this.VertexCount.GetHashCode() ^ this.Vertices.GetHashCode() ^ this.IsVisited.GetHashCode();
        }
        #endregion

        
        public string GetVertexEdgeValue(string vertex)
        {
            return this.Vertices.Where(v => v.GetVertexValue().Equals(vertex.GetVertexValue())).FirstOrDefault();
        }

        public override string ToString()
        {
            return "-" + this.Vertices.Aggregate((a, b) => a.GetVertexValue() + "-" + b.GetVertexValue()) + "->";
        }
    }
}
