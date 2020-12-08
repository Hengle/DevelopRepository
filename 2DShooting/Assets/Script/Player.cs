using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour, IDamageable<int>
{
    [SerializeField] int life;

    public event Action<bool> OnDeathTrigger;

    //初期化
    public void Initialize()
    {
        OnDeathTrigger?.Invoke(false);
    }

    public void Damage(int damage)
    {
        life -= damage;
        if(life <= 0)
        {
            OnDeathTrigger?.Invoke(true);
        }
    }
}
