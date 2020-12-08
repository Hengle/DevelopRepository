using UnityEngine;
using System;

public abstract class SingletonMonoBehaviour<T> : MonoBehaviour where T : MonoBehaviour{
    private static T instance;
    protected bool isCreated = false;

    public static T Instance
    {
        get{
            if (instance == null) {
                Type t = typeof(T);
                instance = (T)FindObjectOfType (t);
                if (instance == null) {
                    Debug.LogError (t + " をアタッチしているGameObjectはありません");
                }
            }

            return instance;
        }
    }

    virtual protected void Awake(){
        // 他のゲームオブジェクトにアタッチされているか調べる
        // アタッチされている場合は破棄する。
        CheckInstance();
    }

    protected bool CheckInstance(){
        if (instance == null) {
            instance = this as T;
            return true;
        } else if (Instance == this) {
            return true;
        }
        Destroy (this.gameObject);
        isCreated = true;
        return false;
    }
}