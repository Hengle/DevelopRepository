using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Leaf : MonoBehaviour
{
    bool isScale;
    bool isCuted;
    bool isBlowed;
    int hitCount;
    public event Action OnBlowedListener;
    public event Action OnCutedListener;

    private void Awake()
    {
        hitCount = 0;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //if(collision.gameObject.tag == "Target" || collision.gameObject.tag == "Leaves")
        //{
        //    hitCount++;
        //}

        //if (collision.gameObject.transform.parent == null) return;
        if (collision.gameObject.transform.parent.tag == "Player" && !isCuted)
        {
            isCuted = true;
            var random = UnityEngine.Random.Range(-5.0f, 1.5f);
            var rate = ((random + 5.0f) / 6.5f) /5.0f;
            transform.DOMoveZ(random, 0.5f);
            transform.DOScale(0.5f - rate, 0.5f);
            OnCutedListener?.Invoke();
            //transform.DOScale(0f,1.0f);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Ground" && !isScale)
        {
            //transform.DOMoveZ(Random.Range(-0.5f, -5.0f) ,0.5f);
            //transform.DOScale(0f,1.0f);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        //if (collision.gameObject.tag == "Target" || collision.gameObject.tag == "Leaves")
        //{
        //    hitCount--;
        //}

        //if(hitCount == 0 && !isCuted)
        //{
        //    isCuted = true;
        //    var random = UnityEngine.Random.Range(-5.0f, 1.5f);
        //    var rate = ((random + 5.0f) / 6.5f) / 5.0f;
        //    transform.DOMoveZ(random, 0.5f).OnComplete(() => GetComponent<Rigidbody2D>().gravityScale = 0f);
        //    transform.DOScale(0.5f - rate, 0.5f);
        //    GetComponent<Rigidbody2D>().gravityScale = 1f;
        //    OnCutedListener?.Invoke();
        //}
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.transform.parent.tag == "Player" && !isBlowed)
        {
            isBlowed = true;
            var rb = GetComponent<Rigidbody>();
            rb.AddForce(new Vector3(UnityEngine.Random.Range(-0.5f, 0.5f), UnityEngine.Random.Range(0.25f,0.75f), UnityEngine.Random.Range(0.5f, 1.0f)) * 15f, ForceMode.Impulse);
            rb.AddTorque(transform.forward,ForceMode.Impulse);
            OnBlowedListener?.Invoke();
        }
    }
}
