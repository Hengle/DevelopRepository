using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Bullet : MonoBehaviour
{
    [SerializeField] AnimationCurve animationCurve;
    [SerializeField] ParticleSystem particleSystem;
    [SerializeField] float speed = 3.0f;
    MeshRenderer myMeshRenderer;
    
    private void Awake()
    {
        myMeshRenderer = GetComponent<MeshRenderer>();
    }

    public void Fire()
    {
        //myMeshRenderer.enabled = true;
        //particleSystem.Play();
        transform.parent = null;
        transform.DORotate(new Vector3(0,-1080.0f,0),3.0f);
        transform.DOMove(new Vector3(0.0f, 2.4f, -3.5f), speed).SetEase(animationCurve);
    }
}
