using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TargetMarker : MonoBehaviour
{
    [SerializeField] GameObject target;
    [SerializeField] Transform cursor;
    [SerializeField] Image maker;

    HandController hand;
    Camera mainCamera;
    RectTransform rectTransform;
    bool isOverlap;
    float distance = 20f;
    float maxSize = 250f;

    void Start()
    {
        mainCamera = Camera.main;
        rectTransform = GetComponent<RectTransform>();
    }

    private void LateUpdate()
    {
        if (target != null && hand != null)
        {
            float canvasScale = 1f;
            var center = 0.5f * new Vector3(Screen.width, Screen.height);
            Vector3 targetPos = target.transform.position;
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

            var nowDistance = cursor.position.z - target.transform.position.z - 10f;

            if(nowDistance < -5f)
            {
                maker.enabled = false;
            }else
            {
                nowDistance = Mathf.Max(nowDistance, 0f);
                var size = Mathf.Min(maxSize, maxSize * (1f - (nowDistance / distance)));
                maker.rectTransform.sizeDelta = new Vector2(Mathf.Min(maxSize, size), Mathf.Min(maxSize, size));

                Vector2 screenPos = RectTransformUtility.WorldToScreenPoint(Camera.main, rectTransform.position);
                if (IsInsideOfCircle(screenPos, hand.GetCursorScreenPosition(), 100f) && maxSize == size)
                {
                    isOverlap = true;
                    maker.color = Color.red;
                }
                else
                {
                    isOverlap = false;
                    maker.color = Color.white;
                }
            }
            rectTransform.anchoredPosition = pos / canvasScale;


            bool isOffscreen = (pos.z > 10f && d < 1f);
            if (isOffscreen)
            {
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
                //if (arrow.enabled)
                //{
                //    arrow.enabled = false;
                //}
            }
        }
    }

    public bool IsInsideOfCircle(Vector2 target, float radius)
    {
        if (Mathf.Pow(target.x, 2) + Mathf.Pow(target.y, 2) <= Mathf.Pow(radius, 2))
        {
            return true;
        }
        return false;
    }
    public bool IsInsideOfCircle(Vector2 target, Vector2 center, float radius)
    {
        var diff = target - center;
        return IsInsideOfCircle(diff, radius);
    }

    public void SetHand(HandController handController)
    {
        hand = handController;
    }

    public bool IsOverlap()
    {
       return isOverlap;
    }
}
