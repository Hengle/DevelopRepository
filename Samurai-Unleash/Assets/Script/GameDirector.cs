using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using TMPro;
using UnityEngine.UI;
using DG.Tweening;


public class GameDirector : MonoBehaviour {

    private string appLovinSdkKey = "TJ2lRrOfe1NQU0rKdce0GiUHc-PsV4WPMsKUmbGuwpCEqTqaCuxx10f4m2yWmAOQsHf0hXg-Aoxnw7okFkqM-a";
    public static bool noBloodMode = false;
    public static int clearCount = 0;
    private int clearADCount = 2;
    public static int retryCount = 0;
    private int retryADCount = 3;

    public static int SELECT_LEVEL = 1;
    int rewardCoinPerStar = 10; // 星１個あたりもらえるコイン
    int bossKilledReward = 300; // ボスを初回撃破時のコイン

    public event Action RouletteADEndListener;

    public ADtype ADVideoType;
    public enum ADtype {
        SKIP,
        ROULETTE,
        STAR3
    }

    public CameraShake shake;
    [SerializeField] GameObject tutorialPanel;

    [SerializeField] GameObject bossUI;
    [SerializeField] Image bossFaceImage;
    [SerializeField] Image bossHealthBarOnDamage;
    [SerializeField] Image bossHealthBar;

    [SerializeField] GameObject finalAttackParticle;
    [SerializeField] GameObject flowerParticle;
    [SerializeField] GameObject bloodParticle; // 血しぶきParticle
    List<GameObject> bloodParticleList = new List<GameObject>();
    [SerializeField] GameObject hitParticle; // 血しぶきParticle
    List<GameObject> hitParticleList = new List<GameObject>();

    [SerializeField] AudioClip resultSoundEffect;
    [SerializeField] AudioClip resultStarSoundEffect;

    [SerializeField] GameObject coinParticle;
    [SerializeField] GameObject coinSpawner;

    [SerializeField] ParticleQuePlay resultCracker;

    [SerializeField] GameObject headerUI;
    [SerializeField] GameObject headerBladeImage; // 画面上部に表示する発射回数アイコン（刀）
    [SerializeField] GameObject headerBladeCount; // 画面上部に表示するアイコンの登録ターゲット

    [SerializeField] GameObject resultBlackMask;
    [SerializeField] TextMeshProUGUI resultText;
    [SerializeField] GameObject resultClearPanel; // CLEAR時のパネル
    [SerializeField] GameObject resultGameOverPanel; // Gameover時のパネル
    [SerializeField] GameObject[] resultStarImages;
    [SerializeField] GameObject resulltRouletteOpenButton;
    [SerializeField] GameObject resultAdClearButton;
    [SerializeField] GameObject resultCoinCounter;
    [SerializeField] GameObject[] resultLineParticle;
    [SerializeField] GameObject resultSlideInMenu;
    [SerializeField] GameObject[] resultFinalStageHiddenButtons;
    [SerializeField] GameObject[] resultFinalStageShowButtons;

    [SerializeField] TextMeshProUGUI starCounter; // ヘッダーのスターカウント
    [SerializeField] GameObject[] starImages; // ヘッダーのスターイメージたち
    [SerializeField] Sprite starSprite;
    [SerializeField] Sprite noStarSprite; // 空スター

    [SerializeField] Color32 goodStarBlade; // 星３が取れるまでの刀を塗りつぶす色

    List<MovePanel> movePanelList = new List<MovePanel>();
    List<SpeedChange> speedChangePanelList = new List<SpeedChange>();

    // カメラのタイプによる縮小率。5がノーマルサイズ
    int[] cameraDistanceType = { 5, 9 };

    bool isBossKilled;
    bool isClear; // げーむをClearしたかどうか
    bool isGameover; // Gameoverになったかどうか
    bool isShowedGameOver;
    bool isBoss;
    bool isHitStop; // ヒットストップ中かどうか
    bool isResultSkip; // リザルトをスキップするかどうか
    bool isSkipMode; // スキップしたか？
    bool isCheckedLastEnemy;
    bool isQueenKilled;

    int bladeCount; // 刀の在庫数
    int shootCount; // 発射した回数
    int enemyCount; // 敵の数
    int nowStarCount; // 獲得したスター数
    int cameraType; // 読み込んだステージのカメラ設定

