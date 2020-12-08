using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cinemachine;

public class PlushDoll : MonoBehaviour
{
    [SerializeField] GameObject[] sewingPoints;
    [SerializeField] GameObject[] wappenPoints;
    [SerializeField] GameObject accessory;
    [SerializeField] float packingSpeed = 0.3f;
    [SerializeField] FaceAnimation faceAnimation;

    GameObject currentSewingPoint;
    GameObject currentWappenPoint;
    DollCollider[] colliders;
    Slider slider;
    GameType gameType;
    int totalRequiredAmount = 0;
    int nowTotalAmount = 0;

    public float defaultSize;
    public Vector3 defaultPosition;
    public event Action<float> OnWashListener;
    public event Action<float> OnPackingListener;

    private void Awake()
    {
        slider = GameObject.FindGameObjectWithTag("ProgressBar").GetComponent<Slider>();
        colliders = transform.GetComponentsInChildren<DollCollider>();
        defaultSize = transform.localScale.z;
        defaultPosition = transform.position;
        gameType = GameType.Sewing;
    }

    public void Initialize(StageData stageData)
    {
        this.gameType = stageData.GameType;
        //指定の場所をアクティブにする
        switch (gameType)
        {
            case GameType.Sewing:
                currentSewingPoint = sewingPoints[stageData.SewingPoint - 1];
                currentSewingPoint.SetActive(true);
                break;
            case GameType.Wappen:
                currentWappenPoint = wappenPoints[stageData.SewingPoint - 1];
                currentWappenPoint.SetActive(true);
                break;
            case GameType.Washing:
                foreach (var collider in colliders)
                {
                    collider.Activate();
                    totalRequiredAmount += collider.GetRequiredAmount();
                    collider.OnHitListener += Wash;
                }
                break;
            case GameType.Decoration:
                accessory.SetActive(true);
                break;
            case GameType.Packing:
                transform.position = new Vector3(0f,0.5f,0f);
                transform.Rotate(new Vector3(0f, -45f, 0f));
                transform.localScale = new Vector3(defaultSize, defaultSize, defaultSize * 0.1f);
                break;
        }
    }

    public void HomeView(StageData stageData)
    {
        this.gameType = stageData.GameType;
        //指定の場所をアクティブにする
        switch (gameType)
        {
            case GameType.Sewing:
                sewingPoints[stageData.SewingPoint - 1].SetActive(true);
                break;
            case GameType.Wappen:
                 wappenPoints[stageData.SewingPoint - 1].SetActive(true);;
                break;
            case GameType.Washing:
                foreach (var collider in colliders)
                {
                    collider.Activate();
                }
                break;
            case GameType.Decoration:
                break;
            case GameType.Packing:
                transform.localScale = new Vector3(defaultSize, defaultSize, defaultSize * 0.1f);
                break;
        }
    }

    public Vector3[] GetStitches()
    {
        List<Vector3> positions = new List<Vector3>();
        switch (gameType)
        {
            case GameType.Sewing:
                foreach (Transform transform in currentSewingPoint.transform.Find("Stitches").transform)
                {
                    positions.Add(transform.position);
                }
                break;
            case GameType.Wappen:
                var radius = 0.29f;
                var angle = 20f;
                var defPosition = currentWappenPoint.transform.position;

                for (int i = 0; i < 20; i++)
                {
                    var x = radius * Mathf.Cos(i * angle * Mathf.PI / 180.0f);      //X軸の設定
                    var y = radius * Mathf.Sin(i * angle * Mathf.PI / 180.0f);      //Z軸の設定
                    positions.Add(new Vector3(x + defPosition.x, y + defPosition.y, defPosition.z));  //自分のいる位置から座標を動かす。
                }
                break;
            default:
                return null;
        }
        

        return positions.ToArray();
    }

    public CinemachineVirtualCamera GetFocusCamera()
    {
        switch (gameType)
        {
            case GameType.Sewing:
                return currentSewingPoint.transform.Find("Camera").GetComponent<CinemachineVirtualCamera>();
            case GameType.Wappen:
                return currentWappenPoint.transform.Find("Camera").GetComponent<CinemachineVirtualCamera>();
            default:
                return null;
        }
    }

    public TubeRenderer GetYarn()
    {
        switch (gameType)
        {
            case GameType.Sewing:
                return currentSewingPoint.transform.Find("Yarn").GetComponent<TubeRenderer>();
            case GameType.Wappen:
                return currentWappenPoint.transform.Find("Yarn").GetComponent<TubeRenderer>();
            default:
                return null;
        }
    }

    public Transform GetWappenPoint()
    {
        return currentWappenPoint.transform;
    }

    public void Packing(float value)
    {
        transform.localScale = transform.localScale.SetZ(Mathf.Min(transform.localScale.z + value * packingSpeed, defaultSize));
        OnPackingListener?.Invoke(transform.localScale.z / defaultSize);
    }

    public Transform GetAccessoryPoint(AccessoryPosition accessoryPosition)
    {
        switch (accessoryPosition)
        {
            case AccessoryPosition.Head:
                return accessory.transform.Find("Head");
            case AccessoryPosition.Eye:
                return accessory.transform.Find("Eye");
            case AccessoryPosition.Neck:
                return accessory.transform.Find("Neck");
        }

        return null;
    }

    public void WashSkin()
    {
        foreach (var collider in colliders)
        {
            collider.Deactivate();
        }
    }

    void Wash()
    {
        nowTotalAmount++;
        OnWashListener?.Invoke((float)nowTotalAmount / totalRequiredAmount);
    }

    public void Refresh()
    {
        transform.localPosition = Vector3.zero;
        transform.localScale = new Vector3(defaultSize,defaultSize,defaultSize);
    }

    public void ResultAnimation()
    {
        faceAnimation.Clear();
    }
}
