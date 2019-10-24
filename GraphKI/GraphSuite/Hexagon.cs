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
        public bool IsComplete { get; private set; }
        private List<HyperEdge> Triominos;
        private List<HyperEdge> outgoingConnectors;
        private List<TileFace> TriominoOutgoingFaces;
        internal int Pointer;
        #endregion

        #region ctor
        /// <summary>
        /// Creates a new Instance of this class.
        /// </summary>
        /// <param name="edge">the starting tile of this hexagon.</param>
        /// <param name="tileFace">the face of the starting tile for the next tile.</param>
        public Hexagon(HyperEdge edge, TileFace tileFace, HyperEdge outgoingConnector)
        {
            if (!edge.IsThreeSidedEdge())
            {
                throw new ArgumentException($"Edge '{edge}' has to be threesided.");
            }

            if (!outgoingConnector.IsTwoSidedEdge())
            {
                throw new ArgumentException($"Edge '{outgoingConnector}' has to be twosided.");
            }

            if (!outgoingConnector.ContainsVertexOnValueBasis(edge.GetVertexOnSpecificSide(tileFace)))
            {
                throw new ArgumentException($"Connector '{outgoingConnector}' has no vertex with edge '{edge}' in common.");
            }

            this.Triominos = new List<HyperEdge>(6);
            this.Triominos.AddRange(Enumerable.Repeat(default(HyperEdge), 6));

            this.outgoingConnectors = new List<HyperEdge>(6);
            this.outgoingConnectors.AddRange(Enumerable.Repeat(default(HyperEdge), 6));

            this.TriominoOutgoingFaces = new List<TileFace>(6);
            this.TriominoOutgoingFaces.AddRange(Enumerable.Repeat(default(TileFace), 6));

            this.Pointer = this.DetermineHexagonPosition(edge.Orientation, tileFace);
            this.Triominos[this.Pointer] = edge;
            this.TriominoOutgoingFaces[this.Pointer] = tileFace;
            this.outgoingConnectors[this.Pointer] = outgoingConnector;
            this.IsComplete = false;
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

        public bool TryAddToHexagon(HyperEdge edge, HyperEdge outgoingConnector)
        {
            if (!edge.IsThreeSidedEdge())
            {
                throw new ArgumentException($"Only three sided edges are eligible as triomino edge ('{edge}' isn't).");
            }

            if (!outgoingConnector.IsTwoSidedEdge())
            {
                throw new ArgumentException($"Only two sided edges are eligible as connector ('{outgoingConnector}' isn't).");

            }

            // Check orientation for new edge according to the next free hexagon position.
            if (edge.Orientation.ToArrayTileOrientation() != this.GetDemandedOrientationForHexagonPosition(this.GetNextPointerValue(this.Pointer)))
            {
                return false;
            }

            // check if connector contains outgoing face vertex 
            TileFace thisOutgoingFace = this.GetOutgoingFace(this.GetNextPointerValue(this.Pointer), edge.Orientation);
            if (!outgoingConnector.ContainsVertexOnValueBasis(edge.GetVertexOnSpecificSide(thisOutgoingFace)))
            {
                //throw new ArgumentException($"Connector '{outgoingConnector}' has no vertex with edge '{edge}' in common.");
                return false;
            }

            HyperEdge lastHexagonTile = this.GetLastEdgeAdded();
            TileFace lastOutgoingFace = this.GetLastEdgeOutgoingFace();
            TileFace thisIncomingTileFace = this.GetIncomingFaceForNextHexagonPosition(edge.Orientation);

            if (!this.CheckIfLastConnectorConnectsInAndOutgoing(lastHexagonTile.GetVertexOnSpecificSide(lastOutgoingFace), edge.GetVertexOnSpecificSide(thisIncomingTileFace), this.outgoingConnectors[this.Pointer]))
            {
                return false;
            }

            // if this tile completes the hexagon, not only ingoing face must fit with previous tile
            // but also outgoing face must fit with first tile
            int lookAHeadPointer = this.GetNextPointerValue(this.Pointer);
            lookAHeadPointer = this.GetNextPointerValue(lookAHeadPointer);
            if (this.Triominos[lookAHeadPointer] != null)
            {
                HyperEdge firstHexagonTile = this.Triominos[lookAHeadPointer];
                TileFace firstIncomingFace = this.GetIncomingFace(lookAHeadPointer, firstHexagonTile.Orientation);

                if (!this.CheckIfLastConnectorConnectsInAndOutgoing(edge.GetVertexOnSpecificSide(thisOutgoingFace), firstHexagonTile.GetVertexOnSpecificSide(firstIncomingFace), outgoingConnector))
                {
                    return false;
                }
            }

            this.Pointer = this.GetNextPointerValue(this.Pointer);

            this.Triominos[this.Pointer] = edge;
            this.TriominoOutgoingFaces[this.Pointer] = this.GetOutgoingFace(this.Pointer, edge.Orientation);
            this.outgoingConnectors[this.Pointer] = outgoingConnector;

            if (this.Triominos[this.GetNextPointerValue(this.Pointer)] != null)
            {
                this.IsComplete = true;
            }

            return true;
        }

        #region GetNextPointerValue
        /// <summary>
        /// Calculates new Pointer value, and considers the max pointer of 6 (after 6 comes 0).
        /// </summary>
        /// <param name="pointer">The actual pointer value.</param>
        /// <returns>New pointer Value.</returns>
        private int GetNextPointerValue(int pointer)
        {
            pointer++;

            if (pointer >= 6)
            {
                pointer = 0;
            }

            return pointer;
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
            return this.GetIncomingFace(this.GetNextPointerValue(this.Pointer), nextTileOrientation);
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
