using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DebugCamera : MonoBehaviour {

    private void Start() {
        GetComponentInChildren<Text>().text = GameDirector.debugCamera ? "カメラ追従OFF" : "カメラ追従ON";
    }

    public void DebugCameraChange() {
        GameDirector.debugCamera = !GameDirector.debugCamera;
        GetComponentInChildren<Text>().text = GameDirector.debugCamera ? "カメラ追従OFF" : "カメラ追従ON";
    }

}
