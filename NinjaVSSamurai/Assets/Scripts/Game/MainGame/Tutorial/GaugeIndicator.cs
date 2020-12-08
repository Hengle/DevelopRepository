using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace MainGame.Tutorial {
	public class GaugeIndicator : MonoBehaviour {
		[SerializeField] GameObject objZones;
		[SerializeField] GameObject objIcons;

		Sequence ReleaseTween { get; set; }


		void Awake () {
			objZones.SetActive(false);
			objIcons.SetActive(false);
		}

		void OnDestroy () {
			ReleaseTween?.Kill();
			ReleaseTween = null;
		}


		public void ActivateObjects () {
			objZones.SetActive(true);
			objIcons.SetActive(true);
		}

		public void PlayReleaseAnimation () {
			ReleaseTween = DOTween.Sequence();
			ReleaseTween.Append(objIcons.transform.DOLocalMoveY(0.05f, 0.25f).SetRelative().SetEase(Ease.Linear));
			ReleaseTween.Append(objIcons.transform.DOLocalMoveY(-1.0f, 0.5f).SetRelative().SetEase(Ease.OutExpo));
			ReleaseTween.Append(objIcons.transform.DOLocalMoveY(1.0f, 0.5f).SetRelative().SetEase(Ease.InExpo));
			ReleaseTween.SetLoops(-1, LoopType.Restart);
		}

	}
}
