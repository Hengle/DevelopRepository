using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LionStudios;

public class LionManager : MonoBehaviour {

    private static LionManager myInstance;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void InitializeBeforeSceneLoad() {
        var dataManager = new GameObject("LionManager", typeof(LionManager));
        myInstance = dataManager.GetComponent<LionManager>();
        myInstance.InitializeSDK();
        DontDestroyOnLoad(dataManager);
    }

    public static LionManager Instance {
        get {
            return myInstance;
        }
        set { }
    }

    private void InitializeSDK() {
        LionKit.Initialize();
    }

    /// <summary>
    /// レベルの開始
    /// </summary>
    /// <param name="_level">レベル（string） 例えば 1-4 とか</param>
    /// <param name="_score">スコア（なければ省略可…初期値-1）</param>
    public void LevelStarted(string _level, int _score = -1) {
        Analytics.Events.LevelStarted(_level, _score);
    }

    /// <summary>
    /// レベルの開始
    /// </summary>
    /// <param name="_level">レベル（int） 例えば 1 とか</param>
    /// <param name="_score">スコア（なければ省略可…初期値-1）</param>
    public void LevelStarted(int _level, int _score = -1) {
        Analytics.Events.LevelStarted(_level, _score);
    }

    /// <summary>
    /// レベルのリスタート
    /// </summary>
    /// <param name="_level">レベル（string） 例えば 1-4 とか</param>
    /// <param name="_score">スコア（なければ省略可…初期値-1）</param>
    public void LevelRestart(string _level, int _score = -1) {
        Analytics.Events.LevelRestart(_level, _score);
    }

    /// <summary>
    /// レベルのリスタート
    /// </summary>
    /// <param name="_level">レベル（int） 例えば 1 とか</param>
    /// <param name="_score">スコア（なければ省略可…初期値-1）</param>
    public void LevelRestart(int _level, int _score = -1) {
        Analytics.Events.LevelRestart(_level, _score);
    }

    /// <summary>
    /// レベルのスキップ
    /// </summary>
    /// <param name="_level">レベル（string） 例えば 1-4 とか</param>
    /// <param name="_score">スコア（なければ省略可…初期値-1）</param>
    public void LevelSkipped(string _level, int _score = -1) {
        Analytics.Events.LevelSkipped(_level, _score);
    }

    /// <summary>
    /// レベルのスキップ
    /// </summary>
    /// <param name="_level">レベル（int） 例えば 1 とか</param>
    /// <param name="_score">スコア（なければ省略可…初期値-1）</param>
    public void LevelSkipped(int _level, int _score = -1) {
        Analytics.Events.LevelSkipped(_level, _score);
    }

    /// <summary>
    /// レベルの失敗
    /// </summary>
    /// <param name="_level">レベル（string） 例えば 1-4 とか</param>
    /// <param name="_score">スコア（なければ省略可…初期値-1）</param>
    public void LevelFailed(string _level, int _score = -1) {
        Analytics.Events.LevelFailed(_level, _score);
    }

    /// <summary>
    /// レベルの失敗
    /// </summary>
    /// <param name="_level">レベル（int） 例えば 1 とか</param>
    /// <param name="_score">スコア（なければ省略可…初期値-1）</param>
    public void LevelFailed(int _level, int _score = -1) {
        Analytics.Events.LevelFailed(_level, _score);
    }

    /// <summary>
    /// レベルの完了（複数工程のあるゲームなら 1-1 ＞ 1-2 のようにそれぞれのステップで記録）
    /// </summary>
    /// <param name="_level">レベル（string） 例えば 1-4 とか</param>
    /// <param name="_score">スコア（なければ省略可…初期値-1）</param>
    public void LevelComplete(string _level, int _score = -1) {
        Analytics.Events.LevelComplete(_level, _score);
    }

    /// <summary>
    /// レベルの完了（複数工程のあるゲームなら 1-1 ＞ 1-2 のようにそれぞれのステップで記録）
    /// </summary>
    /// <param name="_level">レベル（int） 例えば 1 とか</param>
    /// <param name="_score">スコア（なければ省略可…初期値-1）</param>
    public void LevelComplete(int _level, int _score = -1) {
        Analytics.Events.LevelComplete(_level, _score);
    }

