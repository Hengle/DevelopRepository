using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;
using Util;
using UnityEngine.UI;

public class MainGameManager : MonoBehaviour
{
    [SerializeField] CameraManager cameraManager;
    [SerializeField] MultipleParticles resultParticles;
    [SerializeField] GameObject uiBar;
    [SerializeField] GameObject stageSelectButton;
    [SerializeField] GameObject stageSelectListUI;
    [SerializeField] GameObject stageSelectList;
    [SerializeField] List<GameObject> stageList;
    [SerializeField] GameObject resultGameClearUI;
    [SerializeField] GameObject resultGameOverUI;

    int round = 0;
    int nowStage;
    bool isGameOver;
    bool isGameClear;
    bool isAnimation;
    bool isSetting;
    bool isSelecting;
    List<Target> targetList;
    List<Ball> ballList;
    Target nowTarget;
    Ball nowBall;
    Kicker kicker;

    private void Awake()
    {
        DataManager.Instance.Load();

        //ステージの進行度読み込み
        nowStage = DataManager.Instance.NowStage;
        CreateStageList();

        var stage = Instantiate(stageList[nowStage]);
        targetList = new List<Target>(stage.transform.Find("TargetList").transform.GetComponentsInChildren<Target>());
        ballList = new List<Ball>(stage.transform.Find("BallList").transform.GetComponentsInChildren<Ball>());
        kicker = stage.transform.Find("Kicker").transform.GetComponent<Kicker>();

        kicker.OnShootListener += PlayerShoot;
        foreach (var target in targetList)
        {
            target.OnHitListener += TargetHit;
        }
        foreach (var ball in ballList)
        {
            ball.OnMoveEndListener += BallMoveEnd;
        }

        nowTarget = targetList[0];
        nowBall = ballList[0];
        kicker.SetTarget(nowTarget);
        UIBarInitialize();
        cameraManager.SwitchFollowTarget(kicker.gameObject);
        StartCoroutine(MainRoutine());
    }

    private void Start()
    {
        AudioManager.Instance.LoadSe("button_default1");
    }

    IEnumerator MainRoutine()
    {
        while (true)
        {
            kicker.Activate();
            
            yield return new WaitWhile(() => !isAnimation);

            isAnimation = false;

            //ゲームクリア判定
            if (isGameClear || isGameOver)
            {
                break;
            }

            SetNextTarget();

            KickerMoveNextPosition();

            yield return new WaitWhile(() => !isAnimation);

            isAnimation = false;
        }

        ShowResult();
    }

    void TargetHit()
    {
        round++;
        if (round == targetList.Count) isGameClear = true;
    }

    void SetNextTarget()
    {
        if (round == targetList.Count) return;
        nowTarget = targetList[round];
        nowBall = ballList[round];
        kicker.SetBall(ballList[round]);
        kicker.SetTarget(targetList[round]);
    }

    void KickerMoveNextPosition()
    {
        kicker.transform.DOMove(new Vector3(kicker.transform.position.x, kicker.transform.position.y, ballList[round].transform.position.z - 0.5f), 2.0f)
            .OnComplete(() => isAnimation = true );
    }

    void PlayerShoot()
    {
        //カメラをボールに向ける
        //kicker.Deactivate();
    }

    void BallMoveEnd()
    {
        isAnimation = true;
        if (!nowTarget.HitCheck())
        {
            isGameOver = true;
        }
    }

    void ShowResult()
    {
        if (isGameClear)
        {
            ShowGameClear();
            
        }

        if (isGameOver)
        {
            ShowGameOver();
        }
    }

    void ShowGameClear()
    {
        cameraManager.SwitchFollowTarget(nowTarget.gameObject);
        cameraManager.FocusTarget();
        resultParticles.Play();

        Invoke("ShowStar",2.0f);
    }

