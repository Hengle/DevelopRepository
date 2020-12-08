using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Util;

public class UIWindow : MonoBehaviour
{
    [SerializeField] Image finger;
    [SerializeField] Text stageText;
    [SerializeField] Text displayText;
    [SerializeField] Image weaponTutorialImage;
    [SerializeField] Text weaponTutorialText;
    [SerializeField] CanvasGroupHandler canvasGroupHandler;
    TouchEventHandler touchEventHandler;

    const int weaponPliceMin = 500;
    public event Action OnWindowCloseListener;

    void Awake()
    {
        touchEventHandler = Camera.main.transform.GetComponent<TouchEventHandler>();
        stageText.text = "STAGE" + DataManager.Instance.NowStage;
        finger.transform.DOMove(new Vector3(1.0f, finger.transform.position.y, finger.transform.position.z),1.0f).SetEase(Ease.Linear).SetLoops(-1,LoopType.Restart);
        displayText.DOFade(0f,0.5f).SetEase(Ease.Linear).SetLoops(-1,LoopType.Yoyo);

        touchEventHandler.OnTouchKeepListener += TouchKeep;

        if(DataManager.Instance.WeaponTutorial && DataManager.Instance.Coin >= weaponPliceMin)
        {
            weaponTutorialImage.DOFade(1.0f, 0.5f).SetEase(Ease.Linear).SetLoops(-1, LoopType.Yoyo);
            weaponTutorialText.DOFade(1.0f, 0.0f);
        }
    }

    void TouchKeep(Vector3 position)
    {
        canvasGroupHandler.Close(0.0f);
        OnWindowCloseListener?.Invoke();
        touchEventHandler.OnTouchKeepListener -= TouchKeep;
    }
}
