using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[CreateAssetMenu(menuName = "ScriptableObject/Create DrugMaster", fileName = "DrugMaster")]
public class DrugMaster : MasterData
{
    [SerializeField] private List<DrugData> drugList = new List<DrugData>();
    [SerializeField] GameObject drug;

    public List<DrugData> GetDrugList()
    {
        return drugList;
    }

    public Material GetMaterialByDrugType(DrugType drugType)
    {
        return drugList.Find((p) => p.DrugType == drugType).Material;
    }

    public override void Validation()
    {
        var isDuplicate = drugList.GroupBy(p => p.Id).Where(g => g.Count() > 1).Select(g => g.Key).Any();

        if (isDuplicate)
        {
            Debug.LogError("キーが重複しています。");
        }

        var minId = drugList.Where(x => !drugList.Contains(drugList.Find(y => y.Id == x.Id + 1))).Select(x => x.Id + 1).Min();
        if (minId != drugList.Count + 1)
        {
            Debug.LogError(minId + ":キーが連続していません。");
        }
    }
}

[System.Serializable]
public class DrugData
{
    [SerializeField] int id;
    [SerializeField] DrugType drugType;
    [SerializeField] Material material;

    public int Id { get { return id; } }
    public DrugType DrugType { get { return drugType; } }
    public Material Material { get { return material; } }
}
