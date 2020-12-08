using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;
using Util;
using UnityEngine.UI;
using UnityEngine.AI;
using SDKManager;

public class MainGameManager : MonoBehaviour
{
    public enum GameState
    {
        Home,
        Play,
        GameClear,
        GameOver
    }

    [SerializeField] CameraManager cameraManager;
    [SerializeField] MultipleParticles resultParticles;
    [SerializeField] GameObject resultUI;
    [SerializeField] GameObject resultGameOverUI;
    [SerializeField] Image emargency;
    [SerializeField] CanvasGroup bossHpGauge;
    [SerializeField] StageMaster stageMaster;
    [SerializeField] UIWindow uiWindow;
    [SerializeField] Coin coin;
    [SerializeField] Button nextButton;

    int round = 0;
    int nowStage;
    bool isAnimation;
    bool isSetting;
    bool isSelecting;
    Player player;
    List<GameObject> generatorList;
    List<GameObject> enemyList;
    GameState gameState = GameState.Home;
    NavMeshDataInstance instance = new NavMeshDataInstance();

    private void Awake()
    {
        uiWindow.OnWindowCloseListener += () =>
        {
            gameState = GameState.Play;
            coin.transform.GetComponent<CanvasGroup>().alpha = 0f;
        };

        //ステージの進行度読み込み
        nowStage = DataManager.Instance.NowStage;
        StageData stageData = stageMaster.GetStageData(nowStage);
        instance.Remove();
        instance = NavMesh.AddNavMeshData(stageData.NavMeshData);
        var stage = Instantiate(stageData.StagePrefab);

        player = GameObject.FindGameObjectWithTag("Player").transform.GetComponent<Player>();
        generatorList = new List<GameObject>(GameObject.FindGameObjectsWithTag("Generator"));
        enemyList = new List<GameObject>(GameObject.FindGameObjectsWithTag("Enemy"));
        cameraManager.SwitchFollowTarget(player.gameObject);

        player.OnDeadListener += PlayerDead;

        foreach (var generator in generatorList)
        {
            var generatorScript = generator.GetComponent<EnemyGenerator>();
            generatorScript.OnGeneratEnemy += GeneratEnemy;
            generatorScript.OnEndEnemyGenerator += AnnihilationCheck;
        }

        foreach (var enemy in enemyList)
        {
            var enemyScript = enemy.GetComponent<Enemy>();
            enemyScript.OnDeathEnemy += AnnihilationCheck;
            enemyScript.enabled = false;
        }

        StartCoroutine(MainRoutine());
    }

    private void Start()
    {
        AudioManager.Instance.LoadSe("alert");
        AudioManager.Instance.LoadSe("coin_change");
        AudioManager.Instance.LoadSe("coin_get");
        AudioManager.Instance.LoadSe("weapon_spear");
        AudioManager.Instance.LoadSe("weapon_sword");
        AudioManager.Instance.LoadSe("window_close");
        AudioManager.Instance.LoadSe("window_open");
        AudioManager.Instance.LoadSe("zombie_appearance");
        AudioManager.Instance.LoadSe("zombie_bite");
        AudioManager.Instance.LoadSe("zombie_roar");
        AudioManager.Instance.LoadSe("weapon_axe");
    }

    void GameStart()
    {
        player.Activate();
        foreach (var generator in generatorList)
        {
            var generatorScript = generator.GetComponent<EnemyGenerator>();
            generatorScript.Activate();
        }

        foreach (var enemy in enemyList)
        {
            var enemyScript = enemy.GetComponent<Enemy>();
            enemyScript.enabled = true;
        }
    }

    IEnumerator MainRoutine()
    {

        while (true)
        {
            yield return new WaitWhile(() => gameState == GameState.Home);

            GameStart();

            yield return new WaitWhile(() => gameState == GameState.Play);

            player.Deactivate();

            break;
        }

        if (gameState == GameState.GameClear)
        {
            resultParticles.Play();
        }

        yield return StartCoroutine(Complete());

        SetUPResult();
        ShowResult();
    }

    IEnumerator Complete()
    {
        yield return new WaitForSeconds(1.0f);

        foreach (var enemy in enemyList)
        {
            var enemyScript = enemy.GetComponent<Enemy>();
            if (enemyScript.GetDeath())
            {
                enemyScript.EnemyChangeCoin();
            }
        }
        AudioManager.Instance.PlaySE("coin_change");

        yield return new WaitForSeconds(0.1f);  
    }

