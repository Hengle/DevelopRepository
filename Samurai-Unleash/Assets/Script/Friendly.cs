using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using DG.Tweening;

public class Friendly : MonoBehaviour
{
    [SerializeField] GameObject headSprite;
    [SerializeField] GameObject bottomSprite;
    [SerializeField] AudioClip deathVoice;
    [SerializeField] GameObject[] bleedingParticles;

    bool isDead;

    public event Action OnDestroyFriendlyListener; // フレンドリーが倒されたイベント
    public event Action<Vector2> OnBloodParticleListener; // フレンドリーが接触した際のイベント


    public void Damage() {

        if (isDead) { return; }

        isDead = true;
        DestroyAnimation();
        OnBloodParticleListener?.Invoke(transform.position);
    }

    /// <summary>
    /// 登録してある上下パーツに分裂しながら消えるやられアニメ
    /// </summary>
    private void DestroyAnimation() {

        if (DataManager.GetPlayerSettingSound()) {
            GetComponent<AudioSource>().PlayOneShot(deathVoice);
        }

        var randX = UnityEngine.Random.Range(-5, 6);
        var randXbottom = UnityEngine.Random.Range(-5, 6);

        var randY = UnityEngine.Random.Range(0, 6);

        headSprite.AddComponent<Rigidbody2D>();
        headSprite.GetComponent<Rigidbody2D>().AddForce(new Vector2(randX, randY), ForceMode2D.Impulse);
        headSprite.GetComponent<Rigidbody2D>().AddTorque(UnityEngine.Random.Range(-10, 10), ForceMode2D.Impulse);
        headSprite.transform.DOScale(Vector3.one * 0.8f, 0.5f);

        bottomSprite.AddComponent<Rigidbody2D>();
        bottomSprite.GetComponent<Rigidbody2D>().AddForce(new Vector2(randXbottom, -3f), ForceMode2D.Impulse);
        bottomSprite.GetComponent<Rigidbody2D>().AddTorque(UnityEngine.Random.Range(-10, 10), ForceMode2D.Impulse);
        bottomSprite.transform.DOScale(Vector3.one * 0.8f, 0.5f);

        // 流血モードがオフの場合は出血無し
        if (bleedingParticles.Length >= 1 && !GameDirector.noBloodMode) {
            foreach (GameObject obj in bleedingParticles) {
                obj.SetActive(true);
            }
        }

        OnDestroyFriendlyListener?.Invoke();

    }
}
