using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrollChildren : MonoBehaviour
{
    public KeyCode KeyScrollLeft;
    public KeyCode KeyScrollRight;
    public KeyCode KeyScrollUp;
    public KeyCode KeyScrollDown;

    [Range(0.001f, 0.1f)]
    public float ScrollAmount = 0.005f;

    private Vector3 OriginPosition;
    private bool scrollContinouslyLeft = false;
    private bool scrollContinouslyRight = false;

    // Start is called before the first frame update
    void Start()
    {
        OriginPosition = this.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if ((Input.GetKey(KeyScrollLeft) && this.GetComponent<DrawBoardManager>().IsActiveDrawBoard()) || this.scrollContinouslyLeft)
        {
            this.ScrollLeft();
        }

        if ((Input.GetKey(KeyScrollRight) && this.GetComponent<DrawBoardManager>().IsActiveDrawBoard()) || this.scrollContinouslyRight)
        {
            this.ScrollRight();
        }

        if (Input.GetKey(KeyScrollUp))
        {
            this.ScrollUp();
        }

        if (Input.GetKey(KeyScrollDown))
        {
            this.ScrollDown();
        }
    }

    public void ResetPosition()
    {
        this.transform.position = OriginPosition;
    }

    public void ScrollLeft()
    {
        this.ScrollHorizontal(-ScrollAmount);
    }

    public void ScrollRight()
    {
        this.ScrollHorizontal(ScrollAmount);
    }

    public void ScrollDown()
    {
        this.ScrollVertical(-ScrollAmount);
    }

    public void ScrollUp()
    {
        this.ScrollVertical(ScrollAmount);
    }

    public void ScrollHorizontal(float amount)
    {
        for (int i = 0; i < this.transform.childCount; i++)
        {
            Transform childTransform = this.transform.GetChild(i);
            Vector3 childPosition = childTransform.localPosition;
            childTransform.localPosition = new Vector3(childPosition.x + amount, childPosition.y, childPosition.z);
        }
    }

    public void ScrollVertical(float amount)
    {
        for (int i = 0; i < this.transform.childCount; i++)
        {
            Transform childTransform = this.transform.GetChild(i);
            Vector3 childPosition = childTransform.localPosition;
            childTransform.localPosition = new Vector3(childPosition.x, childPosition.y + amount, childPosition.z);
        }
    }

    public void StartScrollingLeft()
    {
        this.scrollContinouslyLeft = true;
    }

    public void StopScrollingLeft()
    {
        this.scrollContinouslyLeft = false;
    }

    public void StartScrollingRight()
    {
        this.scrollContinouslyRight = true;
    }

    public void StopScrollingRight()
    {
        this.scrollContinouslyRight = false;
    }

}
