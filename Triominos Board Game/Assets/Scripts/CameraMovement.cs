using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    [Range(0.0f, 10.0f)]
    public float speed = 10f;

    [Range(1.0f, 100.0f)]
    public float dragSpeed = 10.0f;

    private Vector3 mouseOrigin;
    private readonly float basicSpeed = 100;
    private bool isPanning = false;

    #region Update
    // Update is called once per frame
    void Update()
    {
        Vector3 velocity = this.GetBaseMovement();
        velocity = velocity * Time.deltaTime * speed * basicSpeed;
        transform.Translate(velocity);

        if (Input.GetMouseButtonDown(1) && !GameManager.instance.boardManager.IsDragging)
        {
            mouseOrigin = Input.mousePosition;
            isPanning = true;

        }

        if (Input.GetMouseButtonUp(1) && !GameManager.instance.boardManager.IsDragging)
        {
            isPanning = false;
        }

        if (isPanning)
        {
            Vector3 pos = Camera.main.ScreenToViewportPoint(Input.mousePosition - mouseOrigin);
            Vector3 move = new Vector3(pos.x * dragSpeed,pos.y * dragSpeed, 0);

            transform.Translate(move, Space.Self);
        }
    }
    #endregion

    #region GetBaseMovement
    private Vector3 GetBaseMovement()
    {
        Vector3 velocity = new Vector3();

        if (Input.GetKey(KeyCode.UpArrow))
        {
            velocity += new Vector3(0, 1, 0);
        }

        if (Input.GetKey(KeyCode.DownArrow))
        {
            velocity += new Vector3(0, -1, 0);
        }

        if (Input.GetKey(KeyCode.LeftArrow))
        {
            velocity += new Vector3(-1, 0, 0);
        }

        if (Input.GetKey(KeyCode.RightArrow))
        {
            velocity += new Vector3(1, 0, 0);
        }

        return velocity;
    }
    #endregion
}
