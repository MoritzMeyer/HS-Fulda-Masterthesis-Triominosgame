using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomTile : MonoBehaviour
{
    public GameObject Tiles;
    public GameObject DrawBoardPlayer1;
    public GameObject DrawBoardPlayer2;

    public void PlaceRandomTile()
    {
        int randomIndex = Random.Range(0, Tiles.transform.childCount);
        GameObject randomTile = Tiles.transform.GetChild(randomIndex).gameObject;
        randomTile.gameObject.SetActive(true);
        DrawBoardPlayer2.GetComponent<DrawBoardManager>().AddTile(randomTile);
    }
}
