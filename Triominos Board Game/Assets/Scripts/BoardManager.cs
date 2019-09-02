using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;
using Assets.Scripts;
using System;
using Random = UnityEngine.Random;

public class BoardManager : MonoBehaviour
{
    public GameObject TilePool;
    public GameObject DrawBoardPlayer1;
    public GameObject DrawBoardPlayer2;
    public GameObject DrawBoardPlayer3;
    public GameObject DrawBoardPlayer4;
    public Button DrawButton;

    [HideInInspector]
    public Dictionary<PlayerCode, GameObject> DrawBoards;
    
    public bool IsDragging { get; private set; }
    private GameObject ActualTilePool;
    

    public void InitBoard()
    {
        this.InitTilePool();
    }

    public void InitTilePool()
    {
        // remove existing Tiles
        this.ResetPoolAndTiles();
        ActualTilePool = Instantiate(TilePool) as GameObject;

        DrawBoardPlayer1 = GameObject.Find("DrawBoardPlayer1");
        DrawBoardPlayer2 = GameObject.Find("DrawBoardPlayer2");
        DrawButton = GameObject.Find("DrawButton").GetComponent<Button>();

        DrawBoards = new Dictionary<PlayerCode, GameObject>()
        {
            { PlayerCode.Player1, DrawBoardPlayer1 },
            { PlayerCode.Player2, DrawBoardPlayer2 }
        };

        switch (GameManager.instance.GameMode)
        {
            case GameMode.PlayerVsAi:
            case GameMode.TwoPlayer:
                break;
            case GameMode.ThreePlayer:
                DrawBoards.Add(PlayerCode.Player3, DrawBoardPlayer3);
                break;
            case GameMode.FourPlayer:
                DrawBoards.Add(PlayerCode.Player4, DrawBoardPlayer4);
                break;
            default:
                throw new ArgumentException($"Unbekannter gameMode '{GameManager.instance.GameMode}'");
        }

        this.DrawStartTiles();
    }

    public void DrawStartTiles()
    {
        int numberStartTiles = 0;
        switch(GameManager.instance.GameMode)
        {
            case GameMode.PlayerVsAi:
            case GameMode.TwoPlayer:
                numberStartTiles = 9;
                break;
            case GameMode.ThreePlayer:
            case GameMode.FourPlayer:
                numberStartTiles = 7;
                break;
            default:
                throw new ArgumentException($"Unbekannter gameMode '{GameManager.instance.GameMode}'");
        }

        for (int i = 0; i < numberStartTiles; i++)
        {
            foreach (GameObject drawBoard in this.DrawBoards.Values)
            {
                this.DrawRandomTile(drawBoard);
            }
        }
    }

    public void ResetPoolAndTiles()
    {
        IEnumerable<GameObject> objects = GameObject.FindGameObjectsWithTag("TilePool");
        objects = objects.Concat(GameObject.FindGameObjectsWithTag("PlayerTile"));
        if (objects.Count() > 0)
        {
            foreach (GameObject tilePool in objects)
            {
                Destroy(tilePool);
            }
        }
    }

    public void DrawRandomTile(GameObject targetDrawBoard = null)
    {
        if (targetDrawBoard == null)
        {
            targetDrawBoard = DrawBoards[GameManager.instance.ActivePlayer];
        }

        int randomIndex = Random.Range(0, ActualTilePool.transform.childCount);
        GameObject randomTile = ActualTilePool.transform.GetChild(randomIndex).gameObject;
        randomTile.gameObject.SetActive(true);
        targetDrawBoard.GetComponent<DrawBoardManager>().AddTile(randomTile);
        if (TilePool.transform.childCount < 1)
        {
            DrawButton.gameObject.SetActive(false);
        }
    }

    public int GethHighestTriominoOfSameKindForPlayer(PlayerCode player)
    {
        string[] tileNames = this.GetAllTileNamesForPlayer(player);

        int highestSameKindTriomino = -1;
        foreach (string name in tileNames)
        {
            string[] parts = name.Split('-');
            if (parts.Length != 3)
            {
                throw new ArgumentException("Der Name eins Spielsteines muss die Form '1-2-3' haben.");
            }

            if (parts[0].Equals(parts[1]) && parts[1].Equals(parts[2]))
            {
                int tileNumber = int.Parse(parts[0]);
                if (tileNumber > highestSameKindTriomino)
                {
                    highestSameKindTriomino = tileNumber;
                }
            }
        }

        return highestSameKindTriomino;
    }

    public int GetHighestTileValueForPlayer(PlayerCode player)
    {
        string[] tileNames = this.GetAllTileNamesForPlayer(player);

        int highestTileValue = -1;
        foreach (string name in tileNames)
        {
            int tileValue = GameManager.instance.GetValueFromTileName(name);
            if (tileValue > highestTileValue)
            {
                highestTileValue = tileValue;
            }
        }

        return highestTileValue;
    }

    private string[] GetAllTileNamesForPlayer(PlayerCode player)
    {
        GameObject playerDrawBoard = this.DrawBoards[player];
        List<string> tileNames = new List<string>();
        foreach (Transform child in playerDrawBoard.transform)
        {
            if (child.gameObject.CompareTag("PlayerTile"))
            {
                tileNames.Add(child.gameObject.name);
            }
        }

        return tileNames.ToArray();
    }

    public void StartDragging()
    {
        this.IsDragging = true;
    }

    public void StopDragging()
    {
        this.IsDragging = false;
    }

    public HitDirection GetHitDirection(Vector2 direction)
    {
        if (direction.y < -0.5f && direction.x >= -0.9f && direction.x <= 0.9f)
        //if ((direction.x >= -0.1f && direction.x <= 0.1f) && (direction.y >= -1.1f && direction.y <= -0.9f))
        {
            return HitDirection.Bottom;
        }

        if (direction.x > 0.0f && direction.y >= -0.5f && direction.y <= 1.0f)
        //if ((direction.x <= 1.0f && direction.x >= 0.8f) && (direction.y <= 0.6f && direction.y >= 0.4f))
        {
            return HitDirection.Right;
        }

        if (direction.x <= 0.0f && direction.y >= -0.5f && direction.y <= 1.0f)
        //if ((direction.x >= -1.0f && direction.x <= -0.8f) && (direction.y >= 0.4f && direction.y <= 0.6f))
        {
            return HitDirection.Left;
        }

        return HitDirection.None;
    }
}
