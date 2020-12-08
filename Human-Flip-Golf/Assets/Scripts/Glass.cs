using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Glass : MonoBehaviour, IDamagable, IBreakable
{
    Collider MyCollider;

    private void Awake()
    {
        MyCollider = GetComponent<Collider>();
    }

    public void Break()
    {
        MyCollider.enabled = false;
        foreach(var rb in GetComponentsInChildren<Rigidbody>())
        {
            rb.useGravity = true;
        }
    }

    public void Damage(Character character)
    {
        character.DeathRagdoll((character.transform.position - transform.position).normalized * 15f);
    }
}
