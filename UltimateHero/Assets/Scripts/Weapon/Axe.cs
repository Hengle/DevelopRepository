using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Axe : WeaponBase
{
    protected override void Initialize()
    {
        weaponType = WeaponType.Axe;
    }

    public override void Flip(Vector3 direction, Quaternion rotation)
    {
        base.Flip(direction, rotation);
        direction = direction * 30f;
        transform.position = transform.position.SetY(transform.position.y + 0.5f);
        transform.rotation = rotation;
        mySequence = DOTween.Sequence()
            .Append(transform.DORotate(new Vector3((360f * 5) * 1, transform.eulerAngles.y, 0.0f), 1f, RotateMode.FastBeyond360).SetEase(Ease.Linear))
            .Join(transform.DOMove(new Vector3(direction.x, transform.position.y, direction.z), 1f).SetEase(Ease.Linear).OnComplete(() => Destroy(gameObject)));
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
            transform.eulerAngles = new Vector3(35.0f,0.0f,0.0f);

            AudioManager.Instance.PlaySE("weapon_axe");
        }

        if (collider.gameObject.tag == "Block")
        {
            mySequence.Kill();
            myCollider.enabled = false;
            transform.SetParent(collider.gameObject.transform);
            transform.eulerAngles = new Vector3(35.0f, 0.0f, 0.0f);
        }
    }
}
