using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Util;

public class RouletteDirector : MonoBehaviour
{
    [SerializeField] GameDirector GameDirector;
    [SerializeField] GameObject resultRouletteButton;

    [SerializeField] GameObject rouletteUI;
    [SerializeField] GameObject rouletteObj;
    [SerializeField] GameObject spinButton;
    [SerializeField] GameObject noThxButton;
    [SerializeField] GameObject closeRouletteButton;
    

    // Rouletteの回せる回数
    int maxRouletteCount = 5;
    int rouletteCount;

    // Rewardの角度
    int[] rouletteAngle = { 0, 45, 90, 135, 180, 225, 270, 315 };

    // Rouletteの報酬 0はコイン 1はヒント
    int[,] rouletteReward = new int[,] {
        { 0,300 },       //コイン300
        { 0,100 },      //コイン100
        { 0,25 },       //コイン25
        { 0,50 },      //コイン50
        { 0,300 },       //コイン300
        { 0,100 },      //コイン100
        { 0,25 },       //コイン25
        { 0,50 },      //コイン50
    };

    // 各項目の重み付け
    Dictionary<int, int> rewardWeight = new Dictionary<int, int>() {
        { 0,3 },    //コイン300
        { 1,10 },    //コイン100
        { 2,22 },   //コイン25
        { 3,15 },    //コイン50
        { 4,3 },    //コイン300
        { 5,10 },    //コイン100
        { 6,22 },   //コイン25
        { 7,15 },    //コイン50
    };


    // Start is called before the first frame update
    void Start()
    {
        rouletteCount = maxRouletteCount;
        GameDirector.RouletteADEndListener += ()=> { SpinRoulette(); };
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OpenRoulette() {

        // リザルト画面のRouletteボタンを無効化
        resultRouletteButton.GetComponent<Button>().interactable = false;
        resultRouletteButton.transform.Find("Mask").gameObject.SetActive(true);
        resultRouletteButton.GetComponent<Animator>().enabled = false;


        OnClickSpinButton();
    }

    public void CloseRoulette() {
        rouletteUI.SetActive(false);
    }

    public void OnClickSpinButton() {




#if UNITY_EDITOR

        SpinRoulette();
#else
        GameDirector.ADVideoType = GameDirector.ADtype.ROULETTE;
        AppLovin.ShowRewardedInterstitial();
        
#endif
    }

    private void SpinRoulette() {
        // 初ルーレットですか？
        if (!DataManager.GetFirstRoulette()) {
            DataManager.SetFirstRoulette(true);
            TenjinManager.TenjinCustomEvent("SPIN ROULETTE UNIQUE COUNT");
        }

        TenjinManager.TenjinCustomEvent("SPIN ROULETTE COUNT");

        Debug.Log("Call Spin");
        
        rouletteCount-=1;
        rouletteUI.SetActive(true);

        var indexAngle = RandomUtility.ChooseByWeight(rewardWeight);
        //Debug.Log(indexAngle + ":" + rouletteAngle[indexAngle]);// 重み付けで判定したインデックス番号が判明
        Debug.Log(rouletteReward[indexAngle, 0] + "/" + rouletteReward[indexAngle, 1]);
        var targetBeginAngle = rouletteAngle[indexAngle]; // 対象となる座標が確定
        var targetEndAngle = targetBeginAngle + 44; // 対象の終わりの角度を決定
        float rotateAngle = Random.Range((float)targetBeginAngle, (float)targetEndAngle);

        // リワードの種類：rouletteReward[indexAngle, 0]  リワードの量：rouletteReward[indexAngle, 1]

        Sequence rouletteRotateAnime = DOTween.Sequence()
            .OnStart(() => {
                // ボタン非表示
                spinButton.SetActive(false);
                noThxButton.SetActive(false);
            })
            .Append(rouletteObj.transform.DORotate(new Vector3(0, 0, (-1080) + (+rotateAngle)), 3f, RotateMode.FastBeyond360))
            .OnComplete(() => { RouletteEndSpin(indexAngle); });
        rouletteRotateAnime.Play();
    }

    private void RouletteEndSpin(int indexAngle) {
        Debug.Log("end");

        // リワードを与える
        // リワードの種類：rouletteReward[indexAngle, 0]  リワードの量：rouletteReward[indexAngle, 1]
        if (rouletteReward[indexAngle, 0] == 0) {
            // コイン
            var nowCoin = DataManager.GetPlayerCoin();
            var addCoin = rouletteReward[indexAngle, 1];
            var endCoin = nowCoin + addCoin;


            DataManager.SetPlayerCoin(endCoin);
        } else {
            // ヒント
            var nowHint = DataManager.GetPlayerHint();
            var addHint = rouletteReward[indexAngle, 1];
            var endHint = nowHint + addHint;

            DataManager.SetPlayerHint(endHint);
        }

        //// 回数残ってなければ終わり
        if (rouletteCount <= 0) {
            spinButton.SetActive(false);
            noThxButton.SetActive(false);
            closeRouletteButton.SetActive(true);
        } else {
            spinButton.SetActive(true);
            noThxButton.SetActive(true);
            closeRouletteButton.SetActive(false);
        }

    }

}
