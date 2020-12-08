using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class Config : MonoBehaviour
{
    [SerializeField] GameObject config;
    [SerializeField] GameObject soundButton;
    [SerializeField] GameObject configButton;
    [SerializeField] Sprite soundOn;
    [SerializeField] Sprite soundOff;

    bool isSound;
    bool isSetting;

    void Awake()
    {
        isSetting = false;
        isSound = DataManager.Instance.Sound;
        soundButton.GetComponent<Image>().sprite = isSound ? soundOn : soundOff;
    }

    public void Setteing()
    {
        if (isSetting)
        {
            //config.transform.DOMoveY(7.0f, 0.3f).SetEase(Ease.Linear);
            config.transform.DOMoveY(6.5f, 0.3f).SetEase(Ease.Linear);
        }
        else
        {
            //config.transform.DOMoveY(4.5f, 0.3f).SetEase(Ease.Linear);
            config.transform.DOMoveY(5.0f, 0.3f).SetEase(Ease.Linear);
        }

        isSetting = !isSetting;
    }

    public void ChangeSound()
    {
        isSound = !isSound;
        DataManager.Instance.ChangeSound(isSound);
        DataManager.Instance.Save();
        soundButton.GetComponent<Image>().sprite = isSound ? soundOn : soundOff;
    }
}
