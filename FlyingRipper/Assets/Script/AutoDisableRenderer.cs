using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoDisableRenderer : MonoBehaviour {

	void Start(){
        if (GetComponent<SpriteRenderer>()) { GetComponent<SpriteRenderer>().enabled = false; }
        if (GetComponent<MeshRenderer>()) { GetComponent<MeshRenderer>().enabled = false; }
    }


}
