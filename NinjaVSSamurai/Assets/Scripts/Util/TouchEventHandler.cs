using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Util {
	public class TouchEventHandler : MonoBehaviour {
		bool IsPressing { get; set; }
		bool IsTouchable { get; set; } = true;

		public event Action<Vector2> OnTouchStartListener;
		public event Action<Vector2> OnTouchKeepListener;
		public event Action<Vector2> OnTouchReleaseListener;

		Vector2 TouchingWorldPos { get; set; }


		void Update () {
			if (!IsTouchable) return;

			if (Input.GetMouseButtonDown(0) && !IsPressing) {
				TouchingWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
				OnTouchStartListener?.Invoke(TouchingWorldPos);
				IsPressing = true;
			}

			if (Input.GetMouseButton(0) && IsPressing) {
				TouchingWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
				OnTouchKeepListener?.Invoke(TouchingWorldPos);
			}

			if (Input.GetMouseButtonUp(0) && IsPressing) {
				TouchingWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
				IsPressing = false;
				OnTouchReleaseListener?.Invoke(TouchingWorldPos);
			}
		}

		public void ChangeTouchable (bool isTouchable) {
			IsTouchable = isTouchable;
		}

	}
}
