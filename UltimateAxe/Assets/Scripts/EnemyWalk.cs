using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class EnemyWalk : Enemy
{
    [SerializeField] bool isWalk;
    [SerializeField] float moveSpeedRate = 2.0f;
    Tween tween;
    bool turn;

    private void Start()
    {
        if (isWalk)
        {
            //tween = transform.DOMove(new Vector3(transform.position.x * -1.0f, transform.position.y - 0.25f, transform.position.z), 3.0f).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.Linear);
        }
    }

    private void Update()
    {
        if (isWalk)
        {
            float over = 0f;
            if(transform.position.x >= 3.5f  && !turn)
            {
                over = transform.position.x - 3.5f;
                turn = true;
            }

            if (transform.position.x <= -3.5f && turn)
            {
                over = transform.position.x + 3.5f;
                turn = false;
            }
            transform.Translate(turn ? ((Vector3.right * moveSpeedRate) * Time.deltaTime) + new Vector3(over,0f,0f) : ((Vector3.left * moveSpeedRate) * Time.deltaTime) + new Vector3(over, 0f, 0f));
        }
    }

    protected override void Death()
    {
        base.Death();
        isWalk = false;
        tween.Kill();
    }

    public override void Dance()
    {
        base.Dance();
        isWalk = false;
        tween.Kill();
    }
}
