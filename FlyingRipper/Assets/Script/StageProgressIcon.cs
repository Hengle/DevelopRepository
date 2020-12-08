using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StageProgressIcon : MonoBehaviour {

    [SerializeField] Image myIcon;
    [SerializeField] Sprite clearColor;
    [SerializeField] Sprite tryColor;
    [SerializeField] Sprite notclearColor;

    bool isActive;
    public enum IconStatus {
        CLEARED,
        TRY,
        NOTCLEAR
    }

    public void SetStatus(bool newVal) {
        isActive = newVal;
        myIcon.gameObject.SetActive(newVal);
    }

    public void SetIconStatus(IconStatus myType) {
        switch (myType) {
            case IconStatus.CLEARED:
                myIcon.sprite = clearColor;
                break;
            case IconStatus.TRY:
                myIcon.sprite = tryColor;
                break;
            case IconStatus.NOTCLEAR:
                myIcon.sprite = notclearColor;
                break;
        }
    }
}
