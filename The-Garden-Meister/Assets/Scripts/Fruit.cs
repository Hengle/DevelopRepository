using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fruit : MonoBehaviour
{
    [SerializeField] GameObject center;
    [SerializeField] Deform.BendDeformer deformer;
    [SerializeField] float angleRate = 0.1f;
    [SerializeField] float dropRate = 50.0f;

    Rigidbody myRigidbody;
    Collider myCollider;
    Vector3 defaultPosition;
    float drop = 0;
    float time;
    bool harvested = false;

    public event Action OnDropListener;
    public event Action OnHarvestListener;

    private void Awake()
    {
        defaultPosition = transform.localPosition;
        myRigidbody = GetComponent<Rigidbody>();
        myCollider = GetComponent<Collider>();
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.parent)
        {
            var position = Quaternion.Euler(0f, 0f, deformer.Angle * -1.0f * angleRate) * defaultPosition;
            transform.position = center.transform.position + position;
        }
    }

    public void DropCheck(float value)
    {
        drop += value;
        if (drop >= dropRate)
        {
            transform.parent = null;
            myRigidbody.useGravity = true;
            myCollider.enabled = true;
            OnDropListener?.Invoke();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        time = 0;
    }

    private void OnTriggerStay(Collider other)
    {

        if (other.gameObject.tag == "Bowl")
        {
            time += Time.deltaTime;
            if (time >= 3.0f)
            {
                Harvest();
            }
        }
    }

    void Harvest()
    {
        if (harvested) return;
        harvested = true;
        myRigidbody.useGravity = false;
        myRigidbody.velocity = Vector3.zero;
        myRigidbody.angularVelocity = Vector3.zero;
        myRigidbody.constraints = RigidbodyConstraints.FreezeAll;
        OnHarvestListener?.Invoke();
    }

    public bool IsHarvested()
    {
        return harvested;
    }
}
