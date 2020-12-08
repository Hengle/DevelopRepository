using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class PlayerController : MonoBehaviour {

    [SerializeField] GameObject mainCameraObject;
    [SerializeField] float lowerSpeed = 0f; // 最低速度
    [SerializeField] float maxSpeed = 10f; // 最高速度
    [SerializeField] float reduceSpeed = 0.98f; // 減衰量

    Rigidbody myrb;
    public float speed;
    private Animator anim;
    private bool kurikkunow, rotebool;
    public Vector2 startPos, nowPos, differenceDisVector2;
    private float radian, differenceDisFloat;
    public bool isActive, isRotate, isMove, isInputed;
    public float tapTime = 0f;


    public event Action OnTapped;
    public event Action OnHitEnemy;
    public event Action OnDeath;

    void Start() {
        Input.multiTouchEnabled = false;
        myrb = GetComponent<Rigidbody>();
        speed = lowerSpeed;
        anim = GetComponent<Animator>();
    }

    void Update() {
        if (!isActive) { return; }
        MovementControll();
    }
    void FixedUpdate() {
        Movement();
    }

    void MovementControll() {
        if (!isActive) { return; }

        //移動
        if (Input.GetMouseButtonDown(0)) {
            //マウス左クリック時に始点座標を代入
            startPos = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
            isInputed = true;
            OnTapped?.Invoke();
        }

        if (Input.GetMouseButton(0) && isInputed) {
            // ロングタップタイムを記憶していく
            tapTime += Time.deltaTime;

            //押している最中に今の座標を代入
            nowPos = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
            differenceDisVector2 = nowPos - startPos;

            var twoVec = nowPos - startPos;
            twoVec.y /= Screen.height;
            twoVec.x /= Screen.width;

            //スワイプ量によってSpeedを変化させる.この時、絶対値にする。
            differenceDisFloat = differenceDisVector2.x * differenceDisVector2.y;
            differenceDisFloat /= 100;
            differenceDisFloat = Mathf.Abs(differenceDisFloat);

            isMove = true;
            //if (Mathf.Abs(twoVec.x) <= 0.1f && Mathf.Abs(twoVec.y) <= 0.1f) {
            //    //isMove = false;
            //} else {
            //    isMove = true;
            //}

            // ロングタップタイプ
            var playerMaxSpeed = maxSpeed;
            //if(tapTime >= 1) { playerMaxSpeed += tapTime; }

            //if (playerMaxSpeed > maxSpeed * 1.75f) { playerMaxSpeed = maxSpeed * 1.75f; }

            if (differenceDisFloat > playerMaxSpeed) {
                differenceDisFloat = playerMaxSpeed;
            }

            //最低速度
            if (differenceDisFloat < lowerSpeed) {
                differenceDisFloat = lowerSpeed;
            }

            speed = differenceDisFloat;
            //speed = 2;

            //回転する角度計算
            radian = Mathf.Atan2(differenceDisVector2.x, differenceDisVector2.y) * Mathf.Rad2Deg;

            radian += mainCameraObject.transform.rotation.eulerAngles.y;

            if (radian == 0) {
                isRotate = false;
            } else {
                isRotate = true;
            }

        } else {
            speed = 0;
            rotebool = false;
        }

        if (Input.GetMouseButtonUp(0)) {
            isRotate = false;
            isMove = false;
            isInputed = false;
            speed = 0;
            tapTime = 0;
        }
    }

    void Movement() {
        if (!isActive) { return; }
        if (!isMove) { speed = 0; }

        //myrb.velocity = (transform.forward * speed) + new Vector3(0, myrb.velocity.y, 0);

        myrb.AddForce(transform.forward * (speed * 1.5f), ForceMode.Impulse);
        float gravtiyPower = myrb.velocity.y;
        myrb.velocity = Vector3.ClampMagnitude(myrb.velocity,0.1f);
        var hoge = myrb.velocity;
        hoge.y = gravtiyPower;
        myrb.velocity = hoge;


        Debug.Log(myrb.velocity);

        if (isRotate) {
            myrb.transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0, radian, 0), 10);
        }

    }

    public void SwitchActive(bool newVal) {
        isActive = newVal;
    }


    private void OnTriggerEnter(Collider other) {
        if (other.tag == "Sea") {
            OnDeath?.Invoke();
        }
    }

}
