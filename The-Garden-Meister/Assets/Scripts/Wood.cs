using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Wood : MonoBehaviour
{
    [SerializeField] ParticleSystem particle;
    Deform.DeformerElement deformer;
    Deform.BendDeformer bendDeformer;
    float beforeAngle;
    float angleCount;
    float limitAngle = 15.0f;
    int dropCount;
    bool isAnimation;
    public event Action<float> OnShakeListener;

    private void Awake()
    {
        deformer = GetComponent<Deform.Deformable>().DeformerElements[0];
        bendDeformer = (Deform.BendDeformer)deformer.Component;
        beforeAngle = bendDeformer.Angle;

        particle.collision.SetPlane(0, GameObject.FindGameObjectWithTag("Ground").transform);
    }

    public void Shake(float angle)
    {
        angle = Mathf.Clamp(angle, limitAngle * -1.0f, limitAngle);
        bendDeformer.Angle = angle;
        angleCount += Mathf.Abs(beforeAngle - bendDeformer.Angle);
        if (angleCount >= 20.0f)
        {
            Instantiate(particle).gameObject.SetActive(true);
            angleCount = 0f;
        }

        OnShakeListener?.Invoke(Mathf.Abs(beforeAngle - bendDeformer.Angle));
        beforeAngle = angle;
    }

    public void ShakedAnimation()
    {
        if (isAnimation) return;
        isAnimation = true;
        DOTween.To(() => bendDeformer.Angle, (angle) => bendDeformer.Angle = angle, 0, 0.5f).SetEase(Ease.OutBounce).OnComplete(() => isAnimation = false);
    }
}
