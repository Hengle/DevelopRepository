using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class CameraManager : MonoBehaviour
{
    GameObject target;
    Vector3 distance;
    bool isFocusTarget;

    private float width = 720.0f;
    private float height = 1280.0f;
    private float pixelPerUnit = 100f;

    private void Awake()
    {
        // 開発している画面を元に縦横比取得 (縦画面) ベースサイズ
        float developAspect = width / height;
        float deviceAspect = (float)Screen.width / (float)Screen.height;
        Camera mainCamera = GetComponent<Camera>();
        mainCamera.orthographicSize = height / 2.0f / pixelPerUnit;

        float bgScale = height / Screen.height;
        float camWidth = width / (Screen.width * bgScale);
        //mainCamera.rect = new Rect((1f - camWidth) / 2f, 0f, camWidth, 1f);

        if (developAspect > deviceAspect)
        {
            //画面が横に広いとき
            // 倍率
            //float bgScale = height / Screen.height;
            // viewport rectの幅
            //float camWidth = width / (Screen.width * bgScale);
            // viewportRectを設定
            //mainCamera.rect = new Rect((1f - camWidth) / 2f, 0f, camWidth, 1f);
        }
        else
        {
            //画面が縦に長い
            //想定しているアスペクト比とどれだけ差があるかを出す
            //float bgScale = deviceAspect / developAspect;

            // カメラのorthographicSizeを縦の長さに合わせて設定しなおす
            //mainCamera.orthographicSize *= bgScale;

            // viewportRectを設定
            //mainCamera.rect = new Rect(0f, 0f, 1f, 1f);
        }
    }

    public void SwitchFollowTarget(GameObject target)
    {
        this.target = target;
        distance = transform.position - this.target.transform.position;
    }

    public void SwitchTopView()
    {
        float viewPoint = Camera.main.fieldOfView;
        //Sequence seq = DOTween.Sequence();
        //seq.Append(transform.DOMove(new Vector3(transform.position.x, transform.position.y + 10.0f, transform.position.z), 1.0f));
        //seq.Join(transform.DORotate(new Vector3(45.0f, 0.0f, 0.0f), 1.0f));
        //seq.Join(DOTween.To(() => viewPoint, time => viewPoint = time, 45.0f, 1.0f).OnUpdate(() => Camera.main.fieldOfView = viewPoint));
        //seq.Play();
        //Time.timeScale = 0.5f;
    }

    public void ResetGameTime()
    {
        Time.timeScale = 1.0f;
    }

    void Update()
    {
        if (isFocusTarget) return;
        //transform.position = target.transform.position + distance;
    }
}
