using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using DG.Tweening;
using SDKManager;

public class GameManager : MonoBehaviour
{
    [SerializeField] GameObject almost;
    [SerializeField] GameObject next;
    [SerializeField] GameObject reward;
    [SerializeField] TextMeshProUGUI getCoinText;
    [SerializeField] CanvasGroup getCoin;

    string sceneName;
    PlayerController playerController;
    BlowerGameManager blowerGameManager;
    GardenerGameManager gardenerGameManager;
    WateringGameManager wateringGameManager;
    HarvestGameManager harvestGameManager;
    ParticleSystem resultParticle;
    TextMeshProUGUI scoreText;
    Coin coin;
    float nowScore;
    bool isScoreOver;

    void Awake()
    {
        playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        scoreText = GameObject.FindGameObjectWithTag("Score").GetComponent<TextMeshProUGUI>();
        coin = GameObject.FindGameObjectWithTag("Coin").GetComponent<Coin>();
        resultParticle = GameObject.FindGameObjectWithTag("Star").GetComponent<ParticleSystem>();
    }

    void Start()
    {
        AppLovinManager.Instance.AppLovinBannerAD(false);
        sceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
        switch (sceneName)
        {
            case "Blower":
                blowerGameManager = GetComponent<BlowerGameManager>();
                blowerGameManager.OnUpdateScoreListener += ScoreUpdate;
                break;
            case "Gardener":
                gardenerGameManager = GetComponent<GardenerGameManager>();
                gardenerGameManager.OnUpdateScoreListener += ScoreUpdate;
                break;
            case "Watering":
                wateringGameManager = GetComponent<WateringGameManager>();
                wateringGameManager.OnUpdateScoreListener += ScoreUpdate;
                break;
            case "Harvest":
                harvestGameManager = GetComponent<HarvestGameManager>();
                harvestGameManager.OnUpdateScoreListener += ScoreUpdate;
                break;
        }
        playerController.Activate();
    }

    void ScoreUpdate(float score)
    {
        nowScore = score;
        scoreText.text = (Mathf.Floor(score * 100)).ToString() + "%";
        if (score >= 0.8f && !isScoreOver)
        {
            isScoreOver = true;
            DOTween.Sequence()
            .Append(almost.transform.DOLocalMoveX(0f, 0.5f).SetEase(Ease.Linear))
            .Append(almost.transform.DOLocalMoveX(720f, 0.5f).SetEase(Ease.Linear).SetDelay(0.5f));

            next.SetActive(true);
        }

        if (score >= 1.0f)
        {
            playerController.Deactivate();
            resultParticle.Play();
            reward.SetActive(true);
            coin.Add(100);
            getCoinText.text = (coin.coinCount - DataManager.Instance.Coin).ToString();
            getCoin.DOFade(1.0f,0.3f);
        }
    }

    public void Reward()
    {
        AppLovinManager.Instance.AppLovinShowRewardVideo(() =>
        {
            reward.SetActive(false);
            FacebookManager.Instance.FacebookCustomEvent("GET_REWARD");
            coin.Add(100);
            getCoinText.text = (coin.coinCount - DataManager.Instance.Coin).ToString();
        });
    }

    public void Next()
    {
        next.SetActive(false);
        //AppLovinManager.Instance.AppLovinShowInterstitial(() =>
        //{
        //    if (nowScore >= 1.0f)
        //    {
        //        FacebookManager.Instance.FacebookCustomEvent("FULL_CLEAR");
        //    }
        //    else
        //    {
        //        FacebookManager.Instance.FacebookCustomEvent("INTERRUPT_CLEAR");
        //    }

        //    DataManager.Instance.ChangeCoin(coin.coinCount);
        //    DataManager.Instance.ChangeNowStage(DataManager.Instance.NowStage + 1);
        //    DataManager.Instance.Save();

        //    switch (DataManager.Instance.StageList[DataManager.Instance.NowStage])
        //    {
        //        case GameType.Blower:
        //            SceneManager.Instance.ChangeScene("Blower", 0.5f);
        //            break;
        //        case GameType.Gardener:
        //            SceneManager.Instance.ChangeScene("Gardener", 0.5f);
        //            break;
        //        case GameType.Watering:
        //            SceneManager.Instance.ChangeScene("Watering", 0.5f);
        //            break;
        //        case GameType.Harvest:
        //            SceneManager.Instance.ChangeScene("Harvest", 0.5f);
        //            break;
        //    }
        //});
        DataManager.Instance.ChangeCoin(coin.coinCount);
        DataManager.Instance.ChangeNowStage(DataManager.Instance.NowStage + 1);
        DataManager.Instance.Save();

        switch (DataManager.Instance.StageList[DataManager.Instance.NowStage])
        {
            case GameType.Blower:
                SceneManager.Instance.ChangeScene("Blower", 0.5f);
                break;
            case GameType.Gardener:
                SceneManager.Instance.ChangeScene("Gardener", 0.5f);
                break;
            case GameType.Watering:
                SceneManager.Instance.ChangeScene("Watering", 0.5f);
                break;
            case GameType.Harvest:
                SceneManager.Instance.ChangeScene("Harvest", 0.5f);
                break;
        }
    }
}
