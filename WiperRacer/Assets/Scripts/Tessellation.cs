using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tessellation : MonoBehaviour
{

    public float kaiten = 1.0f;
    public float Strength = 0.001f;

    MeshFilter meshFilter;
    MeshCollider meshCollider;

    void Start()
    {
        meshFilter = GetComponent<MeshFilter>();
        meshCollider = GetComponent<MeshCollider>();
    }

    void Update()
    {
        transform.Rotate(0, 0, kaiten);
        
    }

}
