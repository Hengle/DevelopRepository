using System.Collections.Generic;
using UnityEngine;

//ScriptableObjectを継承したクラス
public class SkinData : ScriptableObject {

    //ListステータスのList
    public List<Skin> SkinMaster = new List<Skin>();

}

//System.Serializableを設定しないと、データを保持できない(シリアライズできない)ので注意
[System.Serializable]
public class Skin {

    // スキンの名前
    [SerializeField]
    private string itemName;


    // スキン解放の条件
    public enum TypeOfUnlockItem {
        Unlocked,
        Coin,
        PlayCount,
        StageClear,
        WatchVideo,
        LoginDate
    }
    //　アイテムの種類
    [SerializeField]
    private TypeOfUnlockItem typeOfUnlockItem;

    //　アイテムの情報
    [SerializeField]
    private Sprite playerSkin;

    //　アイテムの情報
    [SerializeField]
    private int unlockValue;

    // 説明
    [SerializeField]
    private string itemStr;


    public TypeOfUnlockItem GetTypeOfUnlockItem() {
        return typeOfUnlockItem;
    }

    public string GetItemName() {
        return itemName;
    }

    public Sprite GetSkin() {
        return playerSkin;
    }

    public int GetUnlockValue() {
        return unlockValue;
    }

    public string GetItemString() {
        return itemStr;
    }
}