using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Util;

public class Player : MonoBehaviour
{
    [SerializeField] GameObject myModel;
    [SerializeField] SimulationLine[] simulationLines;                         //軌道予測線
    [SerializeField] LayerMask layerMask;

    Rigidbody myRigidBody;
    Collider myCollider;
    Animator myAnimator;
    AnimationEventHandler animationEventHandler;
    GameObject handPoinnt;
    WeaponBase weaponScript;
    TouchEventHandler touchEventHandler;                             //TouchEventHandler
    GameObject weapon;
    public event Action OnDeadListener;


    Vector3 throwPosition;
    Vector3 controllPoint1;
    Vector3 controllPoint2;

    bool isCoolTime = false;
    WeaponMaster weaponMaster;

    private void Awake()
    {
        myRigidBody = transform.GetComponent<Rigidbody>();
        myCollider = transform.GetComponent<CapsuleCollider>();
        myAnimator = Instantiate(myModel,transform).transform.GetComponent<Animator>();
        animationEventHandler = myAnimator.transform.GetComponent<AnimationEventHandler>();
        handPoinnt = GameObject.FindGameObjectWithTag("Hand");
        touchEventHandler = Camera.main.transform.GetComponent<TouchEventHandler>();
        animationEventHandler.SetEventReceiver("Flip", Flip);

    }

    private void Start()
    {
        var path = "Master/WeaponMaster";

        if (string.IsNullOrEmpty(path))
        {
            return;
        }

        weaponMaster = Instantiate(Resources.Load<WeaponMaster>(path));
        DataManager.Instance.OnSetWeaponListener += SetWeapon;
        Invoke("SetWeapon", 0.1f);
    }

    void OnDestroy()
    {
        DataManager.Instance.OnSetWeaponListener -= SetWeapon;
    }

    // ドラック開始
    public void TouchStart(Vector3 positon)
    {
    }

