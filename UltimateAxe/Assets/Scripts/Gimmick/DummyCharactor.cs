using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DummyCharactor : MonoBehaviour, IPushable, IReflectable
{
    Rigidbody rigidbody;
    [SerializeField] int speed = 1;
    [SerializeField] int radius = 1;
    [SerializeField] float degree = 0;
    Vector3 defaultPosition;
    Vector3 lastPosition = Vector3.zero;

    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();
        defaultPosition = transform.position;
    }

    private void Update()
    {
        var radian = Mathf.PI / 180.0f * degree;
        var x = radius * Mathf.Sin(Time.time * speed + radian);
        var z = radius * Mathf.Cos(Time.time * speed + radian);
        var diff = lastPosition  - transform.position ;
        lastPosition = transform.position;
        if (diff != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(diff);
        }

        transform.position = new Vector3(x + defaultPosition.x, defaultPosition.y, z + defaultPosition.z );
    }

    public void Push(Vector3 normal)
    {
        rigidbody.useGravity = true;
        rigidbody.isKinematic = false;
        rigidbody.AddForce(normal * -1 * 10, ForceMode.Impulse);
    }

    public void Reflect(GameObject gameObject, Collision collision)
    {
        gameObject.GetComponent<Rigidbody>().AddForce(collision.contacts[0].normal * 5.0f, ForceMode.Impulse);
    }
}
