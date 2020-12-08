using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_IOS
using UnityEngine.iOS;
using System.Runtime.InteropServices;
#endif

public static class IOSUtil {
#if UNITY_IOS && !UNITY_EDITOR
        [DllImport ("__Internal")]
        static extern void _playSystemSound(int n);
#endif

    public static void PlaySystemSound(int n) //引数にIDを渡す
        {
#if UNITY_IOS && !UNITY_EDITOR
            _playSystemSound(n);
#endif
    }
}