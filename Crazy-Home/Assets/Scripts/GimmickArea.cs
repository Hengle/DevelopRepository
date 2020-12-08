using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GimmickArea : MonoBehaviour
{
    Collider collider;
    Renderer zoneRenderer;

    //public event Action OnEnterTrigger;

    void Awake()
    {
        collider = GetComponent<Collider>();
        zoneRenderer = GetComponent<Renderer>();
    }

    public void Show()
    {
        collider.enabled = true;
        zoneRenderer.enabled = true;
    }

    public void Hide()
    {
        collider.enabled = false;
        zoneRenderer.enabled = false;
    }

    //private void OnTriggerEnter(Collider other)
    //{
    //    if (other.gameObject.tag == "Player")
    //    {
    //        OnEnterTrigger?.Invoke();
    //    }
    //}
}
