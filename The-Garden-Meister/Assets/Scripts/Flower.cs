using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Flower : MonoBehaviour
{
    [SerializeField] GameObject flower;

    Collider myCollider;
    public event Action OnBloomedListener;

    private void Awake()
    {
        myCollider = GetComponent<Collider>();
    }

    private void OnParticleCollision(GameObject collision)
    {
        myCollider.enabled = false;
        flower.transform.DOScale(0.9f, 1.0f)
            .SetDelay(1.0f)
            .OnComplete(() => OnBloomedListener?.Invoke());
    }
}
