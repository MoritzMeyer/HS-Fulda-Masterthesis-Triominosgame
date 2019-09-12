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
                Debug.Log("AdjacentTile (" + kv.Value.name + ") on " + kv.Key);

                if (this.gameObject.GetComponent<TileManager>().CheckIfOtherTileOrientationMatches(kv.Value))
                {
                    if (this.gameObject.GetComponent<TileManager>().CanPlaceNextToOtherTile(kv.Key, kv.Value))
                    {
                        kv.Value.GetComponent<TileManager>().SetColorMatching();
                    }
                    else
                    {
                        kv.Value.GetComponent<TileManager>().SetColorNotMatching();
                    }
                }
            }

            previousTiles = adjacentTiles.Values.Where(g => g != null).ToArray();
        }
    }
}
