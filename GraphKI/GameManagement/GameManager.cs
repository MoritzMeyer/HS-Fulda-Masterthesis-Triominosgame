using GraphKI.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphKI.GameManagement
{
    public class GameManager
    {
        #region fields
        /// <summary>
        /// Number of turns during the actual game.
        /// </summary>
        public int TurnCount { get; private set; }

        /// <summary>
        /// The gameboard on which all the tiles are placed.
        /// </summary>
        public GameBoard GameBoard { get; private set; }

        /// <summary>
        /// The code of the actual player.
        /// </summary>
        public PlayerCode ActivePlayer { get; private set; }

        /// <summary>
        /// Maximal number of tiles a player can draw during its turn.
        /// </summary>
        public int MaxTileDrawCount { get; private set; }

        /// <summary>
        /// Dictionary with points for each player.
        /// </summary>
        public Dictionary<PlayerCode, int> PlayerPoints { get; set; }

        /// <summary>
        /// Event, which indicates a new Turn.
        /// </summary>
        public event EventHandler NextTurnEvent;

        /// <summary>
        /// Dictionary with Drawboards for each player.
        /// </summary>
        internal Dictionary<PlayerCode, DrawBoard> DrawBoards { get; set; }

        /// <summary>
        /// All remaining (drawable) tiles, which are not distributed to a player yet.
        /// This Stack is initialized with all possible tiles in random order, so stack.pop
        /// draws a random tile.
        /// </summary>
        internal Stack<string> TilePool;

        /// <summary>
        ///  Counts numbers of tile draws during one turn.
        /// </summary>
        internal int DrawCount;

        /// <summary>
        /// Determines if actual player has placed a tile.
        /// </summary>
        internal bool HasPlaced;
        #endregion

        #region ctor
        /// <summary>
        /// Intantiates a new Object of this class.
        /// </summary>
        /// <param name="gameMode"></param>
        public GameManager(GameMode gameMode)
        {
            this.TurnCount = 0;
            this.MaxTileDrawCount = 3;
            this.HasPlaced = false;
            this.GameBoard = new GameBoard();
            this.InitDrawBoards(gameMode);
            this.InitPlayerPoints(gameMode);
            this.TilePool = this.InitTilePool();
            this.DrawStartTiles(gameMode);
            this.ActivePlayer = this.GetStartingPlayer();
        }
        #endregion

        #region TryDrawTile
        /// <summary>
        /// Pops a tile from the 'availableTile' Stack, only if the stack is not empty.
        /// As the stack is sorted randomly, the poped tile is drawn random.
        /// </summary>
        /// <param name="randomTile">The Tile to be drawn.</param>
        /// <param name="isInitializing">Determines if tile is drawn during initialization of the game (drawcount will not be incrementet)</param>
        /// <returns>True, if a tile could be drawn, false if not.</returns>
        public bool TryDrawTile(out string randomTile, bool isInitializing = false)
        {
            randomTile = null;
            if (this.TilePool.Count < 1 || !this.CanDrawTile() || this.HasPlaced)
            {
                return false;
            }

            randomTile = this.TilePool.Pop();

            if (!isInitializing)
            {
                if (!this.GetActivePlayersDrawBoard().TryAddTile(randomTile))
                {
                    // Re-add drawn tile to pool.
                    List<string> newTilePool = new List<string>(this.TilePool)
                    {
                        randomTile
                    };
                    this.TilePool = new Stack<string>(newTilePool);

                    return false;
                }

                this.PlayerPoints[this.ActivePlayer] -= 5;
                this.DrawCount++;
            }

            
            return true;
        }
        #endregion

        #region TryPlaceOnGameBoard
        /// <summary>
        /// Adds a tile to the gameboard, if possible.
        /// </summary>
        /// <param name="tile">The tile.</param>
        /// <returns>True if it could be added, false if not.</returns>
        public bool TryPlaceOnGameBoard(string tileName, string otherName = null, TileFace? tileFace = null, TileFace? otherFace = null)
        {
            if (!this.GetActivePlayersDrawBoard().HasTile(tileName))
            {
                throw new InvalidOperationException("Only the active player can place tiles on the gameboard.");
            }

            if (!this.HasPlaced && 
                this.GetActivePlayersDrawBoard().TryRemoveTile(tileName) && 
                this.GameBoard.TryAddTile(tileName, otherName, tileFace, otherFace))
            {               
                this.PlayerPoints[this.ActivePlayer] += tileName.GetTriominoTileValue();
                this.HasPlaced = true;
                this.NextTurn();
                return true;
            }

            // if tile was removed in step before, add it again.
            if (!this.GetActivePlayersDrawBoard().HasTile(tileName))
            {
                this.GetActivePlayersDrawBoard().TryAddTile(tileName);
            }

            return false;
        }
        #endregion

        #region GetActivePlayersDrawBoard
        /// <summary>
        /// Gets the DrawBoard for active player.
        /// </summary>
        /// <returns>Active players drawboard.</returns>
        public DrawBoard GetActivePlayersDrawBoard()
        {
            return this.GetPlayersDrawBoard(this.ActivePlayer);
        }
        #endregion

        #region GetPlayersDrawBoard
        /// <summary>
        /// Gets the Drawboard for a specific player.
        /// </summary>
        /// <param name="player">The Player who owns the drawboard.</param>
        /// <returns>The players Drawboard.</returns>
        public DrawBoard GetPlayersDrawBoard(PlayerCode player)
        {
            if (!this.DrawBoards.ContainsKey(player))
            {
                throw new KeyNotFoundException($"Could not found any Drawboard for Player: '{player}'");
            }

            return this.DrawBoards[player];
        }
        #endregion

        #region CanDrawTile
        /// <summary>
        /// Ascertains if the actual player can draw a tile.
        /// </summary>
        /// <returns></returns>
        public bool CanDrawTile()
        {
            return this.DrawCount < this.MaxTileDrawCount && this.TilePool.Count > 0;
        }
        #endregion

        #region TryNextTurn
        /// <summary>
        /// Tries to initialize the next turn.
        /// </summary>
        /// <returns>True if next turn could be initialized, false if not.</returns>
        public bool TryNextTurn()
        {
            if (this.HasPlaced)
            {
                this.NextTurn();
                return true;
            }

            if (!this.CanDrawTile() && !this.HasPlaced)
            {
                this.PlayerPoints[this.ActivePlayer] -= 10;
                this.NextTurn();
                return true;
            }

            return false;            
        }
        #endregion

        #region NextTurnForDebug
        /// <summary>
        /// Invokes NextTurn without verifications.
        /// </summary>
        public void NextTurnForDebug()
        {
            this.NextTurn();
        }
        #endregion

        #region NextTurn
        /// <summary>
        /// Initializes the next turn.
        /// </summary>
        private void NextTurn()
        {
            this.ActivePlayer = this.GetNextPlayer();
            this.TurnCount++;
            this.DrawCount = 0;
            this.HasPlaced = false;
            this.OnNextTurn(EventArgs.Empty);
        }
        #endregion

        #region GetNextPlayer
        /// <summary>
        /// Determines the active player for next turn.
        /// </summary>
        /// <returns>the next active players PlayerCode</returns>
        private PlayerCode GetNextPlayer()
        {
            List<PlayerCode> players = this.DrawBoards.Keys.ToList();
            int indexActual = players.IndexOf(this.ActivePlayer);
            indexActual++;

            if (indexActual >= players.Count)
            {
                indexActual = 0;
            }

            return players[indexActual];
        }
        #endregion

        #region InitDrawBoards
        /// <summary>
        /// Initializes Dictionary with DrawBoards and assigns Players to the DrawBoards.
        /// </summary>
        /// <param name="gameMode">The GameMode determines how much players participate.</param>
        private void InitDrawBoards(GameMode gameMode)
        {
            this.DrawBoards = new Dictionary<PlayerCode, DrawBoard>
            {
                // Every Game has at least two players.
                { PlayerCode.Player1, new DrawBoard(PlayerCode.Player1) },
                { PlayerCode.Player2, new DrawBoard(PlayerCode.Player2) }
            };

            switch (gameMode)
            {
                case GameMode.TwoPlayer:
                    break;
                case GameMode.ThreePlayer:
                    this.DrawBoards.Add(PlayerCode.Player3, new DrawBoard(PlayerCode.Player3));
                    break;
                case GameMode.FourPlayer:
                    this.DrawBoards.Add(PlayerCode.Player3, new DrawBoard(PlayerCode.Player3));
                    this.DrawBoards.Add(PlayerCode.Player4, new DrawBoard(PlayerCode.Player4));
                    break;
                default:
                    throw new ArgumentException($"No matching GameMode found (given: '{gameMode}')");
            }
        }
        #endregion

        #region InitPlayerPoints
        /// <summary>
        /// Initializes Dictionary with player points based on the gameMode.
        /// </summary>
        /// <param name="gameMode">The gameMode.</param>
        private void InitPlayerPoints(GameMode gameMode)
        {
            this.PlayerPoints = new Dictionary<PlayerCode, int>()
            {
                // Every Game has at least two players.
                { PlayerCode.Player1, 0 },
                { PlayerCode.Player2, 0 }
            };

            switch(gameMode)
            {
                case GameMode.TwoPlayer:
                    break;
                case GameMode.ThreePlayer:
                    this.PlayerPoints.Add(PlayerCode.Player3, 0);
                    break;
                case GameMode.FourPlayer:
                    this.PlayerPoints.Add(PlayerCode.Player3, 0);
                    this.PlayerPoints.Add(PlayerCode.Player4, 0);
                    break;
                default:
                    throw new ArgumentException($"No matching GameMode found (given: '{gameMode}')");
            }
        }
        #endregion

        #region InitTilePool
        /// <summary>
        /// Initializes the TilePool from which new tiles will be drawn.
        /// </summary>
        /// <returns>The TilePool as Stack.</returns>
        internal Stack<string> InitTilePool()
        {
            // use a HashSet for Initilization, which esures no duplicates within the tiles.
            HashSet<string> tilePool = new HashSet<string>();

            for (int i = 0; i < 6; i++)
            {
                for (int j = i; j < 6; j++)
                {
                    for (int k = j; k < 6; k++)
                    {
                        tilePool.Add(i + "-" + j + "-" + k);
                    }
                }
            }

            List<string> unorderedTilePool = new List<string>(tilePool);
            unorderedTilePool.Shuffle();
            return new Stack<string>(unorderedTilePool);
        }
        #endregion

        #region DrawStartTiles
        /// <summary>
        /// Draws all Starttiles for each Player according to the NumberOfPlayers (GameMode).
        /// </summary>
        /// <param name="gameMode">The GameMode, which defines the NumberOfPlayers.</param>
        private void DrawStartTiles(GameMode gameMode)
        {
            int numberOfSTartTiles = 0;
            switch (gameMode)
            {
                case GameMode.TwoPlayer:
                    numberOfSTartTiles = 9;
                    break;
                case GameMode.ThreePlayer:
                case GameMode.FourPlayer:
                    numberOfSTartTiles = 7;
                    break;
            }

            for (int i = 0; i < numberOfSTartTiles; i++)
            {
                foreach (DrawBoard drawBoard in this.DrawBoards.Values)
                {
                    if (!this.TryDrawTile(out string randomTile, true) || !drawBoard.TryAddTile(randomTile))
                    {
                        throw new ArgumentException("Couldn't draw StartTiles.");
                    }
                }
            }
        }
        #endregion

        #region GetStartingPlayer
        /// <summary>
        /// Determines the Player who is allowed to begin the game, based on the tiles within the players drawboards.
        /// </summary>
        /// <returns>The player who may start the game.</returns>
        private PlayerCode GetStartingPlayer()
        {
            // At First get the highest triple TriominoTile for each player
            Dictionary<PlayerCode, string> highestTripleTriominosPerPlayer = new Dictionary<PlayerCode, string>();
            foreach(KeyValuePair<PlayerCode, DrawBoard> kv in this.DrawBoards)
            {
                kv.Value.TryGetHighestTripleTriomino(out string highestTripleTriomino);
                highestTripleTriominosPerPlayer.Add(kv.Key, highestTripleTriomino);
            }

            // return player with highest triomino tile
            Dictionary<PlayerCode, string> playersWithValue = highestTripleTriominosPerPlayer
                .Where(kv => kv.Value != null && kv.Value != string.Empty)
                .ToDictionary(kv => kv.Key, kv => kv.Value);
            if (playersWithValue.Any())
            {
                // Determine keyValuePair in 'highestTripleTriominosPerPlayer'-Dictionry with highest triple Triomino value and return its key (player code).
                return playersWithValue
                    .Aggregate((a, b) => a.Value.GetTriominoTileValue() > b.Value.GetTriominoTileValue() ? a : b)
                    .Key;
            }

            // if none of the players has a triple triomino, return player with highest triomino value.
            Dictionary<PlayerCode, int> highestTriominoValuePerPlayer = new Dictionary<PlayerCode, int>();
            foreach (KeyValuePair<PlayerCode,DrawBoard> kv in this.DrawBoards)
            {
                highestTriominoValuePerPlayer.Add(kv.Key, kv.Value.GetHighestTriominoValue());
            }

            return highestTriominoValuePerPlayer
                .Aggregate((a, b) => (a.Value > b.Value) ? a : b)
                .Key;
        }
        #endregion

        #region OnNextTurn
        /// <summary>
        /// Throws the NextTurn-Event.
        /// </summary>
        /// <param name="e">Argument for the event.</param>
        protected virtual void OnNextTurn(EventArgs e)
        {
            this.NextTurnEvent?.Invoke(this, e);
        }
        #endregion
    }
}
