using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Gimmick : MonoBehaviour
{
    Collider myCollider;
    [SerializeField] protected ParticleSystem collisionEffect;

    public abstract void ActiveTrap();

    private void Awake()
    {
        Initialize();
    }

    protected virtual void Start()
    {

    }

    protected virtual void Initialize()
    {
        myCollider = GetComponent<Collider>();
    }
}
