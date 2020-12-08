using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class StartAnimation : MonoBehaviour
{
    [SerializeField] Player player;
    [SerializeField] Enemy enemy;
    [SerializeField] CameraManager cameraManager;
    [SerializeField] CinemachineVirtualCamera cinemachineVirtualCamera;

    private void Awake()
    {
    }
    private IEnumerator Start()
    {
        cameraManager.FocusEnemy();

        yield return StartCoroutine(enemy.FirstMove());

        yield return new WaitForSeconds(0.5f);

        cinemachineVirtualCamera.Priority = 2;

        yield return new WaitForSeconds(1.0f);

        cameraManager.FocusPlayer();

        yield return new WaitForSeconds(1.0f);

        cinemachineVirtualCamera.Priority = 0;

        player.Activate();
        StartCoroutine(enemy.Move());
    }
}
