using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 水平視野を合わせる機能
/// </summary>
public class HorizontalFOVFitter : MonoBehaviour
{
    /// <summary>カメラ</summary>
    [SerializeField]
    private Camera _camera = null;
    /// <summary>水平視野合わせ機能をOFFにする（垂直視野を合わせる）か否か</summary>
    [SerializeField]
    private bool _isDisable = false;
    /// <summary>基準にするアスペクト比（解像度での指定も可）</summary>
    [SerializeField]
    private Vector2 _baseAspect = new Vector2(750, 1334);
    /// <summary>基準にするCameraのFOV（垂直視野）</summary>
    [SerializeField]
    private float _baseFieldOfView = 60f;

    private void Update()
    {
        if (_camera == null)
        {
            return;
        }
        if (_isDisable)
        {
            // 機能を無効化しているときは、通常通り垂直視野をしてFOVを反映する
            if (_camera.fieldOfView != _baseFieldOfView)
            {
                _camera.fieldOfView = _baseFieldOfView;
            }
            return;
        }
        // 基準の垂直視野とアスペクト比から、基準にする水平視野を計算する
        float baseHorizontalFOV = CalcHorizontalFOV(_baseFieldOfView, CalcAspect(_baseAspect.x, _baseAspect.y));
        float currentAspect = CalcAspect(Screen.width, Screen.height);
        // 基準にする水平視野と現在のアスペクト比から、反映すべき垂直視野を計算する
        _camera.fieldOfView = CalcVerticalFOV(baseHorizontalFOV, currentAspect);
    }

    /// <summary>
    /// アスペクト比を計算する
    /// </summary>
    private float CalcAspect(float width, float height)
    {
        return width / height;
    }

    /// <summary>
    /// 垂直視野とアスペクト比から、水平視野を計算する
    /// </summary>
    private float CalcHorizontalFOV(float verticalFOV, float aspect)
    {
        return Mathf.Atan(Mathf.Tan(verticalFOV / 2f * Mathf.Deg2Rad) * aspect) * 2f * Mathf.Rad2Deg;
    }

    /// <summary>
    /// 水平視野とアスペクト比から、垂直視野を計算する
    /// </summary>
    private float CalcVerticalFOV(float horizontalFOV, float aspect)
    {
        return Mathf.Atan(Mathf.Tan(horizontalFOV / 2f * Mathf.Deg2Rad) / aspect) * 2f * Mathf.Rad2Deg;
    }
}
