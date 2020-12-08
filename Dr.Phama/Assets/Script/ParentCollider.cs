using System;
using System.Collections.Generic;
using UnityEngine;

public class ParentCollider : MonoBehaviour
{
    int CheckCount { get; set; }
    [SerializeField] ChildCollider childCollider;
    List<ChildCollider> boxs = new List<ChildCollider>();
    public event Action OnSurroundedListener;

    void Start()
    {
        //foreach (Transform child in transform)
        //{
        //    var s = child.GetComponent<ChildCollider>();
        //    s.OnCollisionEnterListener += UpdateCount;
        //    CheckCount++;
        //}

        childCollider.OnCollisionEnterListener += UpdateCount;
        CheckCount++;
    }

    void UpdateCount(ChildCollider s)
    {
        if (boxs.Contains(s)) return;
        boxs.Add(s);
        if (boxs.Count == CheckCount)
        {
            OnSurroundedListener?.Invoke();
        }
    }

    public void Refresh()
    {
        boxs.Clear();
    }
}
