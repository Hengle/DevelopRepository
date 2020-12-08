using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Util;
using TMPro;
using DG.Tweening;
using Cinemachine;

public class Character : MonoBehaviour
{
    public enum CharacterType
    {
        Plyer,
        Enemy,
    }

    //protected abstract void Initialize();

    Rigidbody MyRigidbody { get; set; }
    Collider MyCollider { get; set; }
    Animator MyAnimator { get; set; }
    RagdollUtility MyRagdollUtility { get; set; }

    Vector3 startPos, shotPos, force;
    LineRenderer lineRenderer;
    bool isDeath, isCollision, isRagdoll;
    public event Action OnDeathListener;

    [SerializeField] TouchEventHandler eventHandler;
    [SerializeField] float speed;
    [SerializeField] ParticleSystem collisionEffect;
    [SerializeField] TrailRenderer trailRenderer;
    [SerializeField] Transform golfDriver;
    [SerializeField] LayerMask groundLayer;
    [SerializeField] Transform characterAsix;
    [SerializeField] GameObject bulletObj;
    [SerializeField] CinemachineVirtualCameraBase fpsCamera;
    [SerializeField] CinemachineSmoothPath path;

    [SerializeField] Transform simulationLine;
    [SerializeField] GameObject simulationSphere;
    [SerializeField] Transform simulationPoint;
    GameObject[] list;

    float cameraPos;
    float secInterval = 0.1f;

    void Awake()
    {
        MyRigidbody = GetComponent<Rigidbody>();
        MyCollider = GetComponent<Collider>();
        MyAnimator = GetComponent<Animator>();
        MyRagdollUtility = GetComponent<RagdollUtility>();
        lineRenderer = GetComponent<LineRenderer>();
        //MyCollider.isTrigger = true;
        //MyRigidbody.isKinematic = true;
        //MyRigidbody.useGravity = false;

        list = new GameObject[50];
    }

    void Start()
    {
        foreach (var cc in GetComponentsInChildren<CharacterCollider>())
        {
            cc.OnCollisionListener += OnCollision;
        }
        MyRagdollUtility.Setup();
        MyRagdollUtility.Deactivate();

        //eventHandler.OnTouchStartListener += MoveStart;
        //eventHandler.OnTouchKeepListener += Move;
        //eventHandler.OnTouchReleaseListener += MoveEnd;

        shotPos = transform.position;
    }

    private void Update()
    {
        if (isDeath) return;
        if (!isRagdoll) return;
        cameraPos += Time.deltaTime * 0.3f;
        transform.position = path.EvaluatePositionAtUnit(cameraPos, CinemachinePathBase.PositionUnits.Normalized);
    }

    void MoveStart(Vector3 pos)
    {
        simulationLine.gameObject.SetActive(true);
        MyRagdollUtility.Deactivate();
        if (isRagdoll) shotPos = characterAsix.position;
        pos.z = -10f;
        Vector3 worldposition = Camera.main.ScreenToWorldPoint(pos);
        startPos = worldposition;
    }

    void Move(Vector3 pos)
    {
        pos.z = -10f;
        Vector3 worldposition = Camera.main.ScreenToWorldPoint(pos);
        force = worldposition - startPos;
        force.y *= 0.15f;
        force *= 20f;
        if (force.magnitude > 50f * 50f)
        {
            //force *= 50f / force.magnitude;
        }

        SimulationLine(force);
    }

    void MoveEnd(Vector3 pos)
    {
        simulationLine.gameObject.SetActive(false);
        if (isRagdoll)
        {
            MyRagdollUtility.Activate();
            MyRagdollUtility.AddForce(force, force * 2);
        }
        else
        {
            MyAnimator.enabled = true;
        }

        eventHandler.OnTouchStartListener -= MoveStart;
        eventHandler.OnTouchKeepListener -= Move;
        eventHandler.OnTouchReleaseListener -= MoveEnd;
        Invoke("Shot", 0.75f);
        //PlayAnimation("Shot", false);
    }

    public void Move()
    {
        MyAnimator.enabled = true;
        transform.DOMoveZ(transform.position.z + 5f,0.75f).SetEase(Ease.Linear);
        Invoke("Shot", 0.75f);
    }

    void BulletShot(Vector3 pos)
    {
        pos.z = -10f;
        var bullet = Instantiate(bulletObj) as GameObject;
        bullet.transform.position = transform.position;
        Ray r = Camera.main.ScreenPointToRay(pos);
        Vector3 world = r.direction;
        bullet.GetComponent<Rigidbody>().AddForce(world * 500f, ForceMode.Impulse);
    }

    void SimulationLine(Vector3 force)
    {
        Vector3[] positions = new Vector3[0];
        Vector3 collisionPoint;
        Vector3 beforePosition = shotPos;
        bool isCollision = false;
        for (int i = 0; i < 50; i++)
        {
            if (list[i] && isCollision)
            {
                list[i].transform.position = new Vector3(0f,0f,-1000f);
                return;
            }

            var offset = Mathf.Repeat(Time.time * 0.5f, secInterval);

            var pos = Velocity(shotPos, force, Physics.gravity, 1.0f, offset + (secInterval * i));
            Ray ray = new Ray(beforePosition, (pos - beforePosition).normalized);
            Debug.DrawRay(ray.origin, ray.direction * (pos - beforePosition).magnitude, Color.red);

            if (i > 1 && LineCollisionCheck(beforePosition, pos - beforePosition, out collisionPoint))
            {
                isCollision = true;
                simulationPoint.position = collisionPoint.AddY(0.1f);
            }
            else
            {
                if (list[i])
                {
                    list[i].transform.position = pos;
                }
                else
                {
                    list[i] = Instantiate(simulationSphere, pos, Quaternion.identity, simulationLine);
                }
            }

            beforePosition = pos;
        }
    }

