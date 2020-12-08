using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChapterMaster : MonoBehaviour {

    [SerializeField]
    List<ChapterData> chapterData = new List<ChapterData>();

    [System.Serializable]
    public class ChapterData {
        public string name;
        public int chapterNo;
        public List<GameObject> chapterStageDataList = new List<GameObject>();

        public int GetChapterNo() {
            return chapterNo;
        }

        public List<GameObject> GetChapterStageDataList() {
            return chapterStageDataList;
        }
    }

    int loadedStageChapter = 0;
    int loadedStageChapterPhaseCount = 0;
    int loadedStageChapterPhaseMyCount = 0;

    public List<ChapterData> GetChapterData() {
        return chapterData;
    }

    public GameObject GetSelectStageData(int selectStage) {

        // 返すデータ
        GameObject stageData = null;

        // レベル
        int _level = selectStage;
        int _selectLevel = _level;

        // テスト
        var _chapterData = GetChapterData();

        foreach(ChapterData obj in _chapterData) {

            var fuga = obj.GetChapterStageDataList();

            Debug.Log(selectStage + " が 選択されたレベルです");

            if (_selectLevel <= fuga.Count) {
                // 見つかった最初のデータを取得
                stageData = fuga[_selectLevel - 1];

                // ステージ数と、自分の位置をあわせて取得
                loadedStageChapterPhaseCount = fuga.Count; // 3ステージ構成なら3が入る
                loadedStageChapterPhaseMyCount = _selectLevel - 1; // 自分の位置が入る、4なら 4-3 で1 からの 1-1 で0となる

                break;
            } else {
                loadedStageChapter++; // Chapter数を増やす
                _selectLevel -= fuga.Count;
            }

        }
        
        if(stageData == null) { Debug.LogError("ChapterMasterでステージデータを取得できません"); }
        return stageData;
    }

    public int GetChapterCount() {
        return loadedStageChapter;
    }

    public int GetLoadedStageChapterPhaseCount() {
        return loadedStageChapterPhaseCount;
    }

    public int GetLoadedStageMyPhaseCount() {
        return loadedStageChapterPhaseMyCount;
    }
}
