using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class Coin : MonoBehaviour
{
    [SerializeField] GameObject objCoinPrefab;
    [SerializeField] Text coinText;

    public int coinCount { get; private set; }

    void Awake()
    {
        coinCount = DataManager.Instance.Coin;
        DataManager.Instance.OnChangeCoinCountListener += RefreshText;
        RefreshText();
    }

    void OnDestroy()
    {
        DataManager.Instance.OnChangeCoinCountListener -= RefreshText;
    }

    public void Add(int amount, Vector3 position)
    {
        var objCoin = Instantiate(objCoinPrefab, position.SetY(0.5f), Quaternion.identity);

        Sequence coinAnimation = DOTween.Sequence();
        coinAnimation.Append(objCoin.transform.DOJump(objCoin.transform.position.SetY(0.5f), 3, 1,1.5f).SetEase(Ease.OutBounce));
        objCoin.transform.DOJump(objCoin.transform.position.SetY(0.5f), 1, 1, 1.5f).SetEase(Ease.OutBounce)
            .OnComplete(() => { objCoin.transform.DORotate(new Vector3(0.0f, 360.0f, 0.0f), 1.5f, RotateMode.FastBeyond360).SetEase(Ease.Linear).SetLoops(-1); });

        Add(amount);
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
}
