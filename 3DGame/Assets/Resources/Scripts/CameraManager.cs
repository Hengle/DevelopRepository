using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    private float width = 720.0f;
    private float height = 1280.0f;
    private float pixelPerUnit = 100f;

    public GameObject player;
    private Vector3 offset;

    private void Awake()
    {
        // 開発している画面を元に縦横比取得 (縦画面) ベースサイズ
        float developAspect = width / height;
        float deviceAspect = (float)Screen.width / (float)Screen.height;
        Camera mainCamera = GetComponent<Camera>();
        mainCamera.orthographicSize = height / 2.0f / pixelPerUnit;

        float bgScale = height / Screen.height;
        float camWidth = width / (Screen.width * bgScale);
        mainCamera.rect = new Rect((1f - camWidth) / 2f, 0f, camWidth, 1f);

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

        offset = transform.position - player.transform.position;
    }

    // 各フレームで、Update の後に LateUpdate が呼び出されます。
    void LateUpdate()
    {
        //カメラの transform 位置をプレイヤーのものと等しく設定します。ただし、計算されたオフセット距離によるずれも加えます。
        transform.position = player.transform.position + offset;
    }
}
