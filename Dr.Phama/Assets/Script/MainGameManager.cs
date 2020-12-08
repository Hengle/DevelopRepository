using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Util;
using DG.Tweening;

public class MainGameManager : MonoBehaviour
{
    [SerializeField] DrugController drugController;
    [SerializeField] Dictionary<DrugType, Material> drugList;
    [SerializeField] GameObject foil;
    [SerializeField] GameObject pressMachine;
    [SerializeField] GameObject pressParts;
    [SerializeField] GameObject pressButton;
    [SerializeField] PressGauge pressGauge;
    [SerializeField] Result result;
    [SerializeField] MultipleParticles particles;
    [SerializeField] SpriteRenderer pressAfter;

    List<DrugType> requestDrugList = new List<DrugType>();
    List<DrugType> answerDrugList = new List<DrugType>();

    bool isPress;
    bool isPressSuccse;
    bool isPressAnimation;
    int rank;

    void Awake()
    {
        drugController.OnGeneratDrugListener += GeneratDrug;
        CreateRequestDrugList();
    }

    void Start()
    {
        StartCoroutine(MainRoutine());
    }

    IEnumerator MainRoutine()
    {
        while (true)
        {
            yield return new WaitWhile(() => 1 != answerDrugList.Count);

            SetActivePressButton(true);

            yield return new WaitWhile(() => !isPress);

            //※Pressボタン押されたときオブジェクトも消す
            //滑り移動での錠剤袋inバグ
            drugController.Deactivate();
            SetActivePressButton(false);

            yield return new WaitForSeconds(1.0f);

            yield return StartCoroutine(PressProduction());

            yield return new WaitForSeconds(1.0f);

            CalcScore();

            ShowResult();
            break;
        }
    }

    //プレス演出
    IEnumerator PressProduction()
    {
        pressMachine.transform.DOMove(new Vector3(pressMachine.transform.position.x, 3.5f, pressMachine.transform.position.z), 0.5f);
        yield return new WaitForSeconds(0.5f);

        foil.transform.DOMove(new Vector3(foil.transform.position.x, 3f, foil.transform.position.z), 0.5f);
        yield return new WaitForSeconds(0.5f);

        isPressAnimation = true;
        pressGauge.PlayAnimation(PressAnimationEnd);
        yield return new WaitWhile(() => isPressAnimation);

        pressParts.transform.DOLocalRotate(new Vector3(0f, 60f, 0f), 0.5f, RotateMode.LocalAxisAdd).SetLoops(2, LoopType.Yoyo);
        yield return new WaitForSeconds(1.0f);

        pressAfter.enabled = true;

        foil.transform.DOMove(new Vector3(foil.transform.position.x, 1.0f, foil.transform.position.z), 0.5f);
        yield return new WaitForSeconds(0.5f);

        pressMachine.transform.DOMove(new Vector3(pressMachine.transform.position.x, 10.0f, pressMachine.transform.position.z), 0.5f);
        yield return new WaitForSeconds(0.5f);
    }

    void SetActivePressButton(bool active)
    {
        pressButton.SetActive(active);
    }

    void CreateRequestDrugList()
    {
        //for (int i = 0; i < 3; i++)
        //{
            //var drug = EnumExtensions.Random<DrugType>();
            //requestDrugList.Add(drug);
        //}
        requestDrugList.Add(DrugType.Eve);
        requestDrugList.Add(DrugType.Eve);
        requestDrugList.Add(DrugType.Bufferin);
    }

    void GeneratDrug(Drug drug)
    {
        drug.OnDrugFallListener += (drugType) => {
            answerDrugList.Add(drugType);

            //５個入れたら操作不能
            //if (answerDrugList.Count == 5) drugController.Deactivate();
        };
    }

    void CalcScore()
    {
        //ソート
        requestDrugList = requestDrugList.OrderBy(drugType => (drugType)).ToList();
        answerDrugList = answerDrugList.OrderBy(drugType => (drugType)).ToList();

        //答え合わせ
        rank = 1;
        if (requestDrugList.SequenceEqual(answerDrugList))
        {
            rank = 3;
        }
        else if(requestDrugList.Count == answerDrugList.Count || requestDrugList.GroupBy(drugType => drugType).Select(group => group.Key).ToList().SequenceEqual(answerDrugList.GroupBy(drugType => drugType).Select(group => group.Key).ToList()))
        {
            rank = 2;
        }
        else
        {
            rank = 1;
        }
    }

    void ShowResult()
    {
        result.Open(rank);
    }

    void PressAnimationEnd(bool isSuccse)
    {
        isPressSuccse = isSuccse;
        isPressAnimation = false;
    }

    public void Press()
    {
        isPress = true;
    }
}
