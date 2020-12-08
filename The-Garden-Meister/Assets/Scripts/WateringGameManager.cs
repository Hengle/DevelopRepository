using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WateringGameManager : MonoBehaviour
{
    [SerializeField] GameObject[] stages;
    Slider slider;
    Flower[] flowers;
    int bloomedCount;
    public event Action<float> OnUpdateScoreListener;

    private void Awake()
    {
        slider = GameObject.FindGameObjectWithTag("ProgressBar").GetComponent<Slider>();
        Instantiate(stages[UnityEngine.Random.Range(0, stages.Length)]);
    }

    private void Start()
    {
        var flowersObj = GameObject.FindGameObjectsWithTag("Target");
        flowers = new Flower[flowersObj.Length];
        for (int i = 0; i < flowers.Length; i++)
        {
            flowers[i] = flowersObj[i].transform.GetComponent<Flower>();
            flowers[i].OnBloomedListener += CountUpdate;
        }
    }

    void CountUpdate()
    {
        bloomedCount++;
        slider.value = (float)bloomedCount / flowers.Length;
        OnUpdateScoreListener?.Invoke(slider.value);
    }
}
