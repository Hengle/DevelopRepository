using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class DropCoin : MonoBehaviour
{
    [SerializeField] GameObject obj;
    private int spanAngle = 15;
    public List<int> shootAngle = new List<int>();

    // Start is called before the first frame update
    void Start()
    {

        for (int i =0; i<=360; i += spanAngle) {
            shootAngle.Add(i);
        }

        foreach(int num in shootAngle) {
            var newObj = Instantiate(obj);
            newObj.transform.position = transform.position;
            newObj.GetComponent<Rigidbody2D>().AddForce(GetDirection(num)*1, ForceMode2D.Impulse);
            newObj.GetComponent<CoinObj>().PlayAnimation();
        }

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // 指定された角度（ 0 ～ 360 ）をベクトルに変換して返す
    public static Vector3 GetDirection(float angle) {
        return new Vector3
        (
            Mathf.Cos(angle * Mathf.Deg2Rad),
            Mathf.Sin(angle * Mathf.Deg2Rad),
            0
        );
    }
}
