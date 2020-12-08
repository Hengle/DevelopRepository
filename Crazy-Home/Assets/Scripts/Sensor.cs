using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Sensor : Gimmick
{
    [SerializeField] LineRenderer lineRenderer;

    Collider myCollider;
    public override event Action<ObjectType> OnAnimeStart;
    public override event Action<ObjectType> OnAnimeEnd;

    protected override void Start()
    {
        base.Start();
        myCollider = GetComponent<Collider>();
        playerAnimation = "";
        enemyAnimation = "Reaction";
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Enemy")
        {
            OnAnimeStart?.Invoke(ObjectType.Gimmick);
            myCollider.enabled = false;
            lineRenderer.enabled = false;
            cameraManager.FocusEnemy();
            OnAnimeEnd?.Invoke(ObjectType.Gimmick);
            foreach (var p in transform.GetComponentsInChildren<ParticleSystem>())
            {
                p.Play();
            }
        }
    }

    public override void ActiveTrap()
    {
        Standby();
    }

    void Standby()
    {
        OnAnimeStart?.Invoke(ObjectType.Gimmick);
        myCollider.enabled = true;
        cameraManager.FocusObject(transform);
        var pos = lineRenderer.GetPosition(1);
        DOTween.To(
            () => 0f,
            (x) => lineRenderer.SetPosition(1, new Vector3(x, pos.y, pos.z)),
            6.0f,
            2.0f)
            .OnComplete(() => {
                OnAnimeEnd(ObjectType.Gimmick);
                cameraManager.FocusPlayer();
                });
    }
}
