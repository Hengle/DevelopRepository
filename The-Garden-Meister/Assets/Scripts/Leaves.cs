using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class Leaves : MonoBehaviour
{
    SpriteRenderer spriteRenderer;
    float colorG;
    float noTouchTime;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        colorG = 1.0f;
    }

    private void Update()
    {
        if (noTouchTime >= 1.0f)
        {
            colorG = Mathf.Min(colorG + Time.deltaTime, 1.0f);
            spriteRenderer.color = new Color(1.0f, colorG, 1.0f);
        }
        else
        {
            noTouchTime += Time.deltaTime;
        }

        if (colorG <= 0.0f)
        {
            colorG = 1.0f;
            spriteRenderer.color = new Color(1.0f, colorG, 1.0f);
            Camera.main.transform.DOShakePosition(0.5f);
            //SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }
    public void TouchLeaves()
    {
        noTouchTime = 0f;
        colorG = Mathf.Max(colorG - Time.deltaTime, 0.0f);

        spriteRenderer.color = new Color(1.0f, colorG, 1.0f);
    }
}
