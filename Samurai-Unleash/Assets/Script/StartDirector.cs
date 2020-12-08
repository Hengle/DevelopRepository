using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using DG.Tweening;
using TMPro;

public class StartDirector : MonoBehaviour {

    private string appLovinSdkKey = "TJ2lRrOfe1NQU0rKdce0GiUHc-PsV4WPMsKUmbGuwpCEqTqaCuxx10f4m2yWmAOQsHf0hXg-Aoxnw7okFkqM-a";

    [SerializeField] AudioClip startTaikoSoundEffect;

    [SerializeField] GameObject gameTitleLogo;
    [SerializeField] GameObject startButton;
    [SerializeField] GameObject headerMenus;

    [SerializeField] GameObject stageSelectButtonPref;
    [SerializeField] GameObject targetToStageSelectList;

    [SerializeField] GameObject skinButtonPref;
    [SerializeField] GameObject targetToSkinList;

    [SerializeField] GameObject levelPanel;
    [SerializeField] GameObject skinPanel;
    [SerializeField] GameObject optionPanel;
    [SerializeField] GameObject headerPanel;

    [SerializeField] TextMeshProUGUI noBloodText;

    public bool isActiveLevelPanel;
    public bool isActiveSkinPanel;
    public bool isActiveOptionPanel;
    SkinData SkinList;

    // Start is called before the first frame update
    void Awake() {

        Application.targetFrameRate = 60; //60FPSに設定
        TenjinManager.ConnectTenjin();

#if UNITY_EDITOR
#else

        AppLovin.InitializeSdk();
        AppLovin.SetSdkKey(appLovinSdkKey);
        AppLovin.HideAd();
#endif
        FadeManager.FadeIn();
    }

    private void Start() {

        // タイトルアニメーション
        // 初期化
        gameTitleLogo.transform.DOLocalMove(new Vector3(0,4000,0),0);
        startButton.transform.DOLocalMove(new Vector3(0, 4000, 0), 0);
        headerMenus.transform.DOLocalMove(new Vector3(0, -4000, 0), 0);

        // 移動させる
        var anm = DOTween.Sequence();
        anm.Append(gameTitleLogo.transform.DOLocalMove(new Vector3(0,0,0),0.5f));
        anm.Join(startButton.transform.DOLocalMove(new Vector3(0, -520, 0), 0.5f));
        anm.Join(headerMenus.transform.DOLocalMove(new Vector3(0, 25, 0), 0.5f));
        anm.Play();

        SkinList = Resources.Load<SkinData>("Skin/SkinData");

        SkinUnlockCheck();
        CreateStageList();
        CreateSkinList();
    }

    private void CreateStageList() {

        var stageCount = Resources.LoadAll<GameObject>("Stage/");

        for (int i = 1; i <= stageCount.Length; i++) {
            var newBtn = Instantiate(stageSelectButtonPref);
            newBtn.GetComponent<StageSelectButton>().SetStatus(i);
            newBtn.transform.SetParent(targetToStageSelectList.transform, false);
        }

    }

    public void EnableLevelPanel(bool flag) {

        if (flag) {
            levelPanel.transform.DOLocalMove(Vector3.zero, 0.5f);
        } else {
            levelPanel.transform.DOLocalMove(new Vector3(2500, 0, 0), 0.5f);
        }

    }