    int lastCameraSize = 5;
    Vector3 defaultCameraPos;
    Vector3 lastEnemyPos = Vector3.zero;
    GameObject player;
    StageSetting myStageSetting; // ステージの設定を取り出す変数
    IEnumerator hitStopEffect;
    List<GameObject> headerBladeIcons = new List<GameObject>(); // ヘッダーのアイコンを格納するリスト
    List<GameObject> allCharacterList = new List<GameObject>(); // ステージ上の全キャラクター

    public event Action<bool> OnEnablePlayerShootMode; // プレイヤーの発射許可


    void Awake() {
        Application.targetFrameRate = 60; //60FPSに設定

        FadeManager.FadeIn();
    }



    // Start is called before the first frame update
    void Start() {

#if UNITY_EDITOR
#else
                //instance = Tenjin.getInstance("D4DVXQQPQWKAGCGR91UTMTOERXPGWYBO");
                //instance.Connect();
                AppLovin.InitializeSdk();
                AppLovin.SetSdkKey(appLovinSdkKey);
                AppLovin.ShowAd( AppLovin.AD_POSITION_CENTER, AppLovin.AD_POSITION_BOTTOM );
                AppLovin.SetUnityAdListener(gameObject.name);
                // Interstitials動画広告 プリロード（事前読み込み）
                // ユーザーにスキップ可能な設定にするには、App LovinのVideo設定からスキップ可能にすること
                AppLovin.PreloadInterstitial();

                // リワード動画広告読み込み
                AppLovin.LoadRewardedInterstitial();
#endif



        // プレイカウント増加
        DataManager.SetPlayCount(DataManager.GetPlayCount() + 1);

        // 初期カメラの位置を記憶
        defaultCameraPos = Camera.main.transform.position;

        // ステージデータ読込
        var stagePref = Resources.Load<GameObject>("Stage/Stage" + SELECT_LEVEL);

        // ステージ生成
        var newStage = Instantiate(stagePref);

        // ステージセッティング読込
        myStageSetting = newStage.GetComponent<StageSetting>();

        // ボスステージですか？
        isBoss = myStageSetting.GetIsBossStage();
        bossUI.SetActive(isBoss);


        // ヘッダーの刀アイコンを生成
        for (int i = 0; i < myStageSetting.GetBladeCount(); i++) {
            // ヘッダーアイコンプレハブを読込
            var newBlade = Instantiate(headerBladeImage);
            // ヘッダーにセット
            newBlade.transform.SetParent(headerBladeCount.transform, false);
            // リストに追加
            headerBladeIcons.Add(newBlade);
        }

        // 星３評価になるブレードの色を変える
        var goodShot = myStageSetting.GetBestBladeCount();// 良いとされるショット数
        for (int i = 0; i < goodShot; i++) {
            headerBladeIcons[headerBladeIcons.Count - 1 - i].GetComponent<Image>().color = goodStarBlade;
        }

        // ブレードの数をカウント
        bladeCount = myStageSetting.GetBladeCount();

        // カメラ設定を読込
        cameraType = myStageSetting.GetCameraType();
        lastCameraSize = cameraDistanceType[cameraType];
        Camera.main.GetComponent<Camera>().orthographicSize = cameraDistanceType[cameraType];

        // プレイヤーへの参照を確保
        player = GameObject.FindGameObjectWithTag("Player");
        allCharacterList.Add(player); // プレイヤーをキャラ管理リストに追加

        // プレイヤーのイベントを購読
        player.GetComponent<PlayerController>().OnPlayerShootListener += () => PlayerIsShoot();
        player.GetComponent<PlayerController>().OnPlayerStopListener += () => PlayerIsStop();
        player.GetComponent<PlayerController>().OnPlayerDeathListener += () => isGameover = true;
        player.GetComponent<PlayerController>().OnPlayerTouchWallListener += pos => HitParticleControl(pos);

        // プレイヤースキン反映
        var skinID = DataManager.GetSelectedSkin();
        var newSprite = Resources.Load<Sprite>("Skin/Skin" + skinID);
        player.GetComponent<SpriteRenderer>().sprite = newSprite;

        // プレイヤー側のイベント購読を促す
        player.GetComponent<PlayerController>().SetDirectorEvent(gameObject.GetComponent<GameDirector>());

        // 敵をカウント
        var enemys = GameObject.FindGameObjectsWithTag("Enemy");
        enemyCount = enemys.Length;

        // 敵の撃破イベントを購読しつつ、リストに登録
        foreach (GameObject enemy in enemys) {
            enemy.GetComponent<Enemy>().OnDestroyEnemyListener += pos => {
                lastEnemyPos = pos;
                hitStopEffect = HitStop();
                StartCoroutine(hitStopEffect);
                enemyCount--;
            };

            enemy.GetComponent<Enemy>().OnDestroyEnemyBossListener += pos => {
                isBossKilled = true;
                lastEnemyPos = pos;
                hitStopEffect = HitStop();
                StartCoroutine(hitStopEffect);

                enemyCount--;
            };
            enemy.GetComponent<Enemy>().OnBloodParticleListener += pos => BloodParticleControl(pos);

            if (enemy.GetComponent<Enemy>().GetIsBoss()) {
                enemy.GetComponent<Enemy>().SetBossHealthBar(bossUI, bossHealthBar, bossHealthBarOnDamage);
            }

            allCharacterList.Add(enemy);
        }

        // フレンドリーをカウント
        var friendlys = GameObject.FindGameObjectsWithTag("Friendly");

        // フレンドリーのイベントを購読
        foreach (GameObject friend in friendlys) {
            friend.GetComponent<Friendly>().OnDestroyFriendlyListener += () => { isGameover = true;


            };
            friend.GetComponent<Friendly>().OnBloodParticleListener += pos => {
                isQueenKilled = true;
                //KilledQueen(pos);
                BloodParticleControl(pos);
            };
            allCharacterList.Add(friend);
        }

        // MovePanelを取得
        var movePanelOnStage = GameObject.FindGameObjectsWithTag("MovePanel");
        foreach (GameObject obj in movePanelOnStage) {
            movePanelList.Add(obj.GetComponent<MovePanel>());
        }
        // SpeedChangePanelを取得
        var speedChangePanelOnStage = GameObject.FindGameObjectsWithTag("SpeedChange");
        foreach (GameObject obj in speedChangePanelOnStage) {
            speedChangePanelList.Add(obj.GetComponent<SpeedChange>());
        }

        // ヘッダーカウントを更新
        RefreshHeaderStarCounter();

        // スケールサイズを調整 カメラを引いてる場合はキャラを1.8倍に
        if (cameraType == 1) {
            foreach (GameObject obj in allCharacterList) {
                obj.transform.localScale *= 1.6f;
            }
        }

        // プレイヤーの発射許可を出すが、ステージ１ならtutorialを先に出す
        if (SELECT_LEVEL == 1) {
            tutorialPanel.SetActive(true);
        } else {
            OnEnablePlayerShootMode?.Invoke(true);
        }
    }

