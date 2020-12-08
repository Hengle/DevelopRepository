using System;
using UnityEngine;

public class GenerateLine : MonoBehaviour
{

    [SerializeField] LineRenderer line;
    [SerializeField] GameObject target;
    bool Active { get; set; }
    private int count;

    public void SetActive(bool active)
    {
        Active = active;

        if (!active)
        {
            count = 0;
            line.positionCount = count;
        }
    }

    private void FixedUpdate()
    {
        if (!Active) return;
        count += 1;
        line.positionCount = count;
        line.SetPosition(count - 1, target.transform.position);
    }
}
