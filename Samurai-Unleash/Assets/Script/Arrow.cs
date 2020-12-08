using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Arrow : MonoBehaviour
{

    [SerializeField] float delayToDestroy = 3f;

    private void OnTriggerEnter2D(Collider2D collision) {
        var target = collision.gameObject;

        if (target.tag == "Enemy") {

            if (DataManager.GetPlayerSettingVibration()) {
                IOSUtil.PlaySystemSound(1519);
                AndroidUtil.Vibrate(100);
            }

            target.GetComponent<Enemy>()?.Damage(1);
        } else if (target.tag == "Friendly") {
            target.GetComponent<Friendly>()?.Damage();
        } else if (target.tag == "Bomb") {

            target.GetComponent<Bomb>().Explode();
        }
    }

    private void OnCollisionEnter2D(Collision2D collision) {
        var target = collision.gameObject;

        if(target.tag == "Wall") {
            GetComponent<Rigidbody2D>().isKinematic = true;
            GetComponent<PolygonCollider2D>().enabled = false;
            gameObject.transform.SetParent(target.transform, true);
            StartCoroutine(DelayDestroy(delayToDestroy));
        }else if(target.tag == "DestroyableWall") {
            Destroy(gameObject);
        }

    }

    IEnumerator DelayDestroy(float sec) {
        yield return new WaitForSeconds(sec);

        var seq = DOTween.Sequence();
        seq.Append(gameObject.GetComponent<SpriteRenderer>().DOFade(0, 0.5f));
        seq.OnComplete(() => { Destroy(gameObject); });

    }

}
