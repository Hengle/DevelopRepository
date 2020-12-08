using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public enum GameType
{
    Blower,
    Gardener,
    Watering,
    Harvest
}

[System.Serializable]
public class PlayerData
{
    public bool isSound = true;
    public bool isVibration = true;
    public int stageLevel = 1;
    public int nowStage = 0;
    public int coin = 0;
    public GameType[] stageList = new GameType[4];
}

public class DataManager : Singleton<DataManager>
{
    PlayerData PlayerData { get; set; } = new PlayerData();
    BinaryFormatter formatter = new BinaryFormatter();

    static int stageCount = Enum.GetNames(typeof(GameType)).Length;

    public bool Sound { get { return PlayerData.isSound; } set { } }
    public bool Vibration { get { return PlayerData.isVibration; } set { } }
    public int StageLevel { get { return PlayerData.stageLevel; } set { } }
    public int NowStage { get { return PlayerData.nowStage; } set { } }
    public int Coin { get { return PlayerData.coin; } set { } }
    public GameType[] StageList { get { return PlayerData.stageList; } set { } }

    public event Action OnChangeCoinCountListener;
    public static int stagePlayCount = 0;

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
            PlayerData.stageList = Enum.GetValues(typeof(GameType))
                    .OfType<GameType>()
                    .OrderBy(e => Guid.NewGuid())
                    .ToArray();
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

    public void ChangeNowStage(int nowStage)
    {
        if (nowStage >= stageCount)
        {
            nowStage = 0;
            PlayerData.stageList = Enum.GetValues(typeof(GameType))
                    .OfType<GameType>()
                    .OrderBy(e => Guid.NewGuid())
                    .ToArray();
            PlayerData.stageLevel++;
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
}
