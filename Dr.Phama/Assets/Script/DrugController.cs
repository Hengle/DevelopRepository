using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Util;

public class DrugController : MonoBehaviour
{
    [SerializeField] GameObject drugPlefab;
    GameObject drugObject;
    Drug drug;
    TouchEventHandler touchEventHandler;
    bool isMove;

    public float forceAmount = 500;
    Camera targetCamera;
    Vector3 originalScreenTargetPosition;
    Vector3 originalRigidbodyPos;
    float selectionDistance;

    public event Action<Drug> OnGeneratDrugListener;

    void Awake()
    {
        touchEventHandler = Camera.main.transform.GetComponent<TouchEventHandler>();
        touchEventHandler.OnTouchStartListener += TouchStart;
        touchEventHandler.OnTouchKeepListener += TouchKeep;
        touchEventHandler.OnTouchReleaseListener += TouchRelease;
    }

    public void TouchStart(Vector3 positon)
    {
        GeneratDrugFromTap(positon);
    }

    public void TouchKeep(Vector3 positon)
    {
        if (isMove && drugObject)
        {
            if (drug.GetRelease())
            {
                isMove = false;
                drugObject = null;
                drug.AutoFall();
            }
            else
            {
                var rb = drugObject.transform.GetComponent<Rigidbody>();
                Vector3 mousePositionOffset = Camera.main.ScreenToWorldPoint(new Vector3(positon.x, positon.y, selectionDistance)) - originalScreenTargetPosition;
                rb.velocity = (originalRigidbodyPos + mousePositionOffset - rb.transform.position) * forceAmount * Time.deltaTime;
            }
        }
    }

    public void TouchRelease(Vector3 positon)
    {
        isMove = false;
    }

    void GeneratDrugFromTap(Vector3 position)
    {
        Ray ray = Camera.main.ScreenPointToRay(position);
        RaycastHit hit;

        if (Physics.Raycast(ray.origin, ray.direction, out hit, Mathf.Infinity))
        {
            //UI錠剤をクリック
            if(hit.collider.gameObject.tag == "UIDrug")
            {
                //画面上にあったらマテリアル交換無かったら生成
                if (!drugObject)
                {
                    drugObject = Instantiate(drugPlefab, drugPlefab.gameObject.transform.position, hit.collider.gameObject.transform.rotation);
                    drug = drugObject.GetComponent<Drug>();
                    OnGeneratDrugListener?.Invoke(drug);
                }

                var mesh = hit.collider.gameObject.transform.GetComponent<MeshRenderer>();
                drugObject.transform.GetComponent<MeshRenderer>().material = mesh.material;
                drug.SetDrugType(hit.collider.gameObject.transform.GetComponent<UIDrug>().GetDrugType());
            }

            if (hit.collider.gameObject.tag == "Drug")
            {
                isMove = true;
                selectionDistance = Vector3.Distance(ray.origin, hit.point);
                originalScreenTargetPosition = Camera.main.ScreenToWorldPoint(new Vector3(position.x, position.y, selectionDistance));
                originalRigidbodyPos = hit.collider.transform.position;
            }
        }
    }

    public void Deactivate()
    {
        //タッチイベント登録
        touchEventHandler.OnTouchStartListener -= TouchStart;
        touchEventHandler.OnTouchKeepListener -= TouchKeep;
        touchEventHandler.OnTouchReleaseListener -= TouchRelease;
    }
}
