using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Cinemachine;
using Util;
using Deform;

public class Player : MonoBehaviour
{

    public enum ActionState {
        Move,
        Stab,
        JumpWait,
        Jump
    }

    List<Transform> roads = new List<Transform>();

    Vector3 landingPoint;
    Vector3 jampPoint;
    Vector3 slowPoint;
    Vector3 touchStartPosition;

    [SerializeField] CinemachineVirtualCamera jumpCamera;
    [SerializeField] TouchEventHandler touchEventHandler;
    [SerializeField] GameObject pole;
    [SerializeField] BendDeformer bend;
    [SerializeField] GameObject poleLine;
    [SerializeField] GameObject coin;

    public float speed = 3f;
    public float jumpSpeed = 3f;
    Rigidbody myRigidbody;
    Animator myAnimator;
    private Vector3 moveDirection = Vector3.zero;

    ActionState actionState;

    void Awake()
    {
        myRigidbody = GetComponent<Rigidbody>();
        myAnimator = GetComponent<Animator>();
        myAnimator.SetBool("Move", true);
    }

    private void Start()
    {
        foreach(var obj in GameObject.FindGameObjectsWithTag("Road"))
        {
            roads.Add(obj.transform);
        }

        jampPoint = roads[0].Find("JumpPoint").position.AddX(-8f);
        landingPoint = roads[1].Find("LandingPoint").position;
        slowPoint = jampPoint.AddX(-3f);
    }

    void Activate()
    {
        touchEventHandler.OnTouchStartListener += TouchStart;
        touchEventHandler.OnTouchKeepListener += TouchKeep;
        touchEventHandler.OnTouchReleaseListener += TouchRelease;
    }

    void DeActivate()
    {
        touchEventHandler.OnTouchStartListener -= TouchStart;
        touchEventHandler.OnTouchKeepListener -= TouchKeep;
        touchEventHandler.OnTouchReleaseListener -= TouchRelease;
    }

    void TouchStart(Vector3 position)
    {
        touchStartPosition = position;
    }

    void TouchKeep(Vector3 position)
    {
        var signValueY = Mathf.Sign(position.y - touchStartPosition.y);
        var swipeDistY = (new Vector3(0, position.y / Screen.height, 0) - new Vector3(0, touchStartPosition.y / Screen.height, 0)).magnitude;

        if (signValueY < 0)
        {
            bend.Angle = Mathf.Min(swipeDistY * 100f, 45f) * -1.0f;
        }
        else
        {
            bend.Angle = 0f;
        }
    }

    void TouchRelease(Vector3 position)
    {
        bend.Angle = 0f;
        StartCoroutine(Jump());
    }

    void Update()
    {
       
        if (actionState == ActionState.Jump) return;

        if (actionState == ActionState.Move)
        {
            moveDirection = speed * transform.forward;
            myRigidbody.velocity = new Vector3(moveDirection.x, myRigidbody.velocity.y, myRigidbody.velocity.z);
        }

        if (actionState == ActionState.Move && transform.position.x - slowPoint.x >= 0.1f)
        {
            jumpCamera.Priority = 2;
            myAnimator.SetBool("Stab", true);
            Time.timeScale = 0.5f;
            myRigidbody.useGravity = false;
            actionState = ActionState.Stab;
        }

        if (actionState == ActionState.Stab && transform.position.x - jampPoint.x >= 0f)
        {
            myAnimator.SetBool("Move", false);
            myRigidbody.velocity = Vector3.zero;
            actionState = ActionState.JumpWait;
            transform.parent = roads[0].Find("JumpPoint");
            roads[0].Find("JumpPoint").DORotate(new Vector3(0f, 0f, -15f), 0.8f).SetEase(Ease.Linear).OnComplete(() => Activate());
            poleLine.SetActive(true);
            poleLine.GetComponentInChildren<BendDeformer>().Angle = Random.Range(-45f, 0);
        }

    }

    IEnumerator Jump()
    {
        DeActivate();
        jumpCamera.Priority = 0;
        Time.timeScale = 1.0f;
        actionState = ActionState.Jump;
        myRigidbody.velocity = Vector3.zero;
        myRigidbody.useGravity = false;
        poleLine.SetActive(false);
        myAnimator.SetBool("Jump", true);
        myAnimator.SetBool("Stab", false);
        pole.SetActive(false);

        var jumpStartPosition = transform.position;

        DOTween.To(() => 0f, (t) => transform.position = CalcBezier(jumpStartPosition, landingPoint, 10.0f, t), 1f, 1.0f)
            .SetEase(Ease.Linear);

        yield return new WaitForSeconds(1.1f);

        actionState = ActionState.Move;
        myRigidbody.useGravity = true;
        pole.SetActive(true);
        myAnimator.SetBool("Jump", false);
        myAnimator.SetBool("Move", true);


        roads.RemoveAt(0);
        jampPoint = roads[0].Find("JumpPoint").position.AddX(-8f);
        landingPoint = roads[1].Find("LandingPoint").position;
        slowPoint = jampPoint.AddX(-3f);
        transform.parent = null;
        transform.Rotate(new Vector3(-15f,0f,0f));
    }

    Vector3 CalcBezier(Vector3 start, Vector3 end, float tension, float t)
    {
        var controllPoint = Vector3.Lerp(start, end, 0.5f);
        controllPoint = controllPoint.AddY(tension);

        Vector3 P1 = Vector3.Lerp(start, controllPoint, t);
        Vector3 P2 = Vector3.Lerp(controllPoint, end, t);
        return Vector3.Lerp(P1, P2, t);
    }

    private void OnTriggerEnter(Collider other)
    {
        //if (isStab && !isJump)
        //{
        //    isJump = true;
        //    myAnimator.SetBool("Move", false);
        //    myRigidbody.velocity = Vector3.zero;
        //}

        //if (!isStab)
        //{
        //    myAnimator.SetBool("Stab",true);
        //    isStab = true;
        //    Time.timeScale = 0.5f;
        //}
    }
}
