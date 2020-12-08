using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class Coin : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI coinText;

    public int coinCount { get; private set; }

    void Awake()
    {
        //coinCount = DataManager.Instance.Coin;
        //DataManager.Instance.OnChangeCoinCountListener += RefreshText;
        //RefreshText();
    }

    private void Update()
    {
        transform.Rotate(new Vector3(0f,1f,0f));
    }
    void OnDestroy()
    {
        //DataManager.Instance.OnChangeCoinCountListener -= RefreshText;
    }

    public void Add(int amount)
    {
        coinCount += amount;
    }

    public void RefreshText()
    {
        coinCount = DataManager.Instance.Coin;
        coinText.text = coinCount.ToString();
    }

    private void OnTriggerEnter(Collider other)
    {
        Destroy(gameObject);
    }
}
