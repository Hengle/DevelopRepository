using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraManager : MonoBehaviour
{
    [SerializeField] CinemachineVirtualCamera focusCamera;
    Camera mainCamera;
    float distance;

    void Awake()
    {
        focusCamera.m_Follow = GameObject.FindGameObjectWithTag("Player").transform;
        distance = focusCamera.GetCinemachineComponent<CinemachineFramingTransposer>().m_CameraDistance;
        mainCamera = Camera.main;
    }

    public void FocusPlayer()
    {
        focusCamera.m_Follow = GameObject.FindGameObjectWithTag("Player").transform;
    }

    public void FocusEnemy()
    {
        focusCamera.m_Follow = GameObject.FindGameObjectWithTag("Enemy").transform;
    }

    public void FocusObject(Transform transform)
    {
        focusCamera.m_Follow = transform;
    }

    public Camera GetMainCamera()
    {
        return Camera.main;
    }
}
