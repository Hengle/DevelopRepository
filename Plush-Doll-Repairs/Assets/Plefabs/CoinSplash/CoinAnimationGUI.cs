using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class CoinAnimationGUI : MonoBehaviour {

    [SerializeField, HeaderAttribute("コインの最終的なゴール地点")] GameObject targetToEnd;
    [SerializeField, HeaderAttribute("コインのPrefab（uGUI用）")] GameObject coinPrefab;
    [SerializeField, HeaderAttribute("コインの生成枚数")] int coinVal;
    [SerializeField, HeaderAttribute("コインの散らばりランダム具合")] float coinSpread = 500.0f;
    [SerializeField, HeaderAttribute("散らばってからゴールにいくまでの間隔")] float coinStartDelay = 0.5f;


    List<GameObject> coinObjs = new List<GameObject>();

    void Start(){

        for (int i = 0; i < coinVal; i++) {
            var newObj = Instantiate(coinPrefab);
            newObj.SetActive(false);
            newObj.transform.SetParent(gameObject.transform, false);
            coinObjs.Add(newObj);
        }
    }


    public void Play() {
        foreach(GameObject obj in coinObjs) {
            obj.SetActive(true);
            var randRad = Random.Range(0, 360.0f);
            obj.transform.Rotate(new Vector3(0, 0, randRad));

            var endMoveTime = Random.Range(0.3f, 0.7f);

            var anime = DOTween.Sequence();
            anime.Append(obj.transform.DOLocalMove(new Vector2(Random.Range(-coinSpread, coinSpread), Random.Range(-coinSpread, coinSpread)), 1f))
                .AppendInterval(coinStartDelay)
                .Append(obj.transform.DOMove(targetToEnd.transform.position, endMoveTime))
                .Join(obj.transform.DOScale(0.5f, endMoveTime))
                .OnComplete(()=> {
                    obj.SetActive(false);
                    obj.transform.DOScale(1, 0);
                    obj.transform.position = gameObject.transform.position;
                    DataManager.Instance.ChangeCoin(DataManager.Instance.Coin);
                });

        }
    }
}
