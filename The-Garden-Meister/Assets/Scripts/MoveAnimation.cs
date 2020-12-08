using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class MoveAnimation : MonoBehaviour
{
    private void Awake()
    {
        transform.DOLocalMoveY(transform.localPosition.y + 0.1f,0.05f).SetEase(Ease.Linear).SetLoops(-1,LoopType.Yoyo);
    }
}
