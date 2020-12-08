using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SkinButton : MonoBehaviour
{

    [SerializeField] Image myImage;
    [SerializeField] GameObject myString;
    [SerializeField] GameObject myCoin;
    [SerializeField] GameObject mySelect;
    [SerializeField] GameObject myMask;
    int skinID;
    int skinPrice;
    Button myButton;

    // Start is called before the first frame update
    void Awake()
    {
        myButton = GetComponent<Button>();
    }

    public void SetStatus(int id,bool isUnlock, bool isCoin , int price,  string itemStr) {
        skinID = id;
        skinPrice = price;

        // 選択中かな？
        var mySelected = DataManager.GetSelectedSkin();
        if(mySelected == skinID) {
            ChangeSelect(true);
        }

        // 画像適用
        var sprite = Resources.Load<Sprite>("Skin/Skin" + skinID);
        myImage.sprite = sprite;

        // マスク適用
        myMask.SetActive(!isUnlock); // アンロック済みなら非表示 なので反転
        myCoin.SetActive(!isUnlock); // アンロック済みならコインも非表示
        myString.SetActive(true); // コインじゃないならメッセージを表示

        // アンロックされてなければアンロック条件を表示
        if (!isUnlock) {
            myButton.interactable = false;
            myCoin.SetActive(isCoin); // コインによるアンロックかどうか
            myString.SetActive(!isCoin);
        }


        // コイン購入なら値段をチェックする
        if (isCoin) {
            if(DataManager.GetPlayerCoin() >= skinPrice) {
                myButton.interactable = true;
                myMask.SetActive(false);
            } else {
                myButton.interactable = false;
            }
        }

        myString.GetComponentInChildren<TextMeshProUGUI>().text = itemStr; // メッセージ反映
        myCoin.GetComponentInChildren<TextMeshProUGUI>().text = price.ToString();// 価格反映
    }


    public void OnClickSkinButton() {

        // アンロック済みかで処理が変わる
        if (DataManager.GetSkinUnlock(skinID)) {
            // 古いボタンを検索して書き換える
            var oldNum = DataManager.GetSelectedSkin();
            var oldButton = GameObject.Find("SkinButton" + oldNum).GetComponent<SkinButton>();
            oldButton.ChangeSelect(false);

            // 俺自身を書き換える
            ChangeSelect(true);
            DataManager.SetSelectedSkin(skinID);
        }
        else {

            // 価格チェック
            if(DataManager.GetPlayerCoin() < skinPrice) { return; } //金足りなきゃ戻す

            myCoin.SetActive(false); // コインによるアンロックかどうか
            myString.SetActive(true);

            DataManager.SetPlayerCoin(DataManager.GetPlayerCoin()-skinPrice);
            DataManager.SetSkinUnlock(skinID);

            ChangeSelect(false);

        }



    }

    public void ChangeSelect(bool flag) {
        mySelect.SetActive(flag);
        myString.GetComponentInChildren<TextMeshProUGUI>().text = flag ? "USED" : "USE";
    }
}
