using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;

public class Enemy : MonoBehaviour {

    [SerializeField] bool isBoss;
    [SerializeField] bool isFriendly;
    [SerializeField] float ragdollLifeTime = 5f;
    [SerializeField] GameObject hukidasi;
    [SerializeField] TextMeshPro bossText;
    [SerializeField] Camera myCamera;
    [SerializeField] BaymaxDetectionArea myDetectionArea;
    [SerializeField] GameObject myHead;
    [SerializeField] GameObject[] myHands;
    [SerializeField] List<GameObject> myBodys = new List<GameObject>();
    [SerializeField] ParticleSystem headBlood;
    [SerializeField] ParticleSystem bodyBlood;
    [SerializeField] Material nomalEnemyMat;
    [SerializeField] Material bossEnemyMat;
    [SerializeField] Material civilianMat;
    BoxCollider myBoxCollider;
    Animator myAnimator;
    bool isDead;
    bool isFound;
    bool isKilled;
    GameObject targetToLook;

    public event Action<GameObject, bool> OnDestoyedEnemy;
    public event Action OnFriendlyFire;
    public event Action<Transform> OnHitEffect;

    private void Awake() {
        SetMyMaterial();
    }

    void Start() {

        bossText.gameObject.SetActive(isBoss);

        myDetectionArea.FoundSaw += (target) => {
            targetToLook = target;
            FoundSaw();
        };

        myDetectionArea.GoOutSaw += () => {
            targetToLook = null;
            GoOutSaw();
        };

        myAnimator = GetComponent<Animator>();
        myBoxCollider = GetComponent<BoxCollider>();
        SwitchRagdoll(false);
    }

    private void LateUpdate() {
        if (isFound && targetToLook != null && !isDead) {
            gameObject.transform.LookAt(targetToLook.transform);
        }
    }

    private void SetMyMaterial() {

        Material myMaterial;

        if (isFriendly) {
            myMaterial = civilianMat;
        } else if (isBoss) {
            myMaterial = bossEnemyMat;
        } else {
            myMaterial = nomalEnemyMat;
        }

        // 全部位にマテリアル反映
        foreach (GameObject obj in myBodys) {
            obj.GetComponent<MeshRenderer>().material = myMaterial;
        }

    }

    private void FoundSaw() {
        if (isDead) { return; }

        if (!isFound) {
            isFound = true;
            myAnimator.Play("Baymax-Handup");

            // 民間人は吹き出し
            if (isFriendly) {
                hukidasi.SetActive(true);
            }

        }
    }

    // 刃が自分のそばから消えた
    private void GoOutSaw() {
        if (isDead) { return; }

        if (isFound) {
            isFound = false;
            myAnimator.Play("Baymax-Handup-Down");

            // 民間人は吹き出し
            if (isFriendly) {
                hukidasi.SetActive(false);
            }

        }
    }

    private void SwitchRagdoll(bool newVal) {

        if (!isKilled) {
            myBoxCollider.enabled = !newVal;
            myAnimator.enabled = !newVal;
        }
        foreach (GameObject obj in myBodys) {
            obj.GetComponent<Rigidbody>().isKinematic = !newVal;
        }

    }

    // 死亡処理
    public void KillEnemy(Transform direction) {
        isDead = true;
        hukidasi.SetActive(false);
        bossText.gameObject.SetActive(false);

        var vec = gameObject.transform.position - direction.gameObject.transform.position;
        vec = vec.normalized;
        SwitchRagdoll(true);

        // 頭部を切断
        Destroy(myHead.GetComponent<CharacterJoint>());

        // 頭部射出
        var shootVec = (Vector3.up * 20) + new Vector3(UnityEngine.Random.Range(-5, 5), UnityEngine.Random.Range(0, 5), UnityEngine.Random.Range(-5, 5));

        myHead.GetComponent<Rigidbody>().AddForce(shootVec, ForceMode.Impulse);
        myHead.GetComponent<Rigidbody>().AddTorque(new Vector3(UnityEngine.Random.Range(-50, 50), UnityEngine.Random.Range(-50, 50), UnityEngine.Random.Range(-50, 50)));

        // 頭部出血
        headBlood.Play();

        // 体から出血！
        bodyBlood.Play();

        // 胴体を吹き飛ばす
        myBodys[1].GetComponent<Rigidbody>().AddForce(vec * 50, ForceMode.Impulse);

        // 刃を発見してたら手も切れる
        if (isFound) {
            foreach (GameObject obj in myHands) {
                var shootVec2 = (Vector3.up * 20) + new Vector3(UnityEngine.Random.Range(-5, 5), UnityEngine.Random.Range(0, 5), UnityEngine.Random.Range(-5, 5));
                Destroy(obj.GetComponent<CharacterJoint>());
                obj.GetComponent<Rigidbody>().AddForce(shootVec2, ForceMode.Impulse);
            }
        }

        if (isFriendly) {
            OnFriendlyFire?.Invoke();
        } else {
            OnDestoyedEnemy?.Invoke(gameObject, isBoss);
        }

        OnHitEffect?.Invoke(myHead.transform);

        StartCoroutine(DelayMethod(ragdollLifeTime, () => {
            // 物理演算を切る
            isKilled = true;
            SwitchRagdoll(false);
        }));

        SoundManager.Instance.Play("EnemyDead");
    }

    private void OnTriggerEnter(Collider other) {
        if (other.gameObject.tag == "Player") {
            KillEnemy(other.gameObject.transform);
        }
    }

    public Camera GetEnemyCamera() {
        return myCamera;
    }

    private IEnumerator DelayMethod(float waitTime, Action action) {
        yield return new WaitForSeconds(waitTime);
        action();
    }
}
