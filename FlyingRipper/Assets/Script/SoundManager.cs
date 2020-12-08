using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour {

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void InitializeBeforeSceneLoad() {
        var manager = new GameObject("SoundManager", typeof(SoundManager));
        manager.AddComponent<AudioSource>();
        myInstance = manager.GetComponent<SoundManager>();
        
        DontDestroyOnLoad(manager);

    }

    public static List<SoundMaster.AudioData> soundData = new List<SoundMaster.AudioData>();
    public static Dictionary<string, AudioClip> soundList = new Dictionary<string, AudioClip>();

    AudioSource myAuido;

    private static SoundManager myInstance;
    public static SoundManager Instance {
        get {
            return myInstance;
        }
        set { }
    }

    private void Start() {
        myInstance = GetComponent<SoundManager>();
        myAuido = GetComponent<AudioSource>();

        var prefab = Resources.Load<SoundMaster>("Sound/SoundMaster");
        if (prefab == null) { Debug.LogError("Resources/Sound/SoundMaster が見つかりません"); }

        soundData = prefab.GetSoundMaster();

        foreach(SoundMaster.AudioData obj in soundData) {
            soundList.Add(obj.GetAudioTitle(), obj.GetAudioClip());
        }
    }

    public void Play(string key) {

        // サウンド設定管理
        if (!DataManager.GetPlayerSettingSound()) { return; }

        if(soundList[key] == null) { Debug.Log(key + "のサウンドは見当たりません"); return; }

        myAuido.PlayOneShot(soundList[key]);

    }

    public void Stop() {
        myAuido.Stop();
    }


}
