using UnityEngine;
using UnityEngine.EventSystems;

namespace Util {
	[RequireComponent(typeof(Collider2D))]
	public class TouchDownRotator : MonoBehaviour, IPointerDownHandler {
		[SerializeField] float degree = 45.0f;
		[SerializeField] bool isClockwise = true;

		Transform RotateTarget { get; set; }
		Vector2 TouchDownPosision { get; set; }
		Quaternion TouchDownRotation { get; set; }


		void Awake () {
			RotateTarget = transform.parent;
		}

		public void OnPointerDown (PointerEventData eventData) {
			var deg = isClockwise ? -degree : degree;
			RotateTarget.Rotate(new Vector3(0, 0, deg));
		}
	}
}
