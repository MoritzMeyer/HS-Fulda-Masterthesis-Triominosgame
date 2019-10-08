using GraphKI.GameManagement;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;

namespace GraphKITest.GameManagement
{
    [TestClass]
    public class DrawBoardTest
    {
        #region TryAddTile_and_Remove_functionality_has_to_work
        /// <summary>
        /// Verifies all methods and functionality about the TryAddTile and Remove processes.
        /// </summary>
        [TestMethod]
        public void TryAddTile_and_Remove_functionality_has_to_work()
        {
            int addTileCount = 0;
            DrawBoard drawBoard = new DrawBoard(PlayerCode.Player1);

            // EventListner for adding tiles
            drawBoard.TileAdded += (object sender, TriominoTileEventArgs e) =>
            {
                addTileCount++;
                if (addTileCount == 1)
                {
                    Assert.AreEqual("0-0-1", e.TileName);


                }
            };

            // EventListner for removing tiles.
            drawBoard.TileRemoved += (object sender, TriominoTileEventArgs e) =>
            {
                addTileCount--;
                if (addTileCount == 0)
                {
                    Assert.AreEqual("1-2-3", e.TileName);
                }
            };

            Assert.IsTrue(drawBoard.TryAddTile("0-0-1"));
            Assert.IsTrue(drawBoard.TryAddTile("0-1-1"));
            Assert.IsTrue(drawBoard.TryAddTile("0-1-2"));
            Assert.IsTrue(drawBoard.TryAddTile("1-2-3"));

            Assert.IsTrue(drawBoard.HasTile("0-0-1"));
            Assert.IsTrue(drawBoard.HasTile("0-1-1"));
            Assert.IsTrue(drawBoard.HasTile("0-1-2"));
            Assert.IsTrue(drawBoard.HasTile("1-2-3"));

            Assert.IsFalse(drawBoard.TryAddTile("1-2-3"));
            Assert.IsFalse(drawBoard.TryAddTile("1-0-0"));

            Assert.IsFalse(drawBoard.HasTile("1-0-0"));
            Assert.AreEqual(4, addTileCount);

            Assert.IsTrue(drawBoard.TryRemoveTile("0-0-1"));
            Assert.IsTrue(drawBoard.TryRemoveTile("0-1-1"));
            Assert.IsTrue(drawBoard.TryRemoveTile("0-1-2"));
            Assert.IsTrue(drawBoard.TryRemoveTile("1-2-3"));

            Assert.IsFalse(drawBoard.HasTile("0-0-1"));
            Assert.IsFalse(drawBoard.HasTile("0-1-1"));
            Assert.IsFalse(drawBoard.HasTile("0-1-2"));
            Assert.IsFalse(drawBoard.HasTile("1-2-3"));

            Assert.AreEqual(0, addTileCount);
        }
        #endregion

        #region TryGetHighestTripleTriomino_has_to_work
        /// <summary>
        /// Verifies methods und functionalities abpit the TryGetHighestTripleTriomino processes.
        /// </summary>
        [TestMethod]
        public void TryGetHighestTripleTriomino_has_to_work()
        {
            DrawBoard drawBoard = new DrawBoard(PlayerCode.Player1);

            Assert.IsTrue(drawBoard.TryAddTile("0-1-1"));
            Assert.IsTrue(drawBoard.TryAddTile("0-1-2"));
            Assert.IsTrue(drawBoard.TryAddTile("1-2-3"));
            Assert.AreEqual(6, drawBoard.GetHighestTriominoValue());

            Assert.IsFalse(drawBoard.TryGetHighestTripleTriomino(out string highestTripleTriomino));
            Assert.IsNull(highestTripleTriomino);

            Assert.IsTrue(drawBoard.TryAddTile("0-0-0"));

            Assert.IsTrue(drawBoard.TryGetHighestTripleTriomino(out highestTripleTriomino));
            Assert.AreEqual("0-0-0", highestTripleTriomino);

            Assert.IsTrue(drawBoard.TryAddTile("1-1-1"));
            Assert.IsTrue(drawBoard.TryAddTile("3-3-3"));

            Assert.IsTrue(drawBoard.TryGetHighestTripleTriomino(out highestTripleTriomino));
            Assert.AreEqual("3-3-3", highestTripleTriomino);
            Assert.AreEqual(9, drawBoard.GetHighestTriominoValue());
        }
        #endregion
    }
}
