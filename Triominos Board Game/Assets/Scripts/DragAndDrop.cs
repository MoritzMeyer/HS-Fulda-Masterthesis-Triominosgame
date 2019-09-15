using Assets.Scripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DragAndDrop : MonoBehaviour
{
    [HideInInspector]
    public bool selected;
    [HideInInspector]
    public bool IsOverDrawBoard;

    private Vector3 originLocalePosition;
    private Quaternion originRotation;
    private bool hadParent = true;    

    // Update is called once per frame
    void Update()
    {
        if (selected)
        {
            Vector2 cursorPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            this.transform.position = new Vector3(cursorPosition.x, cursorPosition.y, this.transform.position.z);

            if (Input.GetMouseButtonDown(1))
            {
                this.gameObject.transform.Rotate(new Vector3(0, 0, -60));
            }

            if ((Input.GetKey(KeyCode.Escape) ||
                    (Input.GetMouseButtonUp(0) &&
                        (IsOverDrawBoard ||
                            !this.gameObject.GetComponent<TileManager>().CanPlaceTileOnGameBoard()
                        )
                    )
                ) &&
                this.hadParent)
            {
                // the order of setting parent, position and rotation is important
                this.selected = false;
                this.gameObject.transform.rotation = this.originRotation;
                this.transform.SetParent(GameManager.instance.boardManager.GetDrawBoardForActivePlayer().transform);
                this.gameObject.layer = this.transform.parent.gameObject.layer;
                this.gameObject.transform.localPosition = this.originLocalePosition;
                GameManager.instance.boardManager.StopDragging();
            }
            else if (Input.GetMouseButtonUp(0) && !IsOverDrawBoard)
            {
                if (GameManager.instance.TryPlaceTile(this.gameObject))
                {
                    this.selected = false;
                    GameManager.instance.boardManager.StopDragging();
                }
            }
        }
    }

    private void OnMouseOver()
    {
        if (Input.GetMouseButtonDown(0) &&
            !GameManager.instance.boardManager.IsDragging &&
            !this.gameObject.layer.Equals(LayerMask.NameToLayer(LayerManager.DEFAULTLAYER)) &&
            !IsMouseOverUI() &&
            this.gameObject.transform.parent.gameObject.GetComponent<DrawBoardManager>().IsActiveDrawBoard())
        {
            GameManager.instance.boardManager.StartDragging();
            this.hadParent = (this.transform.parent != null);

            // the order of removing Parent and caching position/rotation is important
            this.originLocalePosition = this.gameObject.transform.localPosition;
            this.selected = true;
            this.gameObject.layer = LayerMask.NameToLayer(LayerManager.DEFAULTLAYER);
            this.transform.SetParent(null);
            this.originRotation = this.gameObject.transform.rotation;
            this.transform.position = new Vector3(this.transform.position.x, this.transform.position.y, this.transform.position.z - 1);
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag.Equals(TagManager.DRAWBOARD))
        {
            this.IsOverDrawBoard = true;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.tag.Equals(TagManager.DRAWBOARD))
        {
            this.IsOverDrawBoard = false;
        }
    }

    private bool IsMouseOverUI()
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
