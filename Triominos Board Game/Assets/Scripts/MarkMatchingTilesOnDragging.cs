using Assets.Scripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MarkMatchingTilesOnDragging : MonoBehaviour
{
    public Color ColorMatching = Color.green;
    public Color ColorNotMatching = Color.red;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (this.gameObject.GetComponent<DragAndDrop>().selected && 
            collision.gameObject.tag.Equals("PlayerTile") && 
            collision.gameObject.layer == 0)
        {
            Vector2 direction = this.GetDirection(collision.gameObject);
            Debug.Log("Direction (locale): " + direction);
            HitDirection hitDirection = GameManager.instance.boardManager.GetHitDirection(direction);
            Debug.Log("HitDirection: " + hitDirection);
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (this.gameObject.GetComponent<DragAndDrop>().selected && 
            collision.gameObject.tag.Equals("PlayerTile") && 
            collision.gameObject.layer == 0)
        {
            collision.gameObject.GetComponent<Renderer>().material.color = this.gameObject.GetComponent<Renderer>().material.color;
        }
    }

    public Vector2 GetDirection(GameObject other)
    {
        Vector2 direction = (other.transform.position - this.transform.position).normalized;
        //Debug.Log("Direction world: " + direction);
        direction = this.transform.InverseTransformDirection(direction);
        //Debug.Log("Direction local: " + direction);
        direction = direction.normalized;

        return direction;
    }
}
