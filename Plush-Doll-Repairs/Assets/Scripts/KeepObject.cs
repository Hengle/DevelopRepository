using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeepObject : MonoBehaviour
{

    // 現在存在しているオブジェクト実体の記憶領域
    static KeepObject _instance = null;

    // オブジェクト実体の参照（初期参照時、実体の登録も行う）
    static KeepObject instance
    {
        get { return _instance ?? (_instance = FindObjectOfType<KeepObject>()); }
    }

    void Awake()
    {

        // ※オブジェクトが重複していたらここで破棄される

        // 自身がインスタンスでなければ自滅
        if (this != instance)
        {
            Destroy(gameObject);
            return;
        }

        // 以降破棄しない
        DontDestroyOnLoad(gameObject);

    }

    void OnDestroy()
    {

        // ※破棄時に、登録した実体の解除を行なっている

        // 自身がインスタンスなら登録を解除
        if (this == instance) _instance = null;

    }

    public void DeleteChildlen()
    {
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
    }
}
