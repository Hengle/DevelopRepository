using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VisualEffects {
	public class SampleTwinkle : MonoBehaviour {
		[SerializeField] Twinkle twinkle;
		[SerializeField] float interval = 1.0f;
		[SerializeField] float duration = 1.0f;

		IEnumerator Start () {
			while (true) {
				bool isAnimating = true;
				twinkle.Play(true, duration, interval, () => isAnimating = false);

				yield return new WaitWhile(() => isAnimating);
			}
		}
	}
}
