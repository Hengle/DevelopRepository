using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AppLovinAdManager : SingletonMonoBehaviour<AppLovinAdManager> {
	int CallInterstitialCount { get; set; }
	//int ShowInterstitialNeedCount { get { return Application.systemLanguage == SystemLanguage.Japanese ? 5 : 3; } }
	int ShowInterstitialNeedCount { get { return 2; } }
	int RewardAmount { get; set; }
	bool IsEditor { get; set; } = false;

	public bool IsLoadedInterstitial { get; private set; }
	public bool IsLoadedReward { get; private set; }

	Action OnCloseInterstitial { get; set; }
	Action<int> OnCloseRewardInterstitial { get; set; }


	protected override void Awake () {
		base.Awake();
		if (isCreated) return;

		DontDestroyOnLoad(gameObject);

#if UNITY_EDITOR
		IsEditor = true;
#endif
		if (IsEditor) return;

		// 広告SDK初期化
		AppLovin.SetSdkKey("TJ2lRrOfe1NQU0rKdce0GiUHc-PsV4WPMsKUmbGuwpCEqTqaCuxx10f4m2yWmAOQsHf0hXg-Aoxnw7okFkqM-a");
		AppLovin.InitializeSdk();
		AppLovin.SetUnityAdListener("AppLovinAdManager");

		Preload();
	}

	void Start () {
		//ShowBanner();
	}

	public void Preload () {
		if (IsEditor) return;

		if (!IsLoadedInterstitial) AppLovin.PreloadInterstitial();
		if (!IsLoadedReward) AppLovin.LoadRewardedInterstitial();
	}

	public void ShowInterstitial (Action onClose = null) {
		if (IsEditor) { onClose?.Invoke(); return; }

		OnCloseInterstitial = onClose;
		CallInterstitialCount++;

		if (CallInterstitialCount < ShowInterstitialNeedCount) {
			OnCloseInterstitial?.Invoke();
			OnCloseInterstitial = null;
			return;
		}

		CallInterstitialCount = 0;
		if (AppLovin.HasPreloadedInterstitial()) {
			AppLovin.ShowInterstitial();
		} else {
			OnCloseInterstitial?.Invoke();
			OnCloseInterstitial = null;
		}
	}

	public void ShowRewardedInterstitial (Action<int> onClose = null) {
		if (IsEditor) { onClose?.Invoke(0); return; }

		OnCloseRewardInterstitial = onClose;

		if (AppLovin.IsIncentInterstitialReady()) {
			AppLovin.ShowRewardedInterstitial();
		} else {
			OnCloseRewardInterstitial?.Invoke(0);
			OnCloseRewardInterstitial = null;
		}
	}

	public void ShowBanner () {
		if (IsEditor) return;

		AppLovin.ShowAd(AppLovin.AD_POSITION_CENTER, AppLovin.AD_POSITION_BOTTOM);
	}

	void onAppLovinEventReceived (string ev) {
		if (ev.Contains("LOADEDINTER")) IsLoadedInterstitial = true;
		if (ev.Contains("LOADEDREWARDED")) IsLoadedReward = true;

		if (ev.Contains("LOADINTERFAILED")) IsLoadedInterstitial = false;
		if (ev.Contains("LOADREWARDEDFAILED")) IsLoadedReward = false;


		if (ev.Contains("HIDDENINTER")) {
			OnCloseInterstitial?.Invoke();
			OnCloseInterstitial = null;
			AppLovin.PreloadInterstitial();
		}

		if (ev.Contains("REWARDAPPROVEDINFO")) {
			char[] delimiter = { '|' };
			string[] split = ev.Split(delimiter);
			RewardAmount = (int)double.Parse(split[1]);
		} else if (ev.Contains("USERCLOSEDEARLY")) {
			RewardAmount = 0;
		} else if (ev.Contains("HIDDENREWARDED")) {
			OnCloseRewardInterstitial?.Invoke(RewardAmount);
			OnCloseRewardInterstitial = null;
			RewardAmount = 0;
			AppLovin.LoadRewardedInterstitial();
		}
	}
}
