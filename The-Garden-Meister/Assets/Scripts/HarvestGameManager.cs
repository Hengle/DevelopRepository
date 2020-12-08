using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class HarvestGameManager : MonoBehaviour
{
    [SerializeField] GameObject[] stages;
    Slider slider;
    Wood wood;
    Fruit[] fruits;
    int dropCount;
    int fruitCount = 0;
    public event Action<float> OnUpdateScoreListener;

    void Awake()
    {
        slider = GameObject.FindGameObjectWithTag("ProgressBar").GetComponent<Slider>();
        Instantiate(stages[UnityEngine.Random.Range(0, stages.Length)]);
        dropCount = 0;
    }

    void Start()
    {
        wood = GameObject.FindGameObjectWithTag("Wood").GetComponent<Wood>();
        wood.OnShakeListener += Shake;

        var fruitsObj = GameObject.FindGameObjectsWithTag("Target").OrderBy(i => System.Guid.NewGuid()).ToArray();
        fruits = new Fruit[fruitsObj.Length];
        for (int i = 0; i < fruits.Length; i++)
        {
            fruits[i] = fruitsObj[i].transform.GetComponent<Fruit>();
            fruits[i].OnDropListener += DropFruit;
            fruits[i].OnHarvestListener += CountUpdate;
        }
    }

    public void Shake(float changeAmount)
    {
        if (dropCount >= fruits.Length) return;
        fruits[dropCount].DropCheck(changeAmount);
    }

    void DropFruit()
    {
        dropCount++;
    }

    void CountUpdate()
    {
        fruitCount++;
        slider.value = (float)fruitCount / fruits.Length;
        OnUpdateScoreListener?.Invoke(slider.value);
    }
}
