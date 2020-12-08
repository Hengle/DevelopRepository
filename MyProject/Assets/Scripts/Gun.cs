using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
    [SerializeField] Transform gunTip, character;

    LineRenderer lineRenderer;
    Vector3 grapPoint;
    LayerMask layerMask;
    SpringJoint springJoint;
    float maxDistance = 100f;

    private void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            StartGrap();
        }

        if (Input.GetMouseButtonUp(0))
        {
            StopGrap();
        }
    }

    private void LateUpdate()
    {
        DrawRope();
    }

    void StartGrap()
    {
        RaycastHit hit;
        //if (Physics.Raycast(Camera.main.transform.position, transform.forward, out hit, maxDistance, layerMask))
        {
            //grapPoint = hit.point;
            grapPoint = new Vector3(0f, 10f, 20f);
            springJoint = character.gameObject.AddComponent<SpringJoint>();
            springJoint.autoConfigureConnectedAnchor = false;
            springJoint.connectedAnchor = grapPoint;

            float distanceFromPoint = Vector3.Distance(character.position, grapPoint);

            springJoint.maxDistance = distanceFromPoint * 0.4f;
            springJoint.minDistance = distanceFromPoint * 0.25f;

            springJoint.spring = 4.5f;
            springJoint.damper = 7f;
            springJoint.massScale = 4.5f;

            lineRenderer.positionCount = 2;
        }
    }

    void StopGrap()
    {
        lineRenderer.positionCount = 0;
        Destroy(springJoint);
    }

    void DrawRope()
    {
        if (!springJoint) return;
        lineRenderer.SetPosition(0, gunTip.position);
        lineRenderer.SetPosition(1, grapPoint);
    }

    public bool IsGrappling()
    {
        return springJoint != null;
    }

    public Vector3 GetGrappPoint()
    {
        return grapPoint;
    }
}
