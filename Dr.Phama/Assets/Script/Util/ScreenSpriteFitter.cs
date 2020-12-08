using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Util {
	[ExecuteInEditMode]
	[RequireComponent(typeof(SpriteRenderer))]
	public class ScreenSpriteFitter : MonoBehaviour {
		[SerializeField] Camera targetCamera;

		void Start () {
			if (targetCamera == null) targetCamera = Camera.main;

			var spRenderer = GetComponent<SpriteRenderer>();
			var spWidth = spRenderer.sprite.bounds.size.x * spRenderer.sprite.pixelsPerUnit;
			var spHeight = spRenderer.sprite.bounds.size.y * spRenderer.sprite.pixelsPerUnit;

			var cameraWidth = targetCamera.aspect * targetCamera.orthographicSize * 2 * 100;
			var cameraHeight = targetCamera.orthographicSize * 2 * 100;

			transform.localScale = new Vector3(cameraWidth / spWidth, cameraHeight / spHeight, transform.localScale.z);
		}
	}
}