    // ドラッグ中
    public void TouchKeep(Vector3 positon)
    {
        Ray ray = Camera.main.ScreenPointToRay(positon);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 100, layerMask))
        {
            var direction = hit.point - transform.position;
            transform.LookAt(new Vector3(direction.x, 0.5f, direction.z));
            transform.eulerAngles = transform.eulerAngles.SetX(0f);
            Vector3[] positions = new Vector3[2];
            direction = simulationLines[0].transform.forward * 30f;
            positions[0] = new Vector3(transform.position.x, 0.5f, transform.position.y);
            positions[1] = new Vector3(transform.position.x, 0.5f, transform.position.y) + direction.SetY(0.5f);
            if (!simulationLines[0].GetActive()) simulationLines[0].SetActive(true);
            simulationLines[0].DrawLine(positions);

            //3way処理
            if (weaponScript.GetWeaponType() == WeaponType.Spear)
            {
                for (int i = 1; i < simulationLines.Length; i++)
                {
                    if (!simulationLines[i].GetActive()) simulationLines[i].SetActive(true);
                    direction = simulationLines[i].transform.forward * 30f;
                    positions[1] = new Vector3(transform.position.x, 0.5f, transform.position.y) + direction.SetY(0.5f);
                    simulationLines[i].DrawLine(positions);
                }
            }
        }   
        Shoot();
    }

    // ドラッグ終了
    public void TouchRelease(Vector3 positon)
    {
        if (weaponScript.GetWeaponType() == WeaponType.Spear)
        {
            simulationLines[0].SetActive(false);
            simulationLines[1].SetActive(false);
            simulationLines[2].SetActive(false);
        }
        else
        {
            simulationLines[0].SetActive(false);
        }
    }

    // シュート
    public void Shoot()
    {
        if (isCoolTime) return;
        isCoolTime = true;
        myAnimator.Play("Idle");
        myAnimator.SetBool("CoolTime", isCoolTime);
        myAnimator.SetTrigger("Throw");
    }

    public void Flip()
    {
        var pos = weaponScript.transform.position;
        weaponScript.Flip(transform.forward,transform.rotation);
        if (weaponScript.GetWeaponType() == WeaponType.Spear)
        {
            var copy = Instantiate(weapon,pos, simulationLines[1].transform.rotation).GetComponent<WeaponBase>();
            copy.Flip(simulationLines[1].transform.forward, simulationLines[1].transform.rotation);
            copy.transform.localScale = weaponScript.transform.localScale;
            copy = Instantiate(weapon, pos, simulationLines[1].transform.rotation).GetComponent<WeaponBase>();
            copy.Flip(simulationLines[2 ].transform.forward, simulationLines[2].transform.rotation);
            copy.transform.localScale = weaponScript.transform.localScale;
        }

        Invoke("ReloadWeapon", weaponScript.GetCoolTime());
    }

    void ReloadWeapon()
    {
        isCoolTime = false;
        myAnimator.SetBool("CoolTime", isCoolTime);
        weaponScript = Instantiate(weapon, handPoinnt.transform).GetComponent<WeaponBase>();
    }

    public void Activate()
    {
        //タッチイベント登録
        touchEventHandler.OnTouchStartListener += TouchStart;
        touchEventHandler.OnTouchKeepListener += TouchKeep;
        touchEventHandler.OnTouchReleaseListener += TouchRelease;
    }

    public void Deactivate()
    {
        //タッチイベント登録
        touchEventHandler.OnTouchStartListener -= TouchStart;
        touchEventHandler.OnTouchKeepListener -= TouchKeep;
        touchEventHandler.OnTouchReleaseListener -= TouchRelease;
    }

    void OnTriggerEnter(Collider collider)
    {
        var iHittable = collider.gameObject.GetComponent<IHittable>();
        if (iHittable != null)
        {
            iHittable.Hit();

            if (iHittable.IsKillable())
            {
                Dead();
            }
        }
    }

    void SetWeapon()
    {
        foreach (Transform t in handPoinnt.transform)
        {
            if (t.gameObject.tag == "Weapon")
            {
                Destroy(t.gameObject);
            }
        }
        weapon = weaponMaster.GetWeaponData(DataManager.Instance.WeaponId).Prefab;
        weaponScript = Instantiate(weapon, handPoinnt.transform).GetComponent<WeaponBase>();
    }

    void Dead()
    {
        myAnimator.Play("Death");
        OnDeadListener?.Invoke();
    }

    //新しい斧のセット
    //public void SetWeapon(Weapon weapon)
    //{
    //    this.weaponScript = weapon;
    //    throwPosition = transform.position;
    //}

    ////ターゲットの設定
    //public void SetTarget(Vector3 target)
    //{
    //    targetPositon = targetPositon.SetX(target.x);
    //    targetPositon = targetPositon.SetY(transform.position.y + 1.0f);
    //    targetPositon = targetPositon.SetZ(target.z);
    //    controllPoint1 = Vector3.Lerp(weapon.transform.position, targetPositon, 0.15f);
    //    controllPoint2 = Vector3.Lerp(weapon.transform.position, targetPositon, 0.85f);
    //}

    //void CalcMoveControllPoint(Vector2 move, Vector3 controll1, Vector3 controll2)
    //{
    //    float a = (targetPositon.z - throwPosition.z) / (targetPositon.x - throwPosition.x);
    //    float aa = -1 / a;
    //    float b1 = aa * controll1.x - controll1.z;
    //    float b2 = aa * controll2.x - controll2.z;
    //    controll1 = controll1.SetX(controllPoint1.x + move.x);
    //    controll1 = controll1.SetZ(aa * controll1.x - b1);
    //    controll2 = controll2.SetX(controllPoint2.x + move.x);
    //    controll2 = controll2.SetZ(aa * controll2.x - b2);

    //    Vector3[] positions = new Vector3[15];

    //    for (int i = 0; i < positions.Length; i++)
    //    {
    //        positions[i] = VectorUtility.CalcBezier(throwPosition, targetPositon, controll1, controll2, (float)i / (float)positions.Length);
    //    }
    //    simulationLine.DrawLine(positions);

    //    weaponScript.SetTarget(transform.position, targetPositon, controll1, controll2);
    //}

    //物理挙動終了
    //sinカーブ
    //2πかけて横半分移動すると元の位置にもどる
    //var hz = 1.0f; //周波数
    //distance.x = Mathf.Sin(distance.x / Screen.width * Mathf.PI * 2 * hz ) * forceRate;
    //distance.y = Mathf.Sin(distance.y / Screen.height * Mathf.PI * 2 * hz) * forceRate;

    //var position = positon;
    //var distance = position - dragStart;

    //distance.x = distance.x / Screen.width * forceRate;
    //distance.y = distance.y / Screen.height * forceRate;
    //if (isCurveReady)
    //{
    //curveForce = distance.x * 0.3f;
    //curveForce = Mathf.Min(MaxCurveForce, Mathf.Max(MaxCurveForce * -1.0f, curveForce)) * ballRigidbody.mass;
    //ball.SetCurve(curveForce);
    //}

    //distance.x = Mathf.Min(4.5f, Mathf.Max(-4.5f, distance.x));
    //currentForce = new Vector3(targetPositon.x, targetPositon.y, targetPositon.z + distance.x) * ballRigidbody.mass;


    //Vector3[] positions = new Vector3[10];

    //for (int i = 0; i < positions.Length; i++)
    //{
    //放物線運動の公式に乗っ取り0.1秒毎の位置を予測
    //Vector3 force = (new Vector3(currentForce.x, currentForce.y, currentForce.z - (curveForce * 0.4f * i)) / ballRigidbody.mass);
    //var t = (i * 0.1f);
    //var g = Physics.gravity.y * -1.0f;
    //var x = t * force.x;                                    //v0cosθ
    //var y = (force.y * t) - 0.5f * g * Mathf.Pow(t, 2.0f);  //−0.5gt2 + y0 + v0tsinθ
    //var z = t * force.z;

    //positions[i] = ball.transform.position + new Vector3(x, y, z);
    //}

    //simulationLine.DrawLine(positions);

    //direction.SetPosition(0, currentPosition);
    //direction.SetPosition(1, currentPosition + currentForce);
    //StartCoroutine(Simulation());
}
