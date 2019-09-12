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
    public int? DefaultLayer;
    public int? PlayerHudLayer;
    public int? PlacedTileLayer;

    [HideInInspector]
    public Dictionary<PlayerCode, GameObject> DrawBoards;
    
    public bool IsDragging { get; private set; }
    private GameObject ActualTilePool;

    private void Update()
    {
        if (this.TilePoolIsEmpty())
        {
            this.DrawButton.GetComponent<DrawButtonManager>().Deactivate();
        }
    }


    public void InitBoard()
    {
        this.DefaultLayer = this.DefaultLayer ?? LayerMask.NameToLayer("Default");
        this.PlayerHudLayer = this.PlayerHudLayer ?? LayerMask.NameToLayer("Player HUD");
        this.PlacedTileLayer = this.PlacedTileLayer ?? LayerMask.NameToLayer("PlacedTile");

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
        if (this.TilePoolIsEmpty())
        {
            this.DrawButton.GetComponent<DrawButtonManager>().Deactivate();
        }
    }

    public int GethHighestTriominoOfSameKindForPlayer(PlayerCode player)
    {
        int highestSameKindTriomino = -1;

        foreach (Transform child in this.DrawBoards[player].transform)
        {
            if (child.gameObject.CompareTag("PlayerTile") && child.gameObject.GetComponent<TileManager>().IsSameKindTriomino())
            {
                int tileNumber = (int)(child.gameObject.GetComponent<TileManager>().GetTileValue() / 3);
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
        int highestTileValue = -1;

        foreach (Transform child in this.DrawBoards[player].transform)
        {
            if (child.gameObject.CompareTag("PlayerTile"))
            {
                int tileValue = child.gameObject.GetComponent<TileManager>().GetTileValue();
                if (tileValue > highestTileValue)
                {
                    highestTileValue = tileValue;
                }
            }
        }

        return highestTileValue;
    }

    public void StartDragging()
    {
        this.IsDragging = true;
    }

    public void StopDragging()
    {
        this.IsDragging = false;
    }
    public GameObject GetDrawBoardForActivePlayer()
    {
        return this.DrawBoards[GameManager.instance.ActivePlayer];
    }

    public void PlaceTile(GameObject tile)
    {
        tile.transform.position = new Vector3(tile.transform.position.x, tile.transform.position.y, tile.transform.position.z + 1);
        this.GetDrawBoardForActivePlayer().GetComponent<DrawBoardManager>().RemoveTile(this.gameObject);
    }

    public bool TilePoolIsEmpty()
    {
        return this.TilePool.transform.childCount < 1;
    }
}
