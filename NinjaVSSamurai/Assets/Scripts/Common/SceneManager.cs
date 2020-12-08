using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using DG.Tweening;

public class SceneManager : SingletonMonoBehaviour<SceneManager> {
    [SerializeField] Image imgCover;

	// 追加するときは必ず末尾に. でないとPrefabでの参照がズレる
	public enum SceneNames {
		MainGame,
		Tutorial,
		Title,
	}

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

	public void ChangeScene (SceneNames type) {
		UnityEngine.SceneManagement.SceneManager.LoadScene(type.Name());
	}

	public void ChangeSceneFade (SceneNames type, float delay = 0.0f) {
		StartCoroutine(ChangeSceneFadeCoroutine(type.Name(), delay));
    }

    IEnumerator ChangeSceneFadeCoroutine(string nextSceneName, float delay = 0.0f) {
    	if (IsSceneChanging) yield break;

		// フェードイン開始
		IsSceneChanging = true;

		if (delay > 0.0f) {
			yield return new WaitForSeconds(delay);
		}

		imgCover.raycastTarget = true;
		FadeSequence.PlayForward();

		// シーンロード開始
		Resources.UnloadUnusedAssets();
		var async = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(nextSceneName, UnityEngine.SceneManagement.LoadSceneMode.Single);
		async.allowSceneActivation = false;

		// フェードイン待ち
		yield return new WaitWhile(() => !IsFading);

		// シーンロード待ち
		async.allowSceneActivation = true;
		yield return new WaitWhile(() => !async.isDone);

		// フェードアウト開始
		FadeSequence.PlayBackwards();

		// フェードアウト待ち
		yield return new WaitWhile(() => IsFading);
		imgCover.raycastTarget = false;

		IsSceneChanging = false;
    }

	string ConvertFromEnumToString (SceneNames type) {
		switch (type) {
			case SceneNames.MainGame:
				return "MainGame";
			default:
				Debug.LogError("指定タイプは未定義:" + type);
				return "";
		}
	}
}
