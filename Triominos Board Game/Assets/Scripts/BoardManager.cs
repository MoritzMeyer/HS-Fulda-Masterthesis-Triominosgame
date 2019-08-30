using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;

public class BoardManager : MonoBehaviour
{
    public GameObject TilePool;
    public GameObject DrawBoardPlayer1;
    public GameObject DrawBoardPlayer2;
    public Button DrawButton;

    private GameObject actualTilePool;

    public void InitBoard()
    {
        this.InitTilePool();
    }

    public void InitTilePool()
    {
        // remove existing Tiles
        this.ResetPoolAndTiles();

        //this.TilePool = Resources.Load<GameObject>("Prefabs/TilePool") as GameObject;
        //this.TilePool = Instantiate(TilePool) as GameObject;
        actualTilePool = Instantiate(TilePool) as GameObject;

        DrawBoardPlayer1 = GameObject.Find("DrawBoardPlayer1");
        DrawBoardPlayer2 = GameObject.Find("DrawBoardPlayer2");
        DrawButton = GameObject.Find("DrawButton").GetComponent<Button>();
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

    public void PlaceRandomTile()
    {
        int randomIndex = Random.Range(0, actualTilePool.transform.childCount);
        GameObject randomTile = actualTilePool.transform.GetChild(randomIndex).gameObject;
        randomTile.gameObject.SetActive(true);
        DrawBoardPlayer1.GetComponent<DrawBoardManager>().AddTile(randomTile);
        if (TilePool.transform.childCount < 1)
        {
            DrawButton.gameObject.SetActive(false);
        }
    }

}
