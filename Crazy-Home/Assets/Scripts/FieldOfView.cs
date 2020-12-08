using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldOfView : MonoBehaviour
{
    [SerializeField] LayerMask layerMask;
    Mesh mesh;
    Vector3 origin;
    [SerializeField] int raycount = 50;
    [SerializeField] float baseAngle = 0f;
    [SerializeField] Transform target;
    [SerializeField] Transform eye;
    private void Awake()
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
        origin = Vector3.zero;
    }

    private void Update()
    {

        float fov = 90f;
        float angle = baseAngle;
        float angleIncrease = fov / raycount;
        float viewDistance = 5f;

        Vector3[] vertices = new Vector3[raycount + 1 + 1];
        Vector2[] uv = new Vector2[vertices.Length];
        int[] triangles = new int[raycount * 3];

        SetOrigin(target.transform.position);
        angle += GetAngleFromVectorFloat((eye.transform.forward).normalized) - fov / 2f;

        vertices[0] = origin;

        int vertexIndex = 1;
        int triangleIndex = 0;
        for (int i = 0; i <= raycount; i++)
        {
            Vector3 vertex;
            RaycastHit hit;

            if (Physics.Raycast(origin, GetVectorFromAngle(angle), out hit, viewDistance))
            {
                vertex = hit.point;
            }
            else
            {
                vertex = origin + GetVectorFromAngle(angle) * viewDistance;
            }
            
            vertices[vertexIndex] = vertex;

            if (i > 0)
            {
                triangles[triangleIndex + 0] = 0;
                triangles[triangleIndex + 1] = vertexIndex - 1;
                triangles[triangleIndex + 2] = vertexIndex;

                triangleIndex += 3;
            }
            vertexIndex++;
            angle -= angleIncrease;
        }

        mesh.vertices = vertices;
        mesh.uv = uv;
        mesh.triangles = triangles;
    }

    Vector3 GetVectorFromAngle(float angle)
    {
        float angleRad = angle * (Mathf.PI / 180f);
        return new Vector3(Mathf.Cos(angleRad), 0f, Mathf.Sin(angleRad));
    }

    float GetAngleFromVectorFloat(Vector3 dir)
    {
        dir = dir.normalized;
        float n = Mathf.Atan2(dir.z, dir.x) * Mathf.Rad2Deg;
        if (n < 0) n += 360;

        return n;
    }


    void SetOrigin(Vector3 origin)
    {
        this.origin = origin;
    }
}
