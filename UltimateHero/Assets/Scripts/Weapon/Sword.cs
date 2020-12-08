using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Sword : WeaponBase
{
    [SerializeField] Material capMaterial;

    protected override void Initialize()
    {
        weaponType = WeaponType.Sword;
    }

    public override void Flip(Vector3 direction, Quaternion rotation)
    {
        base.Flip(direction, rotation);
        direction = direction * 30f;
        transform.position = transform.position.SetY(transform.position.y + 0.5f);
        transform.rotation = rotation;
        transform.eulerAngles = transform.eulerAngles.SetZ(90.0f);
        mySequence = DOTween.Sequence()
            .Append(transform.DORotate(new Vector3(transform.eulerAngles.x, (360f * 5f), transform.eulerAngles.z), 1f, RotateMode.FastBeyond360).SetEase(Ease.Linear))
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

            //enemy.SetDeathType(Enemy.DeathType.Cut);
            enemy.SetDeathType(Enemy.DeathType.Down);
            enemy.Damage(power);

            AudioManager.Instance.PlaySE("weapon_sword");
        }

        if (collider.gameObject.tag == "Block")
        {
            //GameObject[] pieces = MeshCut.Cut(collider.gameObject, transform.position, transform.right, capMaterial);
            //pieces[0].transform.GetComponent<Collider>().enabled = false;
            //var rigidbody = pieces[1].AddComponent<Rigidbody>();
            //var nomal = (pieces[1].transform.position - transform.position).normalized;
            //rigidbody.AddForce(nomal * 30f, ForceMode.Impulse);
            mySequence.Kill();
            myCollider.enabled = false;
            transform.SetParent(collider.gameObject.transform);
        }
    }
}
