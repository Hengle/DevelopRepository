using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{

    //弾の移動スピード
    [SerializeField] int speed = 10;

    //ゲームオブジェクト生成から削除するまでの時間
    [SerializeField] float lifeTime = 1;

    //弾オブジェクト
    [SerializeField] GameObject bullet;

    // 攻撃力
    public int power = 1;

    private void Awake()
    {
        GetComponent<Rigidbody2D>().velocity = transform.up.normalized * speed;
        Destroy(this.gameObject, lifeTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //IDamageableを持っていたらダメージを与える
        var damageable = collision.gameObject.GetComponent<IDamageable<int>>();
        if (damageable != null)
        {
            damageable.Damage(power);
        }
    }
}
