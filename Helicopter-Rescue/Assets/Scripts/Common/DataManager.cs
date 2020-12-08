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
    public int nowStage = 1;
    public int loopCount = 0;
    public int coin = 0;
}

public class DataManager : Singleton<DataManager>
{
    PlayerData PlayerData { get; set; } = new PlayerData();
    BinaryFormatter formatter = new BinaryFormatter();

    public bool Sound { get { return PlayerData.isSound; } set { } }
    public bool Vibration { get { return PlayerData.isVibration; } set { } }
    public int NowStage { get { return PlayerData.nowStage; } set { } }
    public int LoopCount { get { return PlayerData.loopCount; } set { } }
    public int Coin { get { return PlayerData.coin; } set { } }

    public event Action OnChangeCoinCountListener;
    public static int stagePlayCount = 0;

    public DataManager()
    {
        Load();
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
            Save();
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

    public void ClearFPS()
    {
        PlayerData.nowStage = 1;
        PlayerData.loopCount++;
    }

    public void ClearTPS()
    {
        PlayerData.nowStage = 2;
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
}
