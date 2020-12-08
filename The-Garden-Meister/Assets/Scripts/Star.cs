using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Star : MonoBehaviour
{
    [SerializeField] ParticleSystem particle;

    bool isPlayed;
    GameObject[] paperList;

    private void Awake()
    {
        isPlayed = false;
        particle.Stop();
        paperList = GameObject.FindGameObjectsWithTag("Paper");
    }
}

