using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

[System.Serializable]
public class PlayerData
{
    public bool isSound = true;
    public bool isVibration = true;
    public int stageLevel = 1;
    public int nowStage = 1;
    public int loopCount = 0;
    public int coin = 0;
    public int weaponId = 1;
    public bool isWeaponTutorial = true;
    public List<int> possessedWeaponIds = new List<int>();
}

public class DataManager : Singleton<DataManager>
{
    PlayerData PlayerData { get; set; } = new PlayerData();
    BinaryFormatter formatter = new BinaryFormatter();

    [SerializeField] int stageCount = 25;

    public bool Sound { get { return PlayerData.isSound; } set { } }
    public bool Vibration { get { return PlayerData.isVibration; } set { } }
    public int StageLevel { get { return PlayerData.stageLevel; } set { } }
    public int NowStage { get { return PlayerData.nowStage; } set { } }
    public int LoopCount { get { return PlayerData.loopCount; } set { } }
    public int Coin { get { return PlayerData.coin; } set { } }
    public int WeaponId { get { return PlayerData.weaponId; } set { } }
    public bool WeaponTutorial { get { return PlayerData.isWeaponTutorial; } set { } }
    public List<int> PossessedWeaponIds { get { return PlayerData.possessedWeaponIds; } set { } }

    public event Action OnSetWeaponListener;
    public event Action OnChangeCoinCountListener;
    int DefaultWeaponId = 1;
    public static int stagePlayCount = 0;

    public DataManager()
    {
        Load();
        PurchaseWeapon(DefaultWeaponId);
    }

    void StageLoad()
    {
        var path = "Master/StageMaster";

        if (string.IsNullOrEmpty(path))
        {
            return;
        }

        //stageMaster = UnityEngine.Object.Instantiate(Resources.Load<StageMaster>(path));
    }

    public void Save()
    {
        string path = Application.persistentDataPath + "/playerdata.fun";
        FileStream stream = new FileStream(path, FileMode.Create);

        formatter.Serialize(stream, PlayerData);
        stream.Close();
    }

    public void Load()
    {
        string path = Application.persistentDataPath + "/playerdata.fun";
        Debug.Log(path);
        if (File.Exists(path))
        {
            FileStream stream = new FileStream(path, FileMode.Open);

            PlayerData = formatter.Deserialize(stream) as PlayerData;
            if (PlayerData == null)
            {
                PlayerData = new PlayerData();
            }
            stream.Close();
        }
        else
        {
            Debug.LogError("Save file not found");
        }
    }

    public void ChangeSound(bool isSound)
    {
        PlayerData.isSound = isSound;
    }

    public void ChangeVibration(bool isVibration)
    {
        PlayerData.isVibration = isVibration;
    }

    public void ClearWeaponTutorial()
    {
        PlayerData.isWeaponTutorial = false;
    }

    public void ChangeNowStage(int nowStage)
    {
        if (nowStage > stageCount)
        {
            nowStage = 1;
            PlayerData.loopCount++;
        }
        if (nowStage > StageLevel)
        {
            PlayerData.stageLevel = nowStage;
        }
        PlayerData.nowStage = nowStage;
    }

    public void ChangeCoin(int value)
    {
        PlayerData.coin = value;
        OnChangeCoinCountListener?.Invoke();
    }

    public bool ConsumeCoin(int amount)
    {
        if (Coin - amount < 0)
        {
            return false;
        }

        PlayerData.coin -= amount;
        OnChangeCoinCountListener?.Invoke();
        return true;
    }

    public void ChangeStageLevel(int stageLevel)
    {
        PlayerData.stageLevel = stageLevel;
        PlayerData.nowStage = stageLevel;
    }

    public bool IsPossessedWeapon(int id)
    {
        return PossessedWeaponIds.Contains(id);
    }

    public bool IsActiveWeapon(int id)
    {
        return WeaponId == id;
    }

    public void SetActiveWeapon(int id)
    {
        PlayerData.weaponId = id;
        OnSetWeaponListener?.Invoke();
    }



    public void PurchaseWeapon(int id)
    {
        if (IsPossessedWeapon(id)) return;
        PlayerData.possessedWeaponIds.Add(id);
    }
}
