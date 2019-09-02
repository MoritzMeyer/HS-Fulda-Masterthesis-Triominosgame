using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RayCastFromMovingToStatic : MonoBehaviour
{
    public GameObject rightSide;
    public GameObject leftSide;
    public GameObject bottomSide;

    private bool rayCastDone = false;
    public enum HitDirection { None, Right, Left, Bottom }

    // Update is called once per frame
    void Update()
    {
        if (!this.rayCastDone)
        {
            Debug.Log("#######################-RayCastFromMovingToStatic-#######################");
            Debug.Log("RayColors: rightSide = red; leftSide = blue; bottomSide = green");
            Debug.Log("RightSide:");
            this.ReturnDirection(rightSide, this.gameObject, Color.red);
            //this.ReturnDirection(this.gameObject, rightSide, Color.red);

            Debug.Log("LeftSide:");
            this.ReturnDirection(leftSide, this.gameObject, Color.blue);
            //this.ReturnDirection(this.gameObject, leftSide, Color.blue);

            Debug.Log("BottomSide:");
            this.ReturnDirection(bottomSide, this.gameObject, Color.green);
            //this.ReturnDirection(this.gameObject, bottomSide, Color.green);

            this.rayCastDone = true;
        }
    }

    //public HitDirection ReturnDirection(GameObject other, GameObject objectHit)
    //{
    //    HitDirection hitDirection = HitDirection.None;
    //    RaycastHit MyRayHit;
    //    Vector3 direction = (other.transform.position - objectHit.transform.position).normalized;
    //    Ray MyRay = new Ray(objectHit.transform.position, direction);

    //    if (Physics.Raycast(MyRay, out MyRayHit))
    //    {
    //        if (MyRayHit.collider != null)
    //        {
    //            Vector3 MyNormal = MyRayHit.normal;
    //            Debug.Log("WorldNormal: " + MyNormal);
    //            MyNormal = MyRayHit.transform.TransformDirection(MyNormal);
    //            Debug.Log("LocalNormal: " + MyNormal);
    //        }
    //    }

    //    return hitDirection;
    //}

    public HitDirection ReturnDirection(GameObject other, GameObject objectHit, Color rayColor)
    {
        HitDirection hitDirection = HitDirection.None;
        Vector2 direction = (other.transform.position - objectHit.transform.position).normalized;
        //Ray MyRay = new Ray(objectHit.transform.position, direction);

        RaycastHit2D MyRayHit = Physics2D.Raycast(objectHit.transform.position, direction);
    
        if (MyRayHit.collider != null)
        {
            Vector2 MyNormal = MyRayHit.normal;
            Debug.Log("WorldNormal: " + MyNormal);
            //MyNormal = MyRayHit.transform.InverseTransformDirection(MyNormal);
            //MyNormal = MyRayHit.transform.TransformDirection(MyNormal);
            Debug.Log("LocalNormal: " + MyNormal);

            //Debug.DrawRay(objectHit.transform.position, direction * 100, rayColor, 100);
            Debug.DrawRay(other.transform.position, MyNormal * 100, rayColor, 1000);
        }
        

        return hitDirection;
    }
}
