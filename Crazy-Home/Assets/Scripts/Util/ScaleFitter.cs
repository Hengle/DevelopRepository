using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Util {
	[RequireComponent(typeof(Collider2D))]
	public class ScaleFitter : MonoBehaviour {
		[SerializeField] Camera targetCamera;

		void Start () {
			if (targetCamera == null) targetCamera = Camera.main;

			var cameraWidth = targetCamera.aspect * targetCamera.orthographicSize * 2;
			var cameraHeight = targetCamera.orthographicSize * 2;
			transform.localScale = new Vector3(cameraWidth, cameraHeight, 1);
		}
	}
}
