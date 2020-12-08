using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartScene : MonoBehaviour
{
    void Start()
    {
        int loadStage = DataManager.Instance.Stage % 3;
        if (loadStage == 0) loadStage = 3; 
        SceneManager.Instance.ChangeScene("Stage" + loadStage);
    }
}
