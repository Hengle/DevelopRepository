using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Util;
using DG.Tweening;
using UnityEngine.UI;

public class Weapon : MonoBehaviour
{
    [SerializeField] Transform throwPoint = null;
    [SerializeField] Transform target = null;
    [SerializeField] LineRenderer line;
    [SerializeField] GameObject weapon = null;
    [SerializeField] AnimationCurve curve;
    [SerializeField] int speedRate;
    [SerializeField] Text text;

    bool isThrow;
    bool isMove;
    Tweener tweener;
    Vector3 controllPoint;
    public event Action OnHitTargetListener;
    Quaternion plevRotation;
    float angle;
    int count;

    void Awake()
    {
        controllPoint = Vector3.Lerp(throwPoint.position, target.position, 0.5f);
        controllPoint = controllPoint.SetY(controllPoint.y + 3.0f * speedRate);
        weapon.transform.position = throwPoint.position;
        plevRotation = weapon.transform.rotation;
        count = 0;
    }

    void Update()
    {

        if (Input.GetMouseButtonDown(0) && !isThrow)
        {
            //Throw(target.position);
        }
        
        if (Input.GetMouseButton(0) && isThrow && isMove)
        {
            Rotate();
        }
    }

    private void Throw(Vector3 targetPosition)
    {
        isThrow = true;
        isMove = true;
        CalcMoveControllPoint(targetPosition);
    }

    private void Rotate()
    {
        weapon.transform.Rotate(new Vector3(0.0f, 5.0f * speedRate, 0.0f));
        angle += Quaternion.Angle(weapon.transform.rotation, plevRotation);
        plevRotation = weapon.transform.rotation;
        Debug.Log(angle);
        if (angle >= 360.0f)
        {
            angle -= 360.0f;
            count++;
            text.text = count.ToString();
        }
    }

    void CalcMoveControllPoint(Vector3 targetPosition)
    {
        Vector3[] positions = new Vector3[5];

        positions[0] = VectorUtility.CalcBezier(throwPoint.position, targetPosition, controllPoint, (float)0 / (float)positions.Length);
        for (int i = 1; i < positions.Length; i++)
        {
            positions[i] = VectorUtility.CalcBezier(throwPoint.position, targetPosition, controllPoint, (float)(i + 1) / (float)positions.Length);
        }
        
        tweener = weapon.transform.DOPath(positions, 5.0f, PathType.CatmullRom).SetEase(curve);
        line.positionCount = positions.Length;
        line.SetPositions(positions);
    }

    private void OnCollisionEnter(Collision collision)
    {
        isMove = false;
        tweener.Kill();
        if(Quaternion.Angle(weapon.transform.rotation, Quaternion.Euler(new Vector3(165.0f, 0f, 270.0f))) <= 75.0f && Quaternion.Angle(weapon.transform.rotation, Quaternion.Euler(new Vector3(90.0f, 0f, 270.0f))) <= 75.0f)
        {
            weapon.GetComponent<Rigidbody>().isKinematic = true;
        }
        else
        {
            weapon.GetComponent<Rigidbody>().useGravity = true;
        }
    }
}
