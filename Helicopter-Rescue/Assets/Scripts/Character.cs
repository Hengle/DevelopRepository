using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Util;
using TMPro;
using DG.Tweening;

public class Character : MonoBehaviour
{
    public enum CharacterType
    {
        Plyer,
        Enemy,
    }

    //protected abstract void Initialize();

    Rigidbody MyRigidbody { get; set; }
    Collider MyCollider { get; set; }
    Animator MyAnimator { get; set; }
    RagdollUtility MyRagdollUtility { get; set; }

    Vector3 startPos;
    Quaternion quaternion;
    bool isDeath;
    public event Action<bool> OnGrabListener;
    public event Action OnDeathListener;
    public event Action OnGoalListener;

    [SerializeField] GameObject outerField;

    void Awake()
    {
        MyRigidbody = GetComponent<Rigidbody>();
        MyCollider = GetComponent<Collider>();
        MyAnimator = GetComponent<Animator>();
        MyRagdollUtility = GetComponent<RagdollUtility>();
        //MyCollider.isTrigger = true;
        //MyRigidbody.isKinematic = true;
        //MyRigidbody.useGravity = false;
    }

    void Start()
    {
        //MyRagdollUtility.Setup();
        //MyRagdollUtility.Deactivate();
        //tweener = transform.DOMoveY(transform.position.y + 0.25f, 1.0f).SetLoops(-1,LoopType.Yoyo);
        MyCollider.enabled = false;
        MyAnimator.enabled = false;
        quaternion = transform.rotation;
    }

    public void StartAnimation()
    {
        MyAnimator.enabled = true;
    }

    public void Rescue()
    {
        MyCollider.enabled = true;
    }

    private void Update()
    {
        if (isDeath) return;
    }

    public bool isPairGround()
    {
        RaycastHit hit;

        //if (Physics.Raycast(sawObjcts[pairIndex].transform.position, Vector3.down, out hit, 3.0f, layerMask))
        //{

        //}
        //else
        //{
        //    return false;
        //}

        return true;
    }

    protected IEnumerator WaitAnimation(string stateName, Action complete = null)
    {
        yield return new WaitForSeconds(0.1f);
        yield return new WaitWhile(() => MyAnimator.GetCurrentAnimatorStateInfo(0).fullPathHash == Animator.StringToHash(stateName));
        complete?.Invoke();
    }

    public void PlayAnimation(String animationName, bool isTrigger)
    {
        if (isTrigger)
        {
            MyAnimator.SetTrigger(animationName);
        }
        else
        {
            MyAnimator.Play(animationName);
        }
    }

    protected void DeathRagdoll(Vector3 direction)
    {
        MyAnimator.enabled = false;
        MyCollider.enabled = false;
        MyRagdollUtility.Activate();
        MyRagdollUtility.AddForce(direction, direction * 2);

        OnDeathListener?.Invoke();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Rope")
        {
            MyAnimator.Play("Jump");
            outerField.SetActive(false);
            OnGrabListener?.Invoke(true);
        }

        if (other.gameObject.tag == "Goal")
        {
            MyAnimator.Play("Wave");
            other.gameObject.transform.GetComponent<Collider>().enabled = false;
            transform.parent = null;
            transform.rotation = quaternion;
            MyCollider.isTrigger = false;
            MyRigidbody.isKinematic = false;
            MyRigidbody.useGravity = true;
            MyRigidbody.velocity = Vector3.zero;
            MyRigidbody.angularVelocity = Vector3.zero;
            OnGoalListener?.Invoke();
        }
    }
}
