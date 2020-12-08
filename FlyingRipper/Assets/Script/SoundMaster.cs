using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundMaster : MonoBehaviour {

    [SerializeField] List<AudioData> soundMaster = new List<AudioData>();

    [System.Serializable]
    public class AudioData {
        public string title;
        public AudioClip myClip;

        public string GetAudioTitle() { return title; }
        public AudioClip GetAudioClip() { return myClip; }
    }

    public List<AudioData> GetSoundMaster() { return soundMaster; }

}