    void ShowStar()
    {
        resultGameClearUI.SetActive(true);
        var stars = resultGameClearUI.transform.Find("Star");
        Sequence sequence = DOTween.Sequence();
        foreach (Transform star in stars)
        {
            sequence.Append(star.DOScale(1.0f, 1.0f));
            sequence.Join(star.DORotate(new Vector3(0.0f,0.0f,360.0f), 1.0f,RotateMode.FastBeyond360));
        }
        sequence.OnComplete(ShowRewaord);
    }
    void ShowRewaord()
    {
        var rewaordBtn = resultGameClearUI.transform.Find("Rewaord");
        rewaordBtn.gameObject.SetActive(true);
        Invoke("ShowNoRewaord", 3.0f);
    }

    void ShowNoRewaord()
    {
        var noRewaordBtn = resultGameClearUI.transform.Find("NoRewaord");
        noRewaordBtn.gameObject.SetActive(true);
    }

    void ShowGameOver()
    {
        resultGameOverUI.SetActive(true);
        var noContinueBtn = resultGameOverUI.transform.Find("NoContinue");
        noContinueBtn.transform.GetComponent<CanvasGroup>().DOFade(1.0f, 0.5f).SetDelay(3.0f).OnComplete(() => noContinueBtn.transform.GetComponent<Button>().enabled = true);
    }

    public void NextStage()
    {
        nowStage++;
        DataManager.Instance.ChangeNowStage(nowStage);
        DataManager.Instance.Save();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    void CreateStageList()
    {
        for (int i = 1; i <= DataManager.Instance.StageLevel; i++)
        {
            var button = Instantiate(stageSelectButton);
            button.GetComponent<StageSelectButton>().SetStatus(i);
            button.transform.SetParent(stageSelectList.transform, false);
        }
    }

    public void Retry()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    void UIBarInitialize()
    {
        var sound = uiBar.transform.Find("Sound").gameObject;
        sound.GetComponent<Image>().sprite = DataManager.Instance.Sound ? Resources.Load<Sprite>("sound_on") : Resources.Load<Sprite>("sound_off");
        var vibration = uiBar.transform.Find("Vibration").gameObject;
        vibration.GetComponent<Image>().sprite = DataManager.Instance.Vibration ? Resources.Load<Sprite>("vibration_on") : Resources.Load<Sprite>("vibration_off");
    }

    public void Setting()
    {
        if (!isSetting)
        {
            isSetting = true;
            uiBar.transform.DOLocalMove(new Vector3(uiBar.transform.localPosition.x + uiBar.GetComponent<RectTransform>().sizeDelta.x * 0.6f, uiBar.transform.localPosition.y, 0.0f), 0.5f);
        }
        else
        {
            isSetting = false;
            uiBar.transform.DOLocalMove(new Vector3(uiBar.transform.localPosition.x - uiBar.GetComponent<RectTransform>().sizeDelta.x * 0.6f, uiBar.transform.localPosition.y, 0.0f), 0.5f);
        }
    }

    public void ChangeSoundSetting()
    {
        DataManager.Instance.ChangeSound(!DataManager.Instance.Sound);
        DataManager.Instance.Save();
        var sound = uiBar.transform.Find("Sound").gameObject;
        sound.GetComponent<Image>().sprite = DataManager.Instance.Sound ? Resources.Load<Sprite>("sound_on") : Resources.Load<Sprite>("sound_off");
    }

    public void ChangeVibrationSetting()
    {
        DataManager.Instance.ChangeVibration(!DataManager.Instance.Vibration);
        var vibration = uiBar.transform.Find("Vibration").gameObject;
        vibration.GetComponent<Image>().sprite = DataManager.Instance.Vibration ? Resources.Load<Sprite>("vibration_on") : Resources.Load<Sprite>("vibration_off");
        DataManager.Instance.Save();
    }

    public void StageSelect()
    {
        if (!isSelecting)
        {
            isSelecting = true;
            stageSelectListUI.transform.DOScale(new Vector3(1.0f, 1.0f, 1.0f), 0.5f).SetEase(Ease.OutBounce);
        }
        else
        {
            isSelecting = false;
            stageSelectListUI.transform.DOScale(new Vector3(0.0f, 0.0f, 0.0f), 0.5f);
        }
        
    }
}
