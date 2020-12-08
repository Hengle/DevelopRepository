using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageSetting : MonoBehaviour
{
    [SerializeField] int bladeCount; // このステージで利用できる刀の数
    [SerializeField] int bestBladeCount; // 理想の刀使用数（星３）
    [SerializeField] type cameraType; // ステージのカメラタイプ
    [SerializeField] bool isBossStage;

    enum type {
        NOMAL,
        TYPE2,
    }

    public bool GetIsBossStage() {
        return isBossStage;
    }

    public int GetBladeCount() {
        return bladeCount;
    }

    public int GetBestBladeCount() {
        return bestBladeCount;
    }

    /// <summary>
    /// カメラタイプを数値で返す （0=通常 1～3 縮小カメラ）
    /// </summary>
    /// <returns>0=通常 1～3 縮小カメラ</returns>
    public int GetCameraType() {

        int i = 0;
        switch (cameraType) {
            case type.TYPE2:
                i = 1;
                break;
            default:
                i = 0;
                break;
        }

        return i;
    }

}
