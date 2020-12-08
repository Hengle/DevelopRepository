using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosiveBarrelEffectiveArea : MonoBehaviour {

    [SerializeField] List<GameObject> myArea = new List<GameObject>();

    private void OnTriggerEnter(Collider other) {
        if(other.gameObject.tag == "Enemy") {
            myArea.Add(other.gameObject);
        }
    }

    private void OnTriggerExit(Collider other) {
        if (other.gameObject.tag == "Enemy") {
            myArea.Remove(other.gameObject);
        }
    }

    public List<GameObject> GetEffectiveObject() {
        return myArea;
    }
}
