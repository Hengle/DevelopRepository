using System;
using System.Collections.Generic;
using UnityEngine;

public class ParentColiider : MonoBehaviour
{
    int CheckCount { get; set; }
    List<ChildCollider> Spheres = new List<ChildCollider>();
    public event Action OnSurroundedListener;

    void Start()
    {
        foreach (Transform child in transform)
        {
            var s = child.GetComponent<ChildCollider>();
            s.OnCollisionEnterListener += UpdateCount;
            CheckCount++;
        }
    }

    void UpdateCount (ChildCollider s)
    {
        if (Spheres.Contains(s)) return;
        Spheres.Add(s);
        if (Spheres.Count == CheckCount)
        {
            OnSurroundedListener?.Invoke();
        }
    }

    public void Refresh()
    {
        Spheres.Clear();
    }
}
