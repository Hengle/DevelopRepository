using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class DestroyableWall : MonoBehaviour
{
    [SerializeField] AudioClip destorySoundEffect;
    [SerializeField] List<GameObject> myChild = new List<GameObject>();


    private void OnCollisionEnter2D(Collision2D collision) {

        if (collision.gameObject.tag == "Player" || collision.gameObject.tag == "Arrow") {
            GetComponent<BoxCollider2D>().enabled = false;
            Explode();
        }

    }

    private void Explode() {

        if (DataManager.GetPlayerSettingSound()) {
            GetComponent<AudioSource>().PlayOneShot(destorySoundEffect);
        }

        foreach(GameObject obj in myChild) {

            obj.GetComponent<Rigidbody2D>().isKinematic = false;
            obj.GetComponent<Rigidbody2D>().AddForce(new Vector2(Random.Range(-4,2), Random.Range(-4, 6)), ForceMode2D.Impulse);
            obj.GetComponent<Rigidbody2D>().AddTorque(Random.Range(-2, 2), ForceMode2D.Impulse);
            obj.transform.DOScale(Vector3.zero, 1f);
        }

        Invoke("Dispose", 1f);
    }

    private void Dispose() {
        Destroy(gameObject);
    }
}
