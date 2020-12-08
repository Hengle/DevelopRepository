using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Util {
	[ExecuteInEditMode]
	[RequireComponent(typeof(SpriteRenderer))]
	public class ScreenSpriteFitter : MonoBehaviour {
		[SerializeField] Camera targetCamera;
        [SerializeField] type EffectType;

        enum type { ALL,WIDTH,HEIGHT };

		void Start () {
			if (targetCamera == null) targetCamera = Camera.main;

			var spRenderer = GetComponent<SpriteRenderer>();
			var spWidth = spRenderer.sprite.bounds.size.x * spRenderer.sprite.pixelsPerUnit;
			var spHeight = spRenderer.sprite.bounds.size.y * spRenderer.sprite.pixelsPerUnit;

			var cameraWidth = targetCamera.aspect * targetCamera.orthographicSize * 2 * 100;
			var cameraHeight = targetCamera.orthographicSize * 2 * 100;

            if(EffectType == type.ALL) {
                transform.localScale = new Vector3(cameraWidth / spWidth, cameraHeight / spHeight, transform.localScale.z);
            }else if(EffectType == type.HEIGHT) {
                transform.localScale = new Vector3(transform.localScale.x, cameraHeight / spHeight, transform.localScale.z);
            } else if (EffectType == type.WIDTH) {
                transform.localScale = new Vector3(cameraWidth / spWidth, transform.localScale.y, transform.localScale.z);
            }
        }
	}
}
