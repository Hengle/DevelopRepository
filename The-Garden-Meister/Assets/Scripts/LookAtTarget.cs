using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtTarget : MonoBehaviour
{
    [SerializeField] Transform target;

    private void Update()
    {
        var diff = target.position - transform.position;
        var axis = Vector3.Cross(Vector3.up, diff);
        var angle = Vector3.Angle(Vector3.up, diff);
        if (axis.z < 0f) angle = 180.0f + (180.0f - angle);
        transform.eulerAngles = new Vector3(0f, 0f, angle - 35.0f);

        //Quaternion lookRotation = Quaternion.LookRotation(target.transform.position - transform.position, Vector3.up);
        //lookRotation.z = 0;
        //transform.rotation = Quaternion.Lerp(transform.rotation, lookRotation, 0.1f);
    }
}
