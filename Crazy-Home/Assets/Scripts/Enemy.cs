using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Cinemachine;
using Util;

public class Enemy : MonoBehaviour, IAnimationMonitorable
{
    [SerializeField] CameraManager cameraManager;
    [SerializeField] CinemachineVirtualCamera enemyCamera;
    [SerializeField] Transform[] checkPoints;
    [SerializeField] GameObject fov;
    [SerializeField] RagdollUtility ragdollUtility;
    [SerializeField] ParticleSystem fire;
    [SerializeField] float power;
    [SerializeField] float upper;

    public event Action<ObjectType> OnAnimeStart;
    public event Action<ObjectType> OnAnimeEnd;
    public event Action OnEscape;
    IAnimationRegistable registerComponent { get; set; }
    Gimmick currentPlayGimmick;

    IEnumerator coroutineMethod;
    NavMeshAgent agent;
    Animator myAnimator;
    int destPoint = 0;
    bool isEvent;
    int life = 1;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        myAnimator = GetComponent<Animator>();
        coroutineMethod = Move();
        ragdollUtility.Setup();
        ragdollUtility.Deactivate();
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

    private void Update()
    {
        if (life == 0 && !agent.isStopped && !agent.pathPending && agent.remainingDistance < 1.0f)
        {
            OnEscape?.Invoke();
        }
    }

    public IEnumerator FirstMove()
    {

        agent.destination = checkPoints[destPoint].position;
        destPoint++;

        myAnimator.Play("Walk");

        fov.SetActive(false);

        yield return new WaitWhile(() => (!agent.pathPending && agent.remainingDistance < 0.1f) == false);

        myAnimator.Play("Look");
    }

    public IEnumerator Move()
    {
        while (checkPoints.Length > destPoint)
        {
            myAnimator.Play("Walk");

            agent.destination = checkPoints[destPoint].position;
            destPoint++;

            yield return new WaitWhile(() => (!agent.pathPending && agent.remainingDistance < 0.1f && !isEvent) == false);

            myAnimator.Play("Look");

            if(destPoint + 1 < checkPoints.Length)
            {
                transform.LookAt(checkPoints[destPoint + 1]);
            }

            yield return new WaitForSeconds(0.1f);

            yield return new WaitForAnimation(myAnimator, 0);

            yield return new WaitWhile(() => isEvent);
        }

        OnEscape?.Invoke();
    }

    public void Play()
    {
        if (life == 0) return;
        isEvent = false;
        agent.isStopped = false;
        fov.SetActive(true);
    }

    public void Stop()
    {
        isEvent = true;
        GetComponent<Rigidbody>().velocity = Vector3.zero;
        agent.isStopped = true;
        fov.SetActive(false);
    }

    void Damage()
    {
        life--;
        cameraManager.FocusEnemy();
        OnAnimeStart?.Invoke(ObjectType.Enemy);
        myAnimator.Play(currentPlayGimmick.enemyAnimation);
    }

    void GetUp()
    {
        OnAnimeEnd?.Invoke(ObjectType.Enemy);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Trap")
        {
            if (other.transform.parent == null)
            {
                currentPlayGimmick = other.transform.GetComponent<Gimmick>();
            }
            else
            {
                currentPlayGimmick = other.transform.parent.GetComponent<Gimmick>();
            }
            Damage();
            StartCoroutine(DelayInvoke(1.0f, GetUp));
        }
    }

    public bool DeathCheck()
    {
        return life == 0;
    }

    public void Escape()
    {
        enemyCamera.m_Priority = 2;
        myAnimator.Play("Escape");
        agent.isStopped = false;
        agent.destination = new Vector3(1.5f,0.1f, 25f);
    }

    IEnumerator DelayInvoke(float time, Action onInvoke)
    {
        yield return new WaitForSeconds(time);
        onInvoke.Invoke();
    }
}
