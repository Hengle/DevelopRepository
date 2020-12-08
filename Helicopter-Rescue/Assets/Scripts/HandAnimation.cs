using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandAnimation : MonoBehaviour {

    Animator myAnimator;
    HandGrabbing handGrab;

	void Start ()
    {
        myAnimator = GetComponentInChildren<Animator>();
        handGrab = GetComponent<HandGrabbing>();
    }

    void Update ()
    {
        if(Input.GetMouseButtonUp(0))
        {
            if (!myAnimator.GetBool("IsGrabbing"))
            {
                myAnimator.SetBool("IsGrabbing", true);
            }
        }
	}
}
