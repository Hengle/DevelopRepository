using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class AutoRotate : MonoBehaviour
{
    [SerializeField] float power = 5f;
    [SerializeField] bool isReverse;
    [SerializeField] bool isAutoStop;

    public event Action OnRotateCompleate;

    private void Awake()
    {
        if (isReverse)
        {
            power *= -1.0f;
        }
        var y = isReverse ? 135f : 135f * -1f;

        if (isAutoStop)
        {
            transform.DORotate(new Vector3(0f, transform.localEulerAngles.y + y, 0f), 1.0f, RotateMode.Fast).SetDelay(5f).OnComplete(() => OnRotateCompleate?.Invoke());
        }
    }

    private void Update()
    {
        if (!isAutoStop)
        {
            transform.Rotate(0f, power, 0f);
        }
    }
}
