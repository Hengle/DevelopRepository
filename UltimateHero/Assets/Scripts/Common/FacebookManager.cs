using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SDKManager {
    public class FacebookManager : MonoBehaviour {

        // FacebookManager
        // スクリプトを配置するとゲーム起動時に自動的にFacebookManagerが生成されます

        private static FacebookManager myInstance;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void InitializeBeforeSceneLoad() {
            var dataManager = new GameObject("FacebookManager", typeof(FacebookManager));
            myInstance = dataManager.GetComponent<FacebookManager>();
            myInstance.InitializeSDK();
            DontDestroyOnLoad(dataManager);
        }

        public static FacebookManager Instance {
            get {
                return myInstance;
            }
            set { }
        }

        private void InitializeSDK() {
            if (Facebook.Unity.FB.IsInitialized) {
                Facebook.Unity.FB.ActivateApp();
            } else {
                Facebook.Unity.FB.Init(Facebook.Unity.FB.ActivateApp);
            }
        }

        /// <summary>
        /// Facebook用カスタムイベントの発火
        /// </summary>
        /// <param name="eventName">カスタムイベント名</param>
        public void FacebookCustomEvent(string eventName) {
#if UNITY_EDITOR
            Debug.Log("Facebookカスタムイベントはエディタ上では動作しません");
            return;
#endif
            Facebook.Unity.FB.LogAppEvent(eventName);
        }

        /// <summary>
        /// ステージクリアイベントを発火させる（自動3桁揃え）
        /// </summary>
        /// <param name="stageID"></param>
        public void FacebookStageClearEvent(int stageID) {
            var idStr = stageID.ToString();
            var eventName = "CLEAR STAGE " + idStr.PadLeft(3, '0');
            FacebookCustomEvent(eventName);
        }

        /// <summary>
        /// ステージリトライイベントを発火させる（自動3桁揃え）
        /// </summary>
        /// <param name="stageID"></param>
        public void FacebookStageRetryEvent(int stageID) {
            var idStr = stageID.ToString();
            var eventName = "RETRY STAGE " + idStr.PadLeft(3, '0');
            FacebookCustomEvent(eventName);
        }

        /// <summary>
        /// スキンアンロックを発火させる（自動2桁揃え）
        /// </summary>
        /// <param name="skinID"></param>
        public void FacebookSkinUnlockEvent(int skinID) {
            var idStr = skinID.ToString();
            var eventName = "UNLOCK SKIN " + idStr.PadLeft(2, '0');
            FacebookCustomEvent(eventName);
        }
    }

}

