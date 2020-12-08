using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CoinUI : MonoBehaviour
{
    TextMeshProUGUI myText;
    int myCoin;

    // Start is called before the first frame update
    void Start()
    {
        myText = GetComponent<TextMeshProUGUI>();
        myCoin = DataManager.GetPlayerCoin();
        myText.text = myCoin.ToString();
    }

    // Update is called once per frame
    void Update()
    {
        var nowCoin = DataManager.GetPlayerCoin();
        if(myCoin != nowCoin) {
            StartCoroutine(CoinAnimation((float)myCoin, (float)nowCoin,1f));
            myCoin = nowCoin;
        }
    }

    // スコアをアニメーションさせる
    private IEnumerator CoinAnimation(float startScore, float endScore, float duration) {
        // 開始時間
        float startTime = Time.time;

        // 終了時間
        float endTime = startTime + duration;

        do {
            // 現在の時間の割合
            float timeRate = (Time.time - startTime) / duration;

            // 数値を更新
            float updateValue = (float)((endScore - startScore) * timeRate + startScore);

            // テキストの更新
            myText.text = updateValue.ToString("f0"); // （"f0" の "0" は、小数点以下の桁数指定）

            // 1フレーム待つ
            yield return null;

        } while (Time.time < endTime);

        // 最終的な着地のスコア
        myText.text = endScore.ToString();
    }
}
