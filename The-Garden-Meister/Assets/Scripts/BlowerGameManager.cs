using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BlowerGameManager : MonoBehaviour
{
    [SerializeField] GameObject[] stages;
    Leaf[] leaves;
    float leafCount = 0;
    Slider slider;
    public event Action<float> OnUpdateScoreListener;

    private void Awake()
    {
        slider = GameObject.FindGameObjectWithTag("ProgressBar").GetComponent<Slider>();
        Instantiate(stages[UnityEngine.Random.Range(0, stages.Length)]);
    }

    private void Start()
    {
        var leavesObj = GameObject.FindGameObjectsWithTag("Target");
        leaves = new Leaf[leavesObj.Length];
        for (int i = 0; i < leavesObj.Length; ++i)
        {
            leaves[i] = leavesObj[i].transform.GetComponent<Leaf>();
            leaves[i].OnBlowedListener += CountUpdate;
        }
    }

    void CountUpdate()
    {
        leafCount++;
        slider.value = (float)leafCount / leaves.Length;
        OnUpdateScoreListener?.Invoke(slider.value);
    }
}
