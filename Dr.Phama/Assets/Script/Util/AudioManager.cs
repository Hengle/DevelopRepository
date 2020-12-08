using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : SingletonMonoBehaviour<AudioManager>
{
    const string BGM_PATH = "Audio/BGM/";
    const string SE_PATH = "Audio/SE/";

    AudioSource bgmSource;
    AudioSource seSource;

    Dictionary<string, AudioClip> bgmPool, sePool;

    protected override void Awake()
    {
        base.Awake();
        bgmSource = gameObject.AddComponent<AudioSource>();
        seSource = gameObject.AddComponent<AudioSource>();
        bgmPool = new Dictionary<string, AudioClip>();
        sePool = new Dictionary<string, AudioClip>();
    }

    public void LoadBGM(string bgmName)
    {
        if (!bgmPool.ContainsKey(bgmName))
        {
            var path = BGM_PATH + bgmName;
            var audioClip = Resources.Load<AudioClip>(path);
            bgmPool.Add(bgmName,audioClip);
        }
        else
        {
            Debug.Log(bgmName + "はロード済みです");
        }
    }

    public void LoadSe(string seName)
    {
        sePool = new Dictionary<string, AudioClip>();
        if (!sePool.ContainsKey(seName))
        {
            var path = SE_PATH + seName;
            var audioClip = Resources.Load<AudioClip>(path);
            sePool.Add(seName, audioClip);
        }
        else
        {
            Debug.Log(seName + "はロード済みです");
        }
    }

    public void PlayBGM(string bgmName, bool isLoop = true)
    {
        if (!bgmPool.ContainsKey(bgmName))
        {
            Debug.Log(bgmName + "という名前のBGMがありません");
            return;
        }

        if (!bgmSource.isPlaying) bgmSource.Play();

        //同じBGMなら流さない
        if (bgmSource.clip.name == bgmName) return;

        bgmSource.clip = bgmPool[bgmName];
        bgmSource.loop = isLoop;
        bgmSource.Play();
    }

    public void PlaySE(string seName)
    {
        if (!sePool.ContainsKey(seName))
        {
            Debug.Log(seName + "という名前のSEがありません");
            return;
        }
        seSource.PlayOneShot(sePool[seName]);
    }

    public void StopBgm()
    {
        bgmSource.Stop();
    }

    public void StopSe()
    {
        seSource.Stop();
    }
}
