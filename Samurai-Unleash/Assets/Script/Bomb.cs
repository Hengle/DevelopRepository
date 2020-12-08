using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Bomb : MonoBehaviour {
    [SerializeField] GameObject explodePref;
    [SerializeField] BombActiveEffectArea myArea;
    [SerializeField] float attckPowerToShuriken = 10f;
    [SerializeField] AudioClip soundEffect;
    public List<GameObject> onMyAreaObjects = new List<GameObject>();
    bool isExplode;

    // Start is called before the first frame update
    void Start() {
        explodePref.transform.position = transform.position;

    }

    private void Update() {
    }

    /// <summary>
    /// パーティーだ！ エネミーは死ぬし、手裏剣は吹き飛ぶぜ！
    /// </summary>
    public void Explode() {

        if (isExplode) { return; }



        // 範囲内にいるオブジェクトを取得する
        onMyAreaObjects = myArea.GetObjectList();

        gameObject.GetComponent<SpriteRenderer>().DOFade(0,0.25f);
        isExplode = true;
        explodePref.SetActive(true);

        if (DataManager.GetPlayerSettingVibration()) {
            IOSUtil.PlaySystemSound(1519);
            AndroidUtil.Vibrate(100);
        }

        if (soundEffect!=null) {
            if (DataManager.GetPlayerSettingSound()) {
                GetComponent<AudioSource>().PlayOneShot(soundEffect);
            }
        }

        Debug.Log("Explode : " + gameObject.name);
        Invoke("DoExplode", 0.25f);


    }

    private void DoExplode() {
        foreach (GameObject obj in onMyAreaObjects) {

            switch (obj.tag) {

                case "Enemy":

                    obj.GetComponent<Enemy>()?.Damage(1);

                    break;

                case "Friendly":

                    obj.GetComponent<Friendly>()?.Damage();

                    break;

                case "Shuriken":

                    var pushVec = (obj.transform.position - transform.position).normalized;
                    obj.GetComponent<Rigidbody2D>().isKinematic = false;
                    obj.GetComponent<Rigidbody2D>().AddForce(pushVec * attckPowerToShuriken, ForceMode2D.Impulse);
                    obj.GetComponent<Rigidbody2D>().AddTorque(Random.Range(-2, 2), ForceMode2D.Impulse);

                    break;

                case "Bomb":
                    obj.GetComponent<Bomb>().Explode();
                    break;
            }

        }
    }

    public void DelayExplode(float sec) {
        Invoke("Explode",sec);
    }



}
