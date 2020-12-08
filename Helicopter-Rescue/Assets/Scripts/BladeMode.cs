using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EzySlice;

public class BladeMode : MonoBehaviour
{
    [SerializeField] Transform target;
    [SerializeField] Material material;

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            SlicedHull hull = SliceObject(target.gameObject, material);
            if (hull != null)
            {
                GameObject bottom = hull.CreateLowerHull(target.gameObject);
                GameObject top = hull.CreateUpperHull(target.gameObject);
                Destroy(target.gameObject);
            }        
        }
    }

    public SlicedHull SliceObject(GameObject obj, Material crossSectionMaterial = null)
    {
        // slice the provided object using the transforms of this object
        if (obj.GetComponent<MeshFilter>() == null)
            return null;

        return obj.Slice(target.position, target.transform.up, crossSectionMaterial);
    }
}
