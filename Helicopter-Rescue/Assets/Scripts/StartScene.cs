using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartScene : MonoBehaviour
{
    void Start()
    {
        if (DataManager.Instance.NowStage == 1)
        {
            SceneManager.Instance.ChangeScene("TPS");
        }
        else
        {
            SceneManager.Instance.ChangeScene("FPS");
        }
    }
}
