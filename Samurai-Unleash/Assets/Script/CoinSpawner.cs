using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class CoinSpawner : MonoBehaviour {
    [SerializeField] GameObject prefab;

    bool isSpawn;

    Vector2 DefaultPos = new Vector2(0, 0);

    List<CoinObj> tests = new List<CoinObj>();

    void OnDestroy() {
        foreach (var a in tests) {
            a.Dispose();
        }
    }


    IEnumerator Start() {

        var basePos = transform.position;

        int limit = 60;
        int nowCount = 0;
        while (true) {
            var newPos = new Vector3(basePos.x + Mathf.Cos(Time.time * 50) * 1, basePos.y + Mathf.Sin(Time.time * 50) * 1,0);
            transform.position = newPos;

            var addVec = basePos - newPos;
            addVec.x *= -1f;
            addVec.y *= -1f;

            // 

                var hoge = Instantiate(prefab);
                hoge.transform.position = transform.position;
                hoge.transform.localScale = prefab.transform.localScale;
            //                hoge.GetComponent<Rigidbody2D>().AddForce(new Vector2(Random.Range(-150, 150), Random.Range(-150, 150)));
            hoge.GetComponent<Rigidbody2D>().AddForce(addVec * Random.Range(100,200));
            if (!isSpawn) { hoge.SetActive(false); }

            yield return new WaitForSeconds(0.001f);

            nowCount++;


            if (nowCount > limit) {
                break;
            }
        }

        Destroy(gameObject);

    }

    private void OnTriggerStay2D(Collider2D collision) {
        var target = collision.gameObject;

        if (target.tag == "Background") {
            isSpawn = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision) {
        var target = collision.gameObject;

        if (target.tag == "Background") {
            isSpawn = false;
        }
    }
}
