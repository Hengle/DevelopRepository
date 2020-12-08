using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Sea : MonoBehaviour
{
    [SerializeField] float y;
    [SerializeField] float time;

    private void Awake()
    {
        transform.DOMoveY(transform.position.y + y,time).SetLoops(-1,LoopType.Yoyo).SetEase(Ease.InOutSine);
    }
}
