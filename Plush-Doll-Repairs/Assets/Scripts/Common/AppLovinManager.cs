using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;

namespace SDKManager {
    public class AppLovinManager : MonoBehaviour {

        // AppLovin Manager 
        // スクリプトを配置するとゲーム起動時に自動的にAppLovinManagerが生成されます

        // ----- 設定ここから --------------------------------------------------------------------------- //

        // --------------------------------------------------------------------------- //
        // Ad Unit 
        // --------------------------------------------------------------------------- //

        // iOS 設定
        string IOS_BANNER_ID = "c2d57f3ac2783041";          // IOS バナーID
        string IOS_INTER_ID = "217728fe5185674e";           // IOS インタースティシャル広告ID
        string IOS_REWARD_ID = "d14bc6c2331f2ed9";          // IOS リワード広告ID

        // Android 設定 
        string ANDROID_BANNER_ID = "c6b44e27da9ec49d";      // ANDROID バナーID
        string ANDROID_INTER_ID = "9633979e0cf9da4e";       // ANDROID インタースティシャル広告ID
        string ANDROID_REWARD_ID = "924fdfd979bc71f9";      // ANDROID リワード広告ID

        // ----- 設定ここまで --------------------------------------------------------------------------- //

        string appLovinSdkKey = "TJ2lRrOfe1NQU0rKdce0GiUHc-PsV4WPMsKUmbGuwpCEqTqaCuxx10f4m2yWmAOQsHf0hXg-Aoxnw7okFkqM-a";

        string rewardedAdUnitId;
        string interstitialAdUnitId;
        string bannerAdUnitId;

        bool isRewardActive = false;

        private static AppLovinManager myInstance;
        public static Action OnEndRewardVideo; // リワードのコールバック用のメソッド
        public static Action OnEndInterstisial; // インタースティシャル閉じたあとのコールバック用

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void InitializeBeforeSceneLoad() {
            var dataManager = new GameObject("AppLovinManager", typeof(AppLovinManager));
            myInstance = dataManager.GetComponent<AppLovinManager>();
            myInstance.InitializeSDK();
            DontDestroyOnLoad(dataManager);
        }

        public static AppLovinManager Instance {
            get {
                return myInstance;
            }
            set { }
        }

        private void InitializeSDK() {
            MaxSdk.SetSdkKey(appLovinSdkKey);
#if UNITY_IOS
                rewardedAdUnitId = IOS_REWARD_ID;
                interstitialAdUnitId = IOS_INTER_ID;
                bannerAdUnitId = IOS_BANNER_ID;
#elif UNITY_ANDROID
                rewardedAdUnitId = ANDROID_REWARD_ID;
                interstitialAdUnitId = ANDROID_INTER_ID;
                bannerAdUnitId = ANDROID_BANNER_ID;
#endif

#if UNITY_EDITOR
                Debug.Log("SDK初期化はエディタ上では動作しません");
#else

            // AppLovin 初期化処理
            MaxSdkCallbacks.OnSdkInitializedEvent += (MaxSdkBase.SdkConfiguration sdkConfiguration) => {
                // Banner広告の初期化
                InitializeBannerAds();
                // インタースティシャル広告の初期化
                InitializeInterstitialAds();
                // リワード動画の初期化
                InitializeRewardedAds();
            };

            MaxSdk.SetSdkKey(appLovinSdkKey);
            MaxSdk.InitializeSdk();
#endif
        }

        /// <summary>
        /// バナー広告の初期化
        /// </summary>
        public void InitializeBannerAds() {
            // Banners are automatically sized to 320x50 on phones and 728x90 on tablets
            // You may use the utility method `MaxSdkUtils.isTablet()` to help with view sizing adjustments
            MaxSdk.CreateBanner(bannerAdUnitId, MaxSdkBase.BannerPosition.BottomCenter);

            // Set background or background color for banners to be fully functional
            MaxSdk.SetBannerBackgroundColor(bannerAdUnitId, Color.white);
        }

