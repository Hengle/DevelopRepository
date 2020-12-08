using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class HomeGameManager : MonoBehaviour
{
    [SerializeField] GameObject female;
    [SerializeField] Transform popup;
    GameType gameType;

    private void Start()
    {
        gameType = DataManager.Instance.StageList[DataManager.Instance.NowStage];

        var animator = female.transform.GetComponent<Animator>();
        var sequence = DOTween.Sequence();
        sequence.Append(female.transform.DOMoveZ(-5.0f, 2.0f).OnComplete(() => animator.Play("Shrug")));
        sequence.Append(popup.DOScale(Vector3.one, 0.5f).SetEase(Ease.InOutSine));
    }

    public void Ready()
    {
        switch (gameType)
        {
            case GameType.Blower:
                SceneManager.Instance.ChangeScene("Blower", 0.5f);
                break;
            case GameType.Gardener:
                SceneManager.Instance.ChangeScene("Gardener", 0.5f);
                break;
            case GameType.Watering:
                SceneManager.Instance.ChangeScene("Watering", 0.5f);
                break;
            case GameType.Harvest:
                SceneManager.Instance.ChangeScene("Harvest", 0.5f);
                break;
        }
    }
}
