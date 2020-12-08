using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Util;

public class PressGauge : MonoBehaviour
{
    [SerializeField] protected SpriteRenderer gaugeRenderer;
    [SerializeField] SpriteRenderer maker;

    MultipleSpriteFader spriteFader;
    TouchEventHandler touchEventHandler;
    Vector3 defaultGaugeScale;
    Vector3 destinationScale;
    Tween scaleTween;
    float[] checkZone = new float[]{0.6f, 1.9f};
    event Action<bool> onAnimationEndListener;

    void Awake()
    {
        defaultGaugeScale = gaugeRenderer.transform.localScale;
        destinationScale = gaugeRenderer.transform.localScale.SetX(0);
        gaugeRenderer.transform.localScale = destinationScale;
        scaleTween = gaugeRenderer.transform.DOScale(defaultGaugeScale, 2.0f).SetEase(Ease.Linear).SetLoops(-1, LoopType.Yoyo).Pause();

        touchEventHandler = Camera.main.transform.GetComponent<TouchEventHandler>();
        spriteFader = transform.GetComponent<MultipleSpriteFader>();
    }

    void Start()
    {
        spriteFader.FadeTo(0, 0f);
    }

    void OnDestroy()
    {
        scaleTween?.Kill();
        scaleTween = null;
    }

    public void PlayAnimation(Action<bool> onAnimationEnd = null)
    {
        spriteFader.FadeTo(1, 0.5f);
        touchEventHandler.OnTouchStartListener += TouchStart;
        scaleTween.Play();

        onAnimationEndListener = onAnimationEnd;
    }

    void TouchStart(Vector3 position)
    {
        touchEventHandler.OnTouchStartListener -= TouchStart;
        scaleTween.Pause();
        spriteFader.FadeTo(0, 0.5f);

        // 判定ゲージ範囲内かチェック
        bool isSuccess = gaugeRenderer.bounds.max.x >= checkZone[0] && gaugeRenderer.bounds.max.x <= checkZone[1];
        onAnimationEndListener?.Invoke(isSuccess);
    }

}
