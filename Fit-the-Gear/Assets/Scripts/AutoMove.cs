using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class AutoMove : MonoBehaviour
{
    [SerializeField] Vector3 position;
    [SerializeField] GameObject gears;
    [SerializeField] GameObject particle;

    bool isParticlePlay;

    private void Awake()
    {
        Sequence sequence = DOTween.Sequence();
        sequence.Append(transform.DOMoveY(5.0f,1.0f));
        sequence.Append(transform.DOMove(new Vector3(position.x,5.0f,position.z), 2.0f));
        sequence.Append(transform.DOMoveY(0.1f, 1.0f).OnComplete(() => ActiveRotate()));
    }

    void ActiveRotate()
    {
        var autoRotates = gears.transform.GetComponentsInChildren<AutoRotate>();
        foreach (var autoRotate in autoRotates)
        {
            autoRotate.enabled = true;
            autoRotate.OnRotateCompleate += PlayParticle;
        }
    }

    void PlayParticle()
    {
        if (isParticlePlay) return;

        isParticlePlay = true;
        Instantiate(particle,new Vector3(0.8f,1.5f,0),Quaternion.identity);
    }
}
