using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


[CreateAssetMenu(menuName = "ScriptableObject/Create StageMaster", fileName = "StageMaster")]
public class StageMaster : MasterData
{
    [SerializeField] private List<StageData> StageList = new List<StageData>();

    public List<StageData> GetStageList()
    {
        return StageList;
    }

    public StageData GetStageData(int stageId)
    {
        return StageList.Find((p) => p.Id == stageId);
    }

    public StageData GetStageData(int day, int task)
    {
        return StageList.Find((p) => p.Day == day && p.Task == task);
    }

    public override void Validation()
    {
        var isDuplicate = StageList.GroupBy(p => p.Id).Where(g => g.Count() > 1).Select(g => g.Key).Any();

        if (isDuplicate)
        {
            Debug.LogError("キーが重複しています。");
        }

        var minId = StageList.Where(x => !StageList.Contains(StageList.Find(y => y.Id == x.Id + 1))).Select(x => x.Id + 1).Min();
        if (minId != StageList.Count + 1)
        {
            Debug.LogError(minId + ":キーが連続していません。");
        }
    }
}

[System.Serializable]
public class StageData
{
    [SerializeField] int id;
    [SerializeField] int day;
    [SerializeField] int task;
    [SerializeField] GameType gameType;
    [SerializeField] GameObject plushDollModel;
    [SerializeField] GameObject clientModel;
    [SerializeField] Sex clientSex;
    [SerializeField] int money;
    [SerializeField] int sewingPoint;
    [SerializeField] int accessoryPattern;

    public int Id { get { return id; } }
    public int Day { get { return day; } }
    public int Task { get { return task; } }
    public GameType GameType { get { return gameType; } }
    public GameObject PlushDollModel { get { return plushDollModel; } }
    public GameObject ClientModel { get { return clientModel; } }
    public Sex ClientSex { get { return clientSex; } }
    public int Money { get { return money; } }
    public int SewingPoint { get { return sewingPoint; } }
    public int AccessoryPattern { get { return accessoryPattern; } }
}
