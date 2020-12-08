using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class Touch : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public event Action<Vector2, float> OnBeginDragListener;
    public event Action<Vector2,float> OnDragListener;
    public event Action<Vector2, float> OnEndDragListener;

    bool IsDragging { get; set; }
    float DragTime { get; set; }
    Vector2 touchPosition { get; set; }

    void Update()
    {
        DragTime += Time.deltaTime;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        touchPosition = Camera.main.ScreenToWorldPoint(eventData.position);
        DragTime = 0;
        IsDragging = true;
        OnBeginDragListener?.Invoke(touchPosition, DragTime);
    }

    public void OnDrag(PointerEventData eventData)
    {
        //if (IsDragging) return;
        touchPosition = Camera.main.ScreenToWorldPoint(eventData.position);
        OnDragListener?.Invoke(touchPosition, DragTime);

    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (IsDragging)
        {
            touchPosition = Camera.main.ScreenToWorldPoint(eventData.position);
            OnEndDragListener?.Invoke(touchPosition,DragTime);
            IsDragging = false;
        }
    }

}
