using Assets.Scripts;
using GraphKI.Extensions;
using GraphKI.GameManagement;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DrawBoardManager : MonoBehaviour
{
    public GameObject triominioTilePrefab;

    public float TilePositionY = 5.0f;
    public float TilePositionZ = -2f;
    public float TileMaxPositionX = 0.32f;
    public float TileMinPositionX = -0.32f;
    public float TileOffsetX = 0.08f;

    public List<GameObject> tilesOnDrawBoard;

    public Color highlightColor = new Color32(92, 255, 0, 255);
    private Color originColor;

    private void Awake()
    {
        this.originColor = this.gameObject.GetComponent<Renderer>().material.color;
    }

    private void Update()
    {
        if (this.IsActiveDrawBoard())
        {
            this.HighlightDrawBoard();
        }
        else
        {
            this.SetNormalColor();
        }
    }

    #region Init
    /// <summary>
    /// Initializes new DrawBoardManager
    /// </summary>
    /// <param name="drawBoard">The DrawBoard to managed.</param>
    public void Init(DrawBoard drawBoard)
    {
        this.tilesOnDrawBoard = new List<GameObject>();
        foreach (string tileName in drawBoard.GetTilesOnDrawBoard())
        {
            this.AddTile(tileName);
        }

        drawBoard.TileAdded += (object sender, TriominoTileEventArgs e) => { this.AddTile(e.TileName); };
        drawBoard.TileRemoved += (object sender, TriominoTileEventArgs e) => { this.RemoveTile(e.TileName); };
    }
    #endregion

    #region AddTile
    /// <summary>
    /// Instantiates a new GameObject for the new Tile based on its name and adds this new GameObject to the drawBoard.
    /// </summary>
    /// <param name="name">Name of the Triomino Tile</param>
    public void AddTile(string name)
    {
        GameObject tile = this.InstantiateTileFromName(name);
        this.AddTile(tile);
    }

    /// <summary>
    /// Adds a new TriominoTile(GameObject) to the DrawBoard
    /// by setting parent, layer and initializing DragAndDrop for the new Tile.
    /// </summary>
    /// <param name="tile">New tiles gameobject.</param>
    public void AddTile(GameObject tile)
    {
        tile.transform.SetParent(this.transform);
        tile.layer = this.gameObject.layer;
        tile.GetComponent<DragAndDrop>().IsOverDrawBoard = true;
        this.ReArrangeTiles();
        this.tilesOnDrawBoard.Add(tile);
        tile.GetComponent<FadeToColor>().StartFadeToOrigin();
    }
    #endregion

    #region RemoveTile
    /// <summary>
    /// Removes a Tile from the DrawBoard.
    /// </summary>
    /// <param name="name">Name of the tile to be removed.</param>
    public void RemoveTile(string name)
    {
        GameObject tile = this.tilesOnDrawBoard.Single(t => t.gameObject.name.Equals(name));
        this.RemoveTile(tile);
    }

    /// <summary>
    /// Removes the GameObject of a triominoTile from drawboard 
    /// by unsetting its parent and changing layer
    /// </summary>
    /// <param name="tile">The tiles gameobject to be removed.</param>
    public void RemoveTile(GameObject tile)
    {
        tile.transform.SetParent(null);
        tile.GetComponent<DragAndDrop>().IsOverDrawBoard = false;
        tile.layer = LayerMask.NameToLayer(LayerManager.PLACEDTILELAYER);
        this.ReArrangeTiles();
        this.tilesOnDrawBoard.Remove(tile);
    }
    #endregion

    #region ReArrangeTiles
    /// <summary>
    /// Rearranges all triomino-tile-gameobjects within the drawboard.
    /// </summary>
    public void ReArrangeTiles()
    {
        float startPositionX = 0 - ((this.transform.childCount - 1) * this.TileOffsetX / 2);
        for (int i = 0; i < this.transform.childCount; i++)
        {
            float positionX = startPositionX + (i * TileOffsetX);
            Transform child = this.transform.GetChild(i);
            //child.localPosition = new Vector3(positionX, TilePositionY, this.transform.position.z);
            child.localPosition = new Vector3(positionX, TilePositionY, TilePositionZ);
        }
    }
    #endregion

    #region IsActiveDrawBoard
    /// <summary>
    /// Determines wether this drawboard is associated with the actual (active) player or not.
    /// </summary>
    /// <returns>True if it is, false if not.</returns>
    public bool IsActiveDrawBoard()
    {
        return this.gameObject.name.Equals(UnityGameManager.instance.boardManager.GetDrawBoardManagerForActivePlayer().gameObject.name);
    }
    #endregion

    #region HighlightDrawBoard
    /// <summary>
    /// Changes the drawboards color, so that it can be identified as active.
    /// </summary>
    public void HighlightDrawBoard()
    {
        this.gameObject.GetComponent<Renderer>().material.color = highlightColor;
    }
    #endregion

    #region SetNormalColor
    /// <summary>
    /// Resets the color of the drawboard (after highlighting it for example).
    /// </summary>
    public void SetNormalColor()
    {
        this.gameObject.GetComponent<Renderer>().material.color = originColor;
    }
    #endregion

    #region InstantiateTileFromName
    /// <summary>
    /// Instantiates a GameObject for a new TriominoTile and sets its 
    /// Numbers based on its name.
    /// </summary>
    /// <param name="tileName">The name of the new TriominoTile</param>
    /// <returns>The GameObject</returns>
    private GameObject InstantiateTileFromName(string tileName)
    {
        GameObject tile = Instantiate(triominioTilePrefab);
        tile.gameObject.name = tileName;

        tile.GetComponent<TileManager>().SetNumber1(tileName.GetTriominoTileNumbersFromName()[0]);
        tile.GetComponent<TileManager>().SetNumber2(tileName.GetTriominoTileNumbersFromName()[1]);
        tile.GetComponent<TileManager>().SetNumber3(tileName.GetTriominoTileNumbersFromName()[2]);

        return tile;
    }
    #endregion
}
