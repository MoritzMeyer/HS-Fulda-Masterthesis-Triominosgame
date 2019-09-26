using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using GraphKI.GameManagement;
using System.Drawing;

namespace GraphKITest
{
    [TestClass]
    public class GameBoardTests
    {
        #region GameBoard_GetTileOrienationFromOtherTileOrientationAndFaces_has_to_work
        /// <summary>
        /// Tests function GetTileOrienationFromOtherTileOrientationAndFaces for its output.
        /// </summary>
        [TestMethod]
        public void GameBoard_GetTileOrienationFromOtherTileOrientationAndFaces_has_to_work()
        {
            GameBoard gameBoard = new GameBoard();

            // Other Orientation = Straight
            // Other Face = Left
            Assert.AreEqual(expected: TileOrientation.Flipped, actual: gameBoard.GetTileOrienationFromOtherTileOrientationAndFaces(otherOrientation: TileOrientation.Straight, otherFace: TileFace.Left, tileFace: TileFace.Left));
            Assert.AreEqual(expected: TileOrientation.TiltRight, actual: gameBoard.GetTileOrienationFromOtherTileOrientationAndFaces(otherOrientation: TileOrientation.Straight, otherFace: TileFace.Left, tileFace: TileFace.Right));
            Assert.AreEqual(expected: TileOrientation.TiltLeft, actual: gameBoard.GetTileOrienationFromOtherTileOrientationAndFaces(otherOrientation: TileOrientation.Straight, otherFace: TileFace.Left, tileFace: TileFace.Bottom));

            // Other Face = Right
            Assert.AreEqual(expected: TileOrientation.TiltLeft, actual: gameBoard.GetTileOrienationFromOtherTileOrientationAndFaces(otherOrientation: TileOrientation.Straight, otherFace: TileFace.Right, tileFace: TileFace.Left));
            Assert.AreEqual(expected: TileOrientation.Flipped, actual: gameBoard.GetTileOrienationFromOtherTileOrientationAndFaces(otherOrientation: TileOrientation.Straight, otherFace: TileFace.Right, tileFace: TileFace.Right));
            Assert.AreEqual(expected: TileOrientation.TiltRight, actual: gameBoard.GetTileOrienationFromOtherTileOrientationAndFaces(otherOrientation: TileOrientation.Straight, otherFace: TileFace.Right, tileFace: TileFace.Bottom));            

            // Other Face = Bottom
            Assert.AreEqual(expected: TileOrientation.TiltRight, actual: gameBoard.GetTileOrienationFromOtherTileOrientationAndFaces(otherOrientation: TileOrientation.Straight, otherFace: TileFace.Bottom, tileFace: TileFace.Left));
            Assert.AreEqual(expected: TileOrientation.TiltLeft, actual: gameBoard.GetTileOrienationFromOtherTileOrientationAndFaces(otherOrientation: TileOrientation.Straight, otherFace: TileFace.Bottom, tileFace: TileFace.Right));
            Assert.AreEqual(expected: TileOrientation.Flipped, actual: gameBoard.GetTileOrienationFromOtherTileOrientationAndFaces(otherOrientation: TileOrientation.Straight, otherFace: TileFace.Bottom, tileFace: TileFace.Bottom));

            // Other Orientation = TiltLeft
            // Other Face = Left
            Assert.AreEqual(expected: TileOrientation.DoubleTiltRight, actual: gameBoard.GetTileOrienationFromOtherTileOrientationAndFaces(otherOrientation: TileOrientation.TiltLeft, otherFace: TileFace.Left, tileFace: TileFace.Left));
            Assert.AreEqual(expected: TileOrientation.Straight, actual: gameBoard.GetTileOrienationFromOtherTileOrientationAndFaces(otherOrientation: TileOrientation.TiltLeft, otherFace: TileFace.Left, tileFace: TileFace.Right));
            Assert.AreEqual(expected: TileOrientation.DoubleTiltLeft, actual: gameBoard.GetTileOrienationFromOtherTileOrientationAndFaces(otherOrientation: TileOrientation.TiltLeft, otherFace: TileFace.Left, tileFace: TileFace.Bottom));

            // Other Face = Right
            Assert.AreEqual(expected: TileOrientation.DoubleTiltLeft, actual: gameBoard.GetTileOrienationFromOtherTileOrientationAndFaces(otherOrientation: TileOrientation.TiltLeft, otherFace: TileFace.Right, tileFace: TileFace.Left));
            Assert.AreEqual(expected: TileOrientation.DoubleTiltRight, actual: gameBoard.GetTileOrienationFromOtherTileOrientationAndFaces(otherOrientation: TileOrientation.TiltLeft, otherFace: TileFace.Right, tileFace: TileFace.Right));
            Assert.AreEqual(expected: TileOrientation.Straight, actual: gameBoard.GetTileOrienationFromOtherTileOrientationAndFaces(otherOrientation: TileOrientation.TiltLeft, otherFace: TileFace.Right, tileFace: TileFace.Bottom));

            // Other Face = Bottom
            Assert.AreEqual(expected: TileOrientation.Straight, actual: gameBoard.GetTileOrienationFromOtherTileOrientationAndFaces(otherOrientation: TileOrientation.TiltLeft, otherFace: TileFace.Bottom, tileFace: TileFace.Left));
            Assert.AreEqual(expected: TileOrientation.DoubleTiltLeft, actual: gameBoard.GetTileOrienationFromOtherTileOrientationAndFaces(otherOrientation: TileOrientation.TiltLeft, otherFace: TileFace.Bottom, tileFace: TileFace.Right));
            Assert.AreEqual(expected: TileOrientation.DoubleTiltRight, actual: gameBoard.GetTileOrienationFromOtherTileOrientationAndFaces(otherOrientation: TileOrientation.TiltLeft, otherFace: TileFace.Bottom, tileFace: TileFace.Bottom));

            // Other Orientation = DoubleTiltLeft
            // Other Face = Left
            Assert.AreEqual(expected: TileOrientation.TiltRight, actual: gameBoard.GetTileOrienationFromOtherTileOrientationAndFaces(otherOrientation: TileOrientation.DoubleTiltLeft, otherFace: TileFace.Left, tileFace: TileFace.Left));
            Assert.AreEqual(expected: TileOrientation.TiltLeft, actual: gameBoard.GetTileOrienationFromOtherTileOrientationAndFaces(otherOrientation: TileOrientation.DoubleTiltLeft, otherFace: TileFace.Left, tileFace: TileFace.Right));
            Assert.AreEqual(expected: TileOrientation.Flipped, actual: gameBoard.GetTileOrienationFromOtherTileOrientationAndFaces(otherOrientation: TileOrientation.DoubleTiltLeft, otherFace: TileFace.Left, tileFace: TileFace.Bottom));

            // Other Face = Right
            Assert.AreEqual(expected: TileOrientation.Flipped, actual: gameBoard.GetTileOrienationFromOtherTileOrientationAndFaces(otherOrientation: TileOrientation.DoubleTiltLeft, otherFace: TileFace.Right, tileFace: TileFace.Left));
            Assert.AreEqual(expected: TileOrientation.TiltRight, actual: gameBoard.GetTileOrienationFromOtherTileOrientationAndFaces(otherOrientation: TileOrientation.DoubleTiltLeft, otherFace: TileFace.Right, tileFace: TileFace.Right));
            Assert.AreEqual(expected: TileOrientation.TiltLeft, actual: gameBoard.GetTileOrienationFromOtherTileOrientationAndFaces(otherOrientation: TileOrientation.DoubleTiltLeft, otherFace: TileFace.Right, tileFace: TileFace.Bottom));

            // Other Face = Bottom
            Assert.AreEqual(expected: TileOrientation.TiltLeft, actual: gameBoard.GetTileOrienationFromOtherTileOrientationAndFaces(otherOrientation: TileOrientation.DoubleTiltLeft, otherFace: TileFace.Bottom, tileFace: TileFace.Left));
            Assert.AreEqual(expected: TileOrientation.Flipped, actual: gameBoard.GetTileOrienationFromOtherTileOrientationAndFaces(otherOrientation: TileOrientation.DoubleTiltLeft, otherFace: TileFace.Bottom, tileFace: TileFace.Right));
            Assert.AreEqual(expected: TileOrientation.TiltRight, actual: gameBoard.GetTileOrienationFromOtherTileOrientationAndFaces(otherOrientation: TileOrientation.DoubleTiltLeft, otherFace: TileFace.Bottom, tileFace: TileFace.Bottom));

            // Other Orientation = Flipped
            // Other Face = Left
            Assert.AreEqual(expected: TileOrientation.Straight, actual: gameBoard.GetTileOrienationFromOtherTileOrientationAndFaces(otherOrientation: TileOrientation.Flipped, otherFace: TileFace.Left, tileFace: TileFace.Left));
            Assert.AreEqual(expected: TileOrientation.DoubleTiltLeft, actual: gameBoard.GetTileOrienationFromOtherTileOrientationAndFaces(otherOrientation: TileOrientation.Flipped, otherFace: TileFace.Left, tileFace: TileFace.Right));
            Assert.AreEqual(expected: TileOrientation.DoubleTiltRight, actual: gameBoard.GetTileOrienationFromOtherTileOrientationAndFaces(otherOrientation: TileOrientation.Flipped, otherFace: TileFace.Left, tileFace: TileFace.Bottom));

            // Other Face = Right
            Assert.AreEqual(expected: TileOrientation.DoubleTiltRight, actual: gameBoard.GetTileOrienationFromOtherTileOrientationAndFaces(otherOrientation: TileOrientation.Flipped, otherFace: TileFace.Right, tileFace: TileFace.Left));
            Assert.AreEqual(expected: TileOrientation.Straight, actual: gameBoard.GetTileOrienationFromOtherTileOrientationAndFaces(otherOrientation: TileOrientation.Flipped, otherFace: TileFace.Right, tileFace: TileFace.Right));
            Assert.AreEqual(expected: TileOrientation.DoubleTiltLeft, actual: gameBoard.GetTileOrienationFromOtherTileOrientationAndFaces(otherOrientation: TileOrientation.Flipped, otherFace: TileFace.Right, tileFace: TileFace.Bottom));

            // Other Face = Bottom
            Assert.AreEqual(expected: TileOrientation.DoubleTiltLeft, actual: gameBoard.GetTileOrienationFromOtherTileOrientationAndFaces(otherOrientation: TileOrientation.Flipped, otherFace: TileFace.Bottom, tileFace: TileFace.Left));
            Assert.AreEqual(expected: TileOrientation.DoubleTiltRight, actual: gameBoard.GetTileOrienationFromOtherTileOrientationAndFaces(otherOrientation: TileOrientation.Flipped, otherFace: TileFace.Bottom, tileFace: TileFace.Right));
            Assert.AreEqual(expected: TileOrientation.Straight, actual: gameBoard.GetTileOrienationFromOtherTileOrientationAndFaces(otherOrientation: TileOrientation.Flipped, otherFace: TileFace.Bottom, tileFace: TileFace.Bottom));

            // Other Orientation = DoubleTiltRight
            // Other Face = Left
            Assert.AreEqual(expected: TileOrientation.TiltLeft, actual: gameBoard.GetTileOrienationFromOtherTileOrientationAndFaces(otherOrientation: TileOrientation.DoubleTiltRight, otherFace: TileFace.Left, tileFace: TileFace.Left));
            Assert.AreEqual(expected: TileOrientation.Flipped, actual: gameBoard.GetTileOrienationFromOtherTileOrientationAndFaces(otherOrientation: TileOrientation.DoubleTiltRight, otherFace: TileFace.Left, tileFace: TileFace.Right));
            Assert.AreEqual(expected: TileOrientation.TiltRight, actual: gameBoard.GetTileOrienationFromOtherTileOrientationAndFaces(otherOrientation: TileOrientation.DoubleTiltRight, otherFace: TileFace.Left, tileFace: TileFace.Bottom));

            // Other Face = Right
            Assert.AreEqual(expected: TileOrientation.TiltRight, actual: gameBoard.GetTileOrienationFromOtherTileOrientationAndFaces(otherOrientation: TileOrientation.DoubleTiltRight, otherFace: TileFace.Right, tileFace: TileFace.Left));
            Assert.AreEqual(expected: TileOrientation.TiltLeft, actual: gameBoard.GetTileOrienationFromOtherTileOrientationAndFaces(otherOrientation: TileOrientation.DoubleTiltRight, otherFace: TileFace.Right, tileFace: TileFace.Right));
            Assert.AreEqual(expected: TileOrientation.Flipped, actual: gameBoard.GetTileOrienationFromOtherTileOrientationAndFaces(otherOrientation: TileOrientation.DoubleTiltRight, otherFace: TileFace.Right, tileFace: TileFace.Bottom));

            // Other Face = Bottom
            Assert.AreEqual(expected: TileOrientation.Flipped, actual: gameBoard.GetTileOrienationFromOtherTileOrientationAndFaces(otherOrientation: TileOrientation.DoubleTiltRight, otherFace: TileFace.Bottom, tileFace: TileFace.Left));
            Assert.AreEqual(expected: TileOrientation.TiltRight, actual: gameBoard.GetTileOrienationFromOtherTileOrientationAndFaces(otherOrientation: TileOrientation.DoubleTiltRight, otherFace: TileFace.Bottom, tileFace: TileFace.Right));
            Assert.AreEqual(expected: TileOrientation.TiltLeft, actual: gameBoard.GetTileOrienationFromOtherTileOrientationAndFaces(otherOrientation: TileOrientation.DoubleTiltRight, otherFace: TileFace.Bottom, tileFace: TileFace.Bottom));

            // Other Orientation = TiltRight
            // Other Face = Left
            Assert.AreEqual(expected: TileOrientation.DoubleTiltLeft, actual: gameBoard.GetTileOrienationFromOtherTileOrientationAndFaces(otherOrientation: TileOrientation.TiltRight, otherFace: TileFace.Left, tileFace: TileFace.Left));
            Assert.AreEqual(expected: TileOrientation.DoubleTiltRight, actual: gameBoard.GetTileOrienationFromOtherTileOrientationAndFaces(otherOrientation: TileOrientation.TiltRight, otherFace: TileFace.Left, tileFace: TileFace.Right));
            Assert.AreEqual(expected: TileOrientation.Straight, actual: gameBoard.GetTileOrienationFromOtherTileOrientationAndFaces(otherOrientation: TileOrientation.TiltRight, otherFace: TileFace.Left, tileFace: TileFace.Bottom));

            // Other Face = Right
            Assert.AreEqual(expected: TileOrientation.Straight, actual: gameBoard.GetTileOrienationFromOtherTileOrientationAndFaces(otherOrientation: TileOrientation.TiltRight, otherFace: TileFace.Right, tileFace: TileFace.Left));
            Assert.AreEqual(expected: TileOrientation.DoubleTiltLeft, actual: gameBoard.GetTileOrienationFromOtherTileOrientationAndFaces(otherOrientation: TileOrientation.TiltRight, otherFace: TileFace.Right, tileFace: TileFace.Right));
            Assert.AreEqual(expected: TileOrientation.DoubleTiltRight, actual: gameBoard.GetTileOrienationFromOtherTileOrientationAndFaces(otherOrientation: TileOrientation.TiltRight, otherFace: TileFace.Right, tileFace: TileFace.Bottom));

            // Other Face = Bottom
            Assert.AreEqual(expected: TileOrientation.DoubleTiltRight, actual: gameBoard.GetTileOrienationFromOtherTileOrientationAndFaces(otherOrientation: TileOrientation.TiltRight, otherFace: TileFace.Bottom, tileFace: TileFace.Left));
            Assert.AreEqual(expected: TileOrientation.Straight, actual: gameBoard.GetTileOrienationFromOtherTileOrientationAndFaces(otherOrientation: TileOrientation.TiltRight, otherFace: TileFace.Bottom, tileFace: TileFace.Right));
            Assert.AreEqual(expected: TileOrientation.DoubleTiltLeft, actual: gameBoard.GetTileOrienationFromOtherTileOrientationAndFaces(otherOrientation: TileOrientation.TiltRight, otherFace: TileFace.Bottom, tileFace: TileFace.Bottom));
        }
        #endregion

