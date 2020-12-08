using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EzySlice;

public class BladeMode : MonoBehaviour
{
    [SerializeField] Transform cutCube;
    [SerializeField] GameObject target;
    [SerializeField] Material material;
    [SerializeField] Animator animator;
    [SerializeField] GameObject bladeObject;
    [SerializeField] ParticleSystem particleSystem;

    Vector3 touchPos;
    

    void Awake()
    {
        animator.SetFloat("x", -1.0f);
        animator.SetFloat("y", 0.35f);
    }

    private void Update()
    {
        //animator.SetFloat("x", Mathf.Clamp(Input.mousePosition.x/720.0f, -1, 1));
        //animator.SetFloat("y", Mathf.Clamp(Input.mousePosition.y/1280.0f, -1, 1));

        if (Input.GetMouseButtonDown(0))
        {
            touchPos = Input.mousePosition;
            
        }

        if (Input.GetMouseButton(0))
        {
            var dir = ((Input.mousePosition - touchPos) / Screen.dpi).normalized;
            //武器の位置更新を一時的に固定
            //animator.SetFloat("x", Mathf.Clamp(dir.x, -1, 1));
            //animator.SetFloat("y", Mathf.Clamp(dir.y, -1, 1));
            //cutCube.transform.forward = dir;
        }

        if (Input.GetMouseButtonUp(0))
        {
            //武器の位置更新を一時的に固定
            //animator.SetFloat("x", animator.GetFloat("x") * -1.0f);
            //animator.SetFloat("y", animator.GetFloat("y") * -1.0f);

            //animator.SetFloat("x", 0f);
            //animator.SetFloat("y", -1.0f);

            //SlicedHull hull = SliceObject(target, material);
            //if (hull != null)
            //{
            //    GameObject bottom = hull.CreateLowerHull(target);
            //    GameObject top = hull.CreateUpperHull(target);
            //    top.AddComponent<SliceObject>().SetRotate(new Vector3(0, -30, 0));
            //    bottom.AddComponent<SliceObject>().SetRotate(new Vector3(0, 30, 0));
            //    Destroy(target);
            //}
        }
    }

    public void Slice()
    {
        animator.SetFloat("x", 1.0f);
        animator.SetFloat("y", -0.35f);

        //SlicedHull hull = SliceObject(target, material);
        //if (hull != null)
        //{
        //    GameObject bottom = hull.CreateLowerHull(target);
        //    GameObject top = hull.CreateUpperHull(target);
        //    top.AddComponent<SliceObject>().SetRotate(new Vector3(0, -30, 0));
        //    bottom.AddComponent<SliceObject>().SetRotate(new Vector3(0, 30, 0));
        //    Destroy(target);
        //}
    }

    public SlicedHull SliceObject(GameObject obj, Material crossSectionMaterial = null)
    {
        // slice the provided object using the transforms of this object
        if (obj.GetComponent<MeshFilter>() == null)
            return null;

        return obj.Slice(cutCube.position, cutCube.transform.up, crossSectionMaterial);
    }
}
