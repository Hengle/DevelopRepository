using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SoundSetting : MonoBehaviour {

    [SerializeField] Sprite onImg;
    [SerializeField] Sprite offImg;
    Image myImg;

    void Start(){
        myImg = GetComponent<Image>();

        myImg.sprite = DataManager.GetPlayerSettingSound() ? onImg : offImg;
    }

    public void ChangeSetting() {

        DataManager.SetPlayerSettingSound(!DataManager.GetPlayerSettingSound());
        myImg.sprite = DataManager.GetPlayerSettingSound() ? onImg : offImg;

    }


}