    public void ChangeTabPanel(int num) {
        // 0 1 2 と左から順番

        // とりあえず開く動作から作成
        if (num == 0) {

            // 他がアクティブでないか
            if (!isActiveLevelPanel && !isActiveOptionPanel) {

                if (!isActiveSkinPanel) {
                    isActiveSkinPanel = true;
                    skinPanel.transform.DOLocalMove(new Vector3(0, 0, 0), 0.5f);
                    headerPanel.transform.DOLocalMove(new Vector3(0, 3300, 0), 0.5f);
                } else {
                    isActiveSkinPanel = false;
                    skinPanel.transform.DOLocalMove(new Vector3(0, -3300, 0), 0.5f);
                    headerPanel.transform.DOLocalMove(new Vector3(0, 25, 0), 0.5f);
                }

            } else if (isActiveLevelPanel) {

                // レベルパネルが開かれてるから、パネルを閉じてから自分を開く
                isActiveLevelPanel = false;
                isActiveSkinPanel = true;

                var seq = DOTween.Sequence();
                seq.Append(levelPanel.transform.DOLocalMove(new Vector3(0, -3300, 0), 0.5f))
                    .Join(headerPanel.transform.DOLocalMove(new Vector3(0, 25, 0), 0.5f));
                seq.Append(skinPanel.transform.DOLocalMove(new Vector3(0, 0, 0), 0.5f))
                    .Join(headerPanel.transform.DOLocalMove(new Vector3(0, 3300, 0), 0.5f));

            } else if (isActiveOptionPanel) {

                // オプションパネルが開かれてるから、閉じてから自分を開く
                isActiveOptionPanel = false;
                isActiveSkinPanel = true;

                var seq = DOTween.Sequence();
                seq.Append(optionPanel.transform.DOLocalMove(new Vector3(0, -3300, 0), 0.5f))
                    .Join(headerPanel.transform.DOLocalMove(new Vector3(0, 25, 0), 0.5f));
                seq.Append(skinPanel.transform.DOLocalMove(new Vector3(0, 0, 0), 0.5f))
                    .Join(headerPanel.transform.DOLocalMove(new Vector3(0, 3300, 0), 0.5f));
            }


        } else if (num == 1) {

            // 他がアクティブでないか
            if (!isActiveSkinPanel && !isActiveLevelPanel) {

                if (!isActiveOptionPanel) {
                    isActiveOptionPanel = true;
                    optionPanel.transform.DOLocalMove(new Vector3(0, 0, 0), 0.5f);
                    headerPanel.transform.DOLocalMove(new Vector3(0, 3300, 0), 0.5f);
                } else {
                    isActiveOptionPanel = false;
                    optionPanel.transform.DOLocalMove(new Vector3(0, -3300, 0), 0.5f);
                    headerPanel.transform.DOLocalMove(new Vector3(0, 25, 0), 0.5f);
                }

            } else if (isActiveSkinPanel) {

                // スキンパネルが開かれてるから、スキンパネルを閉じてから自分を開く
                isActiveSkinPanel = false;
                isActiveOptionPanel = true;

                var seq = DOTween.Sequence();
                seq.Append(skinPanel.transform.DOLocalMove(new Vector3(0, -3300, 0), 0.5f))
                    .Join(headerPanel.transform.DOLocalMove(new Vector3(0, 25, 0), 0.5f));
                seq.Append(optionPanel.transform.DOLocalMove(new Vector3(0, 0, 0), 0.5f))
                    .Join(headerPanel.transform.DOLocalMove(new Vector3(0, 3300, 0), 0.5f));

            } else if (isActiveLevelPanel) {

                // オプションパネルが開かれてるから、閉じてから自分を開く
                isActiveLevelPanel = false;
                isActiveOptionPanel = true;

                var seq = DOTween.Sequence();
                seq.Append(levelPanel.transform.DOLocalMove(new Vector3(0, -3300, 0), 0.5f))
                    .Join(headerPanel.transform.DOLocalMove(new Vector3(0, 25, 0), 0.5f));
                seq.Append(optionPanel.transform.DOLocalMove(new Vector3(0, 0, 0), 0.5f))
                    .Join(headerPanel.transform.DOLocalMove(new Vector3(0, 3300, 0), 0.5f));
            }

        } else if (num == 2) {

            // 他がアクティブでないか
            if (!isActiveSkinPanel && !isActiveOptionPanel) {

                if (!isActiveLevelPanel) {
                    isActiveLevelPanel = true;
                    levelPanel.transform.DOLocalMove(new Vector3(0, 0, 0), 0.5f);
                    headerPanel.transform.DOLocalMove(new Vector3(0, 3300, 0), 0.5f);
                } else {
                    isActiveLevelPanel = false;
                    levelPanel.transform.DOLocalMove(new Vector3(0, -3300, 0), 0.5f);
                    headerPanel.transform.DOLocalMove(new Vector3(0, 25, 0), 0.5f);
                }

            } else if (isActiveSkinPanel) {

                // スキンパネルが開かれてるから、スキンパネルを閉じてから自分を開く
                isActiveSkinPanel = false;
                isActiveLevelPanel = true;

                var seq = DOTween.Sequence();
                seq.Append(skinPanel.transform.DOLocalMove(new Vector3(0, -3300, 0), 0.5f))
                    .Join(headerPanel.transform.DOLocalMove(new Vector3(0, 25, 0), 0.5f));
                seq.Append(levelPanel.transform.DOLocalMove(new Vector3(0, 0, 0), 0.5f))
                    .Join(headerPanel.transform.DOLocalMove(new Vector3(0, 3300, 0), 0.5f));

            } else if (isActiveOptionPanel) {

                // オプションパネルが開かれてるから、閉じてから自分を開く
                isActiveOptionPanel = false;
                isActiveLevelPanel = true;

                var seq = DOTween.Sequence();
                seq.Append(optionPanel.transform.DOLocalMove(new Vector3(0, -3300, 0), 0.5f))
                    .Join(headerPanel.transform.DOLocalMove(new Vector3(0, 25, 0), 0.5f));
                seq.Append(levelPanel.transform.DOLocalMove(new Vector3(0, 0, 0), 0.5f))
                    .Join(headerPanel.transform.DOLocalMove(new Vector3(0, 3300, 0), 0.5f));
            }

        }
    }

