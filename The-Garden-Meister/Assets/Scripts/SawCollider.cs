using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SawCollider : MonoBehaviour
{
    Quaternion quaternion;
    private void Awake()
    {
        quaternion = transform.localRotation;
    }

    private void Update()
    {
        transform.localRotation = quaternion;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Target")
        {
            var rb = collision.gameObject.GetComponent<Rigidbody2D>();
            rb.gravityScale = 1f;
        }
    }
}
