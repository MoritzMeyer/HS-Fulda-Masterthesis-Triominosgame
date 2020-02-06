using GraphKI.Extensions;
using GraphKI.GraphSuite;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace GraphKI.GameManagement
{
    public class GameBoard
    {
        #region fields
        /// <summary>
        /// Number of Tiles placed on the gameboard.
        /// </summary>
        public int NumbTilesOnBoard { get; private set; }

        /// <summary>
        /// Event which is thrown, when a new tile was placed on the gameboard.
        /// </summary>
        public event EventHandler<TriominoTileEventArgs> TilePlaced;

        /// <summary>
        /// Array with the actual tile values on the grid according to the tiles position on the grid.
        /// </summary>
        private int?[,] tileValueGrid;

        /// <summary>
        /// Array with tiles on the GameBoard
        /// </summary>
        internal TriominoTile[,] tileGrid;

        /// <summary>
        /// Dictionary with all actual free faces on the gameboard.
        /// </summary>
        public Dictionary<string, List<TileFace>> tilesWithFreeFaces;

        private readonly int maxGameBoardSize = 56;

        private readonly HyperGraph tileGraph;
        #endregion

        #region ctor
        /// <summary>
        /// Intantiates a new object of this class.
        /// </summary>
        public GameBoard()
        {
            this.tileValueGrid = new int?[this.maxGameBoardSize + 1, this.maxGameBoardSize + 1];
            this.tileGrid = new TriominoTile[this.maxGameBoardSize, this.maxGameBoardSize];
            this.NumbTilesOnBoard = 0;
            this.tilesWithFreeFaces = new Dictionary<string, List<TileFace>>();

            List<Vertex> vertices = new List<Vertex>();
            List<HyperEdge> twoSidedEdges = new List<HyperEdge>();
            for (int i = 0; i < 6; i++)
            {
                for (int j = i; j < 6; j++)
                {
                    vertices.Add(new Vertex(i + "" + j));
                    if (i != j)
                    {
                        vertices.Add(new Vertex(j + "" + i));
                    }

                    twoSidedEdges.Add(new HyperEdge(i + "" + j, j + "" + i));
                }
            }

            tileGraph = new HyperGraph(vertices);
            foreach (HyperEdge twoSidedEdge in twoSidedEdges)
            {
                tileGraph.AddEdge(twoSidedEdge);
            }
        }
        #endregion

        #region GetTileFromGrid
        /// <summary>
        /// Get Tile from TileGrid.
        /// </summary>
        /// <param name="position">Coordinates of Tile.</param>
        /// <returns>The Tile</returns>
        internal TriominoTile GetTileFromGrid(Point position)
        {
            if (position.Y < 0 || position.X < 0)
            {
                throw new IndexOutOfRangeException("Only positive coordinates are allowed");
            }

            return this.tileGrid[position.Y, position.X];
        }
        #endregion

        #region CanPlaceTileOnGameBoard!!!
        /// <summary>
        /// Checks if a tile can placed on the GameBoard, based on the tiles orientation, 
        /// and another tile next to which the new tile should be placed
        /// </summary>
        /// <param name="tile">The tile which should be placed</param>
        /// <param name="otherName">The name of the other tile.</param>
        /// <param name="otherFace">The face of the other tile at which the new tile should be placed.</param>
        /// <returns>True, if the new tile can be placed, false if not.</returns>
        public bool CanPlaceTileOnGameBoard(string tileName, string otherName, TileFace? tileFace, TileFace? otherFace, out TriominoTile placableTile)
        {
            placableTile = null;

            // if it's the first tile it can always be placed.
            if (this.NumbTilesOnBoard == 0)
            {
                placableTile = new TriominoTile(tileName, TileOrientation.Straight, new Point(this.maxGameBoardSize/2, this.maxGameBoardSize/2));
                return true;
            }

            // otherFace and otherName can only be null if it's the first tile
            if (otherFace == null || otherName == null || otherName == string.Empty)
            {
                return false;
            }

            Point otherTileGridCoordinates = this.GetTileCoordsByName(otherName);
            Point possibleNewGridCoordinates = this.GetTileGridPositionFromOtherTilePositionAndFace(otherTileGridCoordinates, otherFace.Value);

            if (possibleNewGridCoordinates.Y == -1 || possibleNewGridCoordinates.X == -1)
            {
                return false;
            }

            TileOrientation tileOrientation = GameBoard.GetTileOrienationFromOtherTileOrientationAndFaces(this.GetTileFromGrid(otherTileGridCoordinates).Orientation, otherFace.Value, tileFace.Value);
            TriominoTile tile = new TriominoTile(tileName, tileOrientation, possibleNewGridCoordinates);

            // If Tile values can be placed it will be returned
            if (this.CheckValueGridAtTileGridPosition(tile))
            {
                placableTile = tile;
                return true;
            }

            return false;
        }
        #endregion

        private bool CheckMatchingAdjacentTiles(TriominoTile newTile)
        {
            bool newTileMatchesAdjacentTiles = true;
            TileFace[] facesToCheck = new TileFace[] { TileFace.Bottom, TileFace.Right, TileFace.Left };

            foreach (TileFace faceToCheck in facesToCheck)
            {
                TriominoTile adjacenTileAtFace = this.GetAdjacentTileAtSpecificFace(newTile, faceToCheck);

                if (adjacenTileAtFace != null)
                {
                    newTileMatchesAdjacentTiles = newTileMatchesAdjacentTiles && this.CheckMatchingAdjacentTileFaces(newTile, adjacenTileAtFace);
                }
                else
                {
                    newTileMatchesAdjacentTiles = newTileMatchesAdjacentTiles && this.CheckBridgeEdgesAtTileFace(newTile, faceToCheck);
                }
            }

            return newTileMatchesAdjacentTiles;
        }

        private bool CheckMatchingAdjacentTileFaces(TriominoTile thisTile, TriominoTile otherTile)
        {
            // adjacentTiles must have different Orientations.
            if (thisTile.Orientation.ToArrayTileOrientation() == otherTile.Orientation.ToArrayTileOrientation())
            {
                return false;
            }

            // other tile is right of this tile
            if (thisTile.TileGridPosition.Y == otherTile.TileGridPosition.Y && thisTile.TileGridPosition.X + 1 == otherTile.TileGridPosition.X)
            {
                switch (thisTile.Orientation.ToArrayTileOrientation())
                {
                    case ArrayTileOrientation.BottomUp:
                        return thisTile.Name.CheckIfFacesMatches(otherTile.Name, TileFace.Right, TileFace.Right);
                    case ArrayTileOrientation.TopDown:
                        return thisTile.Name.CheckIfFacesMatches(otherTile.Name, TileFace.Left, TileFace.Left);
                    default:
                        throw new ArgumentException("Unknown TileOrientation.");
                }
            }

            // other tile is left of this tile
            if (thisTile.TileGridPosition.Y == otherTile.TileGridPosition.Y && thisTile.TileGridPosition.X - 1 == otherTile.TileGridPosition.X)
            {
                switch (thisTile.Orientation.ToArrayTileOrientation())
                {
                    case ArrayTileOrientation.BottomUp:
                        return thisTile.Name.CheckIfFacesMatches(otherTile.Name, TileFace.Left, TileFace.Left);
                    case ArrayTileOrientation.TopDown:
                        return thisTile.Name.CheckIfFacesMatches(otherTile.Name, TileFace.Right, TileFace.Right);
                    default:
                        throw new ArgumentException("Unknown TileOrientation.");
                }
            }

            // other tile is top of this tile
            if (thisTile.TileGridPosition.Y - 1 == otherTile.TileGridPosition.Y && thisTile.TileGridPosition.X == otherTile.TileGridPosition.X)
            {
                if (thisTile.Orientation.ToArrayTileOrientation() == ArrayTileOrientation.TopDown)
                {
                    return thisTile.Name.CheckIfFacesMatches(otherTile.Name, TileFace.Bottom, TileFace.Bottom);
                }
                else
                {
                    throw new ArgumentException("Tiles got now adjacent faces.");
                }
            }

            // other tile is bottom of this tile
            if (thisTile.TileGridPosition.Y + 1 == otherTile.TileGridPosition.Y && thisTile.TileGridPosition.X == otherTile.TileGridPosition.X)
            {
                if (thisTile.Orientation.ToArrayTileOrientation() == ArrayTileOrientation.BottomUp)
                {
                    return thisTile.Name.CheckIfFacesMatches(otherTile.Name, TileFace.Bottom, TileFace.Bottom);
                }
                else
                {
                    throw new ArgumentException("Tiles got now adjacent faces.");
                }
            }

            return false;
        }

        private bool CheckBridgeEdgesAtTileFace(TriominoTile newTile, TileFace tileFace)
        {
            throw new NotImplementedException();
        }

        #region GetAdjacentTileAtSpecificFace
        /// <summary>
        /// Returns a tile from the tileGrid starting from a given face of a specific tile (and it's position on the tileGrid).
        /// </summary>
        /// <param name="tile">Tile to start from.</param>
        /// <returns></returns>
        internal TriominoTile GetAdjacentTileAtSpecificFace(TriominoTile tile, TileFace tileFace)
        {
            ArrayTileOrientation orientation = tile.Orientation.ToArrayTileOrientation();

            if (orientation == ArrayTileOrientation.BottomUp && tileFace == TileFace.Bottom)
            {
                return this.tileGrid[tile.TileGridPosition.Y + 1, tile.TileGridPosition.X];
            }

            if (orientation == ArrayTileOrientation.TopDown && tileFace == TileFace.Bottom)
            {
                return this.tileGrid[tile.TileGridPosition.Y - 1, tile.TileGridPosition.X];
            }

            if (orientation == ArrayTileOrientation.BottomUp && tileFace == TileFace.Right ||
                orientation == ArrayTileOrientation.TopDown && tileFace == TileFace.Left)
            {
                return this.tileGrid[tile.TileGridPosition.Y, tile.TileGridPosition.X + 1];
            }

            if (orientation == ArrayTileOrientation.BottomUp && tileFace == TileFace.Left ||
                orientation == ArrayTileOrientation.TopDown && tileFace == TileFace.Right)
            {
                return this.tileGrid[tile.TileGridPosition.Y, tile.TileGridPosition.X - 1];
            }

            return null;
        }
        #endregion

        #region GetTileGridPositionFromOtherTilePositionAndFace
        /// <summary>
        /// Determines a place in the tileGrid for a tile according to anothers (other) tile 
        /// coordinates and its face on which the new tile should be placed.
        /// </summary>
        /// <param name="otherTileTileGridCoords">The others tile coordinates of the tileGrid.</param>
        /// <param name="otherFace">The others tile face, on which the new tile should be placed.</param>
        /// <returns>The coordniates of the tile Grid, next to the other tile. If there is a tile on the determined coordinates, -1|-1 is returned.</returns>
        internal Point GetTileGridPositionFromOtherTilePositionAndFace(Point otherTileTileGridCoords, TileFace otherFace)
        {
            Point thisTileGridPosition = new Point(-1, -1);

            Tuple<TileOrientation, TileFace> compareTuple = new Tuple<TileOrientation, TileFace>(this.GetTileFromGrid(otherTileTileGridCoords).Orientation, otherFace);

            if ((compareTuple.Item1 == TileOrientation.Straight && compareTuple.Item2 == TileFace.Left) ||
                (compareTuple.Item1 == TileOrientation.DoubleTiltLeft && compareTuple.Item2 == TileFace.Right) ||
                (compareTuple.Item1 == TileOrientation.DoubleTiltRight && compareTuple.Item2 == TileFace.Bottom) ||
                (compareTuple.Item1 == TileOrientation.Flipped && compareTuple.Item2 == TileFace.Right) ||
                (compareTuple.Item1 == TileOrientation.TiltLeft && compareTuple.Item2 == TileFace.Left) ||
                (compareTuple.Item1 == TileOrientation.TiltRight && compareTuple.Item2 == TileFace.Bottom))
            {
                thisTileGridPosition = new Point(otherTileTileGridCoords.X - 1, otherTileTileGridCoords.Y);
            }
            else if ((compareTuple.Item1 == TileOrientation.Straight && compareTuple.Item2 == TileFace.Right) ||
                     (compareTuple.Item1 == TileOrientation.DoubleTiltLeft && compareTuple.Item2 == TileFace.Bottom) ||
                     (compareTuple.Item1 == TileOrientation.DoubleTiltRight && compareTuple.Item2 == TileFace.Left) ||
                     (compareTuple.Item1 == TileOrientation.Flipped && compareTuple.Item2 == TileFace.Left) ||
                     (compareTuple.Item1 == TileOrientation.TiltLeft && compareTuple.Item2 == TileFace.Bottom) ||
                     (compareTuple.Item1 == TileOrientation.TiltRight && compareTuple.Item2 == TileFace.Right))
            {
                thisTileGridPosition = new Point(otherTileTileGridCoords.X + 1, otherTileTileGridCoords.Y);
            }
            else if ((compareTuple.Item1 == TileOrientation.Straight && compareTuple.Item2 == TileFace.Bottom) ||
                     (compareTuple.Item1 == TileOrientation.DoubleTiltLeft && compareTuple.Item2 == TileFace.Left) ||
                     (compareTuple.Item1 == TileOrientation.DoubleTiltRight && compareTuple.Item2 == TileFace.Right))
            {
                thisTileGridPosition = new Point(otherTileTileGridCoords.X, otherTileTileGridCoords.Y + 1);
            }
            else if ((compareTuple.Item1 == TileOrientation.Flipped && compareTuple.Item2 == TileFace.Bottom) ||
                     (compareTuple.Item1 == TileOrientation.TiltLeft && compareTuple.Item2 == TileFace.Right) ||
                     (compareTuple.Item1 == TileOrientation.TiltRight && compareTuple.Item2 == TileFace.Left))
            {
                thisTileGridPosition = new Point(otherTileTileGridCoords.X, otherTileTileGridCoords.Y - 1);
            }

            if (thisTileGridPosition.Y >= 0 && thisTileGridPosition.X >= 0 && this.GetTileFromGrid(thisTileGridPosition) != null)
            {
                thisTileGridPosition.X = -1;
                thisTileGridPosition.Y = -1;
            }

            return thisTileGridPosition;
        }
        #endregion

        #region GetTileOrienationFromOtherTileOrientationAndFaces
        /// <summary>
        /// Determines the orientation of a Tile, which should be placed on the GameBoard, based on another 
        /// tiles orientation, and the faces with which both tiles should be aligned towards each other.
        /// </summary>
        /// <param name="otherOrientation">Other tiles orientation.</param>
        /// <param name="otherFace">Other tiles face.</param>
        /// <param name="tileFace">New tiles face.</param>
        /// <returns>the orientation of the new tile.</returns>
        internal static TileOrientation GetTileOrienationFromOtherTileOrientationAndFaces(TileOrientation otherOrientation, TileFace otherFace, TileFace tileFace)
        {
            if ((otherOrientation == TileOrientation.Straight && otherFace == TileFace.Left && tileFace == TileFace.Left) ||
                (otherOrientation == TileOrientation.Straight && otherFace == TileFace.Right && tileFace == TileFace.Right) ||
                (otherOrientation == TileOrientation.Straight && otherFace == TileFace.Bottom && tileFace == TileFace.Bottom) ||
                (otherOrientation == TileOrientation.DoubleTiltLeft && otherFace == TileFace.Left && tileFace == TileFace.Bottom) ||
                (otherOrientation == TileOrientation.DoubleTiltLeft && otherFace == TileFace.Right && tileFace == TileFace.Left) ||
                (otherOrientation == TileOrientation.DoubleTiltLeft && otherFace == TileFace.Bottom && tileFace == TileFace.Right) ||
                (otherOrientation == TileOrientation.DoubleTiltRight && otherFace == TileFace.Left && tileFace == TileFace.Right) ||
                (otherOrientation == TileOrientation.DoubleTiltRight && otherFace == TileFace.Right && tileFace == TileFace.Bottom) ||
                (otherOrientation == TileOrientation.DoubleTiltRight && otherFace == TileFace.Bottom && tileFace == TileFace.Left))
            {
                return TileOrientation.Flipped;
            }
            else if ((otherOrientation == TileOrientation.Straight && otherFace == TileFace.Left && tileFace == TileFace.Right) ||
                     (otherOrientation == TileOrientation.Straight && otherFace == TileFace.Right && tileFace == TileFace.Bottom) ||
                     (otherOrientation == TileOrientation.Straight && otherFace == TileFace.Bottom && tileFace == TileFace.Left) ||
                     (otherOrientation == TileOrientation.DoubleTiltLeft && otherFace == TileFace.Left && tileFace == TileFace.Left) ||
                     (otherOrientation == TileOrientation.DoubleTiltLeft && otherFace == TileFace.Right && tileFace == TileFace.Right) ||
                     (otherOrientation == TileOrientation.DoubleTiltLeft && otherFace == TileFace.Bottom && tileFace == TileFace.Bottom) ||
                     (otherOrientation == TileOrientation.DoubleTiltRight && otherFace == TileFace.Left && tileFace == TileFace.Bottom) ||
                     (otherOrientation == TileOrientation.DoubleTiltRight && otherFace == TileFace.Right && tileFace == TileFace.Left) ||
                     (otherOrientation == TileOrientation.DoubleTiltRight && otherFace == TileFace.Bottom && tileFace == TileFace.Right))
            {
                return TileOrientation.TiltRight;
            }
            else if ((otherOrientation == TileOrientation.Straight && otherFace == TileFace.Left && tileFace == TileFace.Bottom) ||
                     (otherOrientation == TileOrientation.Straight && otherFace == TileFace.Right && tileFace == TileFace.Left) ||
                     (otherOrientation == TileOrientation.Straight && otherFace == TileFace.Bottom && tileFace == TileFace.Right) ||
                     (otherOrientation == TileOrientation.DoubleTiltLeft && otherFace == TileFace.Left && tileFace == TileFace.Right) ||
                     (otherOrientation == TileOrientation.DoubleTiltLeft && otherFace == TileFace.Right && tileFace == TileFace.Bottom) ||
                     (otherOrientation == TileOrientation.DoubleTiltLeft && otherFace == TileFace.Bottom && tileFace == TileFace.Left) ||
                     (otherOrientation == TileOrientation.DoubleTiltRight && otherFace == TileFace.Left && tileFace == TileFace.Left) ||
                     (otherOrientation == TileOrientation.DoubleTiltRight && otherFace == TileFace.Right && tileFace == TileFace.Right) ||
                     (otherOrientation == TileOrientation.DoubleTiltRight && otherFace == TileFace.Bottom && tileFace == TileFace.Bottom))
            {
                return TileOrientation.TiltLeft;
            }
            else if ((otherOrientation == TileOrientation.TiltLeft && otherFace == TileFace.Left && tileFace == TileFace.Left) ||
                     (otherOrientation == TileOrientation.TiltLeft && otherFace == TileFace.Right && tileFace == TileFace.Right) ||
                     (otherOrientation == TileOrientation.TiltLeft && otherFace == TileFace.Bottom && tileFace == TileFace.Bottom) ||
                     (otherOrientation == TileOrientation.Flipped && otherFace == TileFace.Left && tileFace == TileFace.Bottom) ||
                     (otherOrientation == TileOrientation.Flipped && otherFace == TileFace.Right && tileFace == TileFace.Left) ||
                     (otherOrientation == TileOrientation.Flipped && otherFace == TileFace.Bottom && tileFace == TileFace.Right) ||
                     (otherOrientation == TileOrientation.TiltRight && otherFace == TileFace.Left && tileFace == TileFace.Right) ||
                     (otherOrientation == TileOrientation.TiltRight && otherFace == TileFace.Right && tileFace == TileFace.Bottom) ||
                     (otherOrientation == TileOrientation.TiltRight && otherFace == TileFace.Bottom && tileFace == TileFace.Left))
            {
                return TileOrientation.DoubleTiltRight;
            }
            else if ((otherOrientation == TileOrientation.TiltLeft && otherFace == TileFace.Left && tileFace == TileFace.Right) ||
                     (otherOrientation == TileOrientation.TiltLeft && otherFace == TileFace.Right && tileFace == TileFace.Bottom) ||
                     (otherOrientation == TileOrientation.TiltLeft && otherFace == TileFace.Bottom && tileFace == TileFace.Left) ||
                     (otherOrientation == TileOrientation.Flipped && otherFace == TileFace.Left && tileFace == TileFace.Left) ||
                     (otherOrientation == TileOrientation.Flipped && otherFace == TileFace.Right && tileFace == TileFace.Right) ||
                     (otherOrientation == TileOrientation.Flipped && otherFace == TileFace.Bottom && tileFace == TileFace.Bottom) ||
                     (otherOrientation == TileOrientation.TiltRight && otherFace == TileFace.Left && tileFace == TileFace.Bottom) ||
                     (otherOrientation == TileOrientation.TiltRight && otherFace == TileFace.Right && tileFace == TileFace.Left) ||
                     (otherOrientation == TileOrientation.TiltRight && otherFace == TileFace.Bottom && tileFace == TileFace.Right))
            {
                return TileOrientation.Straight;
            }
            else if ((otherOrientation == TileOrientation.TiltLeft && otherFace == TileFace.Left && tileFace == TileFace.Bottom) ||
                    (otherOrientation == TileOrientation.TiltLeft && otherFace == TileFace.Right && tileFace == TileFace.Left) ||
                    (otherOrientation == TileOrientation.TiltLeft && otherFace == TileFace.Bottom && tileFace == TileFace.Right) ||
                    (otherOrientation == TileOrientation.Flipped && otherFace == TileFace.Left && tileFace == TileFace.Right) ||
                    (otherOrientation == TileOrientation.Flipped && otherFace == TileFace.Right && tileFace == TileFace.Bottom) ||
                    (otherOrientation == TileOrientation.Flipped && otherFace == TileFace.Bottom && tileFace == TileFace.Left) ||
                    (otherOrientation == TileOrientation.TiltRight && otherFace == TileFace.Left && tileFace == TileFace.Left) ||
                    (otherOrientation == TileOrientation.TiltRight && otherFace == TileFace.Right && tileFace == TileFace.Right) ||
                    (otherOrientation == TileOrientation.TiltRight && otherFace == TileFace.Bottom && tileFace == TileFace.Bottom))
            {
                return TileOrientation.DoubleTiltLeft;
            }

            throw new ArgumentException("The combination TileOrientation, TileFace, TileFace cannot be determined");
        }
        #endregion

        #region TryAddTile!!!
        /// <summary>
        /// Adds a tile on the GameBoard, based on the tiles Value, another Tile on the GameBoard besides which the new Tile should be placed
        /// and the faces with which both tiles should be placed towards each other.
        /// </summary>
        /// <param name="player">Der PlayerCode des aktiven Spielers.</param>
        /// <param name="tileName">The new tile.</param>
        /// <param name="hexagons">Die Liste mit Hexagonen, falls vorhanden.</param>
        /// <param name="otherName">The other tile besides which the new tile should be placed.</param>
        /// <param name="tileFace">The new tiles face wich points to the other tile.</param>
        /// <param name="otherFace">The other tiles face which points to the new tile.</param>
        /// <returns>True if tile could be added, false if not</returns>
        public bool TryAddTile(PlayerCode player, string tileName, out List<Hexagon> hexagons, string otherName = null, TileFace? tileFace = null, TileFace? otherFace = null)
        {
            tileName.EnsureTriominoTileName();
            hexagons = new List<Hexagon>();

            if (!this.CanPlaceTileOnGameBoard(tileName, otherName, tileFace, otherFace, out TriominoTile placeableTile))
            {
                return false;
            }

            this.AddTile(placeableTile);
            this.AddFreeFaces(tileName, otherName, tileFace, otherFace);

            IEnumerable<TriominoTile> directNeighbors = this.GetTileNeighbors(placeableTile);
            this.tileGraph.AddEdgeWithDirectNeighbors(placeableTile.CreateHyperEdgeFromTile(), directNeighbors.Select(n => n.CreateHyperEdgeFromTile()), out HyperEdge addedEdge);
            this.tileGraph.IsPartOfHexagon(addedEdge, out hexagons);

            this.OnTilePlaced(new TriominoTileEventArgs()
            {
                TileName = tileName,
                OtherTileName = otherName,
                TileFace = tileFace,
                OtherTileFAce = otherFace,
                Player = player

            });
            return true;
        }
        #endregion

        private IEnumerable<TriominoTile> GetTileNeighbors(TriominoTile tile)
        {
            // No Checks for Arraybounds, cause this should create an Exception (so, the bounds can be enlargened then).
            List<TriominoTile> neighbors = new List<TriominoTile>();
            Point tileGridPosition = this.GetTileCoordsByName(tile.Name);

            // Add neighbors from both tile sides (left and right, according to orientation)
            neighbors.Add(this.tileGrid[tileGridPosition.Y, tileGridPosition.X - 1]);
            neighbors.Add(this.tileGrid[tileGridPosition.Y, tileGridPosition.X + 1]);

            if (tile.Orientation.ToArrayTileOrientation() == ArrayTileOrientation.BottomUp)
            {
                // Bottom
                neighbors.Add(this.tileGrid[tileGridPosition.Y + 1, tileGridPosition.X]);
            }

            if (tile.Orientation.ToArrayTileOrientation() == ArrayTileOrientation.TopDown)
            {
                // Top
                neighbors.Add(this.tileGrid[tileGridPosition.Y - 1, tileGridPosition.X]);
            }

            neighbors = neighbors.Where(n => n != null).ToList();
            return neighbors;
        }

        #region AddTile
        /// <summary>
        /// Places a Tile on the TileGrid and TileValuesGrid
        /// </summary>
        /// <param name="tile"></param>
        /// <returns></returns>
        private bool AddTile(TriominoTile tile)
        {
            if (this.GetTileFromGrid(tile.TileGridPosition) != null)
            {
                throw new ArgumentException($"Cannot add tile {tile.Name} at position(x,y): ({tile.TileGridPosition.X}, {tile.TileGridPosition.Y}). Place is taken by tile: '{this.GetTileFromGrid(tile.TileGridPosition).Name}'");
            }

            this.SetTileOnGrid(tile);
            this.AddValuesToValueGrid(tile.GetArrayName(), tile.TileGridPosition, tile.Orientation.ToArrayTileOrientation());
            this.NumbTilesOnBoard++;

            return true;
        }
        #endregion

        #region SetTileOnGrid
        /// <summary>
        ///  Sets a tile on the grid with coordinates from tile object.
        /// </summary>
        /// <param name="tile">The tile to be set on the grid including the coordinates.</param>
        private void SetTileOnGrid(TriominoTile tile)
        {
            if (tile.TileGridPosition.Y < 0 || tile.TileGridPosition.X < 0)
            {
                throw new IndexOutOfRangeException("Only positive coordinates are allowed");
            }

            this.tileGrid[tile.TileGridPosition.Y, tile.TileGridPosition.X] = tile;
        }
        #endregion

        #region GetTileCoordsByName
        /// <summary>
        /// Determines the coords of a specific tile in the tileGrid by its name.
        /// </summary>
        /// <param name="name">tile name.</param>
        /// <returns>The coordinates of the tile if it can be found.</returns>
        private Point GetTileCoordsByName(string name)
        {
            int otherNameGridPositionX = -1;
            int otherNameGridPositionY = -1;

            for (int i = 0; i < this.tileGrid.GetLength(0); i++)
            {
                for (int j = 0; j < this.tileGrid.GetLength(1); j++)
                {
                    if (this.tileGrid[i, j] != null && this.tileGrid[i, j].Name.Equals(name))
                    {
                        if (otherNameGridPositionX != -1 || otherNameGridPositionY != -1)
                        {
                            throw new InvalidOperationException("Tile with 'name' could be found multiple times in grid.");
                        }

                        otherNameGridPositionY = i;
                        otherNameGridPositionX = j;
                    }
                }
            }

            Point tileCoords = new Point(otherNameGridPositionX, otherNameGridPositionY);
            return tileCoords;
        }
        #endregion

        #region CheckValueGridAtTileGridPosition
        /// <summary>
        /// Checks the values for a tile at a specific tileGridPosition. 
        /// If one value doesn't match the value within the tiles name false is returned, 
        /// while one null-value of the tileValueGrid values is allowed.
        /// </summary>
        /// <param name="tile">The tile whose values should be checked.</param>
        /// <returns>True, if values matching (one tileValueGrid value can be null), false if not.</returns>
        private bool CheckValueGridAtTileGridPosition(TriominoTile tile)
        {
            List<int?> valuesToCheck = this.GetValueGridValuesFromNameGridPositions(tile.TileGridPosition, tile.Orientation.ToArrayTileOrientation());

            // if more than two values are null, this triomino tile
            // isn't added adjacent to another tile (except for the first tile).
            if (this.NumbTilesOnBoard > 0 && valuesToCheck.Where(v => !v.HasValue).Count() > 1)
            {
                return false;
            }

            string[] nameParts = tile.GetArrayName().Split('-');

            for (int i = 0; i < valuesToCheck.Count; i++)
            {
                int intValue = int.Parse(nameParts[i]);

                if (valuesToCheck[i].HasValue && valuesToCheck[i] != intValue)
                {
                    return false;
                }
            }

            return true;
        }
        #endregion

        #region GetValueGridValuesFromNameGridPositions
        /// <summary>
        /// Determines the 'ValueGrid' values for a given tileGrid Position according to the tiles 'ArrayTileOrientation'.
        /// </summary>
        /// <param name="tileGridPosition">the tiles position within the tileGrid.</param>
        /// <param name="orientation"></param>
        /// <returns>the tile values</returns>
        private List<int?> GetValueGridValuesFromNameGridPositions(Point tileGridPosition, ArrayTileOrientation orientation)
        {
            List<int?> valueGridValues = new List<int?>();

            switch (orientation)
            {
                case ArrayTileOrientation.BottomUp:
                    valueGridValues = new List<int?>()
                    {
                        this.tileValueGrid[tileGridPosition.Y, tileGridPosition.X + 1],
                        this.tileValueGrid[tileGridPosition.Y + 1, tileGridPosition.X + 2],
                        this.tileValueGrid[tileGridPosition.Y + 1, tileGridPosition.X]
                    };
                    break;
                case ArrayTileOrientation.TopDown:
                    valueGridValues = new List<int?>()
                    {
                        this.tileValueGrid[tileGridPosition.Y + 1, tileGridPosition.X + 1],
                        this.tileValueGrid[tileGridPosition.Y, tileGridPosition.X],
                        this.tileValueGrid[tileGridPosition.Y, tileGridPosition.X + 2]
                    };
                    break;
            }

            return valueGridValues;
        }
        #endregion

        #region AddValuesToValueGrid
        /// <summary>
        /// Inserts the values of a tile in the 'tileValueGrid' based on the tiles name, its tileGrid-Coordinates and its 'ArrayTileOrientation'
        /// </summary>
        /// <param name="name">The tiles name.</param>
        /// <param name="tileGridPosition">the tiles position within the tileGrid.</param>
        /// <param name="orientation">The tile Orientation within the tileValueGrid.</param>
        private void AddValuesToValueGrid(string name, Point tileGridPosition, ArrayTileOrientation orientation)
        {
            string[] nameParts = name.Split('-');

            switch (orientation)
            {
                case ArrayTileOrientation.BottomUp:
                    this.tileValueGrid[tileGridPosition.Y, tileGridPosition.X + 1] = int.Parse(nameParts[0]);
                    this.tileValueGrid[tileGridPosition.Y + 1, tileGridPosition.X + 2] = int.Parse(nameParts[1]);
                    this.tileValueGrid[tileGridPosition.Y + 1, tileGridPosition.X] = int.Parse(nameParts[2]);
                    break;
                case ArrayTileOrientation.TopDown:
                    this.tileValueGrid[tileGridPosition.Y + 1, tileGridPosition.X + 1] = int.Parse(nameParts[0]);
                    this.tileValueGrid[tileGridPosition.Y, tileGridPosition.X] = int.Parse(nameParts[1]);
                    this.tileValueGrid[tileGridPosition.Y, tileGridPosition.X + 2] = int.Parse(nameParts[2]);
                    break;
            }
        }
        #endregion

        #region OnTilePlaced
        /// <summary>
        /// throws a new tilePlaced event.
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnTilePlaced(TriominoTileEventArgs e)
        {
            this.TilePlaced?.Invoke(this, e);
        }
        #endregion

        #region AddFreeFaces
        /// <summary>
        /// Adds free faces of new tile and removes the now occupied face of othertile from the freeFaces Dictionary.
        /// </summary>
        /// <param name="tile">name of new tile</param>
        /// <param name="otherTile">name of other tile</param>
        /// <param name="tileFace">face of new Tile</param>
        /// <param name="otherFace">face of other tile</param>
        private void AddFreeFaces(string tile, string otherTile = null, TileFace? tileFace = null, TileFace? otherFace = null)
        {
            if (otherTile == null || tileFace == null || otherFace == null)
            {
                this.tilesWithFreeFaces.Add(tile, new List<TileFace>() { TileFace.Left, TileFace.Right, TileFace.Bottom });
                return;
            }

            this.tilesWithFreeFaces[otherTile].Remove(otherFace.Value);

            List<TileFace> oppositeSites = new List<TileFace>();
            switch(tileFace.Value)
            {
                case TileFace.Left:
                    oppositeSites.Add(TileFace.Right);
                    oppositeSites.Add(TileFace.Bottom);
                    break;
                case TileFace.Right:
                    oppositeSites.Add(TileFace.Bottom);
                    oppositeSites.Add(TileFace.Left);
                    break;
                case TileFace.Bottom:
                    oppositeSites.Add(TileFace.Right);
                    oppositeSites.Add(TileFace.Left);
                    break;
            }

            this.tilesWithFreeFaces.Add(tile, oppositeSites);
        }
        #endregion
    }
}