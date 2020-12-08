using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class BlinkAnimation : MonoBehaviour
{
    [SerializeField] float durationSeconds = 1.0f;
    [SerializeField] Ease easeType = Ease.InCubic;

    void Awake()
    {
        transform.GetComponent<Image>().DOFade(0.0f, durationSeconds).SetEase(easeType).SetLoops(-1, LoopType.Yoyo);
    }
}
