using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace Util {
	public class MultipleSpriteFader : MonoBehaviour {
		List<SpriteRenderer> SpRenderers { get; set; } = new List<SpriteRenderer>();


		void Awake () {
			foreach (var child in gameObject.GetComponentsInChildren<Transform>()) {
				var spRenderer = child.GetComponent<SpriteRenderer>();
				if (spRenderer != null) {
					SpRenderers.Add(spRenderer);
				}
			}
		}

		public void FadeTo (float alpha, float duration, Action onComplete = null, Ease ease = Ease.Linear) {
			foreach (var spRenderer in SpRenderers) {
				spRenderer.DOFade(alpha, duration).SetEase(ease);
			}
			StartCoroutine(FadeToRoutine(duration, onComplete));
		}

		IEnumerator FadeToRoutine (float duration, Action onComplete = null) {
			yield return new WaitForSeconds(duration);
			onComplete?.Invoke();
		}
	}
}
