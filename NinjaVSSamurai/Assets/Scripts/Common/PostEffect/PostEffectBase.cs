using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PostEffect {
	[RequireComponent(typeof(Camera))]
	public class PostEffectBase : MonoBehaviour {
		[SerializeField] Material baseMaterial;

		protected Material MyMaterial { get; set; }


		protected virtual void Awake () {
			MyMaterial = new Material(baseMaterial);
		}

		protected virtual void OnRenderImage (RenderTexture source, RenderTexture destination) {
			Graphics.Blit(source, destination, MyMaterial);
		}
	}
}
