using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace MainGame {
	[RequireComponent(typeof(SpriteRenderer))]
	public class FinishAnnounce : MonoBehaviour {
		Material MyMaterial { get; set; }


		void Awake () {
			MyMaterial = GetComponent<SpriteRenderer>().material;
			MyMaterial.SetColor("_Color", Statics.AlphaZero);
		}

		public void PlayFadeAnimation (float alpha, float duration, float delay = 0.0f, Action onComplete = null) {
			float fadeTime = 0;
			DOTween.To(() => fadeTime, v => fadeTime = v, alpha, duration).OnUpdate(() => {
				MyMaterial.SetColor("_Color", new Color(1, 1, 1, fadeTime));
			})
			.SetDelay(delay)
			.OnComplete(() => onComplete?.Invoke());
		}

		public void PlayDissolveAnimation (float targetThreshold, float duration, float delay = 0.0f, Action onComplete = null) {
			float threshold = 0;
			DOTween.To(() => threshold, v => threshold = v, targetThreshold, duration).OnUpdate(() => {
				MyMaterial.SetFloat("_Threshold", threshold);
			})
			.SetDelay(delay)
			.OnComplete(() => onComplete?.Invoke());
		}

	}
}
