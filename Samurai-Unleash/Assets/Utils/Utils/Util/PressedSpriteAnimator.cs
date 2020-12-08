using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Util {
	[RequireComponent(typeof(Collider2D))]
	public class PressedSpriteAnimator : MonoBehaviour, IPointerDownHandler, IPointerUpHandler {
		[SerializeField] float targetScale = 0.9f;

		Vector3 OriginalScale { get; set; }


		void Awake () {
			OriginalScale = transform.localScale;
		}

		public void OnPointerDown (PointerEventData eventData) {
			transform.localScale = OriginalScale * targetScale;
		}

		public void OnPointerUp (PointerEventData eventData) {
			transform.localScale = OriginalScale;
		}
	}
}