    void PlayerDead()
    {
        gameState = GameState.GameOver;

        foreach (var generator in generatorList)
        {
            var generatorScript = generator.GetComponent<EnemyGenerator>();
            generatorScript.Stop();
        }

        foreach (var enemy in enemyList)
        {
            var enemyScript = enemy.GetComponent<Enemy>();
            enemyScript.Stop();
        }
    }

    void GeneratEnemy(Enemy enemy)
    {
        if (enemy.GetEnemyType() == Enemy.EnemyType.Boss)
        {
            AudioManager.Instance.PlaySE("alert");
            emargency.DOFade(0.8f, 0.8f).SetLoops(4, LoopType.Yoyo);
            bossHpGauge.DOFade(1.0f, 1.0f).SetEase(Ease.Linear);
            enemy.OnDeathEnemy += ForcedAnnihilation;
            enemyList.Add(enemy.gameObject);
        }
        else
        {
            enemy.OnDeathEnemy += AnnihilationCheck;
            enemyList.Add(enemy.gameObject);
        }
    }

    void AnnihilationCheck()
    {
        var isAnnihilation = true;

        foreach (var generator in generatorList)
        {
            var generatorScript = generator.GetComponent<EnemyGenerator>();
            if (generatorScript.GetActive())
            {
                isAnnihilation = false;
            }
        }

        foreach (var enemy in enemyList)
        {
            var enemyScript = enemy.GetComponent<Enemy>();
            if (!enemyScript.GetDeath())
            {
                isAnnihilation = false;
            }
        }

        if (isAnnihilation) {
            gameState = GameState.GameClear;
        }
    }

    void ForcedAnnihilation()
    {
        gameState = GameState.GameClear;

        foreach (var generator in generatorList)
        {
            var generatorScript = generator.GetComponent<EnemyGenerator>();
            if (generatorScript.GetActive())
            {
                generatorScript.Stop();
            }
        }

        foreach (var enemy in enemyList)
        {
            var enemyScript = enemy.GetComponent<Enemy>();
            if (!enemyScript.GetDeath())
            {
                enemyScript.SetDeathType(Enemy.DeathType.Down);
                enemyScript.Damage(999999);
            }
        }
    }

    void SetUPResult()
    {
        if (gameState == GameState.GameClear)
        {
            var text = resultUI.transform.Find("Text").GetComponent<Text>();
            text.text = "STAGE " + nowStage + "\nCOMPLETED";

            var monay = resultUI.transform.Find("MoneyBase");
            var moneyText = monay.Find("Text").GetComponent<Text>();
            moneyText.text = (coin.coinCount - DataManager.Instance.Coin).ToString();

            var button = resultUI.transform.Find("Button");
            var buttonText = button.Find("Text").GetComponent<Text>();
            buttonText.text = "NEXT";

            DataManager.Instance.ChangeNowStage(nowStage+1);
            FacebookManager.Instance.FacebookStageClearEvent(DataManager.Instance.NowStage);
        }

        if (gameState == GameState.GameOver)
        {
            var text = resultUI.transform.Find("Text").GetComponent<Text>();
            text.text = "STAGE " + nowStage + "\nFAILED";

            var monay = resultUI.transform.Find("MoneyBase");
            var moneyText = monay.Find("Text").GetComponent<Text>();
            moneyText.text = (coin.coinCount - DataManager.Instance.Coin).ToString();

            var button = resultUI.transform.Find("Button");
            var buttonText = button.Find("Text").GetComponent<Text>();
            buttonText.text = "RETRY";

            FacebookManager.Instance.FacebookStageRetryEvent(DataManager.Instance.NowStage);
        }

        DataManager.Instance.ChangeCoin(coin.coinCount);
        DataManager.Instance.Save();
    }

    void ShowResult()
    {
        resultUI.transform.DOLocalMove(new Vector3(0, 0, 0), 1.5f)
            .SetEase(Ease.OutBounce)
            .OnComplete(ShowButton);
    }

    void ShowButton()
    {
        var button = resultUI.transform.Find("Button");
        button.GetComponent<CanvasGroup>().DOFade(1.0f, 0.5f).OnComplete(() => button.GetComponent<Button>().enabled = true);
    }

    public void NextScene()
    {
        nextButton.enabled = false;
        DataManager.stagePlayCount++;
        if (DataManager.stagePlayCount == 3)
        {
            DataManager.stagePlayCount = 0;
            AppLovinManager.Instance.AppLovinShowInterstitial(() => SceneManager.Instance.ChangeScene("MainGame"));
        }
        else
        {
            SceneManager.Instance.ChangeScene("MainGame");
        }
    }

    private void OnDestroy()
    {
        NavMesh.RemoveNavMeshData(instance);
    }
}
