using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletController : MonoBehaviour {

    [SerializeField] float lowerSpeed = 0.1f; // 最低速度

    Rigidbody myrb;
    Vector2 firstPos, nowPos, twoVec;
    Quaternion myRotation;

    void Start() {
        myrb = GetComponent<Rigidbody>();

    }

    void Update() {
        MovementControll();
    }

    private void FixedUpdate() {

    }

    private void MovementControll() {

        if (Input.GetMouseButtonDown(0)) {
            var _input = Input.mousePosition;
            _input.z = 10;

            firstPos = Camera.main.ScreenToWorldPoint(_input); // マウス位置をワールド座標で取得
            myRotation = gameObject.transform.rotation; // 現在の角度を取得
        } else if (Input.GetMouseButton(0)) {

            var _input = Input.mousePosition;
            _input.z = 10;

            var vecA = firstPos - (Vector2)gameObject.transform.position; //ある地点からのベクトルを求めるときはこう書くんだった
            var vecB = Camera.main.ScreenToWorldPoint(_input) - gameObject.transform.position; // 上に同じく

            Debug.DrawLine(gameObject.transform.position, vecA, Color.red);
            Debug.DrawLine(gameObject.transform.position, vecB, Color.red);

            var angle = Vector2.Angle(vecA, vecB); // vecAとvecBが成す角度を求める
            var AxB = Vector3.Cross(vecA, vecB); // vecAとvecBの外積を求める
            Debug.Log(angle);
            // 外積の z 成分の正負で回転方向を決める
            if (AxB.z > 0) {
                transform.localRotation = myRotation * Quaternion.Euler(0, angle,0); // 初期値との掛け算で相対的に回転させる
            } else {
                transform.localRotation = myRotation * Quaternion.Euler(0, -angle, 0 ); // 初期値との掛け算で相対的に回転させる
            }
        }
    }
}
