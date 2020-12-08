using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponBotu : MonoBehaviour
{
    [SerializeField]
    private Transform shootPoint = null;
    [SerializeField]
    private Transform target = null;
    [SerializeField]
    private GameObject shootObject = null;

    [SerializeField] float speedtime;

    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && target != null)
        {
            //Time.timeScale = 0.1f;
            Shoot(target.position);
        }
    }

    private void Shoot(Vector3 targetPosition)
    {
        ShootFixedTime(targetPosition, speedtime);
    }

    //通過位置と滞空時間を元に力を計算
    private void ShootFixedTime(Vector3 targetPosition, float time)
    {
        float speedVec = ComputeVectorFromTime(targetPosition, time);
        float angle = ComputeAngleFromTime(targetPosition, time);

        if (speedVec <= 0.0f)
        {
            Debug.LogWarning("!!");
            return;
        }

        Vector3 vec = ConvertVectorToVector3(speedVec, angle, targetPosition);
        InstantiateShootObject(vec);
    }

    //u0(速度ベクトル)を求める
    //X、Y方向のベクトル計算で求めた数値を平方根計算
    private float ComputeVectorFromTime(Vector3 targetPosition, float time)
    {
        Vector2 vec = ComputeVectorXYFromTime(targetPosition, time);

        float v_x = vec.x;
        float v_y = vec.y;

        float v0Square = v_x * v_x + v_y * v_y;

        if (v0Square <= 0.0f)
        {
            return 0.0f;
        }

        float v0 = Mathf.Sqrt(v0Square);

        return v0;
    }

    //角度を求める
    //角度=tan * -1 * uy/ux
    private float ComputeAngleFromTime(Vector3 targetPosition, float time)
    {
        Vector2 vec = ComputeVectorXYFromTime(targetPosition, time);

        float v_x = vec.x;
        float v_y = vec.y;

        //象限逆正接
        float rad = Mathf.Atan2(v_y, v_x);
        float angle = rad * Mathf.Rad2Deg;

        return angle;
    }

    //X、Y方向のベクトル計算
    //x方向の速さベクトル　＝　ux
    //y方向の速さベクトル　＝　uy
    //ux = x/t
    //uy = y-y0/t + (g*t)/2
    //u0 = √ux² + uy²
    ////(x/t)²+(y-y0/t + (g*t)/2)²
    private Vector2 ComputeVectorXYFromTime(Vector3 targetPosition, float time)
    {
        if (time <= 0.0f)
        {
            return Vector2.zero;
        }


        // xz平面の距離を計算。
        Vector2 startPos = new Vector2(shootPoint.transform.position.x, shootPoint.transform.position.z);
        Vector2 targetPos = new Vector2(targetPosition.x, targetPosition.z);
        float distance = Vector2.Distance(targetPos, startPos);

        float x = distance;
        float g = -Physics.gravity.y;
        float y0 = shootPoint.transform.position.y;
        float y = targetPosition.y;
        float t = time;

        float v_x = x / t;
        float v_y = (y - y0) / t + (g * t) / 2;

        return new Vector2(v_x, v_y);
    }

    private Vector3 ConvertVectorToVector3(float v0, float angle, Vector3 targetPosition)
    {
        Vector3 startPos = shootPoint.transform.position;
        Vector3 targetPos = targetPosition;
        startPos.y = 0.0f;
        targetPos.y = 0.0f;

        Vector3 dir = (targetPos - startPos).normalized;
        Quaternion yawRot = Quaternion.FromToRotation(Vector3.right, dir);
        Vector3 vec = v0 * Vector3.right;

        vec = yawRot * Quaternion.AngleAxis(angle, Vector3.forward) * vec;

        return vec;
    }

    //斧生成して質量と速さでaddforce
    private void InstantiateShootObject(Vector3 shootVector)
    {
        if (shootObject == null)
        {
            throw new System.NullReferenceException("m_shootObject");
        }

        if (shootPoint == null)
        {
            throw new System.NullReferenceException("m_shootPoint");
        }

        var obj = Instantiate<GameObject>(shootObject, shootPoint.position, Quaternion.identity);
        var rigidbody = obj.GetComponent<Rigidbody>();

        // 速さベクトルのままAddForce()を渡してはいけないぞ。力(速さ×重さ)に変換するんだ
        Vector3 force = shootVector * rigidbody.mass;

        rigidbody.AddForce(force, ForceMode.Impulse);
    }

}
