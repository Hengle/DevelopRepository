using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DG.Tweening;

public class ButtonBlink : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    Button MyButton { get; set; }
    CanvasGroup MyCanvasGroup { get; set; }
    Vector3 OriginalScale { get; set; }


    void Awake()
    {
        MyButton = GetComponent<Button>();
        MyCanvasGroup = GetComponent<CanvasGroup>();

        MyCanvasGroup.DOFade(0.5f,1.0f).SetEase(Ease.InCubic).SetLoops(-1, LoopType.Yoyo);
    }

    public void OnPointerDown(PointerEventData eventData)
    {

        if (MyButton.enabled && MyButton.interactable)
        {
           
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (MyButton.enabled && MyButton.interactable)
        {
        }
    }
}
