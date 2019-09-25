using GraphKI.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphKI.GameManagement
{
    public class DrawBoard
    {
        #region fields
        /// <summary>
        /// The player which owns this DrawBoard.
        /// </summary>
        public PlayerCode Player { get; private set; }

        /// <summary>
        /// Event, which is thrown, when a Tile was added.
        /// </summary>
        public event EventHandler<TriominoTileEventArgs> TileAdded;

        /// <summary>
        /// Event, which is thrown, when a Tile was removed.
        /// </summary>
        public event EventHandler<TriominoTileEventArgs> TileRemoved;

        /// <summary>
        /// The tiles on this drawboard.
        /// </summary>
        internal HashSet<string> Tiles { get; set; }
        #endregion

        #region ctor
        /// <summary>
        /// Intantiates a new Object of this class.
        /// </summary>
        /// <param name="player"></param>
        public DrawBoard(PlayerCode player)
        {
            this.Player = player;
            this.Tiles = new HashSet<string>();
        }
        #endregion

        #region HasTile
        /// <summary>
        /// Checks if this DrawBoard contains a specific, Tile.
        /// </summary>
        /// <param name="tile">The name of the tile.</param>
        /// <returns>True if this Drawboard contains the tile, false if not.</returns>
        public bool HasTile(string tile)
        {
            return this.Tiles.Contains(tile);
        }
        #endregion

        #region TryRemoveTile
        /// <summary>
        /// Removes a specific Tile from the Drawboard.
        /// </summary>
        /// <param name="tile">The specific tile.</param>
        /// <returns>True, if Drawboard contains tile </returns>
        public bool TryRemoveTile(string tile)
        {
            if (!this.HasTile(tile))
            {
                return false;
            }

            if (!this.Tiles.Remove(tile))
            {
                return false;
            }

            this.OnTileRemoved(new TriominoTileEventArgs() { TileName = tile });
            return true;
        }
        #endregion

        #region TryAddTile
        /// <summary>
        /// Adds a Tile to the Drawboard, only if there is no tile with 
        /// the same name in the Drawboard already.
        /// </summary>
        /// <param name="tile">The tile to be added.</param>
        /// <returns>True, if it could be added, false if not.</returns>
        public bool TryAddTile(string tile)
        {
            if (!tile.IsTriominoTileName())
            {
                return false;
            }

            if (!this.Tiles.Add(tile))
            {
                return false;
            }

            this.OnTileAdded(new TriominoTileEventArgs() { TileName = tile });
            return true;
        }
        #endregion

        #region TryGetHighestTripleTriomino
        /// <summary>
        /// Determines the highest triple Triomino within this drawboard.
        /// </summary>
        /// <param name="highestTripleTriomino">the highest tripple triomino if this drawboard contains one, null otherwise.</param>
        /// <returns>True, if this drawBoard contains a triple triomino, false if not.</returns>
        public bool TryGetHighestTripleTriomino(out string highestTripleTriomino)
        {
            highestTripleTriomino = null;
            IEnumerable<string> allTripleTriominos = this.Tiles.Where(t => t.IsTripleTriomino());

            if (allTripleTriominos.Any())
            {
                highestTripleTriomino = allTripleTriominos.Aggregate((a, b) => (a.GetTriominoTileValue() > b.GetTriominoTileValue()) ? a : b);
            }

            if (highestTripleTriomino == string.Empty)
            {
                highestTripleTriomino = null;
            }

            return highestTripleTriomino != null;
        }
        #endregion

        #region GetHighestTriominoValue
        /// <summary>
        /// Determines the highest TriominoTile within this DrawBoard and returns its value.
        /// </summary>
        /// <returns>Value of the highest TriominoTile.</returns>
        public int GetHighestTriominoValue()
        {
            // Determine highest Triomino in all tiles within this drawboard.
            string highestTriomino = this.Tiles.Aggregate((a, b) => (a.GetTriominoTileValue() > b.GetTriominoTileValue()) ? a : b);

            return highestTriomino.GetTriominoTileValue();
        }
        #endregion

        #region GetTilesOnDrawBoard
        /// <summary>
        /// Returns a list as copy of all tiles on the drawboard.
        /// </summary>
        /// <returns>List with all tiles on drawboard.</returns>
        public List<string> GetTilesOnDrawBoard()
        {
            List<string> list = new List<string>(this.Tiles);
            return list;
        }
        #endregion

        #region OnTileAdded
        /// <summary>
        /// Throws Event 'TileAdded'
        /// </summary>
        /// <param name="e">Event Arguments</param>
        protected virtual void OnTileAdded(TriominoTileEventArgs e)
        {
            TileAdded?.Invoke(this, e);
        }
        #endregion

        #region OnTileRemoved
        /// <summary>
        /// Throws Event 'TileRemoved'
        /// </summary>
        /// <param name="e">Event Arguments</param>
        protected virtual void OnTileRemoved(TriominoTileEventArgs e)
        {
            TileRemoved?.Invoke(this, e);
        }
        #endregion
    }
}
