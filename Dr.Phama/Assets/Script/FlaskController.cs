using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Util;

public class FlaskController : MonoBehaviour
{
    [SerializeField] ParticleSystem particle;
    [SerializeField] Material flaskLiquidMaterial;
    [SerializeField] Material jarLiquidMaterial;
    [SerializeField] Vector3 capOrigin;
    [SerializeField] GameObject cap;
    [SerializeField] GameObject jar;
    [SerializeField] MultipleParticles particles;
    [SerializeField] GameObject borderUnderLineObj;
    [SerializeField] GameObject borderTopLineObj;

    Sequence startTween;
    Sequence endtween;
    TouchEventHandler touchEventHandler;
    float flaskAmount;
    float jarAmount;
    float jarMaxAmount = -1.5f;
    bool isPour;

    float borderUnderLine = -0.6f;
    float borderTopLine = -1.15f;

    void Awake()
    {
        startTween = DOTween.Sequence()
            .Append(transform.DOMove(new Vector3(transform.position.x, 4.0f, transform.position.z), 0.5f).SetEase(Ease.Linear))
            .Append(transform.DORotate(new Vector3(0.0f, 0.0f, -120.0f), 0.5f, RotateMode.FastBeyond360))
            .OnComplete(PourStart)
            .Pause();

        endtween = DOTween.Sequence()
            .OnStart(PourEnd)
            .Append(transform.DORotate(new Vector3(0.0f, 0.0f, 0.0f), 0.5f, RotateMode.Fast))
            .Append(transform.DOMove(new Vector3(transform.position.x, 0.0f, transform.position.z), 0.5f).SetEase(Ease.Linear))
            .OnComplete(ClearCheck)
            .Pause();

        touchEventHandler = Camera.main.transform.GetComponent<TouchEventHandler>();
        touchEventHandler.OnTouchStartListener += TouchStart;
        touchEventHandler.OnTouchKeepListener += TouchKeep;
        touchEventHandler.OnTouchReleaseListener += TouchRelease;

        flaskLiquidMaterial.SetFloat("_FillAmount", -1.5f);
        jarLiquidMaterial.SetFloat("_FillAmount", 0.5f);
        flaskAmount = flaskLiquidMaterial.GetFloat("_FillAmount");
        jarAmount = jarLiquidMaterial.GetFloat("_FillAmount");

    }

    void PourStart()
    {
        particle.Play();
        isPour = true;
    }

    void PourEnd()
    {
        particle.Stop();
        isPour = false;
    }

    void ClearCheck()
    {
        if (borderUnderLine >= jarAmount && jarAmount >= borderTopLine)
        {
            Debug.Log("成功");
        }
        else
        {
            Debug.Log("失敗");
        }

        StartCoroutine(ShowResult());
    }

    IEnumerator ShowResult()
    {
        borderUnderLineObj.SetActive(false);
        borderTopLineObj.SetActive(false);
        yield return new WaitForSeconds(0.5f);

        cap.transform.DOLocalPath(new Vector3[] {
            new Vector3(cap.transform.localPosition.x, cap.transform.localPosition.y + 0.4f,cap.transform.localPosition.z),
            new Vector3(cap.transform.localPosition.x - 0.2f, cap.transform.localPosition.y + 0.7f,cap.transform.localPosition.z),
            capOrigin
        }, 2.0f, PathType.CatmullRom);

        yield return new WaitForSeconds(2.0f);

        jar.transform.DOMove(new Vector3(jar.transform.position.x, 0f, 0f), 0.5f);

        yield return new WaitForSeconds(0.5f);

        jar.transform.DOScale(new Vector3(6.0f, 6.0f, 6.0f), 0.5f).SetLoops(2,LoopType.Yoyo);

        yield return new WaitForSeconds(1.0f);

        jar.transform.DORotate(new Vector3(360.0f, 0.0f, 0.0f), 0.5f,RotateMode.FastBeyond360);
        particles.Play();
    }

    void TouchStart(Vector3 position)
    {
        startTween.Play();
        endtween.Pause();
    }

    void TouchKeep(Vector3 position)
    {
        if (isPour)
        {
            flaskAmount += 0.03f;
            jarAmount -= 0.02f;
            flaskLiquidMaterial.SetFloat("_FillAmount", flaskAmount);
            jarLiquidMaterial.SetFloat("_FillAmount", jarAmount);

            if(jarAmount <= jarMaxAmount)
            {
                PourEnd();
            }
        }
    }

    void TouchRelease(Vector3 position)
    {
        startTween.Pause();
        endtween.Play();
    }
}