        #region GameBoard_TryAddTile_has_to_work
        /// <summary>
        /// Tests function TryAddTile
        /// </summary>
        [TestMethod]
        public void GameBoard_TryAddTile_has_to_work()
        {
            int tilesAdded = 0;
            GameBoard gameBoard = new GameBoard();
            gameBoard.TilePlaced += (object sender, TriominoTileEventArgs e) => { tilesAdded++; };

            Assert.IsTrue(gameBoard.TryAddTile(PlayerCode.None, "0-1-4"));
            Assert.IsTrue(gameBoard.TryAddTile(PlayerCode.None, "0-4-5", "0-1-4", TileFace.Right, TileFace.Left));
            Assert.IsTrue(gameBoard.TryAddTile(PlayerCode.None, "4-4-5", "0-4-5", TileFace.Left, TileFace.Bottom));
            Assert.IsTrue(gameBoard.TryAddTile(PlayerCode.None, "3-4-4", "4-4-5", TileFace.Bottom, TileFace.Right));
            Assert.IsTrue(gameBoard.TryAddTile(PlayerCode.None, "3-3-4", "3-4-4", TileFace.Bottom, TileFace.Left));
            Assert.IsTrue(gameBoard.TryAddTile(PlayerCode.None, "3-3-3", "3-3-4", TileFace.Left, TileFace.Right));
            Assert.IsTrue(gameBoard.TryAddTile(PlayerCode.None, "2-3-3", "3-3-3", TileFace.Bottom, TileFace.Right));
            Assert.IsTrue(gameBoard.TryAddTile(PlayerCode.None, "1-2-3", "2-3-3", TileFace.Bottom, TileFace.Left));
            Assert.IsTrue(gameBoard.TryAddTile(PlayerCode.None, "1-1-3", "1-2-3", TileFace.Bottom, TileFace.Left));
            Assert.IsTrue(gameBoard.TryAddTile(PlayerCode.None, "1-3-4", "1-1-3", TileFace.Right, TileFace.Left));

            Assert.IsTrue(gameBoard.TryAddTile(PlayerCode.None, "1-1-1", "1-1-3", TileFace.Bottom, TileFace.Right));
            Assert.IsTrue(gameBoard.TryAddTile(PlayerCode.None, "0-1-1", "1-1-1", TileFace.Bottom, TileFace.Right));
            Assert.IsTrue(gameBoard.TryAddTile(PlayerCode.None, "0-0-1", "0-1-1", TileFace.Bottom, TileFace.Left));
            Assert.IsTrue(gameBoard.TryAddTile(PlayerCode.None, "0-1-2", "0-0-1", TileFace.Right, TileFace.Left));
            Assert.IsTrue(gameBoard.TryAddTile(PlayerCode.None, "1-2-2", "0-1-2",TileFace.Left, TileFace.Bottom));
            Assert.IsTrue(gameBoard.TryAddTile(PlayerCode.None, "1-1-2", "1-1-1", TileFace.Right, TileFace.Left));

            Assert.IsFalse(gameBoard.TryAddTile(PlayerCode.None, "1-3-3", "1-1-3", TileFace.Right, TileFace.Left));
            Assert.IsFalse(gameBoard.TryAddTile(PlayerCode.None, "0-4-4", "0-1-4", TileFace.Right, TileFace.Left));

            Assert.AreEqual(16, tilesAdded);
        }
        #endregion

