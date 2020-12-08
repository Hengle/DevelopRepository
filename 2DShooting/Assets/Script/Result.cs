using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Result : MonoBehaviour
{
    [SerializeField] Canvas result;

    private void Awake()
    {
    }

    public void Show(int score)
    {
        
        var text = result.transform.Find("Score").GetComponent<Text>();

        text.text = score.ToString();

        var canvasGroup = result.GetComponent<CanvasGroup>();
        canvasGroup.alpha = 1;
        canvasGroup.blocksRaycasts = true;
    }

    public void Hide()
    {
        var canvasGroup = result.GetComponent<CanvasGroup>();
        canvasGroup.alpha = 0;
        canvasGroup.blocksRaycasts = false;
    }
}