        /// <summary>
        /// インタースティシャル広告初期化
        /// </summary>
        public void InitializeInterstitialAds() {
            // Attach callback
            MaxSdkCallbacks.OnInterstitialLoadedEvent += OnInterstitialLoadedEvent;
            MaxSdkCallbacks.OnInterstitialLoadFailedEvent += OnInterstitialFailedEvent;
            MaxSdkCallbacks.OnInterstitialAdFailedToDisplayEvent += InterstitialFailedToDisplayEvent;
            MaxSdkCallbacks.OnInterstitialHiddenEvent += OnInterstitialDismissedEvent;

            // Load the first interstitial
            LoadInterstitial();
        }

        /// <summary>
        /// インタースティシャルの事前ロード
        /// </summary>
        private void LoadInterstitial() {
            MaxSdk.LoadInterstitial(interstitialAdUnitId);
        }

        /// <summary>
        /// インタースティシャル広告のロード完了
        /// </summary>
        /// <param name="adUnitId"></param>
        private void OnInterstitialLoadedEvent(string adUnitId) {
            // Interstitial ad is ready to be shown. MaxSdk.IsInterstitialReady(interstitialAdUnitId) will now return 'true'
            // インタースティシャル広告のロードが終わった。MaxSdk.IsInterstitialReady(interstitialAdUnitId) で trueを返す状態
        }

        /// <summary>
        /// インタースティシャル広告のロードに失敗
        /// </summary>
        /// <param name="adUnitId"></param>
        /// <param name="errorCode"></param>
        private void OnInterstitialFailedEvent(string adUnitId, int errorCode) {
            // Interstitial ad failed to load. We recommend re-trying in 3 seconds.
            // インタースティシャル広告のロードに失敗。3秒後に再試行。
            Invoke("LoadInterstitial", 3);

        }

        /// <summary>
        /// インタースティシャル広告の表示に失敗
        /// </summary>
        /// <param name="adUnitId"></param>
        /// <param name="errorCode"></param>
        private void InterstitialFailedToDisplayEvent(string adUnitId, int errorCode) {
            // Interstitial ad failed to display. We recommend loading the next ad
            // インタースティシャル広告の表示に失敗。次の動画をロードしておくことをおすすめします。
            LoadInterstitial();
        }

        /// <summary>
        /// インタースティシャル広告が閉じられたので次の動画を読込
        /// </summary>
        /// <param name="adUnitId"></param>
        private void OnInterstitialDismissedEvent(string adUnitId) {
            OnEndInterstisial?.Invoke();
            OnEndInterstisial = null;
            // Interstitial ad is hidden. Pre-load the next ad
            // インタースティシャル広告が閉じられました。次の広告に備えて広告動画をロードします。
            LoadInterstitial();
        }

        /// <summary>
        /// リワード動画広告の初期化
        /// </summary>
        public void InitializeRewardedAds() {
            // Attach callback
            MaxSdkCallbacks.OnRewardedAdLoadedEvent += OnRewardedAdLoadedEvent;
            MaxSdkCallbacks.OnRewardedAdLoadFailedEvent += OnRewardedAdFailedEvent;
            MaxSdkCallbacks.OnRewardedAdFailedToDisplayEvent += OnRewardedAdFailedToDisplayEvent;
            MaxSdkCallbacks.OnRewardedAdDisplayedEvent += OnRewardedAdDisplayedEvent;
            MaxSdkCallbacks.OnRewardedAdClickedEvent += OnRewardedAdClickedEvent;
            MaxSdkCallbacks.OnRewardedAdHiddenEvent += OnRewardedAdDismissedEvent;
            MaxSdkCallbacks.OnRewardedAdReceivedRewardEvent += OnRewardedAdReceivedRewardEvent;

            // Load the first RewardedAd
            LoadRewardedAd();
        }

        /// <summary>
        /// リワード動画のロード
        /// </summary>
        private void LoadRewardedAd() {
            MaxSdk.LoadRewardedAd(rewardedAdUnitId);
        }

        /// <summary>
        /// リワード動画のロード完了
        /// </summary>
        /// <param name="adUnitId"></param>
        private void OnRewardedAdLoadedEvent(string adUnitId) {
            // Rewarded ad is ready to be shown. MaxSdk.IsRewardedAdReady(rewardedAdUnitId) will now return 'true'
        }