    public void ChangeBossUI(bool flag) {
        bossUI.SetActive(flag);
    }

    public void CloseTutorial() {
        StartCoroutine(CloseTutorialRoutin());
    }

    public int GetEnemyCount() {
        return enemyCount;
    }

    public void PlaySlash() {
        //player.SetActive(false);
        finalAttackParticle.transform.position = lastEnemyPos;
        finalAttackParticle.GetComponent<ParticleSystem>().Play();
    }

    private void KilledQueen(Vector2 queenPos) {
        // プレイヤーの当たり判定消去（コライダー自体は残る）
        player.GetComponent<PlayerController>().SetGameEnd(true);


        // カメラを寄せる処理
        Vector3 pos = new Vector3(queenPos.x, queenPos.y, -10);

        Camera.main.transform.position = pos;

        Camera.main.GetComponent<Camera>().orthographicSize = 2f;

        lastEnemyPos = pos;
        hitStopEffect = HitStop();
        StartCoroutine(hitStopEffect);

    }

    public IEnumerator CloseTutorialRoutin() {
        tutorialPanel.SetActive(false);
        yield return new WaitForSeconds(0.5f);
        OnEnablePlayerShootMode?.Invoke(true);
    }

    /// <summary>
    /// ヒットストップを実現
    /// </summary>
    /// <returns></returns>
    IEnumerator HitStop() {
        Time.timeScale = 0.1f;
        yield return new WaitForSeconds(0.008f);
        Time.timeScale = 1;

        if (isClear || isQueenKilled) {
            isHitStop = true;

            //player.SetActive(false);
            //finalAttackParticle.transform.position = lastEnemyPos;
            //finalAttackParticle.GetComponent<ParticleSystem>().Play();
            //yield return new WaitForSeconds(0.1f);

            Time.timeScale = 0.2f;
            yield return new WaitForSeconds(0.4f);
            Time.timeScale = 1;

            // カメラ戻す
            Camera.main.transform.position = defaultCameraPos;
            Camera.main.GetComponent<Camera>().orthographicSize = lastCameraSize;


        }

        // ボスが死んだ！
        if (isBossKilled) {
            isBossKilled = false;
            var killedCoinEffect = Instantiate(coinSpawner);
            killedCoinEffect.transform.position = lastEnemyPos;
            killedCoinEffect.SetActive(true);
            yield return new WaitForSeconds(3f);
        }


        if (isClear || (isClear && isSkipMode)) {
            ShowResultPanel();
        }

        if (isQueenKilled) {
            isGameover = true;
        }

        isHitStop = false;

    }

