using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Util;
using TMPro;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [SerializeField] GameObject homeCanvas;
    [SerializeField] GameObject resultCanvas;
    [SerializeField] GameObject resultCamera;
    [SerializeField] GameObject rankingCanvas;
    [SerializeField] GameObject gameOverCanvas;
    [SerializeField] GameObject[] stageList;
    [SerializeField] TextMeshProUGUI stageText;
    [SerializeField] Image rankImg;
    [SerializeField] TextMeshProUGUI playerRank;
    [SerializeField] MultipleParticles resultParticle;

    GameObject player;
    CharacterManager characterManager;
    bool isStart,isGoal, isGameOver;
    int stageNumber;

    private void Awake()
    {
        stageNumber = DataManager.Instance.Stage;
        stageText.text = stageNumber.ToString();
        Instantiate(stageList[stageNumber % 2 == 1 ? 0 : 1]);
    }

    void Start()
    {
        characterManager = GetComponent<CharacterManager>();
        player = GameObject.FindGameObjectWithTag("Player");
        player.GetComponent<Character>().OnGoalListener += Goal;
        player.GetComponent<Character>().OnDeathListener += GameOver;
    }

    void Update()
    {
        if(!isStart && Input.GetMouseButtonDown(0))
        {
            isStart = true;
            homeCanvas.SetActive(false);
            characterManager.ActiveCharqacters();
        }

        if (!isGoal || !isGameOver) return; 
    }

    void Goal()
    {
        if (isGoal) return;
        isGoal = true;
        var spr = Resources.Load<Sprite>("Rank/rank_"+ playerRank.text[0]);
        rankImg.sprite = spr;
        resultCanvas.SetActive(true);
        resultCamera.SetActive(true);
        rankingCanvas.SetActive(false);
        resultParticle.Play();
        DataManager.Instance.AddStageCount();
        DataManager.Instance.Save();
    }

    void GameOver()
    {
        if (isGameOver) return;
        isGameOver = true;
        gameOverCanvas.SetActive(true);
        rankingCanvas.SetActive(false);
    }

    public void NextStage()
    {
        SceneManager.Instance.ChangeScene("Cinemachine");
    }
}
