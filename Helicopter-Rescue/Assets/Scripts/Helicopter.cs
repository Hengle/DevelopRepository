using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Util;
using TMPro;
using DG.Tweening;
using LionStudios;

public class Helicopter : MonoBehaviour
{
    Rigidbody MyRigidbody { get; set; }
    Collider MyCollider { get; set; }
    Animator MyAnimator { get; set; }

    Vector3 startPos;
    bool isDeath, isMove;
    public event Action OnDeathListener;

    [SerializeField] TouchEventHandler eventHandler;
    [SerializeField] Rope rope;
    [SerializeField] float speed;
    [SerializeField] ParticleSystem collisionEffect;

    void Awake()
    {
        MyRigidbody = GetComponent<Rigidbody>();
        MyCollider = GetComponent<Collider>();
        MyAnimator = GetComponent<Animator>();
    }

    void Start()
    {
        rope.OnCutListener += Death;
        rope.ResistEvent();
    }

    private void Update()
    {
        if (!isMove) return;
        if (isDeath) return;
        MyRigidbody.AddForce(new Vector3(0, 0, speed), ForceMode.Force);
        transform.localRotation = Quaternion.LookRotation(MyRigidbody.velocity);

        MyRigidbody.velocity *= 0.9f;
    }

    void MoveStart(Vector3 pos)
    {
        pos.z = -10f;
        Vector3 worldposition = Camera.main.ScreenToWorldPoint(pos);
        startPos = worldposition;
    }

    void Move(Vector3 pos)
    {
        pos.z = -10f;
        Vector3 worldposition = Camera.main.ScreenToWorldPoint(pos);
        var distance = worldposition - startPos;
        distance = distance.normalized * speed;
        MyRigidbody.AddForce(new Vector3(distance.x, distance.y, 0), ForceMode.Force);
    }

    void MoveEnd(Vector3 pos)
    {
        pos.z = -10f;
        Vector3 worldposition = Camera.main.ScreenToWorldPoint(pos);
        startPos = Vector3.zero;
    }

    public void SetMoveFlg(bool flg)
    {
        isMove = flg;
    }

    public void Stop()
    {
        isMove = false;
        MyRigidbody.velocity = Vector3.zero;
    }

    public void Activate()
    {
        eventHandler.OnTouchStartListener += MoveStart;
        eventHandler.OnTouchKeepListener += Move;
        eventHandler.OnTouchReleaseListener += MoveEnd;

        speed = 30f;
    }

    void Death()
    {
        isDeath = true;
        OnDeathListener?.Invoke();
        eventHandler.OnTouchStartListener -= MoveStart;
        eventHandler.OnTouchKeepListener -= Move;
        eventHandler.OnTouchReleaseListener -= MoveEnd;
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

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Goal") return;
        if (isDeath) return;
        Dictionary<string, object> eventParams = new Dictionary<string, object>();
        eventParams["FailedType"] = "Explosion";
        Analytics.Events.LevelFailed(eventParams);
        Death();
        Instantiate(collisionEffect, transform.position, Quaternion.identity);
        gameObject.SetActive(false);
    }
}
