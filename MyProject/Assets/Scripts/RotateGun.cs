using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateGun : MonoBehaviour
{
    [SerializeField] Transform character;
    [SerializeField] Gun gun;

    Quaternion desiredRotation;
    float rotationSpeed = 5f;
    private void Update()
    {
        if (!gun.IsGrappling())
        {
            desiredRotation = transform.rotation;
        }
        else
        {
            desiredRotation = Quaternion.LookRotation(gun.GetGrappPoint() - character.transform.position);
        }

        character.LookAt(gun.GetGrappPoint());
        //transform.rotation = Quaternion.Lerp(character.transform.rotation, desiredRotation, Time.deltaTime * rotationSpeed);
    }
}
