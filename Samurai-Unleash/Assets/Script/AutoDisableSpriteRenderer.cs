using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoDisableSpriteRenderer : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        if (GetComponent<SpriteRenderer>()) {
            GetComponent<SpriteRenderer>().enabled = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
