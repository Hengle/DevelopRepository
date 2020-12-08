using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using DigitalRuby.LightningBolt;

public class Lever : Gimmick
{
    [SerializeField] GameObject lever;
    [SerializeField] GameObject bolt;
    //[SerializeField] Spring spring;

    Collider myCollider;
    public override event Action<ObjectType> OnAnimeStart;
    public override event Action<ObjectType> OnAnimeEnd;

    protected override void Start()
    {
        base.Start();
        playerAnimation = "Push";
        enemyAnimation = "Reaction";
        myCollider = GetComponent<Collider>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Enemy")
        {
            OnAnimeStart?.Invoke(ObjectType.Gimmick);
            myCollider.enabled = false;
            cameraManager.FocusEnemy();
            foreach (var p in transform.GetComponentsInChildren<ParticleSystem>())
            {
                p.Play();
            }

            OnAnimeEnd?.Invoke(ObjectType.Gimmick);

            StartCoroutine(DelayInvoke(2.0f, () => {
                foreach (var p in transform.GetComponentsInChildren<ParticleSystem>())
                {
                    p.Stop();
                }
            }));
        }
    }

    public override void ActiveTrap()
    {
        OnAnimeStart?.Invoke(ObjectType.Gimmick);
        var sequence = DOTween.Sequence();
        sequence.Append(lever.transform.DOLocalRotate(new Vector3(45f, 0f, 0f), 1.0f).OnComplete(() =>
        {
            foreach (var s in bolt.transform.GetComponentsInChildren<LightningBoltScript>())
            {
                s.ManualMode = false;
            }
            cameraManager.FocusObject(bolt.transform);
            myCollider.enabled = true;
            OnAnimeEnd?.Invoke(ObjectType.Gimmick);
        }));

        //var sequence = DOTween.Sequence();
        //sequence.Append(lever.transform.DOLocalRotate(new Vector3(45f, 0f, 0f), 1.0f).OnComplete(() => {
        //    spring.Activate();
        //    cameraManager.FocusObject(spring.transform);
        //    OnAnimeEnd?.Invoke(ObjectType.Gimmick);
        //    }));

    }

    IEnumerator DelayInvoke(float time, Action onInvoke)
    {
        yield return new WaitForSeconds(time);
        onInvoke.Invoke();
    }
}
