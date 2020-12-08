using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] Bullet bullet;
    Animator myAnimator;

    private void Awake()
    {
        myAnimator = GetComponent<Animator>();
    }

    public void Shoot()
    {
        myAnimator.SetTrigger("Shoot");
    }

    void Fire()
    {
        myAnimator.SetTrigger("Shoot");
        bullet.Fire();
    }
}
