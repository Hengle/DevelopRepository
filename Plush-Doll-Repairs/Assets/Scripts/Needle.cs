using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Needle : MonoBehaviour
{
    [SerializeField] float sewingSpeed = 0.1f;  //縫う速度
    [SerializeField] float tension = 0.5f;
    [SerializeField] GameObject needleModel;
    TubeRenderer yarn; //糸
    GameObject rift; //裂け目
    Vector3[] stitches = new Vector3[0];        //縫い目
    Vector3[] yarnPosition = new Vector3[0];    //ヒモ
    Vector3 beforeStitch;
    Vector3 beforePosition = Vector3.zero;

    bool isReturn = false;
    int stitchIndex = 0;
    float time = 0;

    public event Action OnSewingCompleteListener;

    void Awake()
    {
    }

    public void SetStitches(Vector3[] position)
    {
        stitches = position;
        beforeStitch = stitches[0];
        rift = GameObject.FindGameObjectWithTag("Rift");
    }

    public void SetYarn(TubeRenderer tubeRenderer)
    {
        yarn = tubeRenderer;
    }

    public void Move()
    {
        if (stitchIndex >= stitches.Length)
        {
            OnSewingCompleteListener?.Invoke();
            return;
        };

        //次の縫い針に向けて位置の更新
        var position = CalcMoveControllPoint(stitches[stitchIndex], time - stitchIndex, tension);
        transform.position = position;

        //向きを進行方向に向ける
        Vector3 diff = transform.position - beforePosition;
        transform.rotation = Quaternion.LookRotation(diff);
        beforePosition = transform.position;

        //糸のメッシュ生成
        Array.Resize(ref yarnPosition, yarnPosition.Length + 1);
        yarnPosition[yarnPosition.Length - 1] = position;
        yarn.SetPositions(yarnPosition);       

        time += sewingSpeed;

        if (time - stitchIndex >= 1.0f)
        {
            isReturn = !isReturn;
            beforeStitch = stitches[stitchIndex];
            stitchIndex++;
        }
    }

    public void Tighten()
    {
        var positions = new Vector3[yarnPosition.Length];
        DOTween.To
        (
                    () => tension,
                    (ten) => {
                        float t = 0f;
                        int index = 0;
                        isReturn = false;

                        beforeStitch = stitches[0];
                        for (int i = 0; i < positions.Length; ++i)
                        {
                            var position = CalcMoveControllPoint(stitches[index], t - index, ten);
                            positions[i] = position;

                            t += sewingSpeed;

                            if (t - index >= 1.0f)
                            {
                                isReturn = !isReturn;
                                beforeStitch = stitches[index];
                                index++;
                            }
                        }
                        
                        yarn.SetPositions(positions);
                    },
                    tension * 0.3f,
                    0.3f
        ).SetEase(Ease.Linear);
        if(rift) rift.transform.DOScaleX(0.0f, 0.3f).SetEase(Ease.Linear);
        needleModel.SetActive(false);
    }

    Vector3 CalcMoveControllPoint(Vector3 target, float t, float tension)
    {
        var controllPoint = Vector3.Lerp(beforeStitch, target, 0.5f);
        controllPoint = isReturn ? controllPoint.AddZ(-tension) : controllPoint.AddZ(tension);

        Vector3 M0 = Vector3.Lerp(beforeStitch, controllPoint, t);
        Vector3 M1 = Vector3.Lerp(controllPoint, target, t);
        return Vector3.Lerp(M0, M1, t);
    }

    //三次ベジェ曲線
    //Vector3 CalcBezier(Vector3 start, Vector3 end, Vector3 control1, Vector3 control2, float t)
    //{
    //    Vector3 M0 = Vector3.Lerp(start, control1, t);
    //    Vector3 M1 = Vector3.Lerp(control1, control2, t);
    //    Vector3 M2 = Vector3.Lerp(control2, end, t);
    //    Vector3 B0 = Vector3.Lerp(M0, M1, t);
    //    Vector3 B1 = Vector3.Lerp(M1, M2, t);
    //    return Vector3.Lerp(B0, B1, t);
    //}
}
