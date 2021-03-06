﻿using GraphKI.GameManagement;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GraphKI.Extensions;

namespace GraphKITest.GameManagement
{
    [TestClass]
    public class GameManagerTest
    {
        #region DrawTile_functionality_has_to_work
        /// <summary>
        /// Verifies all methods and functionality about the DrawTile process.
        /// </summary>
        [TestMethod]
        public void DrawTile_functionality_has_to_work()
        {
            int nextTurnCount = 0;
            GameManager gameManager = new GameManager(GameMode.ThreePlayer);
            Assert.AreEqual(3, gameManager.DrawBoards.Count());
            gameManager = new GameManager(GameMode.FourPlayer);
            Assert.AreEqual(4, gameManager.DrawBoards.Count());
            gameManager = new GameManager(GameMode.TwoPlayer);
            Assert.AreEqual(2, gameManager.DrawBoards.Count());

            gameManager.NextTurnEvent += (sender, e) =>
            {
                nextTurnCount++;
            };

            Assert.AreEqual(0, gameManager.TurnCount);
            Assert.IsTrue(gameManager.TryDrawTile(out string randomTile));
            Assert.IsTrue(gameManager.GetActivePlayersDrawBoard().HasTile(randomTile));
            Assert.AreEqual(1, gameManager.DrawCount);
            Assert.IsFalse(gameManager.TryNextTurn());

            Assert.IsTrue(gameManager.TryDrawTile(out randomTile));
            Assert.IsTrue(gameManager.GetActivePlayersDrawBoard().HasTile(randomTile));
            Assert.AreEqual(2, gameManager.DrawCount);
            Assert.IsTrue(gameManager.TryDrawTile(out randomTile));
            Assert.IsTrue(gameManager.GetActivePlayersDrawBoard().HasTile(randomTile));
            Assert.AreEqual(3, gameManager.DrawCount);

            Assert.IsFalse(gameManager.TryDrawTile(out randomTile));
            Assert.IsFalse(gameManager.CanDrawTile());

            PlayerCode activePlayer = gameManager.ActivePlayer;
            Assert.IsTrue(gameManager.TryNextTurn());
            Assert.AreEqual(gameManager.TurnCount, nextTurnCount);
            Assert.AreNotEqual(activePlayer, gameManager.ActivePlayer);
            Assert.AreEqual(-25, gameManager.PlayerPoints[activePlayer]);
            Assert.AreEqual(0, gameManager.PlayerPoints[gameManager.ActivePlayer]);

            PlayerCode expectedPlayer = (activePlayer == PlayerCode.Player1) ? PlayerCode.Player2 : PlayerCode.Player1;
            Assert.AreEqual(expectedPlayer, gameManager.ActivePlayer);
        }
        #endregion

        #region PlaceTile_functionality_has_to_work
        /// <summary>
        /// Verifies all methods and functionality about the PlaceTile process.
        /// </summary>
        [TestMethod]
        public void PlaceTile_functionality_has_to_work()
        {
            int nextTurnCount = 0;
            GameManager gameManager = new GameManager(GameMode.TwoPlayer);
            gameManager.NextTurnEvent += (sender, e) => { nextTurnCount++; };

            List<string> allTiles = new List<string>(gameManager.InitTilePool());

            string tile = new List<string>(gameManager.TilePool).First();
            Assert.ThrowsException<InvalidOperationException>(() => { gameManager.TryPlaceOnGameBoard(tile); }, "Only the active player can place tiles on the gameboard.");

            string tileFromActiveDrawboard = allTiles.Where(t => gameManager.GetActivePlayersDrawBoard().HasTile(t)).ElementAt(0);
            Assert.IsTrue(gameManager.TryPlaceOnGameBoard(tileFromActiveDrawboard));
            Assert.AreEqual(1, gameManager.TurnCount);
            Assert.AreEqual(gameManager.TurnCount, nextTurnCount);
            Assert.IsFalse(gameManager.HasPlaced);

            PlayerCode firstPlayer = (gameManager.ActivePlayer == PlayerCode.Player1) ? PlayerCode.Player2 : PlayerCode.Player1;
            Assert.AreEqual(tileFromActiveDrawboard.GetTriominoTileValue(), gameManager.PlayerPoints[firstPlayer]);
            Assert.IsFalse(gameManager.DrawBoards[firstPlayer].HasTile(tileFromActiveDrawboard));
            Assert.AreEqual(0, gameManager.PlayerPoints[gameManager.ActivePlayer]);
        }
        #endregion

        #region DrawSpecificTile_has_to_work
        /// <summary>
        /// Verifies the Method DrawSpecificTile for its output
        /// </summary>
        [TestMethod]
        public void DrawSpecificTile_has_to_work()
        {
            GameManager gameManager = new GameManager(GameMode.TwoPlayer);
            List<string> tilePool = new List<string>(gameManager.TilePool);
            string tile1 = tilePool[0];
            string tile2 = tilePool[1];
            string tile3 = tilePool[2];

            Assert.IsTrue(gameManager.DrawSpecificTile(tile1, gameManager.DrawBoards[PlayerCode.Player1]));
            Assert.IsTrue(gameManager.DrawSpecificTile(tile2, gameManager.DrawBoards[PlayerCode.Player1]));
            Assert.IsTrue(gameManager.DrawSpecificTile(tile3, gameManager.DrawBoards[PlayerCode.Player1]));

            Assert.IsTrue(gameManager.DrawBoards[PlayerCode.Player1].HasTile(tile1));
            Assert.IsTrue(gameManager.DrawBoards[PlayerCode.Player1].HasTile(tile2));
            Assert.IsTrue(gameManager.DrawBoards[PlayerCode.Player1].HasTile(tile3));

            Assert.IsFalse(gameManager.DrawSpecificTile(tile1, gameManager.DrawBoards[PlayerCode.Player1]));
        }
        #endregion
    }
}
