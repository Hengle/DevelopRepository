using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Util {
	[RequireComponent(typeof(Collider2D))]
	public class DragRotator : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler {
		const float LimitDegree = 15;

		Collider2D MyCollider { get; set; }
		Transform RotateTarget { get; set; }
		Vector2 TouchDownPosision { get; set; }
		Quaternion TouchDownRotation { get; set; }
		bool IsDragging { get; set; }

		public event Action OnPointerDownListener;
		public event Action<bool> OnPointerUpListener; // IsDrag


		void Awake () {
			MyCollider = GetComponent<Collider2D>();
			RotateTarget = transform.parent;
		}

		public void SetEnabled (bool isEnabled) {
			MyCollider.enabled = isEnabled;
		}

		public void OnPointerDown (PointerEventData eventData) {
			TouchDownPosision = Camera.main.ScreenToWorldPoint(eventData.position);
			TouchDownRotation = transform.parent.rotation;
			OnPointerDownListener?.Invoke();
		}

		public void OnPointerUp (PointerEventData eventData) {
			OnPointerUpListener?.Invoke(IsDragging);
			IsDragging = false;
		}

		public void OnDrag (PointerEventData eventData) {
			IsDragging = true;

			var firstTouchDiff = TouchDownPosision - (Vector2)RotateTarget.position;
			var nowTouchDiff = (Vector2)(Camera.main.ScreenToWorldPoint(eventData.position) - RotateTarget.position);

			var diffAngle = Vector2.Angle(firstTouchDiff, nowTouchDiff);
			var cross = Vector3.Cross(firstTouchDiff, nowTouchDiff).z;

			if (diffAngle / LimitDegree >= 1) {
				diffAngle = LimitDegree * Mathf.Floor(diffAngle / LimitDegree);
				if (cross <= 0) diffAngle = -diffAngle;

				RotateTarget.localRotation = TouchDownRotation * Quaternion.Euler(0, 0, diffAngle);
				TouchDownPosision = Camera.main.ScreenToWorldPoint(eventData.position);
				TouchDownRotation = transform.parent.rotation;
			}
		}

	}
}
