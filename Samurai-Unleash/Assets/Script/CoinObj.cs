using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class CoinObj : MonoBehaviour
{
    [SerializeField] AnimationCurve ease;

    SpriteRenderer spriteRenderer;

    void Start() {
        transform.localScale = transform.localScale * Random.Range(0.75f, 1.0f);
        spriteRenderer = GetComponent<SpriteRenderer>();
        PlayAnimation();

        spriteRenderer.DOFade(1, 0);
    }

    public void Dispose() {
        Destroy(gameObject);
    }

    public void PlayAnimation() {
        //seq.Join(transform.DOLocalMoveX(Random.Range(-1f, 1f), 1.0f).SetRelative());

        var seq = DOTween.Sequence();
        //seq.AppendInterval(1);
        seq.Append(spriteRenderer.DOFade(1, 0));
        seq.Append(transform.DOLocalMoveY(Random.Range(1, 2), 2.0f).SetEase(ease).SetRelative());
        //seq.Join(transform.DOScale(Vector3.one*0.5f,0f));
        //seq.Join(transform.DORotate(new Vector3(0,Random.Range(360,2048),0),2f));
        seq.AppendInterval(1);
        seq.Join(spriteRenderer.DOFade(0, 0.5f));
        seq.OnComplete(Dispose);
    }
}