    IEnumerator ZoomCamera() {
        yield return new WaitForSeconds(0.01f);
    }

    // Update is called once per frame
    void Update() {


        if (enemyCount <= 0 && !isClear && !isGameover) {

            // ゲーム部分UIを非表示
            headerUI.SetActive(false);

            isClear = true;

            // プレイヤーの当たり判定消去（コライダー自体は残る）
            player.GetComponent<PlayerController>().SetGameEnd(true);

            // カメラを寄せる処理
            var pos = lastEnemyPos;
            pos.z = -10;
            Camera.main.transform.position = pos;

            Camera.main.GetComponent<Camera>().orthographicSize = 2f;
            // ヒットストップで読み込む
            //ShowResultPanel();


        } else if (isGameover && !isClear && !isShowedGameOver) {
            // プレイヤーの当たり判定消去（コライダー自体は残る）
            player.GetComponent<PlayerController>().SetGameEnd(true);



            isShowedGameOver = true;
            // 黒マスクON
            resultBlackMask.SetActive(true);
            resultGameOverPanel.SetActive(true);
        }

    }

    private void HitParticleControl(Vector3 pos) {
        GameObject hitParticleObj = null;

        if (hitParticleList.Count >= 1) {
            foreach (GameObject obj in hitParticleList) {
                if (!obj.activeInHierarchy) { hitParticleObj = obj; break; }
            }
        }

        // 使えるのがなければ再利用
        if (hitParticleObj == null) {
            var newParticle = Instantiate(hitParticle);
            newParticle.transform.SetParent(gameObject.transform);
            hitParticleList.Add(newParticle);
            hitParticleObj = newParticle;
        }

        hitParticleObj.transform.position = pos;
        hitParticleObj.SetActive(true);
        hitParticleObj.GetComponent<ParticleSystem>().Play();
    }

    private void BloodParticleControl(Vector2 pos) {

        if (noBloodMode) { return; }

        GameObject blood = null;

        if (bloodParticleList.Count >= 1) {
            foreach (GameObject obj in bloodParticleList) {
                if (!obj.activeInHierarchy) { blood = obj; break; }
            }
        }

        // 使えるのがなければ再利用
        if (blood == null) {
            var newParticle = Instantiate(bloodParticle);
            newParticle.transform.SetParent(gameObject.transform);
            bloodParticleList.Add(newParticle);
            blood = newParticle;
        }

        blood.transform.position = pos;
        blood.SetActive(true);
        blood.GetComponent<ParticleSystem>().Play();

    }

    public void PlayerIsShoot() {
        //Debug.Log("プレイヤー発射されました");

        if (bladeCount <= 0) { return; }

        // ブレードを減らす
        var bladeCnt = headerBladeIcons.Count;
        headerBladeIcons[bladeCount - 1].SetActive(false);
        headerBladeIcons.RemoveAt(bladeCount - 1);
        shootCount++;
        bladeCount--;

        // Header更新
        RefreshHeaderStarCounter();

    }

    public void SkipStage() {

        var nowStage = SELECT_LEVEL.ToString();
        nowStage = nowStage.PadLeft(3, '0');

#if UNITY_EDITOR
        Time.timeScale = 1;
        isSkipMode = true;
        isClear = true;
        ShowResultPanel();
        enemyCount = 0;
#else
    if(AppLovin.IsIncentInterstitialReady()){
        TenjinManager.TenjinCustomEvent("USE SKIP STAGE " + nowStage);
        ADVideoType = ADtype.SKIP;
        AppLovin.ShowRewardedInterstitial();
    }
#endif

    }

