using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Util;
using TMPro;
using DG.Tweening;

public abstract class Character : MonoBehaviour
{
    public enum CharacterType
    {
        Plyer,
        Enemy,
    }

    protected abstract void Initialize();

    Rigidbody MyRigidbody { get; set; }
    Collider MyCollider { get; set; }
    Animator MyAnimator { get; set; }
    RagdollUtility MyRagdollUtility { get; set; }
    public event Action OnDeathListener;

    void Awake()
    {
        MyRigidbody = GetComponent<Rigidbody>();
        MyCollider = GetComponent<CapsuleCollider>();
        MyAnimator = GetComponent<Animator>();
        MyRagdollUtility = GetComponent<RagdollUtility>();

        MyCollider.isTrigger = true;
        MyRigidbody.isKinematic = true;
        MyRigidbody.useGravity = false;
    }

    void Start()
    {
        MyRagdollUtility.Setup();
        MyRagdollUtility.Deactivate();
    }

    protected IEnumerator WaitAnimation(string stateName, Action complete = null)
    {
        yield return new WaitForSeconds(0.1f);
        yield return new WaitWhile(() => MyAnimator.GetCurrentAnimatorStateInfo(0).fullPathHash == Animator.StringToHash(stateName));
        complete?.Invoke();
    }

    public void PlayAnimation(String animationName, bool isTrigger)
    {
        if (isTrigger)
        {
            MyAnimator.SetTrigger(animationName);
        }
        else
        {
            MyAnimator.Play(animationName);
        }
    }

    protected void DeathRagdoll(Vector3 direction)
    {
        MyAnimator.enabled = false;
        MyCollider.enabled = false;
        MyRagdollUtility.Activate();
        MyRagdollUtility.AddForce(direction, direction * 2);

        OnDeathListener?.Invoke();
    }
}
