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
    public int stage = 1;
    public int coin = 0;
}

public class DataManager : Singleton<DataManager>
{
    PlayerData PlayerData { get; set; } = new PlayerData();
    BinaryFormatter formatter = new BinaryFormatter();

    public int Stage { get { return PlayerData.stage; } set { } }
    public int Coin { get { return PlayerData.coin; } set { } }
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
            //Debug.LogError("Save file not found");
        }
    }

    public void AddStageCount()
    {
        PlayerData.stage++;
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
