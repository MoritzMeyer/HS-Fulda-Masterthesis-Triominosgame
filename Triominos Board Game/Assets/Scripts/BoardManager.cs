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
    private Queue<TriominoTileEventArgs> TilesToPlace;

    [HideInInspector]
    public Dictionary<PlayerCode, GameObject> DrawBoardManagers;

    public bool IsDragging { get; private set; }
    private Dictionary<int, Dictionary<TileFace, Vector2>> NewTilePositionsByOtherOrientationAndTileFace;
    private Dictionary<TileFace, Dictionary<TileFace, float>> NewTileOrientationByOtherFaceAndThisFace;
    private bool isReady;

    private void Update()
    {
        if (!UnityGameManager.instance.GameManager.CanDrawTile())
        {
            this.DrawButton.GetComponent<DrawButtonManager>().Deactivate();
        }

        if (this.TilesToPlace.Count > 0 && this.isReady)
        {
            TriominoTileEventArgs e = this.TilesToPlace.Dequeue();
            this.TryPlaceTileFromDrawBoard(e.Player.Value, e.TileName, e.OtherTileName, e.TileFace, e.OtherTileFAce);
        }
    }

    public void InitBoard()
    {
        this.InitGameBoard();
    }

    #region InitGameBoard
    /// <summary>
    /// Initializes the GameBoard
    /// </summary>
    private void InitGameBoard()
    {
        this.ResetTiles();
        this.TilesToPlace = new Queue<TriominoTileEventArgs>();
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

        switch (UnityGameManager.instance.GameManager.GameMode)
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
                throw new ArgumentException($"Unbekannter gameMode '{UnityGameManager.instance.GameManager.GameMode}'");
        }

        UnityGameManager.instance.GameManager.GameBoard.TilePlaced += this.OnTilePlaced;
        this.isReady = true;
    }
    #endregion

    #region ResetTiles
    /// <summary>
    /// Removes all existing tiles within the scene
    /// </summary>
    private void ResetTiles()
    {
        GameObject[] tiles = GameObject.FindGameObjectsWithTag(TagManager.PLAYERTILE);
        foreach (GameObject tile in tiles)
        {
            Destroy(tile);
        }

        this.PlacedTiles = new GameObject
        {
            name = "PlacedTiles"
        };
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

    #region DrawSpecificTile
    /// <summary>
    /// Draws a specific Tile from gameboard
    /// </summary>
    /// <param name="name">name of demanded tile</param>
    /// <param name="targetDrawBoard">drawboard to which the drawn tile should be added.</param>
    /// <returns></returns>
    private bool DrawSpecificTile(string name, DrawBoard targetDrawBoard)
    {
        return UnityGameManager.instance.GameManager.DrawSpecificTile(name, targetDrawBoard);
    }
    #endregion

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
        this.isReady = false;
        // Check if tile can be placed
        if (tile.GetComponent<TileManager>().CanPlaceTileOnGameBoard())
        {
            // if it's the first turn, tile hast to be placed in center with orientation straight
            if (UnityGameManager.instance.GameManager.TurnCount == 0 && UnityGameManager.instance.GameManager.TryPlaceOnGameBoard(tile.gameObject.name))
            {
                this.PlaceTileInCenter(tile);
                this.isReady = true;
                return true;
            }

            // if it's not the first turn, the tile hast to be placed adjacent to another tile, according to the others tile
            // orientation and the faces with which both tiles should be placed adjacent to each other.
            KeyValuePair<TileFace, GameObject> adjacentTile = tile.GetComponent<TileManager>().GetAllAdjacentTiles().First();
            TileFace otherFace = adjacentTile.Value.GetComponent<TileManager>().GetAdjacentFaceToOtherTile(tile);

            if (UnityGameManager.instance.GameManager.TryPlaceOnGameBoard(tile.name, adjacentTile.Value.name, adjacentTile.Key, otherFace))
            {
                this.PlaceTileNextToOther(tile, adjacentTile.Key, adjacentTile.Value);
                this.isReady = true;
                return true;
            }
        }

        this.isReady = true;
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
        if (otherTileName == null || otherTileName == string.Empty|| !tileFace.HasValue || !otherFace.HasValue)
        {
            this.PlaceTileInCenter(tile);
            return;
        }

        GameObject otherTile = this.PlacedTiles.transform.Find(otherTileName).gameObject;
        //GameObject otherTile = GameObject.Find(otherTileName);
        this.PlaceTileNextToOther(tile, tileFace.Value, otherTile, otherFace.Value);
        tile.GetComponent<FadeToColor>().StartFadeToOrigin();
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
        this.TilesToPlace.Enqueue(e);
        //if (UnityGameManager.instance.GameManager.IsAiPlayer(UnityGameManager.instance.GameManager.ActivePlayer))
        //{            
        //    StartCoroutine(WaitUntilReady(e));
        //}
    }
    #endregion

    #region WaitUntilReady
    /// <summary>
    /// Coroutine which waits with placing a tile until the board is ready for placing
    /// (previous tiles has to be placed first)
    /// </summary>
    /// <param name="e">event args.</param>
    /// <returns>IEnumerator for coroutine</returns>
    IEnumerator WaitUntilReady(TriominoTileEventArgs e)
    {
        while(!this.isReady)
        {
            yield return new WaitForSeconds(0.25f);
        }
        this.TryPlaceTileFromDrawBoard(e.Player.Value, e.TileName, e.OtherTileName, e.TileFace, e.OtherTileFAce);
    }
    #endregion
}
