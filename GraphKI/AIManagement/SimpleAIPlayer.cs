using GraphKI.GameManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphKI.AIManagement
{
    public class SimpleAIPlayer : IAIPlayer
    {
        public GameManager GameManager { get; }
        public PlayerCode Player { get; }

        #region ctor
        /// <summary>
        /// Instantiates a new Object of this class.
        /// </summary>
        /// <param name="gameManager">GameManager of the game which this AI-Player is part of.</param>
        /// <param name="player">The AI-Players playerCode.</param>
        public SimpleAIPlayer(GameManager gameManager, PlayerCode player)
        {
            this.GameManager = gameManager;
            this.Player = player;

            gameManager.NextTurnEvent += this.PlayersTurn;
            gameManager.GameStartEvent += this.PlayersTurn;
        }
        #endregion

        private void PlayTurn()
        {
            // try to place a tile from drawboard.
            if (this.TryGetPlacableTileFromDrawBoard(out string placableTile, out string otherTile, out TileFace placalbleTileFace, out TileFace otherTileFace))
            {
                if (otherTile != null)
                {
                    this.GameManager.TryPlaceOnGameBoard(placableTile, otherTile, placalbleTileFace, otherTileFace);
                }
                else
                {
                    this.GameManager.TryPlaceOnGameBoard(placableTile);
                }

                return;
            }
            
            // draw up to 3 new tiles and try to place each time drawn.
            for (int i = 0; i < this.GameManager.MaxTileDrawCount; i++)
            {
                if (this.GameManager.TryDrawTile(out string drawnTile))
                {
                    if (this.CheckAllFreeFacesOnGameBoard(drawnTile, out otherTile, out placalbleTileFace, out otherTileFace))
                    {
                        this.GameManager.TryPlaceOnGameBoard(drawnTile, otherTile, placalbleTileFace, otherTileFace);
                        return;
                    }
                }
            }

            // no tile placemnt possible, next player.
            this.GameManager.TryNextTurn();
            return;
        }

        private bool CheckAllFreeFacesOnGameBoard(string tile, out string matchingTile, out TileFace tileFace, out TileFace matchingTileFace)
        {
            matchingTile = null;
            tileFace = TileFace.None;
            matchingTileFace = TileFace.None;

            foreach(KeyValuePair<string, List<TileFace>> kv in this.GameManager.GameBoard.tilesWithFreeFaces)
            {
                foreach(TileFace freeFace in kv.Value)
                {
                    if (this.CheckAllFacesForNewTileOnOtherTileFace(tile, kv.Key, freeFace, out tileFace))
                    {
                        matchingTile = kv.Key;
                        matchingTileFace = freeFace;
                        return true;
                    }
                }
            }

            return false;
        }

        private bool CheckAllFacesForNewTileOnOtherTileFace(string tileName, string otherTileName, TileFace otherTileFace, out TileFace matchingFace)
        {
            TileFace[] facesToCheck = new TileFace[] { TileFace.Right, TileFace.Bottom, TileFace.Left };
            matchingFace = TileFace.None;

            foreach (TileFace faceToCheck in facesToCheck)
            {
                if (this.GameManager.GameBoard.CanPlaceTileOnGameBoard(tileName, otherTileName, faceToCheck, otherTileFace, out TriominoTile placableTile))
                {
                    matchingFace = faceToCheck;
                    return true;
                }
            }

            return false;
        }

        private bool TryGetPlacableTileFromDrawBoard(out string placableTile, out string otherTile, out TileFace placableTileFace, out TileFace otherTileFace)
        {
            placableTile = null;
            otherTile = null;
            placableTileFace = TileFace.None;
            otherTileFace = TileFace.None;

            // if its the first turn return tile from drawboard with highest value.
            placableTile = null;
            if (this.GameManager.TurnCount == 0)
            {
                placableTile = this.GameManager.GetActivePlayersDrawBoard().GetTriominoWithHighestValue();
                return true;
            }

            string[] drawBoardTiles = this.GameManager.GetActivePlayersDrawBoard().Tiles.ToArray();
            foreach (string tile in drawBoardTiles)
            {
                if (this.CheckAllFreeFacesOnGameBoard(tile, out otherTile, out placableTileFace, out otherTileFace))
                {
                    placableTile = tile;
                    return true;
                }
            }

            return false;
        }

        #region PlayersTurn
        /// <summary>
        /// EventHandler for NextTurn-Event and GameStartEvent from GameManager 
        /// (checks if it is this  players turn)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PlayersTurn(object sender, EventArgs e)
        {
            if (this.GameManager.ActivePlayer.Equals(this.Player))
            {
                this.PlayTurn();
            }
        }
        #endregion
    }
}
