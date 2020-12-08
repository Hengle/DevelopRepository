using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace Util {
	[RequireComponent(typeof(Button))]
	public class PressedButtonAnimator : MonoBehaviour, IPointerDownHandler, IPointerUpHandler {
		[SerializeField] float targetScale = 0.9f;

		Button MyButton { get; set; }
		Vector3 OriginalScale { get; set; }


		void Awake () {
			MyButton = GetComponent<Button>();
			OriginalScale = transform.localScale;
		}

		public void OnPointerDown (PointerEventData eventData) {
			if (MyButton.enabled && MyButton.interactable) {
				transform.localScale = OriginalScale * targetScale;
			}
		}

		public void OnPointerUp (PointerEventData eventData) {
			if (MyButton.enabled && MyButton.interactable) {
				transform.localScale = OriginalScale;
			}
		}
	}
}
