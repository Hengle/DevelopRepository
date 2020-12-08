using System;
using UnityEngine;
using DG.Tweening;

namespace Util {
	[RequireComponent(typeof(CanvasGroup))]
	public class CanvasGroupHandler : MonoBehaviour {
		CanvasGroup MyCanvasGroup { get; set; }


		void Awake () {
			MyCanvasGroup = GetComponent<CanvasGroup>();
		}

		public void Open (float duration, Action onComplete = null) {
			MyCanvasGroup.DOFade(1.0f, duration).SetEase(Ease.Linear).OnComplete(() => {
				MyCanvasGroup.interactable = true;
				MyCanvasGroup.blocksRaycasts = true;
				onComplete?.Invoke();
			});
		}

		public void Close (float duration, Action onComplete = null) {
			MyCanvasGroup.DOFade(0.0f, duration).SetEase(Ease.Linear).OnComplete(() => {
				MyCanvasGroup.interactable = false;
				MyCanvasGroup.blocksRaycasts = false;
				onComplete?.Invoke();
			});
		}
	}
}
