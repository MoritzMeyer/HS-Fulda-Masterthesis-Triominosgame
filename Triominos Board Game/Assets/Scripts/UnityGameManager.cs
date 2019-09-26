using Assets.Scripts;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;
using GraphKI.GameManagement;

public class UnityGameManager : MonoBehaviour
{
    #region fields
    public static UnityGameManager instance = null;
    public BoardManager boardManager;
    public GameMode GameMode;
    public Text player1Score;
    public Text player2Score;
    public GameManager GameManager;
    #endregion

    #region Awake
    public void Start()
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

        this.InitGame();
    }
    #endregion

    #region Update
    private void Update()
    {
        this.player1Score.text = PlayerCode.Player1 + ": " + this.GameManager.PlayerPoints[PlayerCode.Player1];
        this.player2Score.text = PlayerCode.Player2 + ": " + this.GameManager.PlayerPoints[PlayerCode.Player2];
    }
    #endregion

    #region InitGame
    public void InitGame()
    {
        //this.GameManager = new GameManager(GameMode, true);
        this.GameManager = new GameManager(GameMode, false);
        this.GameManager.NextTurnEvent += (sender, e) => { this.NextTurn(); };
        this.boardManager.InitBoard();
        Debug.Log("Aktiver Spieler: " + this.GameManager.ActivePlayer);
        this.GameManager.UnityIsInitialized();
    }
    #endregion

    #region DrawTile
    /// <summary>
    /// Draws a new Tile from the remaining TilePool and adds it to the actives player DrawBoard.
    /// </summary>
    public void DrawTile()
    {
        if (!this.GameManager.TryDrawTile(out string randomTile))
        {
            this.boardManager.DrawButton.GetComponent<DrawButtonManager>().Deactivate();
        }
    }
    #endregion

    #region TryPlaceTile
    public bool TryPlaceTile(GameObject tile)
    {
        return this.boardManager.TryPlaceTile(tile);
    }
    #endregion

    #region NextTurn
    /// <summary>
    /// Resets and initializes all Variables needed for next turn.
    /// </summary>
    public void NextTurn()
    {
        Debug.Log("Next turn invoked");
        if (this.GameManager.CanDrawTile())
        {
            this.boardManager.DrawButton.GetComponent<DrawButtonManager>().Activate();
        }
    }
    #endregion

    #region CheckFaceValues
    public bool CheckFaceValues(string value1, string value2)
    {
        string[] parts1 = value1.Split('-');
        string[] parts2 = value2.Split('-');

        return parts1[0] == parts2[1] && parts1[1] == parts2[0];
    }
    #endregion
}
