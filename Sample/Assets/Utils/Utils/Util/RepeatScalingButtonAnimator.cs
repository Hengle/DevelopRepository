using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DG.Tweening;

namespace Util {
	[RequireComponent(typeof(Button))]
	public class RepeatScalingButtonAnimator : MonoBehaviour, IPointerDownHandler, IPointerUpHandler {
		[SerializeField] AnimationCurve ease;
		[SerializeField] LoopType loopType = LoopType.Yoyo;
		[SerializeField] float time = 0.4f;
		[SerializeField] float scale = 1.05f;
		[SerializeField] bool isFadeOut;
		[SerializeField] bool isAutoStart;

		Button MyButton { get; set; }
		Vector3 OriginalScale { get; set; }
		Sequence ScaleAnimation { get; set; }


		void Awake () {
			MyButton = GetComponent<Button>();
			OriginalScale = transform.localScale;

			ScaleAnimation = DOTween.Sequence();
			ScaleAnimation.Append(transform.DOScale(scale, time).SetEase(ease));
			if (isFadeOut) {
				ScaleAnimation.Join(MyButton.image.DOFade(0, time).SetEase(ease));
			}
			ScaleAnimation.SetLoops(-1, loopType);
			ScaleAnimation.Pause();
		}

		void Start () {
			if (isAutoStart) Play();
		}

		public void Play () {
			ScaleAnimation.Play();
		}

		public void Stop () {
			ScaleAnimation.Kill();
		}

		public void OnPointerDown (PointerEventData eventData) {
			if (MyButton.enabled && MyButton.interactable) {
				ScaleAnimation.Pause();
				transform.localScale = OriginalScale * 0.9f;
			}
		}

		public void OnPointerUp (PointerEventData eventData) {
			if (MyButton.enabled && MyButton.interactable) {
				transform.localScale = OriginalScale;
				ScaleAnimation.Restart();
			}
		}
	}
}
