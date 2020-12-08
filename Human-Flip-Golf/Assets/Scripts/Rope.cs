using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rope : MonoBehaviour
{
    [SerializeField] int length = 5;
    [SerializeField] GameObject childPrefab;
    [SerializeField] float childDistance = 0.5f;
    [SerializeField] Material ropeMaterial;
    [SerializeField] GameObject firstConect, lastConect;

    bool isCut, isGrap;
    int cutIndex = 100;
    RopeRenderer ropeRenderer;
    RopeRenderer cutRopeRenderer;
    Vector3[] positions;
    Vector3[] cutPositions;

    public event Action OnCutListener;

    private void Awake()
    {
        ropeRenderer = GetComponent<RopeRenderer>();
        Spawn();
    }

    private void Start()
    {
        Refresh();
    }

    void Spawn()
    {
        int count = (int)(length / childDistance);
        for (int x = 0; x <= count; x++)
        {
            GameObject tmp;
            tmp = Instantiate(childPrefab, transform);
            tmp.transform.localPosition = Vector3.zero;
            tmp.transform.localRotation = Quaternion.identity;
            tmp.transform.localPosition = tmp.transform.localPosition.SetY(-childDistance * x);
            tmp.name = transform.childCount.ToString();

            if (x == 0 && firstConect != null)
            {
                tmp.GetComponent<HingeJoint>().connectedBody = firstConect.transform.GetComponent<Rigidbody>();
            }else if(x == count && lastConect != null)
            {
                lastConect.transform.localPosition = lastConect.transform.localPosition.SetZ(childDistance * x);

                tmp.GetComponent<HingeJoint>().connectedBody = transform.Find((transform.childCount - 1).ToString()).GetComponent<Rigidbody>();
                lastConect.transform.GetComponent<HingeJoint>().connectedBody = tmp.GetComponent<Rigidbody>();
            }
            else
            {
                tmp.GetComponent<HingeJoint>().connectedBody = transform.Find((transform.childCount - 1).ToString()).GetComponent<Rigidbody>();
            }
        }
    }

    public void Telescopic(int newLength)
    {
        if (length == newLength) return;
        int beforeCount = (int)(length / childDistance);
        length = newLength;
        int count = (int)(length / childDistance);

        count = count - beforeCount;
        var basePosY = -childDistance * (transform.childCount - 1);

        if (count > 0)
        {
            //増やす
            for (int x = 0; x <= count; x++)
            {
                GameObject tmp;
                tmp = Instantiate(childPrefab, transform);
                tmp.transform.localPosition = Vector3.zero;
                tmp.transform.localRotation = Quaternion.identity;
                tmp.transform.localPosition = tmp.transform.localPosition.SetY(-childDistance * x + basePosY);
                tmp.name = transform.childCount.ToString();

                if (x == count && lastConect != null)
                {
                    lastConect.transform.localPosition = lastConect.transform.localPosition.SetZ(childDistance * x - basePosY);

                    tmp.GetComponent<HingeJoint>().connectedBody = transform.Find((transform.childCount - 1).ToString()).GetComponent<Rigidbody>();
                    lastConect.transform.GetComponent<HingeJoint>().connectedBody = tmp.GetComponent<Rigidbody>();
                }
                else
                {
                    tmp.GetComponent<HingeJoint>().connectedBody = transform.Find((transform.childCount - 1).ToString()).GetComponent<Rigidbody>();
                }
            }
        }
        else
        {

        }

        Refresh();
    }

    public void Refresh()
    {
        if (isCut)
        {
            cutPositions = new Vector3[transform.childCount - cutIndex];
            Array.Resize(ref positions, cutIndex);
        }
        else
        {
            positions = new Vector3[transform.childCount];
        }
    }

    public void ResistEvent()
    {
        foreach (var c in transform.GetComponentsInChildren<RopeCollider>())
        {
            c.OnTriggerListener += Grap;
        }
    }

    private void Update()
    {
        if (isCut)
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                if (i >= cutIndex)
                {
                    cutPositions[i - cutIndex] = transform.GetChild(i).position;
                }
                else
                {
                    positions[i] = transform.GetChild(i).position;
                }
            }
            cutRopeRenderer.SetPositions(cutPositions);
            ropeRenderer.SetPositions(positions);
        }
        else
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                positions[i] = transform.GetChild(i).position;
            }
            ropeRenderer.SetPositions(positions);
        }
    }

    void Grap(Transform transform, Collider other)
    {
        if (!isGrap)
        {
            isGrap = true;
            other.gameObject.transform.parent = transform;
            other.gameObject.transform.localPosition = Vector3.zero + new Vector3(0f,-6f,0f);
            other.gameObject.transform.localEulerAngles = Vector3.zero + new Vector3(0f, 90f, 0f);

            foreach (var c in transform.GetComponentsInChildren<RopeCollider>())
            {
                c.OnTriggerListener -= Grap;
                c.OnCollisionListener += Cut;
            }
        }
    }

    void Cut(Transform transform, Collision other)
    {
        //二重発火防止
        if (!isCut)
        {
            var hingeJoint = transform.GetComponent<HingeJoint>();
            isCut = true;
            Destroy(hingeJoint);

            var rope = Instantiate(new GameObject(), this.transform.parent);
            cutRopeRenderer = rope.AddComponent<RopeRenderer>();
            cutRopeRenderer.material = ropeMaterial;

            cutIndex = hingeJoint.transform.GetSiblingIndex();

            PhysicsOn();

            Refresh();

            OnCutListener?.Invoke();
        }
    }

    public void PhysicsOn()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            var r = transform.GetChild(i).GetComponent<Rigidbody>();

            r.isKinematic = false;
            r.useGravity = true;
            r.constraints = RigidbodyConstraints.None;
        }
    }
}
