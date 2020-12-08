using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Util;
using TMPro;
using DG.Tweening;

public abstract class Enemy : MonoBehaviour, IPushable
{
    [SerializeField] RagdollUtility ragdollUtility;
    [SerializeField] float force = 100.0f;
    [SerializeField] TextMeshPro faceIcon;
    //[SerializeField] ParticleSystem smorke;

    Rigidbody myRigidbody;
    Animator animator;
    Collider myCollider;
    bool isDeath;

    protected virtual void Awake()
    {
        myRigidbody = transform.GetComponent<Rigidbody>();
        animator = transform.GetComponent<Animator>();
        myCollider = transform.GetComponent<CapsuleCollider>();
        ragdollUtility.Setup();
        ragdollUtility.Deactivate();
    }

    protected virtual void Death()
    {
        isDeath = true;

        //ラグドール有効化
        ragdollUtility.Activate();

        //ラグドール部分じゃない本体判定用のcollider、rigidbody、animatorを非アクティブに
        myCollider.enabled = false;
        animator.enabled = false;
        myRigidbody.useGravity = false;
        myRigidbody.isKinematic = true;

        DeathEffect();
    }

    void DeathEffect()
    {
        //faceIcon.text = "<sprite index=0>";
        //faceIcon.DOFade(0,1.5f);
    }

    public virtual void Dance()
    {
        faceIcon.text = "<sprite index=1>";
        animator.SetTrigger("Dance");
    }

    public bool GetDeath()
    {
        return isDeath;
    }

    public void Push(Vector3 dirextion)
    {
        Death();
        //法線方向に吹き飛ばす
        var nomal = (transform.position - dirextion).normalized;
        nomal = nomal.SetY(0.0f);
        ragdollUtility.AddForce(nomal * force + transform.up * (force / 2), Vector3.zero);
        //smorke.Play();
    }
}