    private void EndVideoSkip() {
        Time.timeScale = 1;
        isSkipMode = true;
        isClear = true;
        ShowResultPanel();
        enemyCount = 0;
    }

    private void ShowResultPanel() {

        StopAllCoroutines();

        // 黒マスクON
        resultBlackMask.SetActive(true);

        // 花吹雪スタート
        flowerParticle.SetActive(true);
        flowerParticle.GetComponent<ParticleSystem>().Play();

        // リザルトテキスト更新
        resultText.text = "STAGE " + SELECT_LEVEL + " CLEAR!";

        // 


        //GetComponent<AudioSource>().PlayOneShot(resultSoundEffect);

        // 星をセットする
        var bestBladeCount = myStageSetting.GetBestBladeCount();//理想のシュート数を取得
        //shootCount

        // 星３ならRoulette回せるようにしとく
        if (shootCount <= bestBladeCount && !isSkipMode) {
            nowStarCount = 3;
            resulltRouletteOpenButton.SetActive(true);
        } else {
            resulltRouletteOpenButton.SetActive(false);
            resultAdClearButton.SetActive(true);
        }

        //星3を最初にセット
        for (int i = 0; i < resultStarImages.Length; i++) {
            resultStarImages[i].GetComponent<Image>().sprite = starSprite;
        }

        //星2
        if (shootCount == bestBladeCount + 1) {
            nowStarCount = 2;
            resultStarImages[resultStarImages.Length - 1].GetComponent<Image>().sprite = noStarSprite;
        }
        //星1
        else if (shootCount >= bestBladeCount + 2) {
            nowStarCount = 1;
            resultStarImages[resultStarImages.Length - 1].GetComponent<Image>().sprite = noStarSprite;
            resultStarImages[resultStarImages.Length - 2].GetComponent<Image>().sprite = noStarSprite;
        } else if (isSkipMode) {
            nowStarCount = 0;
            resultStarImages[resultStarImages.Length - 1].GetComponent<Image>().sprite = noStarSprite;
            resultStarImages[resultStarImages.Length - 2].GetComponent<Image>().sprite = noStarSprite;
            resultStarImages[resultStarImages.Length - 3].GetComponent<Image>().sprite = noStarSprite;
        }

        // コインをゲット
        var addCoin = rewardCoinPerStar * nowStarCount;
        var nowCoin = DataManager.GetPlayerCoin();

        // ボスを初回撃破なら更にボーナス
        if (isBoss && !DataManager.GetStageClearCheck(SELECT_LEVEL)) {
            addCoin += bossKilledReward;
        }

        DataManager.SetPlayerCoin(nowCoin + addCoin);



        // クリアデータ書き込み
        if (!DataManager.GetStageClearCheck(SELECT_LEVEL)) {
            DataManager.SetStageClear(SELECT_LEVEL);

#if UNITY_EDITOR
#else
            var nowStage = SELECT_LEVEL.ToString();
            nowStage = nowStage.PadLeft(3, '0');
            TenjinManager.TenjinCustomEvent("CLEAR STAGE " + nowStage);
#endif

        }

        // 星の更新
        var clearStarData = DataManager.GetStageStar(SELECT_LEVEL); // 現在の星の数

        // 記録より星が多ければ更新
        if (nowStarCount > clearStarData) {
            DataManager.SetStageStar(SELECT_LEVEL, nowStarCount);
        }

        // Result反映
        if (isSkipMode) { resultCoinCounter.SetActive(false); }
        resultCoinCounter.GetComponentInChildren<TextMeshProUGUI>().text = "+" + addCoin;

        // 星獲得アニメーションを追加
        // まずは星を拡大
        for (int i = 0; i < resultStarImages.Length; i++) {
            resultStarImages[i].transform.localScale = Vector3.one * 3f;
            resultStarImages[i].GetComponent<Image>().DOFade(0, 0f);
        }

        if (DataManager.GetPlayerSettingSound()) {
            GetComponent<AudioSource>().PlayOneShot(resultSoundEffect);
        }


        // 獲得アニメを追加
        var starAnime = DOTween.Sequence();
        starAnime.AppendInterval(0.2f);

        //// 最初にラインが流れる演出を追加
        //starAnime.Append(resultLineParticle[cameraType].transform.DOMoveX(15, 2f).SetRelative());
        //starAnime.InsertCallback(0.3f, () => { resultStarImages[0].transform.DOScale(Vector3.one, 0.5f); })
        //        .Join(resultStarImages[0].GetComponent<Image>().DOFade(1, 0.5f));
        //starAnime.InsertCallback(0.5f, () => { resultStarImages[1].transform.DOScale(Vector3.one, 0.5f); })
        //        .Join(resultStarImages[1].GetComponent<Image>().DOFade(1, 0.5f));
        //starAnime.InsertCallback(0.7f, () => { resultStarImages[2].transform.DOScale(Vector3.one, 0.5f); })
        //        .Join(resultStarImages[2].GetComponent<Image>().DOFade(1, 0.5f));



        for (int i = 0; i < resultStarImages.Length; i++) {
            starAnime.Append(resultStarImages[i].transform.DOScale(Vector3.one, 0.5f))
                .Join(resultStarImages[i].GetComponent<Image>().DOFade(1, 0.5f));

        }



        if (!isSkipMode) {
            starAnime.InsertCallback(0.4f, () => {
                if (nowStarCount >= 1 && DataManager.GetPlayerSettingSound()) { GetComponent<AudioSource>().PlayOneShot(resultStarSoundEffect); }
            });
            starAnime.InsertCallback(0.8f, () => {
                if (nowStarCount >= 2 && DataManager.GetPlayerSettingSound()) { GetComponent<AudioSource>().PlayOneShot(resultStarSoundEffect); }
            });
            starAnime.InsertCallback(1.2f, () => {
                if (nowStarCount >= 3 && DataManager.GetPlayerSettingSound()) { GetComponent<AudioSource>().PlayOneShot(resultStarSoundEffect); }
            });

            // クラッカーやるならここをコメントアウト外す
            //starAnime.InsertCallback(0.8f, () => { resultCracker.Play(); });

        }
        starAnime.AppendInterval(0.2f);
        starAnime.OnComplete(() => {
            //GetComponent<AudioSource>().PlayOneShot(resultSoundEffect);
            resultSlideInMenu.transform.DOLocalMove(Vector3.zero, 0.5f);
        });

        clearCount++;

		// 最後のステージの場合、ネクストボタンを非表示にする
		var maxStage = Resources.LoadAll<GameObject>("Stage/");
		if(SELECT_LEVEL==maxStage.Length){
			//resultNextButton.SetActive(false);
            foreach(GameObject obj in resultFinalStageHiddenButtons) {
                obj.SetActive(false);
            }

            foreach(GameObject obj in resultFinalStageShowButtons) {
                obj.SetActive(true);
            }
		}

		resultClearPanel.SetActive(true);
        starAnime.Play();
    }

