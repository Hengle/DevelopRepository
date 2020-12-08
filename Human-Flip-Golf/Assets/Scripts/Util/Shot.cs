using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shot : MonoBehaviour
{
    Rigidbody rigidbody;
    // Start is called before the first frame update
    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        rigidbody.AddForce(new Vector3 (0f,10f,30f), ForceMode.Impulse);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
