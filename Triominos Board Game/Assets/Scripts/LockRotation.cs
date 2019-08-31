using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockRotation : MonoBehaviour
{
    private Quaternion fixedRotation;

    private void Awake()
    {
        this.fixedRotation = this.transform.rotation;
    }

    private void LateUpdate()
    {
        this.transform.rotation = this.fixedRotation;
    }
}
