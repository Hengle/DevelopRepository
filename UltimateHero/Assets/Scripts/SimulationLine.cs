using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimulationLine : MonoBehaviour
{
    [SerializeField] LineRenderer line;

    public void DrawLine(Vector3[] positions)
    {
        line.positionCount = positions.Length;
        line.SetPositions(positions);
    }

    public void SetActive(bool active)
    {
        line.enabled = active;
    }

    public bool GetActive()
    {
        return line.enabled;
    }
}
