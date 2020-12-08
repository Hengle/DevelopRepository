using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[CreateAssetMenu(menuName = "ScriptableObject/Create DecoretPatternMaster", fileName = "DecoretPatternMaster")]
public class DecoretPatternMaster : MasterData
{
    [SerializeField] private List<DecoretPatternData> DecoretPatternList = new List<DecoretPatternData>();

    public List<DecoretPatternData> GetDecoretPatternList()
    {
        return DecoretPatternList;
    }

    public DecoretPatternData GetDecoretPattern(int id)
    {
        return DecoretPatternList.Find((p) => p.Id == id);
    }

    public override void Validation()
    {
        var isDuplicate = DecoretPatternList.GroupBy(p => p.Id).Where(g => g.Count() > 1).Select(g => g.Key).Any();

        if (isDuplicate)
        {
            Debug.LogError("キーが重複しています。");
        }

        var minId = DecoretPatternList.Where(x => !DecoretPatternList.Contains(DecoretPatternList.Find(y => y.Id == x.Id + 1))).Select(x => x.Id + 1).Min();
        if (minId != DecoretPatternList.Count + 1)
        {
            Debug.LogError(minId + ":キーが連続していません。");
        }
    }
}

[System.Serializable]
public class DecoretPatternData
{
    [SerializeField] int id;
    [SerializeField] AccessoryPosition accessoryPosition;
    [SerializeField] Sprite[] sprites;
    [SerializeField] GameObject[] objects;
    [SerializeField] Color[] color;
    [SerializeField] Color[] subColor;

    public int Id { get { return id; } }
    public AccessoryPosition AccessoryPosition { get { return accessoryPosition; } }
    public Sprite[] Sprites { get { return sprites; } }
    public GameObject[] Objects { get { return objects; } }
    public Color[] Color { get { return color; } }
    public Color[] SubColor { get { return subColor; } }
}