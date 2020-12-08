using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bound : MonoBehaviour
{
    [SerializeField] Rigidbody2D rb;
    [SerializeField] GameObject smokeEffect;

    private Vector2 velocity;
    public enum Dire
    {
        Left,
        Right
    }
    private void Start()
    {
        velocity = rb.velocity;
    }

    public void FirstMove(Dire dire)
    {
        // 今回はX軸から45度の向きに射出するため、XとYを1:1にする
        Vector2 forceDirection;
        if (dire == Dire.Left)
        {
            forceDirection = new Vector2(-1.0f, 1.0f);
        }
        else
        {
            forceDirection = new Vector2(1.0f, 1.0f);
        }

        // 上の向きに加わる力の大きさを定義
        float forceMagnitude = 3f;

        // 向きと大きさからSphereに加わる力を計算する
        Vector2 force = forceMagnitude * forceDirection;

        rb.AddForce(force, ForceMode2D.Impulse);
    }

    //地面に当たったら同じ力で打ち返し一定の高さでバウンドする
    private void OnCollisionEnter2D(Collision2D collision)
    {
        switch (collision.gameObject.tag)
        {
            case "Ground":

                if (velocity.x < 0)
                {
                    rb.AddForce(Vector2.up * 10f + Vector2.left * 1f, ForceMode2D.Impulse);
                }
                else
                {
                    rb.AddForce(Vector2.up * 10f + Vector2.right * 1f, ForceMode2D.Impulse);
                }
                Instantiate(smokeEffect, transform.position, transform.rotation);
                StartCoroutine(CameraShake(0.25f, 0.1f));
                break;
            default:
                break;
        }
        velocity = rb.velocity;
    }

    IEnumerator CameraShake(float duration, float magnitude)
    {
        var pos = Camera.main.gameObject.transform.localPosition;

        var elapsed = 0f;

        while (elapsed < duration)
        {
            var x = pos.x + Random.Range(-1f, 1f) * magnitude;
            var y = pos.y + Random.Range(-1f, 1f) * magnitude;

            Camera.main.gameObject.transform.localPosition = new Vector3(x, y, pos.z);

            elapsed += Time.deltaTime;

            yield return null;
        }

        Camera.main.gameObject.transform.localPosition = pos;
    }
}