    public void AllClose() {
        var seq = DOTween.Sequence();
        seq.Append(headerPanel.transform.DOLocalMove(new Vector3(0, 25, 0), 0.5f));

        if (isActiveSkinPanel) {
            isActiveSkinPanel = false;
            seq.Join(skinPanel.transform.DOLocalMove(new Vector3(0, -3300, 0), 0.5f));
        }
        if (isActiveOptionPanel) {
            isActiveOptionPanel = false;
            seq.Join(optionPanel.transform.DOLocalMove(new Vector3(0, -3300, 0), 0.5f));
        }
        if (isActiveLevelPanel) {
            isActiveLevelPanel = false;
            seq.Join(levelPanel.transform.DOLocalMove(new Vector3(0, -3300, 0), 0.5f));
        }

        seq.Play();
    }

    public void StartGame() {

        StartSound();

        // ボタン類を移動させる
        var anm = DOTween.Sequence();
        anm.Append(gameTitleLogo.transform.DOLocalMove(new Vector3(0, 4000, 0), 0.5f));
        anm.Join(startButton.transform.DOLocalMove(new Vector3(0, 4000, 0), 0.5f));
        anm.Join(headerMenus.transform.DOLocalMove(new Vector3(0, -4000, 0), 0.5f));
        anm.Play();


        var maxStage = Resources.LoadAll<GameObject>("Stage/");
        var continueStage = DataManager.CheckContinueStage(maxStage.Length);
        GameDirector.SELECT_LEVEL = continueStage;

        FadeManager.FadeOut(1);
    }

    public void StartSound() {
        if (DataManager.GetPlayerSettingSound()) {
            GetComponent<AudioSource>().PlayOneShot(startTaikoSoundEffect);

        }
    }

    public void LevelWithStartGame(int num) {
        if (DataManager.GetPlayerSettingSound()) {
            GetComponent<AudioSource>().PlayOneShot(startTaikoSoundEffect);

        }

        GameDirector.SELECT_LEVEL = num;
        FadeManager.FadeOut(1);

    }

    public void CreateSkinList() {
        var SkinMaster = SkinList.SkinMaster;
        bool isUnlock = false;
        //Debug.Log(SkinMaster.Count);

        for (int i = 1; i <= SkinMaster.Count; i++) {

            bool isCoin = false;
            string itemStr = SkinMaster[i - 1].GetItemString();
            int itemPrice = SkinMaster[i - 1].GetUnlockValue();
            // そのスキンはアンロックされているべきものか？
            if (!DataManager.GetSkinUnlock(i) && SkinMaster[i - 1].GetTypeOfUnlockItem() == Skin.TypeOfUnlockItem.Unlocked) {
                DataManager.SetSkinUnlock(i);
            }

            // コインアンロックならフラグ立て
            if (SkinMaster[i - 1].GetTypeOfUnlockItem() == Skin.TypeOfUnlockItem.Coin) {
                isCoin = true;
            }

            // ボタン作成
            var newObj = Instantiate(skinButtonPref);
            newObj.transform.SetParent(targetToSkinList.transform, false);
            newObj.name = "SkinButton" + i;

            // アンロック済みかチェック
            isUnlock = DataManager.GetSkinUnlock(i) ? true : false;

            // 選択されてるかチェック
            if (isUnlock) {
                itemStr = DataManager.GetSelectedSkin() == i ? "USED" : "USE";
            }

            newObj.GetComponent<SkinButton>().SetStatus(i, isUnlock, isCoin, itemPrice, itemStr);
        }


    }

    public void SkinUnlockCheck() {
        var SkinMaster = SkinList.SkinMaster;
        for (int i = 1; i <= SkinMaster.Count; i++) {

            // プレイカウントによるアンロック
            if(SkinMaster[i-1].GetTypeOfUnlockItem() == Skin.TypeOfUnlockItem.PlayCount) {

                // プレイ回数が設定値を超えてたらアンロック
                if(DataManager.GetPlayCount() >= SkinMaster[i - 1].GetUnlockValue()) {
                    DataManager.SetSkinUnlock(i);
                }
            }
        }
    }

    int tapCountBloodMode = 0;
    public void DebugNoBloodMode() {
        tapCountBloodMode++;
        if(tapCountBloodMode >= 5) {
            tapCountBloodMode = 0;
            GameDirector.noBloodMode = GameDirector.noBloodMode ? false : true;
            noBloodText.DOFade(1, 0);
            noBloodText.text = "<color=yellow>[DEBUG] NO BLOOD : " + GameDirector.noBloodMode  + "</color>";
            noBloodText.DOFade(0, 5);
        }
    }

}
