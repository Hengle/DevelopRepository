using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace Util {
	public class StretchBoxCollider2D : MonoBehaviour {
		[SerializeField] SpriteRenderer spArea;
		[SerializeField] BoxCollider2D boxCollider;
		[SerializeField] float destinationLength = 10.0f;
		[SerializeField] float duration = 3.0f;
		[SerializeField] AnimationCurve ease = AnimationCurve.Linear(0, 0, 1, 1);
		[SerializeField] bool isAutoStart;

		Tween StretchAreaTween { get; set; }


		void Awake () {
			float length = 0;
			StretchAreaTween = DOTween.To(() => length, x => length = x, destinationLength, duration).SetEase(ease).OnUpdate(() => {
				var size = new Vector2(spArea.size.x, length);
				spArea.size = size;
				boxCollider.size = size;
				boxCollider.offset = new Vector2(0, -length + (length / 2));
			});
			StretchAreaTween.Pause();
		}

		void Start () {
			if (isAutoStart) {
				StretchAreaTween.Play();
			}
		}

		void OnDestroy () {
			Stop();
		}

		public void Play () {
			StretchAreaTween.Play();
		}

		public void Stop () {
			StretchAreaTween?.Kill();
		}
	}
}
