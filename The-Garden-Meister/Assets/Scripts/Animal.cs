using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Animal : MonoBehaviour
{
    private string[] animatione = new string[] { "Idle", "Run", "Dead" };

    bool isMove;
    bool isActive;
    const float MOVE_RANGE_X = 4.5f;
    const float MOVE_RANGE_Y = 15.0f;
    private Animator myAnimator;
    Tweener tweener;

    void Awake()
    {
        isActive = true;
        myAnimator = GetComponent<Animator>();
        StartCoroutine(RandomWalk());
    }

    IEnumerator RandomWalk()
    {
        
        while (isActive)
        {
            var posX = Random.Range(MOVE_RANGE_X * -1f, MOVE_RANGE_X);
            var posY = Random.Range(MOVE_RANGE_Y * -1f, MOVE_RANGE_Y);

            isMove = true;
            
            //移動先に角度調整
            Vector3 diff = transform.position - new Vector3(posX, posY, transform.position.z);
            Vector2 vec = new Vector2(diff.x, diff.y).normalized;
            float rot = Mathf.Atan2(vec.y, vec.x) * 180 / Mathf.PI;
            if (rot > 180) rot -= 360;
            if (rot < -180) rot += 360;
            transform.rotation = Quaternion.Euler(rot, -90.0f, 90);

            //アニメーション変更
            myAnimator.SetInteger("AnimIndex", 1);
            myAnimator.SetTrigger("Next");

            var disctance = Vector3.Distance(transform.position, new Vector3(posX, posY, transform.position.z));
            float moveTime = disctance / 5.0f;

            tweener = transform.DOMove(new Vector3(posX, posY, transform.position.z), moveTime)
                .SetEase(Ease.Linear)
                .OnComplete(() => isMove = false);
            yield return new WaitWhile(() => isMove);

            myAnimator.SetInteger("AnimIndex", 0);
            myAnimator.SetTrigger("Next");
            yield return new WaitForSeconds(2.0f);
        }
    }

    private void OnParticleCollision(GameObject other)
    {
        if (other.tag == "Player")
        {
            isActive = false;
            isMove = false;
            tweener.Kill();
        }
    }
}
