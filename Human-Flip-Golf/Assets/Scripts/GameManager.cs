using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Cinemachine;
using Util;

public class GameManager : MonoBehaviour
{
    [SerializeField] Character character;
    [SerializeField] Enemy enemy;
    [SerializeField] CanvasGroup homeUI;
    [SerializeField] CanvasGroup goUI;
    [SerializeField] CanvasGroup gcUI;
    [SerializeField] CinemachineSmoothPath path;
    [SerializeField] MultipleParticles resultParticle;

    int stageNumber;
    bool waitAction = true;
    bool isGameOver;

    private void Awake()
    {
        stageNumber = DataManager.Instance.Stage;
        gcUI.blocksRaycasts = false;
        character.OnDeathListener += PlayerDeath;

        TinySauce.OnGameStarted();
        TinySauce.OnGameStarted(levelNumber: stageNumber.ToString());
    }

    void Start()
    {
        StartCoroutine(MainRoutine());
    }

    IEnumerator MainRoutine()
    {
        yield return new WaitWhile(() => !Input.GetMouseButtonDown(0));

        character.Move();

        homeUI.alpha = 0f;

        yield return new WaitWhile(() => waitAction && PathMonitor());

        character.ResetFPS();

        if(enemy != null && enemy.GetIsAlive())
        {
            enemy.ShotAnimation();
        }
        else
        {
            character.Landing();
        }

        yield return new WaitForSeconds(1.0f);

        if (!isGameOver)
        {
            resultParticle.Play();
            gcUI.blocksRaycasts = true;
            gcUI.alpha = 1f;
        }
    }

    public void NextStage()
    {
        DataManager.Instance.AddStageCount();
        DataManager.Instance.Save();
        TinySauce.OnGameFinished(levelNumber: stageNumber.ToString(), 1);
        LoadScene();
    }

    void PlayerDeath()
    {
        waitAction = false;
        isGameOver = true;
        TinySauce.OnGameFinished(levelNumber: stageNumber.ToString(), 0);
        goUI.DOFade(1.0f, 2f)
            .OnComplete(() => LoadScene());
    }

    void LoadScene()
    {
        int loadStage = DataManager.Instance.Stage % 3;
        if (loadStage == 0) loadStage = 3;
        SceneManager.Instance.ChangeScene("Stage" + loadStage);
    }

    bool PathMonitor()
    {
        return character.GetCameraPos() <= 0.85f;
    }
}
