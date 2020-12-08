using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Util;

public class Player : MonoBehaviour
{
    [SerializeField] Weapon weapon;
    [SerializeField] Animator animator;
    [SerializeField] float forceRate = 15.0f;                               //付与力
    [SerializeField] Vector3 targetPositon = new Vector3(0.0f, 2.0f, 0.0f); //バウンド予定地点
    [SerializeField] SimulationLine simulationLine;                         //軌道予測線

    Rigidbody rigidbody;
    Collider collider;
    TouchEventHandler touchEventHandler;                             //TouchEventHandler
    public event Action OnShootListener;
    private Vector3 dragStart = Vector2.zero;                               //ドラッグ開始点
    Vector3 throwPosition;
    Vector3 controllPoint1;
    Vector3 controllPoint2;
    CameraManager cameraManager;

    float time;

    private void Awake()
    {
        rigidbody = transform.GetComponent<Rigidbody>();
        collider = transform.GetComponent<CapsuleCollider>();
        touchEventHandler = Camera.main.transform.GetComponent<TouchEventHandler>();
        cameraManager = Camera.main.transform.GetComponent<CameraManager>();
    }

    // ドラック開始
    public void TouchStart(Vector3 positon)
    {
        throwPosition = transform.position;
        simulationLine.SetActive(true);
        dragStart = positon;
        cameraManager.SwitchTopView();
    }

    // ドラッグ中
    public void TouchKeep(Vector3 positon)
    {
        var distance = positon - dragStart;
        distance.x = distance.x / Screen.width * forceRate;
        CalcMoveControllPoint(distance, controllPoint1, controllPoint2);
    }

    // ドラッグ終了
    public void TouchRelease(Vector3 positon)
    {
        //操作ストップ
        touchEventHandler.OnTouchStartListener -= TouchStart;
        touchEventHandler.OnTouchKeepListener -= TouchKeep;
        touchEventHandler.OnTouchReleaseListener -= TouchRelease;

        cameraManager.ResetGameTime();
        simulationLine.SetActive(false);
        Shoot();
    }

    // シュート
    public void Shoot()
    {
        animator.SetTrigger("Throw");
    }

    void Flip()
    {
        if (DataManager.Instance.Sound)
        {
            //AudioManager.Instance.PlaySE("button_default1");
        }

        if (DataManager.Instance.Vibration)
        {
            VibrationUtil.VibrationAndroid(300);
        }
        OnShootListener?.Invoke();
        weapon.Flip();
    }

    //新しい斧のセット
    public void SetWeapon(Weapon weapon)
    {
        this.weapon = weapon;
        throwPosition = transform.position;
    }

    //ターゲットの設定
    public void SetTarget(Vector3 target)
    {
        targetPositon = targetPositon.SetX(target.x);
        targetPositon = targetPositon.SetY(transform.position.y + 1.0f);
        targetPositon = targetPositon.SetZ(target.z);
        controllPoint1 = Vector3.Lerp(weapon.transform.position, targetPositon, 0.15f);
        controllPoint2 = Vector3.Lerp(weapon.transform.position, targetPositon, 0.85f);
    }

    void CalcMoveControllPoint(Vector2 move, Vector3 controll1, Vector3 controll2)
    {
        float a = (targetPositon.z - throwPosition.z) / (targetPositon.x - throwPosition.x);
        float aa = -1 / a;
        float b1 = aa * controll1.x - controll1.z;
        float b2 = aa * controll2.x - controll2.z;
        controll1 = controll1.SetX(controllPoint1.x + move.x);
        controll1 = controll1.SetZ(aa * controll1.x - b1);
        controll2 = controll2.SetX(controllPoint2.x + move.x);
        controll2 = controll2.SetZ(aa * controll2.x - b2);

        Vector3[] positions = new Vector3[15];

        for (int i = 0; i < positions.Length; i++)
        {
            positions[i] = VectorUtility.CalcBezier(throwPosition, targetPositon, controll1, controll2, (float)i / (float)positions.Length);
        }
        simulationLine.DrawLine(positions);

        weapon.SetTarget(transform.position, targetPositon, controll1, controll2);
    }

    public void Activate()
    {
        //タッチイベント登録
        touchEventHandler.OnTouchStartListener += TouchStart;
        touchEventHandler.OnTouchKeepListener += TouchKeep;
        touchEventHandler.OnTouchReleaseListener += TouchRelease;
    }

    public void Deactivate()
    {
        //タッチイベント登録
        touchEventHandler.OnTouchStartListener -= TouchStart;
        touchEventHandler.OnTouchKeepListener -= TouchKeep;
        touchEventHandler.OnTouchReleaseListener -= TouchRelease;
    }

    //物理挙動終了
    //sinカーブ
    //2πかけて横半分移動すると元の位置にもどる
    //var hz = 1.0f; //周波数
    //distance.x = Mathf.Sin(distance.x / Screen.width * Mathf.PI * 2 * hz ) * forceRate;
    //distance.y = Mathf.Sin(distance.y / Screen.height * Mathf.PI * 2 * hz) * forceRate;

    //var position = positon;
    //var distance = position - dragStart;

    //distance.x = distance.x / Screen.width * forceRate;
    //distance.y = distance.y / Screen.height * forceRate;
    //if (isCurveReady)
    //{
    //curveForce = distance.x * 0.3f;
    //curveForce = Mathf.Min(MaxCurveForce, Mathf.Max(MaxCurveForce * -1.0f, curveForce)) * ballRigidbody.mass;
    //ball.SetCurve(curveForce);
    //}

    //distance.x = Mathf.Min(4.5f, Mathf.Max(-4.5f, distance.x));
    //currentForce = new Vector3(targetPositon.x, targetPositon.y, targetPositon.z + distance.x) * ballRigidbody.mass;


    //Vector3[] positions = new Vector3[10];

    //for (int i = 0; i < positions.Length; i++)
    //{
    //放物線運動の公式に乗っ取り0.1秒毎の位置を予測
    //Vector3 force = (new Vector3(currentForce.x, currentForce.y, currentForce.z - (curveForce * 0.4f * i)) / ballRigidbody.mass);
    //var t = (i * 0.1f);
    //var g = Physics.gravity.y * -1.0f;
    //var x = t * force.x;                                    //v0cosθ
    //var y = (force.y * t) - 0.5f * g * Mathf.Pow(t, 2.0f);  //−0.5gt2 + y0 + v0tsinθ
    //var z = t * force.z;

    //positions[i] = ball.transform.position + new Vector3(x, y, z);
    //}

    //simulationLine.DrawLine(positions);

    //direction.SetPosition(0, currentPosition);
    //direction.SetPosition(1, currentPosition + currentForce);
    //StartCoroutine(Simulation());
}