        #region GameBoard_GetTileGridPositionFromOtherTilePositionAndFace_has_to_work
        /// <summary>
        /// Tests function GetTileGridPositionFromOtherTilePositionAndFace for its output.
        /// </summary>
        [TestMethod]
        public void GameBoard_GetTileGridPositionFromOtherTilePositionAndFace_has_to_work()
        {
            GameBoard gameBoard = new GameBoard();

            // Check all Sides of a Tile placed BottomUp
            Assert.IsTrue(gameBoard.TryAddTile(PlayerCode.None, "0-1-1"));
            Assert.AreEqual(new Point(23, 24), gameBoard.GetTileGridPositionFromOtherTilePositionAndFace(new Point(23, 23), TileFace.Bottom));
            Assert.AreEqual(new Point(24, 23), gameBoard.GetTileGridPositionFromOtherTilePositionAndFace(new Point(23, 23), TileFace.Right));
            Assert.AreEqual(new Point(22, 23), gameBoard.GetTileGridPositionFromOtherTilePositionAndFace(new Point(23, 23), TileFace.Left));

            // Check all Sides of a Tile placed TopDown
            Assert.IsTrue(gameBoard.TryAddTile(PlayerCode.None, "0-0-1", "0-1-1", TileFace.Left, TileFace.Right));
            Assert.IsTrue(gameBoard.TryAddTile(PlayerCode.None, "0-1-2", "0-1-1", TileFace.Right, TileFace.Left));
            Assert.AreEqual(new Point(25, 23), gameBoard.GetTileGridPositionFromOtherTilePositionAndFace(new Point(24, 23), TileFace.Bottom));
            Assert.AreEqual(new Point(21, 23), gameBoard.GetTileGridPositionFromOtherTilePositionAndFace(new Point(22, 23), TileFace.Bottom));
            Assert.AreEqual(new Point(22, 22), gameBoard.GetTileGridPositionFromOtherTilePositionAndFace(new Point(22, 23), TileFace.Left));

        }
        #endregion

