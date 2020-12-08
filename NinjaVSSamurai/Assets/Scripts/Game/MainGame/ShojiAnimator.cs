using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace MainGame {
	[RequireComponent(typeof(CanvasGroup))]
	public class ShojiAnimator : MonoBehaviour {
		[SerializeField] HorizontalLayoutGroup hLayoutGroup;
		[SerializeField] Image imgStartOver;
		[SerializeField] List<Image> imgShojies = new List<Image>();
		[SerializeField] ShojiBlood shojiBlood1;
		[SerializeField] ShojiBlood shojiBlood2;
		[SerializeField] DeadlyTrailEffect deadlyTrailRight;
		[SerializeField] DeadlyTrailEffect deadlyTrailLeft;
		[SerializeField] SpriteRenderer deadlyBackground;

		CanvasGroup MyCanvasGroup { get; set; }


		void Awake () {
			MyCanvasGroup = GetComponent<CanvasGroup>();
			MyCanvasGroup.alpha = 0;
			MyCanvasGroup.interactable = false;
			MyCanvasGroup.blocksRaycasts = false;
			imgStartOver.enabled = false;

			hLayoutGroup.spacing = 1800;
		}

		void Start () {
			AudioManager.Instance.LoadSe(AudioManager.SeName.Shoji);
		}

		public void OpenStartOver (Action onAnimationEnd = null) {
			MyCanvasGroup.blocksRaycasts = true;
			MyCanvasGroup.interactable = true;
			imgStartOver.enabled = false;

			float space = 0;
			DOTween.To(() => space, s => space = s, 1800, 0.25f).OnUpdate(() => {
				hLayoutGroup.spacing = space;
			})
			.SetEase(Ease.Linear)
			.OnComplete(() => {
				MyCanvasGroup.blocksRaycasts = false;
				MyCanvasGroup.interactable = false;
				MyCanvasGroup.alpha = 0;
				onAnimationEnd?.Invoke();
			});
		}

		public void CloseStartOver (Action onAnimationEnd = null) {
			MyCanvasGroup.blocksRaycasts = true;
			MyCanvasGroup.interactable = true;
			MyCanvasGroup.alpha = 1;

			float space = 1800;
			DOTween.To(() => space, s => space = s, 0, 0.25f).OnUpdate(() => {
				hLayoutGroup.spacing = space;
			})
			.SetEase(Ease.Linear)
			.OnComplete(() => {
				AudioManager.Instance.PlaySe(AudioManager.SeName.Shoji);
				imgStartOver.enabled = true;
				onAnimationEnd?.Invoke();
			});
		}

		public void OpenBlood (Action onAnimationEnd = null) {
			foreach (var imgShoji in imgShojies) {
				imgShoji.color = new Color32(255, 255, 255, 109);
			}

			MyCanvasGroup.blocksRaycasts = true;
			MyCanvasGroup.interactable = true;
			shojiBlood1.DisabledImages();
			shojiBlood2.DisabledImages();

			deadlyTrailRight.gameObject.SetActive(false);
			deadlyTrailLeft.gameObject.SetActive(false);
			deadlyBackground.enabled = false;

			float space = 0;
			DOTween.To(() => space, s => space = s, 1800, 0.25f).OnUpdate(() => {
				hLayoutGroup.spacing = space;
			})
			.SetEase(Ease.Linear)
			.OnComplete(() => {
				MyCanvasGroup.blocksRaycasts = false;
				MyCanvasGroup.interactable = false;
				MyCanvasGroup.alpha = 0;
				onAnimationEnd?.Invoke();
			});
		}

		public void CloseBlood (Action onAnimationEnd = null) {
			MyCanvasGroup.blocksRaycasts = true;
			MyCanvasGroup.interactable = true;
			MyCanvasGroup.alpha = 1;

			float space = 1800;
			DOTween.To(() => space, s => space = s, 0, 0.25f).OnUpdate(() => {
				hLayoutGroup.spacing = space;
			})
			.SetEase(Ease.Linear)
			.OnComplete(() => {
				AudioManager.Instance.PlaySe(AudioManager.SeName.Shoji);

				foreach (var imgShoji in imgShojies) {
					imgShoji.color = new Color32(255, 255, 255, 109);
				}

				deadlyTrailRight.Play();
				deadlyTrailLeft.Play();
				deadlyBackground.enabled = true;
				shojiBlood1.PlayAnimation();
				shojiBlood2.PlayAnimation(() => onAnimationEnd?.Invoke());
			});
		}
	}
}
