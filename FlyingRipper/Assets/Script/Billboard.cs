using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Billboard : MonoBehaviour {

    Camera mainCam;
    private void Start() {
        mainCam = Camera.main;
    }

    void LateUpdate() {
        // 回転をカメラと同期させる
        transform.rotation = mainCam.transform.rotation;
    }
}
