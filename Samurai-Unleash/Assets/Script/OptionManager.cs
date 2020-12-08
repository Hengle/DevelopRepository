using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class OptionManager : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI soundBtnText;
    [SerializeField] TextMeshProUGUI vibrationBtnText;
    bool isSound;
    bool isVib;

    // Start is called before the first frame update
    void Start()
    {
        isSound = DataManager.GetPlayerSettingSound();
        isVib = DataManager.GetPlayerSettingVibration();
        ChangeText();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void ChangeText() {

        soundBtnText.text = isSound ? "SOUND : ON" : "SOUND : OFF";
        vibrationBtnText.text = isVib ? "VIBRATION : ON" : "VIBRATION : OFF";

    }

    public void ChangeSoundSetting() {
        isSound = !isSound;
        DataManager.SetPlayerSettingSound(isSound);
        ChangeText();
    }

    public void ChangeVibSetting() {
        isVib = !isVib;
        DataManager.SetPlayerSettingVibration(isVib);
        ChangeText();
    }
}
