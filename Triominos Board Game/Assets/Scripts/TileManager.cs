using Assets.Scripts;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TileManager : MonoBehaviour
{
    public Color originColor;
    public Color matching = Color.green;
    public Color notMatching = Color.red;
    public float rayCastDistance = 100.0f;
    public GameObject rayCastStart;
    public GameObject rayCastRight;
    public GameObject rayCastLeft;
    public GameObject rayCastBottom;

    private LayerMask raycastFilter;

    private void Awake()
    {
        raycastFilter = GameManager.instance.boardManager.PlacedTileLayer.Value;
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
        Dictionary<TileFace, GameObject> adjacentTiles = new Dictionary<TileFace, GameObject>();
        GameObject objectOnLeftFace = this.GetAdjacentTileInDirection(TileFace.Left);
        GameObject objectOnRightFace = this.GetAdjacentTileInDirection(TileFace.Right);
        GameObject objectOnBottomFace = this.GetAdjacentTileInDirection(TileFace.Bottom);

        adjacentTiles.Add(TileFace.Left, objectOnLeftFace);
        adjacentTiles.Add(TileFace.Right, objectOnRightFace);
        adjacentTiles.Add(TileFace.Bottom, objectOnBottomFace);

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

    public bool CanPlacedNextToOtherTile(TileFace thisFace, GameObject other)
    {
        if (!this.CheckIfOtherTileOrientationMatches(other))
        {
            return false;
        }

        TileFace faceOther = other.GetComponent<TileManager>().GetAdjacentFaceToOtherTile(this.gameObject);
        string faceValueOther = other.gameObject.GetComponent<TileManager>().GetValueFromTileFace(faceOther);
        string faceValueThis = this.GetValueFromTileFace(thisFace);

        if (!GameManager.instance.CheckFaceValues(faceValueOther, faceValueThis))
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
}
