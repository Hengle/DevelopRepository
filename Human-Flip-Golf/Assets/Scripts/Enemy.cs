using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Util;
using DG.Tweening;

public class Enemy : MonoBehaviour
{
    [SerializeField] GameObject bulletObj;
    [SerializeField] Transform gun;
    [SerializeField] Transform target;

    bool isAlive = true;
    RagdollUtility MyRagdollUtility { get; set; }
    Animator MyAnimator { get; set; }
    Collider MyCollider { get; set; }

    private void Awake()
    {
        MyRagdollUtility = GetComponent<RagdollUtility>();
        MyAnimator = GetComponent<Animator>();
        MyCollider = GetComponent<Collider>();
    }

    void Start()
    {
        MyRagdollUtility.Setup();
        MyRagdollUtility.Deactivate();
    }

    public bool GetIsAlive()
    {
        return isAlive;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag != "Bullet") return;
        if (!other.GetComponent<Bullet>().isPlayerBullet) return;
        isAlive = false;
        var direction = (other.transform.position - transform.position).normalized * 30f;
        direction.y = Mathf.Abs(direction.y);
        MyCollider.enabled = false;
        MyRagdollUtility.Activate();
        MyRagdollUtility.AddForce(direction, direction);
    }

    public void ShotAnimation()
    {
        MyAnimator.enabled = true;
        MyCollider.enabled = false;
        Invoke("Shot", 0.2f);
    }

    void Shot()
    {
        var bullet = Instantiate(bulletObj) as GameObject;
        bullet.transform.position = gun.transform.position;
        Vector3 world = target.position - gun.transform.position;
        bullet.GetComponent<Bullet>().isPlayerBullet = false;
        bullet.GetComponent<Rigidbody>().AddForce(world.normalized * 500f, ForceMode.Impulse);
    }
    
}
