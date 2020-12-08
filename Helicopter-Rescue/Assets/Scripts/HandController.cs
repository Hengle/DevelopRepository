using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Util;

public class HandController : MonoBehaviour
{
    [SerializeField] TouchEventHandler eventHandler;
    [SerializeField] CanvasGroup canvasGroup;
    [SerializeField] TargetMarker targetMarker;
    [SerializeField] Animator leftHand;
    [SerializeField] Animator rightHand;

    Vector3 startPos;
    public event Action<bool> OnGrabListener;

    public void Activate()
    {
        eventHandler.OnTouchStartListener += MoveStart;
        eventHandler.OnTouchKeepListener += Move;
        eventHandler.OnTouchReleaseListener += MoveEnd;

        canvasGroup.alpha = 1.0f;
    }


    private void Start()
    {
        targetMarker.SetHand(this);
    }

    public void DeActivate()
    {
        eventHandler.OnTouchStartListener -= MoveStart;
        eventHandler.OnTouchKeepListener -= Move;
        eventHandler.OnTouchReleaseListener -= MoveEnd;

        canvasGroup.alpha = 0f;
    }

    void MoveStart(Vector3 pos)
    {
        pos.z = 0.5f;
        Vector3 worldposition = Camera.main.ScreenToWorldPoint(pos);
        startPos = worldposition;
    }

    void Move(Vector3 pos)
    {
        pos.z = 0.5f;
        Vector3 worldposition = Camera.main.ScreenToWorldPoint(pos);
        transform.position = worldposition;
    }

    void MoveEnd(Vector3 pos)
    {
        pos.z = 0.5f;
        Grab();
    }

    void Grab()
    {
        DeActivate();
        leftHand.SetBool("IsGrabbing", true);
        rightHand.SetBool("IsGrabbing", true);
        StartCoroutine(WaitAnimation("GrabAnimation", () => OnGrabListener?.Invoke(targetMarker.IsOverlap())));
    }

    IEnumerator WaitAnimation(string stateName, Action complete = null)
    {
        yield return new WaitForSeconds(0.1f);
        yield return new WaitWhile(() => leftHand.GetCurrentAnimatorStateInfo(0).fullPathHash == Animator.StringToHash(stateName));

        yield return new WaitForSeconds(0.3f);
        complete?.Invoke();
    }

    public Vector3 GetCursorScreenPosition()
    {
        return Camera.main.WorldToScreenPoint(transform.position);
    }
}
