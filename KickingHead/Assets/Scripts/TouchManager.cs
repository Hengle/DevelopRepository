using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class TouchManager : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public event Action<Vector2, float> OnBeginDragListener;
    public event Action<Vector2,float> OnDragListener;
    public event Action<Vector2, float> OnEndDragListener;

    bool IsDragging { get; set; }
    float DragTime { get; set; }
    Vector3 touchPosition { get; set; }

    void Update()
    {
        DragTime += Time.deltaTime;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        //touchPosition = Camera.main.ScreenToWorldPoint(eventData.position);
        DragTime = 0;
        IsDragging = true;
        OnBeginDragListener?.Invoke(eventData.position, DragTime);
    }

    public void OnDrag(PointerEventData eventData)
    {
        //touchPosition = Camera.main.ScreenToWorldPoint(eventData.position);
        OnDragListener?.Invoke(eventData.position, DragTime);

    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (IsDragging)
        {
            //touchPosition = Camera.main.ScreenToWorldPoint(eventData.position);
            OnEndDragListener?.Invoke(eventData.position, DragTime);
            IsDragging = false;
        }
    }

}