        /// <summary>
        /// リワード動画のロードに失敗
        /// </summary>
        /// <param name="adUnitId"></param>
        /// <param name="errorCode"></param>
        private void OnRewardedAdFailedEvent(string adUnitId, int errorCode) {
            // Rewarded ad failed to load. We recommend re-trying in 3 seconds.
            Invoke("LoadRewardedAd", 3);
        }

        /// <summary>
        /// リワード動画の表示に失敗
        /// </summary>
        /// <param name="adUnitId"></param>
        /// <param name="errorCode"></param>
        private void OnRewardedAdFailedToDisplayEvent(string adUnitId, int errorCode) {
            // Rewarded ad failed to display. We recommend loading the next ad
            LoadRewardedAd();
        }

        /// <summary>
        /// リワード動画が表示された
        /// </summary>
        /// <param name="adUnitId"></param>
        private void OnRewardedAdDisplayedEvent(string adUnitId) { }


        /// <summary>
        /// リワード動画がClickされた
        /// </summary>
        /// <param name="adUnitId"></param>
        private void OnRewardedAdClickedEvent(string adUnitId) { }


        /// <summary>
        /// リワード動画が閉じられた
        /// </summary>
        /// <param name="adUnitId"></param>
        private void OnRewardedAdDismissedEvent(string adUnitId) {

            if (!isRewardActive) { return; }
            isRewardActive = false;
            OnEndRewardVideo?.Invoke();
            OnEndRewardVideo = null;
            // Rewarded ad is hidden. Pre-load the next ad
            LoadRewardedAd();
        }

        /// <summary>
        /// リワード動画に対する報酬を支払う
        /// </summary>
        /// <param name="adUnitId"></param>
        /// <param name="reward"></param>
        private void OnRewardedAdReceivedRewardEvent(string adUnitId, MaxSdk.Reward reward) {
            // Rewarded ad was displayed and user should receive the reward
            isRewardActive = true;
        }

        /// <summary>
        /// バナー広告の表示/非表示の切り替え
        /// </summary>
        /// <param name="newVal">true = 表示 / false = 非表示</param>
        public void AppLovinBannerAD(bool newVal) {
#if UNITY_EDITOR
                Debug.Log("バナー広告表示処理はエディタ上では動作しません");
                return;
#endif

            if (newVal) {
                MaxSdk.ShowBanner(bannerAdUnitId);
            } else {
                MaxSdk.HideBanner(bannerAdUnitId);
            }
        }

        /// <summary>
        /// AppLovin インタースティシャル広告の再生（コールバックあり）
        /// </summary>
        /// <param name="callback"></param>
        public void AppLovinShowInterstitial(Action callback) {
            OnEndInterstisial = callback;

#if UNITY_EDITOR
                Debug.Log("インタースティシャル広告の再生はエディタ上では動作しません");
                OnEndInterstisial?.Invoke();
                return;
#endif

            if (Application.internetReachability == NetworkReachability.NotReachable) {
                OnEndInterstisial?.Invoke();
            } else {
                if (MaxSdk.IsInterstitialReady(interstitialAdUnitId)) {
                    MaxSdk.ShowInterstitial(interstitialAdUnitId);
                } else {
                    OnEndInterstisial?.Invoke();
                }
            }


        }

        /// <summary>
        /// AppLovin リワード動画の再生
        /// </summary>
        /// <param name="callback">リワード動画を見終わった際に行いたいコールバックを登録</param>
        public void AppLovinShowRewardVideo(Action callback) {
            OnEndRewardVideo = callback;

#if UNITY_EDITOR
                Debug.Log("リワード動画の再生はエディタ上では動作しません");
                OnEndRewardVideo?.Invoke();
                return;
#endif

            if (Application.internetReachability == NetworkReachability.NotReachable) {
                return;
            } else {

                if (MaxSdk.IsRewardedAdReady(rewardedAdUnitId)) {
                    MaxSdk.ShowRewardedAd(rewardedAdUnitId);
                }

            }

        }

    }

}
