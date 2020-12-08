using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Util;

namespace MainGame {
	public class DeadlyGauge : MonoBehaviour {
		[SerializeField] float gaugePercent = 0.15f;
		[SerializeField] Transform tfPointZero;
		[SerializeField] Transform tfPointOne;
		[SerializeField] Transform tfIndicator;
		[SerializeField] SpriteRenderer spGauge;
		[SerializeField] MultipleSpriteFader spriteFader;
		[SerializeField] TouchEventHandler touchEventHandler;

		Tween TweenIndicator { get; set; }
		Vector2 DefaultGaugeSize { get; set; }
		bool IsStoped { get; set; }
		Action<bool> OnAnimationEndListener { get; set; } // isSuccess


		void Awake () {
			tfIndicator.position = tfIndicator.position.SetX(tfPointZero.position.x);
			DefaultGaugeSize = spGauge.size;
		}

		void Start () {
			AudioManager.Instance.LoadSe(AudioManager.SeName.Drum);
			spriteFader.FadeTo(0, 0);
		}

		void OnDestroy () {
			touchEventHandler.OnTouchStartListener -= OnTouch;
			TweenIndicator?.Kill();
			TweenIndicator = null;
		}

		public void SetGaugePercent (float percent) {
			gaugePercent = percent;
		}

		public void PlayAnimation (Action<bool> onAnimationEnd = null) {
			ChangeSuccessAreaSize(gaugePercent);
			spriteFader.FadeTo(1, 0.5f, StartTween);

			OnAnimationEndListener = onAnimationEnd;
		}

		public void ForceStopIndicator (bool isRandom = true) {
			IsStoped = true;

			if (isRandom) {
				DOVirtual.DelayedCall(UnityEngine.Random.Range(1.75f, 3.0f), () => OnTouch(Vector2.zero));
			} else {
				OnTouch(Vector2.zero);
			}
		}

		void StartTween () {
			TweenIndicator = tfIndicator.DOMoveX(tfPointOne.position.x, 0.75f).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.Linear);

			if (!IsStoped) {
				touchEventHandler.OnTouchStartListener += OnTouch;
			}
		}

		void ChangeSuccessAreaSize (float percent) {
			spGauge.size = new Vector2(DefaultGaugeSize.x * percent, DefaultGaugeSize.y);
		}

		void OnTouch (Vector2 touchPos) {
			touchEventHandler.OnTouchStartListener -= OnTouch;
			TweenIndicator.Pause();

			// 判定ゲージ範囲内かチェック
			bool isSuccess = tfIndicator.position.x >= spGauge.bounds.min.x && tfIndicator.position.x <= spGauge.bounds.max.x;
			StartCoroutine(EndAnimationRoutine(isSuccess));

			if (isSuccess) {
				AudioManager.Instance.PlaySe(AudioManager.SeName.Drum);
			}
		}

		IEnumerator EndAnimationRoutine (bool isSuccess) {
			yield return Statics.WaitQuarterSeconds;
			IsStoped = true;
			spriteFader.FadeTo(0, 0.25f, () => OnAnimationEndListener?.Invoke(isSuccess));
		}

	}
}
