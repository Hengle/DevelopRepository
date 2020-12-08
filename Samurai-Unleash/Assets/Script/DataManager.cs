using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class DataManager 
{

    // ステージnumまでをClearしているか？
    public static bool CheckStageClearFlag(int num) {

        bool returnFlag = true;

        for (int i = 1; i < num; i++) {
            //Debug.Log("STAGE" + i +":"+ GetBool("_CLEAR_STAGE" + i, false));
            if (!GetBool("STAGE" + i + "_CLEARED", false)) { returnFlag = false; }
        }

        return returnFlag;

    }

    // もらったIDまででアンロックしているものをリストにまとめて返す
    public static List<int> CheckUnlockedStageList(int num) {

        List<int> unlockedStage = new List<int>();

        for (int i = 1; i <= num; i++) {
            if (GetBool("STAGE" + i + "_UNLOCK", false)) { unlockedStage.Add(i); }
        }

        return unlockedStage;

    }

    // もらったIDまででアンロックしているものをリストにまとめて返す
    public static List<int> CheckLockedStage(int num) {

        List<int> lockedStage = new List<int>();

        for (int i = 1; i <= num; i++) {
            if (!GetBool("STAGE" + i + "_UNLOCK", false)) { lockedStage.Add(i); }
        }

        return lockedStage;

    }

    // 最新クリアステージの算出
    public static int CheckContinueStage(int maxStage) {

        int stageID = 0;

        for (int i = 1; i <= maxStage; i++) {
            //Debug.Log("STAGE" + i +":"+ GetBool("_CLEAR_STAGE" + i, false));
            if (!GetBool("STAGE" + i + "_CLEARED", false)) { stageID = i; break; }
        }

        // 見つからなかった場合は最後のステージを返す
        if(stageID == 0) {
            stageID = maxStage;
        }

        return stageID;
    }

    // プレイヤーネームのゲット
    public static string GetPlayerName() {
        return PlayerPrefs.GetString("PLAYER_NAME", null);
    }

    // プレイヤーネームのセット
    public static void SetPlayerName(string name) {
        PlayerPrefs.SetString("PLAYER_NAME", name);
    }

    // プレイヤーのコインを返す
    public static int GetPlayerCoin() {
        return PlayerPrefs.GetInt("PLAYER_COIN", 0);
    }

    public static void SetPlayerCoin(int num) {
        PlayerPrefs.SetInt("PLAYER_COIN", num);
    }

    // プレイヤーの最新アンロックステージを返す
    public static int GetLatestUnlockStage() {
        return PlayerPrefs.GetInt("LATEST_UNLOCK_STAGE", 0);
    }

    public static void SetLatestUnlockStage(int num) {
        PlayerPrefs.SetInt("LATEST_UNLOCK_STAGE", num);
    }

    // プレイヤーの1位獲得数を返す
    public static int GetPlayerWinCount() {
        return PlayerPrefs.GetInt("PLAYER_WIN_COUNT", 0);
    }

    public static void SetPlayerWinCount(int num) {
        PlayerPrefs.SetInt("PLAYER_WIN_COUNT", num);
    }

    // プレイヤーの国旗を返す
    public static int GetPlayerBanner() {
        return PlayerPrefs.GetInt("PLAYER_BANNER", 1);
    }

    public static void SetPlayerBanner(int num) {
        PlayerPrefs.SetInt("PLAYER_BANNER", num);
    }

    // プレイヤーのランクを返す
    public static int GetPlayerRank() {
        return PlayerPrefs.GetInt("PLAYER_RANK", 1);
    }

    public static void SetPlayerRank(int num) {
        PlayerPrefs.SetInt("PLAYER_RANK", num);
    }

    // プレイヤーのランク経験値を返す
    public static int GetPlayerRankEXP() {
        return PlayerPrefs.GetInt("PLAYER_RANK_EXP", 0);
    }

    public static void SetPlayerRankEXP(int num) {
        PlayerPrefs.SetInt("PLAYER_RANK_EXP", num);
    }

    // ステージの星獲得数
    public static int GetStageStar(int stageID) {
        return PlayerPrefs.GetInt("STAGE" + stageID + "_STAR", 0);
    }

    public static void SetStageStar(int stageID, int num) {
        PlayerPrefs.SetInt("STAGE" + stageID + "_STAR", num);
    }

    // 指定されたステージまでの総スター獲得数を返す
    public static int GetAllStageStar(int stageID) {
        int haveStar = 0;

        for (int i = 1; i <= stageID; i++) {
            //Debug.Log("STAGE" + i +":"+ GetBool("_CLEAR_STAGE" + i, false));
            haveStar += GetStageStar(i);
        }

        return haveStar;
    }


    public static bool GetFirstRoulette() {
        return GetBool("FIRST_ROULETTE", false);
    }

    public static void SetFirstRoulette(bool val) {
        SetBool("FIRST_ROULETTE", val);
    }


    // デイリーミッションn番目がCLEAR済みかどうかをゲット
    public static bool GetStagePurchase() {
        return GetBool("STAGE_PURCHASE", false);
    }

    public static void SetStagePurchase(bool val) {
        SetBool("STAGE_PURCHASE", val);
    }

    // 指定されたステージからステージまでの総スター獲得数を返す
    public static int GetSelectStageStar(int min, int max) {
        int haveStar = 0;

        for (int i = min; i <= max; i++) {
            //Debug.Log("STAGE" + i +":"+ GetBool("_CLEAR_STAGE" + i, false));
            haveStar += GetStageStar(i);
        }

        return haveStar;
    }

    // ヒントのコインを返す
    public static int GetPlayerHint() {
        return PlayerPrefs.GetInt("PLAYER_HINT", 0);
    }

    public static void SetPlayerHint(int num) {
        PlayerPrefs.SetInt("PLAYER_HINT", num);
    }

    // デイリーミッションn番目のMissionID確認
    public static int GetDailyMissionID(int id) {
        return PlayerPrefs.GetInt("DAILY_MISSION_"+id, 0);
    }

    public static void SetDailyMissionID(int id,int num) {
        PlayerPrefs.SetInt("DAILY_MISSION_" + id, num);
    }

    // デイリーミッションn番目のMission カウントを確認
    public static int GetDailyMissionCount(int id) {
        return PlayerPrefs.GetInt("DAILY_MISSION_" + id + "_COUNT", 0);
    }

    public static void SetDailyMissionCount(int id, int num) {
        PlayerPrefs.SetInt("DAILY_MISSION_" + id + "_COUNT", num);
    }

    // デイリーミッションn番目がCLEAR済みかどうかをゲット
    public static bool GetDailyMissionClear(int id) {
        return GetBool("DAILY_MISSION_" + id + "_CLEAR", false);
    }

    public static void SetDailyMissionClear(int id, bool val) {
        SetBool("DAILY_MISSION_" + id + "_CLEAR", val);
    }
    
    // デイリーミッション報酬タイプを選択
    public static int GetDailyMissionRewardType() {
        // 0:コイン  1:ファイアボール

        return PlayerPrefs.GetInt("DAILY_MISSION_REWARD_TYPE ", 0);
    }
    public static void SetDailyMissionRewardType(int num) {
        // 0:コイン  1:ファイアボール

        PlayerPrefs.SetInt("DAILY_MISSION_REWARD_TYPE ", num);
    }

    // デイリーミッション報酬タイプを選択
    public static int GetDailyMissionRewardAmount() {
        return PlayerPrefs.GetInt("DAILY_MISSION_REWARD_AMOUNT ", 0);
    }
    public static void SetDailyMissionRewardAmount(int num) {
        PlayerPrefs.SetInt("DAILY_MISSION_REWARD_AMOUNT ", num);
    }

    // デイリーミッション 報酬を受け取ったかどうか
    public static bool GetDailyMissionRewarded() {
        return GetBool("DAILY_MISSION_REWARDED", false);
    }

    public static void SetDailyMissionRewarded(bool val) {
        SetBool("DAILY_MISSION_REWARDED", val);
    }

    // デイリーミッション 更新したかどうか 
    public static bool GetDailyMissionUpdate() {
        return GetBool("DAILY_MISSION_NEW", false);
    }

    public static void SetDailyMissionUpdate(bool val) {
        SetBool("DAILY_MISSION_NEW", val);
    }

    // デイリーミッション日付関連
    // 今日の日付を yyyy/m/dd で取る
    public static string GetToday() {
        return DateTime.Today.Year + "/" + DateTime.Today.Month + "/" + DateTime.Today.Day;
    }

    // 
    public static int GetPlayDateCount() {
        return PlayerPrefs.GetInt("PLAY_DATE_COUNT", 0);
    }

    public static void SetPlayDateCount(int num) {
        PlayerPrefs.SetInt("PLAY_DATE_COUNT", num);
    }

    // プレイ最終日を取得
    public static string GetLastPlayDate() {
        return PlayerPrefs.GetString("LAST_PLAY_DATE", "2010/1/1");
    }
    // プレイ最終日をセット
    public static void SetLastPlayDate() {
        var todayDate = DateTime.Today.Year + "/" + DateTime.Today.Month + "/" + DateTime.Today.Day;
        PlayerPrefs.SetString("LAST_PLAY_DATE", todayDate);
    }

    // デイリーミッション最終更新日を取得
    public static string GetLastDailyMissionDate() {
        return PlayerPrefs.GetString("DAILY_MISSION_UPDATE", "2010/1/1");
    }
    // デイリーミッション最終更新日をセット
    public static void SetLastDailyMissionDate() {
        var todayDate = DateTime.Today.Year + "/" + DateTime.Today.Month + "/" + DateTime.Today.Day;
        PlayerPrefs.SetString("DAILY_MISSION_UPDATE", todayDate);
    }


    // デイリーボーナス最終更新日を取得
    public static string GetLastDailyBonusDate() {
        return PlayerPrefs.GetString("DAILY_BONUS_UPDATE", "2010/1/1");
    }
    // デイリーボーナス最終更新日をセット
    public static void SetLastDailyBonusDate() {
        var todayDate = DateTime.Today.Year + "/" + DateTime.Today.Month + "/" + DateTime.Today.Day;
        PlayerPrefs.SetString("DAILY_BONUS_UPDATE", todayDate);
    }

    // 今何回目のデイリーボーナス？
    public static int GetDailyBonusDayCount() {
        return PlayerPrefs.GetInt("DAILY_BONUS_COUNT ", 0);
    }
    public static void SetDailyBonusDayCount(int num) {
        PlayerPrefs.SetInt("DAILY_BONUS_COUNT ", num);
    }


    // プレゼントの時間を取得、セット
    public static DateTime GetPresentTime() {
        string defult = new DateTime(2019, 6, 10, 15, 00, 0, DateTimeKind.Local).ToBinary().ToString();//保存していたデータが存在しない時用のデフォルト値
        string dateTimeString = PlayerPrefs.GetString("PRESENT_TIME", defult);
        //PlayerPrefs.SetString("PRESENT_TIME", new DateTime(2019, 6, 20, 10, 56, 0, DateTimeKind.Local).ToBinary().ToString());

        return System.DateTime.FromBinary(System.Convert.ToInt64(dateTimeString));
    }

    public static void SetPresentTime() {
        DateTime now = DateTime.Now;
        PlayerPrefs.SetString("PRESENT_TIME", now.ToBinary().ToString());
    }

    public static string GetPresentsActiveTime() {
        var dt1 = GetPresentTime();
        dt1 = dt1.AddDays(1);

        DateTime dt2 = DateTime.Now;
        var ts = dt1 - dt2;

        var timerText = new DateTime(1).Add(ts).ToString("HH:mm:ss");
        return timerText;
    }

    public static bool GetIsPresentsActive() {
        var dt1 = GetPresentTime();
        var span = DateTime.Now - dt1;

        bool returnFlag=false;
        if(span.TotalDays >= 1) { returnFlag = true; }

        return returnFlag;
    }

    // プレイヤーのファイアボール所持数を返す
    public static int GetPlayerFireballCount() {
        return PlayerPrefs.GetInt("PLAYER_FIREBALL", 0);
    }

    public static void SetPlayerFireballCount(int num) {
        PlayerPrefs.SetInt("PLAYER_FIREBALL", num);
    }


    // 選択中のスキン
    public static int GetSelectedSkin() {
        return PlayerPrefs.GetInt("PLAYER_SELECTED_SKIN", 1);
    }

    public static void SetSelectedSkin(int num) {
        PlayerPrefs.SetInt("PLAYER_SELECTED_SKIN", num);
    }

    // 選択中の水スキン
    public static int GetSelectedFontColor() {
        return PlayerPrefs.GetInt("PLAYER_SELECTED_FONT_COLOR", 1);
    }

    public static void SetSelectedFontColor(int num) {
        PlayerPrefs.SetInt("PLAYER_SELECTED_FONT_COLOR", num);
    }

    // 選択中の壁スキン
    public static int GetSelectedBrickSkin() {
        return PlayerPrefs.GetInt("PLAYER_SELECTED_BRICK", 1);
    }

    public static void SetSelectedBrickSkin(int num) {
        PlayerPrefs.SetInt("PLAYER_SELECTED_BRICK", num);
    }

    // プレイ回数
    public static int GetPlayCount() {
        return PlayerPrefs.GetInt("PLAY_COUNT", 0);
    }

    public static void SetPlayCount(int num) {
        PlayerPrefs.SetInt("PLAY_COUNT", num);
    }

    // 移動ガイドを表示するかどうか
    public static bool GetClearTutorial() {
        return GetBool("CLEAR_TUTORIAL", false);
    }

    public static void SetClearTutorial(bool val) {
        SetBool("CLEAR_TUTORIAL", val);
    }

    // レビュー済みかどうか？
    public static bool GetRateUs() {
        return GetBool("RATE_US", false);
    }

    public static void SetRateUs(bool val) {
        SetBool("RATE_US", val);
    }

    // レビュー拒否したかどうか？
    public static bool GetRateForgetIt() {
        return GetBool("RATE_FORGET", false);
    }

    public static void SetRateForgetIt(bool val) {
        SetBool("RATE_FORGET", val);
    }

    // プレイヤーのサウンド、振動設定を読み書き
    public static bool GetPlayerSettingSound() {
        return GetBool("SETTING_SOUND", true);
    }

    public static void SetPlayerSettingSound(bool val) {
        SetBool("SETTING_SOUND", val);
    }

    public static bool GetPlayerSettingMusic() {
        return GetBool("SETTING_MUSIC", true);
    }

    public static void SetPlayerSettingMusic(bool val) {
        SetBool("SETTING_MUSIC", val);
    }

    public static bool GetPlayerSettingVibration() {
        return GetBool("SETTING_VIBRATION", true);
    }

    public static void SetPlayerSettingVibration(bool val) {
        SetBool("SETTING_VIBRATION", val);
    }


    // 引数で渡されたステージIDの最大スコアを取得する
    public static int GetStageHighScore(int stageID) {

        return PlayerPrefs.GetInt("STAGE" + stageID + "_BEST_SCORE", 0);
    }

    // 引数で渡されたステージIDの最大スコアを取得する
    public static void SetStageHighScore(int stageID,int score) {

        PlayerPrefs.SetInt("STAGE" + stageID + "_BEST_SCORE", score);
    }

    // 引数で渡されたスキンがアンロックされているかチェック
    public static bool GetBrickSkinUnlock(int id) {

        return GetBool("BRICK_" + id + "_UNLOCK", false);
    }
    public static void SetBrickSkinUnlock(int id) {

        SetBool("BRICK_" + id + "_UNLOCK", true);
    }


    // キーパービデオ
    public static int GetKeeperUnlockVideoCount(int id) {
        return PlayerPrefs.GetInt("KEEPER_" + id + "_VIDEO_COUNT", 0);
    }

    public static void SetKeeperUnlockVideoCount(int id, int num) {
        PlayerPrefs.SetInt("KEEPER_" + id + "_VIDEO_COUNT", num);
    }

    // スキンビデオ
    public static int GetSkinUnlockVideoCount(int id) {
        return PlayerPrefs.GetInt("SKIN" + id + "_VIDEO_COUNT", 0);
    }

    public static void SetSkinUnlockVideoCount(int id, int num) {
        PlayerPrefs.SetInt("SKIN" + id + "_VIDEO_COUNT", num);
    }


    // 引数で渡された引数で渡されたスキンをアンロック
    public static void SetSkinUnlock(int skinID) {
        SetBool("SKIN" + skinID + "_UNLOCK", true);
    }

    // 引数で渡されたステージIDがアンロックされているかチェック
    public static bool GetSkinUnlock(int skinID) {

        return GetBool("SKIN" + skinID + "_UNLOCK", false);
    }

    // 引数で渡された引数で渡されたフォントをアンロック
    public static void SetFontColorUnlock(int skinid) {
        SetBool("FONT_COLOR" + skinid + "_UNLOCK", true);
    }

    // 引数で渡されたステージIDがアンロックされているかチェック
    public static bool GetFontColorUnlock(int skinid) {

        return GetBool("FONT_COLOR" + skinid + "_UNLOCK", false);
    }


    // 引数で渡されたステージIDをアンロックする
    public static void SetStageUnlock(int stageID) {
        SetBool("STAGE" + stageID + "_UNLOCK", true);
    }

    // 引数で渡されたステージIDがアンロックされているかチェックする
    public static bool GetStageUnlock(int stageID) {
        return GetBool("STAGE" + stageID + "_UNLOCK", false);
    }

    // 引数で渡されたステージIDがクリアされているかチェック
    public static bool GetStageClearCheck(int stageID) {

        return GetBool("STAGE" + stageID + "_CLEARED", false);
    }

    // 引数で渡されたステージIDをクリア状態にする
    public static void SetStageClear(int stageID) {
        SetBool("STAGE" + stageID + "_CLEARED", true);
    }

    // アンロックするために必要なビデオ視聴回数を取得
    public static int GetStageUnlockCount (int stageID) {
        return PlayerPrefs.GetInt("STAGE" + stageID + "_UNLOCK_MOVIE", 0);
    }

    // アンロックするために必要なビデオ視聴回数をセット
    public static void SetStageUnlockCount(int stageID, int count) {
        PlayerPrefs.SetInt("STAGE" + stageID + "_UNLOCK_MOVIE", count);
    }


    public static bool GetBool(string key, bool defalutValue) {
        var value = PlayerPrefs.GetInt(key, defalutValue ? 1 : 0);
        return value == 1;
    }

    public static void SetBool(string key, bool value) {
        PlayerPrefs.SetInt(key, value ? 1 : 0);
    }
}
