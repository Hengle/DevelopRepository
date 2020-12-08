using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VisualEffects {
	public class MaterialProvider : MonoBehaviour {
		[SerializeField] Material baseMaterial;


		void Awake () {
			var spRenderer = GetComponent<SpriteRenderer>();
			var image = GetComponent<UnityEngine.UI.Image>();

			var myMaterial = new Material(baseMaterial);

			if (spRenderer != null) {
				spRenderer.material = myMaterial;
			} else if (image != null) {
				image.material = myMaterial;
			} else {
				Debug.LogError("SpriteRendererもしくはImageコンポーネントが必要です");
			}

			GetComponent<VisualEffectBase>().SetMaterial(myMaterial);
		}
	}
}
