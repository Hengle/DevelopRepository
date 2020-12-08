using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class BaymaxDetectionArea : MonoBehaviour {

    [SerializeField] bool isEnter;
    public event Action<GameObject> FoundSaw;
    public event Action GoOutSaw;

    private void OnTriggerEnter(Collider other) {
        if(other.gameObject.tag == "Player") {
            FoundSaw?.Invoke(other.gameObject);
        }
    }

    private void OnTriggerExit(Collider other) {
        if (other.gameObject.tag == "Player") {
            GoOutSaw?.Invoke();
        }
    }

}
