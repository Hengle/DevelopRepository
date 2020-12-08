using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShojiBlood : MonoBehaviour {
	[SerializeField] List<Image> imgBloods = new List<Image>();
	[SerializeField] float firstWait = 1.0f;
	[SerializeField] float afterWait = 1.0f;
	[SerializeField] float interval = 0.05f;


	void Awake () {
		DisabledImages();
	}

	public void DisabledImages () {
		foreach (var img in imgBloods) {
			img.enabled = false;
		}
	}

	public void PlayAnimation (Action onComplete = null) {
		StartCoroutine(PlayAnimationRoutine(onComplete));
	}

	IEnumerator PlayAnimationRoutine (Action onComplete = null) {
		yield return new WaitForSeconds(firstWait);

		foreach (var imgBlood in imgBloods) {
			imgBlood.enabled = true;
			yield return new WaitForSeconds(interval);
			//yield return new WaitForSeconds(UnityEngine.Random.Range(0.15f, 0.3f));
		}

		yield return new WaitForSeconds(afterWait);

		onComplete?.Invoke();
	}

}
