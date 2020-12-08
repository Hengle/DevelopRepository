using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace MainGame {
	public class AttackGauge : MonoBehaviour {
		[SerializeField] Image imgGauge;

		public float FillAmount { get { return imgGauge.fillAmount; } }
		Tween FillTween { get; set; }


		void Awake () {
			imgGauge.fillAmount = 0;

			FillTween = imgGauge.DOFillAmount(1, 0.75f).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.Linear).SetAutoKill(false).Pause();
		}

		void OnDestroy () {
			if (FillTween != null) {
				FillTween.Kill();
				FillTween = null;
			}
		}

		public void ChangeTweenValues (float endValue, float duration) {
			FillTween = imgGauge.DOFillAmount(endValue, duration).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.Linear).SetAutoKill(false).Pause();
		}

		public void PlayFillTween () {
			FillTween.Restart();
		}

		public void PauseFillTween () {
			FillTween.Pause();
		}

		public void ResetAmount () {
			imgGauge.fillAmount = 0;
		}

	}
}
