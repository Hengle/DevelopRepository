using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using DG.Tweening;
using static UnityEngine.SceneManagement.SceneManager;

public class SceneManager : SingletonMonoBehaviour<SceneManager> {
    [SerializeField] Image imgCover;

	bool IsSceneChanging { get; set; }
	bool IsFading { get; set; }
	Sequence FadeSequence { get; set; }


	protected override void Awake () {
		base.Awake();
		if (isCreated) return;

		DontDestroyOnLoad(gameObject);

		FadeSequence = DOTween.Sequence();
		FadeSequence.Append(imgCover.DOFade(1, 0.35f).SetEase(Ease.Linear));
		FadeSequence.SetAutoKill(false);
		FadeSequence.Pause();
		FadeSequence.OnStepComplete(() => IsFading = !IsFading);
	}

	public void ChangeScene (string baseSceneName = "", float delay = 0.0f) {
		if (string.IsNullOrEmpty(baseSceneName)) {
			return;
		}

		StartCoroutine(ChangeSceneCoroutine(baseSceneName, delay));
	}

	IEnumerator ChangeSceneCoroutine (string baseSceneName, float delay = 0.0f) {
		if (IsSceneChanging) yield break;

		// フェードイン開始
		IsSceneChanging = true;

		if (delay > 0.0f) {
			yield return new WaitForSeconds(delay);
		}

		imgCover.raycastTarget = true;
		FadeSequence.PlayForward();

		// フェードイン待ち
		yield return new WaitWhile(() => !IsFading);

		Resources.UnloadUnusedAssets();

		// 基礎シーンロード
		if (!string.IsNullOrEmpty(baseSceneName)) {
			var async = LoadSceneAsync(baseSceneName, UnityEngine.SceneManagement.LoadSceneMode.Single);
			yield return new WaitWhile(() => !async.isDone);
		}

		// フェードアウト開始
		FadeSequence.PlayBackwards();

		// フェードアウト待ち
		yield return new WaitWhile(() => IsFading);
		imgCover.raycastTarget = false;

		IsSceneChanging = false;
	}
}
