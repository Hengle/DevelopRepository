using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_IOS
using System.Runtime.InteropServices;
using UnityEngine.iOS;
#endif

public static class VibrationUtil {
    public static AndroidJavaClass unityPlayer;
    public static AndroidJavaObject currentActivity;
    public static AndroidJavaObject vibrator;
    public static AndroidJavaClass vibrationEffectClass;
    public static int defaultAmplitude;
    private static bool isInitialized;

#if UNITY_IOS && !UNITY_EDITOR
            [DllImport ("__Internal")]
            static extern void _playSystemSound(int n);
	        [DllImport ("__Internal")]
			static extern void _playSystemSoundWithDispose(int n);
#endif

	/// <summary>
	/// iOS用バイブレーション
	/// </summary>
	/// <param name="presetID">振動プリセットIDを指定（1519/1520/1521等）</param>
	public static void VibrationIOS(int presetID) {
        #if UNITY_IOS && !UNITY_EDITOR
            _playSystemSound(presetID);
        #endif
    }

	public static void VibrationIOSwithDispose(int presetID){
		#if UNITY_IOS && !UNITY_EDITOR
				            _playSystemSoundWithDispose(presetID);
		#endif
	}

	/// <summary>
	/// Android用バイブレーション
	/// </summary>
	/// <param name="milliseconds">振動させる秒数（ミリ秒）</param>
	public static void VibrationAndroid(long milliseconds) {
        if (!IsAndroid()) return;
        if (!isInitialized) { InitializeAndroid(); }

        if (GetSDKAndroid() >= 26) {
            CreateOneShot(milliseconds, defaultAmplitude);
        } else {

            if (IsUnity()) {
                Handheld.Vibrate();
            } else {
                OldVibrate(milliseconds);
            }
        }
    }

    private static void InitializeAndroid() {
        if (!IsAndroid()) return;
        defaultAmplitude = 255;

        unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
        vibrator = currentActivity.Call<AndroidJavaObject>("getSystemService", "vibrator");

        if (GetSDKAndroid() >= 26) {
            vibrationEffectClass = new AndroidJavaClass("android.os.VibrationEffect");
            defaultAmplitude = vibrationEffectClass.GetStatic<int>("DEFAULT_AMPLITUDE");
        }

        isInitialized = true;
    }

    private static bool IsAndroid() {
        #if UNITY_ANDROID && !UNITY_EDITOR
                return true;
        #else
                return false;
        #endif
    }

    private static bool IsUnity() {
        #if UNITY_EDITOR
                return true;
        #else
                return false;
        #endif
    }

    private static int GetSDKAndroid() {
        if (IsAndroid()) {
            using (var version = new AndroidJavaClass("android.os.Build$VERSION")) {
                return version.GetStatic<int>("SDK_INT");
            }
        } else {
            return -1;
        }
    }

    private static void OldVibrate(long milliseconds) {
        vibrator.Call("vibrate", milliseconds);
    }
    public static void CreateOneShot(long milliseconds, int amplitude) {
        CreateVibrationEffect("createOneShot", new object[] { milliseconds, amplitude });
    }
    private static void CreateVibrationEffect(string function, params object[] args) {
        AndroidJavaObject vibrationEffect = vibrationEffectClass.CallStatic<AndroidJavaObject>(function, args);
        vibrator.Call("vibrate", vibrationEffect);
    }
}
