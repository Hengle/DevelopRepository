using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Unity.Mathematics.math;

public class Line : MonoBehaviour
{
    float angle = -45f;
    float top = 5;
    float bottom = 0;

    private void Start()
    {
        var line = GetComponent<LineRenderer>();
        line.positionCount = 6;
        for (int t = 0; t <= 5; t++)
        {
            var angleRadians = radians(angle);
            var scale = 1f / (angleRadians * (1f / (top - bottom)));
            var rotation = (clamp(t, bottom, top) - bottom) / (top - bottom) * angleRadians;

            var c = cos((float)PI - rotation);
            var s = sin((float)PI - rotation);

            var pos = new Vector3((scale * s) - (t * s) + 24.5f,(scale * c) + scale - (t * c) + 5f);

            line.SetPosition(t, pos);
            Debug.Log(c);
            Debug.Log(s);
        }

    }
    
}
