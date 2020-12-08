using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainGameManager : MonoBehaviour
{
    public void Retry()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void LowSpeed()
    {
        SceneManager.LoadScene("MainGame");
    }

    public void HighSpeed()
    {
        SceneManager.LoadScene("MainGame2");
    }
}
