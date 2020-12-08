using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Box : MonoBehaviour {

    List<GameObject> myChild = new List<GameObject>();
    bool isUsed;

	void Start(){
        // 子オブジェクトを全て取得する
        foreach (Transform childTransform in gameObject.transform) {
            myChild.Add(childTransform.gameObject);
        }
    }

    private void Dispose() {

        foreach (GameObject obj in myChild) {
            obj.GetComponent<Rigidbody>().isKinematic = false;

            var twoVec = obj.transform.position - gameObject.transform.position;
            Debug.Log(twoVec);
            obj.GetComponent<Rigidbody>().AddForce(twoVec * 25f, ForceMode.Impulse);
        }
    }

    private void OnCollisionEnter(Collision collision) {
        if (isUsed) { return; }

        if(collision.gameObject.tag == "Player") {
            isUsed = true;
            GetComponent<BoxCollider>().enabled = false;
            Dispose();
            
        }
    }

}
