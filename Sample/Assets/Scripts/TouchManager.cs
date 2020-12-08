using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchManager : MonoBehaviour // SingletonMonoBehaviour<TouchManager>
{
    public TouchPhase phase;
    public Vector2 position;
    public Vector2 touchStartPos;
    public Vector2 touchEndPos;
    public float time;
    public bool IsTouch { get; private set; }

    private void Update()
    {
        this.IsTouch = false;

        if (Application.isEditor)
        {

            if (Input.GetMouseButtonDown(0))
            {
                this.IsTouch = true;
                this.phase = TouchPhase.Began;
                this.touchStartPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                this.time = 0;
            }

            if (Input.GetMouseButtonUp(0))
            {
                this.IsTouch = true;
                this.phase = TouchPhase.Ended;
                this.touchEndPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            }

            if (Input.GetMouseButton(0))
            {
                this.IsTouch = true;
                this.phase = TouchPhase.Moved;
                this.time += Time.deltaTime;
            }

            if (this.IsTouch) this.position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        }
        else if (Application.isMobilePlatform)
        {
            if (Input.touchCount > 0)
            {
                UnityEngine.Touch touch = Input.GetTouch(0);
                this.IsTouch = true;
                this.position = touch.position;
                this.phase = touch.phase;

            }
        }
    }

   // public TouchManager getTouch()
    //{
        //return TouchManager.Instance;
    //}
}
