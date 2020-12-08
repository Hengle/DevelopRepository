using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlowerStageGenerate : MonoBehaviour
{
    [SerializeField] GameObject leaf;
    [SerializeField] Transform parent;

    private void Awake()
    {
        for (int i =0; i <= 300; i++)
        {
            var scale = Random.Range(0.25f, 0.4f);
            var position = new Vector3(Random.Range(-5.0f,5.0f), 0.2f, Random.Range(0f, 20.0f));
            Instantiate(leaf,position, Quaternion.Euler(new Vector3(0f, Random.Range(0f, 360.0f), 0f)), parent).transform.localScale = new Vector3(scale, scale, scale); ;
        }

        //for (float y = 0.0f; y <= 20.0f; y += 0.5f)
        //{
        //    for (float x = -5.0f; x <= 5.0f; x += 0.5f)
        //    {
        //        var scale = Random.Range(0.15f, 0.25f);
        //        Instantiate(leaf, new Vector3(x, 0.2f, y), Quaternion.Euler(new Vector3(0f, Random.Range(0f, 360.0f), 0f)), parent).transform.localScale = new Vector3(scale, scale, scale);
        //    }
        //}
    }
}
