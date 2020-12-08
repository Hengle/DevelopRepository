using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TargetMarker : MonoBehaviour
{
    [SerializeField] GameObject target;
    [SerializeField] TextMeshProUGUI text;
    [SerializeField] Image arrow;
    [SerializeField] bool isPositionHold;

    Camera mainCamera;
    RectTransform rectTransform;

    void Start()
    {
        mainCamera = Camera.main;
        rectTransform = GetComponent<RectTransform>();
    }

    private void LateUpdate()
    {
        if (target != null && !isPositionHold)
        {
            float canvasScale = 1f;
            var center = 0.5f * new Vector3(Screen.width, Screen.height);
            Vector3 targetPos = target.transform.position;
            targetPos.y += 3f;
            var pos = mainCamera.WorldToScreenPoint(targetPos) - center;
            if (pos.z < 0f)
            {
                pos.x = -pos.x;
                pos.y = -pos.y;

                if (Mathf.Approximately(pos.y, 0f))
                {
                    pos.y = -center.y;
                }
            }

            var halfSize = 0.5f * canvasScale * rectTransform.sizeDelta;
            float d = Mathf.Max(
                Mathf.Abs(pos.x / (center.x - halfSize.x)),
                Mathf.Abs(pos.y / (center.y - halfSize.y))
            );

            bool isOffscreen = (pos.z > 10f && d < 1f);
            if (isOffscreen)
            {
                if (!text.enabled)
                {
                    text.enabled = true;
                }
                //pos.x /= d;
                //pos.y /= d;

                //arrow.rectTransform.eulerAngles = new Vector3(0f, 0f, Mathf.Atan2(pos.y, pos.x) * Mathf.Rad2Deg);

                //if (!arrow.enabled)
                //{
                //    arrow.enabled = true;
                //}
            }
            else
            {
                if (text.enabled)
                {
                    text.enabled = false;
                }
                //if (arrow.enabled)
                //{
                //    arrow.enabled = false;
                //}
            }
            rectTransform.anchoredPosition = pos / canvasScale;
        }
    }

    public float GetZPosition()
    {
        return target.transform.position.z;
    }

    public void SetText(string text)
    {
        this.text.text = text;
    }

    public void SetCrown(bool value)
    {
        if (!isPositionHold)
        {
            text.transform.gameObject.SetActive(!value);
        }
        target.transform.GetComponent<Character>().SetCrown(value);
    }
}
