using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public abstract class Gimmick : MonoBehaviour, IAnimationMonitorable
{
    [SerializeField] GimmickArea gimmickArea;
    [SerializeField] protected CameraManager cameraManager;

    protected Player player;
    protected Enemy enemy;
    public Vector3 position;
    public Quaternion rotation;
    public string playerAnimation;
    public string enemyAnimation;
    IAnimationRegistable registerComponent { get; set; }

    virtual public event Action<ObjectType> OnAnimeStart;
    virtual public event Action<ObjectType> OnAnimeEnd;

    public abstract void ActiveTrap();

    private void Awake()
    {
        Initialize();
        FindRegisterComponent();
    }

    protected virtual void Start()
    {

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

    protected void Dispose()
    {
        registerComponent.Unregister(this);
    }

    protected virtual void Initialize()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        enemy = GameObject.FindGameObjectWithTag("Enemy").GetComponent<Enemy>();
        position = gimmickArea.transform.position;
        rotation = gimmickArea.transform.rotation;
    }

    public void GimmickAreaHide()
    {
        gimmickArea.Hide();
    }

}
