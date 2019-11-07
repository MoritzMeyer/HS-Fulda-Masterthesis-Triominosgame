using GraphKI.AIManagement;
using GraphKI.Extensions;
using GraphKI.GraphSuite;
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
        /// Dictionary with Drawboards for each player.
        /// </summary>
        public Dictionary<PlayerCode, DrawBoard> DrawBoards { get; set; }

        /// <summary>
        /// List with all Players played by ai.
        /// </summary>
        public List<IAIPlayer> AIPlayers { get; set; }

        /// <summary>
        /// GameMode for this game.
        /// </summary>
        public GameMode GameMode;

        /// <summary>
        /// Event, which indicates a new Turn.
        /// </summary>
        public event EventHandler NextTurnEvent;

        /// <summary>
        /// Event, which indicates the start of the game.
        /// </summary>
        public event EventHandler GameStartEvent;

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

        private GameMode TestScene;
        #endregion

        #region ctor
        /// <summary>
        /// Instantiates a new Object of this class.
        /// </summary>
        /// <param name="gameMode">gamemode</param>
        /// <param name="isPlayer2AI">Determines, if player2 is played by computer.</param>
        public GameManager(GameMode gameMode, bool isPlayer2AI = false)
        {
            int testScene = 0;
            this.TestScene = GameMode.None;

            if (gameMode == GameMode.TestScene1)
            {
                this.TestScene = gameMode;
                testScene = 1;
                gameMode = GameMode.TwoPlayer;
            }

            if (gameMode == GameMode.TestScene2)
            {
                this.TestScene = gameMode;
                testScene = 2;
                gameMode = GameMode.TwoPlayer;
            }

            this.TurnCount = 0;
            this.MaxTileDrawCount = 3;
            this.HasPlaced = false;
            this.GameBoard = new GameBoard();
            this.InitDrawBoards(gameMode);
            this.InitPlayerPoints(gameMode);
            this.TilePool = this.InitTilePool();

            if (testScene != 0)
            {
                this.DrawTilesForTestscene(testScene);
            }
            else
            {
                this.DrawStartTiles(gameMode);
            }

            this.ActivePlayer = this.GetStartingPlayer();
            this.AIPlayers = this.InitAIPlayers(gameMode, isPlayer2AI);
            this.GameMode = gameMode;
        }
        #endregion

        #region UnityIsInitialized
        /// <summary>
        /// When this game is used with unity and a ai player the last
        /// has to wait for its first turn until unity is initialized
        /// So this function throws the game start event, on which the ai player can listen
        /// </summary>
        public void UnityIsInitialized()
        {
            this.StartGame();
        }
        #endregion

        #region StartGame
        /// <summary>
        /// Starts the game and throws the gameisstartet event
        /// </summary>
        public void StartGame()
        {
            if (this.TestScene != GameMode.None)
            {
                this.InitTestScene();
            }
            this.OnGameStart(EventArgs.Empty);
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

        #region DrawSpecificTile
        /// <summary>
        /// Draws a specific Tile from DrawBoard (for testing purposes)
        /// </summary>
        /// <param name="demandedTile">The tile which should be drawn.</param>
        /// <param name="drawBoard">The drawBoard for which the tile should be drawn.</param>
        /// <returns>True, when the tile could be drawn, false if not.</returns>
        public bool DrawSpecificTile(string demandedTile, DrawBoard drawBoard)
        { 
            if (!this.TilePool.Contains(demandedTile))
            {
                return false;
            }

            List<string> tiles = new List<string>(this.TilePool);
            if (!tiles.Remove(demandedTile))
            {
                return false;
            }

            if (!drawBoard.TryAddTile(demandedTile))
            {
                tiles.Add(demandedTile);
                this.TilePool = new Stack<string>(tiles);
                return false;
            }

            this.TilePool = new Stack<string>(tiles);
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
                this.GameBoard.TryAddTile(this.ActivePlayer, tileName, out List<Hexagon> hexagons, otherName, tileFace, otherFace))
            {               
                this.PlayerPoints[this.ActivePlayer] += tileName.GetTriominoTileValue();

                if (hexagons.Count == 1)
                {
                    this.PlayerPoints[this.ActivePlayer] += 50;
                }
                else if (hexagons.Count > 1)
                {
                    this.PlayerPoints[this.ActivePlayer] += 70;
                }

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

        #region IsAiPlayer
        /// <summary>
        /// Determines if  a specific player is played by ai.
        /// </summary>
        /// <param name="player"></param>
        /// <returns></returns>
        public bool IsAiPlayer(PlayerCode player)
        {
            if (!this.AIPlayers.Any() || !this.AIPlayers.Where(ai => ai.Player.Equals(player)).Any())
            {
                return false;
            }

            return true;
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

        #region InitAIPlayers
        /// <summary>
        /// Initializes Dictionary which indicates if a player is ai-Player or not.
        /// </summary>
        /// <param name="gameMode">the gamemode.</param>
        /// <param name="isPlayer2AI">indicates if player2 is ai.</param>
        /// <returns>the Dictionary</returns>
        private List<IAIPlayer> InitAIPlayers(GameMode gameMode, bool isPlayer2AI)
        {
            List<IAIPlayer> aiPlayers = new List<IAIPlayer>();

            if (isPlayer2AI)
            {
                return new List<IAIPlayer>()
                {
                    new SimpleAIPlayer(this, PlayerCode.Player2)
                };
            }

            return new List<IAIPlayer>();
            //Dictionary<PlayerCode, bool> aiPlayers = new Dictionary<PlayerCode, bool>()
            //{
            //    { PlayerCode.Player1, false },
            //    { PlayerCode.Player2, true }
            //};

            //switch(gameMode)
            //{
            //    case GameMode.ThreePlayer:
            //        aiPlayers.Add(PlayerCode.Player3, false);
            //        break;
            //    case GameMode.FourPlayer:
            //        aiPlayers.Add(PlayerCode.Player3, false);
            //        aiPlayers.Add(PlayerCode.Player4, false);
            //        break;
            //}

            //return aiPlayers;
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
        /// <param name="e">Argument for this event.</param>
        protected virtual void OnNextTurn(EventArgs e)
        {
            this.NextTurnEvent?.Invoke(this, e);
        }
        #endregion

        #region OnGameStart
        /// <summary>
        /// Throws the GameStart-Event.
        /// </summary>
        /// <param name="e">Arguments for this event.</param>
        protected virtual void OnGameStart(EventArgs e)
        {
            this.GameStartEvent?.Invoke(this, e);
        }
        #endregion

        private void DrawTilesForTestscene(int sceneNumber)
        {
            switch(sceneNumber)
            {
                case 1:
                    this.DrawTilesForTestScene1();
                    break;
                case 2:
                    this.DrawTilesForTestScene2();
                    break;
                default:
                    return;
            }
        }

        private void DrawTilesForTestScene1()
        {
            string tile0 = "0-1-4"; //1
            string tile1 = "0-4-5"; //2
            string tile2 = "4-4-5"; //1
            string tile3 = "3-4-4"; //2
            string tile4 = "3-3-4"; //1
            string tile5 = "3-3-3"; //2
            string tile6 = "2-3-3"; //1
            string tile7 = "1-2-3"; //2
            string tile8 = "1-1-3"; //1
            string tile9 = "1-3-3"; //2

            this.DrawSpecificTile(tile0, this.DrawBoards[PlayerCode.Player1]);
            this.DrawSpecificTile(tile1, this.DrawBoards[PlayerCode.Player2]);
            this.DrawSpecificTile(tile2, this.DrawBoards[PlayerCode.Player1]);
            this.DrawSpecificTile(tile3, this.DrawBoards[PlayerCode.Player2]);
            this.DrawSpecificTile(tile4, this.DrawBoards[PlayerCode.Player1]);
            this.DrawSpecificTile(tile5, this.DrawBoards[PlayerCode.Player2]);
            this.DrawSpecificTile(tile6, this.DrawBoards[PlayerCode.Player1]);
            this.DrawSpecificTile(tile7, this.DrawBoards[PlayerCode.Player2]);
            this.DrawSpecificTile(tile8, this.DrawBoards[PlayerCode.Player1]);
            this.DrawSpecificTile(tile9, this.DrawBoards[PlayerCode.Player2]);
        }

        private void DrawTilesForTestScene2()
        {
            string tile0 = "0-0-0"; //1
            string tile1 = "0-0-2"; //2
            string tile2 = "0-2-2"; //1
            string tile3 = "0-1-2"; //2
            string tile4 = "0-1-1"; //1
            string tile5 = "0-0-1"; //2
            string tile6 = "0-2-4"; //1
            string tile7 = "0-4-5"; //2
            string tile8 = "0-5-5"; //1
            string tile9 = "0-0-5"; //2

            this.DrawSpecificTile(tile0, this.DrawBoards[PlayerCode.Player1]);
            this.DrawSpecificTile(tile1, this.DrawBoards[PlayerCode.Player2]);
            this.DrawSpecificTile(tile2, this.DrawBoards[PlayerCode.Player1]);
            this.DrawSpecificTile(tile3, this.DrawBoards[PlayerCode.Player2]);
            this.DrawSpecificTile(tile4, this.DrawBoards[PlayerCode.Player1]);
            this.DrawSpecificTile(tile5, this.DrawBoards[PlayerCode.Player2]);
            this.DrawSpecificTile(tile6, this.DrawBoards[PlayerCode.Player1]);
            this.DrawSpecificTile(tile7, this.DrawBoards[PlayerCode.Player2]);
            this.DrawSpecificTile(tile8, this.DrawBoards[PlayerCode.Player1]);
            this.DrawSpecificTile(tile9, this.DrawBoards[PlayerCode.Player2]);
        }

        private void InitTestScene()
        {
            switch(this.TestScene)
            {
                case GameMode.TestScene1:
                    this.InitTestScene1();
                    break;
                case GameMode.TestScene2:
                    this.InitTestScene2();
                    break;
                default:
                    return;
            }
        }

        private void InitTestScene1()
        {
            this.ActivePlayer = PlayerCode.Player1;
            this.TryPlaceOnGameBoard("0-1-4");
            this.TryPlaceOnGameBoard("0-4-5", "0-1-4", TileFace.Right, TileFace.Left);
            this.TryPlaceOnGameBoard("4-4-5", "0-4-5", TileFace.Left, TileFace.Bottom);
            this.TryPlaceOnGameBoard("3-4-4", "4-4-5", TileFace.Bottom, TileFace.Right);
            this.TryPlaceOnGameBoard("3-3-4", "3-4-4", TileFace.Bottom, TileFace.Left);
            this.TryPlaceOnGameBoard("3-3-3", "3-3-4", TileFace.Bottom, TileFace.Right);
            this.TryPlaceOnGameBoard("2-3-3", "3-3-3", TileFace.Bottom, TileFace.Left);
            this.TryPlaceOnGameBoard("1-2-3", "2-3-3", TileFace.Bottom, TileFace.Left);
        }

        private void InitTestScene2()
        {
            this.ActivePlayer = PlayerCode.Player2;
            this.TryPlaceOnGameBoard("0-0-2"); //2
            this.TryPlaceOnGameBoard("0-2-2", "0-0-2", TileFace.Left, TileFace.Bottom); //1
            this.TryPlaceOnGameBoard("0-1-2", "0-2-2", TileFace.Left, TileFace.Right); //2
            this.TryPlaceOnGameBoard("0-1-1", "0-1-2", TileFace.Left, TileFace.Right); //1
            this.TryPlaceOnGameBoard("0-0-1", "0-1-1", TileFace.Left, TileFace.Right); //2
            this.TryPlaceOnGameBoard("0-2-4", "0-0-2", TileFace.Right, TileFace.Left); //1
            this.TryPlaceOnGameBoard("0-4-5", "0-2-4", TileFace.Right, TileFace.Left); //2
            this.TryPlaceOnGameBoard("0-5-5", "0-4-5", TileFace.Right, TileFace.Left); //1
            this.TryPlaceOnGameBoard("0-0-5", "0-5-5", TileFace.Bottom, TileFace.Left); //2

            //this.TryPlaceOnGameBoard("0-0-2", "0-0-0", TileFace.Right, TileFace.Right); //1
        }
    }
}
