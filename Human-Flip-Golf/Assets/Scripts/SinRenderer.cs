using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SinRenderer : MonoBehaviour
{
    [SerializeField] float rate = 0.1f;
    [SerializeField] float width = 0.05f;
    LineRenderer lineRenderer;
    int count = 0;

    private void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
    }

    private void Update()
    {
        if(Input.GetMouseButton(0))
        {
            count++;
            lineRenderer.positionCount = count;
            float x = width * (count - 1);
            float y = Mathf.Sin(x * rate);
            lineRenderer.SetPosition(count - 1, new Vector3(-3f + x, y, width * (count * 0.1f)));
        }
    }
}
