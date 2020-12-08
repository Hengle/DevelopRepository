using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIDrug : MonoBehaviour
{
    [SerializeField] DrugType myDrugType;

    public DrugType GetDrugType()
    {
        return myDrugType;
    }
}
