using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(GeneratePlaneMesh))]
public class Test : MonoBehaviour
{
    public float maximumDepression;
    public List<Vector3> originalVertices;
    public List<Vector3> modifiedVertices;

    private GeneratePlaneMesh plane;

    private void Start()
    {
        MeshRegenerated();
    }
    public void MeshRegenerated()
    {
        plane = GetComponent<GeneratePlaneMesh>();
        plane.mesh.MarkDynamic();
        originalVertices = new List<Vector3>(plane.mesh.vertices);
        modifiedVertices = new List<Vector3>(plane.mesh.vertices);
        plane.mesh.SetVertices(modifiedVertices);
        Debug.Log("Mesh Regenerated");
    }

    public void AddDepression(Vector3 depressionPoint, float radius)
    {
        var worldPos4 = transform.worldToLocalMatrix * depressionPoint;
        var worldPos = new Vector3(worldPos4.x, worldPos4.y, worldPos4.z);
        for (int i = 0; i < modifiedVertices.Count; ++i)
        {
            var distance = (worldPos - (modifiedVertices[i] + Vector3.down * maximumDepression)).magnitude;

            if (distance < radius)
            {
                var newVert = originalVertices[i] + Vector3.down * maximumDepression * 2;
                modifiedVertices.RemoveAt(i);
                modifiedVertices.Insert(i, newVert);
            }
        }

        plane.mesh.SetVertices(modifiedVertices);
        Debug.Log("Mesh Depressed");
    }
}
