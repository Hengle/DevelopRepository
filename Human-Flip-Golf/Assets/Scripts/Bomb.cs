using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Bomb : Gimmick, IDamagable
{
    [SerializeField] ParticleSystem standbyEffect;

    protected override void Start()
    {
        base.Start();

        standbyEffect.Play();
        transform.DOScale(1.25f,1f).SetLoops(-1,LoopType.Yoyo);
    }

    public override void ActiveTrap()
    {
        Instantiate(collisionEffect,transform.position, collisionEffect.transform.rotation);
        gameObject.SetActive(false);
    }

    public void Damage(Character character)
    {
        character.DeathRagdoll((character.transform.position - transform.position).normalized * 15f);
        Instantiate(collisionEffect, transform.position, collisionEffect.transform.rotation);
        gameObject.SetActive(false);
    }

    private void OnCollisionEnter(Collision collision)
    {
        Instantiate(collisionEffect, transform.position, collisionEffect.transform.rotation);
        gameObject.SetActive(false);
        //Debug.Log(collision.gameObject.transform.position);
    }
}
