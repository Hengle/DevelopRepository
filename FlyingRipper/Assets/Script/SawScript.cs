using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class SawScript : MonoBehaviour {

    [SerializeField] float rotateAngle;
   // [SerializeField] float speed;

    //void Start(){
    //    gameObject.transform.DORotate(new Vector3(0, rotateAngle, 0),speed).SetRelative().SetEase(Ease.Linear).SetLoops(-1);
    //}

    private void Update() {
        gameObject.transform.Rotate(0, rotateAngle, 0);  
    }


}
