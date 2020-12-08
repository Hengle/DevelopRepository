using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyModelSupport : MonoBehaviour
{
    [SerializeField] Transform hitPart;
    [SerializeField] GameObject head;

    public Transform GetHitPart()
    {
        return hitPart;
    }

    public GameObject GetHead()
    {
        return head;
    }
}
