using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChildCollider : MonoBehaviour
{
    Collider myCollider;

    public event Action<Transform, Collider> OnTriggerListener;
    public event Action<Transform, Collision> OnCollisionListener;

    private void Awake()
    {
        myCollider = GetComponent<Collider>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        OnCollisionListener?.Invoke(transform, collision);
    }

    private void OnTriggerEnter(Collider other)
    {
        OnTriggerListener?.Invoke(transform, other);
    }
}
