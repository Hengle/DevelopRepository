using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Util;
using DG.Tweening;

public class Katana : MonoBehaviour
{
    [SerializeField] Transform linePosition;
    [SerializeField] float speed = 10.0f;
    TouchEventHandler touchEventHandler;
    SpriteRenderer myRenderer;
    LineRenderer lineRenderer;

    Vector3 touchStart;
    public event Action OnSliceListener;

    private void Awake()
    {
        myRenderer = GetComponent<SpriteRenderer>();
        lineRenderer = GetComponent<LineRenderer>();
        touchEventHandler = Camera.main.GetComponent<TouchEventHandler>();
        touchEventHandler.OnTouchStartListener += TouchStart;
        touchEventHandler.OnTouchKeepListener += TouchKeep;
        touchEventHandler.OnTouchReleaseListener += TouchRelease;

        myRenderer.enabled = false;
        lineRenderer.positionCount = 2;
    }

    void TouchStart(Vector3 position)
    {
        touchStart = position;
        myRenderer.enabled = true;
        lineRenderer.enabled = true;
        var distance = Vector3.Distance(transform.position.SetZ(10.0f), Camera.main.transform.position);
        position = position.SetZ(distance);
        transform.position = Camera.main.ScreenToWorldPoint(position);
        lineRenderer.SetPosition(0, transform.position);
    }

    void TouchKeep(Vector3 position)
    {
        float dx = position.x - touchStart.x;
        float dy = position.y - touchStart.y;
        float rad = Mathf.Atan2(dy, dx);

        var distance = Vector3.Distance(transform.position.SetZ(10.0f), Camera.main.transform.position);
        position = position.SetZ(distance);
        lineRenderer.SetPosition(1, Camera.main.ScreenToWorldPoint(position));
        transform.eulerAngles = new Vector3(0f,0f,rad * Mathf.Rad2Deg + 180f);
    }

    void TouchRelease(Vector3 position)
    {
        myRenderer.transform.DOMove(lineRenderer.GetPosition(1), Vector3.Distance(lineRenderer.GetPosition(0), lineRenderer.GetPosition(1)) / speed).SetEase(Ease.Linear).OnComplete(() => Invoke("MoveComplete",0.5f));

        lineRenderer.SetPosition(0,Vector3.zero);
        lineRenderer.SetPosition(1, Vector3.zero);
        lineRenderer.enabled = false;
    }

    void MoveComplete()
    {
        myRenderer.enabled = false;
        OnSliceListener?.Invoke();
    }
}
