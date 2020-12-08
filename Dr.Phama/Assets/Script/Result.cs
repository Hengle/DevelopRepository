using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class Result : MonoBehaviour
{
    [SerializeField] GameObject stars;
    [SerializeField] Sprite sprStar;

    CanvasGroup myCanvasGroup;

    void Awake()
    {
        myCanvasGroup = GetComponent<CanvasGroup>();
        myCanvasGroup.alpha = 0;
        myCanvasGroup.interactable = false;
        myCanvasGroup.blocksRaycasts = false;

        foreach (Transform star in stars.transform)
        {
            star.localScale = Vector3.zero;
        }
    }

    public void Open(int rank)
    {
        myCanvasGroup.alpha = 1;
        myCanvasGroup.interactable = true;
        myCanvasGroup.blocksRaycasts = true;
        Sequence sequence = DOTween.Sequence();

        foreach (Transform star in stars.transform)
        {
            if (rank >= 1)
            {
                star.GetComponent<Image>().sprite = sprStar;
                rank--;
            }

            sequence.Append(star.DOScale(1.0f, 1.0f));
            sequence.Join(star.DORotate(new Vector3(0.0f, 0.0f, 360.0f), 1.0f, RotateMode.FastBeyond360));
        }
    }
}
