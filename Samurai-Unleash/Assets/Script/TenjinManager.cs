using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TenjinManager
{
    static string TENJIN_SDK = "D4DVXQQPQWKAGCGR91UTMTOERXPGWYBO";
    static BaseTenjin instance;

    // Start is called before the first frame update
    public static void ConnectTenjin()
    {
#if UNITY_EDITOR
        return;
#else
        // Tenjin
        instance = Tenjin.getInstance(TENJIN_SDK);
        instance.Connect();
#endif
    }

    public static void TenjinCustomEvent(string eventName) {


#if UNITY_EDITOR
        return;
#else
        if (instance == null) {
            ConnectTenjin();
        }
        instance.SendEvent(eventName);

#endif
    }
}
