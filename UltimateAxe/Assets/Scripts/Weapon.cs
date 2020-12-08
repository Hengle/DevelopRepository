using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Util;

public class Weapon : MonoBehaviour
{
    [SerializeField] TrailRenderer trail;
    [SerializeField] ParticleSystem hitEffect;
    Rigidbody rigidbody;                    //物理剛体
    Vector3 start, target, p1, p2;          //ボールの起動
    Sequence seq;
    bool isBezierMove;
    public event Action OnMoveEndListener;

    public void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();
    }

    // 弾く
    public void Flip()
    {
        isBezierMove = true;
        rigidbody.isKinematic = false;
        transform.GetComponent<Collider>().enabled = true;
        transform.SetParent(null);
        transform.rotation = Quaternion.Euler(90.0f, 0f, 270.0f);

        trail.enabled = true;
        float t = 0;
        seq.Append(transform.DORotate(new Vector3(90f, (360f * 5) * -1, 270.0f), 1f, RotateMode.FastBeyond360).SetEase(Ease.Linear));
        seq.Join(DOTween.To(() => t, time => t = time, 1.0f, 1.0f).OnUpdate(() => transform.position = VectorUtility.CalcBezier(start, target, p1, p2, t)).SetEase(Ease.Linear).OnComplete(MoveEnd));
    }

    private void OnTriggerEnter(Collider other)
    {
        var gameObject = other.gameObject;
        switch (gameObject.tag)
        {
            case "Target":
                //rigidbody.isKinematic = true;
                //transform.SetParent(gameObject.transform.parent);
                break;
        }

        var iPushable = other.gameObject.GetComponent<IPushable>();
        if (iPushable != null)
        {
            var position = gameObject.transform.position;
            Instantiate(hitEffect).transform.position = gameObject.transform.position.SetY(1.0f);
            iPushable.Push(transform.position);
        }

        var iReflectable = other.gameObject.GetComponent<IReflectable>();
        if (iReflectable != null)
        {
            //iReflectable.Reflect(this.gameObject, other);
        }
    }

    //反射
    void Reflect(Collision collision)
    {
        rigidbody.AddForce(VectorUtility.Reflect(transform.position, collision.contacts[0].normal) * 20, ForceMode.Force);
    }

    //弾道設定
    public void SetTarget(Vector3 start, Vector3 target, Vector3 p1, Vector3 p2)
    {
        this.start = start;
        this.target = target;
        this.p1 = p1;
        this.p2 = p2;
    }

    private void StopBazierMove()
    {
        seq.Kill();
        isBezierMove = false;
        StartCoroutine(DelayMethod(2.0f, MoveEnd));
    }

    void MoveEnd()
    {
        //無理やり位置調整
        transform.position = new Vector3(-0.8f, 1.1f,25.0f);
        transform.rotation = Quaternion.Euler(90.0f, -45.0f, 270.0f);
        OnMoveEndListener?.Invoke();
    }

    IEnumerator DelayMethod(float waitTime, Action action)
    {
        yield return new WaitForSeconds(waitTime);

        action?.Invoke();
    }


    //public void Flip(Vector3 force)
    //{
    //    // 瞬間的に力を加えてはじく
    //    rigidbody.angularVelocity = new Vector3(0.0f, 0.0f, 50.0f);
    //    rigidbody.AddForce(force, ForceMode.Impulse);
    //    if (isCurve)
    //    {
    //        StartCoroutine(Curve());
    //    }
    //}

    ////カーブ
    //private IEnumerator Curve()
    //{
    //    while (isCurve)
    //    {
    //        yield return new WaitForSeconds(0.1f);

    //        if (!isCurve) break;

    //        rigidbody.AddForce(new Vector3(0f, 0f, curveForce * -1.0f), ForceMode.Impulse);
    //    }
    //}

    //public void SetCurve(float curveForce)
    //{
    //    isCurve = true;
    //    this.curveForce = curveForce;
    //}

    ///// 軌跡を予測して描画するコルーチン（物理シミュレーション使用版）
    //private IEnumerator Simulation()
    //{
    //    //direction.enabled = false;

    //    // 自動的な物理運動を停止させる
    //    Physics.autoSimulation = false;

    //    //var points = new List<Vector3> { currentPosition };
    //    //Shoot(currentForce);

    //    // 運動の軌跡をシミュレーションして記録する
    //    for (var i = 0; i < 20; i++)
    //    {
    //        rigidbody.AddForce(new Vector3(0f, 0f, -15f), ForceMode.Force);
    //        //Physics.Simulate(DeltaTime * 2f);
    //        //points.Add(rigidbody.position);
    //    }

    //    // もとの位置に戻す
    //    rigidbody.velocity = Vector3.zero;
    //    //transform.position = currentPosition;

    //    // 予測地点をつないで軌跡を描画
    //    //simulationLine.positionCount = points.Count;
    //    //simulationLine.SetPositions(points.ToArray());

    //    Physics.autoSimulation = true;
    //    //direction.enabled = true;
    //    yield return 0;
    //    //yield return WaitForFixedUpdate;
    //}
}
