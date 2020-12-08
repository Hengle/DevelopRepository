using System;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public event Action OnSurroundedListener;

    private void OnTriggerStay(Collider collision)
    {
        OnSurroundedListener?.Invoke();
    }

    private void OnCollisionEnter(Collision collision)
    {
    }
}
