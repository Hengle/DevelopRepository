using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterCollider : MonoBehaviour
{
    Collider myCollider;

    public event Action<Collision, Transform> OnCollisionListener;
    public event Action<Collider, Transform> OnTriggerListener;

    private void Awake()
    {
        myCollider = GetComponentInChildren<Collider>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        OnCollisionListener?.Invoke(collision, transform);
    }
    private void OnTriggerEnter(Collider other)
    {
        OnTriggerListener?.Invoke(other, transform);
    }
}
