using Assets.Scripts;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance = null;
    public BoardManager boardManager;
    public GameMode GameMode;
    public Text player1Score;
    public Text player2Score;

    //[HideInInspector]
    public PlayerCode ActivePlayer = PlayerCode.None;
    [HideInInspector]
    public List<PlayerCode> ParticipatingPlayers;

    [HideInInspector]
    public int TurnCount { get; private set; }

    //private int TurnCount = 0;
    private Dictionary<PlayerCode, int> playerPoints;
    private int NumbTileDrawsInTurn = 0;

    public void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);
        this.boardManager = GetComponent<BoardManager>();

        Text[] texts = FindObjectsOfType<Text>();
        player1Score = texts.Where(o => o.name == "ScorePlayer1").First();
        player2Score = texts.Where(o => o.name == "ScorePlayer2").First();

        this.TurnCount = 0;

        this.InitGame();
    }

    private void Update()
    {
        this.player1Score.text = PlayerCode.Player1 + ": " + this.playerPoints[PlayerCode.Player1];
        this.player2Score.text = PlayerCode.Player2 + ": " + this.playerPoints[PlayerCode.Player2];
    }

    public void InitGame()
    {
        ParticipatingPlayers = new List<PlayerCode>()
        {
            PlayerCode.Player1,
            PlayerCode.Player2
        };

        switch (GameMode)
        {
            case GameMode.PlayerVsAi:
            case GameMode.TwoPlayer:
                break;
            case GameMode.ThreePlayer:
                playerPoints.Add(PlayerCode.Player3, 0);
                break;
            case GameMode.FourPlayer:
                playerPoints.Add(PlayerCode.Player3, 0);
                playerPoints.Add(PlayerCode.Player4, 0);
                break;
            default:
                throw new ArgumentException($"Unbekannter gameMode '{GameMode}'");
        }

        playerPoints = new Dictionary<PlayerCode, int>();
        foreach (PlayerCode player in this.ParticipatingPlayers)
        {
            this.playerPoints.Add(player, 0);
        }

        this.boardManager.InitBoard();
        this.ActivePlayer = this.GetStartingPlayer();
        Debug.Log("Aktiver Spieler: " + this.ActivePlayer);
    }

    public void DrawTile()
    {
        if (this.NumbTileDrawsInTurn >= 2)
        {
            this.boardManager.DrawButton.GetComponent<DrawButtonManager>().Deactivate();
        }

        this.NumbTileDrawsInTurn++;
        this.playerPoints[ActivePlayer] -= 5;
        GameObject tile = this.boardManager.DrawRandomTile();

        if (tile != null)
        {
            tile.GetComponent<FadeToColor>().StartFadeToOrigin();
        }
    }

    public bool TryPlaceTile(GameObject tile)
    {
        if (this.boardManager.TryPlaceTile(tile))
        {
            int tilePoints = tile.GetComponent<TileManager>().GetTileValue();
            playerPoints[ActivePlayer] += tilePoints;

            this.NextTurn();
            return true;
        }

        return false;
    }

    public void NextTurn()
    {
        this.TurnCount++;
        this.NumbTileDrawsInTurn = 0;
        this.ActivePlayer = this.GetNextPlayer();

        if (!this.boardManager.TilePoolIsEmpty())
        {
            this.boardManager.DrawButton.GetComponent<DrawButtonManager>().Activate();
        }
    }

    public PlayerCode GetStartingPlayer()
    {
        // Zunächst für jeden Spieler den TrippleTriomino mit dem höchsten Wert holen
        Dictionary<PlayerCode, int> highestTriominoOfSameKind = new Dictionary<PlayerCode, int>();
        foreach(PlayerCode player in this.ParticipatingPlayers)
        {
            int highestTriomino = this.boardManager.GethHighestTriominoOfSameKindForPlayer(player);
            highestTriominoOfSameKind.Add(player, highestTriomino);
        }

        // Wenn vorhanden den Spieler mit dem höchsten Tripple-Triomino zurückgeben.
        int firstValue = highestTriominoOfSameKind.Values.First();
        if (!highestTriominoOfSameKind.Values.All(x => x.Equals(firstValue)))
        {
            return highestTriominoOfSameKind.Aggregate((a, b) => a.Value > b.Value ? a : b).Key;
        }

        // Ansonsten den Spieler mit dem höchsten Triomino-Wert ermitteln.
        Dictionary<PlayerCode, int> highestTriominoValue = new Dictionary<PlayerCode, int>();
        foreach(PlayerCode player in this.ParticipatingPlayers)
        {
            int highestTriominio = this.boardManager.GetHighestTileValueForPlayer(player);
            highestTriominoValue.Add(player, highestTriominio);
        }

        PlayerCode playerWithHighestTriominoValue = PlayerCode.Player1;
        foreach (PlayerCode player in highestTriominoValue.Keys)
        {
            if (highestTriominoValue[playerWithHighestTriominoValue] < highestTriominoValue[player])
            {
                playerWithHighestTriominoValue = player;
            }
        }

        return playerWithHighestTriominoValue;
    }

    private PlayerCode GetNextPlayer()
    {
        int actualIndex = this.ParticipatingPlayers.IndexOf(this.ActivePlayer);
        actualIndex++;
        if (actualIndex >= this.ParticipatingPlayers.Count())
        {
            actualIndex = 0;
        }

        return ParticipatingPlayers[actualIndex];
    }

    public bool CheckFaceValues(string value1, string value2)
    {
        string[] parts1 = value1.Split('-');
        string[] parts2 = value2.Split('-');

        return parts1[0] == parts2[1] && parts1[1] == parts2[0];
    }
}
