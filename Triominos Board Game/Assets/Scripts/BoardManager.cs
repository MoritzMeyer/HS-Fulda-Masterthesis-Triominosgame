using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;
using Assets.Scripts;
using System;
using Random = UnityEngine.Random;
using GraphKI.Extensions;
using GraphKI.GameManagement;

public class BoardManager : MonoBehaviour
{
    public float BoardPositionZ = 1.0f;
    public GameObject TilePool;
    public GameObject DrawBoardPlayer1;
    public GameObject DrawBoardPlayer2;
    public GameObject DrawBoardPlayer3;
    public GameObject DrawBoardPlayer4;
    public Button DrawButton;

    [HideInInspector]
    public Dictionary<PlayerCode, GameObject> DrawBoards;

    [HideInInspector]
    public GameBoard gameBoard;


    public bool IsDragging { get; private set; }
    private GameObject ActualTilePool;
    private Dictionary<int, Dictionary<TileFace, Vector2>> NewTilePositionsByOtherOrientationAndTileFace;
    private Dictionary<TileFace, Dictionary<TileFace, float>> NewTileOrientationByOtherFaceAndThisFace;

    private void Update()
    {
        if (this.TilePoolIsEmpty())
        {
            this.DrawButton.GetComponent<DrawButtonManager>().Deactivate();
        }
    }


    public void InitBoard()
    {
        this.gameBoard = new GameBoard();
        //this.InitGameBoard();
        this.InitTestScene();
    }

    private void InitTestScene()
    {
        this.ResetPoolAndTiles();
        ActualTilePool = Instantiate(TilePool) as GameObject;

        this.NewTilePositionsByOtherOrientationAndTileFace = this.InitNewTilePositionsByOtherOrientationAndTileFace();
        this.NewTileOrientationByOtherFaceAndThisFace = this.InitNewTileOrientationByOtherFaceAndThisFace();

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

        this.InitBridgeScene();
    }

    private void InitBridgeScene()
    {
        GameObject tile014 = this.DrawSpecificTile("0-1-4", DrawBoards[PlayerCode.Player1]);
        GameObject tile045 = this.DrawSpecificTile("0-4-5", DrawBoards[PlayerCode.Player1]);
        GameObject tile445 = this.DrawSpecificTile("4-4-5", DrawBoards[PlayerCode.Player1]);
        GameObject tile344 = this.DrawSpecificTile("3-4-4", DrawBoards[PlayerCode.Player1]);
        GameObject tile334 = this.DrawSpecificTile("3-3-4", DrawBoards[PlayerCode.Player1]);
        GameObject tile333 = this.DrawSpecificTile("3-3-3", DrawBoards[PlayerCode.Player1]);
        GameObject tile233 = this.DrawSpecificTile("2-3-3", DrawBoards[PlayerCode.Player1]);
        GameObject tile123 = this.DrawSpecificTile("1-2-3", DrawBoards[PlayerCode.Player1]);
        GameObject tile113 = this.DrawSpecificTile("1-1-3", DrawBoards[PlayerCode.Player1]);
        GameObject tile133 = this.DrawSpecificTile("1-3-3", DrawBoards[PlayerCode.Player1]);

        tile014.transform.SetParent(null);
        tile014.transform.position = new Vector3(0, 0, this.BoardPositionZ);
        this.PlaceTileOnActualPosition(tile014, DrawBoards[PlayerCode.Player1]);

        DrawBoardManager drawBoardManager = DrawBoards[PlayerCode.Player1].GetComponent<DrawBoardManager>();

        drawBoardManager.RemoveTile(tile045);
        drawBoardManager.RemoveTile(tile445);
        drawBoardManager.RemoveTile(tile344);
        drawBoardManager.RemoveTile(tile334);
        drawBoardManager.RemoveTile(tile333);
        drawBoardManager.RemoveTile(tile233);
        drawBoardManager.RemoveTile(tile123);

        this.PlaceTileNextToOther(tile045, TileFace.Right, tile014, TileFace.Left);
        this.PlaceTileNextToOther(tile445, TileFace.Left, tile045, TileFace.Bottom);
        this.PlaceTileNextToOther(tile344, TileFace.Bottom, tile445, TileFace.Right);
        this.PlaceTileNextToOther(tile334, TileFace.Bottom, tile344, TileFace.Left);
        this.PlaceTileNextToOther(tile333, TileFace.Bottom, tile334, TileFace.Right);
        this.PlaceTileNextToOther(tile233, TileFace.Bottom, tile333, TileFace.Left);
        this.PlaceTileNextToOther(tile123, TileFace.Bottom, tile233, TileFace.Left);

        this.gameBoard.TryAddTile("0-1-4");
        this.gameBoard.TryAddTile("0-4-5", "0-1-4", TileFace.Right, TileFace.Left);
        this.gameBoard.TryAddTile("4-4-5", "0-4-5", TileFace.Left, TileFace.Bottom);
        this.gameBoard.TryAddTile("3-4-4", "4-4-5", TileFace.Bottom, TileFace.Right);
        this.gameBoard.TryAddTile("3-3-4", "3-4-4", TileFace.Bottom, TileFace.Left);
        this.gameBoard.TryAddTile("3-3-3", "3-3-4", TileFace.Bottom, TileFace.Right);
        this.gameBoard.TryAddTile("2-3-3", "3-3-3", TileFace.Bottom, TileFace.Left);
        this.gameBoard.TryAddTile("1-2-3", "2-3-3", TileFace.Bottom, TileFace.Left);
    }