    public void OnClickClearByAD() {
        var nowStage = SELECT_LEVEL.ToString();
        nowStage = nowStage.PadLeft(3, '0');

#if UNITY_EDITOR
        ClearByAD();
#else
                if(AppLovin.IsIncentInterstitialReady()){
                    TenjinManager.TenjinCustomEvent("USE VIDEO STAR STAGE " + nowStage);
                    ADVideoType = ADtype.STAR3;
                    AppLovin.ShowRewardedInterstitial();
                }
#endif
    }

    /// <summary>
    /// 動画を見て星３にする勝ち方
    /// </summary>
    public void ClearByAD() {

        resultAdClearButton.transform.Find("Mask").gameObject.SetActive(true);
        resultAdClearButton.GetComponent<Button>().interactable = false;
        resultAdClearButton.GetComponent<Animator>().enabled = false;

        // nowStarCount が 今とったはずの星の数 0 or 1 or 2 が入る
        // 0 の時 0,1,2
        // 1 の時 1,2
        // 2 の時 2
        // 3 から 引いた数 3 , 2 , 1

        for (int i = 2; i >= nowStarCount; i--) {
            resultStarImages[i].GetComponent<Image>().sprite = starSprite;
        }

        // 星獲得アニメーションを追加
        // まずは星を拡大
        for (int i = nowStarCount; i < resultStarImages.Length; i++) {
            resultStarImages[i].transform.localScale = Vector3.one * 3f;
            resultStarImages[i].GetComponent<Image>().DOFade(0, 0f);
        }

        // 獲得アニメを追加
        var starAnime = DOTween.Sequence();

        for (int i = nowStarCount; i < resultStarImages.Length; i++) {
            starAnime.Append(resultStarImages[i].transform.DOScale(Vector3.one, 0.5f))
                .Join(resultStarImages[i].GetComponent<Image>().DOFade(1, 0.5f));
        }

        // 星をオーバーライド
        DataManager.SetStageStar(SELECT_LEVEL, 3);


        starAnime.Play();
    }

