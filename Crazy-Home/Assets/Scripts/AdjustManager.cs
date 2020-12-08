using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using com.adjust.sdk;

public class AdjustManager : MonoBehaviour {
    private static AdjustManager myInstance;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void InitializeBeforeSceneLoad() {
        var dataManager = new GameObject("AdjustManager", typeof(AdjustManager));
        myInstance = dataManager.GetComponent<AdjustManager>();
        myInstance.InitializeSDK();
        DontDestroyOnLoad(dataManager);
    }

    AdjustConfig adjustConfig;

    public static AdjustManager Instance {
        get {
            return myInstance;
        }
        set { }
    }

    private void InitializeSDK() {
#if UNITY_IOS
        /* Mandatory - set your iOS app token here */
        InitAdjust("oedvi6nqu1og");
#elif UNITY_ANDROID
        /* Mandatory - set your Android app token here */
        InitAdjust("YOUR_ANDROID_APP_TOKEN_HERE");
#endif
    }

    private void InitAdjust(string adjustAppToken) {
        adjustConfig = new AdjustConfig(
            adjustAppToken,
            AdjustEnvironment.Production, // AdjustEnvironment.Sandbox to test in dashboard
            true
        );
        adjustConfig.setLogLevel(AdjustLogLevel.Info); // AdjustLogLevel.Suppress to disable logs
        adjustConfig.setSendInBackground(true);
        adjustConfig.setAttributionChangedDelegate(this.attributionChangedDelegate);

        new GameObject("Adjust").AddComponent<Adjust>(); // do not remove or rename

        // Adjust.addSessionCallbackParameter("foo", "bar"); // if requested to set session-level parameters

        //adjustConfig.setAttributionChangedDelegate((adjustAttribution) => {
        //  Debug.LogFormat("Adjust Attribution Callback: ", adjustAttribution.trackerName);
        //});

        Adjust.start(adjustConfig);
    }

    public void attributionChangedDelegate(AdjustAttribution attribution) {
    }
}
