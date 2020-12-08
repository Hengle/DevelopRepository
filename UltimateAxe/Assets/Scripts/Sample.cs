using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sample : MonoBehaviour
{
    LineRenderer line;
    [SerializeField] Vector3 controllPoint1;
    [SerializeField] Vector3 controllPoint2;
    [SerializeField] Vector3 startPos1;
    [SerializeField] Vector3 startPos2;
    [SerializeField] GameObject controllPoint;

    [SerializeField] Vector3 endPosition = new Vector3(0.0f, 2.0f, 0.0f);

    void Start()
    {
        line = GetComponent<LineRenderer>();
        line.positionCount = 15;

        controllPoint1 = Vector3.Lerp(transform.position, endPosition, 0.15f);
        controllPoint2 = Vector3.Lerp(transform.position, endPosition, 0.85f);
        startPos1 = controllPoint1;
        startPos2 = controllPoint2;


        //Vector3 dif = endPosition - transform.position;
        //float radian = Mathf.Atan2(dif.x, dif.z);
        //float degree = radian * Mathf.Rad2Deg;
        //Quaternion quaternion = Quaternion.Euler(new Vector3(0.0f, degree, 0.0f));
        //controllPoint = quaternion * Vector3.right;


        //float a = (endPosition.x - transform.position.x) / (endPosition.z - transform.position.z);
        //float b = transform.position.x - a * transform.position.z;

        //y = ax + b
        //ax + by + c = 0;
        //y = 2x + 1は 2x - y + 1にできる
        //法線の傾きは a = -1/a(上記で求めた傾き)
        float a = (endPosition.x - transform.position.x) / (endPosition.z - transform.position.z);
        float aa = -1 / a;
        Debug.Log(aa * controllPoint1.z - controllPoint1.x);
        Debug.Log(aa * controllPoint1.z + (controllPoint1.x - aa * controllPoint1.z));
        Draw();
    }
    private void Update()
    {

        float a = (endPosition.x - transform.position.x) / (endPosition.z - transform.position.z);
        float aa = -1 / a;
        float b = aa * controllPoint1.z - controllPoint1.x;

        controllPoint1 = controllPoint1.SetZ(startPos1.z + controllPoint.transform.position.z);
        controllPoint1 = controllPoint1.SetX(aa * controllPoint1.z + (controllPoint1.x - aa * controllPoint1.z));
        controllPoint2 = controllPoint2.SetZ(startPos2.z + controllPoint.transform.position.z);
        controllPoint2 = controllPoint2.SetX(aa * controllPoint2.z + (controllPoint2.x - aa * controllPoint2.z ));
        Draw();
    }
    private void Draw()
    {
        for (int i = 0; i < line.positionCount; i++)
        {
            line.SetPosition(i, SampleCurve(transform.position, endPosition, controllPoint1, controllPoint2, (float)i / (float)line.positionCount));
        }
        line.positionCount = 16;
        line.SetPosition(15, SampleCurve(transform.position, endPosition, controllPoint1, controllPoint2, 1.0f));
    }

    public Vector3 SampleCurve(Vector3 start, Vector3 end, Vector3 control1, Vector3 control2, float t)
    {
        Vector3 M0 = Vector3.Lerp(start, control1, t);
        Vector3 M1 = Vector3.Lerp(control1, control2, t);
        Vector3 M2 = Vector3.Lerp(control2, end, t);
        Vector3 B0 = Vector3.Lerp(M0, M1, t);
        Vector3 B1 = Vector3.Lerp(M1, M2, t);
        return Vector3.Lerp(B0, B1, t);
    }
}
