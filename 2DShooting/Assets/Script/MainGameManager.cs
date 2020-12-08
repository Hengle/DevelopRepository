using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameState
{
    Start,
    MainGame,
    Result,
}

public class MainGameManager : MonoBehaviour
{
    // Playerプレハブ
    [SerializeField] GameObject player;

    // Rockプレハブ
    [SerializeField] GameObject rock;

    //リザルト
    [SerializeField] Result result;

    // タイトル
    private GameObject title;

    private bool isDeath;
    private GameState gameState;

    private void Awake()
    {
        // Titleゲームオブジェクトを検索し取得する
        title = GameObject.Find("Title");
        isDeath = false;
        player.SetActive(false);
        player.GetComponent<Player>().OnDeathTrigger += DeathCheck;
    }

    private void Update()
    {
        switch (gameState)
        {
            case GameState.Start:
                GameStart();
                break;
            case GameState.MainGame:
                MainGame();
                break;
            case GameState.Result:
                Result();
                break;
        }
    }
    public void GameStart()
    {
        // クリックされたらゲームスタート
        if (Input.GetMouseButtonDown(0))
        {
            // ゲームスタート時に、タイトルを非表示にしてプレイヤーを作成する
            title.SetActive(false);
            player.SetActive(true);
            Instantiate(rock, rock.transform.position, rock.transform.rotation);

            gameState = GameState.MainGame;
        }
        
    }

    public void MainGame()
    {
        if (isDeath)
        {
            player.SetActive(false);
            result.Show(0);

            gameState = GameState.Result;
        }
    }
    public void Result()
    {
        if (Input.GetMouseButtonDown(0))
        {
            gameState = GameState.Start;
        }
    }

    private void DeathCheck(bool death)
    {
        isDeath = death;
    }
}
