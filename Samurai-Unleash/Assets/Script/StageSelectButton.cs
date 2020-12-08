using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StageSelectButton : MonoBehaviour
{

    [SerializeField] TextMeshProUGUI myText;
    [SerializeField] GameObject myMask;
    [SerializeField] Color enableColor;
    [SerializeField] Color completeColor;
    [SerializeField] Color nomalClearColor;
    [SerializeField] Sprite clearStar;
    [SerializeField] Image[] stars;
    Image myRenderer;
    Button myButton;
    int stageID;

    // Start is called before the first frame update
    void Awake()
    {
        myRenderer = GetComponent<Image>();
        myButton = GetComponent<Button>();
    }

    public void SetStatus(int stage) {
        stageID = stage;
        myText.text = stageID.ToString();
        CheckStage();
    }

    private void CheckStage() {

        // ステージ1がアンロックされてなければ明示的に解放
        if(stageID==1 && !DataManager.GetStageUnlock(1)) {
            DataManager.SetStageUnlock(1);
        }
        // もしくは自分の1個前のステージがClearされていたら自分をアンロック
        if (DataManager.GetStageClearCheck(stageID - 1)) {
            DataManager.SetStageUnlock(stageID);
        }

        // このステージはアンロックされているか？
        if (DataManager.GetStageUnlock(stageID)) {
            myMask.SetActive(false);

             // 星の数は？
            switch (DataManager.GetStageStar(stageID)) {
                case 3:
                    myRenderer.color = completeColor;
                    RefreshStar(3);
                    break;
                case 2:
                    myRenderer.color = nomalClearColor;
                    RefreshStar(2);
                    break;
                case 1:
                    myRenderer.color = nomalClearColor;
                    RefreshStar(1);
                    break;
                default:
                    myRenderer.color = enableColor;
                    RefreshStar(0);
                    break;
            }

        } else {
            myMask.SetActive(true);
            myButton.interactable = false;
        }


    }

    private void RefreshStar(int count) {

        for(int i=0; i<count; i++) {
            stars[i].sprite = clearStar;
        }

    }

    public void StageLoad() {

        var startDirector = GameObject.Find("StartDirector").GetComponent<StartDirector>();

        if (startDirector) {
            startDirector.StartSound();
        }

        GameDirector.SELECT_LEVEL = stageID;
        FadeManager.FadeOut(1);
    }

}
