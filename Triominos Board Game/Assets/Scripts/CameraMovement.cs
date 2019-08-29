using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    [Range(0.0f, 10.0f)]
    public float speed = 10f;
    private readonly float basicSpeed = 100;

    #region Update
    // Update is called once per frame
    void Update()
    {
        Vector3 velocity = this.GetBaseMovement();
        velocity = velocity * Time.deltaTime * speed * basicSpeed;
        transform.Translate(velocity);
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
