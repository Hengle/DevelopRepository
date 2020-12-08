using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BossEnemy : Enemy
{
    [SerializeField] bool isWalk;
    [SerializeField] float speed = 0.01f;
    [SerializeField] float roarCoolTime = 5.0f;

    NavMeshAgent navMeshAgent;
    GameObject target;
    SimpleHealthBar hpGauge;

    protected override void Awake()
    {
        base.Awake();
        enemyType = EnemyType.Boss;
        navMeshAgent = transform.GetComponent<NavMeshAgent>();
        hpGauge = GameObject.FindGameObjectWithTag("BossGauge").GetComponent<SimpleHealthBar>();
        target = GameObject.FindGameObjectWithTag("Player");
        hpGauge.UpdateBar((float)hp, maxHp);
        transform.rotation = Quaternion.LookRotation(target.transform.position - transform.position);
    }

    private void Start()
    {
        if (isWalk)
        {
            navMeshAgent.destination = target.transform.position;
        }
    }

    IEnumerator Roar()
    {
        while (!GetDeath())
        {
            AudioManager.Instance.PlaySE("zombie_roar");
            PlayAnimation("Roar", true);
            navMeshAgent.isStopped = true;
            yield return StartCoroutine(WaitAnimation("Base Layer.Roar"));
            navMeshAgent.isStopped = false;
            yield return new WaitForSeconds(roarCoolTime);
        }
    }

    protected override void Death()
    {
        base.Death();
        isWalk = false;
        navMeshAgent.isStopped = true;
        navMeshAgent.enabled = false;
    }

    public override void Appearance()
    {
        base.Appearance();
        navMeshAgent.isStopped = true;
        StartCoroutine(WaitAnimation("Base Layer.Appearance", () => {
            if (navMeshAgent.enabled)
            {
                navMeshAgent.isStopped = false;
                StartCoroutine(Roar());
            }
        }));
    }

    public override void Stop()
    {
        base.Stop();
        //isWalk = false;
        //navMeshAgent.isStopped = true;
    }

    public override void Damage(int damage)
    {
        base.Damage(damage);
        hpGauge.UpdateBar((float)hp,maxHp);
    }
}
