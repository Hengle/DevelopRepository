using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;
using SDKManager;
using GameAnalyticsSDK;

public class HomeManager : MonoBehaviour
{
    [SerializeField] CanvasGroup homeUI;
    [SerializeField] CanvasGroup resultUI;
    [SerializeField] Sprite[] clientIcons;
    [SerializeField] Image[] stageIcons;
    [SerializeField] TextMeshProUGUI stageText;
    [SerializeField] Transform complete;
    [SerializeField] TextMeshProUGUI completeText;
    [SerializeField] Transform progressBar;
    [SerializeField] TextMeshProUGUI progressBarText;
    [SerializeField] Transform reward;
    [SerializeField] TextMeshProUGUI rewardAmount;
    [SerializeField] Transform noThanks;
    [SerializeField] Button rewardBtn;
    [SerializeField] Button noThanksBtn;
    [SerializeField] Transform newGameText;
    [SerializeField] GameObject startButton;
    [SerializeField] Transform firstClientPosition;
    [SerializeField] Transform secondClientPosition;
    [SerializeField] ParticleSystem resultParticle;
    [SerializeField] CoinAnimationGUI maneyAnimation;

    [SerializeField] StageMaster stageMaster;
    StageData nowData;
    StageData nextData;
    Slider slider;
    Vector3 position;

    int day;
    int task;

    private void Awake()
    {
        day = DataManager.Instance.Day + DataManager.Instance.LoopCount * 6;
        task = DataManager.Instance.Task;
        nowData = stageMaster.GetStageData(DataManager.Instance.Day, task);
        slider = GameObject.FindGameObjectWithTag("ProgressBar").GetComponent<Slider>();

        if (nowData.Id == 18)
        {
            nextData = stageMaster.GetStageData(1, 1);
        }
        else
        {
            nextData = stageMaster.GetStageData(nowData.Id + 1);
        }
    }

    private void Start()
    {
        GameAnalytics.Initialize();

        var firstClient = Instantiate(nowData.ClientModel, firstClientPosition);
        Instantiate(nextData.ClientModel, secondClientPosition).GetComponent<Animator>().Play("Carry");
        Instantiate(nextData.PlushDollModel, secondClientPosition.Find("PlushDollPosition")).GetComponent<PlushDoll>().HomeView(nextData);

        switch (DataManager.Instance.HomeType)
        {
            case "Home":
                Instantiate(nowData.PlushDollModel, firstClientPosition.Find("PlushDollPosition")).GetComponent<PlushDoll>().HomeView(nowData);
                resultUI.alpha = 0;
                resultUI.blocksRaycasts = false;
                RefreshStageIcon();
                break;

            case "Result":
                homeUI.alpha = 0;
                homeUI.blocksRaycasts = false;
                GameObject.FindGameObjectWithTag("Respawn").transform.parent = firstClientPosition.Find("PlushDollPosition");
                GameObject.FindGameObjectWithTag("Respawn").transform.GetComponent<PlushDoll>().Refresh();
                //最初にセーブ
                DataManager.Instance.UpdateDayTask(nextData);
                DataManager.Instance.AddCoin(nowData.Money);
                if (nowData.Id == 18) DataManager.Instance.AddLoopCount(1);
                DataManager.Instance.Save();
                FacebookManager.Instance.FacebookCustomEvent("DAY" + day + "-" + task);
                GameAnalytics.NewProgressionEvent(GAProgressionStatus.Start, "DAY" + day, "TASK" + task, "Clear");
                firstClient.GetComponent<Animator>().Play("Waving");
                StartCoroutine(Result());
                break;
        }

        
    }

    IEnumerator Result()
    {
        yield return new WaitForSeconds(0.5f);

        resultParticle.Play();

        //コンプリート
        if(nowData.Task == 3)
        {
            completeText.text = "DAY" + day;
            complete.DOScale(1.0f, 0.5f).SetEase(Ease.InOutBounce);
        }

        //バー
        if (day <= 4)
        {
            float endValue = 0f;
            switch (task)
            {
                case 1:
                    slider.value = 0f;
                    endValue = 0.3f;
                    break;
                case 2:
                    slider.value = 0.3f;
                    endValue = 0.6f;
                    break;
                case 3:
                    slider.value = 0.6f;
                    endValue = 1.0f;
                    break;
            }
            progressBarText.text = Mathf.FloorToInt(slider.value * 100f) + "%";
            Sequence sequence = DOTween.Sequence();
            sequence.Append(progressBar.DOScale(1.0f, 0.5f).SetEase(Ease.InOutBounce));
            sequence.Append(DOTween.To(() => slider.value, (v) => { slider.value = v; progressBarText.text = Mathf.FloorToInt(v * 100f) + "%"; }, endValue, 2.0f).SetDelay(0.5f));
            if (task == 3)
            {
                sequence.Append(progressBar.DOScale(0.0f, 0.5f).SetEase(Ease.InOutBounce));
                sequence.Append(newGameText.DOScale(1.0f, 0.5f).SetEase(Ease.InOutBounce));
            }

        }

        //リワード
        reward.DOScale(1.0f, 0.5f).SetEase(Ease.InOutBounce).OnComplete(() => rewardBtn.enabled = true);
        rewardAmount.text = nowData.Money.ToString();

        yield return new WaitForSeconds(1.5f);

        noThanks.DOScale(1.0f, 0.5f).SetEase(Ease.InOutBounce).OnComplete(() => noThanksBtn.enabled = true);
    }

    void RefreshStageIcon()
    {
        stageText.text = "DAY" + day;
        switch (task)
        {
            case 1:
            default:
                break;
            case 2:
                stageIcons[0].sprite = clientIcons[2];
                stageIcons[1].sprite = clientIcons[1];
                break;
            case 3:
                stageIcons[0].sprite = clientIcons[2];
                stageIcons[1].sprite = clientIcons[2];
                stageIcons[2].sprite = clientIcons[1];
                break;
        }
    }

    public void OnClickConfig()
    {

    }

    public void Reward()
    {
        rewardBtn.enabled = false;
        noThanksBtn.enabled = false;
        AppLovinManager.Instance.AppLovinShowRewardVideo(() =>
        {
            DataManager.Instance.AddCoin(nowData.Money * 2);
            DataManager.Instance.Save();
            maneyAnimation.Play();
            reward.DOScale(0.0f, 0.5f).SetEase(Ease.Linear);
            noThanks.DOScale(0.0f, 0.5f).SetEase(Ease.Linear);
            Invoke("SceneChange", 3.0f);
        });
    }

    public void BackHome()
    {
        rewardBtn.enabled = false;
        noThanksBtn.enabled = false;
        reward.DOScale(0.0f, 0.5f).SetEase(Ease.Linear);
        noThanks.DOScale(0.0f, 0.5f).SetEase(Ease.Linear);
        maneyAnimation.Play();
        Invoke("SceneChange", 3.0f);
    }

    void SceneChange()
    {
        //AppLovinManager.Instance.AppLovinShowInterstitial(() =>
        //{
            DataManager.Instance.HomeType = "Home";
            SceneManager.Instance.ChangeScene("Home");
        //});
    }

    public void PlayGame()
    {

        SceneManager.Instance.ChangeScene("Game");
    }
}