    private void InitGameBoard()
    {
        // remove existing Tiles
        this.ResetPoolAndTiles();
        ActualTilePool = Instantiate(TilePool) as GameObject;
        this.InitTilePool();

        this.NewTilePositionsByOtherOrientationAndTileFace = this.InitNewTilePositionsByOtherOrientationAndTileFace();
        this.NewTileOrientationByOtherFaceAndThisFace = this.InitNewTileOrientationByOtherFaceAndThisFace();

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
    public Stack<string> InitTilePool()
    {
        List<string> tilePool = new List<string>();

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

        tilePool.Shuffle();
        return new Stack<string>(tilePool);
    }

    private Dictionary<int, Dictionary<TileFace, Vector2>> InitNewTilePositionsByOtherOrientationAndTileFace()
    {
        return new Dictionary<int, Dictionary<TileFace, Vector2>>()
        {
            {
                0, new Dictionary<TileFace, Vector2>()
                {
                    { TileFace.Right, new Vector2(61.5f, 35f) },
                    { TileFace.Left, new Vector2(-61.5f, 35f) },
                    { TileFace.Bottom, new Vector2(0f, -71f) },
                }
            },
            {
                60, new Dictionary<TileFace, Vector2>()
                {
                    { TileFace.Right, new Vector2(0f, 71f) },
                    { TileFace.Left, new Vector2(-61.5f, -35f) },
                    { TileFace.Bottom, new Vector2(61.5f, -35f) },
                }
            },
            {
                120, new Dictionary<TileFace, Vector2>()
                {
                    { TileFace.Right, new Vector2(-61.5f, 35f) },
                    { TileFace.Left, new Vector2(0f, -71f) },
                    { TileFace.Bottom, new Vector2(61.5f, 35f) },
                }
            },
            {
                180, new Dictionary<TileFace, Vector2>()
                {
                    { TileFace.Right, new Vector2(-61.5f, -35f) },
                    { TileFace.Left, new Vector2(61.5f, -35f) },
                    { TileFace.Bottom, new Vector2(0f, 71f) },
                }
            },
            {
                -120, new Dictionary<TileFace, Vector2>()
                {
                    { TileFace.Right, new Vector2(0f, -71f) },
                    { TileFace.Left, new Vector2(61.5f, 35f) },
                    { TileFace.Bottom, new Vector2(-61.5f, 35f) },
                }
            },
            {
                -60, new Dictionary<TileFace, Vector2>()
                {
                    { TileFace.Right, new Vector2(61.5f, -35f) },
                    { TileFace.Left, new Vector2(0f, 71f) },
                    { TileFace.Bottom, new Vector2(-61.5f, -35f) },
                }
            }
        };
    }

    private Dictionary<TileFace, Dictionary<TileFace, float>> InitNewTileOrientationByOtherFaceAndThisFace()
    {
        return new Dictionary<TileFace, Dictionary<TileFace, float>>()
        {
            {
                TileFace.Right, new Dictionary<TileFace, float>()
                {
                    { TileFace.Right, 180f },
                    { TileFace.Bottom, -60f },
                    { TileFace.Left, 60f }
                }
            },
            {
                TileFace.Bottom, new Dictionary<TileFace, float>()
                {
                    { TileFace.Right, 60f },
                    { TileFace.Bottom, 180f },
                    { TileFace.Left, -60f }
                }
            },
            {
                TileFace.Left, new Dictionary<TileFace, float>()
                {
                    { TileFace.Right, -60f },
                    { TileFace.Bottom, +60f },
                    { TileFace.Left, 180f }
                }
            }
        };
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
        IEnumerable<GameObject> objects = GameObject.FindGameObjectsWithTag(TagManager.TILEPOOL);
        objects = objects.Concat(GameObject.FindGameObjectsWithTag(TagManager.PLAYERTILE));
        if (objects.Count() > 0)
        {
            foreach (GameObject tilePool in objects)
            {
                Destroy(tilePool);
            }
        }
    }

    public GameObject DrawRandomTile(GameObject targetDrawBoard = null)
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

        return randomTile;
    }

    private GameObject DrawSpecificTile(string name, GameObject targetDrawBoard = null)
    {
        GameObject tile = ActualTilePool.transform.Find(name).gameObject;
        tile.SetActive(true);

        if (targetDrawBoard != null)
        {
            targetDrawBoard.GetComponent<DrawBoardManager>().AddTile(tile);
        }

        return tile;
    }

