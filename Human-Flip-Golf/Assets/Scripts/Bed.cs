using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bed : MonoBehaviour, IDamagable, IBreakable
{
    bool isBreak;
    Collider MyCollider;

    private void Awake()
    {
        MyCollider = GetComponent<Collider>();
    }

    public void Break()
    {
        isBreak = true;
    }

    public void Damage(Character character)
    {
        if (isBreak) return;
        character.DeathRagdoll((character.transform.position - transform.position).normalized * 15f);
    }
}
