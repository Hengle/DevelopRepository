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

    public void SwitchTopView()
    {
        float viewPoint = Camera.main.fieldOfView;
        //Sequence seq = DOTween.Sequence();
        //seq.Append(transform.DOMove(new Vector3(transform.position.x, transform.position.y + 10.0f, transform.position.z), 1.0f));
        //seq.Join(transform.DORotate(new Vector3(45.0f, 0.0f, 0.0f), 1.0f));
        //seq.Join(DOTween.To(() => viewPoint, time => viewPoint = time, 45.0f, 1.0f).OnUpdate(() => Camera.main.fieldOfView = viewPoint));
        //seq.Play();
        //Time.timeScale = 0.5f;
    }

    public void ResetGameTime()
    {
        Time.timeScale = 1.0f;
    }

    void Update()
    {
        if (isFocusTarget) return;
        //transform.position = target.transform.position + distance;
    }
}
