using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Util;

public enum DrugType
{
    Eve,
    Sedes,
    Bufferin,
    Haitamin,
    Loxonin
}

public class Drug : MonoBehaviour
{
    Rigidbody myRigidBody;
    Collider myCollider;

    bool isRelease = false;
    DrugType myDrugType;

    public event Action<DrugType> OnDrugFallListener;

    private void Awake()
    {
        myRigidBody = transform.GetComponent<Rigidbody>();
        myCollider = transform.GetComponent<Collider>();
    }

    public void AutoFall()
    {
        myRigidBody.useGravity = true;
        myCollider.isTrigger = false;

        OnDrugFallListener?.Invoke(myDrugType);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Finish")
        {
            isRelease = true;
            transform.SetParent(other.gameObject.transform);
            myRigidBody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY;
            myRigidBody.angularVelocity = new Vector3(0f, 0f, -10f);
        }
    }

    public bool GetRelease()
    {
        return isRelease;
    }

    public void SetDrugType(DrugType drugType)
    {
        myDrugType = drugType;
    }
}
