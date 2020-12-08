using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Util;
using UnityEngine.AI;
using DG.Tweening;

public class Player : MonoBehaviour, IAnimationMonitorable
{
    [SerializeField] Camera camera;
    [SerializeField] CameraManager cameraManager;
    TouchEventHandler touchEventHandler;
    Rigidbody myRigidbody;
    Animator myAnimator;
    Vector3 startPos;
    Vector3 beforePos;
    float moveSpeed;
    IAnimationRegistable registerComponent { get; set; }
    Gimmick currentPlayGimmick;

    //public event Action OnAnimationTrigger;
    public event Action<ObjectType> OnAnimeStart;
    public event Action<ObjectType> OnAnimeEnd;

    private void Awake()
    {
        FindRegisterComponent();
        touchEventHandler = Camera.main.GetComponent<TouchEventHandler>();
        myRigidbody = GetComponent<Rigidbody>();
        myAnimator = GetComponent<Animator>();
    }

    private void Start()
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
        cameraManager.FocusPlayer();
        touchEventHandler.OnTouchStartListener += MoveStart;
        touchEventHandler.OnTouchKeepListener += Moving;
        touchEventHandler.OnTouchReleaseListener += MoveEnd;
    }

    public void DeActivate()
    {
        touchEventHandler.OnTouchStartListener -= MoveStart;
        touchEventHandler.OnTouchKeepListener -= Moving;
        touchEventHandler.OnTouchReleaseListener -= MoveEnd;
    }

    public void MoveStart(Vector3 pos)
    {
        pos.z = camera.transform.position.z;
        startPos = camera.ScreenToWorldPoint(pos);
        beforePos = transform.position;
    }

    public void Moving(Vector3 pos)
    {
        myAnimator.SetBool("Move",true);
        pos.z = camera.transform.position.z;
        pos = camera.ScreenToWorldPoint(pos);
        var distance = pos - startPos;
        distance.y = 0f;
        transform.Translate(-1.0f * distance.normalized * Time.deltaTime * 2.0f, Space.World);

        var diff = transform.position - beforePos;
        beforePos = transform.position;
        if (diff.magnitude > 0.01f)
        {
            transform.rotation = Quaternion.LookRotation(diff);
        }
    }

    public void MoveEnd(Vector3 pos)
    {
        myRigidbody.velocity = Vector3.zero;
        myAnimator.SetBool("Move",false);
    }

    public void AutoMoveAndRotate(Vector3 position, Quaternion quaternion, string animationName)
    {
        cameraManager.FocusPlayer();
        OnAnimeStart?.Invoke(ObjectType.Player);
        var sequence = DOTween.Sequence();
        sequence.Append(transform.DOMove(new Vector3(position.x, transform.position.y, position.z), 0.3f));
        sequence.Join(transform.DORotateQuaternion(quaternion, 0.3f));
        sequence.OnComplete(
            () =>
            {
                if (animationName != "")
                {
                    PlayGimmickAnimation(animationName);
                }
                else
                {
                    myAnimator.SetBool("Move", false);
                    myAnimator.Play("Idle");
                    OnAnimeEnd?.Invoke(ObjectType.Player);
                    currentPlayGimmick.ActiveTrap();
                }
            });
    }

    public void PlayGimmickAnimation(string animationName)
    {
        myAnimator.SetBool("Move", false);
        myAnimator.Play(animationName);
        StartCoroutine(DelayInvoke(myAnimator.GetCurrentAnimatorClipInfo(0).Length, () => OnAnimeEnd?.Invoke(ObjectType.Player)));
    }

    public void AnimationTrriger()
    {
        currentPlayGimmick.ActiveTrap();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "GimmickArea")
        {
            currentPlayGimmick = other.transform.parent.GetComponent<Gimmick>();
            currentPlayGimmick.GimmickAreaHide();
            AutoMoveAndRotate(currentPlayGimmick.position, currentPlayGimmick.rotation, currentPlayGimmick.playerAnimation);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Enemy")
        {
            SceneManager.Instance.ChangeScene("Cinemachine");
        }
    }

    IEnumerator DelayInvoke(float time, Action onInvoke)
    {
        yield return new WaitForSeconds(time);
        onInvoke.Invoke();
    }
}
