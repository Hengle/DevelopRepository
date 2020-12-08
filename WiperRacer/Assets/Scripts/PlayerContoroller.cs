using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class PlayerContoroller : MonoBehaviour
{
    [SerializeField] CinemachineVirtualCamera virtualCamera;
    Character character;

    private void Awake()
    {
        character = GetComponent<Character>();
    }
    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            character.AxisSwitching();
            virtualCamera.m_Follow = character.GetAxisObject().transform;
        }
    }
}
