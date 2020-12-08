using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using DG.Tweening;

public class EnemyMob : Enemy
{
    [SerializeField] bool isWalk;
    [SerializeField] float speed = 0.01f;

    NavMeshAgent navMeshAgent;
    GameObject target;

    protected override void Awake()
    {
        base.Awake();
        enemyType = EnemyType.Mob;
        navMeshAgent =  transform.GetComponent<NavMeshAgent>();
        target = GameObject.FindGameObjectWithTag("Player");
        
    }

    public override void IInitializ()
    {
        base.IInitializ();
        transform.rotation = Quaternion.LookRotation(target.transform.position - transform.position);
        navMeshAgent.destination = target.transform.position;
    }

    protected override void Death()
    {
        base.Death();
        isWalk = false;
        navMeshAgent.enabled = false;
    }

    public override void Appearance()
    {
        base.Appearance();
        navMeshAgent.enabled = false;
        StartCoroutine(WaitAnimation("Base Layer.Appearance", () => {
            if(!GetDeath())
            {
                navMeshAgent.enabled = true;
                navMeshAgent.destination = target.transform.position;
            }
        }));
    }

    public override void Stop()
    {
        base.Stop();
        //isWalk = false;
        //navMeshAgent.isStopped = true;
    }
}
