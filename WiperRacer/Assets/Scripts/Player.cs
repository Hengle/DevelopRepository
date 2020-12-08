using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    Rigidbody myRigidBody;

    Vector3 moveDir;

    void Awake()
    {
        myRigidBody = GetComponent<Rigidbody>();
    }

    void Update()
    {
        moveDir = transform.forward;
        if (Input.GetMouseButton(0))
        {
            moveDir += transform.right * 5.0f;
        }
    }

    void FixedUpdate()
    {
        myRigidBody.AddForce(-transform.up * 98f);
        myRigidBody.velocity = new Vector3(moveDir.x, myRigidBody.velocity.y, moveDir.z * 10f);
    }
}
