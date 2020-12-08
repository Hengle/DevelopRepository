using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Head : MonoBehaviour , IPushable
{
    Rigidbody rigidbody;
    public event Action OnPushListener;

    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();
    }

    public void Push(Vector3 normal)
    {
        rigidbody.useGravity = true;
        rigidbody.AddForce(normal * -1 * 10, ForceMode.Impulse);
        OnPushListener?.Invoke();
    }
}
