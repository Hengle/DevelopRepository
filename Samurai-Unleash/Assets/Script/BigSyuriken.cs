using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BigSyuriken : MonoBehaviour {
    Rigidbody2D myrb;                        // 自身のRigidbody2D 参照

    // Start is called before the first frame update
    void Start() {
        myrb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update() {
        myrb.velocity *= 0.98f; // 発射済みなら減速処理
        myrb.angularVelocity *= 0.98f;

        if (Mathf.Abs(myrb.velocity.x) <= 0.4f && Mathf.Abs(myrb.velocity.y) <= 0.4f) {
            myrb.velocity = Vector2.zero;
        }
        if (Mathf.Abs(myrb.angularVelocity) <= 0.4f) {
            myrb.angularVelocity = 0;
        }

        if (myrb.IsSleeping()) {
            myrb.isKinematic = true;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        var target = collision.gameObject;

        if (target.tag == "Enemy") {

            if (DataManager.GetPlayerSettingVibration()) {
                IOSUtil.PlaySystemSound(1519);
                AndroidUtil.Vibrate(100);
            }

            target.GetComponent<Enemy>()?.Damage(1);
        } else if (target.tag == "Switch") {
            target.GetComponent<SwitchPanel>().ChangeStatus();
        } else if (target.tag == "Bomb") {

            target.GetComponent<Bomb>().Explode();
        }
    }


}
