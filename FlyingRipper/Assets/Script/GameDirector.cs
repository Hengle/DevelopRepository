using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using VisualEffects;
using UnityEngine.SceneManagement;
using TMPro;
using System;

public class GameDirector : MonoBehaviour {

    public static int SELECT_LEVEL = 1; // 選択中のレベル
    public static bool isGameMode;
    public static bool debugCamera;

    [SerializeField] ChapterMaster chapterMaster;
    [SerializeField] GameObject clearResultUI;

    [SerializeField] GameObject tapGuide;
    [SerializeField] GameObject gameUIHeader;
    [SerializeField] GameObject startUIHeader;
    [SerializeField] GameObject startUISide;
    [SerializeField] GameObject settingButton;
    [SerializeField] GameObject startUIBottom;

    [SerializeField] GameObject progressIconPrefab;
    [SerializeField] GameObject progressPrevText;
    [SerializeField] GameObject progressNextText;
    [SerializeField] GameObject progressTarget;

    [SerializeField] Image bossImage;

    [SerializeField] GameObject resultHumanIconPrefab;
    [SerializeField] Transform resultIconTarget;

    [SerializeField] ParticleSystem hitEffect;
    [SerializeField] ParticleSystem getEffect;
    [SerializeField] GameObject cameraObject;
    [SerializeField] GameObject player;
    [SerializeField] List<GameObject> goalObject = new List<GameObject>();

    public int nowCount = 0;
    public int bossCount = 0;
    int maxCount = 0;
    public int friendKill = 0;
    PlayerController playerController;
    Vector3 offset;
    bool isTargetChange;
    bool isGameClear;
    bool isGameActive;
    bool isBossStage;
    bool isOpenSetting;

    private void Awake() {
        SetResolution(1280); // 反映はフレーム更新後
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 60;
    }

    void Start() {

        // 参照取得
        playerController = player.GetComponent<PlayerController>();

        // イベント取得
        playerController.OnDeath += () => { Reload(); };
        playerController.OnTapped += () => { tapGuide.SetActive(false); };

        // ステージの読み込み
        LoadLevelData();

        // プログレスバー初期化
        RefreshProgress();

        // 初期のカメラ位置調整
        CameraFollow();

        // ウィンドウ類初期化
        gameUIHeader.transform.DOLocalMove(new Vector3(0, 2000, 0), 0f).SetRelative();
        startUIHeader.transform.DOLocalMove(new Vector3(0, 2000, 0), 0f).SetRelative();
        startUISide.transform.DOLocalMove(new Vector3(-2000, 0, 0), 0f).SetRelative();
        startUIBottom.transform.DOLocalMove(new Vector3(0, -2000, 0), 0f).SetRelative();
        tapGuide.SetActive(false);

        // ゲームモードなら開始
        if (isGameMode) {
            StartUI(false, null);
            GameStart();
        } else {
            StartUI(true, null);
        }

    }

    void Update() {
        if (!isGameMode || !isGameActive) { return; }
        CameraFollow();
    }

    public void PlayButton() {
        StartUI(false, null);
        GameStart();
    }

    private void StartUI(bool open, Action callback) {
        CameraFollow();

        if (open) {
            gameUIHeader.SetActive(false);

            var startAnimeUI = DOTween.Sequence();
            startAnimeUI
                .Append(startUIHeader.transform.DOLocalMove(Vector3.zero, 0.3f))
                .Join(startUISide.transform.DOLocalMove(Vector3.zero, 0.3f))
                .Join(startUIBottom.transform.DOLocalMove(Vector3.zero, 0.3f));
            startAnimeUI.Play();

        } else {
            gameUIHeader.SetActive(true);

            var startAnimeUI = DOTween.Sequence();
            startAnimeUI
                .Append(startUIHeader.transform.DOLocalMove(new Vector3(0, 2000, 0), 0.1f).SetRelative())
                .Join(startUISide.transform.DOLocalMove(new Vector3(-2000, 0, 0), 0.1f).SetRelative())
                .Join(startUIBottom.transform.DOLocalMove(new Vector3(0, -2000, 0), 0.1f).SetRelative())
                .Append(gameUIHeader.transform.DOLocalMove(Vector3.zero, 0.3f))
                .OnComplete(() => {

                    callback?.Invoke();

                });
            startAnimeUI.Play();

        }
    }

