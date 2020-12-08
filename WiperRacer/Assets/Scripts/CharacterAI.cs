using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterAI : MonoBehaviour
{
    [SerializeField] [Range(0f,180f)] float StopAngle;
    [SerializeField] float aiValue = 5;
    Character character;
    bool isSwitch;
    float random;
    float prevAngleY;
    float rollDegree = 0;
    float angle;

    private void Awake()
    {
        character = GetComponent<Character>();
        Random();
        prevAngleY = transform.eulerAngles.y;
    }

    void Update()
    {
        CheckRotate();
    }

    void CheckRotate()
    {
        var from = character.GetAxisObject().transform.position;
        var to = character.GetPairObject().transform.position;
        float angleY = transform.eulerAngles.y; // y軸の回転量
        //angleY = angleY <= 180f ? angleY : (360 - angleY);
        if (IsRange(Mathf.Atan2(to.x - from.x, to.z - from.z) * Mathf.Rad2Deg, character.IsReverse() ? -StopAngle : random, character.IsReverse() ? random : StopAngle))
        {
            if (character.isPairGround() || UnityEngine.Random.Range(0,11) > aiValue)
            {
                character.AxisSwitching();
            }

            Random();
        }


        //回転カウント（仮）
        if (transform.eulerAngles.y - prevAngleY >= 0f)
        {
            rollDegree = rollDegree + (transform.eulerAngles.y - prevAngleY);

        }
        else
        {
            Debug.Log("回転");

        }
        prevAngleY = transform.eulerAngles.y;
    }

    void Random()
    {
        random = UnityEngine.Random.Range(-StopAngle, StopAngle);
        Debug.Log(random);
    }

    bool IsRange(float value, float min, float max)
    {
        return (min <= value && value <= max);
    }
}
