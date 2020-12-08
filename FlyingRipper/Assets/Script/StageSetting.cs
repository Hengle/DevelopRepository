using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageSetting : MonoBehaviour {

    [SerializeField, HeaderAttribute("■ プレイヤーSpawn位置")]
    Transform playerSpawnPos;

    [SerializeField, HeaderAttribute("■ 倒すべきターゲット（手前から順番に登録）")]
    List<GameObject> stageEnemy = new List<GameObject>();

    [SerializeField, HeaderAttribute("■ コインオブジェクト")]
    List<GameObject> stageCoin = new List<GameObject>();

    [SerializeField, HeaderAttribute("■ 鍵オブジェクト")]
    List<GameObject> stageKey = new List<GameObject>();

    [SerializeField, HeaderAttribute("■ このステージはボスを倒すステージかどうか")]
    bool isBoss;

    [SerializeField, HeaderAttribute("■ ボスの手配書イメージ")]
    Sprite bossImage;

    public Vector3 GetPlayerSpawnPos() {
        return playerSpawnPos.position;
    }

    public List<GameObject> GetStageEnemy() {
        return stageEnemy;
    }

    public List<GameObject> GetStageCoin() {
        return stageCoin;
    }

    public List<GameObject> GetStageKey() {
        return stageKey;
    }

    public bool GetStageIsBoss() {
        return isBoss;
    }

    public Sprite GetBossImage() {
        return bossImage;
    }

}
