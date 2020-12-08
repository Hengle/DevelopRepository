using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Gorilla : MonoBehaviour
{
    [SerializeField] Transform characterParent;
    [SerializeField] Transform throwDownTarget;
    Character grabCharacter;

    Sequence sequence;
    Animator myAnimator;

    private void Awake()
    {
        myAnimator = GetComponent<Animator>();
        var pos = throwDownTarget.position;
        pos.y = transform.position.y;

        sequence = DOTween.Sequence();
        sequence.Append(transform.DOLookAt(pos, 1.0f).OnStart(() => { myAnimator.SetBool("isWalking", true); }).OnComplete(() => {
            myAnimator.SetBool("isWalking", false);
            myAnimator.Play("Jump");
        }));
        sequence.Append(transform.DOMoveY(transform.position.y + 3.5f, 0.5f).SetEase(Ease.OutBack).SetDelay(0.3f));
        sequence.Append(transform.DOMoveY(transform.position.y, 0.5f).SetEase(Ease.OutBack).SetDelay(0.2f));
        sequence.Pause();
    }

    void Attacking()
    {
        grabCharacter.transform.parent = null;
        var vector = throwDownTarget.position - grabCharacter.GetCharacterAsix().position;
        vector = vector.normalized;
        grabCharacter.Shot(vector * 50f);
    }

    public void ActiveAction(Character character)
    {
        grabCharacter = character;
        grabCharacter.transform.parent = characterParent;
        grabCharacter.GetCharacterAsix().position = characterParent.position;
        sequence.Play();
    }

    public void StandBy()
    {


    }
}
