using Assets.Scripts;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class GameManager : MonoBehaviour
{
    public static GameManager instance = null;
    public BoardManager boardManager;
    public GameMode GameMode;

    [HideInInspector]
    public PlayerCode ActivePlayer = PlayerCode.None;
    [HideInInspector]
    public List<PlayerCode> ParticipatingPlayers;

    private int TurnCount = 0;
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
        boardManager = GetComponent<BoardManager>();
        GameMode = GameMode.TwoPlayer;
        this.InitGame();
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
            playerPoints.Add(player, 0);
        }

        boardManager.InitBoard();
        this.ActivePlayer = this.GetStartingPlayer();
        Debug.Log("Aktiver Spieler: " + this.ActivePlayer);
    }

    public void DrawTile()
    {
        if (this.NumbTileDrawsInTurn >= 3)
        {
            this.boardManager.DrawButton.GetComponent<Renderer>().material.color = Color.red;
        }

        this.NumbTileDrawsInTurn++;
        playerPoints[ActivePlayer] -= 5;
        this.boardManager.DrawRandomTile();
    }

    public void PlaceTile(GameObject tile)
    {
        int tilePoints = this.GetValueFromTileName(tile.name);
        playerPoints[ActivePlayer] += tilePoints;
    }

    public void NextPlayersTurn()
    {
        this.TurnCount++;
        this.NumbTileDrawsInTurn = 0;
    }

    public PlayerCode GetStartingPlayer()
    {
        // Zunächst für jeden Spieler den TrippleTriomino mit dem höchsten Wert holen
        Dictionary<PlayerCode, int> highestTriominoOfSameKind = new Dictionary<PlayerCode, int>();
        foreach(PlayerCode player in this.ParticipatingPlayers)
        {
            int highestTriomino = boardManager.GethHighestTriominoOfSameKindForPlayer(player);
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
            int highestTriominio = boardManager.GetHighestTileValueForPlayer(player);
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

    public int GetValueFromTileName(string name)
    {
        string[] parts = name.Split('-');
        if (parts.Length != 3)
        {
            throw new ArgumentException("Der Name eins Spielsteines muss die Form '1-2-3' haben.");
        }

        int points = parts.Select(n => int.Parse(n)).Aggregate((a, b) => a + b);
        Debug.Log("TilePoints: " + points);
        return points;
    }
}
