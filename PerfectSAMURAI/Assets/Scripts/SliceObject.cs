using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SliceObject : MonoBehaviour
{
    Rigidbody myRigidbody;
    Collider myCollider;
    Vector3 rotate;

    private void Awake()
    {
        gameObject.layer = 0;
        gameObject.tag = "SliceObject";
        myRigidbody = gameObject.AddComponent<Rigidbody>();
        myRigidbody.useGravity = false;
        myRigidbody.interpolation = RigidbodyInterpolation.Interpolate;

        //myCollider = gameObject.AddComponent<MeshCollider>();
        //myCollider.convex = true;
    }

    public void SetRotate(Vector3 rotate)
    {
        this.rotate = rotate;
    }

    public void AddForce()
    {
        transform.Rotate(rotate);
        myRigidbody.AddForce(transform.forward * 5f, ForceMode.Impulse);
    }
}
