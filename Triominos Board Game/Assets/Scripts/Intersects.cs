using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Intersects : MonoBehaviour
{
    public string OtherTag;
    public Color highlightColor = Color.yellow;
    private Color originColor = new Color();

    public void Start()
    {
        originColor = GetComponent<Renderer>().material.color;
    }

    #region Update
    // Update is called once per frame
    void Update()
    {   
    }
    #endregion

    #region OnCollisionEnter2D
    public void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("OnCollisionEnter2D");
        if (object.Equals(collision.gameObject.tag, OtherTag) && collision.gameObject.GetComponent<DragAndDrop>().selected)
        {
            GetComponent<Renderer>().material.color = highlightColor;
            //GetComponent<Outline>()
        }
    }
    #endregion

    public void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.GetComponent<DragAndDrop>().selected)
        {
            GetComponent<Renderer>().material.color = highlightColor;
        }
        else
        {
            GetComponent<Renderer>().material.color = originColor;
        }
    }

    #region OnCollisionExit2D
    public void OnCollisionExit2D(Collision2D collision)
    {
        Debug.Log("OnCollisionExit2D");
        if (object.Equals(collision.gameObject.tag, OtherTag))
        {
            GetComponent<Renderer>().material.color = originColor;
        }
    }
    #endregion

}
