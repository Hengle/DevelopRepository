using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexController : MonoBehaviour
{
    Color32 color;

    byte r=200;
    byte gb=200;

    bool active;
    bool changed;
    // Start is called before the first frame update
    void Start()
    {
    }
    private void Update()
    {
        if (changed) transform.position = new Vector3(transform.position.x, transform.position.y - 2, transform.position.z);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!active) StartCoroutine("ColorChange");
        active = true;
        other.GetComponent<Collider>().enabled = false;
    }

    IEnumerator ColorChange()
    {
        for(int i =0; i < 10; i++)
        {
            if(r <240)r += 10;
            if(gb >0)gb -= 20;

            color = new Color32(r, gb, gb, 255);
            GetComponent<Renderer>().material.SetColor("_Color", color);
            yield return new WaitForSeconds(0.3f);
            Debug.Log(color);
        }

        changed = true;
        
        //if (r > 250) r = 255;
        //if (gb < 10) gb = 0;
    }
}
