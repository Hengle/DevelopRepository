using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using DG.Tweening;
using UnityEngine.UI;

public class Enemy : MonoBehaviour {
    [SerializeField] int health = 1; // 何回攻撃に耐えるか
    [SerializeField] GameObject headSprite;
    [SerializeField] GameObject bottomSprite;
    [SerializeField] GameObject[] bleedingParticles;
    [SerializeField] AudioClip[] deathVoices;
    [SerializeField] AudioClip[] bossHitSound;

    [SerializeField] bool isBoss;
    int maxHealth;
    bool isDead;
    bool isLastEnemy;
    Vector3 nowPos;
    GameObject bossUI;
    Image bossHealthBar;
    Image bossHealthBarOnDamage;

    GameDirector GameDirector;

    public event Action<Vector2> OnDestroyEnemyListener; // エネミーが倒されたイベント
    public event Action<Vector2> OnDestroyEnemyBossListener; // エネミーが倒されたイベント
    public event Action<Vector2> OnBloodParticleListener; // エネミーが接触した際のイベント

    private void Start() {
        maxHealth = health;//最大HPを記憶
        GameDirector = GameObject.Find("GameDirector").GetComponent<GameDirector>();
        nowPos = gameObject.transform.position;
    }

    public void SetBossHealthBar(GameObject boss, Image bossHealth, Image bossHealthOndamage) {
        bossUI = boss;
        bossHealthBar = bossHealth; //BossHealthBarFill
        bossHealthBarOnDamage = bossHealthOndamage;
    }

    public bool GetIsBoss() {
        return isBoss;
    }



    public void Damage(int dmg) {
        bool lastEnemyFlag = GameDirector.GetEnemyCount() == 1 ? true : false;

        if (isDead) { return; }

        this.health -= dmg;

        // 流血モードがOFFでなければヒット演出あり
        if (!GameDirector.noBloodMode) {
            OnBloodParticleListener?.Invoke(transform.position);
        }

        if (this.health <= 0) {
            isDead = true;

            if (GetComponent<CircleCollider2D>()) { GetComponent<CircleCollider2D>().enabled = false; } else if (GetComponent<BoxCollider2D>()) { GetComponent<BoxCollider2D>().enabled = false; }

            //DestroyAnimation();
            if (DataManager.GetPlayerSettingSound() && isBoss) {
                if (bossHitSound.Length >= 2) {
                    var playSeed = this.health % 2;
                    GetComponent<AudioSource>().PlayOneShot(bossHitSound[playSeed]);
                }
            }
            Invoke("DestroyAnimation", 0f);
        } else if (this.health >= 1) {

            if (DataManager.GetPlayerSettingSound() && isBoss) {
                if (bossHitSound.Length >= 2) {
                    var playSeed = this.health % 2;
                    GetComponent<AudioSource>().PlayOneShot(bossHitSound[playSeed]);
                }
            }

            if (!bossHealthBar) {
                bossHealthBar = GameObject.Find("BossHealthBarFill").GetComponent<Image>();
            }

            if (!bossHealthBarOnDamage) {
                bossHealthBarOnDamage = GameObject.Find("BossHealthBarFillOnDamage").GetComponent<Image>();
            }

            // ダメージ処理
            var damagedHealth = (float)health / (float)maxHealth;


            var damageAnime = DOTween.Sequence();
            damageAnime.Append(DOTween.To(() => bossHealthBar.fillAmount, (n) => bossHealthBar.fillAmount = n, damagedHealth, 0.2f));
            damageAnime.OnComplete(() => {
                DOTween.To(() => bossHealthBarOnDamage.fillAmount, (n) => bossHealthBarOnDamage.fillAmount = n, damagedHealth, 0.2f);
            });
            damageAnime.Play();

        }
    }

    /// <summary>
    /// 登録してある上下パーツに分裂しながら消えるやられアニメ
    /// </summary>
    private void DestroyAnimation() {

        bool lastEnemyFlag = GameDirector.GetEnemyCount() == 1 ? true : false;


        var randX = UnityEngine.Random.Range(-2, 2);
        var randXbottom = UnityEngine.Random.Range(-2, 2);

        var randY = UnityEngine.Random.Range(0, 6);

        // 流血無しモードなら分断もしない
        if (GameDirector.noBloodMode) {
            gameObject.AddComponent<Rigidbody2D>();
            GetComponent<Rigidbody2D>().AddForce(new Vector2(randX, randY), ForceMode2D.Impulse);
            GetComponent<Rigidbody2D>().AddTorque(UnityEngine.Random.Range(-10, 10), ForceMode2D.Impulse);
            if (!isBoss) {
                var deathAnime = DOTween.Sequence();
                deathAnime.Append(gameObject.transform.DOScale(Vector3.one * 2f, 0.5f));
                deathAnime.Append(gameObject.transform.DOScale(Vector3.zero, 2f));
                deathAnime.Play();
            }

        } else {
            headSprite.AddComponent<Rigidbody2D>();
            headSprite.GetComponent<Rigidbody2D>().AddForce(new Vector2(randX, randY), ForceMode2D.Impulse);
            headSprite.GetComponent<Rigidbody2D>().AddTorque(UnityEngine.Random.Range(-10, 10), ForceMode2D.Impulse);
            if (!isBoss) { headSprite.transform.DOScale(Vector3.one * 2, 0.5f); }

            bottomSprite.AddComponent<Rigidbody2D>();
            bottomSprite.GetComponent<Rigidbody2D>().AddForce(new Vector2(randXbottom, -3f), ForceMode2D.Impulse);
            bottomSprite.GetComponent<Rigidbody2D>().AddTorque(UnityEngine.Random.Range(-10, 10), ForceMode2D.Impulse);
            if (!isBoss) { bottomSprite.transform.DOScale(Vector3.one * 2, 0.5f); }
        }


        // 流血モードがオフの場合は出血無し
        if (bleedingParticles.Length >= 1 && !GameDirector.noBloodMode) {
            foreach (GameObject obj in bleedingParticles) {
                obj.SetActive(true);
            }
        }

        if (deathVoices.Length >= 1) {
            if (DataManager.GetPlayerSettingSound()) {
                var randSeed = UnityEngine.Random.Range(0, deathVoices.Length);
                GetComponent<AudioSource>().PlayOneShot(deathVoices[randSeed]);
                Debug.Log("Death Voice: " + deathVoices[randSeed]);
            }
        }

        if (isBoss) {
            OnDestroyEnemyBossListener?.Invoke(gameObject.transform.position);
        } else {
            OnDestroyEnemyListener?.Invoke(gameObject.transform.position);
        }

        Invoke("Dispos", 3);
    }

    private void Dispos() {
        Destroy(gameObject);
    }

}
