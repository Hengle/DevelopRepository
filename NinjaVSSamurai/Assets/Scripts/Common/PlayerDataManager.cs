using System;
using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Data.Model;
using Data.Master;

public class PlayerDataManager : Singleton<PlayerDataManager> {
	public int Coin { get { return PlayerData.coin; } }
	public bool IsTutorialCleared { get { return PlayerData.isTutorialCleared; } }
	//public int SkinId							{ get { return PlayerData.skinId; } }
	//public int NowStageId						{ get { return PlayerData.nowStageId; } }
	//List<ArrivalStageParam> ArrivalStages		{ get { return PlayerData.arrivalStages; } }
	//List<FailureStageParam> FailureStages		{ get { return PlayerData.failureStages; } }
	//List<int> AcquiredSkinIds					{ get { return PlayerData.acquiredSkinIds; } }

	public const int MaxCoin = 99999;
	public const int MaxHint = 99;
	public const int DefaultSkinId = 1;
	public const int FirstStageId = 1;
	public const int DefaultBackgroundId = 1;
	static readonly List<int> FirstSetupSkinIds = new List<int> { 1, };

	Player PlayerData { get; set; } = new Player();
	bool IsLoaded { get; set; }

	// Singletonなので購読側での解除を忘れずに
	public event Action<int, int> OnCoinUpdateListener; // oldNum, newNum


	public PlayerDataManager () {
		Load(); // 初回ロードを強制
		FirstSetup();
	}

	public void Save () {
		var json = JsonUtility.ToJson(PlayerData);
		PlayerPrefs.SetString("PlayerData", json);

		Debug.LogWarning("PlayerDataSaved:" + json);
	}

	void Load () {
		if (IsLoaded) return;

		var str = PlayerPrefs.GetString("PlayerData");
		if (String.IsNullOrEmpty(str)) return;

		PlayerData = JsonUtility.FromJson<Player>(str);
		IsLoaded = true;

		Debug.LogWarning("PlayerDataLoaded:" + str);
	}

	//public void AddSkin (int skinId, bool isChangeImmediate) {
	//	if (AcquiredSkinIds.Contains(skinId)) { Debug.LogError("既に取得済みのスキンです"); return; }

	//	PlayerData.acquiredSkinIds.Add(skinId);

	//	if (isChangeImmediate) {
	//		ChangeSkin(skinId);
	//	}
	//}

	//public void ChangeSkin (int skinId) {
	//	if (!AcquiredSkinIds.Contains(skinId)) { Debug.LogError("未取得のスキンです"); return; }

	//	PlayerData.skinId = skinId;
	//}

	//public bool IsAcquiredSkin (int skinId) {
	//	return AcquiredSkinIds.Contains(skinId);
	//}

	//public void SetStageId (int stageId) {
	//	PlayerData.nowStageId = stageId;
	//}

	public void AddCoin (int num) {
		if (Coin + num > MaxCoin) {
			num = MaxCoin - Coin;
		}

		int old = Coin;
		PlayerData.coin += num;
		OnCoinUpdateListener?.Invoke(old, Coin);
	}

	public void ReduceCoin (int num) {
		if (Coin - num < 0) {
			num = Coin;
		}

		int old = Coin;
		PlayerData.coin -= num;
		OnCoinUpdateListener?.Invoke(old, Coin);
	}

	public void SetTutorialCleared () {
		PlayerData.isTutorialCleared = true;
	}

	//public int GetMaxArrivalStageId () {
	//	if (ArrivalStages.Count <= 0) return 0;

	//	return ArrivalStages.OrderByDescending(pair => pair.stageId).First().stageId;
	//}

	//public int GetArrivalStageRank (int stageId) {
	//	var target = ArrivalStages.FirstOrDefault(p => p.stageId == stageId);
	//	return target != null ? target.rank : 0;
	//}

	//public Dictionary<int, int> GetArrivalStagesDictionary () {
	//	var dict = new Dictionary<int, int>();
	//	foreach (var param in ArrivalStages) {
	//		dict.Add(param.stageId, param.rank);
	//	}
	//	return dict;
	//}

	//public void UpdateArrivalStages (int stageId, int rank) {
	//	var target = ArrivalStages.FirstOrDefault(p => p.stageId == stageId);

	//	if (target != null) {
	//		if (rank <= target.rank) return;
	//		target.rank = rank;
	//	} else {
	//		PlayerData.arrivalStages.Add(new ArrivalStageParam { stageId = stageId, rank = rank });
	//	}
	//}

	//public Dictionary<int, int> GetFailureStagesDictionary () {
	//	var dict = new Dictionary<int, int>();
	//	foreach (var param in FailureStages) {
	//		dict.Add(param.stageId, param.count);
	//	}
	//	return dict;
	//}

	//public void UpdateFailureStages (int stageId) {
	//	var target = FailureStages.FirstOrDefault(p => p.stageId == stageId);

	//	if (target != null) {
	//		target.count++;
	//	} else {
	//		PlayerData.failureStages.Add(new FailureStageParam { stageId = stageId, count = 1 });
	//	}
	//}

	void FirstSetup () {
		//bool needSave = false;

		//// デフォルトスキンを設定
		//if (AcquiredSkinIds.IsNullOrEmpty() || AcquiredSkinIds.Intersect(FirstSetupSkinIds).Count() != FirstSetupSkinIds.Count()) {
		//	PlayerData.acquiredSkinIds = AcquiredSkinIds.Union(FirstSetupSkinIds).ToList();
		//	ChangeSkin(DefaultSkinId);
		//	needSave = true;
		//}

		//// 初期ステージ設定
		//if (NowStageId < FirstStageId) {
		//	SetStageId(FirstStageId);
		//	needSave = true;
		//}

		//// 初回プレイ判定
		//if (!IsLoaded) {
		//	needSave = true;
		//}

		//if (needSave) {
		//	Save();
		//}
	}

}
