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
    public float BoardPositionZ = 1.0f;
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
        this.DefaultLayer = this.DefaultLayer ?? LayerMask.NameToLayer("Default");
        this.PlayerHudLayer = this.PlayerHudLayer ?? LayerMask.NameToLayer("Player HUD");
        this.PlacedTileLayer = this.PlacedTileLayer ?? LayerMask.NameToLayer("PlacedTile");

        this.InitTilePool();
    }

    private void InitTilePool()
    {
        // remove existing Tiles
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

        this.DrawStartTiles();
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
        IEnumerable<GameObject> objects = GameObject.FindGameObjectsWithTag(GameObjectTags.TILEPOOL);
        objects = objects.Concat(GameObject.FindGameObjectsWithTag(GameObjectTags.PLAYERTILE));
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

    public int GethHighestTriominoOfSameKindForPlayer(PlayerCode player)
    {
        int highestSameKindTriomino = -1;

        foreach (Transform child in this.DrawBoards[player].transform)
        {
            if (child.gameObject.CompareTag(GameObjectTags.PLAYERTILE) && child.gameObject.GetComponent<TileManager>().IsSameKindTriomino())
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
            if (child.gameObject.CompareTag(GameObjectTags.PLAYERTILE))
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

    public bool TryPlaceTile(GameObject tile)
    {
        if (tile.GetComponent<TileManager>().CanPlaceTileOnGameBoard())
        {
            if (GameManager.instance.TurnCount == 0)
            {
                this.PlaceTileInCenter(tile);
                return true;
            }

            KeyValuePair<TileFace, GameObject> adjacentTile = tile.GetComponent<TileManager>().GetAllAdjacentTiles().First();
            this.PlaceTileNextToOther(tile, adjacentTile.Key, adjacentTile.Value);
            this.GetDrawBoardForActivePlayer().GetComponent<DrawBoardManager>().RemoveTile(tile);

            return true;
        }

        return false;
    }

    private void PlaceTileOnActualPosition(GameObject tile)
    {
        tile.transform.position = new Vector3(tile.transform.position.x, tile.transform.position.y, this.BoardPositionZ);
        this.GetDrawBoardForActivePlayer().GetComponent<DrawBoardManager>().RemoveTile(tile);
    }

    private void PlaceTileNextToOther(GameObject thisTile, TileFace thisFace, GameObject otherTile)
    {
        TileFace otherFace = otherTile.GetComponent<TileManager>().GetAdjacentFaceToOtherTile(thisTile);
        Vector3 thisTilePosition = this.GetNewTilePositionFromPlacedTile(otherFace, otherTile);
        Quaternion thisTileRotation = this.GetNewTileOrientationByOtherFaceAndThisFace(otherTile, thisTile, otherFace, thisFace);

        thisTile.transform.SetPositionAndRotation(thisTilePosition, thisTileRotation);
    }

    private void PlaceTileInCenter(GameObject tile)
    {
        tile.transform.position = new Vector3(0, 0, this.BoardPositionZ);
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
