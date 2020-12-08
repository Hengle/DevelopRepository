using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Util;
using TMPro;
using DG.Tweening;

public class Character : MonoBehaviour
{
    public enum CharacterType
    {
        Plyer,
        Enemy,
    }

    public enum Mode
    {
        Run,
        Fly,
        Descent,
    }

    [SerializeField] float fowerdSpeed = 10f;
    [SerializeField] float maxVelocityLimit = 30f;
    [SerializeField] float yawEffect = 10f;
    [SerializeField] float downSpeed = 15f;
    [SerializeField] float rotationSpeed = 60f;
    [SerializeField] float boostRollSpeed = 500f;
    [SerializeField] float rollLimit = 45f;
    [SerializeField] float pitchLimit = 30f;
    [SerializeField] Vector3 rocketVector = new Vector3(0f, 0f, 0f);
    [SerializeField] GameObject glider;
    [SerializeField] bool isPlayer;
    [SerializeField] LayerMask layerMask;
    [SerializeField] LayerMask deathMask;
    [SerializeField] ParticleSystem boostEffect;
    [SerializeField] GameObject crown;

    Rigidbody MyRigidbody { get; set; }
    Collider MyCollider { get; set; }
    Animator MyAnimator { get; set; }
    RagdollUtility MyRagdollUtility { get; set; }
    TouchEventHandler touchEventHandler;

    Vector3 colExtents;
    Vector3 touchStartPos;
    float maxVelocity;
    float vertical, horizontal;
    bool isFly;
    bool isDescentMode;
    bool isRolling;
    bool isHold;
    bool isDeath;
    bool isActive;
    int waitAnimationCount;

    public event Action OnFlyListener;
    public event Action OnGoalListener;
    public event Action OnDeathListener;

    void Awake()
    {
        MyRigidbody = GetComponent<Rigidbody>();
        MyCollider = GetComponentInChildren<Collider>();
        MyAnimator = GetComponentInChildren<Animator>();
        MyRagdollUtility = GetComponent<RagdollUtility>();
        touchEventHandler = Camera.main.GetComponent<TouchEventHandler>();

        if (isPlayer)
        {
            touchEventHandler.OnTouchStartListener += (pos) =>
            {
                touchStartPos = pos;
                isHold = true;
            };

            touchEventHandler.OnTouchKeepListener += (pos) =>
            {
                var dir = (pos - touchStartPos) / Screen.dpi;
                dir.y = 0f;
                dir.z = 0f;
                if (dir.magnitude > 0.5f)
                {
                    FlyMove(0f, dir.x > 0f ? 1.0f : -1.0f);
                }
                else
                {
                    FlyMove(0f, 0f);
                }
            };

            touchEventHandler.OnTouchReleaseListener += (pos) =>
            {
                FlyMove(0f, 0f);
                isHold = false;
            };
        }

        MyRigidbody.useGravity = false;
        colExtents = MyCollider.bounds.extents;
        maxVelocity = maxVelocityLimit;
    }

    void Start()
    {
        if (MyRagdollUtility != null)
        {
            MyRagdollUtility.Setup();
            MyRagdollUtility.Deactivate();
        }
    }

    private void Update()
    {
        if (!isActive) return;
        if (isDeath) return;
        FlyingCheck();
        DeathCheck();
        if (!isFly) return;

        if (isPlayer && waitAnimationCount == 0)
        {
            if (isHold && isDescentMode)
            {
                StartCoroutine(SwitchDescentMode(false));
            }
            else if(!isHold && !isDescentMode)
            {
                StartCoroutine(SwitchDescentMode(true));
            }
        }
    }

    void FixedUpdate()
    {
        if (!isActive) return;
        if (isDeath) return;
        if (isFly)
        {
            Fly();

            AddRoll(-horizontal);
            if (horizontal == 0) ResetRoll();
            CapZVelocity();
            CapYVelocity();

            //UpdateYawFromRoll();
        }
        else
        {
            Move();
        }
    }

