using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace MainGame {
	[RequireComponent(typeof(CanvasGroup))]
	public class Result : MonoBehaviour {
		[SerializeField] Image imgTitle;
		[SerializeField] Sprite spTitleWin;
		[SerializeField] Sprite spTitleLose;
		[SerializeField] List<Util.SceneChangeButton> btnToNextScene = new List<Util.SceneChangeButton>();
		[SerializeField] GameObject coin1;
		[SerializeField] GameObject coin2;

		CanvasGroup MyCanvasGroup { get; set; }


		void Awake () {
			MyCanvasGroup = GetComponent<CanvasGroup>();
			MyCanvasGroup.alpha = 0;
			MyCanvasGroup.interactable = false;
			MyCanvasGroup.blocksRaycasts = false;

			if (!btnToNextScene.IsNullOrEmpty()) {
				foreach (var btn in btnToNextScene) {
					btn.OnClickListener += () => AppLovinAdManager.Instance.ShowInterstitial();
				}
			}
		}

		public void Open (bool isWin) {
			imgTitle.sprite = isWin ? spTitleWin : spTitleLose;

			MyCanvasGroup.blocksRaycasts = true;
			MyCanvasGroup.DOFade(1, 1.0f).OnComplete(() => {
				MyCanvasGroup.interactable = true;
			});

			if (coin1 != null && coin2 != null) {
				if (isWin) {
					coin1.SetActive(true);
					coin2.SetActive(false);
				} else {
					coin1.SetActive(false);
					coin2.SetActive(true);
				}
			}
		}

	}
}
