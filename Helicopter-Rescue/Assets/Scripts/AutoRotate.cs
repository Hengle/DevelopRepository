using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoRotate : MonoBehaviour
{
    [SerializeField] float xPower;
    [SerializeField] float yPower;
    [SerializeField] float zPower;

    private void Update()
    {
        var rigidbody = GetComponent<Rigidbody>();
        if (rigidbody != null)
        {
            rigidbody.AddTorque(new Vector3(xPower, yPower, zPower), ForceMode.Force);
        }
        else
        {
            transform.Rotate(new Vector3(xPower, yPower, zPower));
        }
    }
}
