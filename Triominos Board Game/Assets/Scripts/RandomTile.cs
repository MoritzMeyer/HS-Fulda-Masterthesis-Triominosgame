using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomTile : MonoBehaviour
{
    public void DrawRandomTile()
    {
        GameManager.instance.boardManager.PlaceRandomTile();
    }
}