    /// <summary>
    /// アップグレードの購入イベント
    /// </summary>
    /// <param name="_upgrade">購入したアップグレードの名前（ex. Speed UP）</param>
    /// <param name="_upgradeLevel">購入したアップグレードのレベル</param>
    /// <param name="_cost">購入にかかったコスト</param>
    public void UpgradePurchased(string _upgrade, int _upgradeLevel, int _cost) {
        Analytics.Events.UpgradePurchase(_upgrade, _upgradeLevel, _cost);
    }

    /// <summary>
    /// コンテンツをアンロックしたイベント
    /// </summary>
    /// <param name="_content">コンテンツ名</param>
    /// <param name="_unlockNumber">アンロックしたナンバー</param>
    public void ContentUnlocked(string _content, int _unlockNumber = -1) {
        Analytics.Events.ContentUnlocked(_content, _unlockNumber);
    }

    /// <summary>
    /// チュートリアルのクリア
    /// </summary>
    /// <param name="_step">チュートリアルの段階orレベルなど（string型）</param>
    public void TutorialComplete(string _step = null) {
        Analytics.Events.TutorialComplete(_step);
    }

    /// <summary>
    /// チュートリアルのクリア
    /// </summary>
    /// <param name="_step">チュートリアルの段階orレベルなど（int型）</param>
    public void TutorialComplete(int _step = -1) {
        Analytics.Events.TutorialComplete(_step);
    }

    /// <summary>
    /// ハイスコア更新
    /// </summary>
    /// <param name="_level">レベル（string）</param>
    /// <param name="_score">スコア</param>
    public void HichScoreArcieved(string _level, int _score) {
        Analytics.Events.HighScoreUnlocked(_level, _score);
    }

    /// <summary>
    /// ハイスコア更新
    /// </summary>
    /// <param name="_level">レベル（int）</param>
    /// <param name="_score">スコア</param>
    public void HichScoreArcieved(int _level, int _score) {
        Analytics.Events.HighScoreUnlocked(_level, _score);
    }

    /// <summary>
    /// ラウンドの開始
    /// </summary>
    /// <param name="_round">ラウンド</param>
    /// <param name="_wins">ラウンド勝利数（省略可）</param>
    /// <param name="_loses">ラウンド敗北数（省略可）</param>
    public void RondStarted(string _round, int _wins = -1, int _loses = -1) {
        Analytics.Events.RoundStarted(_round, _wins, _loses);
    }

    /// <summary>
    /// ラウンドの開始
    /// </summary>
    /// <param name="_round">ラウンド</param>
    /// <param name="_wins">ラウンド勝利数（省略可）</param>
    /// <param name="_loses">ラウンド敗北数（省略可）</param>
    public void RondStarted(int _round, int _wins = -1, int _loses = -1) {
        Analytics.Events.RoundStarted(_round, _wins, _loses);
    }
    /// <summary>
    /// ラウンドの終了
    /// </summary>
    /// <param name="_round">ラウンド</param>
    /// <param name="_wins">ラウンド勝利数（省略可）</param>
    /// <param name="_loses">ラウンド敗北数（省略可）</param>
    public void RondComplete(string _round, int _wins = -1, int _loses = -1) {
        Analytics.Events.RoundComplete(_round, _wins, _loses);
    }

    /// <summary>
    /// ラウンドの終了
    /// </summary>
    /// <param name="_round">ラウンド</param>
    /// <param name="_wins">ラウンド勝利数（省略可）</param>
    /// <param name="_loses">ラウンド敗北数（省略可）</param>
    public void RondComplete(int _round, int _wins = -1, int _loses = -1) {
        Analytics.Events.RoundComplete(_round, _wins, _loses);
    }

    /// <summary>
    /// カスタムイベント
    /// </summary>
    /// <param name="_eventName">イベント名</param>
    public void CustomEvent(string _eventName) {
        Analytics.LogEvent(_eventName);
    }

    /// <summary>
    /// カスタムイベント
    /// </summary>
    /// <param name="_eventName">イベント名</param>
    /// <param name="_eventParams">パラメーター</param>
    public void CustomEvent(string _eventName, Dictionary<string,object> _eventParams) {
        Analytics.LogEvent(_eventName, _eventParams);
    }

}
