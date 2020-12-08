using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Spear : WeaponBase
{
    protected override void Initialize()
    {
        weaponType = WeaponType.Spear;
    }

    public override void Flip(Vector3 direction, Quaternion rotation)
    {
        base.Flip(direction, rotation);
        direction = direction * 30f;
        transform.position = transform.position.SetY(transform.position.y + 0.5f);
        transform.rotation = rotation;
        transform.eulerAngles = transform.eulerAngles.SetX(90.0f);
        mySequence = DOTween.Sequence()
            .Append(transform.DOMove(new Vector3(direction.x, transform.position.y, direction.z), 1f).SetEase(Ease.Linear).OnComplete(() => Destroy(gameObject)));
    }

    protected override void OnTriggerExtend(Collider collider)
    {
        if (collider.gameObject.tag == "EnemyHit")
        {
            var enemy = collider.gameObject.GetComponentInParent<Enemy>();
            if (!enemy || enemy.GetDeath()) return;

            var effect = Instantiate(hitEffect);
            effect.transform.position = gameObject.transform.position.SetY(1.0f);
            effect.transform.eulerAngles = effect.transform.eulerAngles.SetY(collider.gameObject.transform.eulerAngles.y);

            mySequence.Kill();
            enemy.SetDeathType(Enemy.DeathType.Down);
            enemy.Damage(power);

            transform.SetParent(collider.gameObject.transform);
            myCollider.enabled = false;

            AudioManager.Instance.PlaySE("weapon_spear");
        }

        if (collider.gameObject.tag == "Block")
        {
            mySequence.Kill();
            myCollider.enabled = false;
            transform.SetParent(collider.gameObject.transform);
        }
    }
}
