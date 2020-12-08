using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;

public class Coin : MonoBehaviour {

    public event Action<Vector3> GetCoin;

    private void Start() {

        gameObject.transform.DORotate(new Vector3(0, -360, 0), 3f).SetRelative().SetLoops(-1).SetEase(Ease.Linear);

    }

    private void OnTriggerEnter(Collider other) {
        if(other.tag == "Player") {
            GetCoin?.Invoke(gameObject.transform.position);
            Destroy(gameObject);
        }  
    }

}
