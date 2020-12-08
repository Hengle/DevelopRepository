using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.UI;

public class MainGameManager : MonoBehaviour, IAnimationRegistable
{
    [SerializeField] Player player;
    [SerializeField] Enemy enemy;
    [SerializeField] CameraManager cameraManager;
    [SerializeField] CinemachineVirtualCamera cinemachineVirtualCamera;
    [SerializeField] Text startText;
    [SerializeField] GameObject[] trapList;
    
    int animationMonitorCount { get; set; }
    int animationCount { get; set; }
    List<int> monitorAnimation = new List<int>();

    bool isHome = true;
    bool isPlayerAnimation, isEnemyAnimation, isGimmickAnimation;

    private void Start()
    {
        trapList[UnityEngine.Random.Range(0, trapList.Length)].SetActive(true);
        enemy.OnEscape += Reload;
    }

    private void Update()
    {
        if (isHome && Input.GetMouseButtonDown(0))
        {
            isHome = false;
            startText.enabled = false;
            StartCoroutine(GameStart());
        }
    }

    private IEnumerator GameStart()
    {
        cameraManager.FocusEnemy();

        yield return StartCoroutine(enemy.FirstMove());

        yield return new WaitForSeconds(0.5f);

        cinemachineVirtualCamera.Priority = 2;

        yield return new WaitForSeconds(1.0f);

        cameraManager.FocusPlayer();

        yield return new WaitForSeconds(1.0f);

        cinemachineVirtualCamera.Priority = 0;

        player.Activate();
        StartCoroutine(enemy.Move());
    }

    public void Register(IAnimationMonitorable monitorable)
    {
        animationMonitorCount++;
        monitorable.OnAnimeStart += OnMonitarableAnimationStart;
        monitorable.OnAnimeEnd += OnMonitarableAnimationEnd;
    }

    public void Unregister(IAnimationMonitorable monitorable)
    {
        animationMonitorCount--;
        monitorable.OnAnimeStart -= OnMonitarableAnimationStart;
        monitorable.OnAnimeEnd -= OnMonitarableAnimationEnd;
    }

    IEnumerator AnimationWait()
    {
        player.DeActivate();
        enemy.Stop();

        //プレイヤーとギミックの起動or発動アニメーションを待つ
        yield return new WaitWhile(() => isPlayerAnimation || isGimmickAnimation);

        //プレイヤーとギミックのアニメーションが終了してから3秒待つ
        yield return new WaitForSeconds(2.0f);

        //エネミーのアニメーションが始まっていたらエネミーのアニメーション終了を待つ
        if (isEnemyAnimation)
        {
            yield return new WaitWhile(() => isEnemyAnimation);
        }

        AnimationCheck();
    }

    void OnMonitarableAnimationStart(ObjectType objectType)
    {
        animationCount++;
        switch (objectType)
        {
            case ObjectType.Player:
                isPlayerAnimation = true;
                isGimmickAnimation = true;
                StartCoroutine(AnimationWait());
                break;
            case ObjectType.Gimmick:
                if (!isGimmickAnimation)
                {
                    isGimmickAnimation = true;
                    StartCoroutine(AnimationWait());
                }
                break;
            case ObjectType.Enemy:
                isEnemyAnimation = true;
                break;
        }
    }

    void OnMonitarableAnimationEnd(ObjectType objectType)
    {
        animationCount--;
        switch (objectType)
        {
            case ObjectType.Player:
                isPlayerAnimation = false;
                break;
            case ObjectType.Gimmick:
                isGimmickAnimation = false;
                break;
            case ObjectType.Enemy:
                isEnemyAnimation = false;
                break;
        }
    }

    void AnimationCheck()
    {
        if (animationCount == 0)
        {
            if (enemy.DeathCheck())
            {
                enemy.Escape();
            }
            else
            {
                player.Activate();
                enemy.Play();
            }
        }
    }

    public bool IsAnimationPlay()
    {
        return true;
    }

    public void Reload()
    {
        SceneManager.Instance.ChangeScene("Cinemachine");
    }

    IEnumerator DelayInvoke(float time, Action onInvoke)
    {
        yield return new WaitForSeconds(time);
        onInvoke.Invoke();
    }
}
