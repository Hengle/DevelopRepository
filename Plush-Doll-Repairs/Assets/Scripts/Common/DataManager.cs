using System;
using System.Linq;
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
    public int day = 1;
    public int task = 1;
    public int coin = 0;
    public int loopCount = 0;
}

public class DataManager : Singleton<DataManager>
{
    PlayerData PlayerData { get; set; } = new PlayerData();
    BinaryFormatter formatter = new BinaryFormatter();

    static int stageCount = Enum.GetNames(typeof(GameType)).Length;

    public bool Sound { get { return PlayerData.isSound; } set { } }
    public bool Vibration { get { return PlayerData.isVibration; } set { } }
    public int Day { get { return PlayerData.day; } set { } }
    public int Task { get { return PlayerData.task; } set { } }
    public int Coin { get { return PlayerData.coin; } set { } }
    public int LoopCount { get { return PlayerData.loopCount; } set { } }
    public string HomeType = "Home";

    public event Action OnChangeCoinCountListener;

    public DataManager()
    {
        Load();
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

    public void UpdateDayTask(StageData stageData)
    {
        PlayerData.day = stageData.Day;
        PlayerData.task = stageData.Task;
    }

    public void AddLoopCount(int value)
    {
        PlayerData.loopCount += value;
    }

    public void AddCoin(int value)
    {
        PlayerData.coin += value;
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