    protected IEnumerator WaitAnimation(string stateName, Action complete = null)
    {
        yield return new WaitForSeconds(0.1f);
        yield return new WaitWhile(() => MyAnimator.GetCurrentAnimatorStateInfo(0).fullPathHash == Animator.StringToHash(stateName));
        complete?.Invoke();
    }

    public void PlayAnimation(String animationName, bool isTrigger)
    {
        if (isTrigger)
        {
            MyAnimator.SetTrigger(animationName);
        }
        else
        {
            MyAnimator.Play(animationName);
        }
    }

    protected void DeathRagdoll(Vector3 direction)
    {
        MyAnimator.enabled = false;
        MyCollider.enabled = false;
        MyRagdollUtility.Activate();
        MyRagdollUtility.AddForce(direction, direction * 2);

        OnDeathListener?.Invoke();
    }

    void RocketStart()
    {
        isFly = true;
        MyRigidbody.AddForce(rocketVector, ForceMode.Impulse);
        MyRigidbody.velocity = new Vector3(0f, -3f, 30f);
    }

    void FlyingCheck()
    {
        if (IsGrounded())
        {
            ModeSwitch(false);
        }
        else
        {
            ModeSwitch(true);
        }
    }

    void DeathCheck()
    {
        if (!isDeath && MyRigidbody.velocity.z < 1.0f)
        {
            RaycastHit hit;
            var isHit = Physics.BoxCast(transform.position + Vector3.up + transform.forward * -1f, colExtents * 0.5f, transform.forward, out hit, Quaternion.identity, 3f, deathMask);
            if (isHit && hit.collider.gameObject.tag != "JumpBoard")
            {
                isDeath = true;
                MyRigidbody.velocity = Vector3.zero;
                OnDeathListener?.Invoke();
            }
        }
    }

    void ModeSwitch(bool flg)
    {
        if (isFly == flg) return;
        if (flg) OnFlyListener?.Invoke();
        isFly = flg;
        isDescentMode = false;
        MyAnimator.SetBool("Fly", isFly);
        MyAnimator.SetBool("Dive", isDescentMode);
        maxVelocity = maxVelocityLimit;
        MyRigidbody.useGravity = !isFly;
        MyRigidbody.rotation = Quaternion.identity;

        if (isFly)
        {
            glider.transform.DOScaleX(0.8f, 0.3f);
            glider.transform.DOScaleY(0.8f, 0.3f);
        }
        else
        {
            glider.transform.DOScaleX(0f, 0.3f);
            glider.transform.DOScaleY(0f, 0.3f);
        }
    }

