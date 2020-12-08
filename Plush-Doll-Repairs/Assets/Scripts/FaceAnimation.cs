using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FaceAnimation : MonoBehaviour
{
    Animator myAnimator;
    bool isPuzzled;

    private void Awake()
    {
        isPuzzled = true;
        myAnimator = GetComponent<Animator>();
    }

    private IEnumerator Start()
    {
        while (isPuzzled)
        {
            myAnimator.SetInteger("Puzzled", Random.Range(1,5));

            yield return new WaitForSeconds(Random.Range(0.5f, 3.0f));
        }

        myAnimator.Play("Happy");
    }

    public void Clear()
    {
        isPuzzled = false;
    }
}
