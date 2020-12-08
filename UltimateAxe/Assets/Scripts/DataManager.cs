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
}


public class DataManager : SingletonMonoBehaviour<DataManager>
{
    PlayerData PlayerData { get; set; } = new PlayerData();
    BinaryFormatter formatter = new BinaryFormatter();

    int maxStage = 5;

    public bool Sound { get { return PlayerData.isSound; } set { } }
    public bool Vibration { get { return PlayerData.isVibration; } set { } }
    public int StageLevel { get { return PlayerData.stageLevel; } set { } }
    public int NowStage { get { return PlayerData.nowStage; } set { } }

    protected override void Awake()
    {
        base.Awake();
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

    public void ChangeNowStage(int nowStage)
    {
        if (nowStage > maxStage) return;
        if (nowStage > StageLevel)
        {
            PlayerData.stageLevel = nowStage;
        }
        PlayerData.nowStage = nowStage;
    }

    public void ChangeStageLevel(int stageLevel)
    {
        PlayerData.stageLevel = stageLevel;
        PlayerData.nowStage = stageLevel;
    }
}
