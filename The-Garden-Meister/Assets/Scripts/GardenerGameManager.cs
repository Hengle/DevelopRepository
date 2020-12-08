using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class GardenerGameManager : MonoBehaviour
{
    [SerializeField] GameObject[] stages;
    Slider slider;
    Leaf[] leaves;
    int cutedLeafCount;
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
            leaves[i].OnCutedListener += CountUpdate;
        }
    }

    void CountUpdate()
    {
        cutedLeafCount++;
        slider.value = (float)cutedLeafCount / leaves.Length;
        OnUpdateScoreListener?.Invoke(slider.value);
    }
}
