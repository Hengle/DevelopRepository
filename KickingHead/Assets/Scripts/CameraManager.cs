using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class CameraManager : MonoBehaviour
{
    GameObject target;
    Vector3 distance;
    bool isFocusTarget;

    private void Awake()
    {
        
    }

    public void SwitchFollowTarget(GameObject target)
    {
        this.target = target;
        distance = transform.position - this.target.transform.position;
    }

    public void FocusTarget()
    {
        isFocusTarget = true;
        Sequence seq = DOTween.Sequence();
        seq.Append(transform.DOMove(new Vector3(target.transform.position.x, 2.0f, target.transform.position.z - 4.0f), 1.0f));
        seq.Join(transform.DORotate(new Vector3(25.0f, 0.0f, 0.0f), 1.0f));
        seq.Play();
    }

    void Update()
    {
        if (isFocusTarget) return;
        transform.position = target.transform.position + distance;
    }
}
