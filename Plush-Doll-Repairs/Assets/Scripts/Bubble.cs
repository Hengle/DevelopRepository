using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Bubble : MonoBehaviour
{
    Collider myCollider;

    private void Awake()
    {
        myCollider = GetComponent<Collider>();
        transform.rotation = Random.rotation;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Water")
        {
            myCollider.enabled = false;
            transform.DOScale(0f, 0.3f).OnComplete(() => Destroy(gameObject));
        }
    }
}
