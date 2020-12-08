using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class DollCollider : MonoBehaviour
{
    [SerializeField] int requiredAmount;

    Collider myCollider;
    MeshRenderer myRenderer;
    int bubbleCount;
    public event Action OnHitListener;

    private void Awake()
    {
        myCollider = GetComponent<Collider>();
        myRenderer = GetComponent<MeshRenderer>();
        myCollider.enabled = false;

        foreach (var material in myRenderer.materials)
        {
            //TODO該当のシェーダーかチェック
            material.SetFloat("_DamageThreshold", 0.0f);
        }
    }

    public void Activate()
    {
        myCollider.enabled = true;
        foreach (var material in myRenderer.materials)
        {
            //TODO該当のシェーダーかチェック
            material.SetFloat("_DamageThreshold", 0.5f);
        }
    }

    public void Deactivate()
    {
        myCollider.enabled = false;
        foreach (var material in myRenderer.materials)
        {
            //TODO該当のシェーダーかチェック
            material.SetFloat("_DamageThreshold", 0.0f);
        }
    }

    public void Hit()
    {
        bubbleCount++;
        if (requiredAmount >= bubbleCount)
        {
            OnHitListener?.Invoke();
        }
    }

    public int GetRequiredAmount()
    {
        return requiredAmount;
    }
}
