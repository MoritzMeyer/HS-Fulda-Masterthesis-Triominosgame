using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

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
        if (Input.GetMouseButtonDown(0) && !IsMouseOverUi())
        {
            this.selected = true;
            this.gameObject.layer = 0;
        }
    }

    private bool IsMouseOverUi()
    {
        return EventSystem.current.IsPointerOverGameObject();
    }

    //private bool IsPointerOverUIObject()
    //{
    //    PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current)
    //    {
    //        position = new Vector2(Input.mousePosition.x, Input.mousePosition.y)
    //    };
    //    List<RaycastResult> results = new List<RaycastResult>();
    //    EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
    //    return results.Count > 0;
    //}
}
