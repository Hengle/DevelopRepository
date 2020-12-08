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
    int axisIndex = 0, pairIndex = 1;
    bool isDeath, isReverse;
    Vector3 objectAxis;
    public event Action OnDeathListener;

    [SerializeField] Rope rope;
    [SerializeField] GameObject[] sawObjcts;
    [SerializeField] float speed;
    [SerializeField] ParticleSystem collisionEffect;
    [SerializeField] float fallForce;
    [SerializeField] LayerMask layerMask;

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

        objectAxis = sawObjcts[axisIndex].GetComponent<Transform>().position;

        rope.ResistEvent();
        rope.OnCutListener += () => {
            isDeath = true;

            foreach (var sawObj in sawObjcts)
            {
                sawObj.transform.GetComponent<Rigidbody>().isKinematic = false;
                sawObj.transform.GetComponent<Rigidbody>().useGravity = true;
                sawObj.transform.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
                sawObj.transform.GetComponentInChildren<Collider>().isTrigger = false;
            }
        };

        foreach (var sawObj in sawObjcts)
        {
            sawObj.transform.GetComponent<CharacterCollider>().OnTriggerListener += TriggerEnter;
        }
    }

    private void Update()
    {
        if (isDeath) return;
        transform.RotateAround(objectAxis, Vector3.up * (isReverse ? -1f : 1f), 360 / 2 * Time.deltaTime * speed);
    }

    public void AxisSwitching()
    {
        isReverse = !isReverse;
        objectAxis = sawObjcts[pairIndex].transform.position;
        sawObjcts = sawObjcts.Reverse().ToArray();
        AxisGroundCheck();
    }

    public GameObject GetAxisObject()
    {
        return sawObjcts[axisIndex];
    }

    public GameObject GetPairObject()
    {
        return sawObjcts[pairIndex];
    }

    void AxisGroundCheck()
    {
        RaycastHit hit;

        if (Physics.Raycast(sawObjcts[axisIndex].transform.position, Vector3.down, out hit, 3.0f, layerMask))
        {

        }
        else
        {
            isDeath = true;

            foreach (var sawObj in sawObjcts)
            {
                sawObj.transform.GetComponent<Rigidbody>().isKinematic = false;
                sawObj.transform.GetComponent<Rigidbody>().useGravity = true;
                sawObj.transform.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
                sawObj.transform.GetComponent<Rigidbody>().AddForce(-transform.up * fallForce, ForceMode.Impulse);
                sawObj.transform.GetComponentInChildren<Collider>().isTrigger = false;
            }
            rope.PhysicsOn();
        }
    }

    public bool isPairGround()
    {
        RaycastHit hit;

        if (Physics.Raycast(sawObjcts[pairIndex].transform.position, Vector3.down, out hit, 3.0f, layerMask))
        {

        }
        else
        {
            return false;
        }

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

    private void TriggerEnter(Collider other, Vector3 pos)
    {
        if (other.gameObject.tag == "Saw")
        {
            Instantiate(collisionEffect, other.ClosestPointOnBounds(pos), Quaternion.LookRotation((other.ClosestPointOnBounds(pos) - other.transform.position).normalized));
        }

        if (other.gameObject.tag == "Saw" && pos != objectAxis)
        {
            isReverse = !isReverse;
        }
    }

    public bool IsDeath()
    {
        return isDeath;
    }

    public bool IsReverse()
    {
        return isReverse;
    }
}
