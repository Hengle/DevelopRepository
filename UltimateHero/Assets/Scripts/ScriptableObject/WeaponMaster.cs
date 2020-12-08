using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

[CreateAssetMenu(menuName = "ScriptableObject/Create WeaponMaster", fileName = "WeaponMaster")]
public class WeaponMaster : MasterData
{
    [SerializeField] private List<WeaponData> WeaponList = new List<WeaponData>();

    public List<WeaponData> GetWeaponList()
    {
        return WeaponList;
    }

    public WeaponData GetWeaponData(int stageId)
    {
        return WeaponList.Find((p) => p.Id == stageId);
    }

    public override void Validation()
    {
        var isDuplicate = WeaponList.GroupBy(p => p.Id).Where(g => g.Count() > 1).Select(g => g.Key).Any();

        if (isDuplicate)
        {
            Debug.LogError("キーが重複しています。");
        }

        var minId = WeaponList.Where(x => !WeaponList.Contains(WeaponList.Find(y => y.Id == x.Id + 1))).Select(x => x.Id + 1).Min();
        if (minId != WeaponList.Count + 1)
        {
            Debug.LogError(minId + ":キーが連続していません。");
        }
    }
}

[System.Serializable]
public class WeaponData
{
    [SerializeField] int id;
    [SerializeField] int price;
    [SerializeField] Sprite icon;
    [SerializeField] GameObject prefab;

    public int Id { get { return id; } }
    public int Price { get { return price; } }
    public Sprite Icon { get { return icon; } }
    public GameObject Prefab { get { return prefab; } }
}
