using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Util;

namespace MainGame.Tutorial {
	public class Navigator : MonoBehaviour {
		[SerializeField] SpriteRenderer spHand;
		[SerializeField] MultipleParticles efHold;

		Sequence DragTween { get; set; }


		void Awake () {
			spHand.color = Statics.AlphaZero;
		}

		void OnDestroy () {
			DragTween?.Kill();
			DragTween = null;
		}

		public void PlayAnimation () {
			DragTween = DOTween.Sequence();
			DragTween.AppendInterval(0.2f);
			DragTween.Append(spHand.DOFade(1, 0.3f));
			DragTween.AppendCallback(efHold.Play);
			DragTween.Append(transform.DOMoveY(1.75f, 1.0f).SetRelative().SetEase(Ease.InOutQuad));
			DragTween.AppendInterval(0.5f);
			DragTween.Append(spHand.DOFade(0, 0.3f));
			DragTween.AppendCallback(efHold.Stop);
			DragTween.SetLoops(-1, LoopType.Restart);
		}
	}
}
