using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Deform;
using UnityEditor;

public class Spring : MonoBehaviour, IAnimationMonitorable
{
    [SerializeField] SquashAndStretchDeformer squash;
    [SerializeField] GameObject glove;
    [SerializeField] GameObject spring;
    [SerializeField] Transform collider;

    MeshFilter meshFilter;
    MeshRenderer meshRenderer;
    IAnimationRegistable registerComponent { get; set; }
    public event Action<ObjectType> OnAnimeStart;
    public event Action<ObjectType> OnAnimeEnd;

    private void Awake()
    {
        meshFilter = GetComponent<MeshFilter>();
        meshRenderer = GetComponent<MeshRenderer>();
        FindRegisterComponent();
    }

    public void SetFirstAct()
    {

    }

    public void FindRegisterComponent()
    {
        registerComponent = GameObjectExtensions.FindComponentWithInterface<IAnimationRegistable>();
        if (registerComponent != null)
        {
            registerComponent.Register(this);
        }
    }

    public void Activate()
    {
        OnAnimeStart?.Invoke(ObjectType.Gimmick);
        DOTween.To(
            () => squash.Factor,
            (f) => squash.Factor = f,
            1.0f,
            2.0f
            )
            .SetEase(Ease.OutElastic)
            .SetDelay(1.0f)
            .OnComplete(() => OnAnimeEnd?.Invoke(ObjectType.Gimmick));
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Enemy")
        {

        }
    }

    private void Update()
    {
        collider.position = collider.position.SetX(meshRenderer.bounds.min.x);
    }

    /*
    //メッシュ合成
    private void Hoge()
    {
        MeshFilter[] meshFilters = new MeshFilter[2];
        CombineInstance[] combine = new CombineInstance[2];

        meshFilters[0] = spring.transform.GetComponent<MeshFilter>();
        meshFilters[1] = glove.transform.GetComponent<MeshFilter>();

        //combine[0].mesh = meshFilter.sharedMesh;
        //combine[0].transform = meshFilter.transform.localToWorldMatrix;

        //meshFilter = glove.transform.GetComponent<MeshFilter>();
        //combine[1].mesh = meshFilter.sharedMesh;
        //combine[1].transform = meshFilter.transform.localToWorldMatrix;

        meshFilter = GetComponent<MeshFilter>();
        meshFilter.mesh = new Mesh();
        meshFilter.mesh = CombineMeshes(meshFilters);
        AssetDatabase.CreateAsset(meshFilter.mesh, "Assets/PunchMachine.asset");
        AssetDatabase.SaveAssets();
    }

    Mesh CombineMeshes(MeshFilter[] meshes)
    {
        // Key: shared mesh instance ID, Value: arguments to combine meshes
        var helper = new Dictionary<int, List<CombineInstance>>();

        // Build combine instances for each type of mesh
        foreach (var m in meshes)
        {
            List<CombineInstance> tmp;
            if (!helper.TryGetValue(m.sharedMesh.GetInstanceID(), out tmp))
            {
                tmp = new List<CombineInstance>();
                helper.Add(m.sharedMesh.GetInstanceID(), tmp);
            }

            var ci = new CombineInstance();
            ci.mesh = m.sharedMesh;
            ci.transform = m.transform.localToWorldMatrix;
            tmp.Add(ci);
        }

        // Combine meshes and build combine instance for combined meshes
        var list = new List<CombineInstance>();
        foreach (var e in helper)
        {
            var m = new Mesh();
            m.CombineMeshes(e.Value.ToArray());
            var ci = new CombineInstance();
            ci.mesh = m;
            list.Add(ci);
        }

        // And now combine everything
        var result = new Mesh();
        result.CombineMeshes(list.ToArray(), false, false);

        // It is a good idea to clean unused meshes now
        foreach (var m in list)
        {
            Destroy(m.mesh);
        }

        return result;

    }
    */
}
