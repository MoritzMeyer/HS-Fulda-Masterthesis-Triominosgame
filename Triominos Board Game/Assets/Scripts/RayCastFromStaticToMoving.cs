using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts;

public class RayCastFromStaticToMoving : MonoBehaviour
{
    public GameObject rightSide;
    public GameObject leftSide;
    public GameObject bottomSide;

    private bool rayCastDone = false;

    //Bottom
    //public void Start()
    //{
    //    Color color1 = Color.green;
    //    Color color2 = Color.red;
    //    Color mem = Color.black;
    //    for (float i = -0.9f; i < 1.0f; i += 0.1f)
    //    {
    //        Vector2 direction = new Vector2(i,-0.5f);
    //        direction = direction.normalized;
    //        Debug.Log("Direction local: " + direction);
    //        Debug.DrawRay(this.transform.position, direction * 100, color1, 1000);

    //        mem = color1;
    //        color1 = color2;
    //        color2 = mem;
    //    }
    //}

    //Right
    //public void Start()
    //{
    //    Color color1 = Color.green;
    //    Color color2 = Color.red;
    //    Color mem = Color.black;
    //    float y = -0.5f;
    //    for (float i = 0.9f; i >= 0.0f; i -= 0.1f)
    //    {
    //        Vector2 direction = new Vector2(i, y);
    //        direction = direction.normalized;
    //        Debug.Log("Direction local: " + direction);
    //        Debug.DrawRay(this.transform.position, direction * 100, color1, 1000);

    //        mem = color1;
    //        color1 = color2;
    //        color2 = mem;

    //        y += 0.15f;
    //    }
    //}

     //Left
    public void Start()
    {
        Color color1 = Color.green;
        Color color2 = Color.red;
        Color mem = Color.black;
        float y = -0.5f;
        for (float i = -0.9f; i <= 0.0f; i += 0.1f)
        {
            Vector2 direction = new Vector2(i, y);
            direction = direction.normalized;
            Debug.Log("Direction local: " + direction);
            Debug.DrawRay(this.transform.position, direction * 100, color1, 1000);

            mem = color1;
            color1 = color2;
            color2 = mem;

            y += 0.15f;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!this.rayCastDone)
        {
            Debug.Log("##############-RayCastFromStaticToMoving-################");
            Debug.Log("RayColors: rightSide = red; leftSide = blue; bottomSide = green");
            Debug.Log("RightSide:");
            //this.ReturnDirection(rightSide, this.gameObject, Color.red);
            //this.ReturnDirection(this.gameObject, rightSide, Color.red);
            //this.ReturnDirection(rightSide, Color.red);

            Debug.Log("LeftSide:");
            //this.ReturnDirection(leftSide, this.gameObject, Color.blue);
            //this.ReturnDirection(this.gameObject, leftSide, Color.blue);
            //this.ReturnDirection(leftSide, Color.blue);

            Debug.Log("BottomSide:");
            //this.ReturnDirection(bottomSide, this.gameObject, Color.green);
            //this.ReturnDirection(this.gameObject, bottomSide, Color.green);
            //this.ReturnDirection(bottomSide, Color.green);

            this.rayCastDone = true;

            //this.RayCastForward();
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
        //Vector2 direction = (other.transform.position - objectHit.transform.position).normalized;
        Vector2 direction = (other.transform.position - objectHit.transform.position).normalized;
        Vector2 direction2 = (objectHit.transform.localPosition - objectHit.transform.InverseTransformPoint(other.transform.position)).normalized;
        Debug.Log("Direction world: " + direction);
        Debug.Log("Direction local: " + direction2);
        Debug.Log("Direction local: " + objectHit.transform.InverseTransformDirection(direction));
        //Ray MyRay = new Ray(objectHit.transform.position, direction);

        //RaycastHit2D MyRayHit = Physics2D.Raycast(objectHit.transform.TransformPoint(other.transform.position), objectHit.transform.TransformDirection(direction));
        //RaycastHit2D MyRayHit = Physics2D.Raycast(objectHit.transform.position, objectHit.transform.InverseTransformDirection(direction));
        RaycastHit2D MyRayHit = Physics2D.Raycast(objectHit.transform.position, direction);

        if (MyRayHit.collider != null)
        {
            Vector2 MyNormal = MyRayHit.normal;
            Debug.Log("WorldNormal: " + MyNormal);
            //MyNormal = objectHit.transform.InverseTransformDirection(MyNormal);
            //MyNormal = objectHit.transform.TransformDirection(MyNormal);
            //MyNormal = MyRayHit.transform.InverseTransformDirection(MyNormal);
            //MyNormal = MyRayHit.transform.TransformDirection(MyNormal);

            //Debug.DrawRay(objectHit.transform.position, direction * 100, rayColor, 100);
            Debug.DrawRay(objectHit.transform.position, MyNormal * 100, rayColor, 1000);
            MyNormal = objectHit.transform.TransformDirection(MyNormal);
            Debug.Log("LocalNormal: " + MyNormal);
        }
        

        return hitDirection;
    }

    public void RayCastForward()
    {
        RaycastHit2D myRayHit = Physics2D.Raycast(this.transform.position, this.transform.up);
        Vector2 normal = myRayHit.normal;
        Debug.Log("Transform.up: " + this.transform.up);
        Debug.Log("Forward local: " + normal);
        normal = myRayHit.transform.TransformDirection(normal);
        Debug.Log("Forward world: " + normal);

        Debug.DrawRay(this.transform.position, normal * 100, Color.black, 1000);
    }

    public HitDirection ReturnDirection(GameObject other, Color rayColor)
    {
        HitDirection hitDirection = HitDirection.None;
        Vector2 direction = (other.transform.position - this.transform.position).normalized;
        direction = new Vector2(-direction.x, -direction.y);
        Debug.Log("Direction local: " + this.transform.InverseTransformDirection(direction));
        Debug.DrawRay(this.transform.position, direction * 100, rayColor, 1000);
        return hitDirection;
    }
}
