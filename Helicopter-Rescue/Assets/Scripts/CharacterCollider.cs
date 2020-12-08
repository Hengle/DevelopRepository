using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterCollider : MonoBehaviour
{
    Collider myCollider;

    public event Action<Collider, Vector3> OnTriggerListener;

    private void Awake()
    {
        myCollider = GetComponentInChildren<Collider>();
    }

    private void OnTriggerEnter(Collider other)
    {
        OnTriggerListener?.Invoke(other,transform.position);
    }
}
