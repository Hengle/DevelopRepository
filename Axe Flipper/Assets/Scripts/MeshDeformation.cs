using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshDeformation : MonoBehaviour
{
    public float rotate = 3.0f;
    public float strength = 0.1f;

    MeshFilter meshFilter;
    MeshCollider meshCollider;

    void Start()
    {
        meshFilter = GetComponent<MeshFilter>();
        meshCollider = GetComponent<MeshCollider>();
    }

    void Update()
    {
        //transform.Rotate(0, 0, rotate);
        if (Input.GetMouseButton(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.SphereCast(ray, 0.1f, out hit, Mathf.Infinity))
            {
                if (hit.collider == meshCollider)
                {
                    //オブジェクトの頂点情報をコピー
                    Vector3[] vertices = new Vector3[meshCollider.sharedMesh.vertexCount];
                    meshCollider.sharedMesh.vertices.CopyTo(vertices, 0);

                    //Rayがヒットして三角形を取得
                    //三角形一つに対して3ずつ増えるので3かける
                    int[] triangls = meshCollider.sharedMesh.triangles;
                    int id_1 = triangls[hit.triangleIndex * 3 + 0];
                    int id_2 = triangls[hit.triangleIndex * 3 + 1];
                    int id_3 = triangls[hit.triangleIndex * 3 + 2];

                    //対象の頂点をカメラから見て前方に進める（凹む）
                    //ワールド空間からローカル空間
                    vertices[id_1] = vertices[id_1] + transform.InverseTransformPoint(Camera.main.transform.forward * strength);
                    vertices[id_2] = vertices[id_2] + transform.InverseTransformPoint(Camera.main.transform.forward * strength);
                    vertices[id_3] = vertices[id_3] + transform.InverseTransformPoint(Camera.main.transform.forward * strength);

                    //頂点情報の更新
                    meshFilter.mesh.vertices = vertices;
                    meshCollider.sharedMesh = meshFilter.mesh;

                    // 領域と法線を再計算する
                    meshFilter.mesh.RecalculateBounds();
                    meshFilter.mesh.RecalculateNormals();
                    meshCollider.sharedMesh.RecalculateBounds();
                    meshCollider.sharedMesh.RecalculateNormals();
                }
            }
        }
    }
}
