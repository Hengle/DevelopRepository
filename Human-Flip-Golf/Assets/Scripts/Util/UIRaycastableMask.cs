using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Util {
	public class UIRaycastableMask : MonoBehaviour, ICanvasRaycastFilter {
		[SerializeField] RectTransform raycastableArea;

		public void SetAreaPosition (Vector2 position) {
			raycastableArea.position = position;
		}

		public void SetAreaSize (Vector2 sizeDelta) {
			raycastableArea.sizeDelta = sizeDelta;
		}

		public bool IsRaycastLocationValid (Vector2 sp, Camera eventCamera) {
			return !RectTransformUtility.RectangleContainsScreenPoint(raycastableArea, sp, eventCamera);
		}
	}
}
