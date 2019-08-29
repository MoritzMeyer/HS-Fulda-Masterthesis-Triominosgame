using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragAndDrop : MonoBehaviour
{
    [HideInInspector]
    public bool selected;

    // Update is called once per frame
    void Update()
    {
        if (selected)
        {
            Vector2 cursorPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            this.transform.position = new Vector3(cursorPosition.x, cursorPosition.y, this.transform.position.z);
        }

        if (Input.GetMouseButtonUp(0))
        {
            this.selected = false;
        }
    }

    private void OnMouseOver()
    {
        if (Input.GetMouseButtonDown(0))
        {
            this.selected = true;
            this.gameObject.layer = 0;
        }
    }
}
