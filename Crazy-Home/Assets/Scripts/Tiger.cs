using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using DG.Tweening;

public class Tiger : Gimmick
{
    [SerializeField] GameObject tiger;

    NavMeshAgent agent;
    Animator animator;
    Collider hitBox;

    public override event Action<ObjectType> OnAnimeStart;
    public override event Action<ObjectType> OnAnimeEnd;

    protected override void Start()
    {
        base.Start();
        agent = tiger.GetComponent<NavMeshAgent>();
        animator = tiger.GetComponent<Animator>();
        hitBox = tiger.GetComponent<Collider>();
        agent.isStopped = true;
        playerAnimation = "Yell";
        enemyAnimation = "Fall";
    }

    public override void ActiveTrap()
    {
        OnAnimeStart?.Invoke(ObjectType.Gimmick);
        cameraManager.FocusObject(tiger.transform);
        agent.isStopped = false;
        animator.Play("run");
        agent.destination = enemy.transform.position + enemy.transform.forward;
    }

    private void Update()
    {
        if (!agent.isStopped && !agent.pathPending && agent.remainingDistance < 1.0f)
        {
            agent.isStopped = true;
            hitBox.enabled = true;
            animator.Play("sound");
            Invoke("Destroy", 2.0f);
            OnAnimeEnd?.Invoke(ObjectType.Gimmick);
        }
    }

    private void Destroy()
    {
        Dispose();
        hitBox.enabled = false;
        agent.isStopped = false;
        animator.Play("run");
        agent.destination = new Vector3(1.5f, 0.1f, -9.0f);
        Destroy(gameObject);
    }
}
