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

    public string GetValueFromTileFace(TileFace face, string name)
    {
        string[] parts = name.Split('-');
        if (parts.Count() != 3)
        {
            throw new ArgumentException($"Tile name ('{name}') does not contain three numbers.");
        }

        string faceValue = string.Empty;
        switch(face)
        {
            case TileFace.Right:
                faceValue = parts[0] + "-" + parts[1];
                break;
            case TileFace.Bottom:
                faceValue = parts[1] + "-" + parts[2];
                break;
            case TileFace.Left:
                faceValue = parts[2] + "-" + parts[0];
                break;
            default:
                break;
        }

        return faceValue;
    }

    public bool CheckIfTileOrientationMatches(GameObject tile1, GameObject tile2)
    {
        float rotation1 = tile1.transform.rotation.eulerAngles.z;
        float rotation2 = tile2.transform.rotation.eulerAngles.z;

        int orientation1 = Convert.ToInt32((Math.Abs(rotation1) / 60.0f) % 2);
        int orientation2 = Convert.ToInt32((Math.Abs(rotation2) / 60.0f) % 2);

        if (orientation1 < 0 || orientation1 > 1 || orientation2 < 0 || orientation2 > 1)
        {
            throw new ArgumentException("Orientation value has to be 0 or 1");
        }

        return orientation1 != orientation2;
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
}
