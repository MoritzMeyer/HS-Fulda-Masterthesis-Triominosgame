using Assets.Scripts;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MarkMatchingTilesOnDragging : MonoBehaviour
{
    GameObject[] previousTiles;

    private void Update()
    {
        if (previousTiles != null && previousTiles.Length > 0)
        {
            foreach (GameObject tile in previousTiles)
            {
                tile.GetComponent<TileManager>().ResetColor();
            }

            previousTiles = null;
        }

        if (this.GetComponent<DragAndDrop>().selected)
        {
            Dictionary<TileFace, GameObject> adjacentTiles = this.GetComponent<TileManager>().GetAllAdjacentTiles();

            foreach(KeyValuePair<TileFace, GameObject> kv in adjacentTiles)
            {
                if (kv.Value != null)
                {
                    Debug.Log("AdjacentTile (" + kv.Value.name + ") on " + kv.Key);
                    if (GameManager.instance.boardManager.CheckIfTileOrientationMatches(this.gameObject, kv.Value))
                    {
                        TileFace faceOther = kv.Value.GetComponent<TileManager>().GetSelfAdjacentSide(this.gameObject);
                        string faceValueOther = GameManager.instance.boardManager.GetValueFromTileFace(faceOther, kv.Value.name);
                        string faceValueThis = GameManager.instance.boardManager.GetValueFromTileFace(kv.Key, this.gameObject.name);

                        if (GameManager.instance.CheckFaceValues(faceValueOther, faceValueThis))
                        {
                            kv.Value.GetComponent<TileManager>().SetColorMatching();
                        }
                        else
                        {
                            kv.Value.GetComponent<TileManager>().SetColorNotMatching();
                        }
                    }
                }
            }

            previousTiles = adjacentTiles.Values.Where(g => g != null).ToArray();
        }
    }
}
