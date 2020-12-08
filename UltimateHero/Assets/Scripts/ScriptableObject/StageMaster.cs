using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System.Linq;

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

    public NavMeshData GetNaviMeshByStageID(int stageId)
    {
        return StageList.Find((p) => p.Id == stageId).NavMeshData;
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
    [SerializeField] NavMeshData navMeshData;
    [SerializeField] GameObject stagePrefab;

    public int Id { get { return id; } }
    public NavMeshData NavMeshData { get { return navMeshData; } }
    public GameObject StagePrefab { get { return stagePrefab; } }
}
