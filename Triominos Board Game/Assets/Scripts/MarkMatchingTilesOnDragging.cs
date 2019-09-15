using Assets.Scripts;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MarkMatchingTilesOnDragging : MonoBehaviour
{
    private List<GameObject> previousTiles;
    public float distance = 40f;
    public float radius = 60f;

    private void Update()
    {
        if (previousTiles != null && previousTiles.Count > 0)
        {
            foreach (GameObject tile in previousTiles)
            {
                tile.GetComponent<TileManager>().ResetColor();
            }

            previousTiles = null;
        }

        previousTiles = new List<GameObject>();
        if (this.GetComponent<DragAndDrop>().selected)
        {
            this.GetComponent<TileManager>().CanPlaceTileOnGameBoard((tileMatches, tile) =>
            {
                if (tileMatches)
                {
                    tile.GetComponent<TileManager>().SetColorMatching();
                }
                else
                {
                    tile.GetComponent<TileManager>().SetColorNotMatching();
                }

                this.previousTiles.Add(tile);
            });
        }
    }
}
