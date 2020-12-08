using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoRotate : MonoBehaviour
{
    [SerializeField] float power;

    private void Update()
    {
        var rigidbody = GetComponent<Rigidbody>();
        rigidbody.AddTorque(new Vector3(0f, 0f, power),ForceMode.Force);
    }
}
