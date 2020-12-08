using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class ScaleAnimation : MonoBehaviour
{
    [SerializeField] float durationSeconds = 1.0f;
    [SerializeField] float scale = 0.9f;
    [SerializeField] Ease easeType = Ease.InCubic;

    void Awake()
    {
        transform.DOScale(transform.localScale * scale, durationSeconds).SetEase(easeType).SetLoops(-1, LoopType.Yoyo);
    }
}