    public int GethHighestTriominoOfSameKindForPlayer(PlayerCode player)
    {
        int highestSameKindTriomino = -1;

        foreach (Transform child in this.DrawBoards[player].transform)
        {
            if (child.gameObject.CompareTag(TagManager.PLAYERTILE) && child.gameObject.GetComponent<TileManager>().IsSameKindTriomino())
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
            if (child.gameObject.CompareTag(TagManager.PLAYERTILE))
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

    #region TryPlaceTile
    /// <summary>
    /// Places a tile on the GameBoard only if this tile can be placed.
    /// </summary>
    /// <param name="tile">The tile to be placed.</param>
    /// <returns>True if it was placed, false if not.</returns>
    public bool TryPlaceTile(GameObject tile)
    {
        // Check if tile can be placed
        if (tile.GetComponent<TileManager>().CanPlaceTileOnGameBoard())
        {
            // if it's the first turn, tile hast to be placed in center with orientation straight
            if (GameManager.instance.TurnCount == 0 && this.gameBoard.TryAddTile(tile.gameObject.name))
            {
                this.PlaceTileInCenter(tile);
                return true;
            }

            // if it's not the first turn, the tile hast to be placed adjacent to another tile, according to the others tile
            // orientation and the faces with which both tiles should be placed adjacent to each other.
            KeyValuePair<TileFace, GameObject> adjacentTile = tile.GetComponent<TileManager>().GetAllAdjacentTiles().First();
            TileFace otherFace = adjacentTile.Value.GetComponent<TileManager>().GetAdjacentFaceToOtherTile(tile);

            if (this.gameBoard.TryAddTile(tile.name, adjacentTile.Value.name, adjacentTile.Key, otherFace))
            {
                this.PlaceTileNextToOther(tile, adjacentTile.Key, adjacentTile.Value);
                this.GetDrawBoardForActivePlayer().GetComponent<DrawBoardManager>().RemoveTile(tile);
                return true;
            }
        }

        return false;
    }
    #endregion

    private void PlaceTileOnActualPosition(GameObject tile, GameObject drawBoardManager = null)
    {
        tile.transform.position = new Vector3(tile.transform.position.x, tile.transform.position.y, this.BoardPositionZ);
        if (drawBoardManager != null)
        {
            drawBoardManager.GetComponent<DrawBoardManager>().RemoveTile(tile);
        }        
    }

    private void PlaceTileNextToOther(GameObject thisTile, TileFace thisFace, GameObject otherTile, TileFace otherFace = TileFace.None)
    {
        if (otherFace.Equals(TileFace.None))
        {
            otherFace = otherTile.GetComponent<TileManager>().GetAdjacentFaceToOtherTile(thisTile);
        }

        Vector3 thisTilePosition = this.GetNewTilePositionFromPlacedTile(otherFace, otherTile);
        Quaternion thisTileRotation = this.GetNewTileOrientationByOtherFaceAndThisFace(otherTile, thisTile, otherFace, thisFace);

        thisTile.transform.SetPositionAndRotation(thisTilePosition, thisTileRotation);
    }

    private void PlaceTileInCenter(GameObject tile)
    {
        //tile.transform.position = new Vector3(0, 0, this.BoardPositionZ);
        tile.transform.SetPositionAndRotation(new Vector3(0, 0, this.BoardPositionZ), Quaternion.Euler(new Vector3(0, 0, 0)));
        this.GetDrawBoardForActivePlayer().GetComponent<DrawBoardManager>().RemoveTile(tile);
    }

    public bool TilePoolIsEmpty()
    {
        return this.TilePool.transform.childCount < 1;
    }

    private Vector3 GetNewTilePositionFromPlacedTile(TileFace placedTileFace, GameObject placedTile)
    {
        int orientation = Convert.ToInt32(Math.Round(placedTile.transform.rotation.eulerAngles.z));

        if (orientation > 180)
        {
            orientation -= 360;
        }

        Vector2 positionToAdd = this.NewTilePositionsByOtherOrientationAndTileFace[orientation][placedTileFace];

        Vector3 newPosition = new Vector3(placedTile.transform.position.x, placedTile.transform.position.y, this.BoardPositionZ);
        newPosition += new Vector3(positionToAdd.x, positionToAdd.y, 0);

        return newPosition;
    }

    private Quaternion GetNewTileOrientationByOtherFaceAndThisFace(GameObject otherTile, GameObject thisTile, TileFace otherFace, TileFace thisFace)
    {
        float newZRotation = otherTile.transform.rotation.eulerAngles.z;
        newZRotation += this.NewTileOrientationByOtherFaceAndThisFace[otherFace][thisFace];

        return Quaternion.Euler(thisTile.transform.rotation.eulerAngles.x, thisTile.transform.rotation.eulerAngles.y, newZRotation);
    }
}
