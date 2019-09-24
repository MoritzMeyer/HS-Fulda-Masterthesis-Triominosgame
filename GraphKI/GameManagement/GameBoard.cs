using GraphKI.Extensions;
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
        /// Array with the actual tile values on the grid according to the tiles position on the grid.
        /// </summary>
        private int?[,] tileValueGrid;

        /// <summary>
        /// Array with tiles on the GameBoard
        /// </summary>
        private TriominoTile[,] tileGrid;
        #endregion

        #region ctor
        /// <summary>
        /// Intantiates a new object of this class.
        /// </summary>
        public GameBoard()
        {
            this.InitTriominosGameBoard();
        }
        #endregion

        #region InitTriominosGameBoard
        /// <summary>
        /// Initializes the GameBoard variables.
        /// </summary>
        public void InitTriominosGameBoard()
        {
            this.tileValueGrid = new int?[57, 57];
            this.tileGrid = new TriominoTile[56, 56];
            this.NumbTilesOnBoard = 0;
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

        #region CanPlaceTileOnGameBoard
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
                placableTile = new TriominoTile(tileName, TileOrientation.Straight, new Point(23, 23));
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

            TileOrientation tileOrientation = this.GetTileOrienationFromOtherTileOrientationAndFaces(this.GetTileFromGrid(otherTileGridCoordinates).Orientation, otherFace.Value, tileFace.Value);
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
        internal TileOrientation GetTileOrienationFromOtherTileOrientationAndFaces(TileOrientation otherOrientation, TileFace otherFace, TileFace tileFace)
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

        #region TryAddTile
        /// <summary>
        /// Adds a tile on the GameBoard, based on the tiles Value, another Tile on the GameBoard besides which the new Tile should be placed
        /// and the faces with which both tiles should be placed towards each other.
        /// </summary>
        /// <param name="tileName">The new tile.</param>
        /// <param name="otherName">The other tile besides which the new tile should be placed.</param>
        /// <param name="tileFace">The new tiles face wich points to the other tile.</param>
        /// <param name="otherFace">The other tiles face which points to the new tile.</param>
        /// <returns></returns>
        public bool TryAddTile(string tileName, string otherName = null, TileFace? tileFace = null, TileFace? otherFace = null)
        {
            tileName.EnsureTriominoTileName();

            if (!this.CanPlaceTileOnGameBoard(tileName, otherName, tileFace, otherFace, out TriominoTile placeableTile))
            {
                return false;
            }

            this.AddTile(placeableTile);

            return true;
        }
        #endregion

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
    }
}