    public IEnumerator SwitchDescentMode(bool value)
    {
        isDescentMode = value;
        MyAnimator.SetBool("Dive", isDescentMode);
        maxVelocity = isDescentMode ? maxVelocity * 2f : maxVelocityLimit;
        if (isDescentMode)
        {
            glider.transform.DOScaleX(0f, 0.3f);
            glider.transform.DOScaleY(0f, 0.3f);
        }
        else
        {
            glider.transform.DOScaleX(0.8f, 0.3f);
            glider.transform.DOScaleY(0.8f, 0.3f);
        }
        waitAnimationCount++;

        yield return new WaitForSeconds(0.3f);

        waitAnimationCount--;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Ring")
        {
            MyRigidbody.AddForce(transform.forward * 100f, ForceMode.Impulse);
            StartCoroutine(BoostAnimation());
            if (isDescentMode) StartCoroutine(SwitchDescentMode(false));
            if (isPlayer)
            {
                boostEffect.Play();
            }
        }

        if (other.gameObject.tag == "Goal")
        {
            isActive = false;
            MyRigidbody.isKinematic = true;
            OnGoalListener?.Invoke();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Ground")
        {
            isDeath = true;
            MyRigidbody.velocity = Vector3.zero;
            OnDeathListener?.Invoke();
        }
    }

    public void FlyMove(float v, float h)
    {
        vertical = v;
        horizontal = h;
    }

    void Fly()
    {
        Vector3 velocity = Vector3.zero;

        //前方加速度
        if (MyRigidbody.velocity.z < maxVelocity && !isDescentMode)
        {
            velocity += transform.forward * fowerdSpeed;
        }

        MyRigidbody.velocity = new Vector3(horizontal * 10f, MyRigidbody.velocity.y, MyRigidbody.velocity.z);

        //自動降下処理
        if (MyRigidbody.velocity.y > -3f && !isDescentMode)
        {
            velocity += Vector3.down * downSpeed;
        }

        if (isDescentMode)
        {
            velocity += transform.forward * fowerdSpeed * 2f;
            velocity += Vector3.down * downSpeed * 2f;
        }

        //移動処理
        MyRigidbody.AddForce(velocity);
    }

    void Move()
    {
        //移動処理
        MyRigidbody.velocity = new Vector3(horizontal * yawEffect, MyRigidbody.velocity.y, Mathf.Max(MyRigidbody.velocity.x, maxVelocityLimit));
    }

    void CapYVelocity()
    {
        if (isDescentMode) return;
        var velocity = MyRigidbody.velocity;
        velocity.y = Mathf.Max(velocity.y, -3f);
        MyRigidbody.velocity = velocity;
    }

    void CapZVelocity()
    {
        if (isDescentMode) return;

        // 移動速度制限処理
        Vector3 velocity = MyRigidbody.velocity;
        velocity.x = 0f;
        velocity.y = 0f;
        var maxSqrV = maxVelocity * maxVelocity;
        if (velocity.sqrMagnitude > maxSqrV)
        {
            velocity = MyRigidbody.velocity - (velocity.normalized * maxVelocity);
            velocity.x = 0f;
            velocity.y = 0f;
            velocity *= Time.fixedDeltaTime;
            MyRigidbody.velocity -= velocity;
        }
    }

    public void AddRoll(float additiveRoll, bool dodge = false)
    {

        if (rollLimit > 0 && !dodge)
        {
            if (!CheckRollLimit(additiveRoll))
            {
                return;
            }
        }

        if (!dodge)
        {
            additiveRoll *= Time.deltaTime * rotationSpeed;
        }
        else
        {
            additiveRoll *= Time.deltaTime * boostRollSpeed;
        }

        Quaternion rotator = Quaternion.AngleAxis(additiveRoll, transform.forward);
        transform.rotation = rotator * transform.rotation;
    }

    public void ResetRoll()
    {
        if (isRolling) return;
        Vector3 rightNoY = Vector3.Cross(Vector3.up, transform.forward);
        rightNoY.y = 0;
        Quaternion rotator = Quaternion.FromToRotation(transform.right, rightNoY);

        transform.rotation = Quaternion.Slerp(transform.rotation, rotator * transform.rotation, Time.deltaTime * 5f);
        MyRigidbody.velocity = new Vector3(MyRigidbody.velocity.x * 0.5f, MyRigidbody.velocity.y, MyRigidbody.velocity.z);
    }

    private bool CheckRollLimit(float additiveRoll)
    {
        Vector3 rightNoY = transform.right;
        rightNoY.y = 0;
        rightNoY.Normalize();

        float roll = Vector3.Angle(transform.right, rightNoY);

        if (Vector3.Cross(transform.forward, rightNoY).y < 0)
        {
            roll *= -1;
        }

        if (rollLimit > 0)
        {
            if (roll + additiveRoll > rollLimit)
            {
                return false;
            }
        }

        return true;
    }

    public void AddPitch(float additivePitch)
    {
        if (pitchLimit > 0)
        {
            if (!CheckPitchLimit(pitchLimit))
            {
                return;
            }
        }

        additivePitch *= Time.deltaTime * rotationSpeed;

        Quaternion rotator = Quaternion.AngleAxis(additivePitch, transform.right);
        transform.rotation = rotator * transform.rotation;
    }

    public void ResetPitch()
    {
        if (isRolling) return;
        Vector3 forwardNoY = transform.forward;
        forwardNoY.y = 0;
        Quaternion rotator = Quaternion.FromToRotation(transform.forward, forwardNoY);

        transform.rotation = Quaternion.Slerp(transform.rotation, rotator * transform.rotation, Time.deltaTime);
    }

    private bool CheckPitchLimit(float additivePitch)
    {
        Vector3 forwardNoY = transform.forward;
        forwardNoY.y = 0;
        forwardNoY.Normalize();

        float pitch = Vector3.Angle(transform.forward, forwardNoY);

        if (Vector3.Cross(transform.right, forwardNoY).y < 0)
        {
            pitch *= -1;
        }

        if (pitchLimit > 0)
        {
            if (pitch + additivePitch > pitchLimit)
            {
                return false;
            }
        }

        return true;
    }

    private IEnumerator BoostAnimation()
    {
        float sumInput = 0;
        float input = 1.0f;
        isRolling = true;
        waitAnimationCount++;

        Vector3 oldRight = transform.right;

        while (Mathf.Abs(sumInput) < 360)
        {
            AddRoll(-input, true);

            sumInput += input * boostRollSpeed * Time.deltaTime;

            yield return new WaitForFixedUpdate();
        }

        waitAnimationCount--;
        isRolling = false;
    }

    private void UpdateYawFromRoll()
    {
        if (!isRolling)
        {
            float upSign = 1;

            if (transform.up.y < 0)
            {
                upSign = -1;
            }

            float yawSensibility = 0.33f;
            Vector3 rightNoY = transform.right;
            rightNoY.y = 0;
            rightNoY.Normalize();
            float dot = Vector3.Dot(transform.up, rightNoY);

            yawSensibility *= dot * upSign;

            Quaternion rotator = Quaternion.AngleAxis(yawSensibility, Vector3.up);
            transform.rotation = rotator * transform.rotation;
        }
    }

    public bool IsGrounded()
    {
        Ray ray = new Ray(transform.position + Vector3.up * 2 * colExtents.x, Vector3.down);
        return Physics.SphereCast(ray, colExtents.x, colExtents.x + 0.5f, layerMask);
    }

    public bool IsDescentMode()
    {
        return isDescentMode;
    }

    public Vector3 GetFlyForce()
    {
        Vector3 force = Vector3.zero;
        force += Vector3.forward * fowerdSpeed;
        force += Vector3.right * yawEffect;
        force += Vector3.down * 3f;
        return force;
    }

    public Vector3 GetDescentForce()
    {
        Vector3 force = Vector3.zero;
        force += transform.forward * fowerdSpeed * 2f;
        force += Vector3.down * downSpeed * 2f;
        return force;
    }

    public void SetMaxVelocity(float velcity)
    {
        maxVelocity += velcity;
        maxVelocity = Mathf.Clamp(maxVelocity, 25f, 35);
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        colExtents = GetComponentInChildren<Collider>().bounds.extents;

        Ray ray = new Ray(transform.position + Vector3.up * 2 * colExtents.x, Vector3.down);

        Gizmos.DrawRay(transform.position + Vector3.up * 2 * colExtents.x, Vector3.down * colExtents.x * 2f);
        Gizmos.DrawCube(transform.position + Vector3.up * 2 * colExtents.x + transform.forward, colExtents * 0.5f);
        Gizmos.DrawWireSphere(transform.position + new Vector3(0f, -(colExtents.x + 0.2f), 0f), colExtents.x);
    }

    public GameObject GetGlider()
    {
        return glider;
    }

    public void SetCrown(bool value)
    {
        crown.SetActive(value);
    }

    public void SetActive(bool value)
    {
        isActive = value;
        if (isActive)
        {
            MyAnimator.Play("Run");
        }
        else
        {
            MyAnimator.Play("Idle");
        }
    }
}
