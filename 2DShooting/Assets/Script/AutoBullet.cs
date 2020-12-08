using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoBullet : MonoBehaviour
{
    //弾オブジェクト
    [SerializeField] GameObject bullet;

    //連射速度
    [SerializeField] float shotDelay = 1;

    private IEnumerator Start()
    {
        while (true)
        {
            Instantiate(bullet, transform.position, transform.rotation);

            // ショット音を鳴らす
            //GetComponent<AudioSource>().Play();

            // shotDelay秒待つ
            yield return new WaitForSeconds(shotDelay);
        }
    }
}
