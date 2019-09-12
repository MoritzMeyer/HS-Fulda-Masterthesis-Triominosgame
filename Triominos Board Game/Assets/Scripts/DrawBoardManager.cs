using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawBoardManager : MonoBehaviour
{
    public float TilePositionY = 5.0f;
    public float TilePositionZ = -2f;
    public float TileMaxPositionX = 0.32f;
    public float TileMinPositionX = -0.32f;
    public float TileOffsetX = 0.08f;

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

    public void RemoveTile(GameObject tile)
    {
        tile.transform.SetParent(null);
        tile.layer = GameManager.instance.boardManager.PlacedTileLayer.Value;
        this.ReArrangeTiles();        
    }

    public void AddTile(GameObject tile)
    {
        tile.transform.SetParent(this.transform);
        tile.layer = this.gameObject.layer;
        tile.GetComponent<DragAndDrop>().IsOverDrawBoard = true;
        this.ReArrangeTiles();
    }

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

    public bool IsActiveDrawBoard()
    {
        return this.gameObject.name.Equals(GameManager.instance.boardManager.GetDrawBoardForActivePlayer().gameObject.name);
    }

    public void HighlightDrawBoard()
    {
        this.gameObject.GetComponent<Renderer>().material.color = highlightColor;
    }

    public void SetNormalColor()
    {
        this.gameObject.GetComponent<Renderer>().material.color = originColor;
    }
}
