using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainGameManager : MonoBehaviour
{
    [SerializeField] List<GameObject> stageList;
    [SerializeField] Life lifeGage;
    [SerializeField] GameObject result;

    GameObject nowStage;
    int stageLevel;
    GameObject player;
    GameObject target;

    private bool isGameClear;
    private bool isGameOver;
    private bool isMoveEnd;

    void Awake()
    {
        stageLevel = 0;
        nowStage = Instantiate(stageList[0]);
        player = nowStage.transform.Find("Player").gameObject;
        player.GetComponent<Player>().OnMovwEndListener += PlayerMoveEnd;
        player.GetComponent<Player>().OnDeathListener += GameOver;
        target = nowStage.transform.Find("Target").gameObject;
        target.GetComponent<ParentColiider>().OnSurroundedListener += Clear;
        lifeGage.Initialize(player.GetComponent<Player>().MaxLife());
    }

    void Start()
    {
        StartCoroutine(MainRoutine());
    }

    IEnumerator MainRoutine()
    {
        while (true)
        {
            isMoveEnd = false;
            player.GetComponent<Player>().Refresh();

            yield return new WaitWhile(() => !isMoveEnd);

            yield return new WaitForSeconds(1.5f);

            player.GetComponent<Player>().DrawPolygon();

            yield return new WaitForSeconds(1.5f);

            if (!isGameClear)
            {
                lifeGage.Reduce();
                player.GetComponent<Player>().Failure();
                target.GetComponent<ParentColiider>().Refresh();
            }

            if (isGameClear || isGameOver)
            {
                ShowResult();
                break;
            }
        }
    }

    private void PlayerMoveEnd()
    {
        isMoveEnd = true;
    }

    private void Clear()
    {
        isGameClear = true;
    }

    private void GameOver()
    {
        isGameOver = true;
    }

    private void ShowResult()
    {
        result.transform.Find("BlackMask").gameObject.SetActive(true);

        if (isGameClear)
        {
            var display = result.transform.Find("GameClear").gameObject;
            display.SetActive(true);
            display.transform.Find("Character").gameObject.GetComponent<Image>().sprite = Resources.Load<Sprite>(target.GetComponent<SpriteRenderer>().sprite.name + "_clear");

        }

        if (isGameOver)
        {
            result.transform.Find("GameOver").gameObject.SetActive(true);
        }
    }

    public void Retry()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    public void NextStage()
    {
        if (stageLevel + 1 == stageList.Count) return;
        isGameClear = false;
        result.transform.Find("BlackMask").gameObject.SetActive(false);
        result.transform.Find("GameClear").gameObject.SetActive(false);
        Destroy(nowStage);
        stageLevel++;
        nowStage = Instantiate(stageList[stageLevel]);
        player = nowStage.transform.Find("Player").gameObject;
        player.GetComponent<Player>().OnMovwEndListener += PlayerMoveEnd;
        player.GetComponent<Player>().OnDeathListener += GameOver;
        target = nowStage.transform.Find("Target").gameObject;
        target.GetComponent<ParentColiider>().OnSurroundedListener += Clear;
        lifeGage.Initialize(player.GetComponent<Player>().MaxLife());
        StartCoroutine(MainRoutine());
    }

}
