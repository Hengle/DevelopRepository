using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class People : MonoBehaviour
{
    [SerializeField] GameObject outerField;

    Animator animator;
    Collider collider;
    Rigidbody rigidbody;

    Quaternion quaternion;
    Tweener tweener;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        collider = GetComponent<Collider>();
        rigidbody = GetComponent<Rigidbody>();
        quaternion = transform.rotation;

        tweener = transform.DOMoveY(transform.position.y + 0.25f, 1.0f).SetLoops(-1,LoopType.Yoyo);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Rope")
        {
            animator.Play("RopeGrap");
            outerField.SetActive(false);
            tweener.Kill();
        }

        if (other.gameObject.tag == "Goal")
        {
            animator.Play("Wave");
            other.gameObject.transform.GetComponent<Collider>().enabled = false;
            transform.parent = null;
            transform.rotation = quaternion;
            collider.isTrigger = false;
            rigidbody.isKinematic = false;
            rigidbody.useGravity = true;
            rigidbody.velocity = Vector3.zero;
        }
    }
}
