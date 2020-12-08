using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace MainGame {
	public class HitpointGauge : MonoBehaviour {
		[SerializeField] Image imgGauge;

		void Awake () {
			imgGauge.fillAmount = 1;
		}

		public void PlayFillTween (int nowHp, int maxHp) {
			imgGauge.DOFillAmount((float)nowHp / (float)maxHp, 1.0f).SetEase(Ease.Linear);
		}

	}
}
