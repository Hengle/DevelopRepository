using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace VisualEffects {
	public abstract class VisualEffectBase : MonoBehaviour {
		protected Material MyMaterial { get; set; }

		public virtual void SetMaterial (Material mat) {
			MyMaterial = mat;
		}

		protected virtual void SetValue (string paramName, float from, float to, float duration, float delay = 0.0f, Action onComplete = null) {
			DOTween.To(() => from, val => from = val, to, duration).OnUpdate(() => {
				MyMaterial.SetFloat(paramName, from);
			})
			.SetDelay(delay)
			.OnComplete(() => onComplete?.Invoke());
		}
	}
}