    bool LineCollisionCheck(Vector3 from, Vector3 to, out Vector3 hitPoint)
    {
        RaycastHit hit;
        hitPoint = Vector3.zero;
        if (Physics.Raycast(from, to, out hit, to.magnitude, groundLayer))
        {
            hitPoint = hit.point;
            return true;
        }
        else
        {
            return false;
        }
    }

    public void Landing()
    {
        MyAnimator.SetTrigger("JumpDown");
    }

    void Shot()
    {
        //MyAnimator.enabled = false;
        //MyRagdollUtility.Activate();
        //MyRagdollUtility.AddForce(force, force * 2);
        //MyRigidbody.useGravity = true;
        //MyRigidbody.AddForce(force,ForceMode.VelocityChange);

        isRagdoll = true;
        fpsCamera.Priority = 2;
        trailRenderer.enabled = true;

        Time.timeScale = 0.3f;
        eventHandler.OnTouchStartListener += BulletShot;

        Debug.Log("Shot:"+force);
    }

    public void Shot(Vector3 vector)
    {
        MyAnimator.enabled = false;
        trailRenderer.enabled = true;
        MyRagdollUtility.Activate();
        MyRagdollUtility.AddForce(vector, vector * 2);
        Debug.Log("Shot:" + force);
    }

    public void ResetFPS()
    {
        Time.timeScale = 1.0f;
        fpsCamera.Priority = 0;
        eventHandler.OnTouchStartListener -= BulletShot;
    }

    public float GetCameraPos()
    {
        return cameraPos;
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

    public void DeathRagdoll(Vector3 direction)
    {
        isDeath = true;
        MyAnimator.enabled = false;
        MyCollider.enabled = false;
        MyRagdollUtility.Activate();
        MyRagdollUtility.AddForce(direction, direction * 1000f);
        OnDeathListener?.Invoke();
    }

    void Fly()
    {
        MyRigidbody.AddForce(new Vector3(0, 0, speed), ForceMode.Force);
        transform.localRotation = Quaternion.LookRotation(MyRigidbody.velocity);
        MyRigidbody.velocity *= 0.9f;

        Vector3 worldposition = Camera.main.ScreenToWorldPoint(Vector3.zero);
        var distance = worldposition - startPos;
        distance = distance.normalized * speed;
        MyRigidbody.AddForce(new Vector3(distance.x, distance.y, 0), ForceMode.Force);
    }

    private void OnCollision(Collision collision, Transform transform)
    {
        if (collision.gameObject.tag == "Ground")
        {
            Instantiate(collisionEffect, transform.position, Quaternion.identity);
        }

        if (collision.gameObject.tag == "Gimmick" && !isCollision)
        {
            isCollision = true;
            MyRagdollUtility.ResetPhysics();
            collision.transform.GetComponent<Gimmick>().ActiveTrap();
            force.x *= -1f;
            MyRagdollUtility.AddForce(force, force * 2);
        }

        if (collision.gameObject.tag == "Animal" && !isCollision)
        {
            isCollision = true;
            MyRagdollUtility.ResetPhysics();
            MyRagdollUtility.Deactivate();
            trailRenderer.enabled = false;
            collision.transform.GetComponent<Gorilla>().ActiveAction(this);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isDeath) return;
        Instantiate(collisionEffect, transform.position, Quaternion.identity);

        var damagable = other.GetComponent<IDamagable>();
        if (damagable == null) return;
        damagable.Damage(this);
    }

    public Transform GetCharacterAsix()
    {
        return characterAsix;
    }

    public Vector3 Force(Vector3 start, Vector3 force, float mass, Vector3 gravity, float gravityScale, float time)
    {
        var speedX = force.x / mass * Time.fixedDeltaTime;
        var speedY = force.y / mass * Time.fixedDeltaTime;
        var speedZ = force.z / mass * Time.fixedDeltaTime;

        var halfGravityX = gravity.x * 0.5f * gravityScale;
        var halfGravityY = gravity.y * 0.5f * gravityScale;
        var halfGravityZ = gravity.z * 0.5f * gravityScale;

        var positionX = speedX * time + halfGravityX * Mathf.Pow(time, 2);
        var positionY = speedY * time + halfGravityY * Mathf.Pow(time, 2);
        var positionZ = speedZ * time + halfGravityZ * Mathf.Pow(time, 2);

        return start + new Vector3(positionX, positionY, positionZ);
    }

    public Vector3 Velocity(Vector3 start, Vector3 velocity, Vector3 gravity, float gravityScale, float time)
    {
        var halfGravityX = gravity.x * 0.5f * gravityScale;
        var halfGravityY = gravity.y * 0.5f * gravityScale;
        var halfGravityZ = gravity.z * 0.5f * gravityScale;

        var positionX = velocity.x * time + halfGravityX * Mathf.Pow(time, 2);
        var positionY = velocity.y * time + halfGravityY * Mathf.Pow(time, 2);
        var positionZ = velocity.z * time + halfGravityZ * Mathf.Pow(time, 2);

        return start + new Vector3(positionX, positionY, positionZ);
    }
}
