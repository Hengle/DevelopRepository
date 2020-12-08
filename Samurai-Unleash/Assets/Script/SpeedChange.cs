using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedChange : MonoBehaviour {
    [SerializeField] float power;
    SpriteRenderer myRend;
    BoxCollider2D myCollider;

    private void Start() {

        myRend = GetComponent<SpriteRenderer>();
        myCollider = GetComponent<BoxCollider2D>();
    }

    public float GetSpeedChangeVal() {
        return power;
    }

    public void ChangeEnable(bool flag) {
        if (flag) {
            Color32 myColor = myRend.color;
            myColor.a = 255;
            myRend.color = myColor;
            myCollider.enabled = true;
        } else {
            Color32 myColor = myRend.color;
            myColor.a = 100;
            myRend.color = myColor;
            myCollider.enabled = false;

        }
    }
}
