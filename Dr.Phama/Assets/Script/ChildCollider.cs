using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChildCollider : MonoBehaviour
{
    public event Action<ChildCollider> OnCollisionEnterListener;

    void OnTriggerEnter(Collider collision)
    {
        var i = collision.gameObject.GetComponent<Collider>();
        if (i != null)
        {
            OnCollisionEnterListener?.Invoke(this);
        }
    }
}
