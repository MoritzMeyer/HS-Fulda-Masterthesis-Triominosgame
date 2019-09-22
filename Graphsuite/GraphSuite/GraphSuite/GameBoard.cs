using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Drawing;

namespace GraphSuite
{
    public class GameBoard
    {
        private int?[,] tileValueGrid;
        private TriominoTile[,] tileGrid;

        public int NumbTilesOnBoard { get; private set; }

        public GameBoard()
        {
            this.InitTriominosGameBoard();
            this.NumbTilesOnBoard = 0;
        }

        public void InitTriominosGameBoard()
        {
            this.tileValueGrid = new int?[57, 57];
            this.tileGrid = new TriominoTile[56, 56];
        }

        #region CanPlaceTileOnGameBoard
        /// <summary>
        /// Checks if a tile can placed on the GameBoard, based on the tiles orientation, 
        /// and another tile next to which the new tile should be placed
        /// </summary>
        /// <param name="tile">The tile which should be placed</param>
        /// <param name="otherName">The name of the other tile.</param>
        /// <param name="otherFace">The face of the other tile at which the new tile should be placed.</param>
        /// <returns>True, if the new tile can be placed, false if not.</returns>
        public bool CanPlaceTileOnGameBoard(TriominoTile tile, string otherName, TileFace? otherFace)
        {
            // if it's the first tile it can always be placed.
            if (this.NumbTilesOnBoard == 0)
            {
                return true;
            }

            // otherFace and otherName can only be null if it's the first tile
            if (otherFace == null || otherName == null || otherName == string.Empty)
            {
                return false;
            }

            Point otherTileGridCoordinates = this.GetTileCoordsByName(otherName);
            Point possibleNewGridCoordinates = this.GetTileGridPositionFromOtherTilePositionAndFace(otherTileGridCoordinates, otherFace.Value);

            return this.CheckValueGridAtTileGridPosition(tile, possibleNewGridCoordinates);
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
                for (int j = 0; j < this.tileGrid.GetLength(1); i++)
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

        #region GetTileGridPositionFromOtherTilePositionAndFace
        /// <summary>
        /// Determines a place in the tileGrid for a tile according to anothers (other) tile 
        /// coordinates and its face on which the new tile should be placed.
        /// </summary>
        /// <param name="otherTileTileGridCoords">The others tile coordinates of the tileGrid.</param>
        /// <param name="otherFace">The others tile face, on which the new tile should be placed.</param>
        /// <returns>The coordniates of the tile Grid, next to the other tile.</returns>
        private Point GetTileGridPositionFromOtherTilePositionAndFace(Point otherTileTileGridCoords, TileFace otherFace)
        {
            Point thisTileGridPosition = new Point(-1, -1);

            Tuple<TileOrientation, TileFace> orientationFaceCombination = new Tuple<TileOrientation, TileFace>(
                this.tileGrid[otherTileTileGridCoords.Y, otherTileTileGridCoords.X].Orientation,
                otherFace);

            if ((orientationFaceCombination.Item1 == TileOrientation.Straight && orientationFaceCombination.Item2 == TileFace.Left) ||
                (orientationFaceCombination.Item1 == TileOrientation.TiltLeft && orientationFaceCombination.Item2 == TileFace.Left) ||
                (orientationFaceCombination.Item1 == TileOrientation.DoubleTiltLeft && orientationFaceCombination.Item2 == TileFace.Right) ||
                (orientationFaceCombination.Item1 == TileOrientation.Flipped && orientationFaceCombination.Item2 == TileFace.Right) ||
                (orientationFaceCombination.Item1 == TileOrientation.DoubleTiltRight && orientationFaceCombination.Item2 == TileFace.Bottom) ||
                (orientationFaceCombination.Item1 == TileOrientation.TiltRight && orientationFaceCombination.Item2 == TileFace.Bottom))

            {
                thisTileGridPosition.X = otherTileTileGridCoords.X - 1;
                thisTileGridPosition.Y = otherTileTileGridCoords.Y;
            }
            else if ((orientationFaceCombination.Item1 == TileOrientation.Straight && orientationFaceCombination.Item2 == TileFace.Right) ||
                     (orientationFaceCombination.Item1 == TileOrientation.TiltLeft && orientationFaceCombination.Item2 == TileFace.Bottom) ||
                     (orientationFaceCombination.Item1 == TileOrientation.DoubleTiltLeft && orientationFaceCombination.Item2 == TileFace.Bottom) ||
                     (orientationFaceCombination.Item1 == TileOrientation.Flipped && orientationFaceCombination.Item2 == TileFace.Left) ||
                     (orientationFaceCombination.Item1 == TileOrientation.DoubleTiltRight && orientationFaceCombination.Item2 == TileFace.Left) ||
                     (orientationFaceCombination.Item1 == TileOrientation.TiltRight && orientationFaceCombination.Item2 == TileFace.Right))
            {
                thisTileGridPosition.X = otherTileTileGridCoords.X + 1;
                thisTileGridPosition.Y = otherTileTileGridCoords.Y;
            }
            else if ((orientationFaceCombination.Item1 == TileOrientation.Straight && orientationFaceCombination.Item2 == TileFace.Bottom) ||
                     (orientationFaceCombination.Item1 == TileOrientation.Flipped && orientationFaceCombination.Item2 == TileFace.Bottom) ||
                     (orientationFaceCombination.Item1 == TileOrientation.TiltRight && orientationFaceCombination.Item2 == TileFace.Left))
            {
                thisTileGridPosition.X = otherTileTileGridCoords.X;
                thisTileGridPosition.Y = otherTileTileGridCoords.Y - 1;
            }
            else if ((orientationFaceCombination.Item1 == TileOrientation.TiltLeft && orientationFaceCombination.Item2 == TileFace.Right) ||
                     (orientationFaceCombination.Item1 == TileOrientation.DoubleTiltLeft && orientationFaceCombination.Item2 == TileFace.Left) ||
                     (orientationFaceCombination.Item1 == TileOrientation.DoubleTiltRight && orientationFaceCombination.Item2 == TileFace.Right))
            {
                thisTileGridPosition.X = otherTileTileGridCoords.X;
                thisTileGridPosition.Y = otherTileTileGridCoords.Y + 1;
            }

            //if (thisTileGridPosition.X == -1 || thisTileGridPosition.Y == -1)
            //{
            //    throw new InvalidCastException($"Cannot determine on which side of 'otherTile' ({otherName}) the actual tile ({tile.Name}) should be placed");
            //}

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
        private TileOrientation GetTileOrienationFromOtherTileOrientationAndFaces(TileOrientation otherOrientation, TileFace otherFace, TileFace tileFace)
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
                     (otherOrientation == TileOrientation.Straight && otherFace == TileFace.Bottom && tileFace == TileFace.Right) ||
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
                     (otherOrientation == TileOrientation.Straight && otherFace == TileFace.Bottom && tileFace == TileFace.Left) ||
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
            else if((otherOrientation == TileOrientation.TiltLeft && otherFace == TileFace.Left && tileFace == TileFace.Bottom) ||
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

        //public bool TryAddTile(TriominoTile tile, string otherName, TileFace? otherFace)
        public bool TryAddTile(string tileName, string otherName, TileFace? tileFace, TileFace? otherFace)
        {
            // TODO: CanPlacetileOnGameBoard an diesen Funktionskopf anpassen und hier aufrufen.

            TriominoTile.EnsureName(tileName);

            TriominoTile tile = null;
            if (this.NumbTilesOnBoard == 0)
            {
                tile = new TriominoTile(tileName, TileOrientation.Straight);
                return this.AddTile(tile, new Point(23, 23));
            }

            if (tileFace == null || otherFace == null || otherName == null || otherName == string.Empty)
            {
                throw new ArgumentException("Values for Parameter 'otherName' and 'otherFace' can't be null, if it isn't the first turn.");
            }
                      
            Point otherTileGridCoordinates = this.GetTileCoordsByName(otherName);
            TileOrientation tileOrientationOnGameBoard = this.GetTileOrienationFromOtherTileOrientationAndFaces(this.tileGrid[otherTileGridCoordinates.Y, otherTileGridCoordinates.X].Orientation, otherFace.Value, tileFace.Value);

            tile = new TriominoTile(tileName, tileOrientationOnGameBoard);

            Point possibleNewGridCoordinates = this.GetTileGridPositionFromOtherTilePositionAndFace(otherTileGridCoordinates, otherFace.Value);
            

            return this.AddTile(tile, possibleNewGridCoordinates);
        }

        private bool AddTile(TriominoTile tile, Point tileGridPosition)
        {
            if (this.tileGrid[tileGridPosition.Y, tileGridPosition.X] != null)
            {
                throw new ArgumentException($"Cannot add tile {tile.Name} at position(x,y): ({tileGridPosition.X}, {tileGridPosition.Y}). Place is taken by tile: '{this.tileGrid[tileGridPosition.Y, tileGridPosition.X].Name}'");
            }

            if (!this.CheckValueGridAtTileGridPosition(tile, tileGridPosition))
            {
                return false;
            }

            this.tileGrid[tileGridPosition.Y, tileGridPosition.X] = tile;
            this.AddValuesToValueGrid(tile.GetArrayName(), tileGridPosition, tile.Orientation.ToArrayTileOrientation());

            return true;
        }

        #region CheckValueGridAtTileGridPosition
        /// <summary>
        /// Checks the values for a tile at a specific tileGridPosition. 
        /// If one value doesn't match the value within the tiles name false is returned, 
        /// while one null-value of the tileValueGrid values is allowed.
        /// </summary>
        /// <param name="tile">The tile whose values should be checked.</param>
        /// <param name="tileGridPosition">The tiles position within the tileGrid.</param>
        /// <returns>True, if values matching (one tileValueGrid value can be null), false if not.</returns>
        private bool CheckValueGridAtTileGridPosition(TriominoTile tile, Point tileGridPosition)
        {
            List<int?> valuesToCheck = this.GetValueGridValuesFromNameGridPositions(tileGridPosition, tile.Orientation.ToArrayTileOrientation());

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

            switch(orientation)
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

            switch(orientation)
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
