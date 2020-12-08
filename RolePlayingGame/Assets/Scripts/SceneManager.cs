using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RolePlayingGame
{
    public enum Scene
    {
        Title,
        StageSelect,
        MainGame,
        Ending
    }
    public class SceneManager : SingletonMonoBehaviour<SceneManager>
    {
        public void ChangeNextScene(Scene scene)
        {
            switch (scene)
            {
                case Scene.Title:
                    //currentScene = Scene.StageSelect;
                    UnityEngine.SceneManagement.SceneManager.LoadScene("MainGame");
                    break;
                case Scene.StageSelect:
                    //currentScene = Scene.MainGame;
                    UnityEngine.SceneManagement.SceneManager.LoadScene("MainGame");
                    break;
                case Scene.MainGame:
                    //currentScene = Scene.Result;
                    UnityEngine.SceneManagement.SceneManager.LoadScene("MainGame");
                    break;
                case Scene.Ending:
                    //currentScene = Scene.Title;
                    UnityEngine.SceneManagement.SceneManager.LoadScene("MainGame");
                    break;
            }
        }
    }
}
