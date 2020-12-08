using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowLauncher : MonoBehaviour
{

    [SerializeField] GameObject arrowPref;
    [SerializeField] type shootDirection;
    [SerializeField] float shootPower = 10;
    [SerializeField] float arrowRotation = 0;
    
    enum type {
        VERTICAL,
        HORIZONTAL
    }

    
    private void Update() {
 
    }

    public void Shoot() {

        Vector2 myShootPower = Vector2.one;

        switch (shootDirection) {
            case type.VERTICAL:
                myShootPower.x = 0;
                myShootPower.y = shootPower;
                break;
            case type.HORIZONTAL:
                myShootPower.x = shootPower;
                myShootPower.y = 0;
                break;
        }

        var arrowObj = Instantiate(arrowPref);
        //arrowObj.transform.SetParent(gameObject.transform, true);
        arrowObj.transform.position = gameObject.transform.position;
        arrowObj.transform.Rotate(0,0, arrowRotation);
        arrowObj.GetComponent<Rigidbody2D>().AddForce(myShootPower, ForceMode2D.Impulse);

    }

}
