using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class AspectNormalizer : MonoBehaviour {
	[SerializeField] float width = 720.0f;
	[SerializeField] float height = 1280.0f;
	[SerializeField] bool isHeightFit = true;

	void Awake () {
		var myCamera = GetComponent<Camera>();
		float targetAspect = height / width;
		float deviceAspect = (float)Screen.height / Screen.width;
		float scale = deviceAspect / targetAspect;

		if (isHeightFit) {
			myCamera.orthographicSize = height / 2 / 100;
		} else {
			myCamera.orthographicSize = height / 2 / 100 * scale;
		}
	}
}
