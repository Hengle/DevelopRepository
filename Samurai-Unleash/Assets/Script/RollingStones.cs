using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RollingStones : MonoBehaviour {
    Rigidbody2D myRB;

    // Start is called before the first frame update
    void Start() {
        myRB = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update() {
        myRB.velocity *= 0.98f;
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        var target = collision.gameObject;

        if (target.tag == "Enemy") {

            if (DataManager.GetPlayerSettingVibration()) {
                IOSUtil.PlaySystemSound(1519);
                AndroidUtil.Vibrate(100);
            }
        }
    }
}
