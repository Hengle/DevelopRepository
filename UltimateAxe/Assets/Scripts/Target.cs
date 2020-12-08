using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Target : MonoBehaviour
{
    GameObject body;
    GameObject head;
    bool isHit;

    public event Action OnHitListener;

    private void Awake()
    {
        //foreach (Transform t in transform)
        //{
        //    if (t.tag == "Body")
        //    {
        //        body = t.gameObject;
        //    }

        //    if (t.tag == "Head")
        //    {
        //        head = t.gameObject;
        //    }
        //}
        //head.GetComponent<Head>().OnPushListener += Hit;
    }

    public void Hit()
    {
        isHit = true;
        head.transform.parent = null;

        foreach (Transform t in transform)
        {
            if (t.tag == "Head")
            {
                head = t.gameObject;
            }
        }

        head.transform.localPosition = Vector3.zero;
        head.transform.DORotate(new Vector3(0.0f, 180.0f, 0.0f), 0.5f);
        Dance();
        OnHitListener?.Invoke();
    }

    public bool HitCheck()
    {
        return isHit;
    }

    public void Dance()
    {
        var headAnimator = head.GetComponent<Animator>();
        headAnimator.enabled = true;
        headAnimator.SetTrigger("Dance");
        var bodyAnimator = body.GetComponent<Animator>();
        bodyAnimator.enabled = true;
        bodyAnimator.SetTrigger("Dance");
    }

}
