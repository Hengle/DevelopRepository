using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RopeCollider : MonoBehaviour
{
    Collider myCollider;
    Rigidbody myRigidbody;
    HingeJoint myHingeJoint;
    RopeRenderer ropeRenderer;

    public event Action<Transform, Collider> OnTriggerListener;
    public event Action<Transform, Collision> OnCollisionListener;

    private void Awake()
    {
        myCollider = GetComponent<Collider>();
        myRigidbody = GetComponent<Rigidbody>();
        myHingeJoint = GetComponent<HingeJoint>();
        ropeRenderer = GetComponent<RopeRenderer>();
    }


    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag != "Character")
        {
            OnCollisionListener?.Invoke(transform, collision);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Character")
        {
            OnTriggerListener?.Invoke(transform, other);
        }
    }
}
