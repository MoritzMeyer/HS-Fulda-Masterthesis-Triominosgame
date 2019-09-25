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
    public GameObject DrawBoardPlayer1;
    public GameObject DrawBoardPlayer2;
    public GameObject DrawBoardPlayer3;
    public GameObject DrawBoardPlayer4;
    public Button DrawButton;

    private GameObject PlacedTiles;

    [HideInInspector]
    public Dictionary<PlayerCode, GameObject> DrawBoardManagers;

    public bool IsDragging { get; private set; }
    private Dictionary<int, Dictionary<TileFace, Vector2>> NewTilePositionsByOtherOrientationAndTileFace;
    private Dictionary<TileFace, Dictionary<TileFace, float>> NewTileOrientationByOtherFaceAndThisFace;

    private void Update()
    {
        if (!UnityGameManager.instance.GameManager.CanDrawTile())
        {
            this.DrawButton.GetComponent<DrawButtonManager>().Deactivate();
        }
    }


    public void InitBoard()
    {
        this.InitGameBoard();
        //this.InitTestScene();
    }

    //private void InitTestScene()
    //{
    //    this.NewTilePositionsByOtherOrientationAndTileFace = this.InitNewTilePositionsByOtherOrientationAndTileFace();
    //    this.NewTileOrientationByOtherFaceAndThisFace = this.InitNewTileOrientationByOtherFaceAndThisFace();

    //    DrawBoardPlayer1 = GameObject.Find("DrawBoardPlayer1");
    //    DrawBoardPlayer2 = GameObject.Find("DrawBoardPlayer2");
    //    DrawButton = GameObject.Find("DrawButton").GetComponent<Button>();

    //    DrawBoards = new Dictionary<PlayerCode, GameObject>()
    //    {
    //        { PlayerCode.Player1, DrawBoardPlayer1 },
    //        { PlayerCode.Player2, DrawBoardPlayer2 }
    //    };

    //    switch (UnityGameManager.instance.GameMode)
    //    {
    //        case GameMode.TwoPlayer:
    //            break;
    //        case GameMode.ThreePlayer:
    //            DrawBoards.Add(PlayerCode.Player3, DrawBoardPlayer3);
    //            break;
    //        case GameMode.FourPlayer:
    //            DrawBoards.Add(PlayerCode.Player4, DrawBoardPlayer4);
    //            break;
    //        default:
    //            throw new ArgumentException($"Unbekannter gameMode '{UnityGameManager.instance.GameMode}'");
    //    }

    //    this.InitBridgeScene();
    //}

    //private void InitBridgeScene()
    //{
    //    GameObject tile014 = this.DrawSpecificTile("0-1-4", DrawBoards[PlayerCode.Player1]);
    //    GameObject tile045 = this.DrawSpecificTile("0-4-5", DrawBoards[PlayerCode.Player1]);
    //    GameObject tile445 = this.DrawSpecificTile("4-4-5", DrawBoards[PlayerCode.Player1]);
    //    GameObject tile344 = this.DrawSpecificTile("3-4-4", DrawBoards[PlayerCode.Player1]);
    //    GameObject tile334 = this.DrawSpecificTile("3-3-4", DrawBoards[PlayerCode.Player1]);
    //    GameObject tile333 = this.DrawSpecificTile("3-3-3", DrawBoards[PlayerCode.Player1]);
    //    GameObject tile233 = this.DrawSpecificTile("2-3-3", DrawBoards[PlayerCode.Player1]);
    //    GameObject tile123 = this.DrawSpecificTile("1-2-3", DrawBoards[PlayerCode.Player1]);
    //    GameObject tile113 = this.DrawSpecificTile("1-1-3", DrawBoards[PlayerCode.Player1]);
    //    GameObject tile133 = this.DrawSpecificTile("1-3-3", DrawBoards[PlayerCode.Player1]);

    //    tile014.transform.SetParent(null);
    //    tile014.transform.position = new Vector3(0, 0, this.BoardPositionZ);
    //    this.PlaceTileOnActualPosition(tile014, DrawBoards[PlayerCode.Player1]);

    //    DrawBoardManager drawBoardManager = DrawBoards[PlayerCode.Player1].GetComponent<DrawBoardManager>();

    //    drawBoardManager.RemoveTile(tile045);
    //    drawBoardManager.RemoveTile(tile445);
    //    drawBoardManager.RemoveTile(tile344);
    //    drawBoardManager.RemoveTile(tile334);
    //    drawBoardManager.RemoveTile(tile333);
    //    drawBoardManager.RemoveTile(tile233);
    //    drawBoardManager.RemoveTile(tile123);

    //    this.PlaceTileNextToOther(tile045, TileFace.Right, tile014, TileFace.Left);
    //    this.PlaceTileNextToOther(tile445, TileFace.Left, tile045, TileFace.Bottom);
    //    this.PlaceTileNextToOther(tile344, TileFace.Bottom, tile445, TileFace.Right);
    //    this.PlaceTileNextToOther(tile334, TileFace.Bottom, tile344, TileFace.Left);
    //    this.PlaceTileNextToOther(tile333, TileFace.Bottom, tile334, TileFace.Right);
    //    this.PlaceTileNextToOther(tile233, TileFace.Bottom, tile333, TileFace.Left);
    //    this.PlaceTileNextToOther(tile123, TileFace.Bottom, tile233, TileFace.Left);

    //    UnityGameManager.instance.GameManager.GameBoard.TryAddTile("0-1-4");
    //    UnityGameManager.instance.GameManager.GameBoard.TryAddTile("0-4-5", "0-1-4", TileFace.Right, TileFace.Left);
    //    UnityGameManager.instance.GameManager.GameBoard.TryAddTile("4-4-5", "0-4-5", TileFace.Left, TileFace.Bottom);
    //    UnityGameManager.instance.GameManager.GameBoard.TryAddTile("3-4-4", "4-4-5", TileFace.Bottom, TileFace.Right);
    //    UnityGameManager.instance.GameManager.GameBoard.TryAddTile("3-3-4", "3-4-4", TileFace.Bottom, TileFace.Left);
    //    UnityGameManager.instance.GameManager.GameBoard.TryAddTile("3-3-3", "3-3-4", TileFace.Bottom, TileFace.Right);
    //    UnityGameManager.instance.GameManager.GameBoard.TryAddTile("2-3-3", "3-3-3", TileFace.Bottom, TileFace.Left);
    //    UnityGameManager.instance.GameManager.GameBoard.TryAddTile("1-2-3", "2-3-3", TileFace.Bottom, TileFace.Left);
    //}

    #region InitGameBoard
    /// <summary>
    /// Initializes the GameBoard
    /// </summary>
    private void InitGameBoard()
    {
        this.PlacedTiles = new GameObject();
        this.NewTilePositionsByOtherOrientationAndTileFace = this.InitNewTilePositionsByOtherOrientationAndTileFace();
        this.NewTileOrientationByOtherFaceAndThisFace = this.InitNewTileOrientationByOtherFaceAndThisFace();

        DrawBoardPlayer1 = GameObject.Find("DrawBoardPlayer1");
        DrawBoardPlayer2 = GameObject.Find("DrawBoardPlayer2");
        DrawButton = GameObject.Find("DrawButton").GetComponent<Button>();

        DrawBoardManagers = new Dictionary<PlayerCode, GameObject>()
        {
            { PlayerCode.Player1, DrawBoardPlayer1 },
            { PlayerCode.Player2, DrawBoardPlayer2 }
        };

        foreach(KeyValuePair<PlayerCode, GameObject> kv in DrawBoardManagers)
        {
            kv.Value.GetComponent<DrawBoardManager>().Init(UnityGameManager.instance.GameManager.GetPlayersDrawBoard(kv.Key));
        }

        switch (UnityGameManager.instance.GameMode)
        {
            case GameMode.TwoPlayer:
                break;
            case GameMode.ThreePlayer:
                DrawBoardManagers.Add(PlayerCode.Player3, DrawBoardPlayer3);
                break;
            case GameMode.FourPlayer:
                DrawBoardManagers.Add(PlayerCode.Player4, DrawBoardPlayer4);
                break;
            default:
                throw new ArgumentException($"Unbekannter gameMode '{UnityGameManager.instance.GameMode}'");
        }

        UnityGameManager.instance.GameManager.GameBoard.TilePlaced += this.OnTilePlaced; 
    }
    #endregion

    #region InitNewTilePositionsByOtherOrientationAndTileFace
    /// <summary>
    /// Initiailizes Dictionary with TilePositions based on another tiles orientation and face to place adjacent to.
    /// </summary>
    /// <returns>The Dictionary.</returns>
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
    #endregion

    #region InitNewTileOrientationByOtherFaceAndThisFace
    /// <summary>
    /// Initiailizes Dictionary with TileOrientations based on another tiles orientation and face to place adjacent to.
    /// </summary>
    /// <returns></returns>
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
    #endregion

    //private GameObject DrawSpecificTile(string name, GameObject targetDrawBoard = null)
    //{
    //    GameObject tile = ActualTilePool.transform.Find(name).gameObject;
    //    tile.SetActive(true);

    //    if (targetDrawBoard != null)
    //    {
    //        targetDrawBoard.GetComponent<DrawBoardManager>().AddTile(tile);
    //    }

    //    return tile;
    //}

    #region StartDragging
    /// <summary>
    /// Indicates, that a player is dragging a tile on gameboard.
    /// </summary>
    public void StartDragging()
    {
        this.IsDragging = true;
    }
    #endregion

    #region StopDragging
    /// <summary>
    /// Indicates, that a player is not dragging a tile anymore.
    /// </summary>
    public void StopDragging()
    {
        this.IsDragging = false;
    }
    #endregion

    #region GetDrawBoardForActivePlayer
    /// <summary>
    /// Get Drawboard for active player.
    /// </summary>
    /// <returns>actives player drawboard.</returns>
    public GameObject GetDrawBoardManagerForActivePlayer()
    {
        return this.DrawBoardManagers[UnityGameManager.instance.GameManager.ActivePlayer];
    }
    #endregion

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
            if (UnityGameManager.instance.GameManager.TurnCount == 0 && UnityGameManager.instance.GameManager.TryPlaceOnGameBoard(tile.gameObject.name))
            {
                this.PlaceTileInCenter(tile);
                return true;
            }

            // if it's not the first turn, the tile hast to be placed adjacent to another tile, according to the others tile
            // orientation and the faces with which both tiles should be placed adjacent to each other.
            KeyValuePair<TileFace, GameObject> adjacentTile = tile.GetComponent<TileManager>().GetAllAdjacentTiles().First();
            TileFace otherFace = adjacentTile.Value.GetComponent<TileManager>().GetAdjacentFaceToOtherTile(tile);

            if (UnityGameManager.instance.GameManager.TryPlaceOnGameBoard(tile.name, adjacentTile.Value.name, adjacentTile.Key, otherFace))
            {
                this.PlaceTileNextToOther(tile, adjacentTile.Key, adjacentTile.Value);
                return true;
            }
        }

        return false;
    }
    #endregion

    #region TryPlacedTileFromDrawBoard
    /// <summary>
    /// Places a tile from a Drawboard. 
    /// !!!! This tile was placed within GraphKI.GameManagement.GameBoard before !!!!
    /// </summary>
    /// <param name="player">The Player for whom this tile is placed.</param>
    /// <param name="tileName">The name of the tile to be placed.</param>
    /// <param name="otherTileName">The name of the tile to which the new tile should be placed adjacent to.</param>
    /// <param name="tileFace">The tiles face wich should be adjacent to other tile after placing.</param>
    /// <param name="otherFace">the other tiles face which should be adjacent to tile after placing.</param>
    public void TryPlaceTileFromDrawBoard(PlayerCode player, string tileName, string otherTileName = null, TileFace? tileFace = null, TileFace? otherFace = null)
    {
        GameObject tile = GameObject.Find(tileName);
        if (otherTileName == null || !tileFace.HasValue || !otherFace.HasValue)
        {
            this.PlaceTileInCenter(tile);
        }

        GameObject otherTile = this.PlacedTiles.transform.Find(otherTileName).gameObject;
        this.PlaceTileNextToOther(tile, tileFace.Value, otherTile, otherFace.Value);
    }
    #endregion

    #region PlaceTileOnActualPosition
    /// <summary>
    /// Places a tile on its actual position
    /// </summary>
    /// <param name="tile">GameObject of the tile to be placed.</param>
    /// <param name="drawBoardManager">If existent the drawboardmanager who holds this tile before placement.</param>
    private void PlaceTileOnActualPosition(GameObject tile, GameObject drawBoardManager = null)
    {
        tile.transform.position = new Vector3(tile.transform.position.x, tile.transform.position.y, this.BoardPositionZ);
        tile.transform.SetParent(this.PlacedTiles.transform);
    }
    #endregion

    #region PlaceTileNextToOther
    /// <summary>
    /// Places a Tile on the GameBoard next to another tile, based on both tiles 
    /// gameobjects and the faces with which they should be placed towards each other.
    /// </summary>
    /// <param name="thisTile">The GameObject of the tile to be placed.</param>
    /// <param name="thisFace">The face of the tile to be placed.</param>
    /// <param name="otherTile">The other tiles GameObject.</param>
    /// <param name="otherFace">The others tile face.</param>
    private void PlaceTileNextToOther(GameObject thisTile, TileFace thisFace, GameObject otherTile, TileFace otherFace = TileFace.None)
    {
        if (otherFace.Equals(TileFace.None))
        {
            otherFace = otherTile.GetComponent<TileManager>().GetAdjacentFaceToOtherTile(thisTile);
        }

        Vector3 thisTilePosition = this.GetNewTilePositionFromPlacedTile(otherFace, otherTile);
        Quaternion thisTileRotation = this.GetNewTileOrientationByOtherFaceAndThisFace(otherTile, thisTile, otherFace, thisFace);

        thisTile.transform.SetPositionAndRotation(thisTilePosition, thisTileRotation);
        thisTile.transform.SetParent(this.PlacedTiles.transform);
    }
    #endregion

    #region PlaceTileInCenter
    /// <summary>
    /// Places a tile in the center of the gameboard.
    /// </summary>
    /// <param name="tile">The GameObject of the tile.</param>
    private void PlaceTileInCenter(GameObject tile)
    {
        //tile.transform.position = new Vector3(0, 0, this.BoardPositionZ);
        tile.transform.SetPositionAndRotation(new Vector3(0, 0, this.BoardPositionZ), Quaternion.Euler(new Vector3(0, 0, 0)));
        tile.transform.SetParent(this.PlacedTiles.transform);
    }
    #endregion

    #region GetNewTilePositionFromPlacedTile
    /// <summary>
    /// Determines the position for a new Tile according to another tiles positions.
    /// </summary>
    /// <param name="placedTileFace">The face (which is adjacent to the new tile) of the tile on the gameboard.</param>
    /// <param name="placedTile">The GameObject of the tile on the gameboard.</param>
    /// <returns>The new Position</returns>
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
    #endregion

    #region GetNewTileOrientationByOtherFaceAndThisFace
    /// <summary>
    /// Determines the orientation (rotation) of a new tile, which should be placed on the gameboard.
    /// </summary>
    /// <param name="otherTile">The tile next to which the new tile should be placed.</param>
    /// <param name="thisTile">The tile which should be placed.</param>
    /// <param name="otherFace">The face of the other tile which should be adjacent to the new tile.</param>
    /// <param name="thisFace">The face of the new tile, which should be adjacent to the palced tile.</param>
    /// <returns>The new rotation.</returns>
    private Quaternion GetNewTileOrientationByOtherFaceAndThisFace(GameObject otherTile, GameObject thisTile, TileFace otherFace, TileFace thisFace)
    {
        float newZRotation = otherTile.transform.rotation.eulerAngles.z;
        newZRotation += this.NewTileOrientationByOtherFaceAndThisFace[otherFace][thisFace];

        return Quaternion.Euler(thisTile.transform.rotation.eulerAngles.x, thisTile.transform.rotation.eulerAngles.y, newZRotation);
    }
    #endregion

    #region OnTilePlaced
    /// <summary>
    /// EventHandler for TilePlaced events.
    /// </summary>
    /// <param name="sender">sender</param>
    /// <param name="e">event args</param>
    private void OnTilePlaced(object sender, TriominoTileEventArgs e)
    {
        if (UnityGameManager.instance.GameManager.AIPlayers[UnityGameManager.instance.GameManager.ActivePlayer])
        {
            this.TryPlaceTileFromDrawBoard(e.Player.Value, e.TileName, e.OtherTileName, e.TileFace, e.OtherTileFAce);
        }
    }
    #endregion
}
