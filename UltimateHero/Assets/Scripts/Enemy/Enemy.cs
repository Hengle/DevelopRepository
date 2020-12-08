using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Util;
using TMPro;
using DG.Tweening;

public abstract class Enemy : MonoBehaviour, IPushable, IHittable, IDamagable
{
    public enum DeathType
    {
        Down,
        BlowAway,
        Cut,
    }

    public enum EnemyType
    {
        Mob,
        Boss,
    }

    [SerializeField] RagdollUtility ragdollUtility;
    [SerializeField] float force = 100.0f;
    [SerializeField] TextMeshPro faceIcon;
    [SerializeField] protected int maxHp = 1;
    [SerializeField] GameObject myModel;
    [SerializeField] int coinNum = 1;

    public event Action OnDeathEnemy;
    DeathType deathType;
    protected EnemyType enemyType;

    Rigidbody myRigidbody;
    Collider myCollider;
    Animator myAnimator;
    Coin coin;
    Transform weaponHitParent;
    GameObject head;

    float coinRate;
    bool isDeath;
    protected int hp;

    protected virtual void Awake()
    {
        myRigidbody = transform.GetComponent<Rigidbody>();
        myCollider = transform.GetComponent<CapsuleCollider>();
        myAnimator = Instantiate(myModel, transform).transform.GetComponent<Animator>();
        weaponHitParent = myAnimator.transform.GetComponent<EnemyModelSupport>().GetHitPart();
        head = myAnimator.transform.GetComponent<EnemyModelSupport>().GetHead();
        coin = GameObject.FindGameObjectWithTag("Coin").GetComponent<Coin>();
        ragdollUtility.Setup();
        ragdollUtility.Deactivate();
        hp = maxHp;
    }

    public virtual void IInitializ()
    {
        hp = maxHp;
        ragdollUtility.Deactivate();
    }

    protected virtual void Death()
    {
        isDeath = true;

        switch (deathType)
        {
            case DeathType.Down:
                myCollider.enabled = false;
                myRigidbody.useGravity = false;
                myRigidbody.isKinematic = true;
                PlayAnimation("Death" + UnityEngine.Random.Range(1, 4), false);
                break;

            case DeathType.BlowAway:
                //ラグドール有効化
                ragdollUtility.Activate();
                //ラグドール部分じゃない本体判定用のcollider、rigidbody、animatorを非アクティブに
                myCollider.enabled = false;
                myAnimator.enabled = false;
                myRigidbody.useGravity = false;
                myRigidbody.isKinematic = true;
                break;
            case DeathType.Cut:
                //ラグドール有効化
                ragdollUtility.Activate();
                //ラグドール部分じゃない本体判定用のcollider、rigidbody、animatorを非アクティブに
                myCollider.enabled = false;
                myAnimator.enabled = false;
                myRigidbody.useGravity = false;
                myRigidbody.isKinematic = true;
                Destroy(head.GetComponent<CharacterJoint>());
                head.transform.SetParent(null);
                head.GetComponent<Rigidbody>().AddForce(Vector3.up * 1000);
                break;
        }
        
        OnDeathEnemy?.Invoke();
    }

    protected IEnumerator WaitAnimation(string stateName, Action complete = null)
    {
        yield return new WaitForSeconds(0.1f);
        yield return new WaitWhile(() => myAnimator.GetCurrentAnimatorStateInfo(0).fullPathHash == Animator.StringToHash(stateName));
        complete?.Invoke();
    }

    public virtual void Appearance()
    {
        AudioManager.Instance.PlaySE("zombie_appearance");
        PlayAnimation("Appearance",true);
        myAnimator.speed = 0;
        transform.DOMoveY(0.15f,0.5f).SetEase(Ease.Linear);
        Invoke("AnimationStart",0.4f);
    }

    void AnimationStart()
    {
        myAnimator.speed = 1.0f;
    }

    public void PlayAnimation(String animationName, bool trigger)
    {
        if (trigger)
        {
            myAnimator.SetTrigger(animationName);
        }
        else
        {
            myAnimator.Play(animationName);
        }
    }

    public void SetDeathType(DeathType deathType)
    {
        this.deathType = deathType;
    }

    public bool GetDeath()
    {
        return isDeath;
    }

    public EnemyType GetEnemyType()
    {
        return enemyType;
    }

    public Transform GetWeaponHitParent()
    {
        return weaponHitParent;
    }

    public int MaxHP()
    {
        return maxHp;
    }

    public int HP()
    {
        return hp;
    }

    public void Push(Vector3 direction)
    {
        Death();
        //法線方向に吹き飛ばす
        var nomal = (transform.position - direction).normalized;
        nomal = nomal.SetY(0.0f);
        ragdollUtility.AddForce(nomal * force + transform.up * (force / 2), Vector3.zero);
        //smorke.Play();
    }

    public void EnemyChangeCoin()
    {
        gameObject.SetActive(false);
        //コイン量 * (ランダムRange(0.8　～　1.2) + 現在stage数 / 10)
        coinNum = (int)Mathf.Floor(coinNum * (UnityEngine.Random.Range(0.8f, 1.2f) + (DataManager.Instance.NowStage / 10.0f)));
        coin.Add(coinNum, transform.position);
    }

    public void Hit()
    {
        AudioManager.Instance.PlaySE("zombie_bite");
        PlayAnimation("Dance", true);
    }

    public virtual void Stop()
    {
        //PlayAnimation("Dance");
    }

    public bool IsKillable()
    {
        return true;
    }

    public virtual void Damage(int damage)
    {
        hp -= damage;
        if (hp <= 0)
        {
            Death();
        }
    }
}
