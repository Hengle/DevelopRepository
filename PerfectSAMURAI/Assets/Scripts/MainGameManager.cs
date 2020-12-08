using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using DG.Tweening;

public class MainGameManager : MonoBehaviour
{
    [SerializeField] CinemachineTargetGroup cinemachineTargetGroup;
    [SerializeField] BladeMode bladeMode;
    [SerializeField] Katana katana;
    [SerializeField] GameObject player;
    [SerializeField] GameObject enemy;

    [SerializeField] CinemachineVirtualCamera sceneCamera;
    [SerializeField] CinemachineVirtualCamera bulletCamera;
    [SerializeField] CinemachineVirtualCamera bladeCamera;

    [SerializeField] bool isReplay;

    [SerializeField] GameObject cake;

    Enemy eScript;
    bool isSlice;


    [SerializeField] SkinnedMeshRenderer skinMesh;
    [SerializeField] Material material;

    private void Awake()
    {
        eScript = enemy.GetComponent<Enemy>();
        katana.OnSliceListener += PlayerSlice;
    }

    private IEnumerator Start()
    {
        
        yield return new WaitForSeconds(1.0f);

        if (!isReplay)
        {
            eScript.Shoot();
            bulletCamera.Priority = 2;

            yield return new WaitForSeconds(0.5f);

            bladeCamera.Priority = 3;

            //yield return new WaitWhile(() => !isSlice);

            sceneCamera.Priority = 4;

            yield return new WaitForSeconds(0.5f);

            bladeMode.Slice();

            yield return new WaitForSeconds(2.45f);

            //動画用処理
            var materials = skinMesh.materials;
            materials[1] = material;
            skinMesh.materials = materials;
            cake.transform.Rotate(new Vector3(300f,0,0));
            cake.GetComponent<Rigidbody>().useGravity = true;
            //foreach(var obj in GameObject.FindGameObjectsWithTag("SliceObject"))
            //{
            //    obj.GetComponent<SliceObject>().AddForce();
            //}
            //if (cake)
            //{
            //    cake.GetComponent<MeshRenderer>().enabled = false;
            //    foreach(var halfCake in cake.transform.GetComponentsInChildren<Rigidbody>())
            //    {
            //        halfCake.AddForce(halfCake.transform.forward * 10f,ForceMode.Impulse);
            //    }
            //}
        }
        
    }

    void PlayerSlice()
    {
        isSlice = true;
        cinemachineTargetGroup.m_Targets[1].weight = 0;
        cinemachineTargetGroup.m_Targets[2].weight = 1;
    }
}
