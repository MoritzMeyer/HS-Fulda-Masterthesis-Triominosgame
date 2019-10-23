using GraphKI.GameManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphKI.GraphSuite
{
    public class Hexagon
    {
        #region fields
        private List<HyperEdge> Triominos;
        private List<HyperEdge> Connectors;
        private List<TileFace> TriominoOutgoingFaces;
        internal int Pointer;
        #endregion

        #region ctor
        /// <summary>
        /// Creates a new Instance of this class.
        /// </summary>
        /// <param name="edge">the starting tile of this hexagon.</param>
        /// <param name="tileFace">the face of the starting tile for the next tile.</param>
        public Hexagon(HyperEdge edge, TileFace tileFace)
        {
            this.Triominos = new List<HyperEdge>(6);
            this.Connectors = new List<HyperEdge>(6);
            this.TriominoOutgoingFaces = new List<TileFace>(6);
            this.Pointer = this.DetermineHexagonPosition(edge.Orientation, tileFace);
        }
        #endregion

        #region DetermineHexagonPosition
        /// <summary>
        /// Determiens the possible position of a tile within a Hexagon, based
        /// on its orientation and the face on which the next tile can be placed
        /// to get a hexagon.
        /// </summary>
        /// <param name="orientation">Orientation of the tile.</param>
        /// <param name="tileFace">the face of the tile, on which the next tile can be placed, to get a hexagon.</param>
        /// <returns>The position in a possible hexagon.</returns>
        private int DetermineHexagonPosition(TileOrientation orientation, TileFace tileFace)
        {
            if ((orientation == TileOrientation.Straight && tileFace == TileFace.Right) ||
                (orientation == TileOrientation.DoubleTiltRight && tileFace == TileFace.Left) ||
                (orientation == TileOrientation.DoubleTiltLeft && tileFace == TileFace.Bottom))
            {
                return 0;
            }

            if ((orientation == TileOrientation.TiltLeft && tileFace == TileFace.Right) ||
                (orientation == TileOrientation.TiltRight && tileFace == TileFace.Left) ||
                (orientation == TileOrientation.Flipped && tileFace == TileFace.Bottom))
            {
                return 1;
            }

            if ((orientation == TileOrientation.Straight && tileFace == TileFace.Left) ||
                (orientation == TileOrientation.DoubleTiltRight && tileFace == TileFace.Bottom) ||
                (orientation == TileOrientation.DoubleTiltLeft && tileFace == TileFace.Right))
            {
                return 2;
            }

            if ((orientation == TileOrientation.TiltLeft && tileFace == TileFace.Left) ||
                (orientation == TileOrientation.TiltRight && tileFace == TileFace.Bottom) ||
                (orientation == TileOrientation.Flipped && tileFace == TileFace.Right))
            {
                return 3;
            }

            if ((orientation == TileOrientation.Straight && tileFace == TileFace.Bottom) ||
                (orientation == TileOrientation.DoubleTiltRight && tileFace == TileFace.Right) ||
                (orientation == TileOrientation.DoubleTiltLeft && tileFace == TileFace.Left))
            {
                return 4;
            }

            if ((orientation == TileOrientation.TiltLeft && tileFace == TileFace.Bottom) ||
                (orientation == TileOrientation.TiltRight && tileFace == TileFace.Right) ||
                (orientation == TileOrientation.Flipped && tileFace == TileFace.Left))
            {
                return 5;
            }

            throw new ArgumentException($"No HexagonPosition for Edge with Orientation '{orientation}' and TileFace '{tileFace}'");
        }
        #endregion

        public bool TryAddToHexagon(HyperEdge edge)
        {
            if (!edge.IsThreeSidedEdge())
            {
                throw new ArgumentException($"Edges added to a hexagon must be threesided ('{edge}' isn't).");
            }

            if (this.Triominos.Count != this.Connectors.Count)
            {
                throw new ArgumentException($"Bevor adding a new Triomino Edge to this hexagon a connecting edge is needed.");
            }

            // Check orientation for new edge according to the next free hexagon position.
            if (edge.Orientation.ToArrayTileOrientation() != this.GetDemandedOrientationForHexagonPosition(this.Pointer + 1))
            {
                return false;
            }

            HyperEdge lastHexagonTile = this.GetLastEdgeAdded();
            TileFace lastTileFace = this.GetLastEdgeOutgoingFace();
            TileFace incomingTileFace = this.GetIncomingFaceForNextHexagonPosition(edge.Orientation);

            if (!this.CheckIfLastConnectorConnectsInAndOutgoing(lastHexagonTile.GetVertexOnSpecificSide(lastTileFace), edge.GetVertexOnSpecificSide(incomingTileFace), this.Connectors[this.Pointer]))
            {
                return false;
            }

            this.Pointer++;
            this.Triominos[this.Pointer] = edge;
            this.TriominoOutgoingFaces[this.Pointer] = this.GetOutgoingFace(this.Pointer, edge.Orientation);


            // Get the last edge of this hexagon and it's face
            // get the face of the new edge according to the next free hexagon position
            // if all matches add to hexagon
            // increase pointer (6++ == 0)

            // write tests

            return true;
        }

        #region TryAddConnector
        /// <summary>
        /// Tries to add a connecting edge for the last triomino edge.
        /// </summary>
        /// <param name="connector">the connecting edge.</param>
        /// <returns>True, if connector could be added, false if not.</returns>
        public bool TryAddConnector(HyperEdge connector)
        {
            if (!connector.IsTwoSidedEdge())
            {
                throw new ArgumentException($"Only two sided edges are eligible as connector ('{connector}' isn't).");
                
            }

            if (this.Triominos.Count == this.Connectors.Count)
            {
                throw new ArgumentException("Bevor adding a connector you need to add a Triomino edge.");
            }

            Vertex outgoingVertex = this.Triominos[this.Pointer].GetVertexOnSpecificSide(this.TriominoOutgoingFaces[this.Pointer]);
            if (!connector.ContainsVertexOnValueBasis(outgoingVertex))
            {
                //throw new ArgumentException($"The ")
                return false;
            }

            this.Connectors[this.Pointer] = connector;
            return true;
        }
        #endregion

        #region GetDemandedOrientationForHexagonPosition
        /// <summary>
        /// Returns the needed TileOrientation for a specific hexagon position.
        /// </summary>
        /// <param name="position">hexagon position.</param>
        /// <returns>The demanded tile orientation for this hexagon position.</returns>
        private ArrayTileOrientation GetDemandedOrientationForHexagonPosition(int position)
        {
            if (position == 0 || position == 2 || position == 4)
            {
                return ArrayTileOrientation.BottomUp;
            }

            if (position == 1 || position == 3 || position == 5)
            {
                return ArrayTileOrientation.TopDown;
            }

            throw new ArgumentException($"Unkown hexagon position '{position}'.");
        }
        #endregion

        #region GetIncomingFaceForNextHexagonPosition
        /// <summary>
        /// Determines the incoming face of a edge for the next hexagon position.
        /// </summary>
        /// <param name="nextTileOrientation">The orientation of the edge for the new hexagon position.</param>
        /// <returns>Incoming face for new hexgon position.</returns>
        private TileFace GetIncomingFaceForNextHexagonPosition(TileOrientation nextTileOrientation)
        {
            return this.GetIncomingFace(this.Pointer + 1, nextTileOrientation);
        }
        #endregion

        #region GetIncomingFace
        /// <summary>
        /// Determines the incoming face of a edge for a specific hexagon position.
        /// </summary>
        /// <param name="hexagonPosition">The position within the hexagon.</param>
        /// <param name="tileOrientation">The orientation of the edge for the new hexagon position.</param>
        /// <returns>Incoming face for new hexgon position.</returns>
        internal TileFace GetIncomingFace(int hexagonPosition, TileOrientation tileOrientation)
        {
            if ((hexagonPosition == 0 && tileOrientation == TileOrientation.Straight) ||
                (hexagonPosition == 2 && tileOrientation == TileOrientation.DoubleTiltLeft) ||
                (hexagonPosition == 4 && tileOrientation == TileOrientation.DoubleTiltRight) ||
                (hexagonPosition == 1 && tileOrientation == TileOrientation.TiltLeft) ||
                (hexagonPosition == 3 && tileOrientation == TileOrientation.Flipped) ||
                (hexagonPosition == 5 && tileOrientation == TileOrientation.TiltRight))
            {
                return TileFace.Left;
            }

            if ((hexagonPosition == 0 && tileOrientation == TileOrientation.DoubleTiltRight) ||
                (hexagonPosition == 2 && tileOrientation == TileOrientation.Straight) ||
                (hexagonPosition == 4 && tileOrientation == TileOrientation.DoubleTiltLeft) ||
                (hexagonPosition == 1 && tileOrientation == TileOrientation.TiltRight) ||
                (hexagonPosition == 3 && tileOrientation == TileOrientation.TiltLeft) ||
                (hexagonPosition == 5 && tileOrientation == TileOrientation.Flipped))
            {
                return TileFace.Bottom;
            }

            if ((hexagonPosition == 0 && tileOrientation == TileOrientation.DoubleTiltLeft) ||
                (hexagonPosition == 2 && tileOrientation == TileOrientation.DoubleTiltRight) ||
                (hexagonPosition == 4 && tileOrientation == TileOrientation.Straight) ||
                (hexagonPosition == 1 && tileOrientation == TileOrientation.Flipped) ||
                (hexagonPosition == 3 && tileOrientation == TileOrientation.TiltRight) ||
                (hexagonPosition == 5 && tileOrientation == TileOrientation.TiltLeft))
            {
                return TileFace.Right;
            }

            throw new ArgumentException($"The given combination (hPosition: {hexagonPosition}, orientation: {tileOrientation}) matches no incoming Face.");
        }
        #endregion

        #region GetOutgoingFace
        /// <summary>
        /// Determines the outgoing face of a triomino edge based on the tiles
        /// orientation and hexagon position.
        /// </summary>
        /// <param name="hexagonPosition">position within the hexagon</param>
        /// <param name="orientation">the tiles orientation.</param>
        /// <returns>Outgoing Face for the tile.</returns>
        internal TileFace GetOutgoingFace(int hexagonPosition, TileOrientation orientation)
        {
            if ((hexagonPosition == 0 && orientation == TileOrientation.Straight) ||
                (hexagonPosition == 2 && orientation == TileOrientation.DoubleTiltLeft) ||
                (hexagonPosition == 4 && orientation == TileOrientation.DoubleTiltRight) ||
                (hexagonPosition == 1 && orientation == TileOrientation.TiltLeft) ||
                (hexagonPosition == 3 && orientation == TileOrientation.Flipped) ||
                (hexagonPosition == 5 && orientation == TileOrientation.TiltRight))
            {
                return TileFace.Right;
            }

            if ((hexagonPosition == 0 && orientation == TileOrientation.DoubleTiltRight) ||
                (hexagonPosition == 2 && orientation == TileOrientation.Straight) ||
                (hexagonPosition == 4 && orientation == TileOrientation.DoubleTiltLeft) ||
                (hexagonPosition == 1 && orientation == TileOrientation.TiltRight) ||
                (hexagonPosition == 3 && orientation == TileOrientation.TiltLeft) ||
                (hexagonPosition == 5 && orientation == TileOrientation.Flipped))
            {
                return TileFace.Left;
            }

            if ((hexagonPosition == 0 && orientation == TileOrientation.DoubleTiltLeft) ||
                (hexagonPosition == 2 && orientation == TileOrientation.DoubleTiltRight) ||
                (hexagonPosition == 4 && orientation == TileOrientation.Straight) ||
                (hexagonPosition == 1 && orientation == TileOrientation.Flipped) ||
                (hexagonPosition == 3 && orientation == TileOrientation.TiltRight) ||
                (hexagonPosition == 5 && orientation == TileOrientation.TiltLeft))
            {
                return TileFace.Bottom;
            }

            throw new ArgumentException($"No outgoing face known for combination (hexagonPosition: {hexagonPosition}, orientation: {orientation}).");
        }
        #endregion

        #region GetLastEdgeAdded
        /// <summary>
        /// Get last Edge added to Hexagon.
        /// </summary>
        /// <returns>last edge added to hexagon.</returns>
        public HyperEdge GetLastEdgeAdded()
        {
            return this.Triominos[this.Pointer];
        }
        #endregion

        #region GetLastEdgeOutgoingFace
        /// <summary>
        /// Get outgoing face of last tile added to hexagon
        /// </summary>
        /// <returns>outgoing face of last tile added.</returns>
        public TileFace GetLastEdgeOutgoingFace()
        {
            return this.TriominoOutgoingFaces[this.Pointer];
        }
        #endregion

        #region CheckIfLastConnectorConnectsInAndOutgoing
        /// <summary>
        /// Determines, if two vertices can be connected via a two sided hyper edge.
        /// </summary>
        /// <param name="outgoingVertex"></param>
        /// <param name="incomingVertex"></param>
        /// <param name="connector"></param>
        /// <returns>True, if vertices can be connected, false if not.</returns>
        internal bool CheckIfLastConnectorConnectsInAndOutgoing(Vertex outgoingVertex, Vertex incomingVertex, HyperEdge connector)
        {
            if (!connector.IsTwoSidedEdge())
            {
                throw new ArgumentException($"Only two sided edges are eligible as connector ('{connector}' isn't).");
            }

            Vertex connectorIncomingVertex = connector.GetEdgeVertexInstance(outgoingVertex);
            Vertex connectorOutgoingVertex = connector.GetNeighborVertices(connectorIncomingVertex).First();

            return connectorOutgoingVertex.EqualsOnValueBasis(incomingVertex);
        }
        #endregion
    }
}
