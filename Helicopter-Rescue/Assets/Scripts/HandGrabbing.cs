using UnityEngine;

public class HandGrabbing : MonoBehaviour
{

    [SerializeField] HandGrabbing OtherHandReference;
    [SerializeField] Vector3 ObjectGrabOffset;
    [SerializeField] float GrabDistance = 0.1f;
    [SerializeField] string GrabTag = "Grab";
    [SerializeField] float ThrowMultiplier=1.5f;
    [SerializeField] Transform LookTarget;
    
    private Transform currentGrabObject;
    private Vector3 beforePosition, offset;
    private bool isGrabbing;

    public Transform CurrentGrabObject
    {
        get { return currentGrabObject; }
        set { currentGrabObject = value; }
    }

    void Start()
    {
        beforePosition = transform.position;
        offset = transform.localPosition;
        currentGrabObject = null;
        isGrabbing = false;
    }

    void Update()
    {
        //transform.LookAt(LookTarget);
        transform.localPosition = offset + LookTarget.localPosition;
        //update hand position and rotation
        //transform.localPosition = InputTracking.GetLocalPosition(NodeType);
        //transform.localRotation = InputTracking.GetLocalRotation(NodeType);
    }

    void Grab()
    {
        if (currentGrabObject == null)
        {
            Collider[] colliders = Physics.OverlapSphere(transform.position, GrabDistance);
            if (colliders.Length > 0)
            {
                if (Input.GetMouseButtonDown(0) && colliders[0].transform.CompareTag(GrabTag))
                {
                    if (isGrabbing)
                    {
                        return;
                    }
                    isGrabbing = true;

                    colliders[0].transform.SetParent(transform);

                    if (colliders[0].GetComponent<Rigidbody>() == null)
                    {
                        colliders[0].gameObject.AddComponent<Rigidbody>();
                    }

                    colliders[0].GetComponent<Rigidbody>().isKinematic = true;


                    currentGrabObject = colliders[0].transform;

                    if (OtherHandReference.CurrentGrabObject != null)
                    {
                        OtherHandReference.CurrentGrabObject = null;
                    }



                }
            }
        }
        else
        {

            if (Input.GetMouseButtonUp(0))
            {


                Rigidbody rb = currentGrabObject.GetComponent<Rigidbody>();
                rb.isKinematic = false;
                rb.collisionDetectionMode = CollisionDetectionMode.Continuous;

                Vector3 CurrentVelocity = (transform.position - beforePosition) / Time.deltaTime;
                rb.velocity = CurrentVelocity * ThrowMultiplier;
                currentGrabObject.SetParent(null);
                currentGrabObject = null;
            }

        }

        if (Input.GetMouseButtonUp(0) && isGrabbing)
        {
            isGrabbing = false;
        }

        beforePosition = transform.position;
    }
}
