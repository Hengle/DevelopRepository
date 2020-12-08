using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public enum WeaponType
{
    Axe,
    Spear,
    Sword
}

public abstract class WeaponBase : MonoBehaviour
{
    [SerializeField] protected int power = 1;
    [SerializeField] float cooltime = 1;
    [SerializeField] int speed = 1;
    [SerializeField] TrailRenderer trail;
    [SerializeField] protected ParticleSystem hitEffect;

    protected Sequence mySequence;
    protected Rigidbody myRigidbody;
    protected Collider myCollider;
    protected WeaponType weaponType;

    protected abstract void Initialize();
    protected abstract void OnTriggerExtend(Collider collider);

    void Awake()
    {
        myRigidbody = GetComponent<Rigidbody>();
        myCollider = GetComponent<Collider>();

        myRigidbody.useGravity = false;
        myRigidbody.isKinematic = true;
        myCollider.isTrigger = true;
        myCollider.enabled = false;

        Initialize();
    }

    //軌道は継承先で行う
    public virtual void Flip(Vector3 direction, Quaternion rotation)
    {
        transform.SetParent(null);

        myRigidbody.isKinematic = false;
        myCollider.enabled = true;
        trail.enabled = true;
    }

    void OnTriggerEnter(Collider collider)
    {
        var gameObject = collider.gameObject;

        //var iPushable = gameObject.GetComponent<IPushable>();
        //if (iPushable != null)
        //{
            //iPushable.Push(transform.position);
        //}

        var iReflectable = gameObject.GetComponent<IReflectable>();
        if (iReflectable != null)
        {
            //iReflectable.Reflect(this.gameObject, other);
        }

        OnTriggerExtend(collider);
        trail.enabled = false;
    }

    public WeaponType GetWeaponType()
    {
        return weaponType;
    }

    public float GetCoolTime()
    {
        return cooltime;
    }

    protected void StopMove(Sequence sequence)
    {
        sequence.Kill();
    }

    IEnumerator DelayMethod(float waitTime, Action action)
    {
        yield return new WaitForSeconds(waitTime);

        action?.Invoke();
    }
}