    private void LoadLevelData() {

        // 選択中のレベルデータをロード
        var maxLevel = Resources.LoadAll<GameObject>("Stage");
        SELECT_LEVEL = SELECT_LEVEL > maxLevel.Length ? maxLevel.Length : SELECT_LEVEL;

        //var _stagePrefab = Resources.Load<GameObject>("Stage/Stage" + SELECT_LEVEL); // プレハブ読み込み

        // ChapterMasterからステージを取得
        var _stagePrefab = chapterMaster.GetSelectStageData(SELECT_LEVEL);
        var stageObject = Instantiate(_stagePrefab); // ステージの生成
        var myStageSetting = stageObject.GetComponent<StageSetting>(); // ステージ設定の参照

        // ボスステージの読み込み
        isBossStage = myStageSetting.GetStageIsBoss();

        // ボスステージなら画像読み込み
        bossImage.gameObject.SetActive(isBossStage);
        if (isBossStage) {
            bossImage.sprite = myStageSetting.GetBossImage();
        }

        // エネミーデータの読みこみ
        goalObject = myStageSetting.GetStageEnemy();

        // 敵総数を取得
        maxCount = goalObject.Count;

        // プレイヤー位置の読み込み
        var _spawnPos = myStageSetting.GetPlayerSpawnPos();

        // プレイヤーの位置修正
        player.transform.position = _spawnPos;

        // カメラコンテナの位置調整
        cameraObject.transform.position = _spawnPos;

        // エネミー討伐イベントの購読と設定
        foreach (GameObject obj in goalObject) {
            var enemySC = obj.GetComponent<Enemy>();

            // ヒットエフェクト用
            enemySC.OnHitEffect += (pos) => {
                var newPos = pos.position;
                newPos.y -= 0.1f;
                EmittingHitEffect(newPos);
            };

            // 敵を殺した
            enemySC.OnDestoyedEnemy += (killedObj, boss) => {
                goalObject.Remove(killedObj);

                if (boss) {
                    bossCount++;
                } else {
                    nowCount++;
                }

                if (0 == goalObject.Count) {
                    // スローモー、カメラ切り替え
                    var camera = obj.GetComponent<Enemy>().GetEnemyCamera();
                    ChangeEnemyCamera(camera);

                    isGameClear = true;
                    Invoke("ShowResult", 2f);
                }

            };
        }

        // 通常エネミーのイベントも購読
        var enemyList = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (GameObject obj in enemyList) {
            if (goalObject.Contains(obj)) { continue; }
            obj.GetComponent<Enemy>().OnDestoyedEnemy += (kikkedObj, boss) => nowCount++;

            // ヒットエフェクト用
            obj.GetComponent<Enemy>().OnHitEffect += (pos) => {
                var newPos = pos.position;
                newPos.y -= 0.1f;
                EmittingHitEffect(newPos);
            };
        }

        // 民間人殺害を購読
        var friendList = GameObject.FindGameObjectsWithTag("Friend");
        foreach (GameObject obj in friendList) {
            obj.GetComponent<Enemy>().OnFriendlyFire += () => friendKill++;
            // ヒットエフェクト用
            obj.GetComponent<Enemy>().OnHitEffect += (pos) => {
                var newPos = pos.position;
                newPos.y -= 0.1f;
                EmittingHitEffect(newPos);
            };
        }

        // コインオブジェクトのイベント購読
        var coinData = myStageSetting.GetStageCoin();
        if (coinData.Count >= 1) {
            foreach (GameObject obj in coinData) {
                obj.GetComponent<Coin>().GetCoin += pos => { EmittingGetItemEffect(pos); };
            }
        }

    }

    private void RefreshProgress() {
        Debug.Log("選択されたChapterは" + (chapterMaster.GetChapterCount() + 1) + "です");
        Debug.Log("選択されたChapterのステージ数は" + chapterMaster.GetLoadedStageChapterPhaseCount() + "です");
        Debug.Log("選択されたChapterでの私のフェーズは" + chapterMaster.GetLoadedStageMyPhaseCount() + "です");

        progressPrevText.GetComponentInChildren<TextMeshProUGUI>().text = (chapterMaster.GetChapterCount() + 1).ToString();
        progressNextText.GetComponentInChildren<TextMeshProUGUI>().text = (chapterMaster.GetChapterCount() + 2).ToString();

        for (int i = 0; i < chapterMaster.GetLoadedStageChapterPhaseCount(); i++) {
            var newObj = Instantiate(progressIconPrefab);
            newObj.transform.SetParent(progressTarget.transform, false);
            //newObj.GetComponent<StageProgressIcon>().SetStatus(chapterMaster.GetLoadedStageMyPhaseCount() >= i ? true : false);

            if (chapterMaster.GetLoadedStageMyPhaseCount() == i) {
                newObj.GetComponent<StageProgressIcon>().SetIconStatus(StageProgressIcon.IconStatus.TRY);
            } else if (chapterMaster.GetLoadedStageMyPhaseCount() <= i) {
                newObj.GetComponent<StageProgressIcon>().SetIconStatus(StageProgressIcon.IconStatus.NOTCLEAR);
            } else {
                newObj.GetComponent<StageProgressIcon>().SetIconStatus(StageProgressIcon.IconStatus.CLEARED);
            }

        }

    }

    private void BossAttention(Action callback) {
        var bossAnime = DOTween.Sequence();

        bossAnime
            .Append(bossImage.gameObject.transform.DORotate(new Vector3(0, 0, -720), 1f).SetRelative())
            .Join(bossImage.gameObject.transform.DOLocalMove(Vector3.zero, 1f))
            .OnComplete(() => {
                bossImage.gameObject.GetComponent<BossImageDissolve>().Active(callback);
            });
    }

