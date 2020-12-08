using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
    private void Awake()
    {
        GetComponent<Rigidbody2D>().angularVelocity = 1;
    }
    private void Update()
    {
        // クリックされたら移動
        if (Input.GetMouseButtonDown(0))
        {
            Move();
        }
        Debug.Log(GetComponent<Rigidbody2D>().angularVelocity);
    }
    public void Move()
    {
        var power = 10;
        var angle = 5.0f;
        var rb = GetComponent<Rigidbody2D>();
        rb.AddForce(transform.up * power, ForceMode2D.Impulse);
        if (transform.localEulerAngles.z > 180.0f)
        {
            angle = angle * -1.0f;
        }
        else
        {
            angle = angle * 1.0f;
        }

        rb.AddTorque(angle, ForceMode2D.Impulse);
    }
}
