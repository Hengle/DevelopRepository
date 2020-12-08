using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterAI : MonoBehaviour
{
    [SerializeField] LayerMask ringMask;
    [SerializeField] LayerMask rooftopLayer;
    [SerializeField] LayerMask bill;
    public bool isMove;

    LineRenderer lineRendler;
    Rigidbody myRigidBody;
    Character character;
    Vector3 target = Vector3.zero;
    GameObject player;
    float vertical, horizontal;

    void Start()
    {
        myRigidBody = GetComponent<Rigidbody>();
        lineRendler = GetComponentInChildren<LineRenderer>();
        character = GetComponentInChildren<Character>();
        player = GameObject.FindGameObjectWithTag("Player");

        StartCoroutine(SearchTarget());
        if (!isMove) character.enabled = false;
    }

    private void Update()
    {
        if (!isMove) return; 
        vertical = 0f;
        horizontal = 0f;

        AIMove();
        character.FlyMove(vertical, horizontal);
    }

    void AIMove()
    {
        //プレイヤーから一定以上離れていたらプレイヤーに寄る
        var distance = player.transform.position - transform.position;

        character.SetMaxVelocity(-0.01f);

        if (Mathf.Abs(distance.x) >= 15f)
        {
            Debug.DrawRay(transform.position, distance, Color.red);
        }
        else
        {
            Debug.DrawRay(transform.position, distance, Color.green);
        }

        //建物・リングの方へ寄る
        if (Mathf.Abs(target.x - transform.position.x) <= 1.0f)
        {
            target = Vector3.zero;
        }
        if (transform.position.z <= target.y)
        {
            target = Vector3.zero;
        }
        if (transform.position.z >= target.z)
        {
            target = Vector3.zero;
        }
        if (target != Vector3.zero)
        {
            
            if (target.x >= transform.position.x)
            {
                horizontal = 1.0f;
            }
            else
            {
                horizontal = -1.0f;
            }
        }
        else
        {
            Sensor();
        }
    }

    void FixedUpdate()
    {
        if (!character.IsDescentMode() && !character.IsGrounded())
        {
            SearchLandingPoint();
        }
    }

    IEnumerator SearchTarget()
    {
        var time = Random.Range(0f,6f);
        while (true)
        {
            if (!character.IsDescentMode() && !character.IsGrounded())
            {
                time += 0.1f;

                var velocity = myRigidBody.velocity;
                velocity.x = Mathf.Sin(time) * 10f;
                velocity *= 3.0f;
                var origin = transform.position + velocity;

                RaycastHit hit;
                Debug.DrawRay(transform.position, velocity, Color.red, 0.1f);
                Debug.DrawRay(origin, Vector3.down * 50f, Color.green, 0.1f);

                if (Physics.Raycast(transform.position, velocity.normalized, out hit, velocity.magnitude, ringMask))
                {
                    if (hit.collider.gameObject.tag == "Ring")
                    {
                        target = hit.collider.gameObject.transform.position;
                    }
                }

                if (Physics.Raycast(origin, Vector3.down, out hit, 50f, rooftopLayer))
                {
                    if (hit.collider.gameObject.tag == "LandingPoint")
                    {
                        target = hit.collider.gameObject.transform.position;
                        target.x = target.x + Random.Range(-15f,15f);
                    }
                }
            }

            yield return new WaitForSeconds(0.02f);

        }
    }

    public void Warp(Vector3 position, bool isTarget)
    {
        isMove = true;
        character.enabled = true;
        transform.position = position.AddZ(-10f);
        myRigidBody.isKinematic = false;
        myRigidBody.velocity = new Vector3(0f,0f,30f);

        if (!isTarget)
        {
            transform.position = position.AddZ(-30f);
            StartCoroutine(AltitudeAdjustment());
        }
    }

    IEnumerator AltitudeAdjustment()
    {
        yield return new WaitForSeconds(0.1f);

        myRigidBody.velocity = new Vector3(0f, 0f, 50f);

        StartCoroutine(character.SwitchDescentMode(true));

        yield return new WaitWhile(() => transform.position.y >= player.transform.position.y);

        StartCoroutine(character.SwitchDescentMode(false));
    }

    void SearchLandingPoint()
    {
        Vector3 force = character.GetDescentForce();
        var halfForce = force * 0.5f;

        RaycastHit hit;
        var origin = transform.position;
        for (int i = 0; i < 2; i++)
        {
            var futurePos = transform.position + myRigidBody.velocity * ((float)i + 1.0f) + halfForce * Mathf.Pow(((float)i + 1.0f), 2);
            Debug.DrawRay(origin, futurePos - origin, Color.green);
            if (Physics.Raycast(origin, (futurePos - origin).normalized, out hit, (futurePos - origin).magnitude, rooftopLayer))
            {
                if (hit.collider.gameObject.tag == "LandingPoint")
                {
                    if (hit.point.x <= hit.collider.bounds.center.x + hit.collider.bounds.extents.x * 0.8f 
                        && hit.point.x >= hit.collider.bounds.center.x - hit.collider.bounds.extents.x * 0.8f 
                        && hit.point.z >= hit.collider.bounds.center.z - hit.collider.bounds.extents.z * 0.5f)
                    {
                        StartCoroutine(character.SwitchDescentMode(true));
                    }
                }
                break;
            }
 
            origin = futurePos;
        }
    }

    void Sensor()
    {
        if (character.IsDescentMode() || character.IsGrounded()) return;
        float sensorLength = 3f;
        Vector3 frontSensorPosition = new Vector3(0f,0.2f,0.5f);
        float frontSideSensorPosition = 1.0f;
        float frontSensorAngle = 30f;
        float multiplier = 0f;

        var velocity = myRigidBody.velocity;
        velocity.x = 0f;
        velocity *= sensorLength;

        RaycastHit hit;
        Vector3 sensorStartPos = transform.position + frontSensorPosition;

        sensorStartPos.x += frontSideSensorPosition;
        if (Physics.Raycast(sensorStartPos, velocity.normalized, out hit, velocity.magnitude, bill))
        {
            multiplier -= 1.0f;
            Debug.DrawLine(sensorStartPos, hit.point, Color.black);
        }
        else if (Physics.Raycast(sensorStartPos, Quaternion.AngleAxis(frontSensorAngle, transform.up) * transform.forward, out hit, sensorLength * 3f, bill))
        {
            multiplier -= 0.5f;
            Debug.DrawLine(sensorStartPos, hit.point, Color.black);
        }

        sensorStartPos.x -= 2 * frontSideSensorPosition;
        if (Physics.Raycast(sensorStartPos, velocity.normalized, out hit, velocity.magnitude, bill))
        {
            multiplier += 1.0f;
            Debug.DrawLine(sensorStartPos, hit.point, Color.black);
        }
        else if (Physics.Raycast(sensorStartPos, Quaternion.AngleAxis(-frontSensorAngle, transform.up) * transform.forward, out hit, sensorLength * 3f, bill))
        {
            multiplier += 0.5f;
            Debug.DrawLine(sensorStartPos, hit.point, Color.black);
        }

        sensorStartPos.x += frontSideSensorPosition;
        if (multiplier == 0)
        {
            if (Physics.Raycast(sensorStartPos, velocity.normalized, out hit, velocity.magnitude, bill))
            {
                Debug.DrawLine(sensorStartPos, hit.point, Color.black);
                if (transform.position.x > hit.collider.gameObject.transform.position.x)
                {
                    multiplier = 1f;
                }
                else
                {
                    multiplier = -1f;
                }
            }
        }
        if (multiplier == 0f) return;
        if (multiplier >= 0f)
        {
            horizontal = 1.0f;
        }
        else
        {
            horizontal = -1.0f;
        }
    }

    public void SetGliderColor(Material material)
    {
        var glider = GetComponent<Character>().GetGlider();
        glider.transform.Find("Sail").GetComponent<MeshRenderer>().material = material;
        glider.transform.Find("Sail.001").GetComponent<MeshRenderer>().material = material;
    }
}