        #region TilesWithFreeFaces_functionality_has_to_work
        /// <summary>
        /// Verifies all methods and functionality about the TilesWithFreeFaces process.
        /// </summary>
        [TestMethod]
        public void TilesWithFreeFaces_functionality_has_to_work()
        {
            GameBoard gameBoard = new GameBoard();
            Assert.IsTrue(gameBoard.TryAddTile(PlayerCode.None, "1-2-3"));
            Assert.IsTrue(gameBoard.tilesWithFreeFaces["1-2-3"].Contains(TileFace.Right));
            Assert.IsTrue(gameBoard.tilesWithFreeFaces["1-2-3"].Contains(TileFace.Bottom));
            Assert.IsTrue(gameBoard.tilesWithFreeFaces["1-2-3"].Contains(TileFace.Left));

            Assert.IsTrue(gameBoard.TryAddTile(PlayerCode.None, "2-3-3", "1-2-3", TileFace.Left, TileFace.Bottom));
            Assert.IsTrue(gameBoard.tilesWithFreeFaces["1-2-3"].Contains(TileFace.Right));
            Assert.IsTrue(gameBoard.tilesWithFreeFaces["1-2-3"].Contains(TileFace.Left));
            Assert.IsTrue(gameBoard.tilesWithFreeFaces["2-3-3"].Contains(TileFace.Right));
            Assert.IsTrue(gameBoard.tilesWithFreeFaces["2-3-3"].Contains(TileFace.Bottom));

            Assert.IsFalse(gameBoard.tilesWithFreeFaces["1-2-3"].Contains(TileFace.Bottom));
            Assert.IsFalse(gameBoard.tilesWithFreeFaces["2-3-3"].Contains(TileFace.Left));
        }
        #endregion
    }
}
