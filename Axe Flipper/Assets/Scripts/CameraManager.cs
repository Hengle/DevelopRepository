using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    [SerializeField] GameObject target;
    Vector3 distance;

    private void Awake()
    {
        SwitchFollowTarget(target);
    }

    public void SwitchFollowTarget(GameObject target)
    {
        this.target = target;
        distance = transform.position - this.target.transform.position;
    }

    void Update()
    {
        transform.position = target.transform.position + distance;
    }
}