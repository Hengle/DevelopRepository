using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public bool isPlayerBullet;

    private void Awake()
    {
        isPlayerBullet = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        var breakable = other.transform.GetComponent<IBreakable>();
        if (breakable != null)
        {
            breakable.Break();
        }

        if (!isPlayerBullet && other.gameObject.tag == "Player")
        {
            var c = other.transform.GetComponent<Character>();
            c.DeathRagdoll((c.transform.position - transform.position).normalized * 15f);
        }
    }
}