    private void RefreshHeaderStarCounter() {

        var bestBladeCount = myStageSetting.GetBestBladeCount();//理想のシュート数を取得

        starCounter.text = shootCount + "/" + bestBladeCount; // テキストを更新

        // ベスト本数より超えている場合
        if (shootCount > bestBladeCount) {
            // 星2個の場合
            if (shootCount == bestBladeCount + 1) {
                starImages[starImages.Length - 1].GetComponent<Image>().sprite = noStarSprite;
            } else if (shootCount >= bestBladeCount + 2) {
                starImages[starImages.Length - 1].GetComponent<Image>().sprite = noStarSprite;
                starImages[starImages.Length - 2].GetComponent<Image>().sprite = noStarSprite;
            }
        }

    }

    public void PlayerIsStop() {

        if (isClear || isGameover) { return; }

        Debug.Log("プレイヤー静止しました");

        // 刀が１本以上残ってればプレイヤーの発射許可を出す
        if (bladeCount >= 1) {

            // パネル最有効化
            foreach (SpeedChange obj in speedChangePanelList) {
                obj.ChangeEnable(true);
            }
            foreach (MovePanel obj in movePanelList) {
                obj.ChangeEnable(true);
            }

            OnEnablePlayerShootMode?.Invoke(true);
        } else {
            isGameover = true;
        }

    }

    public void RefreshScene() {
        var nowStage = SELECT_LEVEL.ToString();
        nowStage = nowStage.PadLeft(3, '0');

#if UNITY_EDITOR
#else
        TenjinManager.TenjinCustomEvent("RETRY STAGE " + nowStage);
#endif

        retryCount++;
        Time.timeScale = 1;

        if (retryCount >= retryADCount) {
            retryCount = 0;

#if UNITY_EDITOR
#else
            if(AppLovin.HasPreloadedInterstitial()){
            // 再生
            AppLovin.ShowInterstitial();
        }
#endif
            FadeManager.FadeOut(SceneManager.GetActiveScene().buildIndex);

        } else {
            FadeManager.FadeOut(SceneManager.GetActiveScene().buildIndex);
        }
    }

    public void BackToStart() {
        Time.timeScale = 1;
        FadeManager.FadeOut(0);
    }

    public void DebugLoad(int num) {
        SELECT_LEVEL = num;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void NextLevel() {
        Time.timeScale = 1;
        var stageCount = Resources.LoadAll<GameObject>("Stage/");

        if (SELECT_LEVEL + 1 <= stageCount.Length) {
            SELECT_LEVEL += 1;
        }

        if (clearCount >= clearADCount) {
            clearCount = 0;
#if UNITY_EDITOR
#else
                        if(AppLovin.HasPreloadedInterstitial()){
                        // 再生
                        AppLovin.ShowInterstitial();
                    }
#endif
            FadeManager.FadeOut(SceneManager.GetActiveScene().buildIndex);

        } else {
            FadeManager.FadeOut(SceneManager.GetActiveScene().buildIndex);
        }


    }

    void onAppLovinEventReceived(string ev) {
        if (ev.Contains("REWARDAPPROVEDINFO")) {
            //リワード動画を見終わった報酬を与える処理 
        } else if (ev.Contains("LOADEDREWARDED")) {
            // リワード動画がロードできたときの処理.
        } else if (ev.Contains("LOADREWARDEDFAILED")) {
            // リワード動画がロードできなかったときの処理
        } else if (ev.Contains("HIDDENREWARDED")) {
            // リワード動画が閉じられたとき（次の動画の準備などをすると良い）
            AppLovin.LoadRewardedInterstitial();

            if (ADVideoType == ADtype.ROULETTE) {
                RouletteADEndListener?.Invoke();
            } else if (ADVideoType == ADtype.SKIP) {
                EndVideoSkip();
            } else if (ADVideoType == ADtype.STAR3) {
                ClearByAD();
            }
        }
    }


}