    private void GameStart() {
        // プレイヤーの有効化
        if (isBossStage) {
            BossAttention(() => PlayerActive());
        } else {
            PlayerActive();
        }
    }

    private void PlayerActive() {
        isGameActive = true;
        isGameMode = true;
        playerController.SwitchActive(true);
        CameraFollow();
        tapGuide.SetActive(true);
    }

    private void ShowResult() {
        if (!isGameClear) { return; }
        playerController.SwitchActive(false);

        clearResultUI.SetActive(true);

        // リザルトアイコン追加処理
        List<GameObject> resultIcon = new List<GameObject>();

        // ボスアイコン生成
        if (bossCount >= 1) {
            for (int i = 0; i < bossCount; i++) {
                var newObj = Instantiate(resultHumanIconPrefab);
                newObj.transform.SetParent(resultIconTarget, false);
                newObj.GetComponent<ResultHumanIcon>().SetColor(ResultHumanIcon.Type.BOSS);
                newObj.GetComponent<Image>().DOFade(0, 0);
                resultIcon.Add(newObj);
            }
        }

        // エネミーアイコン生成
        if (nowCount >= 1) {
            for (int i = 0; i < nowCount; i++) {
                var newObj = Instantiate(resultHumanIconPrefab);
                newObj.transform.SetParent(resultIconTarget, false);
                newObj.GetComponent<ResultHumanIcon>().SetColor(ResultHumanIcon.Type.ENEMY);
                newObj.GetComponent<Image>().DOFade(0, 0);
                resultIcon.Add(newObj);
            }
        }

        // 民間人アイコン生成
        if (friendKill >= 1) {
            for (int i = 0; i < friendKill; i++) {
                var newObj = Instantiate(resultHumanIconPrefab);
                newObj.transform.SetParent(resultIconTarget, false);
                newObj.GetComponent<ResultHumanIcon>().SetColor(ResultHumanIcon.Type.FRIEND);
                newObj.GetComponent<Image>().DOFade(0, 0);
                resultIcon.Add(newObj);
            }
        }

        var resultAnime = DOTween.Sequence();
        foreach (GameObject obj in resultIcon) {
            resultAnime.OnStart(() => { obj.transform.DOScale(Vector3.one * 2f, 0); });
            resultAnime.Append(obj.GetComponent<Image>().DOFade(1, 0.1f))
                .Join(obj.transform.DOScale(Vector3.one, 0.1f));
        }

        resultAnime.Play();
    }

    private void ChangeEnemyCamera(Camera target) {
        target.gameObject.SetActive(true);
        cameraObject.SetActive(false);
    }

    private void ChangeMainCamera() {
        cameraObject.SetActive(true);
    }

    private void CameraFollow() {
        cameraObject.transform.position = player.transform.position;

        if (0 == goalObject.Count) { return; }

        // デバッグモード：カメラ追従をオフ
        if (debugCamera) { return; }

        // 自分の回転、相手までの角度、ｔ＝時間
        var vecA = (goalObject[0].transform.position - cameraObject.transform.position).normalized; // カメラからターゲットまでの向きベクトル
        cameraObject.transform.rotation = Quaternion.Slerp(cameraObject.transform.rotation, Quaternion.LookRotation(vecA), 0.125f);

    }

    private void EmittingGetItemEffect(Vector3 pos) {
        getEffect.gameObject.transform.position = pos;
        getEffect.Play();
    }

    private void EmittingHitEffect(Vector3 pos) {
        hitEffect.gameObject.transform.position = pos;
        hitEffect.Play();
    }

    public void SwitchSettingButton() {
        isOpenSetting = !isOpenSetting;

        if (isOpenSetting) {
            settingButton.GetComponent<RectTransform>().DOAnchorPos(new Vector2(25, settingButton.GetComponent<RectTransform>().anchoredPosition.y), 0.25f);
        } else {
            settingButton.GetComponent<RectTransform>().DOAnchorPos(new Vector2(-100, settingButton.GetComponent<RectTransform>().anchoredPosition.y), 0.25f);
        }
    }

    public void Reload() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void NextStage() {
        isGameMode = false;
        var maxLevel = Resources.LoadAll<GameObject>("Stage");
        SELECT_LEVEL = SELECT_LEVEL > maxLevel.Length ? maxLevel.Length : SELECT_LEVEL + 1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void DebugReload(int no) {
        SELECT_LEVEL = no;
        isGameMode = true;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void DebugRestart() {
        isGameMode = false;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    void SetResolution(float baseResolution) {
        float screenRate = baseResolution / Screen.height;
        if (screenRate > 1) screenRate = 1;
        int width = (int)(Screen.width * screenRate);
        int height = (int)(Screen.height * screenRate);

        Screen.SetResolution(width, height, true);
    }
}
