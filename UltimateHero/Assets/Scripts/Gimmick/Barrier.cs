using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Util;

public class Barrier : MonoBehaviour, IReflectable
{
    Rigidbody rigidbody;

    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();
    }

    public void Reflect(GameObject gameObject, Collision collision)
    {
        gameObject.GetComponent<Rigidbody>().AddForce(collision.contacts[0].normal * 5.0f, ForceMode.Impulse);
    }
}
