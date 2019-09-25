using Assets.Scripts;
using GraphKI.GameManagement;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class TileManager : MonoBehaviour
{
    public Color originColor;
    public Color matching = Color.green;
    public Color notMatching = Color.red;
    public float rayCastDistance = 60.0f;
    public GameObject rayCastStart;
    public GameObject rayCastRight;
    public GameObject rayCastLeft;
    public GameObject rayCastBottom;

    private LayerMask raycastFilter;

    private float circleCastDistance = 40f;
    private float cirlceCastRadius = 40f;

    void Awake()
    {
        raycastFilter = LayerMask.GetMask(new string[] { LayerManager.PLACEDTILELAYER });
        originColor = this.GetComponent<Renderer>().material.color;
    }

    public TileFace GetAdjacentFaceToOtherTile(GameObject other) 
    {
        Vector2 direction = this.GetDirection(other);
        Debug.Log("Direction (locale): " + direction);
        TileFace hitDirection = this.GetHitDirection(direction);
        Debug.Log("HitDirection: " + hitDirection);
        return hitDirection;
    }

    private Vector2 GetDirection(GameObject other)
    {
        Vector2 direction = (other.transform.position - this.transform.position).normalized;
        //Debug.Log("Direction world: " + direction);
        direction = this.transform.InverseTransformDirection(direction);
        //Debug.Log("Direction local: " + direction);
        direction = direction.normalized;

        return direction;
    }

    private TileFace GetHitDirection(Vector2 direction)
    {
        if (direction.y < -0.5f && direction.x >= -0.9f && direction.x <= 0.9f)
        //if ((direction.x >= -0.1f && direction.x <= 0.1f) && (direction.y >= -1.1f && direction.y <= -0.9f))
        {
            return TileFace.Bottom;
        }

        if (direction.x > 0.0f && direction.y >= -0.5f && direction.y <= 1.0f)
        //if ((direction.x <= 1.0f && direction.x >= 0.8f) && (direction.y <= 0.6f && direction.y >= 0.4f))
        {
            return TileFace.Right;
        }

        if (direction.x <= 0.0f && direction.y >= -0.5f && direction.y <= 1.0f)
        //if ((direction.x >= -1.0f && direction.x <= -0.8f) && (direction.y >= 0.4f && direction.y <= 0.6f))
        {
            return TileFace.Left;
        }

        return TileFace.None;
    }

    private GameObject GetAdjacentTileInDirection(TileFace tileFace)
    {
        Vector2 direction = this.GetFaceDirection(tileFace);
        RaycastHit2D objectHit = Physics2D.Raycast(this.transform.position, direction, rayCastDistance, layerMask: raycastFilter);
        if (objectHit.collider != null)
        {
            Debug.Log("Side: " + tileFace + " Distance: " + objectHit.distance);
            return objectHit.collider.gameObject;
        }

        // Make Raycast visible in Editor
        Color color = Color.black;
        switch (tileFace)
        {
            case TileFace.Right:
                color = Color.red;
                break;
            case TileFace.Left:
                color = Color.blue;
                break;
            case TileFace.Bottom:
                color = Color.green;
                break;
        }
        Debug.DrawRay(this.transform.position, direction * rayCastDistance, color);

        return null;
    }

    public Dictionary<TileFace, GameObject> GetAllAdjacentTiles()
    {
        TileFace[] facesToCheck = new TileFace[3] { TileFace.Left, TileFace.Right, TileFace.Bottom };
        Dictionary<TileFace, GameObject> adjacentTiles = new Dictionary<TileFace, GameObject>();

        foreach (TileFace face in facesToCheck)
        {
            GameObject adjacentTile = this.GetAdjacentTileInDirection(face);
            if (adjacentTile != null)
            {
                adjacentTiles.Add(face, adjacentTile);
            }
        }

        return adjacentTiles;
    }

    private void SetTileColor(Color color)
    {
        this.GetComponent<Renderer>().material.color = color;
    }

    public void SetColorMatching()
    {
        this.SetTileColor(this.matching);
    }

    public void SetColorNotMatching()
    {
        this.SetTileColor(this.notMatching);
    }

    public void ResetColor()
    {
        this.SetTileColor(originColor);
    }

    private Vector2 GetFaceDirection(TileFace tileFace)
    {
        Vector2 direction;
        switch (tileFace)
        {
            case TileFace.Right:
                direction = this.rayCastRight.transform.position - this.rayCastStart.transform.position;
                break;
            case TileFace.Left:
                direction = this.rayCastLeft.transform.position - this.rayCastStart.transform.position;
                break;
            case TileFace.Bottom:
                direction = this.rayCastBottom.transform.position - this.rayCastStart.transform.position;
                break;
            default:
                direction = new Vector2();
                break;
        }

        direction = direction.normalized;
        return direction;
    }

    public bool CheckIfOtherTileOrientationMatches(GameObject other)
    {
        float rotation1 = this.transform.rotation.eulerAngles.z;
        float rotation2 = other.transform.rotation.eulerAngles.z;

        int orientation1 = Convert.ToInt32((Math.Abs(rotation1) / 60.0f) % 2);
        int orientation2 = Convert.ToInt32((Math.Abs(rotation2) / 60.0f) % 2);

        if (orientation1 < 0 || orientation1 > 1 || orientation2 < 0 || orientation2 > 1)
        {
            throw new ArgumentException("Orientation value has to be 0 or 1");
        }

        return orientation1 != orientation2;
    }

    private string GetValueFromTileFace(TileFace face)
    {
        string[] parts = this.gameObject.name.Split('-');
        if (parts.Length != 3)
        {
            throw new ArgumentException($"Tile name ('{name}') does not contain three numbers.");
        }

        string faceValue = string.Empty;
        switch (face)
        {
            case TileFace.Right:
                faceValue = parts[0] + "-" + parts[1];
                break;
            case TileFace.Bottom:
                faceValue = parts[1] + "-" + parts[2];
                break;
            case TileFace.Left:
                faceValue = parts[2] + "-" + parts[0];
                break;
            default:
                break;
        }

        return faceValue;
    }

    public bool CanPlaceNextToOtherTile(TileFace thisFace, GameObject other)
    {
        if (!this.CheckIfOtherTileOrientationMatches(other))
        {
            return false;
        }

        TileFace faceOther = other.GetComponent<TileManager>().GetAdjacentFaceToOtherTile(this.gameObject);
        string faceValueOther = other.gameObject.GetComponent<TileManager>().GetValueFromTileFace(faceOther);
        string faceValueThis = this.GetValueFromTileFace(thisFace);

        if (!UnityGameManager.instance.CheckFaceValues(faceValueOther, faceValueThis))
        {
            return false;
        }

        return true;
    }

    public int GetTileValue()
    {
        string[] parts = this.GetNameParts();

        int points = parts.Select(n => int.Parse(n)).Aggregate((a, b) => a + b);
        Debug.Log("TilePoints: " + points);
        return points;
    }

    public bool IsSameKindTriomino()
    {
        string[] parts = this.GetNameParts();

        if (parts[0].Equals(parts[1]) && parts[1].Equals(parts[2]))
        {
            return true;
        }

        return false;
    }

    private string[] GetNameParts()
    {
        string[] parts = this.gameObject.name.Split('-');
        if (parts.Length != 3)
        {
            throw new ArgumentException("Der Name eins Spielsteines muss die Form '1-2-3' haben.");
        }

        return parts;
    }

    public bool CanPlaceTileOnGameBoard(Action<bool, GameObject> modifyAdjacentTile = null)
    {
        Dictionary<TileFace, GameObject> adjacentTiles = this.GetAllAdjacentTiles();

        if (UnityGameManager.instance.GameManager.TurnCount > 0 && !adjacentTiles.Any())
        {
            return false;
        }

        foreach (KeyValuePair<TileFace, GameObject> kv in adjacentTiles)
        {
            TileFace otherFace = kv.Value.GetComponent<TileManager>().GetAdjacentFaceToOtherTile(this.gameObject);
            if (!UnityGameManager.instance.GameManager.GameBoard.CanPlaceTileOnGameBoard(this.gameObject.name, kv.Value.gameObject.name, kv.Key, otherFace, out TriominoTile placableTile))
            {                 
                modifyAdjacentTile?.Invoke(false, kv.Value);

                if (modifyAdjacentTile == null)
                {
                    return false;
                }
            }
            else
            {
                modifyAdjacentTile?.Invoke(true, kv.Value);
            }
        }

        // Examine if single tile corner is adjacent to another tiles corner (Bridge) and color this other tile accordingly.
        if (adjacentTiles.Count == 1 && !this.CheckIfNumberOppositeOfFaceMatches(adjacentTiles.First().Key, modifyAdjacentTile))
        {
            return false;
        }

        return true;
    }

    public bool CheckIfNumberOppositeOfFaceMatches(TileFace matchingFace, Action<bool, GameObject> modifyAdjacentTile = null)
    {
        GameObject opositeNumber = this.gameObject.transform.Find("Number3").gameObject;
        switch (matchingFace)
        {
            case TileFace.Left:
                opositeNumber = this.gameObject.transform.Find("Number2").gameObject;
                break;
            case TileFace.Bottom:
                opositeNumber = this.gameObject.transform.Find("Number1").gameObject;
                break;
        }

        Vector2 origin = new Vector2(opositeNumber.transform.position.x, opositeNumber.transform.position.y);
        Vector3 direction = (opositeNumber.transform.position - this.gameObject.GetComponent<TileManager>().rayCastStart.transform.position).normalized;

        Debug.DrawRay(new Vector3(origin.x, origin.y, 0), direction * circleCastDistance, Color.white);

        RaycastHit2D[] numbersInRange = Physics2D.CircleCastAll(origin, cirlceCastRadius, new Vector2(direction.x, direction.y), circleCastDistance);
        numbersInRange = numbersInRange.Where(n => n.collider != null && n.collider.gameObject.CompareTag(TagManager.TILENUMBER)).ToArray();

        if (numbersInRange.Length > 0)
        {
            foreach (RaycastHit2D number in numbersInRange)
            {
                if (opositeNumber.gameObject.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text != number.collider.gameObject.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text)
                {
                    modifyAdjacentTile?.Invoke(false, number.collider.transform.parent.gameObject);
                    return false;
                }
                else
                {
                    modifyAdjacentTile?.Invoke(true, number.collider.transform.parent.gameObject);
                }
            }
        }

        return true;
    }

    #region SetNumber1
    /// <summary>
    /// Sets Text for Number 1
    /// </summary>
    /// <param name="content">The text to be set.</param>
    public void SetNumber1(string content)
    {
        this.SetNumber("Number1", content);
    }
    #endregion

    #region SetNumber2
    /// <summary>
    /// Sets Text for Number 2
    /// </summary>
    /// <param name="content">The text to be set.</param>
    public void SetNumber2(string content)
    {
        this.SetNumber("Number2", content);
    }
    #endregion

    #region SetNumber3
    /// <summary>
    /// Sets Text for Number 3
    /// </summary>
    /// <param name="content">The text to be set.</param>
    public void SetNumber3(string content)
    {
        this.SetNumber("Number3", content);
    }
    #endregion

    #region SetNumber
    /// <summary>
    /// Sets the text for a given Number gameObject.
    /// </summary>
    /// <param name="numberName">The name of the numbers gameobject.</param>
    /// <param name="content">The text to be set.</param>
    private void SetNumber(string numberName, string content)
    {
        GameObject number = this.gameObject.transform.Find(numberName).gameObject;
        number.gameObject.transform.GetChild(0).GetComponent<TextMeshProUGUI>().SetText(content);
    }
    #endregion
}
