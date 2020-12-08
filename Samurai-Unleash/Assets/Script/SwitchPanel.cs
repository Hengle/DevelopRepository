using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class SwitchPanel : MonoBehaviour {
    [SerializeField] type actionType;
    [SerializeField] GameObject[] switchTargetObjects;
    [SerializeField] Sprite usedSwitchSprite;
    [SerializeField] Color inactiveColor;
    [SerializeField] Color activeColor;
    [SerializeField] Color activeLockColor;
    [SerializeField] bool isReactivable; // 再度切り替えできるか？
    [SerializeField] string activateMethodName;
    [SerializeField] AudioClip soundEffect;

    Sprite defaultSprite;
    SpriteRenderer myRenderer;
    bool isActive = false;// 現在のステータス
    bool isUsed;//切り替えたことがあるかを判定するフラグ

    enum type {
        SPAWN,
        HIDDEN,
        ACTIVATE_SCRIPT
    }

    private void Start() {


        // スクリプトを操作するなら1度切りにしておく
        if (actionType == type.ACTIVATE_SCRIPT) {
            isReactivable = false;
        }



        myRenderer = GetComponent<SpriteRenderer>();
        defaultSprite = myRenderer.sprite;
        //myRenderer.color = inactiveColor;
    }

    public void ChangeStatus() {

        // 再使用できず、使用済みなら戻す
        if (!isReactivable && isUsed) { return; }


        if (soundEffect != null) {
            if (DataManager.GetPlayerSettingSound()) {
                GetComponent<AudioSource>().PlayOneShot(soundEffect);
            }
        }

        // 最初に踏んだときのアクション
        if (!isActive) {
            isUsed = true;
            isActive = true;
            myRenderer.sprite = usedSwitchSprite;
            //myRenderer.DOColor(activeColor,0.5f);

            //if (!isReactivable) { myRenderer.DOColor(activeLockColor, 0.5f); } // １度きりならそっちの色で上書き

            ChangeObjStatus(true);

        } else {
            isUsed = true;
            isActive = false;
            //myRenderer.DOColor(inactiveColor, 0.5f);
            myRenderer.sprite = defaultSprite;

            ChangeObjStatus(false);
        }

    }

    private void ChangeObjStatus(bool flag) {


        switch (actionType) {

            case type.SPAWN:

                foreach (GameObject obj in switchTargetObjects) {
                    obj.SetActive(flag);
                }

                break;
            case type.HIDDEN:

                foreach (GameObject obj in switchTargetObjects) {
                    obj.SetActive(!flag);
                }

                break;
            case type.ACTIVATE_SCRIPT:

                foreach(GameObject obj in switchTargetObjects) {
                    obj.SendMessage(activateMethodName);
                }

                break;

        }


    }

}
