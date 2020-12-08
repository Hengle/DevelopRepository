using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Util;
using DG.Tweening;

public class PlayerController : MonoBehaviour
{
    public enum ActionType
    {
        Blower,
        Gardener,
        Watering,
        Harvest
    }

    [SerializeField] ParticleSystem wateringParticle;
    [SerializeField] ParticleSystem blowerParticle;
    [SerializeField] LayerMask layerMask;
    [SerializeField] ActionType actionType;
    [SerializeField] SpriteRenderer leavesSpr;

    Wood wood;
    TouchEventHandler touchEventHandler;
    Vector3 touchStartPos = Vector3.zero;
    GameObject grabObject;
    float selectionDistance;
    Vector3 originalScreenTargetPosition;
    Vector3 originalRigidbodyPos;

    private void Awake()
    {
        touchEventHandler = Camera.main.GetComponent<TouchEventHandler>();

        if (actionType == ActionType.Harvest)
        {
            wood = GameObject.FindGameObjectWithTag("Wood").GetComponent<Wood>();
        }
    }

    void TouchStart(Vector3 position)
    {
        Ray ray;
        RaycastHit hit;
        switch (actionType)
        {
            case ActionType.Blower:
                ray = Camera.main.ScreenPointToRay(position);
                if (Physics.Raycast(ray, out hit, 100f, layerMask))
                {
                    transform.position = new Vector3(hit.point.x, 1.0f, hit.point.z);
                    blowerParticle.Play();
                }
                break;
            case ActionType.Gardener:
                ray = Camera.main.ScreenPointToRay(position);
                RaycastHit2D[] hits = Physics2D.GetRayIntersectionAll(ray, Mathf.Infinity);
                if (hits.Length >= 1)
                {

                    foreach (var hit2d in hits)
                    {
                        if (hit2d.collider.gameObject.tag == "Leaves") return;
                    }
                }
                transform.position = Camera.main.ScreenToWorldPoint(position.SetZ(9.0f));
                break;

            case ActionType.Watering:
                wateringParticle.Play();
                ray = Camera.main.ScreenPointToRay(position);
                if (Physics.Raycast(ray, out hit, 100f, layerMask))
                {
                    transform.position = new Vector3(hit.point.x, transform.position.y, hit.point.z);
                }
                break;

            case ActionType.Harvest:
                touchStartPos = position;
                ray = Camera.main.ScreenPointToRay(position);
                if (Physics.Raycast(ray, out hit, 100f, layerMask))
                {
                    if (hit.transform.GetComponent<Fruit>().IsHarvested()) return;
                    grabObject = hit.transform.gameObject;
                    selectionDistance =  grabObject.transform.position.z - Camera.main.transform.position.z;
                    originalScreenTargetPosition = Camera.main.ScreenToWorldPoint(new Vector3(position.x, position.y, selectionDistance));
                    originalRigidbodyPos = hit.collider.transform.position;
                }
                break;
        }
    }

    void TouchKeep(Vector3 position)
    {
        Ray ray;
        RaycastHit hit;
        switch (actionType)
        {
            case ActionType.Blower:
                ray = Camera.main.ScreenPointToRay(position);
                if (Physics.Raycast(ray, out hit, 100f, layerMask))
                {
                    transform.position = new Vector3(hit.point.x, 1.0f, hit.point.z);
                }
                break;
            case ActionType.Gardener:
                //差で移動はストップ
                //var distance = position - beforPos;
                //distance.x = distance.x / Screen.width * 10f;
                //distance.y = distance.y / Screen.height * 10f;
                //distance.z = 0f;
                //transform.position += distance;
                //beforPos = position;

                ray = Camera.main.ScreenPointToRay(position);
                RaycastHit2D[] hits = Physics2D.GetRayIntersectionAll(ray, Mathf.Infinity);
                if (hits.Length >= 1)
                {

                    foreach (var hit2d in hits)
                    {
                        if (hit2d.collider.gameObject.tag == "Leaves")
                        {
                            //DOTween.To(() => 1.0f, (g) => leavesSpr.color = new Color(1.0f, g, 1.0f), 0f, 0.5f).SetEase(Ease.Linear);
                            hit2d.collider.GetComponent<Leaves>().TouchLeaves();
                            return;
                        };
                    }
                }

                transform.position = Camera.main.ScreenToWorldPoint(position.SetZ(9.0f));
                break;

            case ActionType.Watering:
                ray = Camera.main.ScreenPointToRay(position);
                if (Physics.Raycast(ray, out hit, 100f, layerMask))
                {
                    transform.position = new Vector3(hit.point.x, transform.position.y, hit.point.z);
                }
                break;

            case ActionType.Harvest:
                if (grabObject)
                {
                    //grabObject.transform.position = Camera.main.ScreenToWorldPoint(new Vector3(position.x, position.y, selectionDistance)).SetZ(0f);
                    var rb = grabObject.transform.GetComponent<Rigidbody>();
                    Vector3 mousePositionOffset = Camera.main.ScreenToWorldPoint(new Vector3(position.x, position.y, selectionDistance)) - originalScreenTargetPosition;
                    rb.velocity = (originalRigidbodyPos + mousePositionOffset - rb.transform.position) * 500f * Time.deltaTime;
                }
                else
                {
                    var distance = position - touchStartPos;
                    distance.x = distance.x / Screen.width * 10f;
                    distance.z = distance.y / Screen.height * 10f;
                    distance.y = 0f;
                    var length = distance.sqrMagnitude * Mathf.Sign(distance.x);
                    wood.Shake(length * 0.5f);
                }
                break;
        }
    }

    void TouchRelease(Vector3 position)
    {

        switch (actionType)
        {
            case ActionType.Blower:
                blowerParticle.Stop();
                break;
            case ActionType.Watering:
                wateringParticle.Stop();
                break;

            case ActionType.Harvest:
                grabObject = null;
                wood.ShakedAnimation();
                break;
        }
    }

    public void Activate()
    {
        touchEventHandler.OnTouchStartListener += TouchStart;
        touchEventHandler.OnTouchKeepListener += TouchKeep;
        touchEventHandler.OnTouchReleaseListener += TouchRelease;
    }

    public void Deactivate()
    {
        touchEventHandler.OnTouchStartListener -= TouchStart;
        touchEventHandler.OnTouchKeepListener -= TouchKeep;
        touchEventHandler.OnTouchReleaseListener -= TouchRelease;
    }
}
