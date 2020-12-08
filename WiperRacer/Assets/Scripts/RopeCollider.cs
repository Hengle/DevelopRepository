using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RopeCollider : MonoBehaviour
{
    Collider myCollider;
    HingeJoint myHingeJoint;
    RopeRenderer ropeRenderer;

    public event Action<HingeJoint> OnCollisionListener;

    private void Awake()
    {
        myCollider = GetComponent<Collider>();
        myHingeJoint = GetComponent<HingeJoint>();
        ropeRenderer = GetComponent<RopeRenderer>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag != "Ground")
        {
            //OnCollisionListener?.Invoke(myHingeJoint);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Saw")
        {
            if (other.gameObject.transform.parent.parent != transform.parent.parent)
            {
                OnCollisionListener?.Invoke(myHingeJoint);
            }
        }
    }
}
